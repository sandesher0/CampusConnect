// Ref: workflow.md §4 Architecture Patterns | Feature: API Layer
import apiClient from "@/config/apiClient";
import { z } from "zod";
import { userService } from "./userService";

// Auth schemas - adjusted to match actual backend responses
export const LoginResponseSchema = z.any();

export const RegisterResponseSchema = z.object({
  message: z.string().optional(),
});

// User schema (shared with userService)
export const UserSchema = z.object({
  id: z.string(),
  userId: z.string(),
  username: z.string(),
  user: z.object({
    id: z.string(),
    email: z.string().email(),
    firstName: z.string(),
    lastName: z.string(),
  })
});

export type LoginResponse = z.infer<typeof LoginResponseSchema>;
export type RegisterResponse = z.infer<typeof RegisterResponseSchema>;
export type User = z.infer<typeof UserSchema>;

// Auth service functions
export const authService = {
  login: async (username: string, password: string): Promise<{ accessToken: string; user: User }> => {
    // Step 1: Login to get access token in cookie
    await apiClient.post("/auth/login", {
      username,
      password,
    });
    
    const accessToken = "cookie-auth";

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
  ): Promise<{ accessToken: string; user: User }> => {
    // Step 1: Register the new user
    await apiClient.post("/auth/register", {
      username,
      firstName,
      lastName,
      email,
      password,
    });

    // Step 2: Login with the newly created credentials to get token
    await apiClient.post("/auth/login", {
      username,
      password,
    });
    const accessToken = "cookie-auth";

    // Step 3: Get user information using the token
    const user = await userService.getCurrentUser();

    return { accessToken, user };
  },
};