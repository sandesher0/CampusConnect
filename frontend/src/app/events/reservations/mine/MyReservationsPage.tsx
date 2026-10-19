// Ref: workflow.md §4 Architecture Patterns | Feature: My Reservations
"use client";

import Link from "next/link";
import { Suspense, useState, useRef } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { eventService } from "@/services/eventService";
import { queryKeys } from "@/services/queryKeys";
import { usePaginatedQuery } from "@/hooks/usePaginatedQuery";
import { useAuthStore } from "@/stores/authStore";
import PageHeader from "@/components/common/PageHeader";
import Card from "@/components/common/Card";
import CategoryChip from "@/components/common/CategoryChip";
import StatusDot from "@/components/common/StatusDot";
import Skeleton from "@/components/common/Skeleton";
import EmptyState from "@/components/common/EmptyState";
import ErrorState from "@/components/common/ErrorState";
import Pagination from "@/components/common/Pagination";
import Button from "@/components/common/Button";
import ConfirmDialog from "@/components/common/ConfirmDialog";
import Toast from "@/components/common/Toast";
import { getApiErrorMessage } from "@/lib/apiError";
import type { MyEventReservationResponse } from "@/types/event";

export default function MyReservationsPage() {
  return (
    <Suspense
      fallback={
        <div className="mx-auto max-w-[1200px] px-6 py-12 sm:px-8 space-y-6">
          <Skeleton className="h-10 w-48 mb-8" />
          <div className="space-y-4">
            {[1, 2, 3].map((i) => (
              <Skeleton key={i} className="h-32 w-full" />
            ))}
          </div>
        </div>
      }
    >
      <MyReservationsContent />
    </Suspense>
  );
}

function MyReservationsContent() {
  const { user } = useAuthStore();
  const queryClient = useQueryClient();

  const [toast, setToast] = useState<{ message: string; type: "success" | "error" } | null>(null);
  const [cancellingEvent, setCancellingEvent] = useState<{ id: string; title: string } | null>(null);
  const cancelBtnRef = useRef<HTMLButtonElement>(null);

  const {
    items: reservations,
    totalPages,
    page,
    setPage,
    status,
    isFetching,
    error,
    refetch,
  } = usePaginatedQuery<MyEventReservationResponse>({
    queryKeyPrefix: queryKeys.events.myReservations(),
    queryFn: eventService.getMyReservations,
    defaultPageSize: 10,
    enabled: Boolean(user),
  });

  const cancelMutation = useMutation({
    mutationFn: (eventId: string) => eventService.cancelReservation(eventId),
    onSuccess: async () => {
      setToast({ message: "Reservation cancelled successfully.", type: "success" });
      setCancellingEvent(null);
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: queryKeys.events.myReservations() }),
        queryClient.invalidateQueries({ queryKey: queryKeys.events.all() }),
      ]);
    },
    onError: (err) => {
      setToast({ message: getApiErrorMessage(err, "Failed to cancel reservation."), type: "error" });
    },
  });

  if (!user) {
    return (
      <div className="mx-auto max-w-[1200px] px-6 py-16 sm:px-8">
        <EmptyState
          title="Sign in to view your reservations"
          description="Track upcoming events you've RSVP'd to and manage your campus schedule."
          action={
            <Button href="/login?redirect=/events/reservations/mine" variant="primary">
              Log in to your account
            </Button>
          }
        />
      </div>
    );
  }

  const now = new Date();

  return (
    <div className="mx-auto max-w-[1200px] px-6 py-12 sm:px-8">
      {toast && (
        <Toast
          message={toast.message}
          type={toast.type}
          onDismiss={() => setToast(null)}
        />
      )}

      <PageHeader
        eyebrow="SCHEDULE"
        title="My Reservations"
        description="Upcoming events and sessions you have registered for."
        action={
          <Button href="/events" variant="outline">
            Browse More Events
          </Button>
        }
      />

      {status === "pending" && (
        <div className="space-y-4">
          {[1, 2, 3].map((i) => (
            <Card key={i} className="p-6 space-y-3" hoverEffect={false}>
              <div className="flex justify-between">
                <Skeleton className="h-6 w-24 rounded-full" />
                <Skeleton className="h-4 w-32" />
              </div>
              <Skeleton className="h-6 w-1/2" />
              <Skeleton className="h-4 w-1/3" />
            </Card>
          ))}
        </div>
      )}

      {status === "error" && (
        <ErrorState
          error={error}
          onRetry={() => void refetch()}
          fallbackMessage="Unable to load your event reservations."
        />
      )}

      {status === "success" && reservations.length === 0 && (
        <EmptyState
          title="No reservations yet"
          description="You haven't RSVP'd to any campus events yet. Explore upcoming gatherings and claim a spot!"
          action={
            <Button href="/events" variant="primary">
              Browse Events
            </Button>
          }
        />
      )}

      {status === "success" && reservations.length > 0 && (
        <div className="space-y-4">
          {reservations.map((res) => {
            const isHappeningNow =
              new Date(res.eventDate) <= now && new Date(res.eventEndDate) >= now;

            return (
              <Card
                key={res.reservationId}
                className="p-6 flex flex-col sm:flex-row sm:items-center justify-between gap-6"
              >
                <div className="space-y-2">
                  <div className="flex flex-wrap items-center gap-2.5">
                    <CategoryChip category={res.category} />
                    {isHappeningNow ? (
                      <StatusDot status="good" label="Happening now" pulse />
                    ) : (
                      <span className="text-xs font-semibold text-muted">
                        Reserved on {new Date(res.reservedAt).toLocaleDateString()}
                      </span>
                    )}
                  </div>

                  <Link href={`/events/${res.eventId}`} className="group block focus-visible:outline-none">
                    <h3 className="text-xl font-[800] text-ink group-hover:text-primary transition-colors">
                      {res.title}
                    </h3>
                  </Link>

                  <div className="flex flex-wrap items-center gap-x-4 gap-y-1 text-xs font-semibold text-muted">
                    <span>
                      📅{" "}
                      {new Date(res.eventDate).toLocaleDateString("en-US", {
                        weekday: "short",
                        month: "short",
                        day: "numeric",
                      })}{" "}
                      at {new Date(res.eventDate).toLocaleTimeString([], { hour: "numeric", minute: "2-digit" })}
                    </span>
                    <span>📍 {res.location}</span>
                  </div>
                </div>

                <div className="flex items-center gap-3 shrink-0">
                  <Button href={`/events/${res.eventId}`} variant="outline" size="sm">
                    View
                  </Button>
                  <Button
                    ref={cancelBtnRef}
                    variant="outline"
                    size="sm"
                    className="text-status-warning-text hover:bg-coral-tint hover:border-coral-border"
                    onClick={() => setCancellingEvent({ id: res.eventId, title: res.title })}
                  >
                    Cancel Spot
                  </Button>
                </div>
              </Card>
            );
          })}

          <Pagination
            page={page}
            totalPages={totalPages}
            onPageChange={setPage}
            isLoading={isFetching}
          />
        </div>
      )}

      {/* Accessible Cancel Dialog */}
      <ConfirmDialog
        isOpen={Boolean(cancellingEvent)}
        onClose={() => setCancellingEvent(null)}
        onConfirm={() => {
          if (cancellingEvent) {
            cancelMutation.mutate(cancellingEvent.id);
          }
        }}
        title="Cancel Reservation?"
        description={`Are you sure you want to cancel your spot for "${cancellingEvent?.title}"?`}
        confirmLabel="Confirm Cancel"
        cancelLabel="Keep Reservation"
        isDestructive={true}
        isLoading={cancelMutation.isPending}
        triggerRef={cancelBtnRef}
      />
    </div>
  );
}
