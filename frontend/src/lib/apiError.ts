// Ref: workflow.md §4 Architecture Patterns | Feature: API Error Handling
import axios, { AxiosError } from "axios";
import { ApiError, ProblemDetails, ValidationProblemDetails } from "@/types/api";

export function toApiError(error: unknown): ApiError {
  if (error instanceof ApiError) {
    return error;
  }

  if (axios.isAxiosError(error)) {
    const axiosError = error as AxiosError<ValidationProblemDetails | ProblemDetails | string>;
    const status = axiosError.response?.status ?? (axiosError.code === "ECONNABORTED" ? 408 : 0);
    const data = axiosError.response?.data;

    let title: string | undefined;
    let detail: string | undefined;
    let fieldErrors: Record<string, string[]> | undefined;
    let code: string | undefined;
    let limitKey: string | undefined;
    let limit: number | undefined;

    if (typeof data === "string" && data.trim()) {
      detail = data;
    } else if (data && typeof data === "object") {
      const prob = data as ProblemDetails;
      title = prob.title;
      detail = prob.detail;
      code = prob.code;
      limitKey = prob.limitKey;
      limit = prob.limit;
      fieldErrors = (data as ValidationProblemDetails).errors;
    }

    const message =
      (fieldErrors && Object.values(fieldErrors).flat()[0]) ||
      detail ||
      title ||
      axiosError.message ||
      "An unexpected error occurred.";

    return new ApiError(status, message, title, detail, fieldErrors, code, limitKey, limit);
  }

  if (error instanceof Error) {
    return new ApiError(0, error.message);
  }

  return new ApiError(0, String(error || "An unknown error occurred."));
}

/** Converts API and network failures into a safe, user-facing message. */
export function getApiErrorMessage(error: unknown, fallback: string = "An error occurred. Please try again."): string {
  if (error instanceof ApiError) {
    return error.getFirstFieldError() || error.detail || error.title || error.message || fallback;
  }

  const apiError = toApiError(error);
  if (apiError.status === 0 && apiError.message.includes("Network")) {
    return "Unable to reach the server. Please check your connection and try again.";
  }

  return apiError.getFirstFieldError() || apiError.detail || apiError.title || apiError.message || fallback;
}
