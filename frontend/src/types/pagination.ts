import { z } from "zod";

export const createPagedResultSchema = <T extends z.ZodTypeAny>(itemSchema: T) =>
  z.object({
    items: z.array(itemSchema),
    totalCount: z.number(),
    page: z.number(),
    pageSize: z.number(),
    totalPages: z.number(),
  });

export type PagedResult<T> = {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
};
