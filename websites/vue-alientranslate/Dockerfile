# Multi-stage build for both development and production
FROM --platform=$BUILDPLATFORM node:24-alpine AS base

WORKDIR /app

# Set npm configuration for compatibility
RUN npm config set legacy-peer-deps true

# Copy package files
COPY package*.json ./

# Install all dependencies (including dev dependencies needed for build)
RUN npm ci

# Copy source code
COPY . .

# Development stage
FROM base AS development
# Create data directory with proper permissions
RUN mkdir -p /app/data && \
    chmod 777 /app/data

# Expose development ports
EXPOSE 3001 5173

# Health check for development
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD node -e "require('http').get('http://localhost:3001/api/health', (res) => { process.exit(res.statusCode === 200 ? 0 : 1) })"

# Start development server
CMD ["npm", "run", "dev:full"]

# Build stage
FROM base AS build-stage
# Build the app
RUN npm run build

# Production stage
FROM --platform=$BUILDPLATFORM nginx:alpine AS production-stage

# Copy built app from build stage
COPY --from=build-stage /app/dist /usr/share/nginx/html

# Expose port 80
EXPOSE 80

# Start nginx
CMD ["nginx", "-g", "daemon off;"] 