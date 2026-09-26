// Ref: workflow.md §4 Architecture Patterns | Feature: API Layer
import apiClient from "@/config/apiClient";
import { z } from "zod";

// Event schema
export const EventSchema = z.object({
  id: z.string(),
  title: z.string(),
  description: z.string(),
  location: z.string(),
  communityId: z.string(),
  createdBy: z.string(),
  eventDate: z.string().datetime(),
  eventEndDate: z.string().datetime(),
  visibility: z.enum(["None", "Public", "Private"]),
  category: z.enum(["None", "Academic", "Social", "Sports", "Cultural", "Career", "Volunteering", "Other"]),
  createAt: z.string().datetime(),
  updatedAt: z.string().datetime().optional().nullable(),
});

export type Event = z.infer<typeof EventSchema>;

export type CreateEventPayload = {
  title: string;
  description: string;
  location: string;
  communityId: string;
  eventDate: string;       // ISO DateTimeOffset
  eventEndDate: string;    // ISO DateTimeOffset
  visibility: "Public" | "Private";
  category: "Academic" | "Social" | "Sports" | "Cultural" | "Career" | "Volunteering" | "Other";
};

// Service functions
export const eventService = {
  getEvents: async (filters?: Record<string, any>): Promise<Event[]> => {
    const res = await apiClient.get("/event", { params: filters });
    return z.array(EventSchema).parse(res.data);
  },

  getEventById: async (id: string): Promise<Event> => {
    const res = await apiClient.get(`/event/${id}`);
    return EventSchema.parse(res.data);
  },

  createEvent: async (data: CreateEventPayload): Promise<void> => {
    await apiClient.post("/event/create", data);
  },

  updateEvent: async (id: string, data: Partial<Event>): Promise<void> => {
    await apiClient.patch(`/event/${id}`, data);
  },

  deleteEvent: async (id: string): Promise<void> => {
    await apiClient.delete(`/event/${id}`);
  },
};