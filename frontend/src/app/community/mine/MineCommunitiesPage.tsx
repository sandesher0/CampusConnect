// Ref: workflow.md §4 Architecture Patterns | Feature: My Communities
"use client";

import { useQuery } from "@tanstack/react-query";
import Link from "next/link";
import { communityService } from "@/services/communityService";
import { queryKeys } from "@/services/queryKeys";
import { useAuthStore } from "@/stores/authStore";

export default function MineCommunitiesPage() {
  const { user } = useAuthStore();

  const {
    data: communities = [],
    status,
    error,
  } = useQuery({
    queryKey: queryKeys.communities.mine(),
    queryFn: () => communityService.getMyCommunities(),
  });

  return (
    <div className="min-h-[calc(100vh-160px)] py-12">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">

        {/* Header */}
        <div className="mb-8 flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
          <div>
            <div className="flex items-center gap-2 text-sm text-gray-500 mb-1">
              <Link href="/community" className="hover:text-gray-900">Communities</Link>
              <span>/</span>
              <span className="text-gray-900 font-medium">My Communities</span>
            </div>
            <h1 className="text-2xl font-bold text-gray-900">My Communities</h1>
            <p className="mt-1 text-sm text-gray-500">
              Communities you have created or joined
            </p>
          </div>
          <Link
            href="/community/create"
            className="inline-flex items-center justify-center rounded-md text-sm font-medium h-10 px-4 bg-gray-900 text-white shadow-sm hover:bg-gray-800 transition-colors whitespace-nowrap"
          >
            + Create Community
          </Link>
        </div>

        {/* Loading skeleton */}
        {status === "pending" && (
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {[...Array(3)].map((_, i) => (
              <div key={i} className="bg-white rounded-lg border border-gray-200 p-6 animate-pulse">
                <div className="flex items-center gap-3 mb-4">
                  <div className="h-12 w-12 bg-gray-100 rounded-full" />
                  <div className="flex-1 space-y-2">
                    <div className="h-4 bg-gray-100 rounded w-3/4" />
                    <div className="h-3 bg-gray-100 rounded w-1/2" />
                  </div>
                </div>
                <div className="h-8 bg-gray-100 rounded" />
              </div>
            ))}
          </div>
        )}

        {/* Error */}
        {status === "error" && (
          <div className="text-center py-16 border border-dashed border-gray-200 rounded-lg">
            <p className="text-gray-500 text-sm">
              {error instanceof Error ? error.message : "Failed to load your communities"}
            </p>
            <button
              onClick={() => window.location.reload()}
              className="mt-4 text-sm font-medium text-gray-900 underline"
            >
              Try again
            </button>
          </div>
        )}

        {/* Empty state */}
        {status === "success" && communities.length === 0 && (
          <div className="text-center py-16 border border-dashed border-gray-200 rounded-lg">
            <p className="text-gray-500 text-sm">You haven&apos;t joined any communities yet.</p>
            <div className="mt-4 flex items-center justify-center gap-3">
              <Link
                href="/community"
                className="inline-flex items-center justify-center rounded-md text-sm font-medium h-9 px-4 border border-gray-200 bg-white shadow-sm hover:bg-gray-50 transition-colors"
              >
                Browse Communities
              </Link>
              <Link
                href="/community/create"
                className="inline-flex items-center justify-center rounded-md text-sm font-medium h-9 px-4 bg-gray-900 text-white shadow-sm hover:bg-gray-800 transition-colors"
              >
                Create One
              </Link>
            </div>
          </div>
        )}

        {/* Communities grid */}
        {status === "success" && communities.length > 0 && (
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {communities.map((community) => {
              const isOwner = community.createdBy === user?.user?.id;
              const isPublic =
                community.communityType === "1" ||
                community.communityType === "Public";

              return (
                <Link
                  key={community.id}
                  href={`/community/${community.id}?from=mine`}
                  className="group bg-white rounded-lg border border-gray-200 shadow-sm hover:shadow-md hover:border-gray-300 transition-all p-6 block"
                >
                  <div className="flex items-start gap-4">
                    <div className="shrink-0 h-12 w-12 bg-gray-100 rounded-full flex items-center justify-center">
                      <span className="text-gray-700 font-semibold">
                        {community.communityName.charAt(0).toUpperCase()}
                      </span>
                    </div>
                    <div className="min-w-0 flex-1">
                      <h3 className="text-sm font-semibold text-gray-900 group-hover:text-gray-700 truncate">
                        {community.communityName}
                      </h3>
                      <div className="mt-2 flex flex-wrap items-center gap-2">
                        <span className={`inline-flex items-center px-2 py-0.5 rounded text-xs font-medium ${
                          isPublic ? "bg-green-50 text-green-700" : "bg-gray-100 text-gray-600"
                        }`}>
                          {isPublic ? "Public" : "Private"}
                        </span>
                        {isOwner && (
                          <span className="inline-flex items-center px-2 py-0.5 rounded text-xs font-medium bg-blue-50 text-blue-700">
                            Owner
                          </span>
                        )}
                      </div>
                    </div>
                    <svg className="shrink-0 h-4 w-4 text-gray-300 group-hover:text-gray-500 mt-0.5 transition-colors" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 5l7 7-7 7" />
                    </svg>
                  </div>

                  {/* Create event quick action */}
                  <div className="mt-4 pt-4 border-t border-gray-100">
                    <span className="text-xs text-gray-400 group-hover:text-gray-600 transition-colors">
                      Click to view members &amp; create events →
                    </span>
                  </div>
                </Link>
              );
            })}
          </div>
        )}
      </div>
    </div>
  );
}
