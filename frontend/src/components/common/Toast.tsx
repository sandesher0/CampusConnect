// Ref: workflow.md §4 Architecture Patterns | Feature: UI Components
import React, { useEffect } from "react";

export interface ToastProps {
  message: string;
  type?: "success" | "error" | "info";
  onDismiss: () => void;
  duration?: number;
}

export default function Toast({
  message,
  type = "success",
  onDismiss,
  duration = 4000,
}: ToastProps) {
  useEffect(() => {
    if (!message) return;
    const timer = setTimeout(() => {
      onDismiss();
    }, duration);
    return () => clearTimeout(timer);
  }, [message, duration, onDismiss]);

  if (!message) return null;

  const typeStyles = {
    success: "bg-surface border-border-card text-ink shadow-[0_10px_24px_rgba(77,52,200,0.12)]",
    error: "bg-coral-tint border-coral-border text-status-warning-text shadow-sm",
    info: "bg-primary-tint border-border-tint text-primary-on-tint shadow-sm",
  };

  return (
    <div
      role="status"
      aria-live="polite"
      className="fixed bottom-6 right-6 z-50 flex items-center gap-3 rounded-full border px-5 py-3.5 animate-rise max-w-md backdrop-blur-sm select-none"
    >
      <div className={`flex items-center gap-2.5 text-sm font-bold ${typeStyles[type]}`}>
        {type === "success" && (
          <span className="h-6 w-6 rounded-full bg-status-good-bg text-status-good-text flex items-center justify-center shrink-0">
            <svg className="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="3" strokeLinecap="round" strokeLinejoin="round">
              <polyline points="20 6 9 17 4 12" />
            </svg>
          </span>
        )}
        {type === "error" && (
          <span className="h-6 w-6 rounded-full bg-status-warning-bg text-status-warning-text flex items-center justify-center shrink-0">
            <svg className="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
              <line x1="18" y1="6" x2="6" y2="18" />
              <line x1="6" y1="6" x2="18" y2="18" />
            </svg>
          </span>
        )}
        <span>{message}</span>
        <button
          type="button"
          onClick={onDismiss}
          className="ml-2 text-muted hover:text-ink cursor-pointer"
          aria-label="Dismiss notification"
        >
          <svg className="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
            <line x1="18" y1="6" x2="6" y2="18" />
            <line x1="6" y1="6" x2="18" y2="18" />
          </svg>
        </button>
      </div>
    </div>
  );
}
