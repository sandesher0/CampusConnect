// Ref: workflow.md §4 Architecture Patterns | Feature: Events Page
"use client";

import Link from "next/link";
import { Suspense } from "react";
import { eventService } from "@/services/eventService";
import { queryKeys } from "@/services/queryKeys";
import { usePaginatedQuery } from "@/hooks/usePaginatedQuery";
import PageHeader from "@/components/common/PageHeader";
import Card from "@/components/common/Card";
import CategoryChip from "@/components/common/CategoryChip";
import StatusDot from "@/components/common/StatusDot";
import Skeleton from "@/components/common/Skeleton";
import EmptyState from "@/components/common/EmptyState";
import ErrorState from "@/components/common/ErrorState";
import Pagination from "@/components/common/Pagination";
import Button from "@/components/common/Button";
import type { EventSummaryResponse } from "@/types/event";

export default function EventsPage() {
  return (
    <Suspense
      fallback={
        <div className="mx-auto max-w-[1200px] px-6 py-12 sm:px-8">
          <Skeleton className="h-10 w-48 mb-8" />
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {[1, 2, 3, 4, 5, 6].map((i) => (
              <Skeleton key={i} className="h-64 w-full" />
            ))}
          </div>
        </div>
      }
    >
      <EventsPageContent />
    </Suspense>
  );
}

function EventsPageContent() {
  const {
    items: events,
    totalPages,
    page,
    setPage,
    status,
    isFetching,
    error,
    refetch,
  } = usePaginatedQuery<EventSummaryResponse>({
    queryKeyPrefix: queryKeys.events.all(),
    queryFn: eventService.getEvents,
    defaultPageSize: 12,
  });

  const now = new Date();

  const formatEventDateTime = (dateStr: string) => {
    try {
      const d = new Date(dateStr);
      const weekday = d.toLocaleDateString("en-US", { weekday: "short", month: "short", day: "numeric" });
      const time = d.toLocaleTimeString("en-US", { hour: "numeric", minute: "2-digit" });
      return `${weekday} · ${time}`;
    } catch {
      return dateStr;
    }
  };

  return (
    <div className="mx-auto max-w-[1200px] px-6 py-12 sm:px-8">
      <PageHeader
        eyebrow="What's on"
        title="Events"
        description="Discover talks, club meetups, sports games, and social mixers happening across campus."
        action={
          <Button href="/events/create" variant="primary">
            Host an Event
          </Button>
        }
      />

      {/* Loading Skeleton Grid */}
      {status === "pending" && (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {[1, 2, 3, 4, 5, 6].map((i) => (
            <Card key={i} className="p-6 space-y-4" hoverEffect={false}>
              <div className="flex justify-between items-center">
                <Skeleton className="h-6 w-20 rounded-full" />
                <Skeleton className="h-4 w-28" />
              </div>
              <Skeleton className="h-7 w-4/5" />
              <Skeleton className="h-4 w-1/2" />
              <div className="pt-4 border-t border-border-divider">
                <Skeleton className="h-11 w-full rounded-full" />
              </div>
            </Card>
          ))}
        </div>
      )}

      {/* Error State */}
      {status === "error" && (
        <ErrorState
          error={error}
          onRetry={() => void refetch()}
          fallbackMessage="Unable to load campus events. Please check your network and try again."
        />
      )}

      {/* Empty State */}
      {status === "success" && events.length === 0 && (
        <EmptyState
          title="No upcoming events found"
          description="There are currently no upcoming events posted on campus. Be the first to create one for your community!"
          action={
            <Button href="/events/create" variant="primary">
              Create an Event
            </Button>
          }
        />
      )}

      {/* Events Grid with staggered rise */}
      {status === "success" && events.length > 0 && (
        <>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {events.map((event, index) => {
              const isHappeningNow =
                new Date(event.eventDate) <= now && new Date(event.eventEndDate) >= now;
              // Stagger rise on first 6 items
              const delayClass = index < 6 ? `animate-rise delay-${index * 80}` : "";

              return (
                <Card
                  key={event.id}
                  className={`p-6 flex flex-col justify-between ${delayClass}`}
                >
                  <div>
                    <div className="flex items-center justify-between gap-2 mb-4">
                      <CategoryChip category={event.category} />
                      {isHappeningNow ? (
                        <StatusDot status="good" label="Happening now" pulse />
                      ) : (
                        <span className="text-xs font-bold text-muted">
                          {formatEventDateTime(event.eventDate)}
                        </span>
                      )}
                    </div>

                    <Link href={`/events/${event.id}`} className="group block focus-visible:outline-none">
                      <h2 className="text-[22px] font-[800] leading-snug text-ink group-hover:text-primary transition-colors">
                        {event.title}
                      </h2>
                    </Link>

                    <div className="mt-3 flex items-center gap-2 text-xs font-semibold text-muted">
                      <svg className="w-4 h-4 shrink-0 text-muted" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                        <path strokeLinecap="round" strokeLinejoin="round" d="M12 21s-8-4.5-8-11.8A8 8 0 0 1 12 2a8 8 0 0 1 8 7.2c0 7.3-8 11.8-8 11.8z" />
                        <circle cx="12" cy="10" r="3" />
                      </svg>
                      <span className="truncate">{event.location}</span>
                    </div>
                  </div>

                  <div className="mt-6 pt-4 border-t border-border-divider">
                    <Button href={`/events/${event.id}`} variant="primary" className="w-full">
                      View Event & Reserve
                    </Button>
                  </div>
                </Card>
              );
            })}
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
  );
}
