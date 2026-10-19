// Ref: workflow.md §4 Architecture Patterns | Feature: Create Community
"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { useRouter } from "next/navigation";
import Link from "next/link";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { communityService } from "@/services/communityService";
import { queryKeys } from "@/services/queryKeys";
import Button from "@/components/common/Button";
import { getApiErrorMessage } from "@/lib/apiError";
import { handleServerValidationErrors } from "@/utils/formErrors";
import { createCommunitySchema, type CreateCommunityValues } from "@/validations/community";

export default function CreateCommunityPage() {
  const router = useRouter();
  const queryClient = useQueryClient();
  const [serverError, setServerError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    watch,
    setError: setFieldError,
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
      communityService.createCommunity(data),
    onSuccess: async () => {
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: queryKeys.communities.all() }),
      ]);
      router.push("/community");
    },
    onError: (err) => {
      const hasFieldErrors = handleServerValidationErrors(err, setFieldError);
      if (!hasFieldErrors) {
        setServerError(getApiErrorMessage(err, "Failed to create community. Please try again."));
      }
    },
  });

  const onSubmit = (data: CreateCommunityValues) => {
    setServerError(null);
    mutation.mutate(data);
  };

  return (
    <div className="min-h-[calc(100vh-160px)] py-12">
      <div className="mx-auto max-w-xl px-4 sm:px-6 lg:px-8">
        <div className="mb-6">
          <Link
            href="/community"
            className="text-sm text-gray-500 hover:text-gray-900 inline-flex items-center gap-1"
          >
            ← Back to Communities
          </Link>
        </div>

        <div className="mb-8">
          <h1 className="text-2xl font-bold text-gray-900">Create a Community</h1>
          <p className="mt-1 text-sm text-gray-500">
            Start a new group for students with shared interests.
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

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-3">
                Visibility <span className="text-red-500">*</span>
              </label>
              <div className="grid grid-cols-2 gap-3">
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
                    <span className="text-sm font-medium text-gray-900">Public</span>
                    <p className="text-xs text-gray-500">Anyone can see and join</p>
                  </div>
                </label>

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
                    <span className="text-sm font-medium text-gray-900">Private</span>
                    <p className="text-xs text-gray-500">Invite-only membership</p>
                  </div>
                </label>
              </div>
              {errors.communityType && (
                <p className="mt-1.5 text-sm text-red-600">
                  {errors.communityType.message}
                </p>
              )}
            </div>

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
