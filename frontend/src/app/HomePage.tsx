// Ref: workflow.md §4 Architecture Patterns | Feature: Landing Page
import Link from "next/link";

export default function Home() {
  return (
    <div className="bg-white">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8 py-20 md:py-28">
        <div className="max-w-3xl">
          <h1 className="text-4xl font-bold tracking-tight text-gray-900 sm:text-5xl">
            The campus directory for students.
          </h1>
          <p className="mt-6 text-lg leading-8 text-gray-600">
            CampusConnect helps you find events, join organizations, and discover resources across the university. Clean, straightforward, and built for students.
          </p>
          <div className="mt-10 flex items-center gap-x-4">
            <Link
              href="/events"
              className="rounded-md bg-gray-900 px-4 py-2.5 text-sm font-semibold text-white shadow-sm hover:bg-gray-800 focus-visible:outline focus-visible:outline-2 focus-visible:outline-offset-2 focus-visible:outline-gray-900"
            >
              Browse Events
            </Link>
            <Link href="/community" className="text-sm font-semibold leading-6 text-gray-900 px-4 py-2.5 rounded-md hover:bg-gray-50 border border-gray-200">
              Explore Communities <span aria-hidden="true">→</span>
            </Link>
          </div>
        </div>

        <div className="mt-20 grid grid-cols-1 md:grid-cols-2 gap-8 border-t border-gray-200 pt-16">
          <div>
            <h2 className="text-2xl font-bold tracking-tight text-gray-900">Events</h2>
            <p className="mt-4 text-base text-gray-600">
              Discover what's happening on campus. Filter by academic, social, or sports categories to find events that match your interests.
            </p>
            <Link href="/events" className="mt-6 inline-block text-sm font-semibold text-gray-900 hover:underline">
              View all events
            </Link>
          </div>
          <div>
            <h2 className="text-2xl font-bold tracking-tight text-gray-900">Communities</h2>
            <p className="mt-4 text-base text-gray-600">
              Join clubs and organizations. Connect with peers who share your academic focus or extracurricular passions.
            </p>
            <Link href="/community" className="mt-6 inline-block text-sm font-semibold text-gray-900 hover:underline">
              Browse directory
            </Link>
          </div>
        </div>
      </div>
    </div>
  );
}
