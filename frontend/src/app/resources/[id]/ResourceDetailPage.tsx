// Ref: workflow.md §4 Architecture Patterns | Feature: Resource Detail
"use client";

import Link from "next/link";
import Button from "@/components/common/Button";

export default function ResourceDetailPage() {
  return (
    <div className="min-h-[calc(100vh-160px)] py-16">
      <div className="mx-auto max-w-2xl px-4 sm:px-6 lg:px-8 text-center">
        <h1 className="text-2xl font-bold text-gray-900 mb-2">Resource Details</h1>
        <p className="text-gray-500 mb-6">
          The campus resource directory is coming soon.
        </p>
        <Link href="/resources">
          <Button variant="outline">Back to Resources</Button>
        </Link>
      </div>
    </div>
  );
}