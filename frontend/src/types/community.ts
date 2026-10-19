// Ref: workflow.md §4 Architecture Patterns | Feature: Community Types
import type { PagedResult } from "./pagination";

export type CommunityType = "None" | "Public" | "Private";

export type CommunityStatusType = "None" | "Active" | "Suspended" | "Inactive" | "Closed";

export enum MemberType {
  None = 0,
  ClubOfficer = 1,
  Student = 2,
  ClubMember = 3,
  StudentAffairsStaff = 4,
}

export const MemberTypeLabels: Record<MemberType, string> = {
  [MemberType.None]: "None",
  [MemberType.ClubOfficer]: "Club Officer",
  [MemberType.Student]: "Student",
  [MemberType.ClubMember]: "Club Member",
  [MemberType.StudentAffairsStaff]: "Student Affairs Staff",
};

export interface Community {
  id: string;
  communityName: string;
  communityType: CommunityType;
  createdBy: string;
  status: CommunityStatusType;
}

export interface CommunityUserResponse {
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
  profileImageUrl?: string | null;
}

export interface CommunityMemberResponse {
  memberId: string;
  communityId: string;
  userId: string;
  memberType: MemberType;
  createdAt: string;
  modifiedAt?: string | null;
  user: CommunityUserResponse;
}

export interface CommunityDetailResponse {
  communityId: string;
  communityName: string;
  communityType: CommunityType;
  createdBy: string;
  communityMembers: PagedResult<CommunityMemberResponse>;
}
