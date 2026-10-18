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

import { createPagedResultSchema, PagedResult } from "@/types/pagination";

// Detail schema — matches CommunityDetailResponse
export const CommunityDetailSchema = z.object({
  communityId: z.string(),
  communityName: z.string(),
  communityType: z.union([z.string(), z.number()]).transform((v) => String(v)),
  createdBy: z.string(),
  communityMembers: createPagedResultSchema(CommunityMemberSchema),
});

export type Community = z.infer<typeof CommunitySchema>;
export type CommunityDetail = z.infer<typeof CommunityDetailSchema>;
export type CommunityMember = z.infer<typeof CommunityMemberSchema>;

// Service functions
export const communityService = {
  // GET /api/communities/all — returns PagedResult<Community>
  getCommunities: async (params?: { page?: number; pageSize?: number }): Promise<PagedResult<Community>> => {
    const res = await apiClient.get("/communities/all", { params });
    return createPagedResultSchema(CommunitySchema).parse(res.data);
  },

  // GET /api/community/search/{communityName} — returns matching public communities (PagedResult<Community>)
  searchCommunities: async (communityName: string, params?: { page?: number; pageSize?: number }): Promise<PagedResult<Community>> => {
    const res = await apiClient.get(`/community/search/${encodeURIComponent(communityName)}`, { params });
    return createPagedResultSchema(CommunitySchema).parse(res.data);
  },

  // GET /api/community/{communityId} — returns CommunityDetailResponse
  getCommunityById: async (id: string, params?: { page?: number; pageSize?: number }): Promise<CommunityDetail> => {
    const res = await apiClient.get(`/community/${id}`, { params });
    return CommunityDetailSchema.parse(res.data);
  },

  // POST /api/community/create
  createCommunity: async (data: { communityName: string; communityType: string }): Promise<void> => {
    await apiClient.post("/community/create", data);
  },

  // GET /api/community/mine — returns communities the logged-in user belongs to (PagedResult<Community>)
  getMyCommunities: async (params?: { page?: number; pageSize?: number }): Promise<PagedResult<Community>> => {
    const res = await apiClient.get("/community/mine", { params });
    return createPagedResultSchema(CommunitySchema).parse(res.data);
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
