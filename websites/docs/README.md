# 🌐 AlienTranslate Server Deployment

This folder contains all the files needed to deploy the AlienTranslate application on your Linux server.

## 📁 Folder Structure

```
websites/
├── translation-server/          # Translation API server (copied to /var/www/)
├── vue-alientranslate/         # Vue.js frontend application (copied to /var/www/)
├── nginx_conf/                 # Nginx configuration and SSL certificates
├── docker-compose.production.yml  # Production Docker Compose file
├── docker-compose.dev.yml      # Development Docker Compose file
├── deploy.sh                   # Deployment management script
├── setup.sh                    # Server environment setup script
├── dev.sh                      # Development management script
├── SERVER_DEPLOYMENT_GUIDE.md  # Detailed deployment guide
├── LOCAL_DEVELOPMENT.md        # Local development guide
├── backup.sh                   # Backup script (created by setup)
├── renew-ssl.sh               # SSL renewal script (created by setup)
└── README.md                  # This file
```

## 🚀 Quick Deployment

### 1. Transfer Files to Server
Copy the entire `websites/` folder to your server:
```bash
# On your server
# Copy the websites folder contents to your preferred location
```

### 2. Copy Applications to /var/www/
```bash
# Manually copy applications to /var/www/
sudo mkdir -p /var/www
sudo cp -r translation-server /var/www/
sudo cp -r vue-alientranslate /var/www/
sudo chown -R $USER:$USER /var/www/translation-server /var/www/vue-alientranslate
```

### 3. Run Setup
```bash
chmod +x setup.sh
./setup.sh
```

### 4. Update Environment Files
Edit the environment files with your actual API keys:
```bash
nano /var/www/translation-server/.env
nano /var/www/vue-alientranslate/.env
```

### 5. Deploy
```bash
chmod +x deploy.sh
./deploy.sh build
./deploy.sh start
```

## 🛠 Management Commands

### Check Status
```bash
./deploy.sh status
```

### View Logs
```bash
./deploy.sh logs
```

### Restart Services
```bash
./deploy.sh restart
```

### Update Application
```bash
./deploy.sh update
```

### Create Backup
```bash
./deploy.sh backup
```

### Stop Services
```bash
./deploy.sh stop
```

## 🔐 Environment Files

### /var/www/translation-server/.env
```env
NODE_ENV=production
OPENAI_API_KEY=your-openai-api-key-here
ADMIN_PASSWORD=your-secure-admin-password
```

### /var/www/vue-alientranslate/.env
```env
NODE_ENV=production
BASE_URL=https://alientranslate.ru
JWT_SECRET=your-very-secure-jwt-secret-key-change-this
EMAIL_USER=your-email@gmail.com
EMAIL_PASS=your-app-password
```

## 🌍 Production URLs

After successful deployment:
- **Main App**: https://alientranslate.ru
- **Translation API**: https://api.sefa.name.tr
- **Health Checks**:
  - https://alientranslate.ru/api/health
  - https://api.sefa.name.tr/health

## 🔧 SSL Certificates

### Option A: Let's Encrypt (Recommended)
```bash
# Install certbot
sudo apt install certbot

# Get certificates
sudo certbot certonly --standalone -d alientranslate.ru -d www.alientranslate.ru -d api.sefa.name.tr

# Copy certificates
sudo cp /etc/letsencrypt/live/alientranslate.ru/fullchain.pem nginx_conf/ssl/cert.pem
sudo cp /etc/letsencrypt/live/alientranslate.ru/privkey.pem nginx_conf/ssl/key.pem
sudo chown -R $USER:$USER nginx_conf/ssl
```

### Option B: Self-Signed (Development)
The setup script will generate self-signed certificates automatically.

## 📊 Monitoring

### Check Service Health
```bash
# All services
./deploy.sh status

# Individual containers
docker ps

# Resource usage
docker stats
```

### View Logs
```bash
# All logs
./deploy.sh logs

# Specific service
docker-compose -f docker-compose.production.yml logs -f nginx
docker-compose -f docker-compose.production.yml logs -f alientranslate
docker-compose -f docker-compose.production.yml logs -f vue-backend
```

## 🔄 Updates

### Manual Update
```bash
# Stop services
./deploy.sh stop

# Copy new files to /var/www/
sudo cp -r translation-server /var/www/
sudo cp -r vue-alientranslate /var/www/
sudo chown -R $USER:$USER /var/www/translation-server /var/www/vue-alientranslate

# Rebuild and start
./deploy.sh build
./deploy.sh start
```

### Automated Update
```bash
./deploy.sh update
```

## 💾 Backups

### Create Backup
```bash
./deploy.sh backup
```

### Restore from Backup
```bash
# Extract backup
tar -xzf /var/backups/alientranslate/data_YYYYMMDD_HHMMSS.tar.gz

# Copy environment files
cp /var/backups/alientranslate/translation-server_env_YYYYMMDD_HHMMSS /var/www/translation-server/.env
cp /var/backups/alientranslate/vue_env_YYYYMMDD_HHMMSS /var/www/vue-alientranslate/.env

# Restart services
./deploy.sh restart
```

## 🚨 Troubleshooting

### Services Not Starting
```bash
# Check logs
./deploy.sh logs

# Check container status
docker-compose -f docker-compose.production.yml ps

# Check Docker
docker info
```

### SSL Issues
```bash
# Check certificate
openssl x509 -in nginx_conf/ssl/cert.pem -text -noout

# Test connection
openssl s_client -connect alientranslate.ru:443 -servername alientranslate.ru
```

### Port Conflicts
```bash
# Check what's using ports 80/443
sudo netstat -tulpn | grep :80
sudo netstat -tulpn | grep :443

# Stop conflicting services
sudo systemctl stop apache2 nginx
```

### Database Issues
```bash
# Check data files
ls -la /var/www/translation-server/data/
ls -la /var/www/vue-alientranslate/data/

# Access container
docker exec -it vue-backend-prod sh
```

## 🔒 Security

### Firewall Configuration
```bash
# Allow necessary ports
sudo ufw allow 22/tcp    # SSH
sudo ufw allow 80/tcp    # HTTP
sudo ufw allow 443/tcp   # HTTPS
sudo ufw enable
```

### Regular Maintenance
```bash
# Update system
sudo apt update && sudo apt upgrade -y

# Update Docker images
docker-compose -f docker-compose.production.yml pull

# Renew SSL certificates (if using Let's Encrypt)
./renew-ssl.sh
```

## 📈 Performance

### For High Traffic
1. Add CDN for static assets
2. Implement Redis caching
3. Use load balancer
4. Monitor with Prometheus/Grafana

### For Better Security
1. Rate limiting on API endpoints
2. WAF (Web Application Firewall)
3. Regular security updates
4. Intrusion detection

## 📞 Support

If you encounter issues:
1. Check logs: `./deploy.sh logs`
2. Verify environment files are correctly configured
3. Ensure domains are properly pointing to your server
4. Check Docker and Docker Compose are installed and running

For detailed deployment instructions, see: `SERVER_DEPLOYMENT_GUIDE.md`

Your AlienTranslate application is now ready for production deployment! 🚀 