// Ref: workflow.md §6 State Management Strategy | Feature: Query Keys

export const queryKeys = {
  // Auth & Current User
  auth: {
    me: () => ["auth", "me"] as const,
  },

  // Events
  events: {
    all: () => ["events"] as const,
    list: (params?: { page?: number; pageSize?: number }) =>
      [...queryKeys.events.all(), "list", params ?? {}] as const,
    detail: (id: string) => [...queryKeys.events.all(), "detail", id] as const,
    reservations: (id: string, params?: { page?: number; pageSize?: number }) =>
      [...queryKeys.events.detail(id), "reservations", params ?? {}] as const,
    myReservations: (params?: { page?: number; pageSize?: number }) =>
      [...queryKeys.events.all(), "myReservations", params ?? {}] as const,
  },

  // Communities
  communities: {
    all: () => ["communities"] as const,
    list: (params?: { page?: number; pageSize?: number }) =>
      [...queryKeys.communities.all(), "list", params ?? {}] as const,
    detail: (id: string, params?: { page?: number; pageSize?: number }) =>
      [...queryKeys.communities.all(), "detail", id, params ?? {}] as const,
    mine: (params?: { page?: number; pageSize?: number }) =>
      [...queryKeys.communities.all(), "mine", params ?? {}] as const,
    search: (name: string, params?: { page?: number; pageSize?: number }) =>
      [...queryKeys.communities.all(), "search", name, params ?? {}] as const,
  },
};
