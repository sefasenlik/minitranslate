#!/bin/bash

# SSL Certificate Setup Script for Multiple Domains
# This script uses Let's Encrypt to generate free SSL certificates

echo "🔐 Setting up SSL certificates for multiple domains..."
echo "📋 Domains to be covered:"
echo "   - sefa.name.tr"
echo "   - api.sefa.name.tr"
echo "   - alientranslate.ru"
echo "   - www.alientranslate.ru"

# Check if certbot is installed
if ! command -v certbot &> /dev/null; then
    echo "❌ Certbot is not installed. Installing..."
    
    # Install certbot based on the system
    if command -v apt-get &> /dev/null; then
        # Ubuntu/Debian
        sudo apt-get update
        sudo apt-get install -y certbot
    elif command -v yum &> /dev/null; then
        # CentOS/RHEL
        sudo yum install -y certbot
    elif command -v dnf &> /dev/null; then
        # Fedora
        sudo dnf install -y certbot
    else
        echo "❌ Could not install certbot automatically. Please install it manually:"
        echo "   Visit: https://certbot.eff.org/"
        exit 1
    fi
fi

# Create directory for SSL certificates (Docker nginx location)
sudo mkdir -p /var/www/minitranslate/nginx_conf/ssl

# Generate certificate for all domains
echo "📜 Generating SSL certificate for all domains..."
sudo certbot certonly --standalone --expand \
    -d sefa.name.tr \
    -d api.sefa.name.tr \
    -d alientranslate.ru \
    -d www.alientranslate.ru \
    --email senliksefa@gmail.com \
    --agree-tos \
    --non-interactive

# Check if certificate was generated successfully
if [ -f "/etc/letsencrypt/live/sefa.name.tr/fullchain.pem" ]; then
    echo "✅ SSL certificate generated successfully!"
    
    # Copy certificates to nginx directory
    sudo cp /etc/letsencrypt/live/sefa.name.tr/fullchain.pem /var/www/minitranslate/nginx_conf/ssl/cert.pem
    sudo cp /etc/letsencrypt/live/sefa.name.tr/privkey.pem /var/www/minitranslate/nginx_conf/ssl/key.pem
    
    # Set proper permissions
    sudo chmod 644 /var/www/minitranslate/nginx_conf/ssl/cert.pem
    sudo chmod 600 /var/www/minitranslate/nginx_conf/ssl/key.pem
    
    echo "✅ Certificates copied to Docker nginx location"
    echo "🔄 Restarting Docker containers..."
    cd /var/www/minitranslate
    docker-compose down
    docker-compose up -d
    
    echo "🎉 SSL setup complete!"
    echo "📋 Your certificates are valid for 90 days"
    echo "🔄 To auto-renew, add this to crontab:"
    echo "   0 12 * * * /usr/bin/certbot renew --quiet && cd /var/www/minitranslate && docker-compose restart nginx"
else
    echo "❌ Failed to generate SSL certificate"
    echo "💡 Make sure:"
    echo "   1. All domains point to this server"
    echo "   2. Port 80 and 443 are open"
    echo "   3. You have proper DNS records for:"
    echo "      - sefa.name.tr"
    echo "      - api.sefa.name.tr"
    echo "      - alientranslate.ru"
    echo "      - www.alientranslate.ru"
fi 