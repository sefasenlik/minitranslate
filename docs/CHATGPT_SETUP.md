# ChatGPT API Key Setup Guide (Developer)

Technical guide for obtaining and configuring ChatGPT API keys for MiniTranslate's "ChatGPT Translator" option with context-aware translations.

## Prerequisites

- OpenAI account with billing enabled
- MiniTranslate application installed
- Internet connection (ChatGPT API is blocked in Russia)

## Getting ChatGPT API Key

### Step 1: Create OpenAI Account
1. Go to [OpenAI Platform](https://platform.openai.com/signup)
2. Sign up with email or Google/Microsoft account
3. Verify your email address

### Step 2: Add Payment Method
1. Go to [Billing](https://platform.openai.com/account/billing)
2. Add credit card or payment method
3. Verify payment method

### Step 3: Generate API Key
1. Go to [API Keys](https://platform.openai.com/api-keys)
2. Click "Create new secret key"
3. Enter key name (e.g., "MiniTranslate")
4. Copy the generated key (starts with `sk-`)
5. Store securely - you won't see it again

## API Key Configuration

### In MiniTranslate Application
1. Right-click tray icon → Settings
2. Select "ChatGPT Translator" as preferred translator
3. Enter API key in "ChatGPT API Key" field
4. Click Save

### In Code (for developers)
```csharp
// AppSettings.cs
public string ChatGptApiKey { get; set; } = "sk-your-api-key-here";
```

## Context-Aware Translations

MiniTranslate's ChatGPT Translator supports context input for more accurate translations. This feature allows users to provide additional information about their translation requirements.

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

## API Usage and Costs

### Pricing
- **GPT-4o-mini**: $0.00015 per 1K input tokens, $0.0006 per 1K output tokens
- **GPT-3.5-turbo**: $0.0015 per 1K input tokens, $0.002 per 1K output tokens
- **GPT-4**: $0.03 per 1K input tokens, $0.06 per 1K output tokens
- **Typical translation**: ~$0.001-0.005 per translation

### Rate Limits
- **Free tier**: 3 requests per minute
- **Paid tier**: 3500 requests per minute
- **Token limits**: 4096 tokens per request (GPT-3.5-turbo), 128K tokens (GPT-4o-mini)

## Language Support

MiniTranslate's ChatGPT Translator supports **35+ languages** including:

**European Languages**: English, Russian, Spanish, French, German, Italian, Portuguese, Polish, Dutch, Swedish, Danish, Norwegian, Finnish, Czech, Slovak, Slovenian, Estonian, Latvian, Lithuanian, Hungarian, Romanian, Croatian, Greek

**Asian Languages**: Chinese, Japanese, Korean, Arabic, Hindi, Thai, Vietnamese, Indonesian, Malay

**Other Scripts**: Hebrew

## Testing API Key

### Using OpenAI Platform
1. Go to [Playground](https://platform.openai.com/playground)
2. Enter your API key
3. Test with simple translation prompt

### Using MiniTranslate
1. Configure API key in Settings
2. Use "Test Translation" button
3. Check for successful response

## Security Best Practices

### API Key Protection
- Never commit API keys to version control
- Use environment variables in production
- Rotate keys regularly
- Monitor usage for unauthorized access

### Usage Monitoring
- Check [Usage](https://platform.openai.com/usage) dashboard
- Set up billing alerts
- Monitor request patterns

## Troubleshooting

### Invalid API Key
- Verify key starts with `sk-`
- Check key hasn't been revoked
- Ensure account has billing enabled

### Rate Limit Exceeded
- Wait for rate limit window to reset
- Upgrade to paid plan for higher limits
- Implement request throttling

### Regional Blocking
- ChatGPT API is blocked in Russia
- Use VPN or Translation Server option
- Consider alternative translation services

### Billing Issues
- Check payment method is valid
- Verify account has sufficient credits
- Contact OpenAI support if needed

### Context Not Working
- Ensure you're using ChatGPT Translator
- Check that context field is visible in translation window
- Verify context is being passed correctly in API requests

## API Endpoints Used

### Translation Request
```
POST https://api.openai.com/v1/chat/completions
Authorization: Bearer sk-your-api-key
Content-Type: application/json

{
  "model": "gpt-4o-mini",
  "messages": [
    {
      "role": "user",
      "content": "Translate from English to Russian:\nHello world\nRequirements: formal tone"
    }
  ],
  "max_tokens": 3000,
  "temperature": 0.3
}
```

### Context-Aware Translation
```javascript
// Example with context
const prompt = `Translate from ${sourceLanguageName} to ${targetLanguageName}:\n${text}`;
if (context) {
    prompt += `\nRequirements: ${context}`;
}
prompt += `\nOnly output the translation.`;
```

## Alternative Options

If ChatGPT API is not suitable:
- **Translation Server**: Contact senliksefa@gmail.com for server token
- **Google Translate**: Free, no API key required
- **Yandex Translate**: Free, excellent for Russian translations 