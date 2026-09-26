// Ref: workflow.md §4 Architecture Patterns | Feature: Events
"use client";

import Link from "next/link";
import { useQuery } from "@tanstack/react-query";
import { eventService } from "@/services/eventService";
import { queryKeys } from "@/services/queryKeys";

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
  const { data: events = [], status } = useQuery({
    queryKey: queryKeys.events.lists(),
    queryFn: () => eventService.getEvents(),
  });

  if (status === "pending") return (
    <div className="py-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="mb-6 flex items-center justify-between">
          <div className="h-8 w-40 bg-gray-100 rounded animate-pulse" />
          <div className="h-10 w-32 bg-gray-100 rounded-md animate-pulse" />
        </div>
        <div className="space-y-4">
          {[...Array(4)].map((_, i) => (
            <div key={i} className="bg-white border border-gray-200 rounded-lg p-6 animate-pulse">
              <div className="flex items-start gap-4">
                <div className="h-10 w-10 bg-gray-100 rounded-lg shrink-0" />
                <div className="flex-1 space-y-2">
                  <div className="h-4 bg-gray-100 rounded w-1/2" />
                  <div className="h-3 bg-gray-100 rounded w-1/3" />
                  <div className="h-3 bg-gray-100 rounded w-3/4" />
                </div>
              </div>
            </div>
          ))}
        </div>
      </div>
    </div>
  );

  if (status === "error") return (
    <div className="py-8 text-center">
      <p className="text-gray-500">Failed to load events.</p>
      <button onClick={() => window.location.reload()} className="mt-3 text-sm text-gray-900 underline">
        Try again
      </button>
    </div>
  );

  return (
    <div className="py-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">

        {/* Header */}
        <div className="mb-6 flex items-center justify-between">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">Campus Events</h1>
            <p className="mt-1 text-sm text-gray-500">
              {events.length > 0 ? `${events.length} public event${events.length === 1 ? "" : "s"}` : ""}
            </p>
          </div>
          <Link
            href="/events/create"
            className="inline-flex items-center justify-center rounded-md text-sm font-medium h-10 px-4 bg-gray-900 text-white shadow-sm hover:bg-gray-800 transition-colors"
          >
            + Create Event
          </Link>
        </div>

        {/* Events list */}
        {events.length === 0 ? (
          <div className="text-center py-16 border border-dashed border-gray-200 rounded-lg">
            <p className="text-gray-500 text-sm">No public events yet. Be the first to create one!</p>
            <Link
              href="/events/create"
              className="inline-flex items-center justify-center rounded-md text-sm font-medium h-9 px-4 bg-gray-900 text-white shadow-sm hover:bg-gray-800 transition-colors mt-4"
            >
              Create Event
            </Link>
          </div>
        ) : (
          <div className="space-y-4">
            {events.map((event) => {
              const startDate = new Date(event.eventDate);
              const endDate = new Date(event.eventEndDate);
              const categoryColor = CATEGORY_COLORS[event.category] ?? CATEGORY_COLORS.Other;

              return (
                <Link
                  key={event.id}
                  href={`/events/${event.id}`}
                  className="group block rounded-lg border border-gray-200 bg-white shadow-sm hover:shadow-md hover:border-gray-300 transition-all"
                >
                  <div className="p-5">
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
                      <div className="flex-1 min-w-0">
                        <div className="flex flex-wrap items-center gap-2 mb-1">
                          <h3 className="text-base font-semibold text-gray-900 group-hover:text-gray-700 transition-colors truncate">
                            {event.title}
                          </h3>
                          <span className={`inline-flex items-center px-2 py-0.5 rounded text-xs font-medium shrink-0 ${categoryColor}`}>
                            {event.category}
                          </span>
                          {(event.visibility === "Public" || event.visibility === "1") && (
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
                          <span>{event.location}</span>
                        </p>
                        <p className="text-sm text-gray-600 line-clamp-2">{event.description}</p>
                      </div>

                      {/* Arrow */}
                      <svg className="shrink-0 h-4 w-4 text-gray-300 group-hover:text-gray-500 mt-1 transition-colors" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
                      </svg>
                    </div>
                  </div>
                </Link>
              );
            })}
          </div>
        )}
      </div>
    </div>
  );
}