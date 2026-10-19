import type { Metadata } from "next";
import ResourcesPage from "./ResourcesPage";

export const metadata: Metadata = {
  title: "Resources",
  description: "Explore campus academic, health, and campus facilities resources.",
};

export default function Page() {
  return <ResourcesPage />;
}
