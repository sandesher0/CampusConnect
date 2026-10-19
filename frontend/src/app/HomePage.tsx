// Ref: workflow.md §4 Architecture Patterns | Feature: Landing Page
"use client";

import Link from "next/link";
import { useSyncExternalStore } from "react";
import { useQuery } from "@tanstack/react-query";
import { useAuthStore } from "@/stores/authStore";
import { eventService } from "@/services/eventService";
import { queryKeys } from "@/services/queryKeys";
import Button from "@/components/common/Button";
import Card from "@/components/common/Card";
import CategoryChip from "@/components/common/CategoryChip";
import StatusDot from "@/components/common/StatusDot";
import type { EventSummaryResponse } from "@/types/event";

export default function HomePage() {
  const { user } = useAuthStore();
  const isHydrated = useSyncExternalStore(
    () => () => {},
    () => true,
    () => false
  );

  // Phase 1B: Fetch exactly pageSize: 3 upcoming events
  const {
    data: eventsData,
    status: eventsStatus,
    error: eventsError,
    refetch: refetchEvents,
  } = useQuery({
    queryKey: queryKeys.events.list({ page: 1, pageSize: 3 }),
    queryFn: () => eventService.getEvents({ page: 1, pageSize: 3 }),
  });

  const events = eventsData?.items ?? [];
  const now = new Date();

  // Date formatter: "Fri · 6:00 PM"
  const formatEventDateTime = (dateStr: string) => {
    try {
      const d = new Date(dateStr);
      const weekday = d.toLocaleDateString("en-US", { weekday: "short" });
      const time = d.toLocaleTimeString("en-US", { hour: "numeric", minute: "2-digit" });
      return `${weekday} · ${time}`;
    } catch {
      return dateStr;
    }
  };

  return (
    <div className="flex flex-col gap-0 overflow-hidden">
      {/* 1. HERO SECTION */}
      <section
        className="relative bg-page overflow-hidden pt-12 pb-20 md:pt-20 md:pb-32"
        aria-labelledby="hero-heading"
      >
        {/* Decorative flat shapes */}
        <div
          className="absolute -top-32 -right-32 w-[520px] h-[520px] rounded-full bg-primary-soft-tint pointer-events-none"
          aria-hidden="true"
        />
        <div
          className="absolute top-28 right-[42%] w-3.5 h-3.5 rounded-full bg-coral pointer-events-none"
          aria-hidden="true"
        />

        <div className="mx-auto max-w-[1200px] px-6 sm:px-8 relative z-10">
          <div className="grid grid-cols-1 lg:grid-cols-12 gap-12 lg:gap-8 items-center">
            {/* Left Column: Copy & Actions */}
            <div className="lg:col-span-7 flex flex-col items-start">
              {/* Signed-in Status / Placeholder */}
              {!isHydrated ? (
                <div className="h-8 w-44 rounded-full bg-primary-soft-tint animate-pulse mb-6" aria-hidden="true" />
              ) : user ? (
                <div className="animate-rise delay-0 inline-flex items-center gap-2 px-3.5 py-1.5 rounded-full bg-primary-tint border border-border-tint mb-6">
                  <StatusDot status="good" pulse />
                  <span className="text-xs font-bold text-primary-on-tint">
                    Welcome back, {user.user.firstName || user.username}
                  </span>
                </div>
              ) : null}

              {/* H1 Heading */}
              <h1
                id="hero-heading"
                className="animate-rise delay-80 text-[clamp(40px,6vw,66px)] leading-[1.06] font-[800] tracking-[-0.035em] text-ink"
              >
                Find your people.
                <br />
                Find your{" "}
                <span className="text-primary [box-shadow:inset_0_-0.26em_0_var(--color-highlight)]">
                  next thing.
                </span>
              </h1>

              {/* Subtitle */}
              <p className="animate-rise delay-160 mt-6 text-xl leading-relaxed text-muted max-w-[520px]">
                Events, clubs and campus resources, all in one place. Zero boring.
              </p>

              {/* Action Buttons & Quick Links */}
              <div className="animate-rise delay-240 mt-8 flex flex-col sm:flex-row items-stretch sm:items-center gap-4 w-full sm:w-auto">
                {!isHydrated ? (
                  <div className="h-12 w-64 rounded-full bg-primary-soft-tint animate-pulse" aria-hidden="true" />
                ) : user ? (
                  <>
                    <Button
                      href="/events"
                      variant="primary"
                      size="hero"
                      rightIcon={
                        <svg className="w-5 h-5 text-white" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
                          <path d="M5 12h14m-7-7 7 7-7 7" />
                        </svg>
                      }
                    >
                      Browse Events
                    </Button>
                    <Button href="/community" variant="outline" size="hero">
                      Explore Communities
                    </Button>
                  </>
                ) : (
                  <>
                    <Button href="/register" variant="primary" size="hero">
                      Sign up free
                    </Button>
                    <Button href="/login" variant="outline" size="hero">
                      Log in
                    </Button>
                  </>
                )}
              </div>

              {/* Secondary Quick Links */}
              {isHydrated && user && (
                <div className="animate-rise delay-320 mt-6 flex flex-wrap items-center gap-3">
                  <Button href="/events/reservations/mine" variant="tint" size="sm">
                    My Reservations
                  </Button>
                  <Button href="/community/mine" variant="tint" size="sm">
                    My Communities
                  </Button>
                </div>
              )}

              {isHydrated && !user && (
                <div className="animate-rise delay-320 mt-6">
                  <Link
                    href="/events"
                    className="inline-flex items-center gap-1.5 text-sm font-bold text-primary hover:underline"
                  >
                    Browse events as guest
                    <svg className="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round">
                      <path d="M5 12h14m-7-7 7 7-7 7" />
                    </svg>
                  </Link>
                </div>
              )}
            </div>

            {/* Right Column: Decorative Preview Cards */}
            <div
              className="lg:col-span-5 relative flex flex-col gap-4 items-center lg:items-end select-none"
              aria-hidden="true"
            >
              {/* Event preview card */}
              <div className="w-full max-w-[340px] animate-float delay-float-1 bg-surface rounded-[22px] border border-border-card p-5 shadow-[0_2px_4px_rgba(27,21,48,0.05),0_18px_40px_rgba(77,52,200,0.12)]">
                <div className="flex items-center justify-between mb-3">
                  <CategoryChip category="Social" />
                  <span className="text-xs font-bold text-muted">Fri · 6:00 PM</span>
                </div>
                <h4 className="text-base font-extrabold text-ink">Welcome Week Mixer</h4>
                <p className="text-xs text-muted mt-1">Student Union Courtyard</p>
              </div>

              {/* Community preview card */}
              <div className="w-full max-w-[340px] sm:translate-x-4 animate-float delay-float-2 bg-surface rounded-[22px] border border-border-card p-5 shadow-[0_2px_4px_rgba(27,21,48,0.05),0_18px_40px_rgba(77,52,200,0.12)]">
                <div className="flex items-center gap-3">
                  <div className="h-10 w-10 rounded-full bg-cat-sports-bg flex items-center justify-center text-status-good-text shrink-0">
                    <svg className="w-5 h-5" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
                      <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2" />
                      <circle cx="9" cy="7" r="4" />
                      <path d="M23 21v-2a4 4 0 0 0-3-3.87" />
                      <path d="M16 3.13a4 4 0 0 1 0 7.75" />
                    </svg>
                  </div>
                  <div>
                    <h4 className="text-base font-extrabold text-ink">Robotics Club</h4>
                    <p className="text-xs font-semibold text-status-good-text">Open to everyone</p>
                  </div>
                </div>
              </div>

              {/* Confirmation pill */}
              <div className="w-full max-w-[340px] -translate-x-2 animate-float delay-float-3 bg-primary text-white rounded-full px-5 py-3.5 flex items-center gap-3 shadow-[0_6px_18px_rgba(109,74,255,0.28)]">
                <div className="h-6 w-6 rounded-full bg-white/20 flex items-center justify-center shrink-0">
                  <svg className="w-4 h-4 text-white" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="3" strokeLinecap="round" strokeLinejoin="round">
                    <polyline points="20 6 9 17 4 12" />
                  </svg>
                </div>
                <span className="text-xs sm:text-sm font-bold tracking-tight">
                  You&apos;re in! Your spot is saved.
                </span>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* 2. HAPPENING SOON SECTION (Real Data) */}
      <section
        className="bg-surface border-y border-border-divider py-20"
        aria-labelledby="events-heading"
      >
        <div className="mx-auto max-w-[1200px] px-6 sm:px-8">
          <div className="flex flex-col sm:flex-row sm:items-end justify-between gap-4 mb-10">
            <div>
              <p className="text-xs font-[800] uppercase tracking-[0.08em] text-primary">
                THIS WEEK ON CAMPUS
              </p>
              <h2 id="events-heading" className="mt-1 text-3xl font-[800] text-ink">
                Happening soon
              </h2>
            </div>
            <Button href="/events" variant="outline">
              See all events
            </Button>
          </div>

          {/* Loading state: 3 pulse skeleton cards */}
          {eventsStatus === "pending" && (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {[1, 2, 3].map((n) => (
                <div
                  key={n}
                  className="bg-surface rounded-[22px] border border-border-card p-6 space-y-4 animate-pulse"
                >
                  <div className="flex justify-between items-center">
                    <div className="h-6 w-20 rounded-full bg-primary-soft-tint" />
                    <div className="h-4 w-24 rounded bg-primary-soft-tint" />
                  </div>
                  <div className="h-7 w-3/4 rounded bg-primary-soft-tint" />
                  <div className="h-4 w-1/2 rounded bg-primary-soft-tint" />
                  <div className="pt-4 border-t border-border-divider">
                    <div className="h-11 w-full rounded-full bg-primary-soft-tint" />
                  </div>
                </div>
              ))}
            </div>
          )}

          {/* Error state */}
          {eventsStatus === "error" && (
            <Card className="p-8 text-center max-w-lg mx-auto" hoverEffect={false}>
              <h3 className="text-lg font-bold text-ink">Could not load upcoming events</h3>
              <p className="mt-2 text-sm text-muted">
                {eventsError ? String(eventsError) : "An unexpected error occurred while loading events."}
              </p>
              <div className="mt-6 flex justify-center gap-3">
                <Button onClick={() => void refetchEvents()} variant="primary" size="sm">
                  Try Again
                </Button>
                <Button href="/events" variant="outline" size="sm">
                  Browse Events
                </Button>
              </div>
            </Card>
          )}

          {/* Empty state */}
          {eventsStatus === "success" && events.length === 0 && (
            <Card className="p-12 text-center" hoverEffect={false}>
              <h3 className="text-lg font-bold text-ink">No upcoming events yet</h3>
              <p className="mt-2 text-sm text-muted">
                Check back soon or explore communities hosting new campus activities.
              </p>
              <div className="mt-6 flex justify-center">
                <Button href="/events" variant="primary">
                  Explore Events
                </Button>
              </div>
            </Card>
          )}

          {/* Real Events Grid */}
          {eventsStatus === "success" && events.length > 0 && (
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
              {events.map((event: EventSummaryResponse) => {
                const isHappeningNow =
                  new Date(event.eventDate) <= now && new Date(event.eventEndDate) >= now;

                return (
                  <Card key={event.id} className="p-6 flex flex-col justify-between">
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
                        <h3 className="text-[23px] font-[800] leading-snug text-ink group-hover:text-primary transition-colors">
                          {event.title}
                        </h3>
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
                        Save my spot
                      </Button>
                    </div>
                  </Card>
                );
              })}
            </div>
          )}
        </div>
      </section>

      {/* 3. TWO FEATURE CARDS SECTION */}
      <section className="bg-page py-20" aria-labelledby="features-heading">
        <div className="mx-auto max-w-[1200px] px-6 sm:px-8">
          <h2 id="features-heading" className="sr-only">
            Features
          </h2>
          <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
            {/* Violet Tint Feature Card */}
            <div className="bg-primary-tint border border-border-tint rounded-[28px] p-9 sm:p-10 flex flex-col justify-between transition-transform duration-200 hover:-translate-y-1">
              <div>
                <div className="h-14 w-14 rounded-2xl bg-primary flex items-center justify-center text-white mb-6 shadow-[0_6px_18px_rgba(109,74,255,0.25)]">
                  {/* Ticket Icon */}
                  <svg className="w-7 h-7" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                    <path d="M2 9a3 3 0 0 1 0 6v2a2 2 0 0 0 2 2h16a2 2 0 0 0 2-2v-2a3 3 0 0 1 0-6V7a2 2 0 0 0-2-2H4a2 2 0 0 0-2 2Z" />
                    <path d="M13 5v2m0 10v2m0-6v2" />
                  </svg>
                </div>
                <h3 className="text-2xl font-[800] text-ink">
                  Find your next event
                </h3>
                <p className="mt-3 text-base text-muted leading-relaxed">
                  Academic, social or sports, filter by what you&apos;re into and grab a spot before it fills up.
                </p>
              </div>
              <div className="mt-8">
                <Button href="/events" variant="primary">
                  Browse events
                </Button>
              </div>
            </div>

            {/* Coral Tint Feature Card */}
            <div className="bg-coral-tint border border-coral-border rounded-[28px] p-9 sm:p-10 flex flex-col justify-between transition-transform duration-200 hover:-translate-y-1">
              <div>
                <div className="h-14 w-14 rounded-2xl bg-coral flex items-center justify-center text-ink mb-6 shadow-[0_6px_18px_rgba(255,107,87,0.25)]">
                  {/* People Icon */}
                  <svg className="w-7 h-7 text-ink" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                    <path d="M16 21v-2a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v2" />
                    <circle cx="11" cy="7" r="4" />
                    <path d="M22 21v-2a4 4 0 0 0-3-3.87" />
                    <path d="M16 3.13a4 4 0 0 1 0 7.75" />
                  </svg>
                </div>
                <h3 className="text-2xl font-[800] text-ink">
                  Find your people
                </h3>
                <p className="mt-3 text-base text-muted leading-relaxed">
                  Join clubs and organizations that share your focus, or start one of your own.
                </p>
              </div>
              <div className="mt-8">
                <Button href="/community" variant="outline">
                  Explore communities
                </Button>
              </div>
            </div>
          </div>
        </div>
      </section>
    </div>
  );
}
