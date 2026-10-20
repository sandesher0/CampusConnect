// Ref: workflow.md §4 Architecture Patterns | Feature: API Types & Error Model

export interface ProblemDetails {
  type?: string;
  title?: string;
  status?: number;
  detail?: string;
  instance?: string;
  code?: string;
  limitKey?: string;
  limit?: number;
}

export interface ValidationProblemDetails extends ProblemDetails {
  errors?: Record<string, string[]>;
}

export class ApiError extends Error {
  public readonly status: number;
  public readonly title?: string;
  public readonly detail?: string;
  public readonly fieldErrors?: Record<string, string[]>;
  public readonly code?: string;
  public readonly limitKey?: string;
  public readonly limit?: number;

  constructor(
    status: number,
    message: string,
    title?: string,
    detail?: string,
    fieldErrors?: Record<string, string[]>,
    code?: string,
    limitKey?: string,
    limit?: number
  ) {
    super(message);
    this.name = "ApiError";
    this.status = status;
    this.title = title;
    this.detail = detail;
    this.fieldErrors = fieldErrors;
    this.code = code;
    this.limitKey = limitKey;
    this.limit = limit;
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
