// Ref: workflow.md §4 Architecture Patterns | Feature: API Layer
import apiClient from "@/config/apiClient";
import type { PagedResult } from "@/types/pagination";
import type {
  EventSummaryResponse,
  EventDetailResponse,
  EventReservationSummaryResponse,
  MyEventReservationResponse,
  CreateEventRequest,
} from "@/types/event";

export const eventService = {
  // GET api/event/all — returns PagedResult<EventSummaryResponse>
  getEvents: async (params?: { page?: number; pageSize?: number }): Promise<PagedResult<EventSummaryResponse>> => {
    const res = await apiClient.get<PagedResult<EventSummaryResponse>>("/event/all", { params });
    return res.data;
  },

  // GET api/event/{eventId} — returns EventDetailResponse
  getEventById: async (id: string): Promise<EventDetailResponse> => {
    const res = await apiClient.get<EventDetailResponse>(`/event/${id}`);
    return res.data;
  },

  // GET api/event/reservation/{eventId} — returns PagedResult<EventReservationSummaryResponse>
  getEventReservations: async (
    id: string,
    params?: { page?: number; pageSize?: number }
  ): Promise<PagedResult<EventReservationSummaryResponse>> => {
    const res = await apiClient.get<PagedResult<EventReservationSummaryResponse>>(`/event/reservation/${id}`, { params });
    return res.data;
  },

  // GET api/event/reservations/mine — returns PagedResult<MyEventReservationResponse>
  getMyReservations: async (
    params?: { page?: number; pageSize?: number }
  ): Promise<PagedResult<MyEventReservationResponse>> => {
    const res = await apiClient.get<PagedResult<MyEventReservationResponse>>("/event/reservations/mine", { params });
    return res.data;
  },

  // DELETE api/event/reservation/{eventId} — cancel a reservation
  cancelReservation: async (eventId: string): Promise<void> => {
    await apiClient.delete(`/event/reservation/${eventId}`);
  },

  // POST api/event/create
  createEvent: async (data: CreateEventRequest): Promise<void> => {
    await apiClient.post("/event/create", data);
  },

  // POST api/event/join?communityId={communityId}&eventId={eventId}
  joinEvent: async (eventId: string, communityId: string): Promise<void> => {
    await apiClient.post("/event/join", undefined, {
      params: { eventId, communityId },
    });
  },
};
