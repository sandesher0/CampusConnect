"use client";
// Ref: workflow.md §4 Architecture Patterns | Feature: Base Layout
import Link from "next/link";
import { usePathname } from "next/navigation";
import { useAuthStore } from "@/stores/authStore";
import { useState, useRef, useEffect, useSyncExternalStore } from "react";
import { getApiErrorMessage } from "@/lib/apiError";
import Avatar from "@/components/common/Avatar";
import Button from "@/components/common/Button";

export default function Header() {
  const pathname = usePathname();
  const { user, logout } = useAuthStore();
  const [logoutError, setLogoutError] = useState<string | null>(null);
  const [menuOpen, setMenuOpen] = useState(false);
  const menuRef = useRef<HTMLDivElement>(null);
  const accountBtnRef = useRef<HTMLButtonElement>(null);

  // Avoid hydration flicker
  const isHydrated = useSyncExternalStore(
    () => () => {},
    () => true,
    () => false
  );

  const handleLogout = async () => {
    setLogoutError(null);
    setMenuOpen(false);
    try {
      await logout();
    } catch (error) {
      setLogoutError(getApiErrorMessage(error, "Unable to log out. Please try again."));
    }
  };

  // Close menu on click outside or Escape
  useEffect(() => {
    function handleKeyDown(e: KeyboardEvent) {
      if (e.key === "Escape" && menuOpen) {
        setMenuOpen(false);
        accountBtnRef.current?.focus();
      }
    }

    function handleClickOutside(e: MouseEvent) {
      if (menuRef.current && !menuRef.current.contains(e.target as Node)) {
        setMenuOpen(false);
      }
    }

    if (menuOpen) {
      document.addEventListener("keydown", handleKeyDown);
      document.addEventListener("mousedown", handleClickOutside);
    }
    return () => {
      document.removeEventListener("keydown", handleKeyDown);
      document.removeEventListener("mousedown", handleClickOutside);
    };
  }, [menuOpen]);

  const navLinks = [
    { label: "Events", href: "/events" },
    { label: "Communities", href: "/community" },
    { label: "Resources", href: "/resources" },
  ];

  return (
    <header className="border-b border-border-divider bg-page/90 backdrop-blur-md sticky top-0 z-50">
      {logoutError && (
        <div className="bg-coral-tint border-b border-coral-border px-4 py-2 text-center text-sm font-semibold text-status-warning-text" role="alert">
          {logoutError}
        </div>
      )}

      <div className="mx-auto flex max-w-[1200px] flex-wrap items-center justify-between gap-y-3 px-6 py-3.5 sm:px-8">
        {/* Logo and Nav */}
        <div className="flex items-center gap-6 sm:gap-10">
          <Link
            href="/"
            className="flex items-center gap-2.5 rounded-lg focus-visible:outline-none transition-transform hover:scale-[1.02]"
            aria-label="CampusConnect home"
          >
            {/* 36px violet rounded square with white "C" arc */}
            <div className="h-9 w-9 rounded-[11px] bg-primary flex items-center justify-center shadow-[0_2px_8px_rgba(109,74,255,0.35)] shrink-0">
              <svg className="w-5 h-5 text-white" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="3" strokeLinecap="round">
                <path d="M16 8a5.5 5.5 0 1 0 0 8" />
              </svg>
            </div>
            <span className="text-[21px] font-[800] tracking-[-0.03em] text-ink">
              CampusConnect
            </span>
          </Link>

          {/* Navigation Pills */}
          <nav aria-label="Primary navigation" className="flex items-center gap-1 sm:gap-2">
            {navLinks.map((link) => {
              const isActive = pathname.startsWith(link.href);
              return (
                <Link
                  key={link.href}
                  href={link.href}
                  aria-current={isActive ? "page" : undefined}
                  className={`px-4 py-2 text-sm font-bold rounded-full transition-all duration-200 ${
                    isActive
                      ? "bg-primary-tint text-primary-on-tint"
                      : "text-nav hover:bg-primary-tint hover:text-primary-on-tint"
                  }`}
                >
                  {link.label}
                </Link>
              );
            })}
          </nav>
        </div>

        {/* Right side: Auth Action */}
        <div className="flex items-center gap-3">
          {!isHydrated ? (
            /* Neutral placeholder of same dimensions to prevent layout flash */
            <div className="h-10 w-28 rounded-full bg-primary-soft-tint/60 animate-pulse" aria-hidden="true" />
          ) : user ? (
            /* Signed in: Single Account Button + Accessible Dropdown */
            <div className="relative" ref={menuRef}>
              <button
                ref={accountBtnRef}
                type="button"
                onClick={() => setMenuOpen(!menuOpen)}
                aria-haspopup="menu"
                aria-expanded={menuOpen}
                aria-label="Account menu"
                className="flex items-center gap-2.5 rounded-full pl-1 pr-3 py-1 bg-surface border border-border-btn hover:bg-primary-tint hover:border-border-card-hover transition-all cursor-pointer shadow-sm"
              >
                <Avatar name={user.user.firstName || user.username} size="sm" />
                <span className="text-sm font-bold text-ink max-w-[120px] truncate">
                  {user.user.firstName || user.username}
                </span>
                <svg
                  className={`w-4 h-4 text-muted transition-transform duration-200 ${menuOpen ? "rotate-180" : ""}`}
                  viewBox="0 0 24 24"
                  fill="none"
                  stroke="currentColor"
                  strokeWidth="2.5"
                  strokeLinecap="round"
                  strokeLinejoin="round"
                  aria-hidden="true"
                >
                  <path d="m6 9 6 6 6-6" />
                </svg>
              </button>

              {menuOpen && (
                <div
                  role="menu"
                  aria-orientation="vertical"
                  className="absolute right-0 mt-2 w-56 rounded-[22px] bg-surface border border-border-card p-2 shadow-[0_18px_40px_rgba(77,52,200,0.16)] z-50 animate-rise"
                >
                  <div className="px-3.5 py-2.5 border-b border-border-divider">
                    <p className="text-xs font-semibold text-muted">Signed in as</p>
                    <p className="text-sm font-bold text-ink truncate">{user.user.email}</p>
                  </div>

                  <div className="py-1">
                    <Link
                      href="/events/reservations/mine"
                      role="menuitem"
                      onClick={() => setMenuOpen(false)}
                      className="flex items-center gap-2.5 px-3.5 py-2 text-sm font-bold text-nav rounded-xl hover:bg-primary-tint hover:text-primary-on-tint transition-colors"
                    >
                      <svg className="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                        <path strokeLinecap="round" strokeLinejoin="round" d="M16.5 6v11.5m-9-11.5v11.5M3 8.5h18M5 4h14a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2z" />
                      </svg>
                      My Reservations
                    </Link>

                    <Link
                      href="/community/mine"
                      role="menuitem"
                      onClick={() => setMenuOpen(false)}
                      className="flex items-center gap-2.5 px-3.5 py-2 text-sm font-bold text-nav rounded-xl hover:bg-primary-tint hover:text-primary-on-tint transition-colors"
                    >
                      <svg className="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                        <path strokeLinecap="round" strokeLinejoin="round" d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2m16-10a4 4 0 1 0 0-8 4 4 0 0 0 0 8zm6 10v-2a4 4 0 0 0-3-3.87m-4-12a4 4 0 0 1 0 7.75" />
                      </svg>
                      My Communities
                    </Link>

                    <Link
                      href="/profile"
                      role="menuitem"
                      onClick={() => setMenuOpen(false)}
                      className="flex items-center gap-2.5 px-3.5 py-2 text-sm font-bold text-nav rounded-xl hover:bg-primary-tint hover:text-primary-on-tint transition-colors"
                    >
                      <svg className="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                        <path strokeLinecap="round" strokeLinejoin="round" d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2M12 11a4 4 0 1 0 0-8 4 4 0 0 0 0 8z" />
                      </svg>
                      Profile
                    </Link>
                  </div>

                  <div className="pt-1 border-t border-border-divider">
                    <button
                      type="button"
                      role="menuitem"
                      onClick={() => void handleLogout()}
                      className="w-full text-left flex items-center gap-2.5 px-3.5 py-2 text-sm font-bold text-status-warning-text rounded-xl hover:bg-cat-social-bg transition-colors"
                    >
                      <svg className="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                        <path strokeLinecap="round" strokeLinejoin="round" d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4m7 14 5-5-5-5m5 5H9" />
                      </svg>
                      Log out
                    </button>
                  </div>
                </div>
              )}
            </div>
          ) : (
            /* Signed out */
            <div className="flex items-center gap-2 sm:gap-3">
              <Button href="/login" variant="outline" size="sm">
                Log in
              </Button>
              <Button href="/register" variant="primary" size="sm">
                Sign up
              </Button>
            </div>
          )}
        </div>
      </div>
    </header>
  );
}
