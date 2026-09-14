// Ref: workflow.md §4 Architecture Patterns | Feature: API Layer
import apiClient from "@/config/apiClient";
import { z } from "zod";

// Event schema
export const EventSchema = z.object({
  id: z.string(),
  title: z.string(),
  description: z.string(),
  date: z.string().datetime(),
  location: z.string(),
  organizerId: z.string(),
  category: z.string(),
  isPublic: z.boolean(),
  createdAt: z.string().datetime(),
  updatedAt: z.string().datetime(),
});

export type Event = z.infer<typeof EventSchema>;

// Service functions
export const eventService = {
  getEvents: async (filters?: Record<string, any>): Promise<Event[]> => {
    const res = await apiClient.get("/events", { params: filters });
    return z.array(EventSchema).parse(res.data);
  },

  getEventById: async (id: string): Promise<Event> => {
    const res = await apiClient.get(`/events/${id}`);
    return EventSchema.parse(res.data);
  },

  createEvent: async (data: Omit<Event, "id" | "createdAt" | "updatedAt">): Promise<Event> => {
    const res = await apiClient.post("/events", data);
    return EventSchema.parse(res.data);
  },

  updateEvent: async (id: string, data: Partial<Event>): Promise<Event> => {
    const res = await apiClient.patch(`/events/${id}`, data);
    return EventSchema.parse(res.data);
  },

  deleteEvent: async (id: string): Promise<void> => {
    await apiClient.delete(`/events/${id}`);
  },
};