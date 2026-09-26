// Ref: workflow.md §4 Architecture Patterns | Feature: API Layer
import apiClient from "@/config/apiClient";
import { z } from "zod";

// EventResponse schema — matches GET /api/event/all response
// Enums are serialized as strings via JsonStringEnumConverter
// Dates are DateTimeOffset (ISO 8601 with offset)
export const EventSchema = z.object({
  id: z.string(),
  title: z.string(),
  description: z.string(),
  location: z.string(),
  communityId: z.string(),
  createdBy: z.string(),
  eventDate: z.string(),
  eventEndDate: z.string(),
  visibility: z.union([z.string(), z.number()]).transform((v) => String(v)),
  category: z.union([z.string(), z.number()]).transform((v) => String(v)),
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
  // GET /api/event/all — returns List<EventResponse> (public events only)
  getEvents: async (): Promise<Event[]> => {
    const res = await apiClient.get("/event/all");
    return z.array(EventSchema).parse(res.data);
  },

  getEventById: async (id: string): Promise<Event> => {
    const res = await apiClient.get(`/event/${id}`);
    return EventSchema.parse(res.data);
  },

  // POST /api/event/create
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