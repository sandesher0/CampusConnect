import Button from "@/components/common/Button";
import Card from "@/components/common/Card";

export default function NotFound() {
  return (
    <div className="min-h-[calc(100vh-160px)] flex items-center justify-center py-16 px-6 sm:px-8">
      <Card className="p-10 sm:p-14 text-center max-w-md w-full flex flex-col items-center" hoverEffect={false}>
        <div className="h-16 w-16 rounded-[20px] bg-primary-soft-tint flex items-center justify-center text-primary mb-6 shadow-sm">
          <span className="text-2xl font-[800]">404</span>
        </div>

        <h1 className="text-2xl font-[800] text-ink">Page not found</h1>
        <p className="mt-2 text-sm text-muted leading-relaxed">
          The campus page or link you were looking for doesn&apos;t exist or has moved.
        </p>

        <div className="mt-8 flex flex-col sm:flex-row gap-3 w-full sm:w-auto justify-center">
          <Button href="/" variant="primary">
            Back to Home
          </Button>
          <Button href="/events" variant="outline">
            Browse Events
          </Button>
        </div>
      </Card>
    </div>
  );
}
