// Ref: workflow.md §4 Architecture Patterns | Feature: Event Detail
"use client";

import { useParams } from "next/navigation";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { getApiErrorMessage } from "@/lib/apiError";
import Link from "next/link";
import { Suspense, useState, useRef } from "react";
import { eventService } from "@/services/eventService";
import { queryKeys } from "@/services/queryKeys";
import { useAuthStore } from "@/stores/authStore";
import Card from "@/components/common/Card";
import Button from "@/components/common/Button";
import CategoryChip from "@/components/common/CategoryChip";
import Avatar from "@/components/common/Avatar";
import StatusDot from "@/components/common/StatusDot";
import Skeleton from "@/components/common/Skeleton";
import ErrorState from "@/components/common/ErrorState";
import ConfirmDialog from "@/components/common/ConfirmDialog";
import Toast from "@/components/common/Toast";
import Pagination from "@/components/common/Pagination";
import type { EventReservationSummaryResponse } from "@/types/event";

export default function EventDetailPage() {
  return (
    <Suspense
      fallback={
        <div className="mx-auto max-w-[1200px] px-6 py-12 sm:px-8 space-y-6">
          <Skeleton className="h-8 w-48" />
          <div className="grid grid-cols-1 lg:grid-cols-12 gap-8">
            <Skeleton className="lg:col-span-8 h-96" />
            <Skeleton className="lg:col-span-4 h-64" />
          </div>
        </div>
      }
    >
      <EventDetailContent />
    </Suspense>
  );
}

function EventDetailContent() {
  const { id } = useParams<{ id: string }>();
  const { user } = useAuthStore();
  const queryClient = useQueryClient();

  const [toast, setToast] = useState<{ message: string; type: "success" | "error" } | null>(null);
  const [reservationsPage, setReservationsPage] = useState(1);
  const [showCancelConfirm, setShowCancelConfirm] = useState(false);
  const cancelBtnRef = useRef<HTMLButtonElement>(null);

  const {
    data: event,
    status,
    error,
    refetch,
  } = useQuery({
    queryKey: queryKeys.events.detail(id),
    queryFn: () => eventService.getEventById(id),
    enabled: !!id,
  });

  const isOwner = user ? event?.createdBy === user.userId || event?.createdBy === user.id : false;

  // Reservation details (visible to event organizer)
  const {
    data: reservationsData,
    status: reservationsStatus,
    isFetching: isFetchingReservations,
    error: reservationsError,
  } = useQuery({
    queryKey: queryKeys.events.reservations(id, { page: reservationsPage, pageSize: 20 }),
    queryFn: () => eventService.getEventReservations(id, { page: reservationsPage, pageSize: 20 }),
    enabled: Boolean(id && isOwner),
    retry: false,
  });

  // Check if current user already reserved
  const { data: myReservationsData } = useQuery({
    queryKey: queryKeys.events.myReservations({ page: 1, pageSize: 50 }),
    queryFn: () => eventService.getMyReservations({ page: 1, pageSize: 50 }),
    enabled: Boolean(user),
  });

  const hasReserved = myReservationsData?.items?.some((r) => r.eventId === id);

  const joinMutation = useMutation({
    mutationFn: () => eventService.joinEvent(id, event!.communityId),
    onSuccess: async () => {
      setToast({ message: "You're in! Your spot is saved.", type: "success" });
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: queryKeys.events.myReservations() }),
        queryClient.invalidateQueries({ queryKey: queryKeys.events.reservations(id) }),
      ]);
    },
    onError: (err) => {
      setToast({ message: getApiErrorMessage(err, "Failed to join event."), type: "error" });
    },
  });

  const cancelMutation = useMutation({
    mutationFn: () => eventService.cancelReservation(id),
    onSuccess: async () => {
      setToast({ message: "Reservation cancelled.", type: "success" });
      setShowCancelConfirm(false);
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: queryKeys.events.myReservations() }),
        queryClient.invalidateQueries({ queryKey: queryKeys.events.reservations(id) }),
      ]);
    },
    onError: (err) => {
      setToast({ message: getApiErrorMessage(err, "Failed to cancel reservation."), type: "error" });
    },
  });

  if (status === "pending") {
    return (
      <div className="mx-auto max-w-[1200px] px-6 py-12 sm:px-8 space-y-6">
        <Skeleton className="h-6 w-40" />
        <div className="grid grid-cols-1 lg:grid-cols-12 gap-8">
          <Skeleton className="lg:col-span-8 h-96" />
          <Skeleton className="lg:col-span-4 h-64" />
        </div>
      </div>
    );
  }

  if (status === "error" || !event) {
    return (
      <div className="mx-auto max-w-[1200px] px-6 py-16 sm:px-8">
        <ErrorState
          error={error}
          onRetry={() => void refetch()}
          fallbackMessage="The requested event could not be found."
        />
      </div>
    );
  }

  const now = new Date();
  const isHappeningNow =
    new Date(event.eventDate) <= now && new Date(event.eventEndDate) >= now;

  const reservations = reservationsData?.items ?? [];
  const totalReservationPages = reservationsData?.totalPages ?? 1;

  return (
    <div className="mx-auto max-w-[1200px] px-6 py-12 sm:px-8 space-y-8">
      {/* Toast Notification */}
      {toast && (
        <Toast
          message={toast.message}
          type={toast.type}
          onDismiss={() => setToast(null)}
        />
      )}

      {/* Breadcrumb */}
      <div className="flex items-center gap-2 text-sm font-semibold text-muted">
        <Link href="/events" className="hover:text-primary transition-colors">
          Events
        </Link>
        <span>/</span>
        <span className="text-ink truncate max-w-sm">{event.title}</span>
      </div>

      {/* Two-Column Responsive Layout */}
      <div className="grid grid-cols-1 lg:grid-cols-12 gap-8 items-start">
        {/* Left Column: Details Card */}
        <div className="lg:col-span-8 space-y-8">
          <Card className="p-6 sm:p-10" hoverEffect={false}>
            <div className="flex flex-wrap items-center justify-between gap-3 mb-4">
              <div className="flex items-center gap-2.5">
                <CategoryChip category={event.category} />
                <span className="text-xs font-bold text-muted bg-primary-tint/50 px-3 py-1 rounded-full">
                  {event.visibility}
                </span>
              </div>
              {isHappeningNow && (
                <StatusDot status="good" label="Happening now" pulse />
              )}
            </div>

            <h1 className="text-3xl sm:text-4xl font-[800] leading-tight text-ink">
              {event.title}
            </h1>

            <p className="mt-3 text-sm font-semibold text-muted">
              Organized by{" "}
              <Link
                href={`/community/${event.communityId}`}
                className="text-primary hover:underline font-bold"
              >
                {event.organizerCommunity.communityName}
              </Link>
            </p>

            {/* When & Where Grid */}
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 my-8 p-6 rounded-[18px] bg-page border border-border-card text-sm">
              <div>
                <p className="text-xs font-extrabold uppercase tracking-wider text-muted">WHEN</p>
                <p className="mt-1 font-bold text-ink">
                  {new Date(event.eventDate).toLocaleDateString("en-US", {
                    weekday: "long",
                    month: "short",
                    day: "numeric",
                  })}
                </p>
                <p className="text-xs text-muted font-medium mt-0.5">
                  {new Date(event.eventDate).toLocaleTimeString([], { hour: "numeric", minute: "2-digit" })} –{" "}
                  {new Date(event.eventEndDate).toLocaleTimeString([], { hour: "numeric", minute: "2-digit" })}
                </p>
              </div>

              <div>
                <p className="text-xs font-extrabold uppercase tracking-wider text-muted">WHERE</p>
                <p className="mt-1 font-bold text-ink">{event.location}</p>
              </div>
            </div>

            {/* Description */}
            <div>
              <h2 className="text-xl font-[800] text-ink mb-3">About this event</h2>
              <p className="text-base text-ink/80 leading-relaxed whitespace-pre-line">
                {event.description}
              </p>
            </div>
          </Card>

          {/* Organizer Attendees Section */}
          {isOwner && (
            <Card className="p-6 sm:p-8" hoverEffect={false}>
              <div className="flex items-center justify-between mb-6">
                <div>
                  <h2 className="text-xl font-[800] text-ink">
                    Attendees ({reservationsData?.totalCount ?? 0})
                  </h2>
                  <p className="text-xs text-muted font-medium mt-0.5">
                    Only you and fellow organizers can view attendee details.
                  </p>
                </div>
              </div>

              {reservationsStatus === "pending" ? (
                <div className="space-y-3">
                  {[1, 2, 3].map((i) => (
                    <Skeleton key={i} className="h-16 w-full rounded-2xl" />
                  ))}
                </div>
              ) : reservationsStatus === "error" ? (
                <ErrorState
                  error={reservationsError}
                  fallbackMessage="Unable to load attendee reservations."
                />
              ) : reservations.length === 0 ? (
                <p className="text-sm text-muted py-4">No attendees have reserved spots yet.</p>
              ) : (
                <div className="divide-y divide-border-divider">
                  {reservations.map((r: EventReservationSummaryResponse) => (
                    <div key={r.id} className="py-3.5 flex items-center justify-between">
                      <div className="flex items-center gap-3">
                        <Avatar name={r.firstName} size="sm" />
                        <div>
                          <p className="text-sm font-bold text-ink">
                            {r.firstName} {r.lastName}
                          </p>
                          <p className="text-xs text-muted font-medium">{r.email}</p>
                        </div>
                      </div>
                      <span className="text-xs font-semibold text-muted">
                        Reserved {new Date(r.reservedAt).toLocaleDateString()}
                      </span>
                    </div>
                  ))}
                </div>
              )}

              <Pagination
                page={reservationsPage}
                totalPages={totalReservationPages}
                onPageChange={setReservationsPage}
                isLoading={isFetchingReservations}
              />
            </Card>
          )}

          {/* Non-Organizer Friendly Indicator */}
          {!isOwner && user && (
            <div className="p-6 rounded-[22px] bg-primary-soft-tint/60 border border-border-card text-center">
              <p className="text-xs font-bold text-primary-on-tint">
                The full attendee list is only visible to the event organizer.
              </p>
            </div>
          )}
        </div>

        {/* Right Column: Sticky Action Card */}
        <div className="lg:col-span-4 sticky top-24">
          <Card className="p-6 sm:p-8 space-y-6" hoverEffect={false}>
            <div>
              <p className="text-xs font-extrabold uppercase tracking-wider text-primary">RESERVATION</p>
              <h3 className="text-xl font-[800] text-ink mt-1">Join this event</h3>
              <p className="text-xs text-muted font-medium mt-1">
                Free for all registered students and community members.
              </p>
            </div>

            {/* Action State Patterns */}
            {user ? (
              hasReserved ? (
                <div className="space-y-4">
                  <div className="flex items-center gap-2 p-3.5 rounded-[16px] bg-status-good-bg border border-border-card text-status-good-text">
                    <svg className="w-5 h-5 shrink-0" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
                      <polyline points="20 6 9 17 4 12" />
                    </svg>
                    <span className="text-xs font-bold">You have a spot reserved!</span>
                  </div>

                  <Button
                    ref={cancelBtnRef}
                    variant="outline"
                    className="w-full text-status-warning-text hover:bg-coral-tint hover:border-coral-border"
                    onClick={() => setShowCancelConfirm(true)}
                  >
                    Cancel Reservation
                  </Button>
                </div>
              ) : (
                <Button
                  variant="primary"
                  className="w-full"
                  size="lg"
                  disabled={joinMutation.isPending}
                  loading={joinMutation.isPending}
                  onClick={() => joinMutation.mutate()}
                  aria-busy={joinMutation.isPending}
                >
                  {joinMutation.isPending ? "Reserving Spot..." : "Reserve My Spot"}
                </Button>
              )
            ) : (
              <div className="space-y-3">
                <Button href={`/login?redirect=/events/${id}`} variant="primary" className="w-full">
                  Sign in to Reserve
                </Button>
                <p className="text-xs text-center text-muted font-medium">
                  Don&apos;t have an account?{" "}
                  <Link href="/register" className="text-primary font-bold hover:underline">
                    Sign up free
                  </Link>
                </p>
              </div>
            )}
          </Card>
        </div>
      </div>

      {/* Accessible Native Cancel Confirm Dialog */}
      <ConfirmDialog
        isOpen={showCancelConfirm}
        onClose={() => setShowCancelConfirm(false)}
        onConfirm={() => cancelMutation.mutate()}
        title="Cancel Reservation?"
        description={`Are you sure you want to cancel your reservation for "${event.title}"? Your spot will be freed up for fellow students.`}
        confirmLabel="Confirm Cancel"
        cancelLabel="Keep Reservation"
        isDestructive={true}
        isLoading={cancelMutation.isPending}
        triggerRef={cancelBtnRef}
      />
    </div>
  );
}
