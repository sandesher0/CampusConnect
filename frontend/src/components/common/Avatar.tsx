// Ref: workflow.md §4 Architecture Patterns | Feature: UI Components

interface AvatarProps {
  name?: string;
  size?: "sm" | "md" | "lg";
  className?: string;
}

export default function Avatar({ name = "", size = "md", className = "" }: AvatarProps) {
  const initial = name.trim().charAt(0).toUpperCase() || "?";

  const sizeClasses = {
    sm: "h-8 w-8 text-xs",
    md: "h-9 w-9 text-sm",
    lg: "h-11 w-11 text-base",
  };

  return (
    <div
      className={`inline-flex items-center justify-center rounded-full bg-primary text-white font-bold select-none ${sizeClasses[size]} ${className}`}
      aria-hidden="true"
    >
      {initial}
    </div>
  );
}
