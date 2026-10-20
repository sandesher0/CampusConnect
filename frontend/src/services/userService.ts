// Ref: workflow.md §4 Architecture Patterns | Feature: API Layer
import apiClient from "@/config/apiClient";
import type { AccountInfo, UserLimitsResponse } from "@/types/user";

export const userService = {
  // GET api/user/me
  getCurrentUser: async (): Promise<AccountInfo> => {
    const res = await apiClient.get<AccountInfo>("/user/me");
    return res.data;
  },

  // GET api/user/limits
  getUserLimits: async (): Promise<UserLimitsResponse> => {
    const res = await apiClient.get<UserLimitsResponse>("/user/limits");
    return res.data;
  },
};