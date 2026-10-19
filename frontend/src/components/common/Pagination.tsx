// Ref: workflow.md §4 Architecture Patterns | Feature: UI Components
import React from "react";
import Button from "./Button";

interface PaginationProps {
  page: number;
  totalPages: number;
  onPageChange: (newPage: number) => void;
  isLoading?: boolean;
}

export default function Pagination({
  page,
  totalPages,
  onPageChange,
  isLoading = false,
}: PaginationProps) {
  if (totalPages <= 1) return null;

  // Generate page numbers with ellipses
  const getPageNumbers = () => {
    const pages: (number | string)[] = [];
    const maxVisible = 5;

    if (totalPages <= maxVisible) {
      for (let i = 1; i <= totalPages; i++) pages.push(i);
    } else {
      pages.push(1);
      let start = Math.max(2, page - 1);
      let end = Math.min(totalPages - 1, page + 1);

      if (page <= 2) {
        end = 3;
      } else if (page >= totalPages - 1) {
        start = totalPages - 2;
      }

      if (start > 2) pages.push("...");
      for (let i = start; i <= end; i++) pages.push(i);
      if (end < totalPages - 1) pages.push("...");
      pages.push(totalPages);
    }

    return pages;
  };

  return (
    <nav
      aria-label="Pagination Navigation"
      className="flex flex-wrap items-center justify-center gap-2 pt-8 select-none"
    >
      {/* Previous button */}
      <Button
        variant="outline"
        size="sm"
        disabled={page <= 1 || isLoading}
        onClick={() => onPageChange(page - 1)}
        aria-label="Go to previous page"
        className="px-3"
      >
        <svg className="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
          <polyline points="15 18 9 12 15 6" />
        </svg>
        <span className="hidden sm:inline">Prev</span>
      </Button>

      {/* Number pills */}
      <div className="flex items-center gap-1.5">
        {getPageNumbers().map((p, idx) => {
          if (p === "...") {
            return (
              <span key={`ellipsis-${idx}`} className="px-2 text-muted text-sm font-bold">
                ...
              </span>
            );
          }

          const pageNum = p as number;
          const isCurrent = pageNum === page;

          return (
            <button
              key={pageNum}
              type="button"
              disabled={isLoading}
              onClick={() => onPageChange(pageNum)}
              aria-current={isCurrent ? "page" : undefined}
              aria-label={`Page ${pageNum}`}
              className={`min-w-[40px] h-10 px-3 rounded-full text-sm font-bold transition-all duration-150 cursor-pointer ${
                isCurrent
                  ? "bg-primary text-white shadow-sm"
                  : "bg-surface border border-border-btn text-ink hover:bg-primary-tint hover:border-border-card-hover"
              }`}
            >
              {pageNum}
            </button>
          );
        })}
      </div>

      {/* Next button */}
      <Button
        variant="outline"
        size="sm"
        disabled={page >= totalPages || isLoading}
        onClick={() => onPageChange(page + 1)}
        aria-label="Go to next page"
        className="px-3"
      >
        <span className="hidden sm:inline">Next</span>
        <svg className="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round" aria-hidden="true">
          <polyline points="9 18 15 12 9 6" />
        </svg>
      </Button>
    </nav>
  );
}
