// Ref: workflow.md §4 Architecture Patterns | Feature: UI Components
import type { EventCategory } from "@/types/event";

interface CategoryChipProps {
  category: EventCategory | string;
  className?: string;
}

export default function CategoryChip({ category, className = "" }: CategoryChipProps) {
  // Category chips mapped to design tokens
  let colorClasses = "bg-primary-tint text-primary-on-tint";

  switch (category) {
    case "Social":
      colorClasses = "bg-cat-social-bg text-cat-social-text";
      break;
    case "Academic":
      colorClasses = "bg-cat-academic-bg text-cat-academic-text";
      break;
    case "Sports":
      colorClasses = "bg-cat-sports-bg text-cat-sports-text";
      break;
    default:
      colorClasses = "bg-primary-tint text-primary-on-tint";
      break;
  }

  return (
    <span
      className={`inline-flex items-center px-3 py-1 rounded-full text-xs font-bold tracking-tight select-none ${colorClasses} ${className}`}
    >
      {category}
    </span>
  );
}
