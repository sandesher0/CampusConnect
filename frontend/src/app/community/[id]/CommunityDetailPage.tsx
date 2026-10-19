// Ref: workflow.md §4 Architecture Patterns | Feature: Community Detail
"use client";

import { useParams, useSearchParams } from "next/navigation";
import { Suspense, useState } from "react";
import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import Link from "next/link";
import { communityService } from "@/services/communityService";
import { queryKeys } from "@/services/queryKeys";
import { useAuthStore } from "@/stores/authStore";
import { getApiErrorMessage } from "@/lib/apiError";
import Card from "@/components/common/Card";
import Button from "@/components/common/Button";
import Avatar from "@/components/common/Avatar";
import Skeleton from "@/components/common/Skeleton";
import ErrorState from "@/components/common/ErrorState";
import Pagination from "@/components/common/Pagination";
import Toast from "@/components/common/Toast";
import { MemberTypeLabels } from "@/types/community";

export default function CommunityDetailPage() {
  return (
    <Suspense
      fallback={
        <div className="mx-auto max-w-4xl px-6 py-12 sm:px-8 space-y-6">
          <Skeleton className="h-6 w-32" />
          <Skeleton className="h-44 w-full rounded-[24px]" />
          <Skeleton className="h-72 w-full rounded-[24px]" />
        </div>
      }
    >
      <CommunityDetailContent />
    </Suspense>
  );
}

function CommunityDetailContent() {
  const { id } = useParams<{ id: string }>();
  const searchParams = useSearchParams();
  const fromMine = searchParams.get("from") === "mine";
  const { user } = useAuthStore();
  const queryClient = useQueryClient();

  const [membersPage, setMembersPage] = useState(1);
  const [toast, setToast] = useState<{ message: string; type: "success" | "error" } | null>(null);

  const { data: community, status, isFetching, error, refetch } = useQuery({
    queryKey: queryKeys.communities.detail(id, { page: membersPage, pageSize: 20 }),
    queryFn: () => communityService.getCommunityById(id, { page: membersPage, pageSize: 20 }),
    enabled: !!id,
  });

  const joinMutation = useMutation({
    mutationFn: () => communityService.joinCommunity(id),
    onSuccess: async () => {
      setToast({ message: "You have joined this community!", type: "success" });
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: queryKeys.communities.all() }),
      ]);
    },
    onError: (err) => {
      setToast({ message: getApiErrorMessage(err, "Failed to join community."), type: "error" });
    },
  });

  const members = community?.communityMembers?.items ?? [];
  const totalPages = community?.communityMembers?.totalPages ?? 1;
  const isMember = user ? members.some((m) => m.userId === user.userId || m.userId === user.id) : false;

  if (status === "pending") {
    return (
      <div className="mx-auto max-w-4xl px-6 py-12 sm:px-8 space-y-6">
        <Skeleton className="h-6 w-32" />
        <Skeleton className="h-44 w-full rounded-[24px]" />
        <Skeleton className="h-72 w-full rounded-[24px]" />
      </div>
    );
  }

  if (status === "error" || !community) {
    return (
      <div className="mx-auto max-w-4xl px-6 py-16 sm:px-8">
        <ErrorState
          error={error}
          onRetry={() => void refetch()}
          fallbackMessage="The requested community could not be found."
        />
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-4xl px-6 py-12 sm:px-8 space-y-8">
      {toast && (
        <Toast
          message={toast.message}
          type={toast.type}
          onDismiss={() => setToast(null)}
        />
      )}

      {/* Breadcrumb */}
      <div className="flex items-center gap-2 text-sm font-semibold text-muted">
        <Link
          href={fromMine ? "/community/mine" : "/community"}
          className="hover:text-primary transition-colors"
        >
          {fromMine ? "My Communities" : "Communities"}
        </Link>
        <span>/</span>
        <span className="text-ink truncate max-w-sm">{community.communityName}</span>
      </div>

      {/* Header Info Card */}
      <Card className="p-6 sm:p-10" hoverEffect={false}>
        <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-6">
          <div>
            <div className="flex items-center gap-3">
              <h1 className="text-2xl sm:text-3xl font-[800] text-ink">
                {community.communityName}
              </h1>
              <span className="text-xs px-3 py-1 rounded-full bg-primary-tint text-primary-on-tint font-bold">
                {community.communityType}
              </span>
            </div>
            <p className="mt-2 text-xs font-semibold text-muted">
              Official student organization on CampusConnect
            </p>
          </div>

          <div>
            {user && community.communityType === "Public" && !isMember && (
              <Button
                variant="primary"
                onClick={() => joinMutation.mutate()}
                loading={joinMutation.isPending}
                disabled={joinMutation.isPending}
                aria-busy={joinMutation.isPending}
              >
                {joinMutation.isPending ? "Joining..." : "Join Community"}
              </Button>
            )}

            {isMember && (
              <div className="flex items-center gap-2 px-4 py-2 rounded-full bg-status-good-bg border border-border-card text-status-good-text text-xs font-bold">
                <svg className="w-4 h-4" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="3" strokeLinecap="round" strokeLinejoin="round">
                  <polyline points="20 6 9 17 4 12" />
                </svg>
                <span>You are a member</span>
              </div>
            )}
          </div>
        </div>
      </Card>

      {/* Members List Card */}
      <Card className="p-6 sm:p-10" hoverEffect={false}>
        <h2 className="text-xl font-[800] text-ink mb-6">
          Community Members ({community.communityMembers?.totalCount ?? 0})
        </h2>

        {members.length === 0 ? (
          <p className="text-sm text-muted py-4">No members listed yet.</p>
        ) : (
          <div className="divide-y divide-border-divider">
            {members.map((member) => (
              <div key={member.memberId} className="py-4 flex items-center justify-between">
                <div className="flex items-center gap-3.5">
                  <Avatar name={member.user.firstName || member.user.email} size="md" />
                  <div>
                    <p className="text-sm font-bold text-ink">
                      {member.user.firstName} {member.user.lastName}
                    </p>
                    <p className="text-xs text-muted font-medium">{member.user.email}</p>
                  </div>
                </div>

                <span className="text-xs font-bold px-3 py-1 rounded-full bg-page border border-border-card text-muted">
                  {MemberTypeLabels[member.memberType] ?? "Member"}
                </span>
              </div>
            ))}
          </div>
        )}

        <Pagination
          page={membersPage}
          totalPages={totalPages}
          onPageChange={setMembersPage}
          isLoading={isFetching}
        />
      </Card>
    </div>
  );
}
