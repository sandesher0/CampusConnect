// Ref: workflow.md §4 Architecture Patterns | Feature: Base Layout
import Link from "next/link";

export default function Footer() {
  return (
    <footer className="bg-footer text-footer-text border-t border-footer-border mt-auto">
      <div className="mx-auto max-w-[1200px] px-6 py-12 sm:px-8 flex flex-col md:flex-row justify-between items-center gap-8">
        <div className="flex flex-col items-center md:items-start gap-2.5">
          <Link href="/" className="flex items-center gap-2.5" aria-label="CampusConnect home">
            <div className="h-8 w-8 rounded-[9px] bg-primary flex items-center justify-center text-white shrink-0">
              <svg className="w-4 h-4 text-white" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="3" strokeLinecap="round">
                <path d="M16 8a5.5 5.5 0 1 0 0 8" />
              </svg>
            </div>
            <span className="text-lg font-[800] tracking-[-0.03em] text-white">
              CampusConnect
            </span>
          </Link>
          <p className="text-xs text-footer-muted">
            © {new Date().getFullYear()} CampusConnect. Built for students.
          </p>
        </div>

        <nav aria-label="Footer navigation" className="flex flex-wrap justify-center gap-x-6 gap-y-3 text-sm font-semibold text-footer-muted">
          <Link href="/events" className="hover:text-white transition-colors">
            Events
          </Link>
          <Link href="/community" className="hover:text-white transition-colors">
            Communities
          </Link>
          <Link href="/resources" className="hover:text-white transition-colors">
            Resources
          </Link>
          <Link href="/privacy" className="hover:text-white transition-colors">
            Privacy
          </Link>
          <Link href="/terms" className="hover:text-white transition-colors">
            Terms
          </Link>
        </nav>
      </div>
    </footer>
  );
}