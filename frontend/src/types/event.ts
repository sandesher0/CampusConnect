// Ref: workflow.md §4 Architecture Patterns | Feature: Event Types
import type { components } from "./api.generated";

export type EventVisibility = components["schemas"]["EventVisibility"];
export type EventCategory = components["schemas"]["EventCategory"];

export type EventSummaryResponse = components["schemas"]["EventSummaryResponse"];
export type EventOrganizerUserResponse = components["schemas"]["EventOrganizerUserResponse"];
export type EventOrganizerCommunityResponse = components["schemas"]["EventOrganizerCommunityResponse"];
export type EventDetailResponse = components["schemas"]["EventDetailResponse"];
export type EventReservationSummaryResponse = components["schemas"]["EventReservationSummaryResponse"];
export type MyEventReservationResponse = components["schemas"]["MyEventReservationResponse"];
export type EventRequest = components["schemas"]["EventRequest"];

// Backward-compatibility alias
export type CreateEventRequest = EventRequest;
