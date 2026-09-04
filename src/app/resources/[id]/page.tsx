// Ref: workflow.md §4 Architecture Patterns | Feature: Resource Detail
import Link from "next/link";
import { useParams } from "next/navigation";
import { resourceService } from "@/services/resourceService";
import Button from "@/components/common/Button";

export default function ResourceDetailPage() {
  const { id } = useParams<{ id: string }>();

  // TODO: Implement proper data fetching with useQuery
  // For now, placeholder implementation

  return (
    <div className="min-h-[calc(100vh-160px)] py-12">
      <div className="mx-auto max-w-4xl px-4 sm:px-6 lg:px-8">
        <div className="mb-8">
          <Link href="/resources" className="inline-flex items-center text-sm font-medium text-indigo-600 hover:text-indigo-500">
            ← Back to Resources
          </Link>
          <h1 className="mt-4 text-3xl font-bold text-gray-900">
            Resource Title Placeholder
          </h1>
          <div className="mt-4 flex items-center text-sm text-gray-500">
            <span>Submitted by • </span>
            <span>Category • </span>
            <span>Created at</span>
          </div>
        </div>

        <div className="bg-white rounded-lg border border-gray-200 shadow-sm">
          <div className="p-6">
            <p className="text-gray-700 leading-relaxed mb-6">
              Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.
            </p>
            <p className="text-gray-700 leading-relaxed mb-6">
              Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.
            </p>
          </div>

          {false && (
            <div className="p-6 bg-indigo-50 rounded-lg">
              <h3 className="font-semibold text-indigo-800 mb-3">Related Resources</h3>
              <div className="space-y-3">
                {/* Related resources would go here */}
              </div>
            </div>
          )}

          <div className="mt-6 pt-5 border-t border-gray-100">
            <Button
              variant="outline"
              className="mr-3"
              onClick={() => {
                // Approve/resource actions would go here
              }}
            >
              Approve Resource
            </Button>
            <Button variant="secondary">
              Share Resource
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
}