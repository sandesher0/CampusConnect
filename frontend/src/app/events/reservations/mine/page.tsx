import type { Metadata } from "next";
import MyReservationsPage from "./MyReservationsPage";

export const metadata: Metadata = {
  title: "My Reservations",
  description: "View and manage your upcoming event reservations.",
};

export default function Page() {
  return <MyReservationsPage />;
}
