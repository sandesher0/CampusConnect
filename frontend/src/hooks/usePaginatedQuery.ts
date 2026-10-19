// Ref: workflow.md §4 Architecture Patterns | Feature: Paginated Query Hook
import { useQuery, keepPreviousData } from "@tanstack/react-query";
import { usePathname, useRouter, useSearchParams } from "next/navigation";
import { useCallback, useMemo } from "react";
import type { PagedResult } from "@/types/pagination";

interface UsePaginatedQueryOptions<TData> {
  queryKeyPrefix: readonly unknown[];
  queryFn: (params: { page: number; pageSize: number }) => Promise<PagedResult<TData>>;
  defaultPageSize?: number;
  enabled?: boolean;
}

export function usePaginatedQuery<TData>({
  queryKeyPrefix,
  queryFn,
  defaultPageSize = 20,
  enabled = true,
}: UsePaginatedQueryOptions<TData>) {
  const router = useRouter();
  const pathname = usePathname();
  const searchParams = useSearchParams();

  const page = useMemo(() => {
    const rawPage = searchParams.get("page");
    const parsed = rawPage ? parseInt(rawPage, 10) : 1;
    return isNaN(parsed) || parsed < 1 ? 1 : parsed;
  }, [searchParams]);

  const pageSize = useMemo(() => {
    const rawPageSize = searchParams.get("pageSize");
    const parsed = rawPageSize ? parseInt(rawPageSize, 10) : defaultPageSize;
    return isNaN(parsed) || parsed < 1 ? defaultPageSize : Math.min(parsed, 50);
  }, [searchParams, defaultPageSize]);

  const setPage = useCallback(
    (newPage: number) => {
      const params = new URLSearchParams(searchParams.toString());
      if (newPage <= 1) {
        params.delete("page");
      } else {
        params.set("page", newPage.toString());
      }
      const query = params.toString();
      router.push(query ? `${pathname}?${query}` : pathname);
    },
    [router, pathname, searchParams]
  );

  const query = useQuery({
    queryKey: [...queryKeyPrefix, { page, pageSize }],
    queryFn: () => queryFn({ page, pageSize }),
    placeholderData: keepPreviousData,
    enabled,
  });

  return {
    ...query,
    items: query.data?.items ?? [],
    totalCount: query.data?.totalCount ?? 0,
    page: query.data?.page ?? page,
    pageSize: query.data?.pageSize ?? pageSize,
    totalPages: query.data?.totalPages ?? 1,
    setPage,
  };
}
