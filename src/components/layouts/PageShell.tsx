// Ref: workflow.md §4 Architecture Patterns | Feature: Base Layout
import { ReactNode } from "react";

interface PageShellProps {
  children: ReactNode;
  className?: string;
}

export default function PageShell({ children, className = "" }: PageShellProps) {
  return (
    <div className={`min-h-screen flex flex-col ${className}`}>
      {/* Header will be inserted here */}
      <main className="flex-1">{children}</main>
      {/* Footer will be inserted here */}
    </div>
  );
}