// Ref: workflow.md §4 Architecture Patterns | Feature: Communities Page
"use client";

import Link from "next/link";
import { Suspense, useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { communityService } from "@/services/communityService";
import { queryKeys } from "@/services/queryKeys";
import PageHeader from "@/components/common/PageHeader";
import Card from "@/components/common/Card";
import Input from "@/components/common/Input";
import Button from "@/components/common/Button";
import Skeleton from "@/components/common/Skeleton";
import EmptyState from "@/components/common/EmptyState";
import ErrorState from "@/components/common/ErrorState";
import Pagination from "@/components/common/Pagination";
import type { Community } from "@/types/community";

export default function CommunityPage() {
  return (
    <Suspense
      fallback={
        <div className="mx-auto max-w-[1200px] px-6 py-12 sm:px-8 space-y-6">
          <Skeleton className="h-10 w-48 mb-8" />
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {[1, 2, 3, 4, 5, 6].map((i) => (
              <Skeleton key={i} className="h-44 w-full" />
            ))}
          </div>
        </div>
      }
    >
      <CommunityPageContent />
    </Suspense>
  );
}

function CommunityPageContent() {
  const [page, setPage] = useState(1);
  const [searchTerm, setSearchTerm] = useState("");
  const [activeSearch, setActiveSearch] = useState("");

  const isSearching = Boolean(activeSearch.trim());

  // Search communities query
  const searchQuery = useQuery({
    queryKey: queryKeys.communities.search(activeSearch, { page, pageSize: 12 }),
    queryFn: () => communityService.searchCommunities(activeSearch, { page, pageSize: 12 }),
    enabled: isSearching,
  });

  // Browse all public communities query
  const allQuery = useQuery({
    queryKey: queryKeys.communities.list({ page, pageSize: 12 }),
    queryFn: () => communityService.getCommunities({ page, pageSize: 12 }),
    enabled: !isSearching,
  });

  const activeQuery = isSearching ? searchQuery : allQuery;
  const communities = activeQuery.data?.items ?? [];
  const totalPages = activeQuery.data?.totalPages ?? 1;

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    setPage(1);
    setActiveSearch(searchTerm.trim());
  };

  const handleClear = () => {
    setSearchTerm("");
    setActiveSearch("");
    setPage(1);
  };

  return (
    <div className="mx-auto max-w-[1200px] px-6 py-12 sm:px-8">
      <PageHeader
        eyebrow="DIRECTORY"
        title="Communities"
        description="Connect with student organizations, academic clubs, and interest groups on campus."
        action={
          <Button href="/community/create" variant="primary">
            Start a Community
          </Button>
        }
      />

      {/* Search Form using Form kit Input */}
      <form onSubmit={handleSearch} className="mb-10 flex flex-col sm:flex-row items-stretch sm:items-center gap-3 max-w-xl">
        <div className="relative flex-1">
          <Input
            id="search"
            type="search"
            placeholder="Search communities by name..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="pl-11"
          />
          <div className="pointer-events-none absolute left-4 top-1/2 -translate-y-1/2 text-muted">
            <svg className="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
              <circle cx="11" cy="11" r="8" />
              <line x1="21" y1="21" x2="16.65" y2="16.65" />
            </svg>
          </div>
        </div>

        <div className="flex items-center gap-2">
          <Button type="submit" variant="primary">
            Search
          </Button>
          {activeSearch && (
            <Button type="button" variant="outline" onClick={handleClear}>
              Clear
            </Button>
          )}
        </div>
      </form>

      {/* Loading Skeleton */}
      {activeQuery.status === "pending" && (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {[1, 2, 3, 4, 5, 6].map((i) => (
            <Card key={i} className="p-6 space-y-4" hoverEffect={false}>
              <div className="flex justify-between items-center">
                <Skeleton className="h-6 w-32" />
                <Skeleton className="h-5 w-16 rounded-full" />
              </div>
              <Skeleton className="h-4 w-24" />
            </Card>
          ))}
        </div>
      )}

      {/* Error State */}
      {activeQuery.status === "error" && (
        <ErrorState
          error={activeQuery.error}
          onRetry={() => void activeQuery.refetch()}
          fallbackMessage="Unable to load communities. Please try again."
        />
      )}

      {/* Empty State */}
      {activeQuery.status === "success" && communities.length === 0 && (
        <EmptyState
          title="No communities found"
          description={
            activeSearch
              ? `No communities matched "${activeSearch}". Try another search term.`
              : "No public communities have been created yet. Be the first to start one!"
          }
          action={
            activeSearch ? (
              <Button onClick={handleClear} variant="outline">
                Clear Search
              </Button>
            ) : (
              <Button href="/community/create" variant="primary">
                Create a Community
              </Button>
            )
          }
        />
      )}

      {/* Communities Grid */}
      {activeQuery.status === "success" && communities.length > 0 && (
        <>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {communities.map((community: Community, index) => {
              const delayClass = index < 6 ? `animate-rise delay-${index * 80}` : "";

              return (
                <Link
                  key={community.id}
                  href={`/community/${community.id}`}
                  className="group block focus-visible:outline-none"
                >
                  <Card className={`p-6 flex flex-col justify-between h-full ${delayClass}`}>
                    <div>
                      <div className="flex items-start justify-between gap-3 mb-2">
                        <h2 className="text-xl font-[800] text-ink group-hover:text-primary transition-colors leading-tight">
                          {community.communityName}
                        </h2>
                        <span className="text-xs px-3 py-1 rounded-full bg-primary-tint text-primary-on-tint font-bold shrink-0">
                          {community.communityType}
                        </span>
                      </div>
                      <p className="text-xs font-semibold text-muted mt-2">
                        Status: <span className="text-ink">{community.status || "Active"}</span>
                      </p>
                    </div>

                    <div className="mt-6 pt-4 border-t border-border-divider flex items-center justify-between text-xs font-bold text-primary group-hover:translate-x-1 transition-transform">
                      <span>View community & members</span>
                      <span>→</span>
                    </div>
                  </Card>
                </Link>
              );
            })}
          </div>

          <Pagination
            page={page}
            totalPages={totalPages}
            onPageChange={setPage}
            isLoading={activeQuery.isFetching}
          />
        </>
      )}
    </div>
  );
}
