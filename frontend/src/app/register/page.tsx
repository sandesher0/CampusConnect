import type { Metadata } from "next";
import RegisterPage from "./RegisterPage";

export const metadata: Metadata = {
  title: "Create an Account",
  description: "Sign up for CampusConnect to join campus activities.",
};

export default function Page() {
  return <RegisterPage />;
}
