// Ref: workflow.md §4 Architecture Patterns | Feature: API Layer
import apiClient from "@/config/apiClient";
import { z } from "zod";

// Fields shared by the event list and detail responses.
// Enums are serialized as strings via JsonStringEnumConverter
// Dates are DateTimeOffset (ISO 8601 with offset)
const EventBaseSchema = z.object({
  id: z.string(),
  title: z.string(),
  location: z.string(),
  eventDate: z.string(),
  eventEndDate: z.string(),
  visibility: z.union([z.string(), z.number()]).transform((v) => String(v)),
  category: z.union([z.string(), z.number()]).transform((v) => String(v)),
});

// GET /api/event/all returns the lightweight event-card representation.
export const EventSummarySchema = EventBaseSchema;
export type EventSummary = z.infer<typeof EventSummarySchema>;

const EventOrganizerSchema = z.object({
  id: z.string(),
  firstName: z.string(),
  lastName: z.string(),
  profileImageUrl: z.string().nullable(),
});

const EventOrganizerCommunitySchema = z.object({
  communityId: z.string(),
  communityName: z.string(),
  communityType: z.union([z.string(), z.number()]).transform((v) => String(v)),
});

// GET /api/event/{eventId} returns the full event-detail representation.
export const EventDetailSchema = EventBaseSchema.extend({
  description: z.string(),
  communityId: z.string(),
  createdBy: z.string(),
  organizer: EventOrganizerSchema,
  organizerCommunity: EventOrganizerCommunitySchema,
});
export type EventDetail = z.infer<typeof EventDetailSchema>;

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
  // GET /api/event/all — returns List<EventSummaryResponse> (public events only)
  getEvents: async (): Promise<EventSummary[]> => {
    const res = await apiClient.get("/event/all");
    return z.array(EventSummarySchema).parse(res.data);
  },

  // GET /api/event/{eventId} — returns one EventDetailResponse
  getEventById: async (id: string): Promise<EventDetail> => {
    const res = await apiClient.get(`/event/${id}`);
    return EventDetailSchema.parse(res.data);
  },

  // POST /api/event/create
  createEvent: async (data: CreateEventPayload): Promise<void> => {
    await apiClient.post("/event/create", data);
  },

  updateEvent: async (id: string, data: Partial<EventDetail>): Promise<void> => {
    await apiClient.patch(`/event/${id}`, data);
  },

  deleteEvent: async (id: string): Promise<void> => {
    await apiClient.delete(`/event/${id}`);
  },
};
