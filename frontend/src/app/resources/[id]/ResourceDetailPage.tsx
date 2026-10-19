// Ref: workflow.md §4 Architecture Patterns | Feature: Resources
import EmptyState from "@/components/common/EmptyState";
import Button from "@/components/common/Button";

export default function ResourceDetailPage() {
  return (
    <div className="mx-auto max-w-[800px] px-6 py-16 sm:px-8">
      <EmptyState
        title="Resource Details Coming Soon"
        description="Detailed room booking, equipment requests, and department guides are being finalized."
        action={
          <Button href="/resources" variant="outline">
            Back to Resources
          </Button>
        }
      />
    </div>
  );
}