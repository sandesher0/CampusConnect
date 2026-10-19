// Ref: workflow.md §4 Architecture Patterns | Feature: Resources Directory
"use client";

import Link from "next/link";
import Button from "@/components/common/Button";

export default function ResourcesPage() {
  return (
    <div className="min-h-[calc(100vh-160px)] py-16">
      <div className="mx-auto max-w-4xl px-4 sm:px-6 lg:px-8 text-center">
        <div className="inline-flex items-center justify-center p-3 bg-blue-50 rounded-full mb-4">
          <span className="text-2xl">📚</span>
        </div>
        <h1 className="text-3xl font-bold text-gray-900 mb-2">Campus Resources Directory</h1>
        <p className="text-gray-500 max-w-lg mx-auto mb-8">
          The campus resource directory module is currently under development for v2. In the meantime, explore active events and student communities!
        </p>

        <div className="flex justify-center gap-4">
          <Link href="/events">
            <Button variant="primary">Browse Events</Button>
          </Link>
          <Link href="/community">
            <Button variant="outline">Explore Communities</Button>
          </Link>
        </div>
      </div>
    </div>
  );
}
