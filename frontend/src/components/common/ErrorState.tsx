// Ref: workflow.md §4 Architecture Patterns | Feature: UI Components
import React from "react";
import Card from "./Card";
import Button from "./Button";
import { toApiError } from "@/lib/apiError";

interface ErrorStateProps {
  error: unknown;
  onRetry?: () => void;
  fallbackMessage?: string;
  className?: string;
}

export default function ErrorState({
  error,
  onRetry,
  fallbackMessage = "Something went wrong while loading this data.",
  className = "",
}: ErrorStateProps) {
  let title = "Unable to load";
  let message = fallbackMessage;
  let isForbidden = false;
  let isNotFound = false;

  if (error) {
    const apiError = toApiError(error);
    if (apiError.status === 403) {
      isForbidden = true;
      title = "Access restricted";
      message = apiError.detail || "You do not have permission to view this content or attendee list.";
    } else if (apiError.status === 404) {
      isNotFound = true;
      title = "Not found";
      message = apiError.detail || "The requested item could not be found.";
    } else if (apiError.status === 0 || apiError.title === "Network Error") {
      title = "Connection issue";
      message = "Unable to reach the server. The backend may be starting up, please try again in a moment.";
    } else {
      message = apiError.detail || apiError.message || fallbackMessage;
    }
  }

  return (
    <Card className={`p-8 sm:p-12 text-center max-w-lg mx-auto flex flex-col items-center ${className}`} hoverEffect={false}>
      <div
        className={`h-14 w-14 rounded-2xl flex items-center justify-center mb-4 ${
          isForbidden ? "bg-coral-tint text-status-warning-text" : "bg-primary-soft-tint text-primary"
        }`}
      >
        {isForbidden ? (
          <svg className="w-7 h-7" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
            <rect width="18" height="11" x="3" y="11" rx="2" ry="2" />
            <path d="M7 11V7a5 5 0 0 1 10 0v4" />
          </svg>
        ) : isNotFound ? (
          <svg className="w-7 h-7" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
            <circle cx="12" cy="12" r="10" />
            <line x1="12" y1="8" x2="12" y2="12" />
            <line x1="12" y1="16" x2="12.01" y2="16" />
          </svg>
        ) : (
          <svg className="w-7 h-7" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
            <path d="m21.73 18-8-14a2 2 0 0 0-3.48 0l-8 14A2 2 0 0 0 4 21h16a2 2 0 0 0 1.73-3Z" />
            <line x1="12" y1="9" x2="12" y2="13" />
            <line x1="12" y1="17" x2="12.01" y2="17" />
          </svg>
        )}
      </div>

      <h3 className="text-xl font-[800] text-ink">{title}</h3>
      <p className="mt-2 text-sm text-muted leading-relaxed max-w-sm">{message}</p>

      {onRetry && (
        <div className="mt-6 flex justify-center">
          <Button onClick={onRetry} variant="primary" size="sm">
            Try again
          </Button>
        </div>
      )}
    </Card>
  );
}
