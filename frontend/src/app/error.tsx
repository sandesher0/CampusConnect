"use client";

import { useEffect } from "react";
import Button from "@/components/common/Button";
import Card from "@/components/common/Card";

export default function GlobalError({
  error,
  reset,
}: {
  error: Error & { digest?: string };
  reset: () => void;
}) {
  useEffect(() => {
    // Log error if needed
    console.error("App boundary error:", error);
  }, [error]);

  return (
    <div className="min-h-[calc(100vh-160px)] flex items-center justify-center py-16 px-6 sm:px-8">
      <Card className="p-10 sm:p-14 text-center max-w-md w-full flex flex-col items-center" hoverEffect={false}>
        <div className="h-16 w-16 rounded-[20px] bg-coral-tint flex items-center justify-center text-status-warning-text mb-6 shadow-sm">
          <svg className="w-8 h-8 text-status-warning" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
            <polygon points="7.86 2 16.14 2 22 7.86 22 16.14 16.14 22 7.86 22 2 16.14 2 7.86 7.86 2" />
            <line x1="12" y1="8" x2="12" y2="12" />
            <line x1="12" y1="16" x2="12.01" y2="16" />
          </svg>
        </div>

        <h1 className="text-2xl font-[800] text-ink">Something went wrong</h1>
        <p className="mt-2 text-sm text-muted leading-relaxed">
          An unexpected error occurred. You can try refreshing or returning home.
        </p>

        <div className="mt-8 flex flex-col sm:flex-row gap-3 w-full sm:w-auto justify-center">
          <Button onClick={() => reset()} variant="primary">
            Try Again
          </Button>
          <Button href="/" variant="outline">
            Back to Home
          </Button>
        </div>
      </Card>
    </div>
  );
}
