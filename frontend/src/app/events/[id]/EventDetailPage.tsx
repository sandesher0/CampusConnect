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
            <h2 className="text-lg font-medium text-gray-900 mb-4">Organized by</h2>
            <div className="flex items-center space-x-4">
              <div className="flex-shrink-0 h-12 w-12 rounded-full bg-gray-100 flex items-center justify-center">
                {/* Organizer avatar would go here */}
                <svg className="w-6 h-6 text-gray-500" fill="none" viewBox="0 0 24 24" stroke="currentColor"><path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" /></svg>
              </div>
              <div>
                <h3 className="text-base font-medium text-gray-900">Organizer Name</h3>
                <p className="text-sm text-gray-500">{event.createdBy}</p>
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