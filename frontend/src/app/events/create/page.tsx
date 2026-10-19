import type { Metadata } from "next";
import CreateEventPage from "./CreateEventPage";

export const metadata: Metadata = {
  title: "Create Event",
  description: "Organize and publish an event for your community.",
};

export default function Page() {
  return <CreateEventPage />;
}
