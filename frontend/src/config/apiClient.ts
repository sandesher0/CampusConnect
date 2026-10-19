// Ref: workflow.md §4 Architecture Patterns | Feature: API Layer
import axios, { AxiosError, InternalAxiosRequestConfig } from "axios";
import { toApiError } from "@/lib/apiError";
import { useAuthStore } from "@/stores/authStore";

const apiClient = axios.create({
  baseURL: "/api",
  withCredentials: true,
  timeout: 15000, // 15s timeout for free-tier backend cold starts
  headers: {
    "Content-Type": "application/json",
  },
});

// Retry GET requests once on network/5xx errors
apiClient.interceptors.response.use(
  (response) => response,
  async (error: AxiosError) => {
    const config = error.config as (InternalAxiosRequestConfig & { _retryCount?: number }) | undefined;

    // Retry once for idempotent GET requests on 5xx or network errors
    if (config && config.method?.toLowerCase() === "get" && (!config._retryCount || config._retryCount < 1)) {
      const status = error.response?.status;
      if (!status || status >= 500) {
        config._retryCount = (config._retryCount || 0) + 1;
        return apiClient(config);
      }
    }

    const apiError = toApiError(error);

    // If 401 Unauthorized, clear auth store and redirect to login if on browser
    if (apiError.status === 401) {
      if (typeof window !== "undefined") {
        const pathname = window.location.pathname;
        useAuthStore.getState().setUser(null);

        if (!pathname.startsWith("/login") && !pathname.startsWith("/register")) {
          const redirectUrl = `/login?redirect=${encodeURIComponent(pathname + window.location.search)}`;
          window.location.href = redirectUrl;
        }
      }
    }

    return Promise.reject(apiError);
  }
);

export default apiClient;
