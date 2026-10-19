// Ref: workflow.md §4 Architecture Patterns | Feature: UI Components
import React, { ReactNode } from "react";
import Card from "./Card";

interface EmptyStateProps {
  title: string;
  description: string;
  action?: ReactNode;
  icon?: ReactNode;
  className?: string;
}

export default function EmptyState({
  title,
  description,
  action,
  icon,
  className = "",
}: EmptyStateProps) {
  return (
    <Card className={`p-10 sm:p-14 text-center max-w-xl mx-auto flex flex-col items-center ${className}`} hoverEffect={false}>
      <div className="h-16 w-16 rounded-[20px] bg-primary-soft-tint flex items-center justify-center text-primary mb-5 shadow-sm">
        {icon ?? (
          <svg className="w-8 h-8 text-primary" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
            <path d="M4 19.5v-15A2.5 2.5 0 0 1 6.5 2H20v20H6.5a2.5 2.5 0 0 1-2.5-2.5Z" />
            <path d="M6 6h10" />
            <path d="M6 10h10" />
          </svg>
        )}
      </div>

      <h3 className="text-xl font-[800] text-ink">{title}</h3>
      <p className="mt-2 text-sm text-muted max-w-md leading-relaxed">{description}</p>

      {action && <div className="mt-6">{action}</div>}
    </Card>
  );
}
