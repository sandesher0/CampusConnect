import type { NextConfig } from "next";

const nextConfig: NextConfig = {
  output: "standalone",
  async rewrites() {
    // 1. Get the raw variable
    let rawUrl = process.env.NEXT_PUBLIC_API_BASE_URL || "http://localhost:5252";
    
    // 2. Strip out any accidental spaces or quotation marks
    let cleanUrl = rawUrl.trim().replace(/['"]+/g, '');

    // 3. Force https:// if it is somehow still missing
    if (!cleanUrl.startsWith("http")) {
      cleanUrl = `https://${cleanUrl}`;
    }

    // 4. Remove any trailing slashes just in case (prevents double slashes in the destination)
    cleanUrl = cleanUrl.replace(/\/+$/, '');

    return [
      {
        source: "/api/:path*",
        destination: `${cleanUrl}/api/:path*`, 
      },
    ];
  },
};

export default nextConfig;