// Ref: workflow.md §4 Architecture Patterns | Feature: Events
"use client";

import Link from "next/link";
import { useInfiniteQuery } from "@tanstack/react-query";
import { eventService } from "@/services/eventService";
import { queryKeys } from "@/services/queryKeys";

export default function EventsPage() {
  const {
    data,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
    status,
  } = useInfiniteQuery({
    queryKey: queryKeys.events.lists(),
    queryFn: ({ pageParam = {} }) => eventService.getEvents(pageParam),
    getNextPageParam: (lastPage) =>
      lastPage.length > 0 ? { page: ((lastPage[0] as any).page || 0) + 1 } : undefined,
    initialPageParam: {},
  });

  if (status === "pending") return <div className="text-center py-12">Loading events...</div>;
  if (status === "error") return <div className="text-center py-12">Failed to load events</div>;

  return (
    <div className="py-8">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="mb-6 flex items-center justify-between">
          <h1 className="text-3xl font-bold text-gray-900">Campus Events</h1>
          <Link href="/events/create" className="btn btn-primary">
            Create Event
          </Link>
        </div>

        <div className="space-y-6">
          {(data?.pages.flatMap(page => page) || []).map((event) => (
            <Link
              key={event.id}
              href={`/events/${event.id}`}
              className="group block rounded-lg border border-gray-200 hover:border-gray-300 hover:bg-gray-50 transition-colors"
            >
              <div className="p-6">
                <div className="flex items-start space-x-4">
                  <div className="flex-shrink-0 h-10 w-10 rounded-lg bg-gray-100">
                    {/* Event icon would go here */}
                    📅
                  </div>
                  <div>
                    <h3 className="text-lg font-medium text-gray-900 group-hover:text-gray-800 transition-colors">
                      {event.title}
                    </h3>
                    <p className="mt-1 text-sm text-gray-600">
                      {new Date(event.date).toLocaleDateString()} • {event.location}
                    </p>
                    <p className="mt-2 line-clamp-2 text-gray-700">
                      {event.description}
                    </p>
                    <div className="mt-3 flex items-center text-sm text-gray-500">
                      {/* <span>By: Organizer name</span> */}
                    </div>
                  </div>
                </div>
              </div>
            </Link>
          ))}

          {!(data?.pages.flatMap(page => page) || []).length && (
            <div className="text-center py-12">
              <p className="text-gray-500">No events found. Be the first to create one!</p>
              <Link href="/events/create" className="btn btn-primary mt-4">
                Create Event
              </Link>
            </div>
          )}
        </div>

        {hasNextPage && (
          <div className="text-center py-6">
            <button
              onClick={() => fetchNextPage()}
              disabled={isFetchingNextPage}
              className={`btn btn-outline ${isFetchingNextPage ? "opacity-50" : ""}`}
            >
              {isFetchingNextPage ? "Loading..." : "Load More Events"}
            </button>
          </div>
        )}
      </div>
    </div>
  );
}