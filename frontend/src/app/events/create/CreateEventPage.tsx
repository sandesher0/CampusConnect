// Ref: workflow.md §4 Architecture Patterns | Feature: Create Event
"use client";

import { useState, Suspense } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter, useSearchParams } from "next/navigation";
import Link from "next/link";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { eventService } from "@/services/eventService";
import { communityService } from "@/services/communityService";
import { queryKeys } from "@/services/queryKeys";
import Button from "@/components/common/Button";
import { getApiErrorMessage } from "@/lib/apiError";
import { handleServerValidationErrors } from "@/utils/formErrors";
import { createEventSchema, type CreateEventValues } from "@/validations/event";
import { useAuthStore } from "@/stores/authStore";

const EVENT_CATEGORIES = [
  { value: "Academic", label: "Academic" },
  { value: "Social", label: "Social" },
  { value: "Sports", label: "Sports" },
  { value: "Cultural", label: "Cultural" },
  { value: "Career", label: "Career" },
  { value: "Volunteering", label: "Volunteering" },
  { value: "Other", label: "Other" },
];

export default function CreateEventPage() {
  return (
    <Suspense fallback={<div className="py-12 text-center text-gray-500">Loading...</div>}>
      <CreateEventPageInner />
    </Suspense>
  );
}

function CreateEventPageInner() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const preselectedCommunityId = searchParams.get("communityId") ?? "";
  const queryClient = useQueryClient();
  const { user } = useAuthStore();
  const [serverError, setServerError] = useState<string | null>(null);

  // Fetch communities user belongs to so they can select where to host the event
  const { data: myCommunitiesData } = useQuery({
    queryKey: queryKeys.communities.mine({ page: 1, pageSize: 50 }),
    queryFn: () => communityService.getMyCommunities({ page: 1, pageSize: 50 }),
    enabled: Boolean(user),
  });

  const myCommunities = myCommunitiesData?.items ?? [];

  const {
    register,
    handleSubmit,
    setError: setFieldError,
    formState: { errors, isSubmitting },
  } = useForm<CreateEventValues>({
    resolver: zodResolver(createEventSchema),
    defaultValues: {
      title: "",
      description: "",
      location: "",
      communityId: preselectedCommunityId,
      eventDate: "",
      eventEndDate: "",
      visibility: "Public",
      category: "Academic",
    },
  });

  const mutation = useMutation({
    mutationFn: (data: CreateEventValues) =>
      eventService.createEvent({
        title: data.title,
        description: data.description,
        location: data.location,
        communityId: data.communityId,
        eventDate: new Date(data.eventDate).toISOString(),
        eventEndDate: new Date(data.eventEndDate).toISOString(),
        visibility: data.visibility,
        category: data.category,
      }),
    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: queryKeys.events.all() }),
      ]);
      router.push("/events");
    },
    onError: (err) => {
      const hasFieldErrors = handleServerValidationErrors(err, setFieldError);
      if (!hasFieldErrors) {
        setServerError(getApiErrorMessage(err, "Failed to create event. Please check your permissions."));
      }
    },
  });

  const onSubmit = (data: CreateEventValues) => {
    setServerError(null);
    mutation.mutate(data);
  };

  return (
    <div className="min-h-[calc(100vh-160px)] py-12">
      <div className="mx-auto max-w-2xl px-4 sm:px-6 lg:px-8">
        <div className="mb-6">
          <Link
            href="/events"
            className="text-sm text-gray-500 hover:text-gray-900 inline-flex items-center gap-1"
          >
            ← Back to Events
          </Link>
        </div>

        <div className="mb-8">
          <h1 className="text-2xl font-bold text-gray-900">Host an Event</h1>
          <p className="mt-1 text-sm text-gray-500">
            Create an official event for one of your communities. Only club officers may host events.
          </p>
        </div>

        <div className="bg-white border border-gray-200 shadow-sm rounded-lg p-6 sm:p-8">
          {serverError && (
            <div className="mb-6 bg-red-50 border-l-4 border-red-500 text-red-700 p-4 rounded-md text-sm" role="alert">
              {serverError}
            </div>
          )}

          <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
            <div>
              <label htmlFor="title" className="block text-sm font-medium text-gray-700 mb-1">
                Event Title <span className="text-red-500">*</span>
              </label>
              <input
                id="title"
                type="text"
                {...register("title")}
                className={`w-full rounded-md border py-2 px-3 text-sm ${
                  errors.title ? "border-red-500" : "border-gray-300"
                }`}
              />
              {errors.title && <p className="mt-1 text-xs text-red-600">{errors.title.message}</p>}
            </div>

            <div>
              <label htmlFor="communityId" className="block text-sm font-medium text-gray-700 mb-1">
                Hosting Community <span className="text-red-500">*</span>
              </label>
              <select
                id="communityId"
                {...register("communityId")}
                className={`w-full rounded-md border py-2 px-3 text-sm ${
                  errors.communityId ? "border-red-500" : "border-gray-300"
                }`}
              >
                <option value="">Select a community...</option>
                {myCommunities.map((c) => (
                  <option key={c.id} value={c.id}>
                    {c.communityName} ({c.communityType})
                  </option>
                ))}
              </select>
              {errors.communityId && <p className="mt-1 text-xs text-red-600">{errors.communityId.message}</p>}
            </div>

            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div>
                <label htmlFor="category" className="block text-sm font-medium text-gray-700 mb-1">
                  Category <span className="text-red-500">*</span>
                </label>
                <select
                  id="category"
                  {...register("category")}
                  className="w-full rounded-md border border-gray-300 py-2 px-3 text-sm"
                >
                  {EVENT_CATEGORIES.map((c) => (
                    <option key={c.value} value={c.value}>
                      {c.label}
                    </option>
                  ))}
                </select>
              </div>

              <div>
                <label htmlFor="visibility" className="block text-sm font-medium text-gray-700 mb-1">
                  Visibility <span className="text-red-500">*</span>
                </label>
                <select
                  id="visibility"
                  {...register("visibility")}
                  className="w-full rounded-md border border-gray-300 py-2 px-3 text-sm"
                >
                  <option value="Public">Public (all students)</option>
                  <option value="Private">Private (community members only)</option>
                </select>
              </div>
            </div>

            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div>
                <label htmlFor="eventDate" className="block text-sm font-medium text-gray-700 mb-1">
                  Start Date & Time <span className="text-red-500">*</span>
                </label>
                <input
                  id="eventDate"
                  type="datetime-local"
                  {...register("eventDate")}
                  className={`w-full rounded-md border py-2 px-3 text-sm ${
                    errors.eventDate ? "border-red-500" : "border-gray-300"
                  }`}
                />
                {errors.eventDate && <p className="mt-1 text-xs text-red-600">{errors.eventDate.message}</p>}
              </div>

              <div>
                <label htmlFor="eventEndDate" className="block text-sm font-medium text-gray-700 mb-1">
                  End Date & Time <span className="text-red-500">*</span>
                </label>
                <input
                  id="eventEndDate"
                  type="datetime-local"
                  {...register("eventEndDate")}
                  className={`w-full rounded-md border py-2 px-3 text-sm ${
                    errors.eventEndDate ? "border-red-500" : "border-gray-300"
                  }`}
                />
                {errors.eventEndDate && <p className="mt-1 text-xs text-red-600">{errors.eventEndDate.message}</p>}
              </div>
            </div>

            <div>
              <label htmlFor="location" className="block text-sm font-medium text-gray-700 mb-1">
                Location <span className="text-red-500">*</span>
              </label>
              <input
                id="location"
                type="text"
                placeholder="e.g. Campus Student Union, Room 204"
                {...register("location")}
                className={`w-full rounded-md border py-2 px-3 text-sm ${
                  errors.location ? "border-red-500" : "border-gray-300"
                }`}
              />
              {errors.location && <p className="mt-1 text-xs text-red-600">{errors.location.message}</p>}
            </div>

            <div>
              <label htmlFor="description" className="block text-sm font-medium text-gray-700 mb-1">
                Description <span className="text-red-500">*</span>
              </label>
              <textarea
                id="description"
                rows={4}
                placeholder="Describe your event, agenda, and expectations..."
                {...register("description")}
                className={`w-full rounded-md border py-2 px-3 text-sm ${
                  errors.description ? "border-red-500" : "border-gray-300"
                }`}
              />
              {errors.description && <p className="mt-1 text-xs text-red-600">{errors.description.message}</p>}
            </div>

            <div className="flex justify-end gap-3 pt-4 border-t">
              <Link href="/events">
                <Button variant="outline" type="button">
                  Cancel
                </Button>
              </Link>
              <Button
                variant="primary"
                type="submit"
                disabled={isSubmitting || mutation.isPending}
              >
                {mutation.isPending ? "Creating Event..." : "Create Event"}
              </Button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
