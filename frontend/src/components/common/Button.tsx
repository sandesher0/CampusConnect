// Ref: workflow.md §4 Architecture Patterns | Feature: UI Components
import { ReactNode } from "react";

interface ButtonProps {
  children: ReactNode;
  variant?: "primary" | "secondary" | "outline" | "ghost" | "destructive";
  size?: "sm" | "md" | "lg" | "xl";
  className?: string;
  asChild?: boolean;
  disabled?: boolean;
  type?: "button" | "submit" | "reset";
  loading?: boolean;
  leftIcon?: ReactNode;
  rightIcon?: ReactNode;
  onClick?: () => void;
}

export default function Button({
  children,
  variant = "primary",
  size = "md",
  className = "",
  asChild = false,
  disabled = false,
  type = "button",
  loading = false,
  leftIcon,
  rightIcon,
}: ButtonProps) {
  // Base classes
  const baseClasses = "inline-flex items-center justify-center rounded-md text-sm font-medium transition-all focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-ring focus-visible:ring-offset-2 disabled:opacity-50 disabled:pointer-events-nowait gap-x-2";

  // Variant classes
  const variantClasses = {
    primary: "bg-primary-600 text-primary-foreground hover:bg-primary-700 focus:ring-primary-500",
    secondary: "bg-secondary-600 text-secondary-foreground hover:bg-secondary-700 focus:ring-secondary-500",
    outline: "border border-input hover:bg-accent hover:text-accent-foreground focus:ring-accent",
    ghost: "hover:bg-accent hover:text-accent-foreground focus:ring-accent",
    destructive: "bg-red-600 text-red-50 hover:bg-red-700 focus:ring-red-500",
  };

  // Size classes
  const sizeClasses = {
    sm: "h-9 px-3 text-xs",
    md: "h-10 px-4 text-sm",
    lg: "h-11 px-8 text-base",
    xl: "h-12 px-10 text-lg",
  };

  const Component = asChild ? "span" : "button";

  return (
    <Component
      type={type}
      disabled={disabled || loading}
      className={`${baseClasses} ${variantClasses[variant]} ${sizeClasses[size]} ${className}`}
    >
      {leftIcon && <span className="shrink-0">{leftIcon}</span>}
      {loading ? (
        <span className="inline-flex items-center justify-center w-4 h-4 animate-spin">
          {/* Simple loading spinner */}
          <svg className="w-4 h-4 text-current" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" d="M12 8v4l4 2-4 4V8z" />
          </svg>
        </span>
      ) : (
        <>
          {children}
          {rightIcon && <span className="shrink-0">{rightIcon}</span>}
        </>
      )}
    </Component>
  );
}