# Authentication System Setup

This project now includes a complete authentication system with a secure backend API and user management features.

## Features

- ✅ User registration and login
- ✅ Secure password hashing with bcrypt
- ✅ JWT token authentication
- ✅ User profile management
- ✅ Password change functionality
- ✅ Account information display
- ✅ Secure logout

## Backend API

The backend server provides the following endpoints:

### Authentication
- `POST /api/auth/register` - User registration
- `POST /api/auth/login` - User login
- `POST /api/auth/logout` - User logout

### User Management
- `GET /api/auth/profile` - Get user profile
- `PUT /api/auth/profile` - Update user profile
- `PUT /api/auth/change-password` - Change password

## Setup Instructions

### 1. Install Dependencies

```bash
npm install
```

### 2. Environment Configuration

Create a `.env` file in the root directory with the following variables:

```env
# Server Configuration
PORT=3001
NODE_ENV=development

# JWT Configuration
JWT_SECRET=your-super-secret-jwt-key-change-this-in-production
```

### 3. Start the Development Server

Run both the backend and frontend servers:

```bash
npm run dev:full
```

This will start:
- Backend server on http://localhost:3001
- Frontend development server on http://localhost:5173

### 4. Alternative: Run Servers Separately

Backend only:
```bash
npm run server
```

Frontend only:
```bash
npm run dev
```

## Security Features

- **Password Hashing**: All passwords are hashed using bcrypt with 12 salt rounds
- **JWT Tokens**: Secure authentication tokens with 7-day expiration
- **Input Validation**: Comprehensive validation on both frontend and backend
- **CORS Protection**: Configured for development environment
- **Error Handling**: Proper error responses without exposing sensitive information

## User Interface

### Authentication Pages
- **Login Page**: Email/password authentication with remember me option
- **Register Page**: User registration with password strength validation
- **My Account Page**: Complete profile management interface

### Features
- Real-time password strength indicator
- Form validation with error messages
- Loading states and success/error notifications
- Responsive design for mobile devices
- Secure password visibility toggle

## API Response Format

### Success Response
```json
{
  "message": "Operation successful",
  "user": {
    "id": "user_id",
    "firstName": "John",
    "lastName": "Doe",
    "email": "john@example.com",
    "createdAt": "2024-01-01T00:00:00.000Z",
    "lastLogin": "2024-01-01T12:00:00.000Z"
  },
  "token": "jwt_token_here"
}
```

### Error Response
```json
{
  "error": "Error message here"
}
```

## Development Notes

- The backend uses **SQLite database** for persistent storage. Data is stored in `alientranslate-server/users.db`.
- JWT tokens are stored in localStorage. Consider using httpOnly cookies for enhanced security.
- The frontend automatically handles token management and API requests.
- All API calls include proper error handling and user feedback.
- User tokens are automatically generated and stored securely in the database.

## Production Considerations

1. **Database**: SQLite is included for development. For production, consider PostgreSQL or MySQL for better performance and concurrent access.
2. **JWT Secret**: Use a strong, randomly generated secret in production
3. **HTTPS**: Always use HTTPS in production
4. **Rate Limiting**: Implement rate limiting for authentication endpoints
5. **Logging**: Add proper logging for security events
6. **Monitoring**: Set up monitoring for authentication failures
7. **Backup**: Implement regular database backups for user data 