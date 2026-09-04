// Ref: workflow.md §4 Architecture Patterns | Feature: Base Layout
import Link from "next/link";

export default function Header() {
  return (
    <header className="border-b bg-white/80 backdrop-blur-support sticky top-0 z-50">
      <div className="mx-auto flex max-w-7xl items-center justify-between px-6 sm:px-4 lg:px-8">
        {/* Logo/Brand */}
        <div className="flex-shrink-0 flex items-center space-x-3">
          <Link href="/" className="text-xl font-bold text-gray-900">
            CampusConnect
          </Link>
        </div>

        {/* Navigation */}
        <nav className="hidden md:flex space-x-6">
          <Link href="/" className="text-gray-600 hover:text-gray-900 transition-colors">
            Home
          </Link>
          <Link href="/events" className="text-gray-600 hover:text-gray-900 transition-colors">
            Events
          </Link>
          <Link href="/resources" className="text-gray-600 hover:text-gray-900 transition-colors">
            Resources
          </Link>
          <Link href="/community" className="text-gray-600 hover:text-gray-900 transition-colors">
            Community
          </Link>
        </nav>

        {/* Auth buttons */}
        <div className="flex space-x-3">
          <Link href="/login" className="btn btn-ghost hover:bg-gray-100">
            Login
          </Link>
          <Link href="/register" className="btn btn-primary hover:bg-primary-600">
            Register
          </Link>
        </div>
      </div>
    </header>
  );
}