// Ref: workflow.md §4 Architecture Patterns | Feature: Event Types

export type EventVisibility = "None" | "Public" | "Private";

export type EventCategory =
  | "None"
  | "Academic"
  | "Social"
  | "Sports"
  | "Cultural"
  | "Career"
  | "Volunteering"
  | "Other";

export interface EventSummaryResponse {
  id: string;
  title: string;
  location: string;
  eventDate: string;
  eventEndDate: string;
  visibility: EventVisibility;
  category: EventCategory;
}

export interface EventOrganizerUserResponse {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string | null;
  profileImageUrl?: string | null;
}

export interface EventOrganizerCommunityResponse {
  communityId: string;
  communityName: string;
  communityType: "None" | "Public" | "Private";
}

export interface EventDetailResponse {
  id: string;
  title: string;
  description: string;
  location: string;
  communityId: string;
  createdBy: string;
  eventDate: string;
  eventEndDate: string;
  visibility: EventVisibility;
  category: EventCategory;
  organizer: EventOrganizerUserResponse;
  organizerCommunity: EventOrganizerCommunityResponse;
}

export interface EventReservationSummaryResponse {
  id: string;
  userId: string;
  reservedAt: string;
  firstName: string;
  lastName: string;
  email: string;
  profileImageUrl?: string | null;
}

export interface MyEventReservationResponse {
  reservationId: string;
  eventId: string;
  reservedAt: string;
  title: string;
  description: string;
  location: string;
  eventDate: string;
  eventEndDate: string;
  visibility: EventVisibility;
  category: EventCategory;
  communityId: string;
}

export interface CreateEventRequest {
  title: string;
  description: string;
  location: string;
  communityId: string;
  eventDate: string;
  eventEndDate: string;
  visibility: EventVisibility;
  category: EventCategory;
}
