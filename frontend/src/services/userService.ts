// Ref: workflow.md §4 Architecture Patterns | Feature: API Layer
import apiClient from "@/config/apiClient";
import { z } from "zod";

// User schema (will be shared with validations later)
export const UserSchema = z.object({
  id: z.string(),
  userId: z.string(),
  username: z.string(),
  accountVerifiedAt: z.string().datetime().optional().nullable(),
  user: z.object({
    id: z.string(),
    email: z.string().email(),
    firstName: z.string(),
    lastName: z.string(),
  })
});

export type User = z.infer<typeof UserSchema>;

// Service functions
export const userService = {
  getCurrentUser: async (): Promise<User> => {
    const res = await apiClient.get("/user/me");
    return UserSchema.parse(res.data);
  },

  getUserById: async (id: string): Promise<User> => {
    const res = await apiClient.get(`/users/${id}`);
    return UserSchema.parse(res.data);
  },

  updateUser: async (id: string, data: Partial<User>): Promise<User> => {
    const res = await apiClient.patch(`/users/${id}`, data);
    return UserSchema.parse(res.data);
  },

  // Additional methods can be added as needed
};