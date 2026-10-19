import type { Metadata } from "next";
import ProfilePage from "./ProfilePage";

export const metadata: Metadata = {
  title: "Profile",
  description: "Manage your CampusConnect account details and security settings.",
};

export default function Page() {
  return <ProfilePage />;
}
