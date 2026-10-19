// Ref: workflow.md §4 Architecture Patterns | Feature: UI Components
import React, { forwardRef, SelectHTMLAttributes } from "react";

export interface SelectProps extends SelectHTMLAttributes<HTMLSelectElement> {
  hasError?: boolean;
}

const Select = forwardRef<HTMLSelectElement, SelectProps>(
  ({ className = "", hasError = false, disabled = false, children, ...props }, ref) => {
    const errorClasses = hasError
      ? "border-coral focus:outline-coral bg-coral-tint/30 text-ink"
      : "border-border-btn focus:outline-primary bg-surface text-ink hover:border-border-card-hover";

    return (
      <div className="relative w-full">
        <select
          ref={ref}
          disabled={disabled}
          className={`h-12 w-full appearance-none rounded-[14px] border px-4 pr-10 text-sm font-medium transition-all duration-150 disabled:opacity-50 disabled:cursor-not-allowed cursor-pointer ${errorClasses} ${className}`}
          {...props}
        >
          {children}
        </select>
        <div className="pointer-events-none absolute right-3.5 top-1/2 -translate-y-1/2 text-muted">
          <svg className="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
            <polyline points="6 9 12 15 18 9" />
          </svg>
        </div>
      </div>
    );
  }
);

Select.displayName = "Select";
export default Select;
