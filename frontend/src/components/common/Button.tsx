// Ref: workflow.md §4 Architecture Patterns | Feature: UI Components
import React, { ReactNode, forwardRef } from "react";
import Link from "next/link";

export interface ButtonProps {
  children: ReactNode;
  variant?: "primary" | "secondary" | "outline" | "ghost" | "destructive" | "tint";
  size?: "sm" | "md" | "lg" | "xl" | "hero";
  className?: string;
  asChild?: boolean;
  disabled?: boolean;
  type?: "button" | "submit" | "reset";
  loading?: boolean;
  leftIcon?: ReactNode;
  rightIcon?: ReactNode;
  href?: string;
  onClick?: (e: React.MouseEvent<HTMLButtonElement | HTMLAnchorElement>) => void;
}

const Button = forwardRef<HTMLButtonElement, ButtonProps>(
  (
    {
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
      href,
      onClick,
    },
    ref
  ) => {
    // Base classes: pill shape, weight 700, smooth hover lift, accessible focus outline
    const baseClasses =
      "inline-flex items-center justify-center rounded-full font-bold transition-all duration-200 select-none cursor-pointer disabled:opacity-50 disabled:cursor-not-allowed disabled:pointer-events-none gap-x-2 shrink-0 active:scale-[0.98]";

    // Variant classes according to playful polish spec mapped to tokens
    const variantClasses: Record<string, string> = {
      primary:
        "bg-primary text-white shadow-[0_6px_18px_rgba(109,74,255,0.28)] hover:bg-primary-hover hover:shadow-[0_10px_24px_rgba(109,74,255,0.35)] hover:-translate-y-[2px]",
      outline:
        "border border-border-btn bg-surface text-ink hover:bg-primary-tint hover:border-border-card-hover hover:-translate-y-[1px]",
      tint:
        "bg-primary-tint text-primary-on-tint hover:bg-primary-tint-hover hover:-translate-y-[1px]",
      secondary:
        "bg-primary-tint text-primary-on-tint hover:bg-primary-tint-hover hover:-translate-y-[1px]",
      ghost:
        "text-nav hover:bg-primary-tint hover:text-primary-on-tint",
      destructive:
        "bg-coral text-white shadow-sm hover:bg-coral-hover hover:-translate-y-[1px]",
    };

    // Size classes: min-height 44px (52px hero size)
    const sizeClasses: Record<string, string> = {
      sm: "min-h-[38px] px-4 text-xs tracking-tight",
      md: "min-h-[44px] px-5 text-sm",
      lg: "min-h-[48px] px-7 text-base",
      xl: "min-h-[52px] px-8 text-base",
      hero: "min-h-[52px] px-8 text-base md:text-lg",
    };

    const content = (
      <>
        {leftIcon && <span className="shrink-0">{leftIcon}</span>}
        {loading ? (
          <span className="inline-flex items-center justify-center w-4 h-4 animate-spin" aria-hidden="true">
            <svg className="w-4 h-4 text-current" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5">
              <path strokeLinecap="round" strokeLinejoin="round" d="M12 2v4m0 12v4M4.93 4.93l2.83 2.83m8.48 8.48l2.83 2.83M2 12h4m12 0h4M4.93 19.07l2.83-2.83m8.48-8.48l2.83-2.83" />
            </svg>
          </span>
        ) : (
          <span>{children}</span>
        )}
        {rightIcon && <span className="shrink-0">{rightIcon}</span>}
      </>
    );

    if (href && !disabled && !loading) {
      return (
        <Link
          href={href}
          className={`${baseClasses} ${variantClasses[variant] ?? variantClasses.primary} ${sizeClasses[size] ?? sizeClasses.md} ${className}`}
          onClick={onClick}
        >
          {content}
        </Link>
      );
    }

    if (asChild) {
      return (
        <span
          className={`${baseClasses} ${variantClasses[variant] ?? variantClasses.primary} ${sizeClasses[size] ?? sizeClasses.md} ${className}`}
          onClick={onClick}
        >
          {content}
        </span>
      );
    }

    return (
      <button
        ref={ref}
        type={type}
        disabled={disabled || loading}
        className={`${baseClasses} ${variantClasses[variant] ?? variantClasses.primary} ${sizeClasses[size] ?? sizeClasses.md} ${className}`}
        onClick={onClick}
      >
        {content}
      </button>
    );
  }
);

Button.displayName = "Button";
export default Button;