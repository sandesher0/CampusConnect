import type { Metadata } from "next";
import CreateCommunityPage from "./CreateCommunityPage";

export const metadata: Metadata = {
  title: "Create Community",
  description: "Start a new club or chapter on campus.",
};

export default function Page() {
  return <CreateCommunityPage />;
}
