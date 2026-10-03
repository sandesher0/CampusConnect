# syntax=docker/dockerfile:1

# ==========================================
# STAGE 1: Base (Shared Environment)
# ==========================================
FROM node:22-alpine AS base
ENV NEXT_TELEMETRY_DISABLED=1
WORKDIR /app

# ==========================================
# STAGE 2: Install Dependencies
# ==========================================
FROM base AS dependencies
RUN apk add --no-cache libc6-compat
COPY package.json package-lock.json ./
RUN npm install --legacy-peer-deps

# ==========================================
# STAGE 3: Build
# ==========================================
FROM base AS build
COPY --from=dependencies /app/node_modules ./node_modules
COPY . .

# Allow passing the backend API URL during the build pipeline
ARG NEXT_PUBLIC_API_BASE_URL
ENV NEXT_PUBLIC_API_BASE_URL=${NEXT_PUBLIC_API_BASE_URL}

RUN npm run build

# ==========================================
# STAGE 4: Production Runtime
# ==========================================
FROM node:22-alpine AS runtime
WORKDIR /app

ENV NODE_ENV=production \
    NEXT_TELEMETRY_DISABLED=1 \
    HOSTNAME=0.0.0.0 \
    PORT=3000

# Create a non-root user for security
RUN addgroup --system --gid 1001 nodejs \
    && adduser --system --uid 1001 --ingroup nodejs nextjs

# Copy only the necessary standalone files from the build stage
COPY --from=build --chown=nextjs:nodejs /app/public ./public
COPY --from=build --chown=nextjs:nodejs /app/.next/standalone ./
COPY --from=build --chown=nextjs:nodejs /app/.next/static ./.next/static

EXPOSE 3000

CMD ["node", "server.js"]