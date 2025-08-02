# SSL Certificate Setup Guide for alientranslate.ru

## Overview
This guide will help you generate SSL certificates for your domains:
- `sefa.name.tr`
- `api.sefa.name.tr`
- `alientranslate.ru`
- `www.alientranslate.ru`

## Prerequisites
1. **Domain DNS Setup**: Make sure your domains point to your server's IP address
2. **Server Access**: You need root/sudo access to your server
3. **Open Ports**: Ports 80 and 443 must be open on your server

## Option 1: Let's Encrypt (Recommended for Production)

### Step 1: Install Certbot
```bash
# Ubuntu/Debian
sudo apt-get update
sudo apt-get install -y certbot

# CentOS/RHEL
sudo yum install -y certbot

# Or use snap
sudo snap install --classic certbot
```

### Step 2: Generate Certificates
```bash
# Run the SSL setup script
chmod +x ssl-setup.sh
./ssl-setup.sh
```

**Important**: Replace `your-email@example.com` in the script with your actual email.

**Note**: The script will generate a single certificate covering all four domains.

### Step 3: Verify Certificates
```bash
# Check certificate status
sudo certbot certificates

# Test renewal
sudo certbot renew --dry-run
```

## Option 2: Self-Signed Certificate (For Testing)

### Step 1: Generate Self-Signed Certificate
```bash
# Run the self-signed certificate script
chmod +x generate-self-signed.sh
./generate-self-signed.sh
```

### Step 2: Use in Development
The certificates will be created in `nginx_conf/ssl/` and can be used immediately.

## Option 3: Docker-based Setup

### Step 1: Generate Certificates with Docker
```bash
# Run the Docker SSL setup script
chmod +x docker-ssl-setup.sh
./docker-ssl-setup.sh
```

### Step 2: Restart Docker Containers
```bash
docker-compose down
docker-compose up -d
```

## Configuration Files

### Nginx Configuration
The nginx configuration is already set up to use SSL certificates from:
- `/etc/nginx/ssl/cert.pem` (certificate)
- `/etc/nginx/ssl/key.pem` (private key)

### Docker Compose
The Docker setup mounts the SSL certificates from the host to the nginx container.

## Available URLs After Setup

Once SSL is configured, these URLs will be available:

### sefa.name.tr
- `https://sefa.name.tr/` (main application)

### api.sefa.name.tr
- `https://api.sefa.name.tr/translation-server.html`
- `https://api.sefa.name.tr/translator.html`

### alientranslate.ru
- `https://alientranslate.ru/translation-server.html`
- `https://alientranslate.ru/translator.html`
- `https://www.alientranslate.ru/translation-server.html`
- `https://www.alientranslate.ru/translator.html`

## Troubleshooting

### Common Issues

1. **Certificate Not Found**
   ```bash
   # Check if certificates exist
   ls -la nginx_conf/ssl/
   ```

2. **Nginx Permission Errors**
   ```bash
   # Fix permissions
   sudo chmod 644 nginx_conf/ssl/cert.pem
   sudo chmod 600 nginx_conf/ssl/key.pem
   ```

3. **Domain Not Pointing to Server**
   ```bash
   # Check DNS resolution
   nslookup sefa.name.tr
   nslookup api.sefa.name.tr
   nslookup alientranslate.ru
   nslookup www.alientranslate.ru
   ```

4. **Port 80/443 Blocked**
   ```bash
   # Check if ports are open
   sudo netstat -tlnp | grep :80
   sudo netstat -tlnp | grep :443
   ```

### Renewal Setup

For Let's Encrypt certificates (valid for 90 days):

```bash
# Add to crontab for auto-renewal
sudo crontab -e

# Add this line:
0 12 * * * /usr/bin/certbot renew --quiet
```

## Security Best Practices

1. **Keep Certificates Updated**: Set up auto-renewal for Let's Encrypt
2. **Use Strong Ciphers**: The nginx config already includes secure cipher suites
3. **Regular Backups**: Backup your SSL certificates
4. **Monitor Expiry**: Set up monitoring for certificate expiration

## Testing

After setup, test your SSL configuration:

```bash
# Test SSL configuration
curl -I https://sefa.name.tr/
curl -I https://api.sefa.name.tr/translation-server.html
curl -I https://alientranslate.ru/translation-server.html
```

## Support

If you encounter issues:
1. Check the nginx error logs: `sudo tail -f /var/log/nginx/error.log`
2. Verify DNS settings with your domain registrar
3. Ensure firewall allows ports 80 and 443 