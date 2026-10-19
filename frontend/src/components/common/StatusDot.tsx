// Ref: workflow.md §4 Architecture Patterns | Feature: UI Components

interface StatusDotProps {
  status?: "good" | "warning";
  label?: string;
  pulse?: boolean;
  className?: string;
}

export default function StatusDot({
  status = "good",
  label,
  pulse = false,
  className = "",
}: StatusDotProps) {
  const dotColor = status === "good" ? "bg-status-good" : "bg-status-warning";
  const textColor = status === "good" ? "text-status-good-text" : "text-status-warning-text";

  return (
    <span className={`inline-flex items-center gap-2 select-none ${className}`}>
      <span className="relative flex h-2.5 w-2.5 items-center justify-center">
        {pulse && (
          <span
            className={`absolute inline-flex h-full w-full rounded-full opacity-75 animate-ping-pulse ${dotColor}`}
            aria-hidden="true"
          />
        )}
        <span className={`relative inline-flex h-2 w-2 rounded-full ${dotColor}`} />
      </span>
      {label && <span className={`text-xs font-semibold ${textColor}`}>{label}</span>}
    </span>
  );
}
