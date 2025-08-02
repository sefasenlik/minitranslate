# Multi-platform server Dockerfile
FROM --platform=$BUILDPLATFORM node:24-alpine AS base

# Set working directory
WORKDIR /app

# Copy package files
COPY package*.json ./

# Install dependencies (include dev dependencies for development)
RUN npm ci

# Copy source code
COPY . .

# Create non-root user (works on both Linux and Windows)
RUN addgroup -g 1001 -S nodejs 2>/dev/null || true && \
    adduser -S nodejs -u 1001 2>/dev/null || true

# Create data directory with proper permissions
RUN mkdir -p /app/data && \
    chown -R nodejs:nodejs /app/data && \
    chmod 755 /app/data

# Development stage
FROM base AS development
# Switch to non-root user
USER nodejs

# Expose port
EXPOSE 3333

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD node -e "require('http').get('http://localhost:3333/health', (res) => { process.exit(res.statusCode === 200 ? 0 : 1) })"

# Start the application in development mode
CMD ["npm", "run", "dev"]

# Production stage
FROM base AS production
# Switch to non-root user
USER nodejs

# Expose port
EXPOSE 3333

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD node -e "require('http').get('http://localhost:3333/health', (res) => { process.exit(res.statusCode === 200 ? 0 : 1) })"

# Start the application
CMD ["npm", "start"] 