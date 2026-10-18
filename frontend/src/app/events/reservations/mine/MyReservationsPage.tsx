// Ref: workflow.md §4 Architecture Patterns | Feature: Events
"use client";

import Link from "next/link";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";
import { eventService } from "@/services/eventService";
import { queryKeys } from "@/services/queryKeys";
import { getApiErrorMessage } from "@/lib/apiError";
import { useState } from "react";

const CATEGORY_COLORS: Record<string, string> = {
  Academic: "bg-blue-50 text-blue-700",
  Social: "bg-purple-50 text-purple-700",
  Sports: "bg-green-50 text-green-700",
  Cultural: "bg-yellow-50 text-yellow-700",
  Career: "bg-orange-50 text-orange-700",
  Volunteering: "bg-teal-50 text-teal-700",
  Other: "bg-gray-100 text-gray-600",
};

import Pagination from "@/components/common/Pagination";

export default function MyReservationsPage() {
  const queryClient = useQueryClient();
  const [cancellingEventId, setCancellingEventId] = useState<string | null>(null);
  const [cancelError, setCancelError] = useState<string | null>(null);
  const [page, setPage] = useState(1);

  const {
    data,
    status,
    isFetching,
    error,
    refetch,
  } = useQuery({
    queryKey: [...queryKeys.events.myReservations(), page],
    queryFn: () => eventService.getMyReservations({ page, pageSize: 10 }),
  });

  const reservations = data?.items ?? [];
  const totalPages = data?.totalPages ?? 1;

  const cancelMutation = useMutation({
    mutationFn: (eventId: string) => eventService.cancelReservation(eventId),
    onMutate: (eventId) => {
      setCancellingEventId(eventId);
      setCancelError(null);
    },
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: queryKeys.events.myReservations() });
    },
    onError: (err) => {
      setCancelError(getApiErrorMessage(err, "Failed to cancel reservation. Please try again."));
    },
    onSettled: () => {
      setCancellingEventId(null);
    },
  });

  if (status === "pending") {
    return (
      <div className="py-8">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
          <div className="mb-6 flex items-center justify-between">
            <div className="h-8 w-48 bg-gray-100 rounded animate-pulse" />
          </div>
          <div className="space-y-4">
            {[...Array(3)].map((_, i) => (
              <div key={i} className="bg-white border border-gray-200 rounded-lg p-6 animate-pulse">
                <div className="flex items-start gap-4">
                  <div className="h-12 w-12 bg-gray-100 rounded-lg shrink-0" />
                  <div className="flex-1 space-y-2">
                    <div className="h-4 bg-gray-100 rounded w-1/2" />
                    <div className="h-3 bg-gray-100 rounded w-1/3" />
                  </div>
                </div>
              </div>
            ))}
          </div>
        </div>
      </div>
    );
  }

  if (status === "error") {
    return (
      <div className="py-8 text-center">
        <p className="text-red-600" role="alert">
          {getApiErrorMessage(error, "Failed to load your reservations.")}
        </p>
        <button
          type="button"
          onClick={() => void refetch()}
          className="mt-3 text-sm text-gray-900 underline"
        >
          Try again
        </button>
      </div>
    );
  }

  return (
    <div className="py-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Header */}
        <div className="mb-6 flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <div className="flex items-center gap-2 text-sm text-gray-500 mb-1">
              <Link href="/events" className="hover:text-gray-900">Events</Link>
              <span>/</span>
              <span className="text-gray-900 font-medium">My Reservations</span>
            </div>
            <h1 className="text-2xl font-bold text-gray-900">My Event Reservations</h1>
            <p className="mt-1 text-sm text-gray-500">
              {reservations.length > 0
                ? `You have reserved ${reservations.length} event${reservations.length === 1 ? "" : "s"}`
                : "Events you have signed up to attend"}
            </p>
          </div>

          <Link
            href="/events"
            className="inline-flex items-center justify-center rounded-md text-sm font-medium h-10 px-4 border border-gray-200 bg-white shadow-sm hover:bg-gray-50 text-gray-900 transition-colors self-start sm:self-auto"
          >
            ← Browse More Events
          </Link>
        </div>

        {cancelError && (
          <div className="mb-6 rounded-md bg-red-50 p-4 text-sm text-red-700" role="alert">
            {cancelError}
          </div>
        )}

        {/* Reservations list */}
        {reservations.length === 0 ? (
          <div className="text-center py-16 border border-dashed border-gray-200 rounded-lg">
            <svg
              className="mx-auto h-12 w-12 text-gray-400"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={1.5}
                d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"
              />
            </svg>
            <p className="mt-3 text-gray-500 text-sm">
              You haven&apos;t reserved a spot in any events yet.
            </p>
            <Link
              href="/events"
              className="inline-flex items-center justify-center rounded-md text-sm font-medium h-9 px-4 bg-gray-900 text-white shadow-sm hover:bg-gray-800 transition-colors mt-4"
            >
              Browse Events
            </Link>
          </div>
        ) : (
          <div className="space-y-4">
            {reservations.map((reservation) => {
              const startDate = new Date(reservation.eventDate);
              const endDate = new Date(reservation.eventEndDate);
              const categoryColor = CATEGORY_COLORS[reservation.category] ?? CATEGORY_COLORS.Other;
              const isCancelling = cancellingEventId === reservation.eventId;

              return (
                <div
                  key={reservation.reservationId}
                  className="rounded-lg border border-gray-200 bg-white shadow-sm transition-all hover:border-gray-300 p-5"
                >
                  <div className="flex flex-col gap-4 sm:flex-row sm:items-start sm:justify-between">
                    <div className="flex items-start gap-4">
                      {/* Date block */}
                      <div className="shrink-0 h-12 w-12 rounded-lg bg-gray-50 border border-gray-100 flex flex-col items-center justify-center">
                        <span className="text-xs text-gray-500 leading-none">
                          {startDate.toLocaleString("default", { month: "short" })}
                        </span>
                        <span className="text-lg font-bold text-gray-900 leading-tight">
                          {startDate.getDate()}
                        </span>
                      </div>

                      {/* Content */}
                      <div className="min-w-0 flex-1">
                        <div className="flex flex-wrap items-center gap-2 mb-1">
                          <Link
                            href={`/events/${reservation.eventId}`}
                            className="text-base font-semibold text-gray-900 hover:text-gray-700 transition-colors"
                          >
                            {reservation.title}
                          </Link>
                          <span className={`inline-flex items-center px-2 py-0.5 rounded text-xs font-medium shrink-0 ${categoryColor}`}>
                            {reservation.category}
                          </span>
                          {(reservation.visibility === "Public" || reservation.visibility === "1") && (
                            <span className="inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-green-50 text-green-700 shrink-0">
                              Public
                            </span>
                          )}
                        </div>

                        <p className="text-sm text-gray-500 mb-2">
                          <span>
                            {startDate.toLocaleDateString(undefined, { weekday: "short", month: "short", day: "numeric" })}
                            {" · "}
                            {startDate.toLocaleTimeString(undefined, { hour: "2-digit", minute: "2-digit" })}
                            {" – "}
                            {endDate.toLocaleTimeString(undefined, { hour: "2-digit", minute: "2-digit" })}
                          </span>
                          <span className="mx-2 text-gray-300">|</span>
                          <span>{reservation.location}</span>
                        </p>

                        <p className="text-sm text-gray-600 line-clamp-2">{reservation.description}</p>

                        <p className="text-xs text-gray-400 mt-2">
                          Reserved on {new Date(reservation.reservedAt).toLocaleDateString()}
                        </p>
                      </div>
                    </div>

                    {/* Actions */}
                    <div className="flex items-center gap-3 shrink-0 self-end sm:self-center">
                      <Link
                        href={`/events/${reservation.eventId}`}
                        className="inline-flex items-center justify-center rounded-md text-xs font-medium h-8 px-3 border border-gray-200 bg-white text-gray-700 hover:bg-gray-50 transition-colors"
                      >
                        View Details
                      </Link>
                      <button
                        type="button"
                        onClick={() => {
                          if (window.confirm(`Are you sure you want to cancel your reservation for "${reservation.title}"?`)) {
                            cancelMutation.mutate(reservation.eventId);
                          }
                        }}
                        disabled={isCancelling}
                        className="inline-flex items-center justify-center rounded-md text-xs font-medium h-8 px-3 border border-red-200 bg-red-50 text-red-700 hover:bg-red-100 disabled:opacity-50 transition-colors"
                      >
                        {isCancelling ? "Cancelling..." : "Cancel Reservation"}
                      </button>
                    </div>
                  </div>
                </div>
              );
            })}
          </div>
        )}

        {status === "success" && (
          <Pagination
            page={page}
            totalPages={totalPages}
            onPageChange={setPage}
            isLoading={isFetching}
          />
        )}
      </div>
    </div>
  );
}
