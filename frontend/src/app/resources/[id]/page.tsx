import type { Metadata } from "next";
import ResourceDetailPage from "./ResourceDetailPage";

export const metadata: Metadata = {
  title: "Resource Details",
  description: "View university department and facility resource details.",
};

export default function Page() {
  return <ResourceDetailPage />;
}
