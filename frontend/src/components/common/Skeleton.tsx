// Ref: workflow.md §4 Architecture Patterns | Feature: UI Components
import React from "react";

interface SkeletonProps {
  className?: string;
}

export default function Skeleton({ className = "" }: SkeletonProps) {
  return (
    <div
      className={`bg-primary-soft-tint/70 animate-pulse rounded-[14px] ${className}`}
      aria-hidden="true"
    />
  );
}
