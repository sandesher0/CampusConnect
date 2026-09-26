// Ref: workflow.md §4 Architecture Patterns | Feature: Create Event
"use client";

import { useState, Suspense } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import { eventService } from "@/services/eventService";
import { communityService } from "@/services/communityService";
import { queryKeys } from "@/services/queryKeys";
import Button from "@/components/common/Button";
import { useSearchParams } from "next/navigation";

const createEventSchema = z
  .object({
    title: z
      .string()
      .min(3, "Title must be at least 3 characters")
      .max(200, "Title must be less than 200 characters")
      .trim(),
    description: z
      .string()
      .min(10, "Description must be at least 10 characters")
      .trim(),
    location: z.string().min(2, "Location is required").trim(),
    communityId: z.string().min(1, "Please select a community"),
    eventDate: z.string().min(1, "Start date is required"),
    eventEndDate: z.string().min(1, "End date is required"),
    visibility: z.union([z.literal("Public"), z.literal("Private")]),
    category: z.union([
      z.literal("Academic"),
      z.literal("Social"),
      z.literal("Sports"),
      z.literal("Cultural"),
      z.literal("Career"),
      z.literal("Volunteering"),
      z.literal("Other"),
    ]),
  })
  .refine(
    (data) => new Date(data.eventEndDate) > new Date(data.eventDate),
    {
      message: "End date must be after start date",
      path: ["eventEndDate"],
    }
  );

type CreateEventValues = z.infer<typeof createEventSchema>;

const EVENT_CATEGORIES = [
  { value: "Academic", label: "Academic" },
  { value: "Social", label: "Social" },
  { value: "Sports", label: "Sports" },
  { value: "Cultural", label: "Cultural" },
  { value: "Career", label: "Career" },
  { value: "Volunteering", label: "Volunteering" },
  { value: "Other", label: "Other" },
] as const;

// Shared input class
const inputClass =
  "block w-full rounded-md border-0 py-2 px-3 text-gray-900 ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-gray-900 sm:text-sm sm:leading-6 shadow-sm";
const errorInputClass =
  "block w-full rounded-md border-0 py-2 px-3 text-gray-900 ring-1 ring-inset ring-red-400 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-red-500 sm:text-sm sm:leading-6 shadow-sm";

export default function CreateEventPage() {
  return (
    <Suspense fallback={<div className="py-12 text-center text-gray-500">Loading...</div>}>
      <CreateEventForm />
    </Suspense>
  );
}

function CreateEventForm() {
  const router = useRouter();
  const queryClient = useQueryClient();
  const searchParams = useSearchParams();
  const preselectedCommunityId = searchParams.get("communityId") ?? "";
  const [serverError, setServerError] = useState<string | null>(null);

  // Fetch user's communities to populate the dropdown
  const { data: myCommunities = [], isLoading: communitiesLoading } = useQuery({
    queryKey: queryKeys.communities.mine(),
    queryFn: () => communityService.getMyCommunities(),
  });

  const {
    register,
    handleSubmit,
    watch,
    formState: { errors, isSubmitting },
  } = useForm<CreateEventValues>({
    resolver: zodResolver(createEventSchema),
    defaultValues: {
      communityId: preselectedCommunityId,
      visibility: "Public",
      category: "Other",
    },
  });

  const selectedVisibility = watch("visibility");

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
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: queryKeys.events.all });
      router.push("/events");
    },
    onError: (err: any) => {
      setServerError(
        err?.response?.data?.message ||
          err?.message ||
          "Failed to create event. Please try again."
      );
    },
  });

  const onSubmit = (data: CreateEventValues) => {
    setServerError(null);
    mutation.mutate(data);
  };

  return (
    <div className="min-h-[calc(100vh-160px)] py-12">
      <div className="mx-auto max-w-2xl px-4 sm:px-6 lg:px-8">

        {/* Back nav */}
        <div className="mb-6">
          <Link
            href="/events"
            className="text-sm text-gray-500 hover:text-gray-900 inline-flex items-center gap-1"
          >
            <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
            </svg>
            Back to Events
          </Link>
        </div>

        {/* Heading */}
        <div className="mb-8">
          <h1 className="text-2xl font-bold text-gray-900">Create an Event</h1>
          <p className="mt-1 text-sm text-gray-500">
            Events are tied to a community you belong to.
          </p>
        </div>

        <div className="bg-white border border-gray-200 shadow-sm rounded-lg p-6 sm:p-8">
          {serverError && (
            <div className="mb-6 bg-red-50 border-l-4 border-red-500 text-red-700 p-4 rounded-md text-sm">
              {serverError}
            </div>
          )}

          <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">

            {/* Community */}
            <div>
              <label htmlFor="communityId" className="block text-sm font-medium text-gray-700 mb-1.5">
                Community <span className="text-red-500">*</span>
              </label>
              {communitiesLoading ? (
                <div className="h-9 bg-gray-100 animate-pulse rounded-md" />
              ) : myCommunities.length === 0 ? (
                <div className="rounded-md bg-amber-50 border border-amber-200 p-3 text-sm text-amber-800">
                  You are not a member of any community yet.{" "}
                  <Link href="/community" className="font-medium underline">
                    Browse communities
                  </Link>{" "}
                  to join one first.
                </div>
              ) : (
                <select
                  id="communityId"
                  {...register("communityId")}
                  className={errors.communityId ? errorInputClass : inputClass}
                >
                  <option value="">Select a community</option>
                  {myCommunities.map((c) => (
                    <option key={c.id} value={c.id}>
                      {c.communityName}
                    </option>
                  ))}
                </select>
              )}
              {errors.communityId && (
                <p className="mt-1.5 text-sm text-red-600">{errors.communityId.message}</p>
              )}
            </div>

            {/* Title */}
            <div>
              <label htmlFor="title" className="block text-sm font-medium text-gray-700 mb-1.5">
                Title <span className="text-red-500">*</span>
              </label>
              <input
                id="title"
                type="text"
                {...register("title")}
                placeholder="e.g. Annual Tech Talk 2025"
                className={errors.title ? errorInputClass : inputClass}
              />
              {errors.title && (
                <p className="mt-1.5 text-sm text-red-600">{errors.title.message}</p>
              )}
            </div>

            {/* Description */}
            <div>
              <label htmlFor="description" className="block text-sm font-medium text-gray-700 mb-1.5">
                Description <span className="text-red-500">*</span>
              </label>
              <textarea
                id="description"
                {...register("description")}
                rows={4}
                placeholder="Describe the event, agenda, and what attendees can expect..."
                className={`${errors.description ? errorInputClass : inputClass} resize-none`}
              />
              {errors.description && (
                <p className="mt-1.5 text-sm text-red-600">{errors.description.message}</p>
              )}
            </div>

            {/* Location */}
            <div>
              <label htmlFor="location" className="block text-sm font-medium text-gray-700 mb-1.5">
                Location <span className="text-red-500">*</span>
              </label>
              <input
                id="location"
                type="text"
                {...register("location")}
                placeholder="e.g. Main Auditorium, Room 204, or Online"
                className={errors.location ? errorInputClass : inputClass}
              />
              {errors.location && (
                <p className="mt-1.5 text-sm text-red-600">{errors.location.message}</p>
              )}
            </div>

            {/* Dates */}
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div>
                <label htmlFor="eventDate" className="block text-sm font-medium text-gray-700 mb-1.5">
                  Start <span className="text-red-500">*</span>
                </label>
                <input
                  id="eventDate"
                  type="datetime-local"
                  {...register("eventDate")}
                  className={errors.eventDate ? errorInputClass : inputClass}
                />
                {errors.eventDate && (
                  <p className="mt-1.5 text-sm text-red-600">{errors.eventDate.message}</p>
                )}
              </div>
              <div>
                <label htmlFor="eventEndDate" className="block text-sm font-medium text-gray-700 mb-1.5">
                  End <span className="text-red-500">*</span>
                </label>
                <input
                  id="eventEndDate"
                  type="datetime-local"
                  {...register("eventEndDate")}
                  className={errors.eventEndDate ? errorInputClass : inputClass}
                />
                {errors.eventEndDate && (
                  <p className="mt-1.5 text-sm text-red-600">{errors.eventEndDate.message}</p>
                )}
              </div>
            </div>

            {/* Category + Visibility */}
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div>
                <label htmlFor="category" className="block text-sm font-medium text-gray-700 mb-1.5">
                  Category <span className="text-red-500">*</span>
                </label>
                <select
                  id="category"
                  {...register("category")}
                  className={errors.category ? errorInputClass : inputClass}
                >
                  {EVENT_CATEGORIES.map((c) => (
                    <option key={c.value} value={c.value}>
                      {c.label}
                    </option>
                  ))}
                </select>
                {errors.category && (
                  <p className="mt-1.5 text-sm text-red-600">{errors.category.message}</p>
                )}
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-3">
                  Visibility <span className="text-red-500">*</span>
                </label>
                <div className="flex gap-3">
                  {(["Public", "Private"] as const).map((v) => (
                    <label
                      key={v}
                      className={`flex-1 relative flex cursor-pointer rounded-lg border p-3 shadow-sm transition-all ${
                        selectedVisibility === v
                          ? "border-gray-900 bg-gray-50 ring-1 ring-gray-900"
                          : "border-gray-200 bg-white hover:border-gray-300"
                      }`}
                    >
                      <input
                        type="radio"
                        value={v}
                        {...register("visibility")}
                        className="sr-only"
                      />
                      <span className="text-sm font-medium text-gray-900">{v}</span>
                      {selectedVisibility === v && (
                        <div className="absolute top-2 right-2">
                          <svg className="h-4 w-4 text-gray-900" viewBox="0 0 20 20" fill="currentColor">
                            <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clipRule="evenodd" />
                          </svg>
                        </div>
                      )}
                    </label>
                  ))}
                </div>
              </div>
            </div>

            {/* Actions */}
            <div className="flex items-center justify-end gap-3 pt-2 border-t border-gray-100">
              <Link href="/events">
                <Button variant="outline" type="button">
                  Cancel
                </Button>
              </Link>
              <Button
                variant="primary"
                type="submit"
                disabled={isSubmitting || mutation.isPending || myCommunities.length === 0}
              >
                {mutation.isPending ? "Creating..." : "Create Event"}
              </Button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
