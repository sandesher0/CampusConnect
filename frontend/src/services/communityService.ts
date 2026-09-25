// Ref: workflow.md §4 Architecture Patterns | Feature: API Layer
import apiClient from "@/config/apiClient";
import { z } from "zod";

// Community schema
export const CommunitySchema = z.object({
  id: z.string(),
  communityName: z.string(),
  communityType: z.enum(["None", "Public", "Private"]),
  createdBy: z.string(),
  status: z.string(),
  createdAt: z.string().datetime(),
  updatedAt: z.string().datetime().optional(),
});

export type Community = z.infer<typeof CommunitySchema>;

// Service functions
export const communityService = {
  getCommunities: async (filters?: Record<string, any>): Promise<Community[]> => {
    const res = await apiClient.get("/community", { params: filters });
    return z.array(CommunitySchema).parse(res.data);
  },

  getCommunityById: async (id: string): Promise<Community> => {
    const res = await apiClient.get(`/community/${id}`);
    return CommunitySchema.parse(res.data);
  },

  createCommunity: async (data: { communityName: string, communityType: string }): Promise<void> => {
    await apiClient.post("/community/create", data);
  },

  joinCommunity: async (id: string): Promise<void> => {
    await apiClient.post(`/community/${id}/join`);
  },

  leaveCommunity: async (id: string): Promise<void> => {
    await apiClient.delete(`/community/${id}/leave`);
  },
};