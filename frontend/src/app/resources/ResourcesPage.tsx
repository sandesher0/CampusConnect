// Ref: workflow.md §4 Architecture Patterns | Feature: Resources
import PageHeader from "@/components/common/PageHeader";
import EmptyState from "@/components/common/EmptyState";
import Button from "@/components/common/Button";

export default function ResourcesPage() {
  return (
    <div className="mx-auto max-w-[1200px] px-6 py-12 sm:px-8">
      <PageHeader
        eyebrow="CAMPUS SERVICES"
        title="Resources"
        description="Academic support, wellness centers, university equipment, and study spaces."
      />

      <EmptyState
        title="Resource directory coming soon"
        description="The campus resources module is currently under development. In the meantime, explore campus communities and upcoming events."
        action={
          <div className="flex flex-wrap justify-center gap-3">
            <Button href="/events" variant="primary">
              Browse Events
            </Button>
            <Button href="/community" variant="outline">
              Explore Communities
            </Button>
          </div>
        }
      />
    </div>
  );
}
