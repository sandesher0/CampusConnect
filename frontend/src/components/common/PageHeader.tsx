// Ref: workflow.md §4 Architecture Patterns | Feature: UI Components
import React, { ReactNode } from "react";

interface PageHeaderProps {
  title: string;
  eyebrow?: string;
  description?: string;
  action?: ReactNode;
  className?: string;
}

export default function PageHeader({
  title,
  eyebrow,
  description,
  action,
  className = "",
}: PageHeaderProps) {
  return (
    <div className={`flex flex-col sm:flex-row sm:items-end justify-between gap-4 mb-8 ${className}`}>
      <div>
        {eyebrow && (
          <p className="text-xs font-extrabold uppercase tracking-[0.08em] text-primary mb-1 select-none">
            {eyebrow}
          </p>
        )}
        <h1 className="text-3xl sm:text-4xl font-[800] tracking-[-0.035em] text-ink">
          {title}
        </h1>
        {description && (
          <p className="mt-2 text-base text-muted max-w-2xl leading-relaxed">
            {description}
          </p>
        )}
      </div>

      {action && <div className="shrink-0">{action}</div>}
    </div>
  );
}
