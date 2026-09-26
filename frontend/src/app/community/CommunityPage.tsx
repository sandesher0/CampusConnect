// Ref: workflow.md §4 Architecture Patterns | Feature: Community Forums
"use client";

import { Suspense, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import Link from "next/link";
import { communityService } from "@/services/communityService";
import { useSearchParams } from "next/navigation";
import Button from "@/components/common/Button";

export default function CommunityPage() {
  return (
    <Suspense fallback={<div className="min-h-[calc(100vh-160px)] flex items-center justify-center text-gray-500">Loading...</div>}>
      <CommunityPageInner />
    </Suspense>
  );
}

function CommunityPageInner() {
  const searchParams = useSearchParams();
  const [search, setSearch] = useState(searchParams.get("q") || "");

  const {
    data: communities = [],
    status,
    error,
  } = useQuery({
    queryKey: ["communities"],
    queryFn: () => communityService.getCommunities(),
  });

  // Client-side filter since the API returns all public communities at once
  const filtered = search
    ? communities.filter((c) =>
        c.communityName.toLowerCase().includes(search.toLowerCase())
      )
    : communities;

  if (status === "error") {
    return (
      <div className="min-h-[calc(100vh-160px)] flex flex-col items-center justify-center py-12">
        <p className="text-gray-600 mb-4">
          {error instanceof Error ? error.message : "Failed to load communities"}
        </p>
        <Button variant="outline" onClick={() => window.location.reload()}>
          Try Again
        </Button>
      </div>
    );
  }

  return (
    <div className="min-h-[calc(100vh-160px)] py-12">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">

        {/* Header */}
        <div className="mb-8 flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">Campus Communities</h1>
            <p className="mt-1 text-sm text-gray-500">
              Public student organizations, clubs, and groups
            </p>
          </div>
          <div className="flex items-center gap-3">
            <Link
              href="/community/mine"
              className="inline-flex items-center justify-center rounded-md text-sm font-medium h-10 px-4 border border-gray-200 bg-white shadow-sm hover:bg-gray-50 transition-colors whitespace-nowrap"
            >
              My Communities
            </Link>
            <Link
              href="/community/create"
              className="inline-flex items-center justify-center rounded-md text-sm font-medium h-10 px-4 bg-gray-900 text-white shadow-sm hover:bg-gray-800 transition-colors whitespace-nowrap"
            >
              + Create Community
            </Link>
          </div>
        </div>

        {/* Search bar */}
        <div className="mb-8 bg-white p-3 rounded-lg border border-gray-200 shadow-sm flex items-center gap-3">
          <div className="relative flex-1">
            <div className="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3">
              <svg className="h-4 w-4 text-gray-400" viewBox="0 0 20 20" fill="currentColor">
                <path fillRule="evenodd" d="M9 3.5a5.5 5.5 0 100 11 5.5 5.5 0 000-11zM2 9a7 7 0 1112.452 4.391l3.328 3.329a.75.75 0 11-1.06 1.06l-3.329-3.328A7 7 0 012 9z" clipRule="evenodd" />
              </svg>
            </div>
            <input
              type="text"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder="Search communities..."
              className="block w-full rounded-md border-0 py-1.5 pl-9 text-gray-900 ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-gray-900 sm:text-sm sm:leading-6"
            />
          </div>
          {search && (
            <button
              onClick={() => setSearch("")}
              className="text-sm text-gray-500 hover:text-gray-900 px-2 whitespace-nowrap"
            >
              Clear
            </button>
          )}
        </div>

        {/* Loading skeleton */}
        {status === "pending" && (
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {[...Array(6)].map((_, i) => (
              <div key={i} className="bg-white rounded-lg border border-gray-200 p-6 animate-pulse">
                <div className="flex items-center gap-3 mb-4">
                  <div className="h-10 w-10 bg-gray-100 rounded-full" />
                  <div className="flex-1 space-y-2">
                    <div className="h-4 bg-gray-100 rounded w-3/4" />
                    <div className="h-3 bg-gray-100 rounded w-1/2" />
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}

        {/* Empty state */}
        {status === "success" && filtered.length === 0 && (
          <div className="text-center py-16 border border-dashed border-gray-200 rounded-lg">
            <p className="text-gray-500 text-sm">
              {search ? `No communities matching "${search}"` : "No public communities yet."}
            </p>
          </div>
        )}

        {/* Communities grid */}
        {status === "success" && filtered.length > 0 && (
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {filtered.map((community) => (
              <Link
                key={community.id}
                href={`/community/${community.id}`}
                className="group bg-white rounded-lg border border-gray-200 shadow-sm hover:shadow-md hover:border-gray-300 transition-all p-6 block"
              >
                <div className="flex items-start gap-4">
                  <div className="shrink-0 h-10 w-10 bg-gray-100 rounded-full flex items-center justify-center">
                    <span className="text-gray-700 font-semibold text-sm">
                      {community.communityName.charAt(0).toUpperCase()}
                    </span>
                  </div>
                  <div className="min-w-0 flex-1">
                    <h3 className="text-sm font-semibold text-gray-900 group-hover:text-gray-700 truncate">
                      {community.communityName}
                    </h3>
                    <div className="mt-2 flex items-center gap-2">
                      <span className={`inline-flex items-center px-2 py-0.5 rounded text-xs font-medium ${
                        community.communityType === "1" || community.communityType === "Public"
                          ? "bg-green-50 text-green-700"
                          : "bg-gray-100 text-gray-600"
                      }`}>
                        {community.communityType === "1" || community.communityType === "Public" ? "Public" : "Private"}
                      </span>
                    </div>
                  </div>
                  <svg className="shrink-0 h-4 w-4 text-gray-300 group-hover:text-gray-500 mt-0.5 transition-colors" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
                  </svg>
                </div>
              </Link>
            ))}
          </div>
        )}
      </div>
    </div>
  );
}
