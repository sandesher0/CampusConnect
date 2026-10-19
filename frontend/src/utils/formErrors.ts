// Ref: workflow.md §4 Architecture Patterns | Feature: Form Error Mapping
import type { UseFormSetError, FieldValues, Path } from "react-hook-form";
import { ApiError } from "@/types/api";

export function handleServerValidationErrors<TFieldValues extends FieldValues>(
  error: unknown,
  setError: UseFormSetError<TFieldValues>
): boolean {
  if (error instanceof ApiError && error.fieldErrors) {
    let hasFieldErrors = false;
    for (const [field, messages] of Object.entries(error.fieldErrors)) {
      if (Array.isArray(messages) && messages.length > 0) {
        // Find matching field in camelCase or lowercase
        const fieldName = (field.charAt(0).toLowerCase() + field.slice(1)) as Path<TFieldValues>;
        setError(fieldName, {
          type: "server",
          message: messages[0],
        });
        hasFieldErrors = true;
      }
    }
    return hasFieldErrors;
  }
  return false;
}
