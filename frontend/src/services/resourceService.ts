// Ref: workflow.md §4 Architecture Patterns | Feature: API Layer
import apiClient from "@/config/apiClient";
import { z } from "zod";

// Resource schema
export const ResourceSchema = z.object({
  id: z.string(),
  title: z.string(),
  description: z.string(),
  url: z.string().url(),
  category: z.string(),
  tags: z.array(z.string()),
  submittedBy: z.string(),
  isApproved: z.boolean(),
  createdAt: z.string().datetime(),
  updatedAt: z.string().datetime(),
});

export type Resource = z.infer<typeof ResourceSchema>;

// Service functions
export const resourceService = {
  getResources: async (filters?: Record<string, any>): Promise<Resource[]> => {
    const res = await apiClient.get("/resources", { params: filters });
    return z.array(ResourceSchema).parse(res.data);
  },

  getResourceById: async (id: string): Promise<Resource> => {
    const res = await apiClient.get(`/resources/${id}`);
    return ResourceSchema.parse(res.data);
  },

  submitResource: async (data: Omit<Resource, "id" | "submittedBy" | "isApproved" | "createdAt" | "updatedAt">): Promise<Resource> => {
    const res = await apiClient.post("/resources", data);
    return ResourceSchema.parse(res.data);
  },

  approveResource: async (id: string): Promise<Resource> => {
    const res = await apiClient.patch(`/resources/${id}/approve`);
    return ResourceSchema.parse(res.data);
  },
};