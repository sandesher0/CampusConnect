// Ref: workflow.md §4 Architecture Patterns | Feature: User Types

export interface UserInfo {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string | null;
  profileImageUrl?: string | null;
}

export interface AccountInfo {
  id: string;
  userId: string;
  username: string;
  accountVerifiedAt?: string | null;
  user: UserInfo;
}

// Backward-compatibility alias
export type User = AccountInfo;
