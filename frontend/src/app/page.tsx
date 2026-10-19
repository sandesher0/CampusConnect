import type { Metadata } from "next";
import HomePage from "./HomePage";

export const metadata: Metadata = {
  title: "Home",
  description: "Find your people. Find your next thing on campus.",
};

export default function Page() {
  return <HomePage />;
}
