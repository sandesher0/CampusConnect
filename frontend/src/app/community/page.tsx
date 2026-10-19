import type { Metadata } from "next";
import CommunityPage from "./CommunityPage";

export const metadata: Metadata = {
  title: "Communities",
  description: "Browse and discover campus clubs, organizations, and groups.",
};

export default function Page() {
  return <CommunityPage />;
}