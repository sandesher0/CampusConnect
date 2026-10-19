// Ref: workflow.md §13 Component & API Documentation Template | Feature: Community Validation
import { z } from "zod";

export const createCommunitySchema = z.object({
  communityName: z
    .string()
    .min(1, "Community name is required")
    .min(3, "Community name must be at least 3 characters")
    .max(100, "Community name must not exceed 100 characters"),
  communityType: z.enum(["Public", "Private"], {
    error: "Community type must be Public or Private",
  }),
});

export type CreateCommunityValues = z.infer<typeof createCommunitySchema>;
