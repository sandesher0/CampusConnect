// Ref: workflow.md §4 Architecture Patterns | Feature: Base Layout

export default function Footer() {
  return (
    <footer className="border-t border-indigo-200 bg-indigo-50">
      <div className="mx-auto flex max-w-7xl flex-col items-center justify-between px-6 sm:px-4 lg:px-8 pt-12 pb-10">
        {/* Logo/Brand */}
        <div className="flex-shrink-0 flex items-center space-x-3">
          <Link href="/" className="flex items-center space-x-2">
            <span className="text-2xl font-bold text-indigo-600">Campus</span>
            <span className="text-2xl font-bold text-gray-800">Connect</span>
          </Link>
        </div>

        {/* Navigation */}
        <nav className="hidden md:flex flex-1 items-center justify-between space-x-6">
          <a
            href="/"
            className="px-3 py-1 rounded-md text-sm font-medium text-gray-500 hover:text-gray-900 hover:bg-gray-50 transition-colors"
          >
            Home
          </a>
          <a
            href="/events"
            className="px-3 py-1 rounded-md text-sm font-medium text-gray-500 hover:text-gray-900 hover:bg-gray-50 transition-colors"
          >
            Events
          </a>
          <a
            href="/resources"
            className="px-3 py-1 rounded-md text-sm font-medium text-gray-500 hover:text-gray-900 hover:bg-gray-50 transition-colors"
          >
            Resources
          </a>
          <a
            href="/community"
            className="px-3 py-1 rounded-md text-sm font-medium text-gray-500 hover:text-gray-900 hover:bg-gray-50 transition-colors"
          >
            Community
          </a>
        </nav>

        {/* Social links */}
        <div className="flex space-x-4">
          <a
            href="#"
            aria-label="Twitter"
            className="p-2 rounded hover:bg-indigo-100 transition-colors"
          >
            {/* Twitter icon would go here */}
            🐦
          </a>
          <a
            href="#"
            aria-label="Instagram"
            className="p-2 rounded hover:bg-indigo-100 transition-colors"
          >
            {/* Instagram icon would go here */}
            📷
          </a>
          <a
            href="#"
            aria-label="Discord"
            className="p-2 rounded hover:bg-indigo-100 transition-colors"
          >
            {/* Discord icon would go here */}
            💬
          </a>
        </div>
      </div>

      <div className="border-t border-indigo-200">
        <div className="mx-auto flex max-w-7xl flex-col items-center justify-between px-6 py-6 sm:flex-row sm:py-4">
          <span className="text-xs text-gray-500">
            © {new Date().getFullYear()} CampusConnect. All rights reserved.
          </span>
          <div className="flex space-x-4 text-xs text-gray-500">
            <a
              href="#"
              className="hover:text-indigo-600 transition-colors"
            >
              Privacy Policy
            </a>
            <a
              href="#"
              className="hover:text-indigo-600 transition-colors"
            >
              Terms of Service
            </a>
          </div>
        </div>
      </div>
    </footer>
  );
}