<template>
  <div class="auth-page" @click.self="closeAuth">
    <div class="auth-container">
      <div class="auth-card">
        <button @click="closeAuth" class="close-btn">
          <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <line x1="18" y1="6" x2="6" y2="18"></line>
            <line x1="6" y1="6" x2="18" y2="18"></line>
          </svg>
        </button>
        <div class="auth-header">
          <img src="/logo.png" alt="логотип переводчужик" class="auth-logo" />
          <h1>Регистрация</h1>
          <p>Создайте аккаунт для доступа к сервисам</p>
        </div>

        <form @submit.prevent="handleRegister" class="auth-form">
          <div class="form-row">
            <div class="form-group">
              <label for="firstName">Имя</label>
              <input
                type="text"
                id="firstName"
                v-model="form.firstName"
                required
                placeholder="Введите ваше имя"
                class="form-input"
                :class="{ 'error': errors.firstName }"
              />
              <span v-if="errors.firstName" class="error-message">{{ errors.firstName }}</span>
            </div>

            <div class="form-group">
              <label for="lastName">Фамилия</label>
              <input
                type="text"
                id="lastName"
                v-model="form.lastName"
                required
                placeholder="Введите вашу фамилию"
                class="form-input"
                :class="{ 'error': errors.lastName }"
              />
              <span v-if="errors.lastName" class="error-message">{{ errors.lastName }}</span>
            </div>
          </div>

          <div class="form-group">
            <label for="email">Email</label>
            <input
              type="email"
              id="email"
              v-model="form.email"
              required
              placeholder="Введите ваш email"
              class="form-input"
              :class="{ 'error': errors.email }"
            />
            <span v-if="errors.email" class="error-message">{{ errors.email }}</span>
          </div>

          <div class="form-row">
            <div class="form-group">
              <label for="password">Пароль</label>
              <div class="password-input">
                <input
                  :type="showPassword ? 'text' : 'password'"
                  id="password"
                  v-model="form.password"
                  required
                  placeholder="Создайте пароль"
                  class="form-input"
                  :class="{ 'error': errors.password }"
                />
                <button
                  type="button"
                  @click="showPassword = !showPassword"
                  class="password-toggle"
                >
                  <svg v-if="showPassword" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/>
                    <circle cx="12" cy="12" r="3"/>
                  </svg>
                  <svg v-else width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"/>
                    <line x1="1" y1="1" x2="23" y2="23"/>
                  </svg>
                </button>
              </div>
              <span v-if="errors.password" class="error-message">{{ errors.password }}</span>
            </div>

            <div class="form-group">
              <label for="confirmPassword">Подтвердите пароль</label>
              <div class="password-input">
                <input
                  :type="showConfirmPassword ? 'text' : 'password'"
                  id="confirmPassword"
                  v-model="form.confirmPassword"
                  required
                  placeholder="Повторите пароль"
                  class="form-input"
                  :class="{ 'error': errors.confirmPassword }"
                />
                <button
                  type="button"
                  @click="showConfirmPassword = !showConfirmPassword"
                  class="password-toggle"
                >
                  <svg v-if="showConfirmPassword" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/>
                    <circle cx="12" cy="12" r="3"/>
                  </svg>
                  <svg v-else width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"/>
                    <line x1="1" y1="1" x2="23" y2="23"/>
                  </svg>
                </button>
              </div>
              <span v-if="errors.confirmPassword" class="error-message">{{ errors.confirmPassword }}</span>
            </div>
          </div>

          <div class="password-strength" v-if="form.password">
            <div class="strength-bar">
              <div 
                class="strength-fill" 
                :class="passwordStrength.class"
                :style="{ width: passwordStrength.percentage + '%' }"
              ></div>
            </div>
            <span class="strength-text" :class="passwordStrength.class">{{ passwordStrength.text }}</span>
          </div>

          <div class="form-group">
            <label class="checkbox-label">
              <input type="checkbox" name="agreeToTerms" v-model="form.agreeToTerms" />
              <span class="checkmark"></span>
              Я согласен с <a href="#" class="terms-link">условиями использования</a> и <a href="#" class="terms-link">политикой конфиденциальности</a>
            </label>
            <span v-if="errors.agreeToTerms" class="error-message">{{ errors.agreeToTerms }}</span>
          </div>

          <button type="submit" class="btn btn-primary btn-full" :disabled="loading">
            <span v-if="loading" class="loading-spinner"></span>
            {{ loading ? 'Регистрация...' : 'Зарегистрироваться' }}
          </button>
        </form>

        <div class="auth-divider">
          <span>или</span>
        </div>

        <div class="social-login">
          <button @click="showNotAvailablePopup" class="btn btn-secondary btn-full social-btn">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="currentColor">
              <path d="M22.56 12.25c0-.78-.07-1.53-.2-2.25H12v4.26h5.92c-.26 1.37-1.04 2.53-2.21 3.31v2.77h3.57c2.08-1.92 3.28-4.74 3.28-8.09z"/>
              <path d="M12 23c2.97 0 5.46-.98 7.28-2.66l-3.57-2.77c-.98.66-2.23 1.06-3.71 1.06-2.86 0-5.29-1.93-6.16-4.53H2.18v2.84C3.99 20.53 7.7 23 12 23z"/>
              <path d="M5.84 14.09c-.22-.66-.35-1.36-.35-2.09s.13-1.43.35-2.09V7.07H2.18C1.43 8.55 1 10.22 1 12s.43 3.45 1.18 4.93l2.85-2.22.81-.62z"/>
              <path d="M12 5.38c1.62 0 3.06.56 4.21 1.64l3.15-3.15C17.45 2.09 14.97 1 12 1 7.7 1 3.99 3.47 2.18 7.07l3.66 2.84c.87-2.6 3.3-4.53 6.16-4.53z"/>
            </svg>
            Зарегистрироваться через Google
          </button>
        </div>

        <!-- Not Available Popup -->
        <NotAvailablePopup 
          :show="showNotAvailable" 
          @close="hideNotAvailablePopup"
          message="Регистрация через Google пока недоступна. Мы работаем над интеграцией с Google OAuth."
        />

        <div class="auth-footer">
          <p>Уже есть аккаунт? <a href="#" @click.prevent="goToLogin">Войти</a></p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, onUnmounted } from 'vue'
import { createUnifiedBackgroundText } from '../utils/backgroundText.js';
import apiService from '../services/api.js'
import NotAvailablePopup from './NotAvailablePopup.vue'

const emit = defineEmits(['navigate'])

const loading = ref(false)
const showPassword = ref(false)
const showConfirmPassword = ref(false)
const showNotAvailable = ref(false)

const form = reactive({
  firstName: '',
  lastName: '',
  email: '',
  password: '',
  confirmPassword: '',
  agreeToTerms: false
})

const errors = reactive({
  firstName: '',
  lastName: '',
  email: '',
  password: '',
  confirmPassword: '',
  agreeToTerms: ''
})

const passwordStrength = computed(() => {
  const password = form.password
  if (!password) return { text: '', percentage: 0, class: '' }
  
  let score = 0
  let feedback = []
  
  if (password.length >= 8) score += 25
  else feedback.push('Минимум 8 символов')
  
  if (/[a-z]/.test(password)) score += 25
  else feedback.push('Добавьте строчные буквы')
  
  if (/[A-Z]/.test(password)) score += 25
  else feedback.push('Добавьте заглавные буквы')
  
  if (/[0-9]/.test(password)) score += 25
  else feedback.push('Добавьте цифры')
  
  if (score <= 25) return { text: 'Слабый', percentage: score, class: 'weak' }
  if (score <= 50) return { text: 'Средний', percentage: score, class: 'medium' }
  if (score <= 75) return { text: 'Хороший', percentage: score, class: 'good' }
  return { text: 'Отличный', percentage: score, class: 'strong' }
})

const validateForm = () => {
  // Reset errors
  Object.keys(errors).forEach(key => errors[key] = '')
  
  let isValid = true
  
  // First name validation
  if (!form.firstName.trim()) {
    errors.firstName = 'Имя обязательно'
    isValid = false
  } else if (form.firstName.length < 2) {
    errors.firstName = 'Имя должно содержать минимум 2 символа'
    isValid = false
  }
  
  // Last name validation
  if (!form.lastName.trim()) {
    errors.lastName = 'Фамилия обязательна'
    isValid = false
  } else if (form.lastName.length < 2) {
    errors.lastName = 'Фамилия должна содержать минимум 2 символа'
    isValid = false
  }
  
  // Email validation
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  if (!form.email.trim()) {
    errors.email = 'Email обязателен'
    isValid = false
  } else if (!emailRegex.test(form.email)) {
    errors.email = 'Введите корректный email'
    isValid = false
  }
  
  // Password validation
  if (!form.password) {
    errors.password = 'Пароль обязателен'
    isValid = false
  } else if (form.password.length < 8) {
    errors.password = 'Пароль должен содержать минимум 8 символов'
    isValid = false
  }
  
  // Confirm password validation
  if (!form.confirmPassword) {
    errors.confirmPassword = 'Подтвердите пароль'
    isValid = false
  } else if (form.password !== form.confirmPassword) {
    errors.confirmPassword = 'Пароли не совпадают'
    isValid = false
  }
  
  // Terms agreement validation
  if (!form.agreeToTerms) {
    errors.agreeToTerms = 'Необходимо согласиться с условиями'
    isValid = false
  }
  
  return isValid
}

const handleRegister = async () => {
  if (!validateForm()) return
  
  loading.value = true
  
  try {
    const response = await apiService.register({
      firstName: form.firstName,
      lastName: form.lastName,
      email: form.email,
      password: form.password
    })
    
    // Store user data
    apiService.setCurrentUser(response.user)
    
    // Show success message about email verification
    alert('Регистрация успешна! Пожалуйста, проверьте ваш email для подтверждения аккаунта.')
    
    // Navigate to home page
    emit('navigate', 'home')
    
  } catch (error) {
    console.error('Registration error:', error)
    alert(error.message || 'Ошибка регистрации')
  } finally {
    loading.value = false
  }
}

const goToLogin = () => {
  emit('navigate', 'login')
}

const closeAuth = () => {
  emit('navigate', 'home')
}

const showNotAvailablePopup = () => {
  showNotAvailable.value = true
}

const hideNotAvailablePopup = () => {
  showNotAvailable.value = false
}

// ===== Unified background text generator =====
onMounted(() => {
  const authPage = document.querySelector('.auth-page');
  if (!authPage) return;

  const background = createUnifiedBackgroundText(authPage);

  onUnmounted(() => {
    if (background && typeof background.remove === 'function') {
      background.remove();
    }
  });
});
// ===== end background text generator =====
</script>

<style scoped>
.auth-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 2rem 1rem;
  background: transparent; /* Background handled by global blur overlay */
}

.auth-container {
  width: 100%;
  max-width: 600px;
}

.auth-card {
  position: relative;
  z-index: 2;
  background: #161616;
  border: 1px solid #1e1e1e;
  border-radius: 16px;
  padding: 2.5rem;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.6), 0 0 120px rgba(56,183,161,0.20), 0 0 240px rgba(56,183,161,0.10);
}

.auth-header {
  text-align: center;
  margin-bottom: 2rem;
}

.auth-logo {
  height: 42px;
  margin-bottom: 1.5rem;
  display: block;
  margin-left: auto;
  margin-right: auto;
}

.auth-header h1 {
  font-size: 1.75rem;
  font-weight: 700;
  margin-bottom: 0.5rem;
  color: var(--text);
}

.auth-header p {
  color: var(--text-muted);
  font-size: 0.95rem;
}

.auth-form {
  margin-bottom: 1.5rem;
}

.form-row {
  display: grid;
  grid-template-columns: 1fr 1fr;
  gap: 1rem;
}

.form-group {
  margin-bottom: 1.25rem;
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
  border-color: #ff6b6b;
}

.form-input::placeholder {
  color: var(--text-muted);
}

.error-message {
  display: block;
  color: #ff6b6b;
  font-size: 0.8rem;
  margin-top: 0.25rem;
}

.password-input {
  position: relative;
}

.password-toggle {
  position: absolute;
  right: 0.75rem;
  top: 50%;
  transform: translateY(-50%);
  background: none;
  border: none;
  color: var(--text-muted);
  cursor: pointer;
  padding: 0.25rem;
  border-radius: 4px;
  transition: color 0.2s ease;
}

.password-toggle:hover {
  color: var(--text);
}

.password-strength {
  margin-bottom: 1.25rem;
}

.strength-bar {
  width: 100%;
  height: 4px;
  background: rgba(255, 255, 255, 0.1);
  border-radius: 2px;
  overflow: hidden;
  margin-bottom: 0.5rem;
}

.strength-fill {
  height: 100%;
  transition: all 0.3s ease;
}

.strength-fill.weak {
  background: #ff6b6b;
}

.strength-fill.medium {
  background: #ffa726;
}

.strength-fill.good {
  background: #66bb6a;
}

.strength-fill.strong {
  background: #4caf50;
}

.strength-text {
  font-size: 0.8rem;
  color: var(--text-muted);
}

.strength-text.weak {
  color: #ff6b6b;
}

.strength-text.medium {
  color: #ffa726;
}

.strength-text.good {
  color: #66bb6a;
}

.strength-text.strong {
  color: #4caf50;
}

.checkbox-label {
  display: flex;
  align-items: flex-start;
  cursor: pointer;
  color: var(--text-muted);
  font-size: 0.9rem;
  line-height: 1.4;
}

.checkbox-label input[type="checkbox"] {
  display: none;
}

.checkmark {
  width: 18px;
  height: 18px;
  border: 1px solid rgba(255, 255, 255, 0.3);
  border-radius: 4px;
  margin-right: 0.5rem;
  margin-top: 0.1rem;
  position: relative;
  transition: all 0.2s ease;
  flex-shrink: 0;
}

.checkbox-label input[type="checkbox"]:checked + .checkmark {
  background: var(--accent);
  border-color: var(--accent);
}

.checkbox-label input[type="checkbox"]:checked + .checkmark::after {
  content: '';
  position: absolute;
  left: 5px;
  top: 2px;
  width: 6px;
  height: 10px;
  border: solid white;
  border-width: 0 2px 2px 0;
  transform: rotate(45deg);
}

.terms-link {
  color: var(--accent);
  text-decoration: none;
  transition: color 0.2s ease;
}

.terms-link:hover {
  color: #2da58c;
}

.btn-full {
  width: 100%;
  justify-content: center;
}

.loading-spinner {
  display: inline-block;
  width: 16px;
  height: 16px;
  border: 2px solid transparent;
  border-top: 2px solid currentColor;
  border-radius: 50%;
  animation: spin 1s linear infinite;
  margin-right: 0.5rem;
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

.auth-divider {
  text-align: center;
  margin: 1.5rem 0;
  position: relative;
}

.auth-divider::before {
  content: '';
  position: absolute;
  top: 50%;
  left: 0;
  right: 0;
  height: 1px;
  background: rgba(255, 255, 255, 0.2);
}

.auth-divider span {
  background: #161616;
  padding: 0 1rem;
  color: var(--text-muted);
  font-size: 0.9rem;
  position: relative;
  z-index: 1;
}

.social-login {
  margin-bottom: 1.5rem;
}

.social-btn {
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.5rem;
  background: rgba(255, 255, 255, 0.1);
  border: 1px solid rgba(255, 255, 255, 0.2);
  color: var(--text);
  transition: all 0.2s ease;
}

.social-btn:hover {
  background: rgba(255, 255, 255, 0.15);
  border-color: rgba(255, 255, 255, 0.3);
}

.auth-footer {
  text-align: center;
  color: var(--text-muted);
  font-size: 0.9rem;
}

.auth-footer a {
  color: var(--accent);
  text-decoration: none;
  font-weight: 500;
  transition: color 0.2s ease;
}

.auth-footer a:hover {
  color: #2da58c;
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

@media (max-width: 480px) {
  .auth-card {
    padding: 2rem 1.5rem;
  }
  
  .form-row {
    grid-template-columns: 1fr;
    gap: 0;
  }
}
</style> 