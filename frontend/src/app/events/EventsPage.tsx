// Ref: workflow.md §4 Architecture Patterns | Feature: Events Page
"use client";

import { Suspense } from "react";
import Link from "next/link";
import { eventService } from "@/services/eventService";
import { queryKeys } from "@/services/queryKeys";
import Button from "@/components/common/Button";
import { getApiErrorMessage } from "@/lib/apiError";
import Pagination from "@/components/common/Pagination";
import { usePaginatedQuery } from "@/hooks/usePaginatedQuery";
import type { EventSummaryResponse } from "@/types/event";

const CATEGORY_COLORS: Record<string, string> = {
  Academic: "bg-blue-50 text-blue-700",
  Social: "bg-purple-50 text-purple-700",
  Sports: "bg-green-50 text-green-700",
  Cultural: "bg-yellow-50 text-yellow-700",
  Career: "bg-orange-50 text-orange-700",
  Volunteering: "bg-teal-50 text-teal-700",
  Other: "bg-gray-100 text-gray-600",
};

export default function EventsPage() {
  return (
    <Suspense fallback={<div className="min-h-[calc(100vh-160px)] flex items-center justify-center text-gray-500">Loading events...</div>}>
      <EventsPageInner />
    </Suspense>
  );
}

function EventsPageInner() {
  const {
    items: events,
    page,
    totalPages,
    setPage,
    status,
    isFetching,
    error,
    refetch,
  } = usePaginatedQuery<EventSummaryResponse>({
    queryKeyPrefix: queryKeys.events.all(),
    queryFn: (params) => eventService.getEvents(params),
    defaultPageSize: 12,
  });

  if (status === "error") {
    return (
      <div className="min-h-[calc(100vh-160px)] flex flex-col items-center justify-center py-12">
        <p className="text-gray-600 mb-4" role="alert">
          {getApiErrorMessage(error, "Failed to load events.")}
        </p>
        <Button variant="outline" onClick={() => void refetch()}>
          Try Again
        </Button>
      </div>
    );
  }

  return (
    <div className="min-h-[calc(100vh-160px)] py-12">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="mb-8 flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">Campus Events</h1>
            <p className="mt-1 text-sm text-gray-500">
              Explore upcoming activities, workshops, and gatherings
            </p>
          </div>
          <Link
            href="/events/create"
            className="inline-flex items-center justify-center rounded-md text-sm font-medium h-10 px-4 bg-gray-900 text-white shadow-sm hover:bg-gray-800 transition-colors whitespace-nowrap"
          >
            + Host Event
          </Link>
        </div>

        {status === "pending" ? (
          <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {[...Array(6)].map((_, i) => (
              <div key={i} className="bg-white rounded-lg border border-gray-200 p-6 animate-pulse space-y-3">
                <div className="h-5 bg-gray-200 rounded w-3/4" />
                <div className="h-4 bg-gray-100 rounded w-1/2" />
                <div className="h-4 bg-gray-100 rounded w-1/3" />
              </div>
            ))}
          </div>
        ) : events.length === 0 ? (
          <div className="bg-white rounded-lg border border-gray-200 p-12 text-center">
            <h3 className="text-base font-semibold text-gray-900">No events scheduled</h3>
            <p className="mt-1 text-sm text-gray-500">Check back later or host one yourself!</p>
          </div>
        ) : (
          <>
            <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
              {events.map((event) => (
                <Link
                  key={event.id}
                  href={`/events/${event.id}`}
                  className="bg-white rounded-lg border border-gray-200 p-6 hover:border-gray-300 transition-shadow shadow-sm hover:shadow flex flex-col justify-between"
                >
                  <div>
                    <div className="flex items-start justify-between gap-2 mb-2">
                      <span className={`text-xs px-2.5 py-0.5 rounded-full font-medium ${CATEGORY_COLORS[event.category] ?? "bg-gray-100 text-gray-600"}`}>
                        {event.category}
                      </span>
                      <span className="text-xs text-gray-500">{event.visibility}</span>
                    </div>
                    <h3 className="font-semibold text-lg text-gray-900 mb-2 hover:text-blue-600 transition-colors">
                      {event.title}
                    </h3>
                    <p className="text-xs text-gray-500 mb-1">
                      📍 {event.location}
                    </p>
                    <p className="text-xs text-gray-500">
                      📅 {new Date(event.eventDate).toLocaleDateString()} at {new Date(event.eventDate).toLocaleTimeString([], { hour: "2-digit", minute: "2-digit" })}
                    </p>
                  </div>
                </Link>
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
