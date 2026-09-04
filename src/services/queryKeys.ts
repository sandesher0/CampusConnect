// Ref: workflow.md §6 State Management Strategy | Feature: Query Keys
export const queryKeys = {
  // Auth
  auth: {
    me: ["auth", "me"],
  },

  // Users
  users: {
    all: ["users"],
    detail: (id: string) => ["users", id],
    search: (params: Record<string, any>) => [...["users", "search"], params],
  },

  // Events
  events: {
    all: ["events"],
    lists: () => [...queryKeys.events.all, "list"],
    list: (filters: Record<string, any>) => [...queryKeys.events.lists(), filters],
    detail: (id: string) => [...queryKeys.events.all, "detail", id],
  },

  // Resources
  resources: {
    all: ["resources"],
    lists: () => [...queryKeys.resources.all, "list"],
    list: (filters: Record<string, any>) => [...queryKeys.resources.lists(), filters],
    detail: (id: string) => [...queryKeys.resources.all, "detail", id],
  },

  // Communities
  communities: {
    all: ["communities"],
    lists: () => [...queryKeys.communities.all, "list"],
    list: (filters: Record<string, any>) => [...queryKeys.communities.lists(), filters],
    detail: (id: string) => [...queryKeys.communities.all, "detail", id],
  },
};