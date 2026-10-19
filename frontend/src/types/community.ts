// Ref: workflow.md §4 Architecture Patterns | Feature: Community Types
import type { components } from "./api.generated";
import type { PagedResult } from "./pagination";

export type CommunityType = components["schemas"]["CommunityType"];
export type CommunityStatusType = components["schemas"]["CommunityStatusType"];
export type MemberType = components["schemas"]["MemberType"];

export const MemberTypeLabels: Record<MemberType, string> = {
  None: "None",
  ClubOfficer: "Club Officer",
  Student: "Student",
  ClubMember: "Club Member",
  StudentAffairsStaff: "Student Affairs Staff",
};

export type Community = components["schemas"]["Community"] & { id: string };
export type CommunityUserResponse = components["schemas"]["CommunityUserResponse"];

export type CommunityMemberResponse = Omit<components["schemas"]["CommunityMemberResponse"], "memberType"> & {
  memberType: MemberType;
};

export interface CommunityDetailResponse {
  communityId: string;
  communityName: string;
  communityType: CommunityType;
  createdBy: string;
  communityMembers: PagedResult<CommunityMemberResponse>;
}

export type CreateCommunityRequest = components["schemas"]["CreateCommunityRequest"];
