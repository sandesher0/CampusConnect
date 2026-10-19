import type { Metadata } from "next";
import Button from "@/components/common/Button";

export const metadata: Metadata = {
  title: "Privacy Policy",
  description: "CampusConnect Privacy Policy.",
};

export default function PrivacyPage() {
  return (
    <div className="mx-auto max-w-[800px] px-6 py-16 sm:px-8">
      <div className="bg-white rounded-[28px] border border-[#E4DEF7] p-8 sm:p-12 shadow-[0_1px_2px_rgba(27,21,48,0.05),0_8px_24px_rgba(27,21,48,0.05)]">
        <h1 className="text-3xl font-extrabold text-[#1B1530] mb-4">Privacy Policy</h1>
        <div className="space-y-4 text-[#5A5478] leading-relaxed text-base">
          <p>
            CampusConnect is an academic course project built to connect university students with campus events, communities, and resources.
          </p>
          <p>
            Currently, accounts, events, and community registrations created on this platform are for demonstration and course evaluation purposes. Full formal privacy policies and data management guidelines will be released as the application transitions to production campus deployment.
          </p>
          <p>
            If you have any questions regarding your test account data, please contact the course instruction team.
          </p>
        </div>
        <div className="mt-8 pt-6 border-t border-[#ECE7FA]">
          <Button href="/" variant="outline">
            Back to Home
          </Button>
        </div>
      </div>
    </div>
  );
}
