// Ref: workflow.md §4 Architecture Patterns | Feature: Community Forums
"use client";

import { Suspense, useState } from "react";
import Link from "next/link";
import { communityService } from "@/services/communityService";
import { queryKeys } from "@/services/queryKeys";
import { useSearchParams } from "next/navigation";
import Button from "@/components/common/Button";
import { getApiErrorMessage } from "@/lib/apiError";
import Pagination from "@/components/common/Pagination";
import { usePaginatedQuery } from "@/hooks/usePaginatedQuery";
import type { Community } from "@/types/community";

export default function CommunityPage() {
  return (
    <Suspense fallback={<div className="min-h-[calc(100vh-160px)] flex items-center justify-center text-gray-500">Loading...</div>}>
      <CommunityPageInner />
    </Suspense>
  );
}

function CommunityPageInner() {
  const searchParams = useSearchParams();
  const queryParam = searchParams.get("q") || "";
  const [searchTerm, setSearchTerm] = useState(queryParam);
  const [activeSearch, setActiveSearch] = useState(queryParam);

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
    queryKeyPrefix: activeSearch
      ? queryKeys.communities.search(activeSearch)
      : queryKeys.communities.all(),
    queryFn: (params) =>
      activeSearch
        ? communityService.searchCommunities(activeSearch, params)
        : communityService.getCommunities(params),
    defaultPageSize: 12,
  });

  const handleSearchSubmit = (e: React.FormEvent) => {
    e.preventDefault();
    setActiveSearch(searchTerm.trim());
    setPage(1);
  };

  const handleClear = () => {
    setSearchTerm("");
    setActiveSearch("");
    setPage(1);
  };

  if (status === "error") {
    return (
      <div className="min-h-[calc(100vh-160px)] flex flex-col items-center justify-center py-12">
        <p className="text-gray-600 mb-4" role="alert">
          {getApiErrorMessage(error, "Failed to load communities.")}
        </p>
        <Button variant="outline" onClick={() => void refetch()}>
          Try Again
        </Button>
      </div>
    );
  }

  return (
    <div className="min-h-[calc(100vh-160px)] py-12">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="mb-8 flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
          <div>
            <h1 className="text-2xl font-bold text-gray-900">Student Communities</h1>
            <p className="mt-1 text-sm text-gray-500">
              Discover student groups, academic societies, and clubs
            </p>
          </div>
          <Link
            href="/community/create"
            className="inline-flex items-center justify-center rounded-md text-sm font-medium h-10 px-4 bg-gray-900 text-white shadow-sm hover:bg-gray-800 transition-colors whitespace-nowrap"
          >
            + Create Community
          </Link>
        </div>

        {/* Search bar */}
        <form onSubmit={handleSearchSubmit} className="mb-6 flex gap-2 max-w-md">
          <input
            type="text"
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            placeholder="Search communities..."
            className="flex-1 rounded-md border border-gray-300 px-3 py-2 text-sm shadow-sm focus:border-gray-900 focus:outline-none"
          />
          <Button type="submit" variant="primary">
            Search
          </Button>
          {activeSearch && (
            <Button type="button" variant="outline" onClick={handleClear}>
              Clear
            </Button>
          )}
        </form>

        {isFetching && status !== "pending" && (
          <p className="text-xs text-gray-400 mb-4">Refreshing results...</p>
        )}

        {status === "pending" ? (
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {[...Array(6)].map((_, i) => (
              <div key={i} className="bg-white rounded-lg border border-gray-200 p-6 animate-pulse">
                <div className="h-5 bg-gray-200 rounded w-2/3 mb-2" />
                <div className="h-4 bg-gray-100 rounded w-1/3" />
              </div>
            ))}
          </div>
        ) : communities.length === 0 ? (
          <div className="bg-white rounded-lg border border-gray-200 p-12 text-center">
            <h3 className="text-base font-semibold text-gray-900">No communities found</h3>
            <p className="mt-1 text-sm text-gray-500">
              {activeSearch ? "Try a different search term" : "Be the first to create one!"}
            </p>
          </div>
        ) : (
          <>
            <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
              {communities.map((community) => (
                <Link
                  key={community.id}
                  href={`/community/${community.id}`}
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
