# Translation Server Setup Guide (Developer)

Technical guide for setting up a custom translation server to work with MiniTranslate's "Translation Server" option with context-aware translations and user management.

## Prerequisites

- Server/VPS outside of Russia (or any region where ChatGPT is blocked)
- Node.js 16+ installed on the server
- ChatGPT API key
- Domain name with SSL certificate (for production)

**Note**: Node.js on the server can be installed through package managers (apt, yum, etc.) or Docker.

## Server Deployment

### Step 1: Install Node.js on Server

```bash
# Ubuntu/Debian
curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash -
sudo apt-get install -y nodejs

# CentOS/RHEL
curl -fsSL https://rpm.nodesource.com/setup_18.x | sudo bash -
sudo yum install -y nodejs

# Or using Docker (recommended)
docker pull node:18-alpine
```

### Step 2: Upload Server Files

```bash
# Create server directory
mkdir -p /var/www/minitranslate
cd /var/www/minitranslate

# Upload server files from the 'server' folder
```

### Step 3: Install Dependencies

```bash
npm install
```

### Step 4: Configure Environment

```bash
cp env.example .env
```

Edit `.env` file:
```env
# Server Configuration
PORT=3333

# ChatGPT API Key (server-side)
OPENAI_API_KEY=sk-your-api-key-here

# Default server URL
DEFAULT_SERVER_URL=https://api.sefa.name.tr

# Rate limiting
RATE_LIMIT_WINDOW_MS=900000
RATE_LIMIT_MAX_REQUESTS=100

# CORS settings
CORS_ORIGIN=*
```

### Step 5: Start Server

#### Option A: Docker (Recommended)
```bash
cd server
docker-compose up -d
```

#### Option B: PM2
```bash
# Install PM2 globally
npm install -g pm2

# Start the server
pm2 start server.js --name "minitranslate"

# Save PM2 configuration
pm2 save

# Set up PM2 to start on boot
pm2 startup
```

#### Option C: Manual
```bash
npm start
```

## API Endpoints

### Health Check
```
GET /health
```

### Translation
```
POST /translate
Content-Type: application/json
Authorization: Bearer YOUR_TOKEN

{
  "text": "Hello world",
  "sourceLang": "en",
  "targetLang": "ru",
  "context": "formal tone"
}
```

Response:
```json
{
  "success": true,
  "translation": "Привет мир",
  "sourceLang": "en",
  "targetLang": "ru",
  "originalText": "Hello world",
  "context": "formal tone"
}
```

### Get Supported Languages
```
GET /languages
```

### User Management
```
GET /admin/users
POST /admin/users
DELETE /admin/users/:token
```

## Context-Aware Translations

The Translation Server supports context input for more accurate translations. This feature allows users to provide additional information about their translation requirements.

### Context Features
- **Speaker Context**: Specify male/female speaker, formal/informal tone
- **Domain Context**: Technical, medical, legal, casual, etc.
- **Style Requirements**: Formal, informal, academic, creative
- **Specific Terms**: Preferred terminology or translations for specific words

### Context Examples
```javascript
// Translation with context
{
  "text": "Hello, how are you?",
  "sourceLang": "en",
  "targetLang": "ru",
  "context": "female speaker, formal tone"
}
```

### Context Processing
The server processes context by:
1. Including context in the ChatGPT prompt
2. Providing specific instructions based on context
3. Ensuring consistent terminology usage
4. Maintaining appropriate tone and style

## Language Support

The Translation Server supports **35+ languages** including:

**European Languages**: English, Russian, Spanish, French, German, Italian, Portuguese, Polish, Dutch, Swedish, Danish, Norwegian, Finnish, Czech, Slovak, Slovenian, Estonian, Serbian, Lithuanian, Hungarian, Romanian, Croatian, Greek

**Asian Languages**: Chinese, Japanese, Korean, Arabic, Hindi, Thai, Vietnamese, Indonesian, Malay

**Other Scripts**: Hebrew

## Security Configuration

### HTTPS Setup
```bash
# Install Certbot
sudo apt-get install certbot

# Get SSL certificate
sudo certbot certonly --standalone -d your-domain.com

# Configure nginx with SSL
```

### Firewall Configuration
```bash
# Allow only necessary ports
sudo ufw allow 22
sudo ufw allow 80
sudo ufw allow 443
sudo ufw enable
```

### Rate Limiting
Configure in `.env`:
```env
RATE_LIMIT_WINDOW_MS=900000  # 15 minutes
RATE_LIMIT_MAX_REQUESTS=100  # 100 requests per window
```

### User Authentication
- Token-based authentication
- User management through admin panel
- Usage tracking and monitoring
- Secure token generation and validation

## Monitoring

### PM2 Monitoring
```bash
pm2 monit
pm2 logs minitranslate
pm2 status
```

### Health Check
```bash
curl https://your-domain.com/health
```

### User Management
Access the admin panel at `https://your-domain.com/admin.html` to:
- View all users and their usage
- Generate new tokens
- Monitor server performance
- Track translation requests

## Testing

### Test Translation
```bash
curl -X POST https://your-domain.com/translate \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "text": "Hello world",
    "sourceLang": "en",
    "targetLang": "ru",
    "context": "formal tone"
  }'
```

### Test Languages
```bash
curl https://your-domain.com/languages
```

### Test Context Support
```bash
curl -X POST https://your-domain.com/translate \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "text": "Hello, how are you?",
    "sourceLang": "en",
    "targetLang": "ru",
    "context": "female speaker, casual conversation"
  }'
```

## MiniTranslate Integration

After server deployment, configure MiniTranslate:

1. Set `PreferredTranslator` to `TranslatorType.TranslationServer`
2. Set `TranslationServerUrl` to your server URL
3. Set `TranslationServerToken` to your authentication token

### Context Support in MiniTranslate
- Context field appears in translation interface
- Context is automatically passed to server
- Server processes context for more accurate translations
- Context is included in translation requests

## Troubleshooting

### Server Won't Start
- Check PORT availability
- Verify Node.js version (16+)
- Check `.env` configuration
- Review error logs

### API Key Issues
- Verify API key format (starts with `sk-`)
- Check OpenAI account credits
- Ensure API key has translation permissions

### CORS Errors
- Verify `CORS_ORIGIN` setting
- Check client domain configuration
- Test with different origins

### Rate Limiting
- Monitor request frequency
- Adjust rate limit settings
- Check OpenAI API limits

### Context Not Working
- Verify context is being passed in requests
- Check server logs for context processing
- Ensure context field is visible in MiniTranslate interface

### User Management Issues
- Check admin panel accessibility
- Verify token generation process
- Monitor user usage tracking
- Check server logs for authentication errors 