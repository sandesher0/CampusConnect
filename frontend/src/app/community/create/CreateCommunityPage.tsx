// Ref: workflow.md §4 Architecture Patterns | Feature: Create Community
"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { z } from "zod";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { communityService } from "@/services/communityService";
import Button from "@/components/common/Button";

const createCommunitySchema = z.object({
  communityName: z
    .string()
    .min(3, "Community name must be at least 3 characters")
    .max(100, "Community name must be less than 100 characters")
    .trim(),
  communityType: z.union([z.literal("Public"), z.literal("Private")], {
    error: "Please select a community type",
  }),
});

type CreateCommunityValues = z.infer<typeof createCommunitySchema>;

export default function CreateCommunityPage() {
  const router = useRouter();
  const queryClient = useQueryClient();
  const [serverError, setServerError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    watch,
    formState: { errors, isSubmitting },
  } = useForm<CreateCommunityValues>({
    resolver: zodResolver(createCommunitySchema),
    defaultValues: {
      communityName: "",
      communityType: "Public",
    },
  });

  const selectedType = watch("communityType");

  const mutation = useMutation({
    mutationFn: (data: CreateCommunityValues) =>
      communityService.createCommunity({
        communityName: data.communityName,
        communityType: data.communityType,
      }),
    onSuccess: () => {
      // Invalidate the communities list so it re-fetches
      queryClient.invalidateQueries({ queryKey: ["communities"] });
      router.push("/community");
    },
    onError: (err: any) => {
      setServerError(
        err?.response?.data?.message || err?.message || "Failed to create community. Please try again."
      );
    },
  });

  const onSubmit = (data: CreateCommunityValues) => {
    setServerError(null);
    mutation.mutate(data);
  };

  return (
    <div className="min-h-[calc(100vh-160px)] py-12">
      <div className="mx-auto max-w-xl px-4 sm:px-6 lg:px-8">

        {/* Back nav */}
        <div className="mb-6">
          <Link
            href="/community"
            className="text-sm text-gray-500 hover:text-gray-900 inline-flex items-center gap-1"
          >
            <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
            </svg>
            Back to Communities
          </Link>
        </div>

        {/* Heading */}
        <div className="mb-8">
          <h1 className="text-2xl font-bold text-gray-900">Create a Community</h1>
          <p className="mt-1 text-sm text-gray-500">
            Start a new group for students with shared interests.
          </p>
        </div>

        {/* Form card */}
        <div className="bg-white border border-gray-200 shadow-sm rounded-lg p-6 sm:p-8">
          {serverError && (
            <div className="mb-6 bg-red-50 border-l-4 border-red-500 text-red-700 p-4 rounded-md text-sm">
              {serverError}
            </div>
          )}

          <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
            {/* Community Name */}
            <div>
              <label
                htmlFor="communityName"
                className="block text-sm font-medium text-gray-700 mb-1.5"
              >
                Community name <span className="text-red-500">*</span>
              </label>
              <input
                id="communityName"
                type="text"
                {...register("communityName")}
                placeholder="e.g. Photography Club, Debate Society"
                className={`block w-full rounded-md border-0 py-2 px-3 text-gray-900 ring-1 ring-inset placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-gray-900 sm:text-sm sm:leading-6 shadow-sm ${
                  errors.communityName
                    ? "ring-red-400 focus:ring-red-500"
                    : "ring-gray-300"
                }`}
              />
              {errors.communityName && (
                <p className="mt-1.5 text-sm text-red-600">
                  {errors.communityName.message}
                </p>
              )}
            </div>

            {/* Community Type */}
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-3">
                Visibility <span className="text-red-500">*</span>
              </label>
              <div className="grid grid-cols-2 gap-3">
                {/* Public option */}
                <label
                  className={`relative flex cursor-pointer rounded-lg border p-4 shadow-sm transition-all ${
                    selectedType === "Public"
                      ? "border-gray-900 bg-gray-50 ring-1 ring-gray-900"
                      : "border-gray-200 bg-white hover:border-gray-300"
                  }`}
                >
                  <input
                    type="radio"
                    value="Public"
                    {...register("communityType")}
                    className="sr-only"
                  />
                  <div className="flex flex-col gap-1">
                    <div className="flex items-center gap-2">
                      <svg className="h-4 w-4 text-gray-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3.055 11H5a2 2 0 012 2v1a2 2 0 002 2 2 2 0 012 2v2.945M8 3.935V5.5A2.5 2.5 0 0010.5 8h.5a2 2 0 012 2 2 2 0 104 0 2 2 0 012-2h1.064M15 20.488V18a2 2 0 012-2h3.064" />
                      </svg>
                      <span className="text-sm font-medium text-gray-900">Public</span>
                    </div>
                    <p className="text-xs text-gray-500">Anyone can see and join</p>
                  </div>
                  {selectedType === "Public" && (
                    <div className="absolute top-3 right-3">
                      <svg className="h-4 w-4 text-gray-900" viewBox="0 0 20 20" fill="currentColor">
                        <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clipRule="evenodd" />
                      </svg>
                    </div>
                  )}
                </label>

                {/* Private option */}
                <label
                  className={`relative flex cursor-pointer rounded-lg border p-4 shadow-sm transition-all ${
                    selectedType === "Private"
                      ? "border-gray-900 bg-gray-50 ring-1 ring-gray-900"
                      : "border-gray-200 bg-white hover:border-gray-300"
                  }`}
                >
                  <input
                    type="radio"
                    value="Private"
                    {...register("communityType")}
                    className="sr-only"
                  />
                  <div className="flex flex-col gap-1">
                    <div className="flex items-center gap-2">
                      <svg className="h-4 w-4 text-gray-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z" />
                      </svg>
                      <span className="text-sm font-medium text-gray-900">Private</span>
                    </div>
                    <p className="text-xs text-gray-500">Invite-only membership</p>
                  </div>
                  {selectedType === "Private" && (
                    <div className="absolute top-3 right-3">
                      <svg className="h-4 w-4 text-gray-900" viewBox="0 0 20 20" fill="currentColor">
                        <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z" clipRule="evenodd" />
                      </svg>
                    </div>
                  )}
                </label>
              </div>
              {errors.communityType && (
                <p className="mt-1.5 text-sm text-red-600">
                  {errors.communityType.message}
                </p>
              )}
            </div>

            {/* Actions */}
            <div className="flex items-center justify-end gap-3 pt-2">
              <Link href="/community">
                <Button variant="outline" type="button">
                  Cancel
                </Button>
              </Link>
              <Button
                variant="primary"
                type="submit"
                disabled={isSubmitting || mutation.isPending}
              >
                {mutation.isPending ? "Creating..." : "Create Community"}
              </Button>
            </div>
          </form>
        </div>
      </div>
    </div>
  );
}
