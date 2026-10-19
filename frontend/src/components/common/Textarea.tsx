// Ref: workflow.md §4 Architecture Patterns | Feature: UI Components
import React, { forwardRef, TextareaHTMLAttributes } from "react";

export interface TextareaProps extends TextareaHTMLAttributes<HTMLTextAreaElement> {
  hasError?: boolean;
}

const Textarea = forwardRef<HTMLTextAreaElement, TextareaProps>(
  ({ className = "", hasError = false, disabled = false, rows = 4, ...props }, ref) => {
    const errorClasses = hasError
      ? "border-coral focus:outline-coral bg-coral-tint/30 text-ink"
      : "border-border-btn focus:outline-primary bg-surface text-ink hover:border-border-card-hover";

    return (
      <textarea
        ref={ref}
        rows={rows}
        disabled={disabled}
        className={`w-full rounded-[14px] border p-4 text-sm font-medium transition-all duration-150 placeholder:text-muted/60 disabled:opacity-50 disabled:cursor-not-allowed ${errorClasses} ${className}`}
        {...props}
      />
    );
  }
);

Textarea.displayName = "Textarea";
export default Textarea;
