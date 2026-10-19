// Ref: workflow.md §4 Architecture Patterns | Feature: Events
"use client";

import { useParams } from "next/navigation";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { getApiErrorMessage } from "@/lib/apiError";
import Link from "next/link";
import { Suspense, useState } from "react";
import { eventService } from "@/services/eventService";
import { queryKeys } from "@/services/queryKeys";
import { useAuthStore } from "@/stores/authStore";
import Pagination from "@/components/common/Pagination";
import Button from "@/components/common/Button";
import { ApiError } from "@/types/api";

export default function EventDetailPage() {
  return (
    <Suspense fallback={<div className="py-12 text-center text-gray-500">Loading event...</div>}>
      <EventDetailInner />
    </Suspense>
  );
}

function EventDetailInner() {
  const { id } = useParams<{ id: string }>();
  const { user } = useAuthStore();
  const queryClient = useQueryClient();
  const [feedback, setFeedback] = useState<{ message: string; type: "success" | "error" } | null>(null);
  const [reservationsPage, setReservationsPage] = useState(1);
  const [showCancelConfirm, setShowCancelConfirm] = useState(false);

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

  // Reservation details (visible to event owner)
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
      setFeedback({ message: "You have successfully reserved your spot for this event!", type: "success" });
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: queryKeys.events.myReservations() }),
        queryClient.invalidateQueries({ queryKey: queryKeys.events.reservations(id) }),
      ]);
    },
    onError: (err) => {
      setFeedback({ message: getApiErrorMessage(err, "Failed to join event."), type: "error" });
    },
  });

  const cancelMutation = useMutation({
    mutationFn: () => eventService.cancelReservation(id),
    onSuccess: async () => {
      setFeedback({ message: "Your reservation has been cancelled.", type: "success" });
      setShowCancelConfirm(false);
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: queryKeys.events.myReservations() }),
        queryClient.invalidateQueries({ queryKey: queryKeys.events.reservations(id) }),
      ]);
    },
    onError: (err) => {
      setFeedback({ message: getApiErrorMessage(err, "Failed to cancel reservation."), type: "error" });
    },
  });

  if (status === "pending") {
    return (
      <div className="py-12 max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 animate-pulse space-y-4">
        <div className="h-6 bg-gray-200 rounded w-1/3" />
        <div className="h-8 bg-gray-200 rounded w-2/3" />
        <div className="h-32 bg-gray-100 rounded" />
      </div>
    );
  }

  if (status === "error" || !event) {
    return (
      <div className="min-h-[calc(100vh-160px)] flex flex-col items-center justify-center py-12">
        <p className="text-gray-600 mb-4" role="alert">
          {getApiErrorMessage(error, "Event not found.")}
        </p>
        <div className="flex gap-3">
          <Link href="/events">
            <Button variant="outline">Back to Events</Button>
          </Link>
          <Button variant="primary" onClick={() => void refetch()}>
            Try Again
          </Button>
        </div>
      </div>
    );
  }

  const reservations = reservationsData?.items ?? [];
  const totalReservationPages = reservationsData?.totalPages ?? 1;

  return (
    <div className="min-h-[calc(100vh-160px)] py-12">
      <div className="mx-auto max-w-4xl px-4 sm:px-6 lg:px-8 space-y-8">
        <div className="flex items-center gap-2 text-sm text-gray-500">
          <Link href="/events" className="hover:text-gray-900">Events</Link>
          <span>/</span>
          <span className="text-gray-900 font-medium">{event.title}</span>
        </div>

        {feedback && (
          <div
            className={`p-4 rounded-md text-sm flex justify-between items-center ${
              feedback.type === "success" ? "bg-green-50 text-green-800" : "bg-red-50 text-red-800"
            }`}
            role="status"
          >
            <span>{feedback.message}</span>
            <button
              type="button"
              onClick={() => setFeedback(null)}
              className="font-medium underline ml-4"
            >
              Dismiss
            </button>
          </div>
        )}

        {/* Event Detail Card */}
        <div className="bg-white border border-gray-200 shadow-sm rounded-lg p-6 sm:p-8">
          <div className="flex flex-col sm:flex-row sm:items-start sm:justify-between gap-4 mb-6">
            <div>
              <div className="flex items-center gap-2 mb-2">
                <span className="text-xs px-2.5 py-0.5 rounded-full bg-blue-50 text-blue-700 font-medium">
                  {event.category}
                </span>
                <span className="text-xs text-gray-500">{event.visibility}</span>
              </div>
              <h1 className="text-2xl font-bold text-gray-900">{event.title}</h1>
              <p className="mt-1 text-sm text-gray-500">
                Organized by{" "}
                <Link
                  href={`/community/${event.communityId}`}
                  className="font-medium text-blue-600 hover:underline"
                >
                  {event.organizerCommunity.communityName}
                </Link>
              </p>
            </div>

            {/* Action buttons */}
            {user && (
              <div>
                {hasReserved ? (
                  <div className="flex items-center gap-2">
                    <span className="text-xs px-3 py-1.5 rounded-md bg-green-50 text-green-700 border border-green-200 font-medium">
                      ✓ Registered
                    </span>
                    <Button
                      variant="outline"
                      size="sm"
                      className="text-red-600 border-red-300 hover:bg-red-50"
                      onClick={() => setShowCancelConfirm(true)}
                    >
                      Cancel
                    </Button>
                  </div>
                ) : (
                  <Button
                    variant="primary"
                    onClick={() => joinMutation.mutate()}
                    disabled={joinMutation.isPending}
                  >
                    {joinMutation.isPending ? "Reserving..." : "Reserve Spot"}
                  </Button>
                )}
              </div>
            )}
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 py-4 border-y border-gray-100 text-sm">
            <div>
              <p className="text-gray-500 text-xs font-medium">WHEN</p>
              <p className="mt-1 text-gray-900 font-medium">
                {new Date(event.eventDate).toLocaleDateString()} at{" "}
                {new Date(event.eventDate).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })}
              </p>
              <p className="text-xs text-gray-500">
                Until {new Date(event.eventEndDate).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })}
              </p>
            </div>
            <div>
              <p className="text-gray-500 text-xs font-medium">WHERE</p>
              <p className="mt-1 text-gray-900 font-medium">{event.location}</p>
            </div>
          </div>

          <div className="mt-6">
            <h2 className="text-sm font-semibold text-gray-900 mb-2">About this event</h2>
            <p className="text-sm text-gray-700 whitespace-pre-line leading-relaxed">
              {event.description}
            </p>
          </div>
        </div>

        {/* Confirmation modal for cancellation */}
        {showCancelConfirm && (
          <div className="fixed inset-0 z-50 bg-black/40 flex items-center justify-center p-4">
            <div className="bg-white rounded-lg max-w-sm w-full p-6 space-y-4 shadow-xl">
              <h3 className="font-semibold text-gray-900 text-lg">Cancel Reservation?</h3>
              <p className="text-sm text-gray-600">
                Are you sure you want to cancel your reservation for <strong>{event.title}</strong>? Your spot will be freed up.
              </p>
              <div className="flex justify-end gap-3 pt-2">
                <Button variant="outline" onClick={() => setShowCancelConfirm(false)}>
                  Keep Reservation
                </Button>
                <Button
                  variant="primary"
                  className="bg-red-600 hover:bg-red-700 text-white"
                  disabled={cancelMutation.isPending}
                  onClick={() => cancelMutation.mutate()}
                >
                  {cancelMutation.isPending ? "Cancelling..." : "Confirm Cancel"}
                </Button>
              </div>
            </div>
          </div>
        )}

        {/* Organizer Attendee List */}
        {isOwner && (
          <div className="bg-white border border-gray-200 shadow-sm rounded-lg p-6 sm:p-8">
            <div className="flex items-center justify-between mb-4">
              <h2 className="text-lg font-semibold text-gray-900">
                Attendee Roster ({reservationsData?.totalCount ?? 0})
              </h2>
              <span className="text-xs bg-amber-50 text-amber-800 border border-amber-200 px-2 py-1 rounded">
                Organizer View
              </span>
            </div>

            {reservationsStatus === "pending" ? (
              <p className="text-sm text-gray-500 py-4">Loading attendees...</p>
            ) : reservationsError instanceof ApiError && reservationsError.status === 403 ? (
              <div className="p-4 bg-gray-50 rounded text-sm text-gray-600">
                You do not have permission to view the attendee list.
              </div>
            ) : reservations.length === 0 ? (
              <p className="text-sm text-gray-500 py-4">No reservations yet.</p>
            ) : (
              <div className="divide-y divide-gray-100">
                {reservations.map((attendee) => (
                  <div key={attendee.id} className="py-3 flex items-center justify-between text-sm">
                    <div>
                      <p className="font-medium text-gray-900">
                        {attendee.firstName} {attendee.lastName}
                      </p>
                      <p className="text-xs text-gray-500">{attendee.email}</p>
                    </div>
                    <span className="text-xs text-gray-400">
                      Reserved {new Date(attendee.reservedAt).toLocaleDateString()}
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
          </div>
        )}
      </div>
    </div>
  );
}
