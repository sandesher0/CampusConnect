// Ref: workflow.md §4 Architecture Patterns | Feature: Community Detail
"use client";

import { useParams, useSearchParams } from "next/navigation";
import { Suspense } from "react";
import { useQuery } from "@tanstack/react-query";
import Link from "next/link";
import { communityService } from "@/services/communityService";
import { queryKeys } from "@/services/queryKeys";
import { useAuthStore } from "@/stores/authStore";

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

  const { data: community, status, error } = useQuery({
    queryKey: queryKeys.communities.detail(id),
    queryFn: () => communityService.getCommunityById(id),
    enabled: !!id,
  });

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
      <div className="py-8">
        <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">
          <Link href={fromMine ? "/community/mine" : "/community"} className="text-sm text-gray-500 hover:text-gray-900">
            ← Back
          </Link>
          <div className="mt-8 text-center py-16 border border-dashed border-gray-200 rounded-lg">
            <p className="text-gray-500">
              {error instanceof Error ? error.message : "Community not found."}
            </p>
          </div>
        </div>
      </div>
    );
  }

  const isPublic =
    community.communityType === "1" || community.communityType === "Public";

  const isOwner = community.createdBy === user?.user?.id;

  return (
    <div className="py-8">
      <div className="max-w-4xl mx-auto px-4 sm:px-6 lg:px-8">

        {/* Back nav */}
        <div className="mb-6">
          <Link
            href={fromMine ? "/community/mine" : "/community"}
            className="text-sm text-gray-500 hover:text-gray-900 inline-flex items-center gap-1"
          >
            <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 19l-7-7 7-7" />
            </svg>
            {fromMine ? "Back to My Communities" : "Back to Communities"}
          </Link>
        </div>

        {/* Community header */}
        <div className="bg-white border border-gray-200 shadow-sm rounded-lg p-6 mb-6">
          <div className="flex items-start gap-4">
            <div className="shrink-0 h-14 w-14 bg-gray-100 rounded-full flex items-center justify-center">
              <span className="text-gray-700 font-bold text-xl">
                {community.communityName.charAt(0).toUpperCase()}
              </span>
            </div>
            <div className="flex-1 min-w-0">
              <div className="flex flex-wrap items-center gap-3">
                <h1 className="text-xl font-bold text-gray-900">
                  {community.communityName}
                </h1>
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
              <p className="mt-1 text-sm text-gray-500">
                {community.communityMembers.length}{" "}
                {community.communityMembers.length === 1 ? "member" : "members"}
              </p>
            </div>

            {/* Create Event button — visible to all members */}
            <div className="shrink-0">
              <Link
                href={`/events/create?communityId=${id}`}
                className="inline-flex items-center justify-center gap-2 rounded-md text-sm font-medium h-9 px-4 bg-gray-900 text-white shadow-sm hover:bg-gray-800 transition-colors"
              >
                <svg className="h-4 w-4" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 4v16m8-8H4" />
                </svg>
                Create Event
              </Link>
            </div>
          </div>
        </div>

        {/* Members section */}
        <div className="bg-white border border-gray-200 shadow-sm rounded-lg p-6">
          <h2 className="text-base font-semibold text-gray-900 mb-4">
            Members
          </h2>

          {community.communityMembers.length === 0 ? (
            <p className="text-sm text-gray-500 py-4 text-center border border-dashed border-gray-200 rounded-lg">
              No members yet.
            </p>
          ) : (
            <ul className="divide-y divide-gray-100">
              {community.communityMembers.map((member) => (
                <li key={member.memberId} className="flex items-center gap-3 py-3">
                  <div className="shrink-0 h-8 w-8 bg-gray-100 rounded-full flex items-center justify-center">
                    {member.user.profileImageUrl ? (
                      <img
                        src={member.user.profileImageUrl}
                        alt={`${member.user.firstName} ${member.user.lastName}`}
                        className="h-8 w-8 rounded-full object-cover"
                      />
                    ) : (
                      <span className="text-gray-600 text-xs font-medium">
                        {member.user.firstName.charAt(0)}{member.user.lastName.charAt(0)}
                      </span>
                    )}
                  </div>
                  <div className="min-w-0 flex-1">
                    <p className="text-sm font-medium text-gray-900 truncate">
                      {member.user.firstName} {member.user.lastName}
                    </p>
                    <p className="text-xs text-gray-500 truncate">{member.user.email}</p>
                  </div>
                  <span className="text-xs text-gray-400 shrink-0">
                    {member.memberType === "1" || member.memberType === "Admin" ? "Admin" : "Member"}
                  </span>
                </li>
              ))}
            </ul>
          )}
        </div>
      </div>
    </div>
  );
}
