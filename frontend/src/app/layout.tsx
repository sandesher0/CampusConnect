// Ref: workflow.md §4 Architecture Patterns | Feature: Base Layout
import type { Metadata } from "next";
import { Plus_Jakarta_Sans } from "next/font/google";
import "./globals.css";
import Header from "@/components/layouts/Header";
import Footer from "@/components/layouts/Footer";
import AppQueryClientProvider from "@/app/query-client";

const plusJakartaSans = Plus_Jakarta_Sans({
  variable: "--font-plus-jakarta-sans",
  subsets: ["latin"],
  weight: ["500", "600", "700", "800"],
  display: "swap",
});

export const metadata: Metadata = {
  title: {
    template: "%s | CampusConnect",
    default: "CampusConnect | The Student Campus Directory",
  },
  description: "Connecting college students with campus events, communities, and resources.",
};

export default function RootLayout({ children }: { children: React.ReactNode }) {
  return (
    <html
      lang="en"
      className={`${plusJakartaSans.variable} h-full antialiased`}
    >
      <body className="min-h-full flex flex-col bg-[var(--bg-page)] text-[var(--ink)]">
        <Header />
        <main className="flex-1">
          <AppQueryClientProvider>
            {children}
          </AppQueryClientProvider>
        </main>
        <Footer />
      </body>
    </html>
  );
}
