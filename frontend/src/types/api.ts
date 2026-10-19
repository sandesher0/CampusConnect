// Ref: workflow.md §4 Architecture Patterns | Feature: API Types & Error Model

export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
}

export interface ValidationProblemDetails extends ProblemDetails {
  errors?: Record<string, string[]>;
}

export class ApiError extends Error {
  public readonly status: number;
  public readonly title?: string;
  public readonly detail?: string;
  public readonly fieldErrors?: Record<string, string[]>;

  constructor(status: number, message: string, title?: string, detail?: string, fieldErrors?: Record<string, string[]>) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.title = title;
    this.detail = detail;
    this.fieldErrors = fieldErrors;
  }

  public getFirstFieldError(): string | undefined {
    if (!this.fieldErrors) return undefined;
    for (const messages of Object.values(this.fieldErrors)) {
      if (Array.isArray(messages) && messages.length > 0) {
        return messages[0];
      }
    }
    return undefined;
  }
}
