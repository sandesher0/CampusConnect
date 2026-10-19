// Ref: workflow.md §4 Architecture Patterns | Feature: Auth Flow
"use client";

import { Suspense, useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { loginSchema, type LoginValues } from "@/validations/auth";
import { useAuthStore } from "@/stores/authStore";
import { authService } from "@/services/authService";
import Link from "next/link";
import { useRouter, useSearchParams } from "next/navigation";
import Button from "@/components/common/Button";
import { getApiErrorMessage } from "@/lib/apiError";
import { handleServerValidationErrors } from "@/utils/formErrors";

export default function LoginPage() {
  return (
    <Suspense fallback={<div className="min-h-screen flex items-center justify-center bg-gray-50">Loading...</div>}>
      <LoginPageInner />
    </Suspense>
  );
}

function LoginPageInner() {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const { login: storeLogin, setLoading: setStoreLoading } = useAuthStore();
  const router = useRouter();
  const searchParams = useSearchParams();
  const redirect = searchParams.get("redirect") || "/";

  const {
    register,
    handleSubmit,
    setError: setFieldError,
    formState: { errors },
  } = useForm<LoginValues>({
    resolver: zodResolver(loginSchema),
    defaultValues: {
      username: "",
      password: "",
    },
  });

  const onSubmit = async (data: LoginValues) => {
    setIsSubmitting(true);
    setError(null);
    setStoreLoading(true);

    try {
      const user = await authService.login(data);
      storeLogin(user);
      router.push(redirect);
    } catch (err) {
      const hasFieldErrors = handleServerValidationErrors(err, setFieldError);
      if (!hasFieldErrors) {
        setError(getApiErrorMessage(err, "Login failed. Please check your credentials."));
      }
    } finally {
      setIsSubmitting(false);
      setStoreLoading(false);
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50">
      <div className="w-full max-w-md space-y-6 p-4">
        <div className="space-y-4 text-center">
          <h2 className="text-2xl font-bold text-gray-900">
            Welcome Back
          </h2>
          <p className="text-gray-600">
            Sign in to your CampusConnect account
          </p>
        </div>

        {error && (
          <div className="bg-red-50 border-l-4 border-red-500 text-red-700 p-4 rounded-md text-sm" role="alert">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
          <div>
            <label
              htmlFor="username"
              className="block text-sm font-medium text-gray-700"
            >
              Username
            </label>
            <input
              id="username"
              type="text"
              {...register("username")}
              className={`mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border ${
                errors.username ? "border-red-500" : ""
              }`}
            />
            {errors.username && (
              <p className="mt-1 text-sm text-red-600">
                {errors.username.message}
              </p>
            )}
          </div>

          <div>
            <label
              htmlFor="password"
              className="block text-sm font-medium text-gray-700"
            >
              Password
            </label>
            <input
              id="password"
              type="password"
              {...register("password")}
              className={`mt-1 block w-full rounded-md border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500 sm:text-sm p-2 border ${
                errors.password ? "border-red-500" : ""
              }`}
            />
            {errors.password && (
              <p className="mt-1 text-sm text-red-600">
                {errors.password.message}
              </p>
            )}
          </div>

          <Button
            type="submit"
            className="w-full"
            disabled={isSubmitting}
          >
            {isSubmitting ? "Signing in..." : "Sign In"}
          </Button>

          <div className="text-center text-sm text-gray-600">
            Don&apos;t have an account?{" "}
            <Link
              href="/register"
              className="font-medium text-blue-600 hover:text-blue-500"
            >
              Register here
            </Link>
          </div>
        </form>
      </div>
    </div>
  );
}
