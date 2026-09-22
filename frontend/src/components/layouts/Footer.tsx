// Ref: workflow.md §4 Architecture Patterns | Feature: Base Layout
import Link from "next/link";

export default function Footer() {
  return (
    <footer className="border-t border-gray-200 bg-white">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8 py-12 flex flex-col md:flex-row justify-between items-center gap-6">
        <div className="flex flex-col items-center md:items-start gap-2">
          <Link href="/" className="text-xl font-semibold text-gray-900 tracking-tight">
            CampusConnect
          </Link>
          <p className="text-sm text-gray-500">
            © {new Date().getFullYear()} CampusConnect. All rights reserved.
          </p>
        </div>

        <nav className="flex flex-wrap justify-center gap-x-6 gap-y-2 text-sm text-gray-600">
          <Link href="/events" className="hover:text-gray-900 transition-colors">
            Events
          </Link>
          <Link href="/community" className="hover:text-gray-900 transition-colors">
            Community
          </Link>
          <a href="#" className="hover:text-gray-900 transition-colors">
            Privacy
          </a>
          <a href="#" className="hover:text-gray-900 transition-colors">
            Terms
          </a>
        </nav>
      </div>
    </footer>
  );
}