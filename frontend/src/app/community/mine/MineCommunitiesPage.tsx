// Ref: workflow.md §4 Architecture Patterns | Feature: My Communities
"use client";

import Link from "next/link";
import { Suspense } from "react";
import { useQuery } from "@tanstack/react-query";
import { communityService } from "@/services/communityService";
import { queryKeys } from "@/services/queryKeys";
import { useAuthStore } from "@/stores/authStore";
import PageHeader from "@/components/common/PageHeader";
import Card from "@/components/common/Card";
import Button from "@/components/common/Button";
import Skeleton from "@/components/common/Skeleton";
import EmptyState from "@/components/common/EmptyState";
import ErrorState from "@/components/common/ErrorState";
import Pagination from "@/components/common/Pagination";
import { useState } from "react";
import type { Community } from "@/types/community";

export default function MineCommunitiesPage() {
  return (
    <Suspense
      fallback={
        <div className="mx-auto max-w-[1200px] px-6 py-12 sm:px-8 space-y-6">
          <Skeleton className="h-10 w-48 mb-8" />
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {[1, 2, 3].map((i) => (
              <Skeleton key={i} className="h-44 w-full" />
            ))}
          </div>
        </div>
      }
    >
      <MineCommunitiesContent />
    </Suspense>
  );
}

function MineCommunitiesContent() {
  const { user } = useAuthStore();
  const [page, setPage] = useState(1);

  const {
    data: communitiesData,
    status,
    isFetching,
    error,
    refetch,
  } = useQuery({
    queryKey: queryKeys.communities.mine({ page, pageSize: 12 }),
    queryFn: () => communityService.getMyCommunities({ page, pageSize: 12 }),
    enabled: Boolean(user),
  });

  const communities = communitiesData?.items ?? [];
  const totalPages = communitiesData?.totalPages ?? 1;

  if (!user) {
    return (
      <div className="mx-auto max-w-[1200px] px-6 py-16 sm:px-8">
        <EmptyState
          title="Sign in to view your communities"
          description="Explore student organizations you have joined or created across campus."
          action={
            <Button href="/login?redirect=/community/mine" variant="primary">
              Log in to your account
            </Button>
          }
        />
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-[1200px] px-6 py-12 sm:px-8">
      <PageHeader
        eyebrow="MEMBERSHIPS"
        title="My Communities"
        description="Organizations and clubs you are currently part of."
        action={
          <Button href="/community/create" variant="primary">
            Start a Community
          </Button>
        }
      />

      {status === "pending" && (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {[1, 2, 3].map((i) => (
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

      {status === "error" && (
        <ErrorState
          error={error}
          onRetry={() => void refetch()}
          fallbackMessage="Unable to load your communities. Please try again."
        />
      )}

      {status === "success" && communities.length === 0 && (
        <EmptyState
          title="No communities yet"
          description="You haven't joined or created any communities yet. Browse the student directory and get involved!"
          action={
            <div className="flex gap-3">
              <Button href="/community" variant="primary">
                Browse Communities
              </Button>
              <Button href="/community/create" variant="outline">
                Start One
              </Button>
            </div>
          }
        />
      )}

      {status === "success" && communities.length > 0 && (
        <>
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
            {communities.map((community: Community, index) => {
              const delayClass = index < 6 ? `animate-rise delay-${index * 80}` : "";

              return (
                <Link
                  key={community.id}
                  href={`/community/${community.id}?from=mine`}
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
                      <span>View details</span>
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
            isLoading={isFetching}
          />
        </>
      )}
    </div>
  );
}
