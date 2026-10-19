import type { Metadata } from "next";
import EventDetailPage from "./EventDetailPage";

export const metadata: Metadata = {
  title: "Event Details",
  description: "View event schedule, location, and reserve your spot.",
};

export default function Page() {
  return <EventDetailPage />;
}
