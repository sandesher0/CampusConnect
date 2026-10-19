import type { Metadata } from "next";
import MineCommunitiesPage from "./MineCommunitiesPage";

export const metadata: Metadata = {
  title: "My Communities",
  description: "View communities you have joined or created.",
};

export default function Page() {
  return <MineCommunitiesPage />;
}
