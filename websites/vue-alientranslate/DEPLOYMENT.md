# Vue AlienTranslate Deployment

This Vue.js application is deployed as part of the main server infrastructure using Docker.

## Architecture

- **Domain**: `alientranslate.ru`
- **Vue App**: Served on the root domain (`alientranslate.ru/`)
- **Translation Interfaces**: Still accessible on specific paths:
  - `alientranslate.ru/translator.html`
  - `alientranslate.ru/translation_server.html`

## Docker Setup

The Vue app is containerized and runs alongside the Node.js translation server:

- **Container Name**: `vue-alientranslate`
- **Port**: 80 (internal)
- **Network**: `shared-network` (same as other services)

## Build Process

1. **Multi-stage Dockerfile**: 
   - Stage 1: Build the Vue app with Node.js
   - Stage 2: Serve with nginx

2. **Nginx Configuration**:
   - Handles client-side routing with `try_files`
   - Caches static assets
   - Adds security headers

## Deployment

The app is deployed through the main `docker-compose.yml` in the `/server` directory:

```bash
cd server
docker-compose up -d
```

This will:
1. Build the Vue app from `../vue-alientranslate`
2. Start the Vue container
3. Configure nginx to route requests appropriately

## Development

For local development:

```bash
npm run dev
```

For production build:

```bash
npm run build
```

## Routing

- All routes except `/translator.html` and `/translation_server.html` are handled by the Vue app
- The Vue app uses client-side routing, so all paths fall back to `index.html`
- Translation interfaces are proxied to the Node.js server 