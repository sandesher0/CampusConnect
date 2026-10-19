import type { Metadata } from "next";
import CommunityDetailPage from "./CommunityDetailPage";

export const metadata: Metadata = {
  title: "Community Details",
  description: "View community members, details, and join discussions.",
};

export default function Page() {
  return <CommunityDetailPage />;
}
