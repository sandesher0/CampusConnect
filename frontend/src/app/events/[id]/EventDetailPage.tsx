// Ref: workflow.md §4 Architecture Patterns | Feature: Events
"use client";

import { notFound } from "next/navigation";
import { useParams } from "next/navigation";
import { useMutation, useQuery } from "@tanstack/react-query";
import Link from "next/link";
import axios from "axios";
import { eventService } from "@/services/eventService";
import { queryKeys } from "@/services/queryKeys";

export default function EventDetailPage() {
  const params = useParams<{ id: string }>();
  const { id } = params;

  const {
    data: event,
    status,
    isLoading,
  } = useQuery({
    queryKey: queryKeys.events.detail(id),
    queryFn: () => eventService.getEventById(id),
    enabled: !!id,
  });

  const joinMutation = useMutation({
    mutationFn: () => eventService.joinEvent(id, event!.communityId),
  });

  if (status === "pending" || isLoading) return <div className="text-center py-12">Loading event...</div>;
  if (status === "error") return notFound();

  if (!event) return notFound();

  const organizerName = `${event.organizer.firstName} ${event.organizer.lastName}`;
  const organizerInitials = `${event.organizer.firstName.charAt(0)}${event.organizer.lastName.charAt(0)}`;

  return (
    <div className="py-8">
      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="mb-6 flex items-center justify-between">
          <h1 className="text-2xl font-bold text-gray-900">{event.title}</h1>
          <Link href="/events" className="inline-flex items-center justify-center rounded-md text-sm font-medium h-10 px-4 border border-gray-200 bg-white shadow-sm hover:bg-gray-50 hover:text-gray-900 transition-colors">
            ← Back to Events
          </Link>
        </div>

        <div className="space-y-8 bg-white border border-gray-200 shadow-sm rounded-lg p-6 sm:p-8">
          <div>
            <div className="text-sm font-medium text-gray-500 mb-3 flex items-center gap-2">
              <svg className="w-4 h-4" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z" /></svg>
              {new Date(event.eventDate).toLocaleDateString()} • {event.location}
            </div>
            <p className="text-gray-700 leading-relaxed">{event.description}</p>
          </div>

          <div className="border-t border-gray-100 pt-6">
            <div className="flex flex-col gap-2 sm:flex-row sm:items-center sm:justify-between">
              <div>
                <h2 className="text-lg font-medium text-gray-900">Attend this event</h2>
                <p className="mt-1 text-sm text-gray-500">
                  You must be a member of the organizing community to join.
                </p>
              </div>
              <button
                type="button"
                onClick={() => joinMutation.mutate()}
                disabled={joinMutation.isPending || joinMutation.isSuccess}
                className="inline-flex h-10 shrink-0 items-center justify-center rounded-md bg-gray-900 px-4 text-sm font-medium text-white shadow-sm transition-colors hover:bg-gray-800 disabled:cursor-not-allowed disabled:opacity-60"
              >
                {joinMutation.isPending
                  ? "Joining..."
                  : joinMutation.isSuccess
                    ? "Joined"
                    : "Join Event"}
              </button>
            </div>
            {joinMutation.isSuccess && (
              <p className="mt-3 text-sm text-green-700" role="status">
                You have successfully joined this event.
              </p>
            )}
            {joinMutation.isError && (
              <p className="mt-3 text-sm text-red-600" role="alert">
                {axios.isAxiosError(joinMutation.error)
                  ? joinMutation.error.response?.data?.detail ?? "Unable to join this event. Please try again."
                  : "Unable to join this event. Please try again."}
              </p>
            )}
          </div>

          <div className="border-t border-gray-100 pt-6">
            <h2 className="text-lg font-medium text-gray-900 mb-4">Organized by</h2>
            <div className="flex items-center space-x-4">
              <div className="shrink-0 h-12 w-12 rounded-full bg-gray-100 flex items-center justify-center">
                {event.organizer.profileImageUrl ? (
                  <img
                    src={event.organizer.profileImageUrl}
                    alt={organizerName}
                    className="h-12 w-12 rounded-full object-cover"
                  />
                ) : (
                  <span className="text-sm font-medium text-gray-600">{organizerInitials}</span>
                )}
              </div>
              <div>
                <h3 className="text-base font-medium text-gray-900">{organizerName}</h3>
                <Link
                  href={`/community/${event.organizerCommunity.communityId}`}
                  className="text-sm text-gray-500 hover:text-gray-700 hover:underline"
                >
                  {event.organizerCommunity.communityName}
                </Link>
              </div>
            </div>
          </div>

          {event.visibility === 'Public' ? (
            <div className="border-t border-gray-100 pt-6">
              <h2 className="text-lg font-medium text-gray-900 mb-4">Event Details</h2>
              <dl className="grid grid-cols-1 gap-4 sm:grid-cols-2 bg-gray-50 rounded-lg p-4 border border-gray-100">
                <div>
                  <dt className="text-sm font-medium text-gray-500">Category</dt>
                  <dd className="mt-1 text-sm text-gray-900">{event.category}</dd>
                </div>
                <div>
                  <dt className="text-sm font-medium text-gray-500">Visibility</dt>
                  <dd className="mt-1 text-sm text-gray-900">{event.visibility === 'Public' ? "Public" : "Private"}</dd>
                </div>
              </dl>
            </div>
          ) : null}
        </div>
      </div>
    </div>
  );
}
