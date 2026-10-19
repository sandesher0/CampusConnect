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
import Pagination from "@/components/common/Pagination";
import Button from "@/components/common/Button";
import { MemberTypeLabels } from "@/types/community";

export default function CommunityDetailPage() {
  return (
    <Suspense fallback={<div className="py-8 text-center text-gray-500">Loading...</div>}>
      <CommunityDetailInner />
    </Suspense>
  );
}

function CommunityDetailInner() {
  const { id } = useParams<{ id: string }>();
  const searchParams = useSearchParams();
  const fromMine = searchParams.get("from") === "mine";
  const { user } = useAuthStore();
  const queryClient = useQueryClient();
  const [membersPage, setMembersPage] = useState(1);
  const [joinFeedback, setJoinFeedback] = useState<string | null>(null);

  const { data: community, status, isFetching, error, refetch } = useQuery({
    queryKey: queryKeys.communities.detail(id, { page: membersPage, pageSize: 20 }),
    queryFn: () => communityService.getCommunityById(id, { page: membersPage, pageSize: 20 }),
    enabled: !!id,
  });

  const joinMutation = useMutation({
    mutationFn: () => communityService.joinCommunity(id),
    onSuccess: async () => {
      setJoinFeedback("You have successfully joined this community!");
      await Promise.all([
        queryClient.invalidateQueries({ queryKey: queryKeys.communities.all() }),
      ]);
    },
    onError: (err) => {
      setJoinFeedback(getApiErrorMessage(err, "Failed to join community."));
    },
  });

  const members = community?.communityMembers?.items ?? [];
  const totalPages = community?.communityMembers?.totalPages ?? 1;
  const isMember = user ? members.some((m) => m.userId === user.userId || m.userId === user.id) : false;

  if (status === "pending") {
    return (
      <div className="py-8">
        <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8 animate-pulse space-y-4">
          <div className="h-6 bg-gray-100 rounded w-1/3" />
          <div className="h-4 bg-gray-100 rounded w-1/4" />
          <div className="mt-6 bg-white border border-gray-200 rounded-lg p-6 space-y-4">
            <div className="h-4 bg-gray-100 rounded w-full" />
            <div className="h-4 bg-gray-100 rounded w-3/4" />
          </div>
        </div>
      </div>
    );
  }

  if (status === "error" || !community) {
    return (
      <div className="min-h-[calc(100vh-160px)] flex flex-col items-center justify-center py-12">
        <p className="text-gray-600 mb-4" role="alert">
          {getApiErrorMessage(error, "Community not found.")}
        </p>
        <div className="flex gap-3">
          <Link href="/community">
            <Button variant="outline">Back to Communities</Button>
          </Link>
          <Button variant="primary" onClick={() => void refetch()}>
            Try Again
          </Button>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-[calc(100vh-160px)] py-12">
      <div className="mx-auto max-w-4xl px-4 sm:px-6 lg:px-8">
        <div className="mb-6 flex items-center gap-2 text-sm text-gray-500">
          <Link href={fromMine ? "/community/mine" : "/community"} className="hover:text-gray-900">
            {fromMine ? "My Communities" : "Communities"}
          </Link>
          <span>/</span>
          <span className="text-gray-900 font-medium">{community.communityName}</span>
        </div>

        {joinFeedback && (
          <div className="mb-6 p-4 bg-blue-50 text-blue-800 rounded-md text-sm flex justify-between items-center" role="status">
            <span>{joinFeedback}</span>
            <button type="button" onClick={() => setJoinFeedback(null)} className="text-blue-500 hover:text-blue-700">
              Dismiss
            </button>
          </div>
        )}

        <div className="bg-white border border-gray-200 shadow-sm rounded-lg p-6 sm:p-8 mb-8">
          <div className="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
            <div>
              <div className="flex items-center gap-3">
                <h1 className="text-2xl font-bold text-gray-900">{community.communityName}</h1>
                <span className="text-xs px-2.5 py-1 rounded-full bg-blue-50 text-blue-700 font-medium">
                  {community.communityType}
                </span>
              </div>
              <p className="mt-2 text-sm text-gray-500">
                Created by user {community.createdBy}
              </p>
            </div>

            {user && community.communityType === "Public" && !isMember && (
              <Button
                variant="primary"
                onClick={() => joinMutation.mutate()}
                disabled={joinMutation.isPending}
              >
                {joinMutation.isPending ? "Joining..." : "Join Community"}
              </Button>
            )}

            {isMember && (
              <span className="text-xs px-3 py-1.5 rounded-md bg-green-50 text-green-700 border border-green-200 font-medium">
                You are a member
              </span>
            )}
          </div>
        </div>

        {/* Members List */}
        <div className="bg-white border border-gray-200 shadow-sm rounded-lg p-6 sm:p-8">
          <h2 className="text-lg font-semibold text-gray-900 mb-4">
            Community Members ({community.communityMembers?.totalCount ?? 0})
          </h2>

          {members.length === 0 ? (
            <p className="text-sm text-gray-500">No members listed yet.</p>
          ) : (
            <div className="divide-y divide-gray-100">
              {members.map((member) => (
                <div key={member.memberId} className="py-3 flex items-center justify-between">
                  <div className="flex items-center gap-3">
                    <div className="h-9 w-9 rounded-full bg-gray-200 flex items-center justify-center text-sm font-semibold text-gray-600">
                      {member.user.firstName.charAt(0)}{member.user.lastName.charAt(0)}
                    </div>
                    <div>
                      <p className="text-sm font-medium text-gray-900">
                        {member.user.firstName} {member.user.lastName}
                      </p>
                      <p className="text-xs text-gray-500">{member.user.email}</p>
                    </div>
                  </div>
                  <span className="text-xs px-2 py-1 rounded bg-gray-100 text-gray-700">
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
        </div>
      </div>
    </div>
  );
}
