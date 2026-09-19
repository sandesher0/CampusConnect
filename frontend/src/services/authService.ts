// Ref: workflow.md §4 Architecture Patterns | Feature: API Layer
import apiClient from "@/config/apiClient";
import { z } from "zod";
import jwtDecode from "jwt-decode";

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
  login: async (username: string, password: string): Promise<{ accessToken: string; user?: User }> => {
    const res = await apiClient.post("/auth/login", {
      username,
      password,
    });
    const loginResponse = LoginResponseSchema.parse(res.data);
    const accessToken = loginResponse.accessToken;

    // Try to decode JWT to get user information
    let user: User | undefined;
    try {
      const decoded: any = jwtDecode(accessToken);
      // Map decoded token to User schema
      user = {
        id: decoded.sub || decoded.id || "",
        email: decoded.email || "",
        firstName: decoded.firstName || "",
        lastName: decoded.lastName || "",
        role: (decoded.role || decoded.rol || "student") as "student" | "admin" | "moderator",
        isActive: decoded.isActive !== undefined ? !!decoded.isActive : true,
        createdAt: decoded.createdAt || new Date().toISOString(),
        updatedAt: decoded.updatedAt || new Date().toISOString(),
      };
    } catch (error) {
      console.warn("Failed to decode JWT token:", error);
      // If we can't decode token, we'll rely on having just the token for now
      // The user information may need to be fetched separately if needed
    }

    return { accessToken, user };
  },

  register: async (
    username: string,
    firstName: string,
    lastName: string,
    email: string,
    password: string,
    role: string = "student"
  ): Promise<{ accessToken?: string; user?: User }> => {
    const res = await apiClient.post("/auth/register", {
      username,
      firstName,
      lastName,
      email,
      password,
      role,
    });
    const registerResponse = RegisterResponseSchema.parse(res.data);

    // Register endpoint only returns a message, not a token
    // This suggests we might need to login after registration to get the token
    // Or the token might be in a different format than expected

    console.log("Registration response:", registerResponse);

    // For now, return empty object - caller may need to handle this appropriately
    // In a real implementation, we might:
    // 1. Automatically login after registration
    // 2. Expect the token to be returned in a different way
    // 3. Have a separate endpoint to get token after registration

    return {};
  },

  // Additional methods can be added as needed (e.g., logout, refreshToken)
};