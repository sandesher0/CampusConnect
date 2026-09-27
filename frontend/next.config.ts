import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  // Emit a self-contained server bundle so the production Docker image excludes build tooling.
  output: "standalone",
  async rewrites() {
    // Check if the env variable exists to determine the base URL
    const baseUrl = process.env.NEXT_PUBLIC_API_BASE_URL 
      ? process.env.NEXT_PUBLIC_API_BASE_URL 
      : "http://localhost:5252";

    return [
      {
        source: "/api/:path*",
        // This ensures the /api/:path* is always appended to whatever the base URL is
        destination: `${baseUrl}/api/:path*`, 
      },
    ];
  },
};

export default nextConfig;