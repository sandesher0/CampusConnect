// Ref: workflow.md §4 Architecture Patterns | Feature: Base Layout
import Link from "next/link";

export default function Header() {
  return (
    <header className="border-b bg-white/90 backdrop-blur-lg sticky top-0 z-50 shadow-sm">
      <div className="mx-auto flex max-w-7xl items-center justify-between px-6 sm:px-4 lg:px-8">
        {/* Logo/Brand */}
        <div className="flex-shrink-0 flex items-center space-x-3">
          <Link href="/" className="flex items-center space-x-2">
            <span className="text-2xl font-bold text-indigo-600">Campus</span>
            <span className="text-2xl font-bold text-gray-800">Connect</span>
          </Link>
        </div>

        {/* Navigation */}
        <nav className="hidden md:flex space-x-6">
          <Link
            href="/"
            className="flex items-center px-3 py-2 rounded-md text-sm font-medium text-gray-500 hover:text-gray-900 hover:bg-gray-50 transition-colors"
          >
            Home
          </Link>
          <Link
            href="/events"
            className="flex items-center px-3 py-2 rounded-md text-sm font-medium text-gray-500 hover:text-gray-900 hover:bg-gray-50 transition-colors"
          >
            Events
          </Link>
          <Link
            href="/resources"
            className="flex items-center px-3 py-2 rounded-md text-sm font-medium text-gray-500 hover:text-gray-900 hover:bg-gray-50 transition-colors"
          >
            Resources
          </Link>
          <Link
            href="/community"
            className="flex items-center px-3 py-2 rounded-md text-sm font-medium text-gray-500 hover:text-gray-900 hover:bg-gray-50 transition-colors"
          >
            Community
          </Link>
        </nav>

        {/* Auth buttons */}
        <div className="flex space-x-3">
          <Link
            href="/login"
            className="btn btn-outline hover:bg-indigo-50 hover:text-indigo-600 border-indigo-200"
          >
            Login
          </Link>
          <Link
            href="/register"
            className="btn btn-primary hover:bg-indigo-600"
          >
            Register
          </Link>
        </div>
      </div>
    </header>
  );
}