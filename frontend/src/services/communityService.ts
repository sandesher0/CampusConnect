// Ref: workflow.md §4 Architecture Patterns | Feature: API Layer
import apiClient from "@/config/apiClient";
import { z } from "zod";

// List schema — matches what GetAllPublicCommunitiesController returns (Community domain object)
export const CommunitySchema = z.object({
  id: z.string(),
  communityName: z.string(),
  communityType: z.union([z.string(), z.number()]).transform((v) => String(v)),
  createdBy: z.string(),
  status: z.union([z.string(), z.number()]).transform((v) => String(v)).optional(),
});

// Member schema — matches CommunityMemberResponse
export const CommunityMemberSchema = z.object({
  memberId: z.string(),
  communityId: z.string(),
  userId: z.string(),
  memberType: z.union([z.string(), z.number()]).transform((v) => String(v)),
  createdAt: z.string(),
  modifiedAt: z.string().optional().nullable(),
  user: z.object({
    userId: z.string(),
    email: z.string(),
    firstName: z.string(),
    lastName: z.string(),
    profileImageUrl: z.string().optional().nullable(),
  }),
});

// Detail schema — matches CommunityDetailResponse
export const CommunityDetailSchema = z.object({
  communityId: z.string(),
  communityName: z.string(),
  communityType: z.union([z.string(), z.number()]).transform((v) => String(v)),
  createdBy: z.string(),
  communityMembers: z.array(CommunityMemberSchema).default([]),
});

export type Community = z.infer<typeof CommunitySchema>;
export type CommunityDetail = z.infer<typeof CommunityDetailSchema>;
export type CommunityMember = z.infer<typeof CommunityMemberSchema>;

// Service functions
export const communityService = {
  // GET /api/communities/all — returns List<Community>
  getCommunities: async (filters?: Record<string, any>): Promise<Community[]> => {
    const res = await apiClient.get("/communities/all", { params: filters });
    return z.array(CommunitySchema).parse(res.data);
  },

  // GET /api/community/{communityId} — returns CommunityDetailResponse
  getCommunityById: async (id: string): Promise<CommunityDetail> => {
    const res = await apiClient.get(`/community/${id}`);
    return CommunityDetailSchema.parse(res.data);
  },

  // POST /api/community/create
  createCommunity: async (data: { communityName: string; communityType: string }): Promise<void> => {
    await apiClient.post("/community/create", data);
  },

  // GET /api/community/mine — returns communities the logged-in user belongs to
  getMyCommunities: async (): Promise<Community[]> => {
    const res = await apiClient.get("/community/mine");
    return z.array(CommunitySchema).parse(res.data);
  },

  // POST /api/community/{id}/join  (endpoint defined in CommunityController if present)
  joinCommunity: async (id: string): Promise<void> => {
    await apiClient.post(`/community/${id}/join`);
  },

  // DELETE /api/community/{id}/leave
  leaveCommunity: async (id: string): Promise<void> => {
    await apiClient.delete(`/community/${id}/leave`);
  },
};