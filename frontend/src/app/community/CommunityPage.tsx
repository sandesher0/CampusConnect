// Ref: workflow.md §4 Architecture Patterns | Feature: Community Forums
"use client";

import { Suspense } from "react";
import { useQuery } from "@tanstack/react-query";
import Link from "next/link";
import { communityService } from "@/services/communityService";
import { useSearchParams } from "next/navigation";
import { useState } from "react";
import Button from "@/components/common/Button";

export default function CommunityPage() {
  return (
    <Suspense fallback={<div className="min-h-[calc(100vh-160px)] flex flex-col items-center justify-center py-12">Loading...</div>}>
      <CommunityPageInner />
    </Suspense>
  );
}

function CommunityPageInner() {
  const searchParams = useSearchParams();
  const [search, setSearch] = useState(searchParams.get("q") || "");
  const [category, setCategory] = useState(searchParams.get("category") || "");

  const {
    data: communities = [],
    status,
    error,
  } = useQuery({
    queryKey: ["communities", { search, category }],
    queryFn: () =>
      communityService.getCommunities({
        search,
        category,
      }),
  });

  if (status === "error") {
    return (
      <div className="min-h-[calc(100vh-160px)] flex flex-col items-center justify-center py-12">
        <h2 className="text-2xl font-bold text-gray-800 mb-4">
          Failed to load communities
        </h2>
        <p className="text-gray-600 mb-6">
          {error instanceof Error ? error.message : "An unknown error occurred"}
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
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">
            Campus Communities
          </h1>
          <p className="mt-2 text-gray-600">
            Find and join student organizations, clubs, and groups
          </p>
        </div>

        {/* Search and filters */}
        <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4 mb-8">
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Search communities
            </label>
            <div className="relative">
              <input
                type="text"
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                placeholder="Search by name or description..."
                className="block w-full rounded-md border-gray-300 shadow-sm focus:border-gray-900 focus:ring-gray-900 sm:text-sm"
              />
            </div>
          </div>
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Category
            </label>
            <select
              value={category}
              onChange={(e) => setCategory(e.target.value)}
              className="block w-full rounded-md border-gray-300 shadow-sm focus:border-gray-900 focus:ring-gray-900 sm:text-sm"
            >
              <option value="">All Categories</option>
              <option value="academic">Academic</option>
              <option value="sports">Sports & Recreation</option>
              <option value="arts">Arts & Culture</option>
              <option value="service">Community Service</option>
              <option value="cultural">Cultural & Identity</option>
              <option value="professional">Professional Development</option>
              <option value="social">Social & Special Interest</option>
            </select>
          </div>
          <div className="flex items-end">
            <Button
              variant="outline"
              onClick={() => {
                setSearch("");
                setCategory("");
              }}
            >
              Reset
            </Button>
          </div>
          <div className="flex items-end justify-end">
            <Button
              variant="primary"
              onClick={() => {
                // Trigger refetch with new params
              }}
            >
              Search
            </Button>
          </div>
        </div>

        {/* Communities grid */}
        {communities.length === 0 && status === "success" ? (
          <div className="text-center py-12">
            <p className="text-gray-500">
              No communities found matching your criteria.
            </p>
          </div>
        ) : (
          <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {communities.map((community) => (
              <div
                key={community.id}
                className="bg-white rounded-lg border border-gray-200 shadow-sm hover:shadow-md transition-shadow"
              >
                <div className="p-6">
                  <div className="flex items-start space-x-4">
                    <div className="flex-shrink-0">
                      <div className="h-10 w-10 bg-gray-100 rounded-full flex items-center justify-center">
                        <span className="text-gray-900 font-bold">{community.communityName.charAt(0)}</span>
                      </div>
                    </div>
                    <div>
                      <h3 className="text-lg font-semibold text-gray-900 mb-2">
                        {community.communityName}
                      </h3>
                      <p className="text-sm text-gray-500 mb-3">
                        {"A CampusConnect community"}
                      </p>
                      <div className="flex items-center text-sm text-gray-500 mb-4">
                        <span className="mr-4">
                          {0} members
                        </span>
                        <span className={`
                          px-2 py-0.5 rounded text-xs font-medium
                          ${community.communityType === 'Public' ? "bg-green-50 text-green-800" : "bg-red-50 text-red-800"}
                        `}>
                          {community.communityType === 'Public' ? "Public" : "Private"}
                        </span>
                      </div>
                      <div className="flex items-center text-xs text-gray-500">
                        <span>Created by {community.createdBy}</span>
                      </div>
                    </div>
                  </div>
                  <div className="mt-4">
                    <Button
                      variant="outline"
                      className="w-full"
                      onClick={() => {
                        // Join community logic would go here
                      }}
                    >
                      Join Community
                    </Button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}

      </div>
    </div>
  );
}