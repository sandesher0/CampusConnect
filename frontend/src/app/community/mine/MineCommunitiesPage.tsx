// Ref: workflow.md §4 Architecture Patterns | Feature: My Communities
"use client";

import { Suspense } from "react";
import Link from "next/link";
import { communityService } from "@/services/communityService";
import { queryKeys } from "@/services/queryKeys";
import { useAuthStore } from "@/stores/authStore";
import { getApiErrorMessage } from "@/lib/apiError";
import Pagination from "@/components/common/Pagination";
import { usePaginatedQuery } from "@/hooks/usePaginatedQuery";
import Button from "@/components/common/Button";
import type { Community } from "@/types/community";

export default function MineCommunitiesPage() {
  return (
    <Suspense fallback={<div className="min-h-[calc(100vh-160px)] flex items-center justify-center text-gray-500">Loading...</div>}>
      <MineCommunitiesPageInner />
    </Suspense>
  );
}

function MineCommunitiesPageInner() {
  const { user } = useAuthStore();

  const {
    items: communities,
    page,
    totalPages,
    setPage,
    status,
    isFetching,
    error,
    refetch,
  } = usePaginatedQuery<Community>({
    queryKeyPrefix: queryKeys.communities.mine(),
    queryFn: (params) => communityService.getMyCommunities(params),
    defaultPageSize: 12,
    enabled: Boolean(user),
  });

  if (!user) {
    return (
      <div className="min-h-[calc(100vh-160px)] flex flex-col items-center justify-center py-12">
        <p className="text-gray-500 mb-4">Please sign in to view your communities.</p>
        <Link href="/login">
          <Button variant="primary">Sign In</Button>
        </Link>
      </div>
    );
  }

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

        {status === "error" && (
          <div className="mb-6 p-4 bg-red-50 text-red-700 rounded-md text-sm" role="alert">
            {getApiErrorMessage(error, "Failed to load your communities.")}
            <Button variant="outline" className="ml-4" onClick={() => void refetch()}>
              Retry
            </Button>
          </div>
        )}

        {/* Loading skeleton */}
        {status === "pending" && (
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {[...Array(3)].map((_, i) => (
              <div key={i} className="bg-white rounded-lg border border-gray-200 p-6 animate-pulse">
                <div className="h-5 bg-gray-200 rounded w-2/3 mb-2" />
                <div className="h-4 bg-gray-100 rounded w-1/3" />
              </div>
            ))}
          </div>
        )}

        {status === "success" && communities.length === 0 && (
          <div className="bg-white rounded-lg border border-gray-200 p-12 text-center">
            <h3 className="text-base font-semibold text-gray-900">No communities yet</h3>
            <p className="mt-1 text-sm text-gray-500">
              You haven&apos;t joined or created any communities.
            </p>
            <div className="mt-6 flex justify-center gap-3">
              <Link href="/community">
                <Button variant="outline">Browse Communities</Button>
              </Link>
              <Link href="/community/create">
                <Button variant="primary">Create One</Button>
              </Link>
            </div>
          </div>
        )}

        {status === "success" && communities.length > 0 && (
          <>
            <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
              {communities.map((community) => (
                <Link
                  key={community.id}
                  href={`/community/${community.id}?from=mine`}
                  className="bg-white rounded-lg border border-gray-200 p-6 hover:border-gray-300 transition-shadow shadow-sm hover:shadow"
                >
                  <div className="flex items-start justify-between">
                    <h3 className="font-semibold text-gray-900 hover:text-blue-600 transition-colors">
                      {community.communityName}
                    </h3>
                    <span className="text-xs px-2 py-0.5 rounded-full bg-blue-50 text-blue-700 font-medium">
                      {community.communityType}
                    </span>
                  </div>
                  <p className="mt-2 text-xs text-gray-400">
                    Status: {community.status || "Active"}
                  </p>
                </Link>
              ))}
            </div>

            <Pagination
              page={page}
              totalPages={totalPages}
              onPageChange={setPage}
              isLoading={isFetching}
            />
          </>
        )}
      </div>
    </div>
  );
}
