// Ref: workflow.md §4 Architecture Patterns | Feature: Events
"use client";

import Link from "next/link";
import { Suspense, useState } from "react";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { eventService } from "@/services/eventService";
import { queryKeys } from "@/services/queryKeys";
import { getApiErrorMessage } from "@/lib/apiError";
import Pagination from "@/components/common/Pagination";
import { usePaginatedQuery } from "@/hooks/usePaginatedQuery";
import Button from "@/components/common/Button";
import { useAuthStore } from "@/stores/authStore";
import type { MyEventReservationResponse } from "@/types/event";

const CATEGORY_COLORS: Record<string, string> = {
  Academic: "bg-blue-50 text-blue-700",
  Social: "bg-purple-50 text-purple-700",
  Sports: "bg-green-50 text-green-700",
  Cultural: "bg-yellow-50 text-yellow-700",
  Career: "bg-orange-50 text-orange-700",
  Volunteering: "bg-teal-50 text-teal-700",
  Other: "bg-gray-100 text-gray-600",
};

export default function MyReservationsPage() {
  return (
    <Suspense fallback={<div className="min-h-[calc(100vh-160px)] flex items-center justify-center text-gray-500">Loading reservations...</div>}>
      <MyReservationsPageInner />
    </Suspense>
  );
}

function MyReservationsPageInner() {
  const { user } = useAuthStore();
  const queryClient = useQueryClient();
  const [cancellingEventId, setCancellingEventId] = useState<string | null>(null);
  const [cancellingEventTitle, setCancellingEventTitle] = useState<string | null>(null);
  const [cancelError, setCancelError] = useState<string | null>(null);

  const {
    items: reservations,
    page,
    totalPages,
    setPage,
    status,
    isFetching,
    error,
    refetch,
  } = usePaginatedQuery<MyEventReservationResponse>({
    queryKeyPrefix: queryKeys.events.myReservations(),
    queryFn: (params) => eventService.getMyReservations(params),
    defaultPageSize: 10,
    enabled: Boolean(user),
  });

  const cancelMutation = useMutation({
    mutationFn: (eventId: string) => eventService.cancelReservation(eventId),
    onSuccess: async () => {
      setCancellingEventId(null);
      setCancellingEventTitle(null);
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: queryKeys.events.myReservations() }),
        queryClient.invalidateQueries({ queryKey: queryKeys.events.all() }),
      ]);
    },
    onError: (err) => {
      setCancelError(getApiErrorMessage(err, "Failed to cancel reservation. Please try again."));
    },
  });

  if (!user) {
    return (
      <div className="min-h-[calc(100vh-160px)] flex flex-col items-center justify-center py-12">
        <p className="text-gray-500 mb-4">Please sign in to view your reservations.</p>
        <Link href="/login">
          <Button variant="primary">Sign In</Button>
        </Link>
      </div>
    );
  }

  return (
    <div className="min-h-[calc(100vh-160px)] py-12">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="mb-8">
          <div className="flex items-center gap-2 text-sm text-gray-500 mb-1">
            <Link href="/events" className="hover:text-gray-900">Events</Link>
            <span>/</span>
            <span className="text-gray-900 font-medium">My Reservations</span>
          </div>
          <h1 className="text-2xl font-bold text-gray-900">My Event Reservations</h1>
          <p className="mt-1 text-sm text-gray-500">
            View and manage your upcoming registered campus events
          </p>
        </div>

        {cancelError && (
          <div className="mb-6 p-4 bg-red-50 text-red-700 rounded-md text-sm" role="alert">
            {cancelError}
          </div>
        )}

        {/* Confirmation Modal */}
        {cancellingEventId && (
          <div className="fixed inset-0 z-50 bg-black/40 flex items-center justify-center p-4">
            <div className="bg-white rounded-lg max-w-sm w-full p-6 space-y-4 shadow-xl">
              <h3 className="font-semibold text-gray-900 text-lg">Cancel Reservation?</h3>
              <p className="text-sm text-gray-600">
                Are you sure you want to cancel your reservation for <strong>{cancellingEventTitle}</strong>?
              </p>
              <div className="flex justify-end gap-3 pt-2">
                <Button
                  variant="outline"
                  onClick={() => {
                    setCancellingEventId(null);
                    setCancellingEventTitle(null);
                  }}
                >
                  Keep Reservation
                </Button>
                <Button
                  variant="primary"
                  className="bg-red-600 hover:bg-red-700 text-white"
                  disabled={cancelMutation.isPending}
                  onClick={() => cancelMutation.mutate(cancellingEventId)}
                >
                  {cancelMutation.isPending ? "Cancelling..." : "Confirm Cancel"}
                </Button>
              </div>
            </div>
          </div>
        )}

        {status === "error" && (
          <div className="p-12 text-center bg-white border border-gray-200 rounded-lg">
            <p className="text-gray-600 mb-4">{getApiErrorMessage(error, "Failed to load reservations.")}</p>
            <Button variant="outline" onClick={() => void refetch()}>
              Retry
            </Button>
          </div>
        )}

        {status === "pending" && (
          <div className="space-y-4">
            {[...Array(3)].map((_, i) => (
              <div key={i} className="bg-white rounded-lg border border-gray-200 p-6 animate-pulse space-y-2">
                <div className="h-5 bg-gray-200 rounded w-1/3" />
                <div className="h-4 bg-gray-100 rounded w-1/4" />
              </div>
            ))}
          </div>
        )}

        {status === "success" && reservations.length === 0 && (
          <div className="bg-white rounded-lg border border-gray-200 p-12 text-center">
            <h3 className="text-base font-semibold text-gray-900">No active reservations</h3>
            <p className="mt-1 text-sm text-gray-500">You have not registered for any upcoming events.</p>
            <Link href="/events" className="mt-6 inline-block">
              <Button variant="primary">Browse Events</Button>
            </Link>
          </div>
        )}

        {status === "success" && reservations.length > 0 && (
          <>
            <div className="space-y-4">
              {reservations.map((reservation) => (
                <div
                  key={reservation.reservationId}
                  className="bg-white rounded-lg border border-gray-200 p-6 shadow-sm flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4"
                >
                  <div className="space-y-1">
                    <div className="flex items-center gap-2">
                      <span className={`text-xs px-2.5 py-0.5 rounded-full font-medium ${CATEGORY_COLORS[reservation.category] ?? "bg-gray-100 text-gray-600"}`}>
                        {reservation.category}
                      </span>
                      <span className="text-xs text-gray-400">
                        Reserved {new Date(reservation.reservedAt).toLocaleDateString()}
                      </span>
                    </div>
                    <Link
                      href={`/events/${reservation.eventId}`}
                      className="text-lg font-semibold text-gray-900 hover:text-blue-600 transition-colors"
                    >
                      {reservation.title}
                    </Link>
                    <p className="text-xs text-gray-500">
                      📍 {reservation.location} • 📅 {new Date(reservation.eventDate).toLocaleDateString()} at {new Date(reservation.eventDate).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })}
                    </p>
                  </div>

                  <div className="flex items-center gap-3">
                    <Link href={`/events/${reservation.eventId}`}>
                      <Button variant="outline" size="sm">
                        View Event
                      </Button>
                    </Link>
                    <Button
                      variant="outline"
                      size="sm"
                      className="text-red-600 border-red-300 hover:bg-red-50"
                      onClick={() => {
                        setCancellingEventId(reservation.eventId);
                        setCancellingEventTitle(reservation.title);
                      }}
                    >
                      Cancel
                    </Button>
                  </div>
                </div>
              ))}
            </div>

            <Pagination
              page={page}
              totalPages={totalPages}
              onPageChange={setPage}
              isLoading={isFetching}
            />
          </>
        )}
      </div>
    </div>
  );
}
