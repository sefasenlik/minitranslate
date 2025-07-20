<template>
  <div class="verification-page" @click.self="closeVerification">
    <div class="verification-container">
      <div class="verification-card">
        <button @click="closeVerification" class="close-btn">
          <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <line x1="18" y1="6" x2="6" y2="18"></line>
            <line x1="6" y1="6" x2="18" y2="18"></line>
          </svg>
        </button>
        <div class="verification-header">
          <img src="/logo.png" alt="логотип переводчужик" class="verification-logo" />
          <h1>Подтверждение Email</h1>
          <p>Завершите регистрацию вашего аккаунта</p>
        </div>

        <!-- Loading state -->
        <div v-if="loading" class="loading-container">
          <div class="loading-spinner"></div>
          <p>Проверка токена...</p>
        </div>

        <!-- Success state -->
        <div v-else-if="verified" class="success-container">
          <div class="success-icon">
            <svg width="60" height="60" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/>
              <polyline points="22,4 12,14.01 9,11.01"/>
            </svg>
          </div>
          <h2>Email подтвержден!</h2>
          <p>Ваш email адрес успешно подтвержден. Теперь вы можете получить доступ к вашему токену пользователя в разделе "Мой аккаунт".</p>
          <button @click="goToAccount" class="btn btn-primary">
            Перейти в аккаунт
          </button>
        </div>

        <!-- Error state -->
        <div v-else-if="error" class="error-container">
          <div class="error-icon">
            <svg width="60" height="60" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
              <circle cx="12" cy="12" r="10"/>
              <line x1="15" y1="9" x2="9" y2="15"/>
              <line x1="9" y1="9" x2="15" y2="15"/>
            </svg>
          </div>
          <h2>Ошибка подтверждения</h2>
          <p>{{ errorMessage }}</p>
          <button @click="goToHome" class="btn btn-secondary">
            Вернуться на главную
          </button>
        </div>

        <!-- Verification form -->
        <div v-else class="verification-form">
          <p>Для подтверждения email введите токен, который был отправлен на ваш email адрес:</p>
          
          <div class="form-group">
            <label for="verificationToken">Токен подтверждения</label>
            <input
              type="text"
              id="verificationToken"
              v-model="token"
              placeholder="Введите токен подтверждения"
              class="form-input"
              :class="{ 'error': tokenError }"
            />
            <span v-if="tokenError" class="error-message">{{ tokenError }}</span>
          </div>

          <button @click="verifyEmail" class="btn btn-primary btn-full" :disabled="verifying">
            <span v-if="verifying" class="loading-spinner"></span>
            {{ verifying ? 'Подтверждение...' : 'Подтвердить Email' }}
          </button>
        </div>

        <div class="verification-footer">
          <button @click="closeVerification" class="btn btn-outline">
            Закрыть
          </button>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from 'vue'
import { createBackgroundText } from '../utils/backgroundText.js'
import apiService from '../services/api.js'

const emit = defineEmits(['navigate'])

const loading = ref(true)
const verifying = ref(false)
const verified = ref(false)
const error = ref(false)
const errorMessage = ref('')
const token = ref('')
const tokenError = ref('')

const verifyEmail = async () => {
  if (!token.value.trim()) {
    tokenError.value = 'Введите токен подтверждения'
    return
  }

  verifying.value = true
  tokenError.value = ''
  loading.value = true

  try {
    console.log('Calling API to verify token:', token.value)
    const response = await apiService.verifyEmail(token.value)
    console.log('Verification response:', response)
    
    // After successful email verification, register user with translation server
    if (response.verified) {
      try {
        // Get user data from the verification response or fetch from backend
        let userData = null
        
        // First try to get from localStorage (if user is logged in)
        const currentUser = apiService.getCurrentUser()
        if (currentUser) {
          userData = currentUser
        } else {
          // If not in localStorage, try to get from the verification response
          // The backend should return user data in the verification response
          if (response.user) {
            userData = response.user
          }
        }
        
        if (userData) {
          console.log('Registering user with translation server:', {
            name: userData.firstName,
            surname: userData.lastName,
            email: userData.email,
            token: userData.userToken
          })
          
          await apiService.registerWithTranslationServer({
            name: userData.firstName,
            surname: userData.lastName,
            email: userData.email,
            token: userData.userToken
          })
          
          console.log('Successfully registered with translation server')
        } else {
          console.warn('No user data available for translation server registration')
        }
      } catch (translationServerError) {
        console.error('Failed to register with translation server:', translationServerError)
        // Don't fail the verification process if translation server registration fails
        // Just log the error and continue
      }
    }
    
    verified.value = true
    loading.value = false
  } catch (err) {
    console.error('Verification error:', err)
    error.value = true
    errorMessage.value = err.message || 'Неверный или истекший токен подтверждения'
    loading.value = false
  } finally {
    verifying.value = false
  }
}

const goToAccount = () => {
  emit('navigate', 'account')
}

const goToHome = () => {
  emit('navigate', 'home')
}

const closeVerification = () => {
  emit('navigate', 'home')
}

onMounted(() => {
  // Check if token is provided in URL path
  const pathParts = window.location.pathname.split('/')
  const urlToken = pathParts[pathParts.length - 1]
  
  console.log('Verification page loaded, path:', window.location.pathname)
  console.log('Extracted token:', urlToken)
  
  if (urlToken && urlToken.length > 10) { // Basic validation for token length
    token.value = urlToken
    console.log('Starting verification for token:', urlToken)
    verifyEmail()
  } else {
    console.log('No valid token found, showing form')
    loading.value = false
  }

  // Background text effect
  const verificationPage = document.querySelector('.verification-page')
  if (!verificationPage) return

  const backgroundTextElement = createBackgroundText(verificationPage, 60)

  onUnmounted(() => {
    backgroundTextElement.remove()
  })
})
</script>

<style scoped>
.verification-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 2rem 1rem;
  background: transparent;
}

.verification-container {
  width: 100%;
  max-width: 500px;
}

.verification-card {
  position: relative;
  z-index: 2;
  background: #161616;
  border: 1px solid #1e1e1e;
  border-radius: 16px;
  padding: 2.5rem;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.6), 0 0 120px rgba(56,183,161,0.20), 0 0 240px rgba(56,183,161,0.10);
}

.verification-header {
  text-align: center;
  margin-bottom: 2rem;
}

.verification-logo {
  height: 42px;
  margin-bottom: 1.5rem;
}

.verification-header h1 {
  font-size: 1.75rem;
  font-weight: 700;
  margin-bottom: 0.5rem;
  color: var(--text);
}

.verification-header p {
  color: var(--text-muted);
  font-size: 0.95rem;
}

.loading-container,
.success-container,
.error-container {
  text-align: center;
  padding: 2rem 0;
}

.loading-spinner {
  display: inline-block;
  width: 20px;
  height: 20px;
  border: 2px solid rgba(56,183,161,0.3);
  border-radius: 50%;
  border-top-color: var(--accent);
  animation: spin 1s ease-in-out infinite;
}

@keyframes spin {
  to { transform: rotate(360deg); }
}

.success-icon {
  color: #10b981;
  margin-bottom: 1rem;
}

.error-icon {
  color: #ef4444;
  margin-bottom: 1rem;
}

.success-container h2,
.error-container h2 {
  font-size: 1.5rem;
  font-weight: 600;
  margin-bottom: 1rem;
  color: var(--text);
}

.success-container p,
.error-container p {
  color: var(--text-muted);
  margin-bottom: 2rem;
  line-height: 1.6;
}

.verification-form {
  margin-bottom: 2rem;
}

.verification-form p {
  color: var(--text-muted);
  margin-bottom: 1.5rem;
  text-align: center;
}

.form-group {
  margin-bottom: 1.5rem;
}

.form-group label {
  display: block;
  margin-bottom: 0.5rem;
  font-weight: 500;
  color: var(--text);
  font-size: 0.9rem;
}

.form-input {
  width: 100%;
  padding: 0.75rem 1rem;
  border: 1px solid rgba(255, 255, 255, 0.2);
  border-radius: 8px;
  background: rgba(255, 255, 255, 0.05);
  color: var(--text);
  font-size: 1rem;
  transition: all 0.2s ease;
}

.form-input:focus {
  outline: none;
  border-color: var(--accent);
  background: rgba(255, 255, 255, 0.08);
}

.form-input.error {
  border-color: #ef4444;
}

.form-input::placeholder {
  color: var(--text-muted);
}

.error-message {
  display: block;
  color: #ef4444;
  font-size: 0.85rem;
  margin-top: 0.25rem;
}

.btn {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  padding: 0.75rem 1.5rem;
  border: none;
  border-radius: 8px;
  font-size: 0.95rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  text-decoration: none;
}

.btn:disabled {
  opacity: 0.6;
  cursor: not-allowed;
}

.btn-primary {
  background: var(--accent);
  color: #000;
}

.btn-primary:hover:not(:disabled) {
  background: #38b7a1;
  transform: translateY(-1px);
}

.btn-secondary {
  background: rgba(255, 255, 255, 0.1);
  color: var(--text);
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.btn-secondary:hover:not(:disabled) {
  background: rgba(255, 255, 255, 0.15);
  border-color: rgba(255, 255, 255, 0.3);
}

.btn-outline {
  background: transparent;
  color: var(--text);
  border: 1px solid rgba(255, 255, 255, 0.2);
}

.btn-outline:hover {
  background: rgba(255, 255, 255, 0.05);
  border-color: rgba(255, 255, 255, 0.3);
}

.btn-full {
  width: 100%;
  justify-content: center;
}

.verification-footer {
  text-align: center;
  margin-top: 2rem;
}

.close-btn {
  position: absolute;
  top: -12px;
  right: -12px;
  width: 32px;
  height: 32px;
  border-radius: 50%;
  background: #161616;
  border: 2px solid #1e1e1e;
  color: var(--text-muted);
  cursor: pointer;
  display: flex;
  align-items: center;
  justify-content: center;
  transition: all 0.2s ease;
  z-index: 10;
}

.close-btn:hover {
  background: #1e1e1e;
  color: var(--text);
  transform: scale(1.1);
}
</style> 