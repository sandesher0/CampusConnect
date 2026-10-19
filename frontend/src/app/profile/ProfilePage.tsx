// Ref: workflow.md §4 Architecture Patterns | Feature: User Profile & Security
"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { changePasswordSchema, type ChangePasswordValues } from "@/validations/auth";
import { useAuthStore } from "@/stores/authStore";
import { authService } from "@/services/authService";
import Card from "@/components/common/Card";
import Button from "@/components/common/Button";
import Avatar from "@/components/common/Avatar";
import Field from "@/components/common/Field";
import Input from "@/components/common/Input";
import Toast from "@/components/common/Toast";
import PageHeader from "@/components/common/PageHeader";
import EmptyState from "@/components/common/EmptyState";
import { getApiErrorMessage } from "@/lib/apiError";
import { handleServerValidationErrors } from "@/utils/formErrors";
import { useRouter } from "next/navigation";

export default function ProfilePage() {
  const router = useRouter();
  const { user, logout } = useAuthStore();
  const [isLoggingOut, setIsLoggingOut] = useState(false);
  const [logoutError, setLogoutError] = useState<string | null>(null);
  const [toast, setToast] = useState<{ message: string; type: "success" | "error" } | null>(null);
  const [passwordError, setPasswordError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    setError: setFieldError,
    formState: { errors, isSubmitting },
  } = useForm<ChangePasswordValues>({
    resolver: zodResolver(changePasswordSchema),
    defaultValues: {
      oldPassword: "",
      newPassword: "",
    },
  });

  const handleLogout = async () => {
    setIsLoggingOut(true);
    setLogoutError(null);
    try {
      await logout();
      router.push("/login");
    } catch (error) {
      setLogoutError(getApiErrorMessage(error, "Unable to log out. Please try again."));
    } finally {
      setIsLoggingOut(false);
    }
  };

  const onChangePassword = async (data: ChangePasswordValues) => {
    setPasswordError(null);
    try {
      const message = await authService.changePassword(data);
      setToast({ message: message || "Password updated successfully!", type: "success" });
      reset();
    } catch (err) {
      const hasFieldErrors = handleServerValidationErrors(err, setFieldError);
      if (!hasFieldErrors) {
        setPasswordError(getApiErrorMessage(err, "Failed to update password. Please check your current password."));
      }
    }
  };

  if (!user) {
    return (
      <div className="mx-auto max-w-[1200px] px-6 py-16 sm:px-8">
        <EmptyState
          title="Sign in to view your profile"
          description="Manage your account preferences, student details, and password security."
          action={
            <Button href="/login?redirect=/profile" variant="primary">
              Log in to your account
            </Button>
          }
        />
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-4xl px-6 py-12 sm:px-8 space-y-8">
      {toast && (
        <Toast
          message={toast.message}
          type={toast.type}
          onDismiss={() => setToast(null)}
        />
      )}

      <PageHeader
        eyebrow="ACCOUNT SETTINGS"
        title="Profile"
        description="View your enrolled student profile and manage account security."
      />

      {logoutError && (
        <div className="rounded-[14px] bg-coral-tint border border-coral-border p-4 text-sm font-semibold text-status-warning-text" role="alert">
          {logoutError}
        </div>
      )}

      {/* Profile Header Card */}
      <Card className="p-6 sm:p-10 flex flex-col sm:flex-row sm:items-center justify-between gap-6" hoverEffect={false}>
        <div className="flex items-center gap-5">
          <Avatar name={user.user.firstName || user.username} size="lg" className="h-16 w-16 text-xl" />
          <div>
            <h2 className="text-2xl font-[800] text-ink">
              {user.user.firstName} {user.user.lastName}
            </h2>
            <p className="text-sm font-semibold text-muted">@{user.username}</p>
            <div className="mt-2 flex items-center gap-2">
              <span className="text-xs px-2.5 py-0.5 rounded-full bg-primary-tint text-primary-on-tint font-bold">
                Student Account
              </span>
            </div>
          </div>
        </div>

        <Button
          variant="outline"
          onClick={() => void handleLogout()}
          loading={isLoggingOut}
          disabled={isLoggingOut}
          className="text-status-warning-text hover:bg-coral-tint hover:border-coral-border"
        >
          {isLoggingOut ? "Signing out..." : "Log out"}
        </Button>
      </Card>

      {/* Account Details & Security Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 gap-8 items-start">
        {/* Read-only details */}
        <Card className="p-6 sm:p-8 space-y-4" hoverEffect={false}>
          <h3 className="text-lg font-[800] text-ink pb-2 border-b border-border-divider">
            Account Information
          </h3>

          <div className="space-y-3 text-sm">
            <div>
              <p className="text-xs font-bold text-muted">EMAIL</p>
              <p className="font-semibold text-ink mt-0.5">{user.user.email}</p>
            </div>
            <div>
              <p className="text-xs font-bold text-muted">PHONE NUMBER</p>
              <p className="font-semibold text-ink mt-0.5">{user.user.phoneNumber || "Not provided"}</p>
            </div>
            <div>
              <p className="text-xs font-bold text-muted">ACCOUNT ID</p>
              <p className="font-mono text-xs text-muted mt-0.5">{user.id}</p>
            </div>
          </div>
        </Card>

        {/* Change Password Card using Form kit */}
        <Card className="p-6 sm:p-8 space-y-5" hoverEffect={false}>
          <h3 className="text-lg font-[800] text-ink pb-2 border-b border-border-divider">
            Change Password
          </h3>

          {passwordError && (
            <div className="rounded-[14px] bg-coral-tint border border-coral-border p-3 text-xs font-semibold text-status-warning-text" role="alert">
              {passwordError}
            </div>
          )}

          <form onSubmit={handleSubmit(onChangePassword)} className="space-y-4" noValidate>
            <Field id="oldPassword" label="Current Password" required error={errors.oldPassword?.message}>
              <Input
                id="oldPassword"
                type="password"
                autoComplete="current-password"
                placeholder="Enter current password"
                hasError={Boolean(errors.oldPassword)}
                {...register("oldPassword")}
              />
            </Field>

            <Field id="newPassword" label="New Password" required error={errors.newPassword?.message}>
              <Input
                id="newPassword"
                type="password"
                autoComplete="new-password"
                placeholder="Enter new strong password"
                hasError={Boolean(errors.newPassword)}
                {...register("newPassword")}
              />
            </Field>

            <Button
              type="submit"
              variant="primary"
              className="w-full mt-2"
              loading={isSubmitting}
              disabled={isSubmitting}
            >
              {isSubmitting ? "Updating Password..." : "Update Password"}
            </Button>
          </form>
        </Card>
      </div>
    </div>
  );
}
