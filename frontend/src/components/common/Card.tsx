// Ref: workflow.md §4 Architecture Patterns | Feature: UI Components
import React, { ReactNode } from "react";

interface CardProps {
  children: ReactNode;
  size?: "default" | "large";
  className?: string;
  hoverEffect?: boolean;
}

export default function Card({
  children,
  size = "default",
  className = "",
  hoverEffect = true,
}: CardProps) {
  // Radius: 22px default, 28px large
  const radiusClass = size === "large" ? "rounded-[28px]" : "rounded-[22px]";

  const hoverClass = hoverEffect
    ? "transition-all duration-250 ease-[cubic-bezier(0.22,1,0.36,1)] hover:-translate-y-[5px] hover:border-border-card-hover hover:shadow-[0_2px_4px_rgba(27,21,48,0.06),0_18px_40px_rgba(77,52,200,0.16)]"
    : "";

  return (
    <div
      className={`bg-surface border border-border-card shadow-[0_1px_2px_rgba(27,21,48,0.05),0_8px_24px_rgba(27,21,48,0.05)] ${radiusClass} ${hoverClass} ${className}`}
    >
      {children}
    </div>
  );
}
