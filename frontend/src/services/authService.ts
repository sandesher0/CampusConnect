// Ref: workflow.md §4 Architecture Patterns | Feature: API Layer
import apiClient from "@/config/apiClient";
import { z } from "zod";
import { userService } from "./userService";

// Auth schemas - adjusted to match actual backend responses
export const LoginResponseSchema = z.object({
  accessToken: z.string(),
});

export const RegisterResponseSchema = z.object({
  message: z.string(),
});

// User schema (shared with userService)
export const UserSchema = z.object({
  id: z.string(),
  email: z.string().email(),
  firstName: z.string(),
  lastName: z.string(),
  role: z.enum(["student", "admin", "moderator"]),
  isActive: z.boolean(),
  createdAt: z.string().datetime(),
  updatedAt: z.string().datetime(),
});

export type LoginResponse = z.infer<typeof LoginResponseSchema>;
export type RegisterResponse = z.infer<typeof RegisterResponseSchema>;
export type User = z.infer<typeof UserSchema>;

// Auth service functions
export const authService = {
  login: async (username: string, password: string): Promise<{ accessToken: string; user: User }> => {
    // Step 1: Login to get access token
    const loginRes = await apiClient.post("/auth/login", {
      username,
      password,
    });
    const loginResponse = LoginResponseSchema.parse(loginRes.data);
    const accessToken = loginResponse.accessToken;

    // Step 2: Get user information using the token
    const user = await userService.getCurrentUser();

    return { accessToken, user };
  },

  register: async (
    username: string,
    firstName: string,
    lastName: string,
    email: string,
    password: string,
    role: string = "student"
  ): Promise<{ accessToken: string; user: User }> => {
    // Step 1: Register the new user
    await apiClient.post("/auth/register", {
      username,
      firstName,
      lastName,
      email,
      password,
      role,
    });

    // Step 2: Login with the newly created credentials to get token
    const loginRes = await apiClient.post("/auth/login", {
      username,
      password,
    });
    const loginResponse = LoginResponseSchema.parse(loginRes.data);
    const accessToken = loginResponse.accessToken;

    // Step 3: Get user information using the token
    const user = await userService.getCurrentUser();

    return { accessToken, user };
  },

  // Additional methods can be added as needed (e.g., logout, refreshToken)
};