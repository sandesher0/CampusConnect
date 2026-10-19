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
import Card from "@/components/common/Card";
import Button from "@/components/common/Button";
import Field from "@/components/common/Field";
import Input from "@/components/common/Input";
import Select from "@/components/common/Select";
import Textarea from "@/components/common/Textarea";
import Toast from "@/components/common/Toast";
import Skeleton from "@/components/common/Skeleton";
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
    <Suspense
      fallback={
        <div className="mx-auto max-w-2xl px-6 py-12 sm:px-8">
          <Skeleton className="h-10 w-48 mb-6" />
          <Skeleton className="h-96 w-full rounded-[24px]" />
        </div>
      }
    >
      <CreateEventPageContent />
    </Suspense>
  );
}

function CreateEventPageContent() {
  const router = useRouter();
  const searchParams = useSearchParams();
  const preselectedCommunityId = searchParams.get("communityId") ?? "";
  const queryClient = useQueryClient();
  const { user } = useAuthStore();
  const [serverError, setServerError] = useState<string | null>(null);
  const [toast, setToast] = useState<{ message: string; type: "success" | "error" } | null>(null);

  // Fetch communities user belongs to so they can select where to host the event
  const { data: myCommunitiesData, isLoading: isLoadingCommunities } = useQuery({
    queryKey: queryKeys.communities.mine({ page: 1, pageSize: 50 }),
    queryFn: () => communityService.getMyCommunities({ page: 1, pageSize: 50 }),
    enabled: Boolean(user),
  });

  const myCommunities = myCommunitiesData?.items ?? [];

  const {
    register,
    handleSubmit,
    setError: setFieldError,
    formState: { errors },
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
      setToast({ message: "Event created successfully!", type: "success" });
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: queryKeys.events.all() }),
      ]);
      setTimeout(() => {
        router.push("/events");
      }, 1000);
    },
    onError: (err) => {
      const hasFieldErrors = handleServerValidationErrors(err, setFieldError);
      if (!hasFieldErrors) {
        setServerError(
          getApiErrorMessage(
            err,
            "Failed to create event. You must be an authorized officer of the community to host events."
          )
        );
      }
    },
  });

  const onSubmit = (data: CreateEventValues) => {
    setServerError(null);
    mutation.mutate(data);
  };

  return (
    <div className="mx-auto max-w-2xl px-6 py-12 sm:px-8">
      {toast && (
        <Toast
          message={toast.message}
          type={toast.type}
          onDismiss={() => setToast(null)}
        />
      )}

      {/* Back button */}
      <div className="mb-6">
        <Link
          href="/events"
          className="text-sm font-bold text-muted hover:text-primary inline-flex items-center gap-1.5 transition-colors"
        >
          <svg className="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
            <polyline points="15 18 9 12 15 6" />
          </svg>
          Back to Events
        </Link>
      </div>

      {/* Header */}
      <div className="mb-8">
        <p className="text-xs font-extrabold uppercase tracking-wider text-primary mb-1">HOST AN EVENT</p>
        <h1 className="text-3xl font-[800] tracking-[-0.035em] text-ink">Create Event</h1>
        <p className="mt-1 text-sm text-muted">
          Organize an official campus event. Note: only designated community officers are eligible to create events.
        </p>
      </div>

      <Card className="p-6 sm:p-10" hoverEffect={false}>
        {serverError && (
          <div
            className="mb-6 rounded-[14px] bg-coral-tint border border-coral-border p-4 text-sm font-semibold text-status-warning-text flex items-center gap-2.5"
            role="alert"
          >
            <svg className="w-5 h-5 shrink-0 text-status-warning" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
              <circle cx="12" cy="12" r="10" />
              <line x1="12" y1="8" x2="12" y2="12" />
              <line x1="12" y1="16" x2="12.01" y2="16" />
            </svg>
            <span>{serverError}</span>
          </div>
        )}

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6" noValidate>
          {/* Title */}
          <Field id="title" label="Event Title" required error={errors.title?.message}>
            <Input
              id="title"
              type="text"
              placeholder="e.g. Annual Hackathon Kickoff"
              hasError={Boolean(errors.title)}
              {...register("title")}
            />
          </Field>

          {/* Organizing Community */}
          <Field
            id="communityId"
            label="Hosting Community"
            required
            hint="You must be a member of the community to organize an event."
            error={errors.communityId?.message}
          >
            <Select
              id="communityId"
              hasError={Boolean(errors.communityId)}
              disabled={isLoadingCommunities}
              {...register("communityId")}
            >
              <option value="">Select one of your communities...</option>
              {myCommunities.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.communityName} ({c.communityType})
                </option>
              ))}
            </Select>
          </Field>

          {/* Category & Visibility Grid */}
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <Field id="category" label="Category" required error={errors.category?.message}>
              <Select id="category" hasError={Boolean(errors.category)} {...register("category")}>
                {EVENT_CATEGORIES.map((cat) => (
                  <option key={cat.value} value={cat.value}>
                    {cat.label}
                  </option>
                ))}
              </Select>
            </Field>

            <Field id="visibility" label="Visibility" required error={errors.visibility?.message}>
              <Select id="visibility" hasError={Boolean(errors.visibility)} {...register("visibility")}>
                <option value="Public">Public (all students)</option>
                <option value="Private">Private (club members only)</option>
              </Select>
            </Field>
          </div>

          {/* Location */}
          <Field id="location" label="Location" required error={errors.location?.message}>
            <Input
              id="location"
              type="text"
              placeholder="e.g. Science Hall 102 or Courtyard"
              hasError={Boolean(errors.location)}
              {...register("location")}
            />
          </Field>

          {/* Dates Grid */}
          <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
            <Field id="eventDate" label="Starts At" required error={errors.eventDate?.message}>
              <Input
                id="eventDate"
                type="datetime-local"
                hasError={Boolean(errors.eventDate)}
                {...register("eventDate")}
              />
            </Field>

            <Field id="eventEndDate" label="Ends At" required error={errors.eventEndDate?.message}>
              <Input
                id="eventEndDate"
                type="datetime-local"
                hasError={Boolean(errors.eventEndDate)}
                {...register("eventEndDate")}
              />
            </Field>
          </div>

          {/* Description */}
          <Field id="description" label="Description" required error={errors.description?.message}>
            <Textarea
              id="description"
              rows={4}
              placeholder="Tell students about activities, speakers, or items to bring..."
              hasError={Boolean(errors.description)}
              {...register("description")}
            />
          </Field>

          {/* Action Buttons */}
          <div className="pt-4 border-t border-border-divider flex flex-col-reverse sm:flex-row items-center justify-end gap-3">
            <Button href="/events" variant="outline" className="w-full sm:w-auto">
              Cancel
            </Button>
            <Button
              type="submit"
              variant="primary"
              className="w-full sm:w-auto"
              loading={mutation.isPending}
              disabled={mutation.isPending}
            >
              {mutation.isPending ? "Publishing..." : "Publish Event"}
            </Button>
          </div>
        </form>
      </Card>
    </div>
  );
}
