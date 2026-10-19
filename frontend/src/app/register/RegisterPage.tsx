// Ref: workflow.md §4 Architecture Patterns | Feature: Auth Flow
"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { registerSchema, type RegisterValues } from "@/validations/auth";
import { useAuthStore } from "@/stores/authStore";
import { authService } from "@/services/authService";
import Link from "next/link";
import { useRouter } from "next/navigation";
import Card from "@/components/common/Card";
import Button from "@/components/common/Button";
import Field from "@/components/common/Field";
import Input from "@/components/common/Input";
import { getApiErrorMessage } from "@/lib/apiError";
import { handleServerValidationErrors } from "@/utils/formErrors";

export default function RegisterPage() {
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const { login: storeLogin, setLoading: setStoreLoading } = useAuthStore();
  const router = useRouter();

  const {
    register,
    handleSubmit,
    watch,
    setError: setFieldError,
    formState: { errors },
  } = useForm<RegisterValues>({
    resolver: zodResolver(registerSchema),
    defaultValues: {
      username: "",
      firstName: "",
      lastName: "",
      email: "",
      password: "",
      phoneNumber: "",
    },
  });

  const passwordValue = watch("password") || "";

  // Live password rules checklist
  const passwordRules = [
    { label: "At least 8 characters", met: passwordValue.length >= 8 },
    { label: "One lowercase letter", met: /[a-z]/.test(passwordValue) },
    { label: "One uppercase letter", met: /[A-Z]/.test(passwordValue) },
    { label: "One number", met: /[0-9]/.test(passwordValue) },
    { label: "One special character", met: /[^a-zA-Z0-9]/.test(passwordValue) },
  ];

  const onSubmit = async (data: RegisterValues) => {
    setIsSubmitting(true);
    setError(null);
    setStoreLoading(true);

    try {
      const user = await authService.register({
        username: data.username,
        firstName: data.firstName,
        lastName: data.lastName,
        email: data.email,
        password: data.password,
        phoneNumber: data.phoneNumber ? data.phoneNumber : null,
      });
      storeLogin(user);
      router.push("/");
    } catch (err) {
      const hasFieldErrors = handleServerValidationErrors(err, setFieldError);
      if (!hasFieldErrors) {
        setError(getApiErrorMessage(err, "Registration failed. Please check your information and try again."));
      }
    } finally {
      setIsSubmitting(false);
      setStoreLoading(false);
    }
  };

  return (
    <div className="min-h-[calc(100vh-160px)] flex items-center justify-center py-12 px-6 sm:px-8">
      <div className="w-full max-w-[480px]">
        {/* Top Logo Mark */}
        <div className="flex flex-col items-center mb-8 text-center">
          <Link href="/" className="inline-flex flex-col items-center gap-3 group">
            <div className="h-12 w-12 rounded-[14px] bg-primary flex items-center justify-center shadow-[0_6px_18px_rgba(109,74,255,0.35)] group-hover:scale-105 transition-transform">
              <svg className="w-7 h-7 text-white" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="3" strokeLinecap="round">
                <path d="M16 8a5.5 5.5 0 1 0 0 8" />
              </svg>
            </div>
            <span className="text-2xl font-[800] tracking-[-0.035em] text-ink">
              CampusConnect
            </span>
          </Link>
          <p className="mt-2 text-sm text-muted font-medium">
            Join the student directory for your campus.
          </p>
        </div>

        <Card size="large" className="p-7 sm:p-9" hoverEffect={false}>
          {error && (
            <div
              className="mb-6 rounded-[14px] bg-coral-tint border border-coral-border p-4 text-sm font-semibold text-status-warning-text flex items-center gap-2.5"
              role="alert"
            >
              <svg className="w-5 h-5 shrink-0 text-status-warning" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
                <circle cx="12" cy="12" r="10" />
                <line x1="12" y1="8" x2="12" y2="12" />
                <line x1="12" y1="16" x2="12.01" y2="16" />
              </svg>
              <span>{error}</span>
            </div>
          )}

          <form onSubmit={handleSubmit(onSubmit)} className="space-y-4" noValidate>
            {/* Username */}
            <Field id="username" label="Username" required error={errors.username?.message}>
              <Input
                id="username"
                type="text"
                autoComplete="username"
                placeholder="Choose a username"
                hasError={Boolean(errors.username)}
                {...register("username")}
              />
            </Field>

            {/* Names Grid */}
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <Field id="firstName" label="First Name" required error={errors.firstName?.message}>
                <Input
                  id="firstName"
                  type="text"
                  autoComplete="given-name"
                  placeholder="First name"
                  hasError={Boolean(errors.firstName)}
                  {...register("firstName")}
                />
              </Field>

              <Field id="lastName" label="Last Name" required error={errors.lastName?.message}>
                <Input
                  id="lastName"
                  type="text"
                  autoComplete="family-name"
                  placeholder="Last name"
                  hasError={Boolean(errors.lastName)}
                  {...register("lastName")}
                />
              </Field>
            </div>

            {/* Email */}
            <Field id="email" label="Student Email" required error={errors.email?.message}>
              <Input
                id="email"
                type="email"
                autoComplete="email"
                placeholder="student@university.edu"
                hasError={Boolean(errors.email)}
                {...register("email")}
              />
            </Field>

            {/* Phone (optional) */}
            <Field id="phoneNumber" label="Phone (optional)" error={errors.phoneNumber?.message}>
              <Input
                id="phoneNumber"
                type="tel"
                autoComplete="tel"
                placeholder="(555) 000-0000"
                hasError={Boolean(errors.phoneNumber)}
                {...register("phoneNumber")}
              />
            </Field>

            {/* Password with Show/Hide toggle */}
            <Field id="password" label="Password" required error={errors.password?.message}>
              <div className="relative">
                <Input
                  id="password"
                  type={showPassword ? "text" : "password"}
                  autoComplete="new-password"
                  placeholder="Create a strong password"
                  hasError={Boolean(errors.password)}
                  className="pr-12"
                  {...register("password")}
                />
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  aria-label={showPassword ? "Hide password" : "Show password"}
                  aria-pressed={showPassword}
                  className="absolute right-3.5 top-1/2 -translate-y-1/2 text-muted hover:text-ink cursor-pointer p-1"
                >
                  {showPassword ? (
                    <svg className="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
                      <path d="M9.88 9.88a3 3 0 1 0 4.24 4.24" />
                      <path d="M10.73 5.08A10.43 10.43 0 0 1 12 5c7 0 10 7 10 7a13.16 13.16 0 0 1-1.67 2.68" />
                      <path d="M6.61 6.61A13.526 13.526 0 0 0 2 12s3 7 10 7a9.74 9.74 0 0 0 5.39-1.61" />
                      <line x1="2" y1="2" x2="22" y2="22" />
                    </svg>
                  ) : (
                    <svg className="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
                      <path d="M2 12s3-7 10-7 10 7 10 7-3 7-10 7-10-7-10-7Z" />
                      <circle cx="12" cy="12" r="3" />
                    </svg>
                  )}
                </button>
              </div>
            </Field>

            {/* Live Password Checklist */}
            <div className="p-3.5 rounded-[14px] bg-page border border-border-card space-y-1.5" aria-label="Password requirements">
              <p className="text-xs font-bold text-muted mb-1 select-none">Password requirements:</p>
              {passwordRules.map((rule, idx) => (
                <div key={idx} className="flex items-center gap-2 text-xs font-semibold">
                  <span
                    className={`h-4 w-4 rounded-full flex items-center justify-center shrink-0 ${
                      rule.met ? "bg-status-good-bg text-status-good-text" : "bg-border-btn text-muted"
                    }`}
                  >
                    {rule.met ? (
                      <svg className="w-3 h-3" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="3" strokeLinecap="round" strokeLinejoin="round">
                        <polyline points="20 6 9 17 4 12" />
                      </svg>
                    ) : (
                      <span className="h-1.5 w-1.5 rounded-full bg-muted/60" />
                    )}
                  </span>
                  <span className={rule.met ? "text-status-good-text" : "text-muted"}>
                    {rule.label}
                  </span>
                </div>
              ))}
            </div>

            <Button
              type="submit"
              variant="primary"
              size="lg"
              className="w-full mt-4"
              loading={isSubmitting}
              disabled={isSubmitting}
            >
              {isSubmitting ? "Creating account..." : "Sign Up"}
            </Button>
          </form>

          <div className="mt-8 pt-6 border-t border-border-divider text-center">
            <p className="text-sm text-muted font-medium">
              Already have an account?{" "}
              <Link
                href="/login"
                className="font-bold text-primary hover:underline focus-visible:outline-none"
              >
                Sign in
              </Link>
            </p>
          </div>
        </Card>
      </div>
    </div>
  );
}
