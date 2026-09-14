// Ref: workflow.md §4 Architecture Patterns | Feature: Events
"use client";

import { notFound } from "next/navigation";
import { useParams } from "next/navigation";
import { useQuery } from "@tanstack/react-query";
import Link from "next/link";
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

  if (status === "pending" || isLoading) return <div className="text-center py-12">Loading event...</div>;
  if (status === "error") return notFound();

  if (!event) return notFound();

  return (
    <div className="py-8">
      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="mb-6 flex items-center justify-between">
          <h1 className="text-2xl font-bold text-gray-900">{event.title}</h1>
          <Link href="/events" className="btn btn-outline">
            ← Back to Events
          </Link>
        </div>

        <div className="space-y-6">
          <div className="border-t pt-6">
            <div className="text-sm text-gray-500 mb-2">
              {new Date(event.date).toLocaleDateString()} • {event.location}
            </div>
            <p className="text-gray-700">{event.description}</p>
          </div>

          <div className="border-t pt-6">
            <h2 className="text-lg font-medium text-gray-900 mb-4">Organized by</h2>
            <div className="flex items-center space-x-4">
              <div className="flex-shrink-0 h-12 w-12 rounded-lg bg-gray-100">
                {/* Organizer avatar would go here */}
                👤
              </div>
              <div>
                <h3 className="text-base font-medium text-gray-900">Organizer Name</h3>
                <p className="text-sm text-gray-500">{event.organizerId}</p>
              </div>
            </div>
          </div>

          {event.isPublic ? (
            <div className="border-t pt-6">
              <h2 className="text-lg font-medium text-gray-900 mb-4">Event Details</h2>
              <dl className="grid grid-cols-1 gap-4 sm:grid-cols-2">
                <div>
                  <dt className="text-sm font-medium text-gray-500">Category</dt>
                  <dd className="mt-1 text-sm text-gray-900">{event.category}</dd>
                </div>
                <div>
                  <dt className="text-sm font-medium text-gray-500">Visibility</dt>
                  <dd className="mt-1 text-sm text-gray-900">{event.isPublic ? "Public" : "Private"}</dd>
                </div>
              </dl>
            </div>
          ) : null}
        </div>
      </div>
    </div>
  );
}