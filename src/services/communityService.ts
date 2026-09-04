// Ref: workflow.md §4 Architecture Patterns | Feature: API Layer
import apiClient from "@/config/apiClient";
import { z } from "zod";

// Community schema
export const CommunitySchema = z.object({
  id: z.string(),
  name: z.string(),
  description: z.string(),
  category: z.string(),
  memberCount: z.number().int(),
  isPublic: z.boolean(),
  createdBy: z.string(),
  createdAt: z.string().datetime(),
  updatedAt: z.string().datetime(),
});

export type Community = z.infer<typeof CommunitySchema>;

// Service functions
export const communityService = {
  getCommunities: async (filters?: Record<string, any>): Promise<Community[]> => {
    const res = await apiClient.get("/communities", { params: filters });
    return z.array(CommunitySchema).parse(res.data);
  },

  getCommunityById: async (id: string): Promise<Community> => {
    const res = await apiClient.get(`/communities/${id}`);
    return CommunitySchema.parse(res.data);
  },

  createCommunity: async (data: Omit<Community, "id" | "memberCount" | "createdBy" | "createdAt" | "updatedAt">): Promise<Community> => {
    const res = await apiClient.post("/communities", data);
    return CommunitySchema.parse(res.data);
  },

  joinCommunity: async (id: string): Promise<Community> => {
    const res = await apiClient.post(`/communities/${id}/join`);
    return CommunitySchema.parse(res.data);
  },

  leaveCommunity: async (id: string): Promise<void> => {
    await apiClient.delete(`/communities/${id}/leave`);
  },
};