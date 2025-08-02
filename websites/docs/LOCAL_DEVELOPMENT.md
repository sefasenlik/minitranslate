# 🚀 Local Development Guide

This guide will help you run the AlienTranslate application locally for development.

## 📋 Prerequisites

- Docker Desktop installed and running
- Git (optional)
- A code editor (VS Code recommended)

## 🛠 Quick Start

### 1. Setup Environment Files

**Create `translation-server/.env`:**
```env
NODE_ENV=development
OPENAI_API_KEY=your-openai-api-key-here
ADMIN_PASSWORD=admin123
```

**Create `vue-alientranslate/.env`:**
```env
NODE_ENV=development
BASE_URL=http://localhost:5173
JWT_SECRET=dev-secret-key-change-in-production
EMAIL_USER=your-email@gmail.com
EMAIL_PASS=your-app-password
```

### 2. Start Development Environment

```bash
# Make script executable
chmod +x dev.sh

# Start services
./dev.sh start
```

### 3. Access Applications

- **Vue.js App**: http://localhost:5173
- **Vue.js API**: http://localhost:3001
- **Translation API**: http://localhost:3333

## 🛠 Development Commands

### View Logs
```bash
./dev.sh logs
```

### Check Status
```bash
./dev.sh status
```

### Restart Services
```bash
./dev.sh restart
```

### Stop Services
```bash
./dev.sh stop
```

### Rebuild Images
```bash
./dev.sh build
```

## 📁 Project Structure

```
websites/
├── translation-server/          # Translation API server
├── vue-alientranslate/         # Vue.js frontend application
├── docker-compose.dev.yml      # Development Docker Compose file
├── dev.sh                      # Development management script
└── LOCAL_DEVELOPMENT.md        # This guide
```

## 🔧 Development Features

- **Hot Reload**: Changes to your code will automatically reload
- **Volume Mounts**: Local files are mounted into containers
- **Development Mode**: All services run in development mode
- **Easy Management**: Simple commands to manage services
- **Health Checks**: Automatic service health monitoring

## 🚨 Troubleshooting

### Services Not Starting
```bash
# Check logs
./dev.sh logs

# Check status
./dev.sh status

# Rebuild images
./dev.sh build
```

### Port Conflicts
If you see errors about ports being in use:
1. Check what's using the ports:
   ```bash
   sudo netstat -tulpn | grep :5173
   sudo netstat -tulpn | grep :3001
   sudo netstat -tulpn | grep :3333
   ```
2. Stop conflicting services or change ports in `docker-compose.dev.yml`

### Database Issues
```bash
# Check data directories
ls -la translation-server/data/
ls -la vue-alientranslate/data/

# Access containers
docker exec -it vue-dev sh
docker exec -it translation-server-dev sh
```

## 📝 Development Tips

### 1. Vue.js Development
- Changes to Vue files will automatically trigger hot reload
- Check Vue DevTools at http://localhost:5173/__devtools__/
- API calls are proxied through Vite to the backend

### 2. Translation Server Development
- Changes to server files will restart nodemon
- Check logs for errors: `./dev.sh logs translation-server`
- Test API at http://localhost:3333/health

### 3. Database Development
- SQLite database is persisted in `vue-alientranslate/data/`
- User data is persisted in `translation-server/data/`
- Logs are available in `translation-server/logs/`

## 🔐 Security Notes

1. **Environment Files**:
   - Don't commit `.env` files
   - Use development-only values
   - Keep API keys secure

2. **Development Mode**:
   - Debug logging is enabled
   - Error details are exposed
   - Security features might be relaxed

## 📊 Monitoring

### View Logs
```bash
# All services
./dev.sh logs

# Specific service
docker-compose -f docker-compose.dev.yml logs -f vue-dev
docker-compose -f docker-compose.dev.yml logs -f translation-server
```

### Check Health
```bash
# All services
./dev.sh status

# Individual endpoints
curl http://localhost:3001/api/health
curl http://localhost:3333/health
```

## 🔄 Common Workflows

### 1. Making Changes
1. Edit files in your IDE
2. Changes are automatically detected
3. Services reload as needed
4. Check logs for errors

### 2. Adding Dependencies
1. Add to package.json
2. Run `./dev.sh build`
3. Run `./dev.sh restart`

### 3. Database Changes
1. Stop services: `./dev.sh stop`
2. Backup data if needed
3. Make changes
4. Start services: `./dev.sh start`

## 🎯 Next Steps

1. Update environment files with your values
2. Start the development environment
3. Make changes to the code
4. Test your changes
5. Monitor logs for issues

Your local development environment is ready! 🚀