import type { Metadata } from "next";
import EventsPage from "./EventsPage";

export const metadata: Metadata = {
  title: "Events",
  description: "Explore upcoming campus events, talks, activities, and mixers.",
};

export default function Page() {
  return <EventsPage />;
}
