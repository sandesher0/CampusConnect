import type { Metadata } from "next";
import LoginPage from "./LoginPage";

export const metadata: Metadata = {
  title: "Log in",
  description: "Sign in to your CampusConnect student account.",
};

export default function Page() {
  return <LoginPage />;
}
