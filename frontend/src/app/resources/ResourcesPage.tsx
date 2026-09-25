// Ref: workflow.md §4 Architecture Patterns | Feature: Resources Directory
"use client";

import { Suspense } from "react";
import { useInfiniteQuery } from "@tanstack/react-query";
import Link from "next/link";
import { resourceService } from "@/services/resourceService";
import { useSearchParams } from "next/navigation";
import { useState } from "react";
import Button from "@/components/common/Button";

export default function ResourcesPage() {
  return (
    <Suspense fallback={<div className="min-h-[calc(100vh-160px)] flex flex-col items-center justify-center py-12">Loading...</div>}>
      <ResourcesPageInner />
    </Suspense>
  );
}

function ResourcesPageInner() {
  const searchParams = useSearchParams();
  const [search, setSearch] = useState(searchParams.get("q") || "");
  const [category, setCategory] = useState(searchParams.get("category") || "");

  const {
    data,
    fetchNextPage,
    hasNextPage,
    isFetchingNextPage,
    status,
    error,
  } = useInfiniteQuery({
    queryKey: ["resources", { search, category }],
    queryFn: ({ pageParam = 1 }) =>
      resourceService.getResources({
        search,
        category,
        page: pageParam,
        limit: 10,
      }),
    getNextPageParam: (lastPage, allPages) => {
      if (lastPage.length < 10) return undefined;
      return allPages.length + 1;
    },
    initialPageParam: 1,
  });

  if (status === "error") {
    return (
      <div className="min-h-[calc(100vh-160px)] flex flex-col items-center justify-center py-12">
        <h2 className="text-2xl font-bold text-gray-800 mb-4">
          Failed to load resources
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

  const resources = data?.pages.flatMap((page) => page) || [];

  return (
    <div className="min-h-[calc(100vh-160px)] py-12">
      <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="mb-8">
          <h1 className="text-3xl font-bold text-gray-900">
            Campus Resources
          </h1>
          <p className="mt-2 text-gray-600">
            Discover helpful links, tools, and documents for campus life
          </p>
        </div>

        {/* Search and filters */}
        <div className="mb-8 bg-white p-4 rounded-lg border border-gray-200 shadow-sm flex flex-col sm:flex-row gap-4 items-center">
          <div className="relative flex-1 w-full">
            <div className="pointer-events-none absolute inset-y-0 left-0 flex items-center pl-3">
              <svg className="h-5 w-5 text-gray-400" viewBox="0 0 20 20" fill="currentColor">
                <path fillRule="evenodd" d="M9 3.5a5.5 5.5 0 100 11 5.5 5.5 0 000-11zM2 9a7 7 0 1112.452 4.391l3.328 3.329a.75.75 0 11-1.06 1.06l-3.329-3.328A7 7 0 012 9z" clipRule="evenodd" />
              </svg>
            </div>
            <input
              type="text"
              value={search}
              onChange={(e) => setSearch(e.target.value)}
              placeholder="Search resources..."
              className="block w-full rounded-md border-0 py-2 pl-10 text-gray-900 ring-1 ring-inset ring-gray-300 placeholder:text-gray-400 focus:ring-2 focus:ring-inset focus:ring-gray-900 sm:text-sm sm:leading-6 shadow-sm"
            />
          </div>
          <div className="w-full sm:w-64">
            <select
              value={category}
              onChange={(e) => setCategory(e.target.value)}
              className="block w-full rounded-md border-0 py-2 pl-3 pr-10 text-gray-900 ring-1 ring-inset ring-gray-300 focus:ring-2 focus:ring-inset focus:ring-gray-900 sm:text-sm sm:leading-6 shadow-sm"
            >
              <option value="">All Categories</option>
              <option value="academics">Academics</option>
              <option value="housing">Housing</option>
              <option value="finance">Finance</option>
              <option value="health">Health & Wellness</option>
              <option value="technology">Technology</option>
              <option value="clubs">Clubs & Organizations</option>
            </select>
          </div>
          {(search || category) && (
            <button
              onClick={() => {
                setSearch("");
                setCategory("");
              }}
              className="text-sm font-medium text-gray-500 hover:text-gray-900 whitespace-nowrap px-2"
            >
              Clear filters
            </button>
          )}
        </div>

        {/* Resources grid */}
        {resources.length === 0 && status === "success" ? (
          <div className="text-center py-12">
            <p className="text-gray-500">
              No resources found matching your criteria.
            </p>
          </div>
        ) : (
          <div className="grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
            {resources.map((resource) => (
              <div
                key={resource.id}
                className="bg-white rounded-lg border border-gray-200 shadow-sm hover:shadow-md transition-shadow"
              >
                <div className="p-6">
                  <h3 className="text-lg font-semibold text-gray-900 mb-2">
                    {resource.title}
                  </h3>
                  <p className="text-sm text-gray-500 mb-3">
                    {resource.description}
                  </p>
                  <div className="flex flex-wrap gap-2 mb-4">
                    {resource.tags?.map((tag) => (
                      <span
                        key={tag}
                        className="bg-gray-50 text-gray-700 text-xs font-medium me-2 mb-2 px-2.5 py-0.5 rounded"
                      >
                        #{tag}
                      </span>
                    ))}
                  </div>
                  <div className="flex items-center text-xs text-gray-500">
                    <span>Submitted by {resource.submittedBy}</span>
                  </div>
                  <div className="mt-4">
                    <Link
                      href={`/resources/${resource.id}`}
                      className="inline-flex items-center px-3 py-1 border border-gray-200 text-sm font-medium rounded-md shadow-sm text-gray-900 bg-white hover:bg-gray-100"
                    >
                      View Resource
                    </Link>
                    {resource.url && (
                      <a
                        href={resource.url}
                        target="_blank"
                        rel="noopener noreferrer"
                        className="ml-3 inline-flex items-center px-3 py-1 border border-gray-200 text-sm font-medium rounded-md shadow-sm text-gray-600 bg-white hover:bg-gray-100"
                      >
                        External Link
                      </a>
                    )}
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}

        {/* Load more button */}
        {hasNextPage && (
          <div className="flex justify-center mt-8">
            <Button
              variant="outline"
              onClick={() => fetchNextPage()}
              disabled={isFetchingNextPage}
            >
              {isFetchingNextPage ? "Loading..." : "Load More Resources"}
            </Button>
          </div>
        )}
      </div>
    </div>
  );
}