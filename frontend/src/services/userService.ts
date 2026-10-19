// Ref: workflow.md §4 Architecture Patterns | Feature: API Layer
import apiClient from "@/config/apiClient";
import type { AccountInfo } from "@/types/user";

export const userService = {
  // GET api/user/me
  getCurrentUser: async (): Promise<AccountInfo> => {
    const res = await apiClient.get<AccountInfo>("/user/me");
    return res.data;
  },
};