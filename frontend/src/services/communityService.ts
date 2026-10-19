// Ref: workflow.md §4 Architecture Patterns | Feature: API Layer
import apiClient from "@/config/apiClient";
import type { PagedResult } from "@/types/pagination";
import type {
  Community,
  CommunityDetailResponse,
  CommunityType,
} from "@/types/community";

export interface CreateCommunityPayload {
  communityName: string;
  communityType: CommunityType;
}

export const communityService = {
  // GET api/communities/all — returns PagedResult<Community>
  getCommunities: async (params?: { page?: number; pageSize?: number }): Promise<PagedResult<Community>> => {
    const res = await apiClient.get<PagedResult<Community>>("/communities/all", { params });
    return res.data;
  },

  // GET api/community/search/{communityName} — returns matching public communities (PagedResult<Community>)
  searchCommunities: async (
    communityName: string,
    params?: { page?: number; pageSize?: number }
  ): Promise<PagedResult<Community>> => {
    const res = await apiClient.get<PagedResult<Community>>(
      `/community/search/${encodeURIComponent(communityName)}`,
      { params }
    );
    return res.data;
  },

  // GET api/community/{communityId} — returns CommunityDetailResponse
  getCommunityById: async (
    id: string,
    params?: { page?: number; pageSize?: number }
  ): Promise<CommunityDetailResponse> => {
    const res = await apiClient.get<CommunityDetailResponse>(`/community/${id}`, { params });
    return res.data;
  },

  // POST api/community/create
  createCommunity: async (data: CreateCommunityPayload): Promise<void> => {
    await apiClient.post("/community/create", data);
  },

  // GET api/community/mine — returns communities the logged-in user belongs to (PagedResult<Community>)
  getMyCommunities: async (params?: { page?: number; pageSize?: number }): Promise<PagedResult<Community>> => {
    const res = await apiClient.get<PagedResult<Community>>("/community/mine", { params });
    return res.data;
  },

  // POST api/community/{id}/join
  joinCommunity: async (id: string): Promise<void> => {
    await apiClient.post(`/community/${id}/join`);
  },
};
