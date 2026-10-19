// Ref: workflow.md §6 State Management Strategy | Feature: Auth Store
import { create } from "zustand";
import { persist } from "zustand/middleware";
import { authService } from "@/services/authService";
import type { AccountInfo } from "@/types/user";

interface AuthState {
  user: AccountInfo | null;
  isLoading: boolean;
  setUser: (user: AccountInfo | null) => void;
  setLoading: (isLoading: boolean) => void;
  login: (user: AccountInfo) => void;
  logout: () => Promise<void>;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      user: null,
      isLoading: false,
      setUser: (user) => set({ user }),
      setLoading: (isLoading) => set({ isLoading }),
      login: (user) => set({ user, isLoading: false }),
      logout: async () => {
        try {
          await authService.logout();
        } finally {
          set({ user: null, isLoading: false });
        }
      },
    }),
    {
      name: "auth-storage",
      partialize: (state) => ({ user: state.user }),
    }
  )
);
