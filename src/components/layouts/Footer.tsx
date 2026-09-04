// Ref: workflow.md §4 Architecture Patterns | Feature: Base Layout

export default function Footer() {
  return (
    <footer className="border-t bg-gray-50">
      <div className="mx-auto flex max-w-7xl flex-col items-center justify-between px-6 py-10 sm:flex-row sm:py-16">
        {/* Logo/Brand */}
        <div className="flex-shrink-0 flex items-center space-x-3">
          <span className="text-xl font-bold text-gray-900">
            CampusConnect
          </span>
        </div>

        {/* Navigation */}
        <nav className="flex flex-col items-center space-y-4 sm:flex-row sm:space-y-0 sm:space-x-8 text-sm">
          <a href="/" className="text-gray-600 hover:text-gray-900 transition-colors">
            Home
          </a>
          <a href="/events" className="text-gray-600 hover:text-gray-900 transition-colors">
            Events
          </a>
          <a href="/resources" className="text-gray-600 hover:text-gray-900 transition-colors">
            Resources
          </a>
          <a href="/community" className="text-gray-600 hover:text-gray-900 transition-colors">
            Community
          </a>
        </nav>

        {/* Social links */}
        <div className="flex space-x-4 mt-6 sm:mt-0 text-gray-500 hover:text-gray-700 transition-colors">
          <a href="#" aria-label="Twitter">
            {/* Twitter icon would go here */}
          </a>
          <a href="#" aria-label="Instagram">
            {/* Instagram icon would go here */}
          </a>
          <a href="#" aria-label="Discord">
            {/* Discord icon would go here */}
          </a>
        </div>
      </div>

      <div className="border-t border-gray-200">
        <div className="mx-auto flex max-w-7xl flex-col items-center justify-between px-6 py-6 sm:flex-row sm:py-10">
          <span className="text-xs text-gray-500">
            © {new Date().getFullYear()} CampusConnect. All rights reserved.
          </span>
          <div className="flex space-x-4 text-xs text-gray-500">
            <a href="#" className="hover:text-gray-900 transition-colors">
              Privacy Policy
            </a>
            <a href="#" className="hover:text-gray-900 transition-colors">
              Terms of Service
            </a>
          </div>
        </div>
      </div>
    </footer>
  );
}