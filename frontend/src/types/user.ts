// Ref: workflow.md §4 Architecture Patterns | Feature: User Types
import type { components } from "./api.generated";

export type UserInfo = components["schemas"]["UserInfo"];
export type AccountInfo = components["schemas"]["AccountInfo"];
export type UserLimitsResponse = components["schemas"]["UserLimitsResponse"];
export type LimitDetailResponse = components["schemas"]["LimitDetailResponse"];

// Backward-compatibility alias
export type User = AccountInfo;
