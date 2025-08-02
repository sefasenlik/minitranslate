# Server Deployment Guide

This guide will help you deploy the AlienTranslate project to your Linux server.

## Prerequisites

### Server Requirements
- **OS**: Ubuntu 20.04+ or CentOS 8+
- **RAM**: Minimum 2GB (4GB recommended)
- **Storage**: At least 10GB free space
- **Docker**: Docker Engine 20.10+
- **Docker Compose**: Version 2.0+
- **Domain**: Configured DNS pointing to your server

### Domain Setup
Ensure your domains are properly configured:
- `alientranslate.ru` (main Vue.js app)
- `api.sefa.name.tr` (translation API)

## Step 1: Server Preparation

### Install Docker and Docker Compose
```bash
# Update system
sudo apt update && sudo apt upgrade -y

# Install Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh

# Add user to docker group
sudo usermod -aG docker $USER
newgrp docker

# Install Docker Compose
sudo curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
sudo chmod +x /usr/local/bin/docker-compose

# Verify installation
docker --version
docker-compose --version
```

### Create Project Directory and Copy Applications
```bash
# Create web directory
sudo mkdir -p /var/www
sudo chown $USER:$USER /var/www

# Copy applications to /var/www/
sudo cp -r translation-server /var/www/
sudo cp -r vue-alientranslate /var/www/
sudo chown -R $USER:$USER /var/www/translation-server /var/www/vue-alientranslate
```

## Step 2: Environment Configuration

### Create Production Environment Files

**Create `/var/www/translation-server/.env`:**
```bash
# Create translation-server environment file
cat > /var/www/translation-server/.env << 'EOF'
NODE_ENV=production
OPENAI_API_KEY=your-openai-api-key-here
ADMIN_PASSWORD=your-secure-admin-password
EOF
```

**Create `/var/www/vue-alientranslate/.env`:**
```bash
# Create Vue.js environment file
cat > /var/www/vue-alientranslate/.env << 'EOF'
NODE_ENV=production
BASE_URL=https://alientranslate.ru
JWT_SECRET=your-very-secure-jwt-secret-key-change-this
EMAIL_USER=your-email@gmail.com
EMAIL_PASS=your-app-password
EOF
```

### Set Proper Permissions
```bash
# Set secure permissions
chmod 600 /var/www/translation-server/.env /var/www/vue-alientranslate/.env

# Create data directories
mkdir -p /var/www/translation-server/data /var/www/translation-server/logs /var/www/vue-alientranslate/data
chmod 755 /var/www/translation-server/data /var/www/translation-server/logs /var/www/vue-alientranslate/data
```

## Step 3: SSL Certificate Setup

### Option A: Let's Encrypt (Recommended)
```bash
# Install Certbot
sudo apt install certbot -y

# Get SSL certificate for your domains
sudo certbot certonly --standalone -d alientranslate.ru -d www.alientranslate.ru -d api.sefa.name.tr

# Create SSL directory and copy certificates
sudo mkdir -p nginx_conf/ssl
sudo cp /etc/letsencrypt/live/alientranslate.ru/fullchain.pem nginx_conf/ssl/cert.pem
sudo cp /etc/letsencrypt/live/alientranslate.ru/privkey.pem nginx_conf/ssl/key.pem
sudo chown -R $USER:$USER nginx_conf/ssl
```

### Option B: Self-Signed Certificate (Development)
```bash
# Create SSL directory
mkdir -p nginx_conf/ssl

# Generate self-signed certificate
openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout nginx_conf/ssl/key.pem \
  -out nginx_conf/ssl/cert.pem \
  -subj "/C=TR/ST=Istanbul/L=Istanbul/O=AlienTranslate/CN=alientranslate.ru"
```

## Step 4: Build and Deploy

### Build Production Images
```bash
# Build all services
docker-compose -f docker-compose.production.yml build --no-cache

# Verify images were built
docker images | grep alientranslate
```

### Start Production Services
```bash
# Start all services
docker-compose -f docker-compose.production.yml up -d

# Check service status
docker-compose -f docker-compose.production.yml ps

# View logs
docker-compose -f docker-compose.production.yml logs -f
```

## Step 5: Verification

### Check Service Health
```bash
# Check all containers are running
docker ps

# Test health endpoints
curl -k https://alientranslate.ru/api/health
curl -k https://api.sefa.name.tr/health
```

### Test Application
1. **Vue.js App**: https://alientranslate.ru
2. **Translation API**: https://api.sefa.name.tr
3. **Health Checks**: 
   - https://alientranslate.ru/api/health
   - https://api.sefa.name.tr/health

## Step 6: Monitoring and Maintenance

### View Logs
```bash
# View all logs
docker-compose -f docker-compose.production.yml logs -f

# View specific service logs
docker-compose -f docker-compose.production.yml logs -f nginx
docker-compose -f docker-compose.production.yml logs -f alientranslate
docker-compose -f docker-compose.production.yml logs -f vue-backend
```

### Update Application
```bash
# Pull latest changes
git pull origin main

# Copy updated applications to /var/www/
sudo cp -r translation-server /var/www/
sudo cp -r vue-alientranslate /var/www/
sudo chown -R $USER:$USER /var/www/translation-server /var/www/vue-alientranslate

# Rebuild and restart
docker-compose -f docker-compose.production.yml down
docker-compose -f docker-compose.production.yml build --no-cache
docker-compose -f docker-compose.production.yml up -d
```

### Backup Data
```bash
# Create backup script
cat > backup.sh << 'EOF'
#!/bin/bash
BACKUP_DIR="/var/backups/alientranslate"
DATE=$(date +%Y%m%d_%H%M%S)

mkdir -p $BACKUP_DIR

# Backup data directories
tar -czf $BACKUP_DIR/data_$DATE.tar.gz \
  /var/www/translation-server/data/ \
  /var/www/vue-alientranslate/data/ \
  /var/www/translation-server/logs/

# Backup environment files
cp /var/www/translation-server/.env $BACKUP_DIR/translation-server_env_$DATE
cp /var/www/vue-alientranslate/.env $BACKUP_DIR/vue_env_$DATE

echo "Backup completed: $BACKUP_DIR/data_$DATE.tar.gz"
EOF

chmod +x backup.sh
```

## Step 7: SSL Certificate Renewal (Let's Encrypt)

### Create Renewal Script
```bash
cat > renew-ssl.sh << 'EOF'
#!/bin/bash

# Renew certificates
sudo certbot renew

# Copy renewed certificates
sudo cp /etc/letsencrypt/live/alientranslate.ru/fullchain.pem nginx_conf/ssl/cert.pem
sudo cp /etc/letsencrypt/live/alientranslate.ru/privkey.pem nginx_conf/ssl/key.pem
sudo chown -R $USER:$USER nginx_conf/ssl

# Reload nginx
docker-compose -f docker-compose.production.yml restart nginx

echo "SSL certificates renewed and nginx reloaded"
EOF

chmod +x renew-ssl.sh
```

### Add to Crontab
```bash
# Add SSL renewal to crontab (runs twice daily)
(crontab -l 2>/dev/null; echo "0 0,12 * * * /var/www/renew-ssl.sh") | crontab -
```

## Troubleshooting

### Common Issues

1. **Port 80/443 already in use**
   ```bash
   # Check what's using the ports
   sudo netstat -tulpn | grep :80
   sudo netstat -tulpn | grep :443
   
   # Stop conflicting services
   sudo systemctl stop apache2 nginx
   ```

2. **SSL Certificate Issues**
   ```bash
   # Check certificate validity
   openssl x509 -in nginx_conf/ssl/cert.pem -text -noout
   
   # Test SSL connection
   openssl s_client -connect alientranslate.ru:443 -servername alientranslate.ru
   ```

3. **Container Build Failures**
   ```bash
   # Clean Docker cache
   docker system prune -a
   
   # Rebuild without cache
   docker-compose -f docker-compose.production.yml build --no-cache
   ```

4. **Database Issues**
   ```bash
   # Check database files
   ls -la /var/www/translation-server/data/
   ls -la /var/www/vue-alientranslate/data/
   
   # Access container to debug
   docker exec -it vue-backend-prod sh
   ```

### Performance Monitoring
```bash
# Monitor resource usage
docker stats

# Check disk usage
df -h

# Monitor logs in real-time
docker-compose -f docker-compose.production.yml logs -f --tail=100
```

## Security Considerations

1. **Firewall Configuration**
   ```bash
   # Allow only necessary ports
   sudo ufw allow 22/tcp    # SSH
   sudo ufw allow 80/tcp    # HTTP
   sudo ufw allow 443/tcp   # HTTPS
   sudo ufw enable
   ```

2. **Regular Updates**
   ```bash
   # Update system packages
   sudo apt update && sudo apt upgrade -y
   
   # Update Docker images
   docker-compose -f docker-compose.production.yml pull
   ```

3. **Backup Strategy**
   - Daily automated backups
   - Test restore procedures
   - Store backups off-site

## Next Steps

1. **Set up monitoring** (Prometheus, Grafana)
2. **Configure logging** (ELK Stack)
3. **Set up CI/CD pipeline**
4. **Implement rate limiting**
5. **Add CDN for static assets**

Your AlienTranslate application should now be successfully deployed and accessible at your configured domains! 