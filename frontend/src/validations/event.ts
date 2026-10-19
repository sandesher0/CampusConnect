// Ref: workflow.md §13 Component & API Documentation Template | Feature: Event Validation
import { z } from "zod";

export const createEventSchema = z
  .object({
    title: z
      .string()
      .min(1, "Title is required")
      .min(3, "Title must be at least 3 characters")
      .max(200, "Title must not exceed 200 characters"),
    description: z
      .string()
      .min(1, "Description is required")
      .min(10, "Description must be at least 10 characters"),
    location: z
      .string()
      .min(1, "Location is required")
      .min(2, "Location must be at least 2 characters"),
    communityId: z
      .string()
      .min(1, "CommunityId is required")
      .uuid("Invalid community ID"),
    eventDate: z.string().min(1, "Start date is required"),
    eventEndDate: z.string().min(1, "End date is required"),
    visibility: z.enum(["Public", "Private"], {
      error: "Invalid event visibility",
    }),
    category: z.enum(
      [
        "Academic",
        "Social",
        "Sports",
        "Cultural",
        "Career",
        "Volunteering",
        "Other",
      ],
      {
        error: "Invalid event category",
      }
    ),
  })
  .refine(
    (data) => {
      const start = new Date(data.eventDate);
      const end = new Date(data.eventEndDate);
      return !isNaN(start.getTime()) && !isNaN(end.getTime()) && end > start;
    },
    {
      message: "End date must be after start date",
      path: ["eventEndDate"],
    }
  );

export type CreateEventValues = z.infer<typeof createEventSchema>;
