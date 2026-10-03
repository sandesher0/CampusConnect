import axios from "axios";

type ErrorResponse = {
  detail?: unknown;
  message?: unknown;
  title?: unknown;
  errors?: unknown;
};

function firstValidationError(errors: unknown): string | undefined {
  if (!errors || typeof errors !== "object") return undefined;

  for (const value of Object.values(errors as Record<string, unknown>)) {
    if (Array.isArray(value) && typeof value[0] === "string") return value[0];
  }

  return undefined;
}

/** Converts API and network failures into a safe, user-facing message. */
export function getApiErrorMessage(error: unknown, fallback: string): string {
  if (axios.isAxiosError(error)) {
    const data = error.response?.data as ErrorResponse | string | undefined;

    if (typeof data === "string" && data.trim()) return data;
    if (data && typeof data === "object") {
      if (typeof data.detail === "string" && data.detail) return data.detail;
      if (typeof data.message === "string" && data.message) return data.message;
      if (typeof data.title === "string" && data.title) return data.title;

      const validationError = firstValidationError(data.errors);
      if (validationError) return validationError;
    }

    if (!error.response) return "Unable to reach the server. Please check your connection and try again.";
  }

  return error instanceof Error && error.message ? error.message : fallback;
}
