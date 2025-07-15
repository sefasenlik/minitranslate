# ChatGPT Translation API Server

A Node.js/Express server that provides ChatGPT translation services via REST API with context-aware translations and user management. This server can be deployed to bypass regional restrictions and provide translation services to your MiniTranslate application.

**🔒 Privacy First: This server does NOT log or store any translation text. All translations are processed in memory and never written to disk or logs.**

## Features

- **No logging of translations** – translation text is never stored or logged
- **Context-aware translations** – support for translation requirements and context
- **RESTful API** for ChatGPT translations
- **CORS enabled** for cross-origin requests
- **Input validation** and error handling
- **Health check endpoint** for monitoring
- **Language support** for 35+ languages
- **Rate limiting** (optional)
- **Environment configuration**
- **User management** with token-based authentication
- **Admin panel** for user management and monitoring
- **Usage tracking** for monitoring and billing

## Quick Start

### Prerequisites

- Node.js 16+ 
- npm or yarn
- ChatGPT API key

### Installation

1. **Clone or download** the server files to your server
2. **Install dependencies:**
   ```bash
   cd server
   npm install
   ```

3. **Configure environment:**
   ```bash
   cp env.example .env
   # Edit .env with your settings
   ```

4. **Start the server:**
   ```bash
   npm start
   ```

### Development Mode

```bash
npm run dev
```

## API Endpoints

### Health Check
```
GET /health
```
Returns server status and uptime information.

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

**Response:**
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
Returns list of all supported languages.

### User Management
```
GET /admin/users
POST /admin/users
DELETE /admin/users/:token
```

## Context-Aware Translations

The server supports context input for more accurate translations. This feature allows users to provide additional information about their translation requirements.

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

The server supports **35+ languages** including:

**European Languages**: English, Russian, Spanish, French, German, Italian, Portuguese, Polish, Dutch, Swedish, Danish, Norwegian, Finnish, Czech, Slovak, Slovenian, Estonian, Latvian, Lithuanian, Hungarian, Romanian, Croatian, Greek

**Asian Languages**: Chinese, Japanese, Korean, Arabic, Hindi, Thai, Vietnamese, Indonesian, Malay

**Other Scripts**: Hebrew

## Deployment Options

### 1. VPS/Cloud Server (Recommended)

**Requirements:**
- Ubuntu/CentOS server
- Node.js 16+
- PM2 (for process management)

**Deployment steps:**
```bash
# Install Node.js
curl -fsSL https://deb.nodesource.com/setup_18.x | sudo -E bash -
sudo apt-get install -y nodejs

# Install PM2
npm install -g pm2

# Clone/upload your server files
cd /var/www/minitranslate
npm install

# Start with PM2
pm2 start server.js --name "minitranslate"
pm2 startup
pm2 save
```

### 2. Railway/Heroku

**Railway:**
1. Connect your GitHub repository
2. Set environment variables
3. Deploy automatically

**Heroku:**
```bash
heroku create your-minitranslate
git push heroku main
heroku config:set PORT=3000
```

### 3. Docker

Create a `Dockerfile`:
```dockerfile
FROM node:18-alpine
WORKDIR /app
COPY package*.json ./
RUN npm install
COPY . .
EXPOSE 3000
CMD ["npm", "start"]
```

Build and run:
```bash
docker build -t minitranslate .
docker run -p 3000:3000 minitranslate
```

## Configuration

### Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `PORT` | Server port | 3000 |
| `OPENAI_API_KEY` | Default API key (optional) | - |
| `RATE_LIMIT_WINDOW_MS` | Rate limit window | 900000 (15min) |
| `RATE_LIMIT_MAX_REQUESTS` | Max requests per window | 100 |
| `CORS_ORIGIN` | CORS origin | * |

### Security Considerations

1. **Use HTTPS** in production
2. **Set up rate limiting** for public APIs
3. **Validate API keys** on your server
4. **Monitor usage** and costs
5. **Set up logging** for debugging (never logs translation text)
6. **Implement user authentication** with tokens
7. **Use admin panel** for user management

## User Management

### Admin Panel
Access the admin panel at `https://your-domain.com/admin.html` to:
- View all users and their usage
- Generate new tokens
- Monitor server performance
- Track translation requests

### Token Authentication
- Secure token-based authentication
- User usage tracking and monitoring
- Token generation and validation
- Admin panel for user management

## Integration with MiniTranslate

After deploying your server, update your MiniTranslate application to use the API instead of the local HTML file.

The server URL will be: `https://your-server.com:3000/translate`

### Context Support in MiniTranslate
- Context field appears in translation interface
- Context is automatically passed to server
- Server processes context for more accurate translations
- Context is included in translation requests

## Monitoring

### Health Check
```bash
curl https://your-server.com:3000/health
```

### Logs (with PM2)
```bash
pm2 logs minitranslate
pm2 monit
```

### User Management
```bash
# View all users
curl https://your-server.com/admin/users

# Generate new token
curl -X POST https://your-server.com/admin/users
```

## Testing

### Test Translation
```bash
curl -X POST https://your-server.com/translate \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "text": "Hello world",
    "sourceLang": "en",
    "targetLang": "ru",
    "context": "formal tone"
  }'
```

### Test Context Support
```bash
curl -X POST https://your-server.com/translate \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_TOKEN" \
  -d '{
    "text": "Hello, how are you?",
    "sourceLang": "en",
    "targetLang": "ru",
    "context": "female speaker, casual conversation"
  }'
```

### Test Languages
```bash
curl https://your-server.com/languages
```

## Troubleshooting

### Common Issues

1. **CORS errors**: Check CORS_ORIGIN setting
2. **API key errors**: Verify ChatGPT API key format
3. **Rate limiting**: Check OpenAI API limits
4. **Server not starting**: Check PORT availability
5. **Context not working**: Verify context is being passed correctly
6. **User management issues**: Check admin panel accessibility

### Debug Mode

Enable debug logging:
```bash
DEBUG=* npm start
```

### Context Not Working
- Verify context is being passed in requests
- Check server logs for context processing
- Ensure context field is visible in MiniTranslate interface

### User Management Issues
- Check admin panel accessibility
- Verify token generation process
- Monitor user usage tracking
- Check server logs for authentication errors

## License

MIT License - feel free to modify and deploy as needed. 