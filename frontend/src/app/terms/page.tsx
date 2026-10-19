import type { Metadata } from "next";
import Button from "@/components/common/Button";

export const metadata: Metadata = {
  title: "Terms of Service",
  description: "CampusConnect Terms of Service.",
};

export default function TermsPage() {
  return (
    <div className="mx-auto max-w-[800px] px-6 py-16 sm:px-8">
      <div className="bg-white rounded-[28px] border border-[#E4DEF7] p-8 sm:p-12 shadow-[0_1px_2px_rgba(27,21,48,0.05),0_8px_24px_rgba(27,21,48,0.05)]">
        <h1 className="text-3xl font-extrabold text-[#1B1530] mb-4">Terms of Service</h1>
        <div className="space-y-4 text-[#5A5478] leading-relaxed text-base">
          <p>
            Welcome to CampusConnect. By using this platform, you acknowledge that CampusConnect is developed as an educational project.
          </p>
          <p>
            Users are expected to adhere to standard campus honor codes and community guidelines when creating communities, posting events, and reserving spots.
          </p>
          <p>
            Official campus terms of service will be established prior to institution-wide integration.
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
