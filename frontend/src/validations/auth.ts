// Ref: workflow.md §13 Component & API Documentation Template | Feature: Auth Validation
import { z } from "zod";

export const loginSchema = z.object({
  username: z
    .string()
    .min(1, "Username is required")
    .min(3, "Username must be at least 3 characters"),
  password: z
    .string()
    .min(1, "Password is required")
    .min(6, "Password must be at least 6 characters"),
});

export const registerSchema = z.object({
  username: z
    .string()
    .min(1, "Username is required")
    .min(3, "Username must be at least 3 characters")
    .max(50, "Username must not exceed 50 characters"),
  email: z
    .string()
    .min(1, "Email is required")
    .email("Invalid email address"),
  firstName: z
    .string()
    .min(1, "First name is required")
    .min(2, "First name must be at least 2 characters")
    .max(50, "First name must not exceed 50 characters"),
  lastName: z
    .string()
    .min(1, "Last name is required")
    .min(2, "Last name must be at least 2 characters")
    .max(50, "Last name must not exceed 50 characters"),
  password: z
    .string()
    .min(1, "Password is required")
    .min(8, "Password must be at least 8 characters")
    .regex(/[a-z]/, "Password must contain at least one lowercase letter")
    .regex(/[A-Z]/, "Password must contain at least one uppercase letter")
    .regex(/[0-9]/, "Password must contain at least one number")
    .regex(/[^a-zA-Z0-9]/, "Password must contain at least one special character"),
  phoneNumber: z
    .string()
    .max(20, "Phone number must not exceed 20 characters")
    .optional()
    .or(z.literal("")),
});

export const changePasswordSchema = z
  .object({
    oldPassword: z.string().min(1, "Old password is required"),
    newPassword: z
      .string()
      .min(1, "New password is required")
      .min(8, "New password must be at least 8 characters")
      .regex(/[a-z]/, "New password must contain at least one lowercase letter")
      .regex(/[A-Z]/, "New password must contain at least one uppercase letter")
      .regex(/[0-9]/, "New password must contain at least one number")
      .regex(/[^a-zA-Z0-9]/, "New password must contain at least one special character"),
  })
  .refine((data) => data.newPassword !== data.oldPassword, {
    message: "New password cannot be the same as old password",
    path: ["newPassword"],
  });

export type LoginValues = z.infer<typeof loginSchema>;
export type RegisterValues = z.infer<typeof registerSchema>;
export type ChangePasswordValues = z.infer<typeof changePasswordSchema>;