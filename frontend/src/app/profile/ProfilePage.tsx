// Ref: workflow.md §4 Architecture Patterns | Feature: User Profile & Security
"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { changePasswordSchema, type ChangePasswordValues } from "@/validations/auth";
import { useAuthStore } from "@/stores/authStore";
import { authService } from "@/services/authService";
import Button from "@/components/common/Button";
import { getApiErrorMessage } from "@/lib/apiError";
import { handleServerValidationErrors } from "@/utils/formErrors";
import Link from "next/link";
import { useRouter } from "next/navigation";

export default function ProfilePage() {
  const router = useRouter();
  const { user, logout } = useAuthStore();
  const [isLoggingOut, setIsLoggingOut] = useState(false);
  const [logoutError, setLogoutError] = useState<string | null>(null);
  const [passwordSuccess, setPasswordSuccess] = useState<string | null>(null);
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
    setPasswordSuccess(null);
    setPasswordError(null);
    try {
      const message = await authService.changePassword(data);
      setPasswordSuccess(message || "Password changed successfully.");
      reset();
    } catch (err) {
      const hasFieldErrors = handleServerValidationErrors(err, setFieldError);
      if (!hasFieldErrors) {
        setPasswordError(getApiErrorMessage(err, "Failed to change password. Check your old password."));
      }
    }
  };

  if (!user) {
    return (
      <div className="min-h-[calc(100vh-160px)] flex flex-col items-center justify-center py-12">
        <p className="text-gray-500 mb-4">Please sign in to view your profile.</p>
        <Link href="/login">
          <Button variant="primary">Sign In</Button>
        </Link>
      </div>
    );
  }

  return (
    <div className="min-h-[calc(100vh-160px)] py-12">
      <div className="mx-auto max-w-4xl px-4 sm:px-6 lg:px-8 space-y-8">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">Account Profile</h1>
          <p className="mt-1 text-sm text-gray-500">
            View your verified student profile and manage your security settings.
          </p>
        </div>

        {logoutError && (
          <div className="bg-red-50 border-l-4 border-red-500 text-red-700 p-4 rounded-md text-sm" role="alert">
            {logoutError}
          </div>
        )}

        {/* Profile Information (Read-only from Backend AccountInfo) */}
        <div className="bg-white border border-gray-200 shadow-sm rounded-lg p-6 sm:p-8">
          <div className="flex items-center justify-between border-b pb-4 mb-6">
            <h2 className="text-lg font-semibold text-gray-900">Profile Information</h2>
            <span className="text-xs bg-gray-100 text-gray-600 px-2.5 py-1 rounded-full">
              Verified Student Account
            </span>
          </div>

          <div className="grid grid-cols-1 sm:grid-cols-2 gap-6">
            <div>
              <p className="text-xs font-medium text-gray-500">Username</p>
              <p className="mt-1 text-sm font-semibold text-gray-900">{user.username}</p>
            </div>

            <div>
              <p className="text-xs font-medium text-gray-500">Email Address</p>
              <p className="mt-1 text-sm font-semibold text-gray-900">{user.user.email}</p>
            </div>

            <div>
              <p className="text-xs font-medium text-gray-500">First Name</p>
              <p className="mt-1 text-sm font-semibold text-gray-900">{user.user.firstName}</p>
            </div>

            <div>
              <p className="text-xs font-medium text-gray-500">Last Name</p>
              <p className="mt-1 text-sm font-semibold text-gray-900">{user.user.lastName}</p>
            </div>

            <div>
              <p className="text-xs font-medium text-gray-500">Phone Number</p>
              <p className="mt-1 text-sm font-semibold text-gray-900">{user.user.phoneNumber || "Not provided"}</p>
            </div>

            <div>
              <p className="text-xs font-medium text-gray-500">Account Verification</p>
              <p className="mt-1 text-sm font-semibold text-gray-900">
                {user.accountVerifiedAt ? new Date(user.accountVerifiedAt).toLocaleDateString() : "Active"}
              </p>
            </div>
          </div>
        </div>

        {/* Change Password Card */}
        <div className="bg-white border border-gray-200 shadow-sm rounded-lg p-6 sm:p-8">
          <h2 className="text-lg font-semibold text-gray-900 border-b pb-4 mb-6">
            Change Password
          </h2>

          {passwordSuccess && (
            <div className="mb-4 bg-green-50 border-l-4 border-green-500 text-green-700 p-4 rounded-md text-sm" role="status">
              {passwordSuccess}
            </div>
          )}

          {passwordError && (
            <div className="mb-4 bg-red-50 border-l-4 border-red-500 text-red-700 p-4 rounded-md text-sm" role="alert">
              {passwordError}
            </div>
          )}

          <form onSubmit={handleSubmit(onChangePassword)} className="space-y-4 max-w-lg">
            <div>
              <label
                htmlFor="oldPassword"
                className="block text-sm font-medium text-gray-700"
              >
                Current Password
              </label>
              <input
                id="oldPassword"
                type="password"
                {...register("oldPassword")}
                className={`mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border ${
                  errors.oldPassword ? "border-red-500" : ""
                }`}
              />
              {errors.oldPassword && (
                <p className="mt-1 text-sm text-red-600">{errors.oldPassword.message}</p>
              )}
            </div>

            <div>
              <label
                htmlFor="newPassword"
                className="block text-sm font-medium text-gray-700"
              >
                New Password
              </label>
              <input
                id="newPassword"
                type="password"
                {...register("newPassword")}
                className={`mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border ${
                  errors.newPassword ? "border-red-500" : ""
                }`}
              />
              {errors.newPassword && (
                <p className="mt-1 text-sm text-red-600">{errors.newPassword.message}</p>
              )}
              <p className="mt-1 text-xs text-gray-500">
                Must be at least 8 characters with uppercase, lowercase, number, and special character.
              </p>
            </div>

            <Button
              type="submit"
              variant="primary"
              disabled={isSubmitting}
            >
              {isSubmitting ? "Updating Password..." : "Update Password"}
            </Button>
          </form>
        </div>

        {/* Account Actions */}
        <div className="flex justify-end pt-4">
          <Button
            type="button"
            variant="outline"
            className="text-red-600 border-red-300 hover:bg-red-50"
            disabled={isLoggingOut}
            onClick={handleLogout}
          >
            {isLoggingOut ? "Signing Out..." : "Sign Out of CampusConnect"}
          </Button>
        </div>
      </div>
    </div>
  );
}
