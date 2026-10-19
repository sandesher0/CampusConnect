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
import Card from "@/components/common/Card";
import Button from "@/components/common/Button";
import Field from "@/components/common/Field";
import Input from "@/components/common/Input";
import Select from "@/components/common/Select";
import Toast from "@/components/common/Toast";
import { getApiErrorMessage } from "@/lib/apiError";
import { handleServerValidationErrors } from "@/utils/formErrors";
import { createCommunitySchema, type CreateCommunityValues } from "@/validations/community";

export default function CreateCommunityPage() {
  const router = useRouter();
  const queryClient = useQueryClient();
  const [serverError, setServerError] = useState<string | null>(null);
  const [toast, setToast] = useState<{ message: string; type: "success" | "error" } | null>(null);

  const {
    register,
    handleSubmit,
    setError: setFieldError,
    formState: { errors },
  } = useForm<CreateCommunityValues>({
    resolver: zodResolver(createCommunitySchema),
    defaultValues: {
      communityName: "",
      communityType: "Public",
    },
  });

  const mutation = useMutation({
    mutationFn: (data: CreateCommunityValues) => communityService.createCommunity(data),
    onSuccess: async () => {
      setToast({ message: "Community created successfully!", type: "success" });
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: queryKeys.communities.all() }),
      ]);
      setTimeout(() => {
        router.push("/community");
      }, 1000);
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
    <div className="mx-auto max-w-xl px-6 py-12 sm:px-8">
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
          href="/community"
          className="text-sm font-bold text-muted hover:text-primary inline-flex items-center gap-1.5 transition-colors"
        >
          <svg className="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
            <polyline points="15 18 9 12 15 6" />
          </svg>
          Back to Communities
        </Link>
      </div>

      {/* Header */}
      <div className="mb-8">
        <p className="text-xs font-extrabold uppercase tracking-wider text-primary mb-1">NEW ORGANIZATION</p>
        <h1 className="text-3xl font-[800] tracking-[-0.035em] text-ink">Start a Community</h1>
        <p className="mt-1 text-sm text-muted">
          Create an organization for students to join, participate in discussions, and organize events.
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
          {/* Community Name */}
          <Field id="communityName" label="Community Name" required error={errors.communityName?.message}>
            <Input
              id="communityName"
              type="text"
              placeholder="e.g. Computer Science Society"
              hasError={Boolean(errors.communityName)}
              {...register("communityName")}
            />
          </Field>

          {/* Community Type */}
          <Field
            id="communityType"
            label="Community Type"
            required
            hint="Public communities allow any enrolled student to join immediately."
            error={errors.communityType?.message}
          >
            <Select id="communityType" hasError={Boolean(errors.communityType)} {...register("communityType")}>
              <option value="Public">Public (open to all students)</option>
              <option value="Private">Private (restricted membership)</option>
            </Select>
          </Field>

          {/* Buttons */}
          <div className="pt-4 border-t border-border-divider flex flex-col-reverse sm:flex-row items-center justify-end gap-3">
            <Button href="/community" variant="outline" className="w-full sm:w-auto">
              Cancel
            </Button>
            <Button
              type="submit"
              variant="primary"
              className="w-full sm:w-auto"
              loading={mutation.isPending}
              disabled={mutation.isPending}
            >
              {mutation.isPending ? "Creating..." : "Create Community"}
            </Button>
          </div>
        </form>
      </Card>
    </div>
  );
}
