// Ref: workflow.md §4 Architecture Patterns | Feature: UI Components
import React, { ReactNode } from "react";

interface FieldProps {
  id: string;
  label: string;
  error?: string;
  hint?: string;
  required?: boolean;
  children: ReactNode;
  className?: string;
}

export default function Field({
  id,
  label,
  error,
  hint,
  required = false,
  children,
  className = "",
}: FieldProps) {
  const errorId = `${id}-error`;
  const hintId = `${id}-hint`;

  return (
    <div className={`flex flex-col gap-1.5 ${className}`}>
      <div className="flex items-center justify-between">
        <label htmlFor={id} className="text-sm font-bold text-ink select-none">
          {label} {required && <span className="text-coral" aria-hidden="true">*</span>}
        </label>
      </div>

      {children}

      {hint && !error && (
        <p id={hintId} className="text-xs text-muted">
          {hint}
        </p>
      )}

      {error && (
        <div id={errorId} className="flex items-center gap-1.5 text-xs font-semibold text-status-warning-text" role="alert">
          <svg className="w-4 h-4 shrink-0 text-status-warning" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
            <circle cx="12" cy="12" r="10" />
            <line x1="12" y1="8" x2="12" y2="12" />
            <line x1="12" y1="16" x2="12.01" y2="16" />
          </svg>
          <span>{error}</span>
        </div>
      )}
    </div>
  );
}
