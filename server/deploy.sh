#!/bin/bash

# Deployment script for MiniTranslate Server with Nginx

set -e

echo "🚀 Starting deployment..."

# Check if running as root (needed for certificate copying)
if [ "$EUID" -ne 0 ]; then
    echo "❌ This script must be run as root (use sudo)"
    exit 1
fi

# Create SSL directory if it doesn't exist
mkdir -p ./nginx_conf/ssl

# Copy SSL certificates
echo "📋 Copying SSL certificates..."
if [ -f "/etc/letsencrypt/live/sefa.name.tr/fullchain.pem" ]; then
    cp /etc/letsencrypt/live/sefa.name.tr/fullchain.pem ./nginx_conf/ssl/cert.pem
    cp /etc/letsencrypt/live/sefa.name.tr/privkey.pem ./nginx_conf/ssl/key.pem
    echo "✅ Certificates copied successfully"
else
    echo "⚠️  Warning: SSL certificates not found at /etc/letsencrypt/live/sefa.name.tr/"
    echo "   Make sure certificates are available or create self-signed ones for testing"
fi

# Set proper permissions for SSL files
chmod 644 ./nginx_conf/ssl/cert.pem 2>/dev/null || true
chmod 600 ./nginx_conf/ssl/key.pem 2>/dev/null || true

# Stop existing containers
echo "🛑 Stopping existing containers..."
docker-compose down

# Build and start containers
echo "🔨 Building and starting containers..."
docker-compose up --build -d

# Wait for services to be ready
echo "⏳ Waiting for services to be ready..."
sleep 10

# Check if services are running
echo "🔍 Checking service status..."
if docker-compose ps | grep -q "Up"; then
    echo "✅ Services are running successfully"
    echo "🌐 Main application: https://sefa.name.tr"
    echo "🔧 Translation API: https://api.sefa.name.tr"
else
    echo "❌ Some services failed to start"
    docker-compose logs
    exit 1
fi

echo "🎉 Deployment completed successfully!" 