"use client";
// Ref: workflow.md §4 Architecture Patterns | Feature: Base Layout
import Link from "next/link";
import { useAuthStore } from "@/stores/authStore";
import { useEffect, useState } from "react";

export default function Header() {
  const { user, logout } = useAuthStore();
  const [isClient, setIsClient] = useState(false);

  useEffect(() => {
    setIsClient(true);
  }, []);

  return (
    <header className="border-b border-gray-200 bg-white sticky top-0 z-50">
      <div className="mx-auto flex h-16 max-w-7xl items-center justify-between px-4 sm:px-6 lg:px-8">
        <div className="flex items-center gap-8">
          <Link href="/" className="text-xl font-semibold text-gray-900 tracking-tight">
            CampusConnect
          </Link>

          <nav className="hidden md:flex gap-6">
            <Link href="/events" className="text-sm font-medium text-gray-600 hover:text-gray-900 transition-colors">
              Events
            </Link>
            <Link href="/community" className="text-sm font-medium text-gray-600 hover:text-gray-900 transition-colors">
              Directory
            </Link>
          </nav>
        </div>

        <div className="flex items-center gap-4">
          {isClient && user ? (
            <div className="flex items-center gap-4">
              <Link href="/profile" className="text-sm font-medium text-gray-600 hover:text-gray-900 transition-colors">
                {user.user.firstName}
              </Link>
            </div>
          ) : (
            <>
              <Link
                href="/login"
                className="text-sm font-medium text-gray-600 hover:text-gray-900 transition-colors"
              >
                Log in
              </Link>
              <Link
                href="/register"
                className="rounded-md bg-gray-900 px-3 py-2 text-sm font-medium text-white hover:bg-gray-800 transition-colors"
              >
                Sign up
              </Link>
            </>
          )}
        </div>
      </div>
    </header>
  );
}