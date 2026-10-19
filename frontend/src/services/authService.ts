// Ref: workflow.md §4 Architecture Patterns | Feature: API Layer
import apiClient from "@/config/apiClient";
import { userService } from "./userService";
import type { AccountInfo } from "@/types/user";

export interface RegisterPayload {
  username: string;
  email: string;
  password: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string | null;
}

export interface LoginPayload {
  username: string;
  password: string;
}

export interface ChangePasswordPayload {
  oldPassword: string;
  newPassword: string;
}

export const authService = {
  // POST api/auth/login
  login: async (payload: LoginPayload): Promise<AccountInfo> => {
    await apiClient.post("/auth/login", payload);
    return userService.getCurrentUser();
  },

  // POST api/auth/register
  register: async (payload: RegisterPayload): Promise<AccountInfo> => {
    await apiClient.post("/auth/register", payload);
    await apiClient.post("/auth/login", {
      username: payload.username,
      password: payload.password,
    });
    return userService.getCurrentUser();
  },

  // POST api/auth/logout
  logout: async (): Promise<void> => {
    await apiClient.post("/auth/logout");
  },

  // PATCH api/auth/change-password
  changePassword: async (payload: ChangePasswordPayload): Promise<string> => {
    const res = await apiClient.patch<string>("/auth/change-password", payload);
    return res.data;
  },
};
