// Ref: workflow.md §4 Architecture Patterns | Feature: UI Components
import React, { forwardRef, InputHTMLAttributes } from "react";

export interface InputProps extends InputHTMLAttributes<HTMLInputElement> {
  hasError?: boolean;
}

const Input = forwardRef<HTMLInputElement, InputProps>(
  ({ className = "", hasError = false, disabled = false, ...props }, ref) => {
    const errorClasses = hasError
      ? "border-coral focus:outline-coral bg-coral-tint/30 text-ink"
      : "border-border-btn focus:outline-primary bg-surface text-ink hover:border-border-card-hover";

    return (
      <input
        ref={ref}
        disabled={disabled}
        className={`h-12 w-full rounded-[14px] border px-4 text-sm font-medium transition-all duration-150 placeholder:text-muted/60 disabled:opacity-50 disabled:cursor-not-allowed ${errorClasses} ${className}`}
        {...props}
      />
    );
  }
);

Input.displayName = "Input";
export default Input;
