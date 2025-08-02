<template>
  <div class="account-page" @click.self="closeAccount">
    <div class="account-container">
      <div class="account-card">
        <button @click="closeAccount" class="close-btn">
          <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <line x1="18" y1="6" x2="6" y2="18"></line>
            <line x1="6" y1="6" x2="18" y2="18"></line>
          </svg>
        </button>
        <div class="account-header">
          <img src="/logo.png" alt="логотип переводчужик" class="account-logo" />
          <h1>Мой аккаунт</h1>
          <p>Управление профилем и настройками</p>
        </div>

        <!-- Loading state -->
        <div v-if="loading" class="loading-container">
          <div class="loading-spinner"></div>
          <p>Загрузка...</p>
        </div>

        <!-- Account content -->
        <div v-else class="account-content">
          <!-- Profile Section -->
          <div class="section">
            <h2>Профиль</h2>
            <form @submit.prevent="updateProfile" class="profile-form">
              <div class="form-row">
                <div class="form-group">
                  <label for="firstName">Имя</label>
                  <input
                    type="text"
                    id="firstName"
                    v-model="profileForm.firstName"
                    required
                    placeholder="Введите ваше имя"
                    class="form-input"
                    :class="{ 'error': profileErrors.firstName }"
                  />
                  <span v-if="profileErrors.firstName" class="error-message">{{ profileErrors.firstName }}</span>
                </div>

                <div class="form-group">
                  <label for="lastName">Фамилия</label>
                  <input
                    type="text"
                    id="lastName"
                    v-model="profileForm.lastName"
                    required
                    placeholder="Введите вашу фамилию"
                    class="form-input"
                    :class="{ 'error': profileErrors.lastName }"
                  />
                  <span v-if="profileErrors.lastName" class="error-message">{{ profileErrors.lastName }}</span>
                </div>
              </div>

              <div class="form-group">
                <label for="email">Email</label>
                <input
                  type="email"
                  id="email"
                  v-model="profileForm.email"
                  required
                  placeholder="Введите ваш email"
                  class="form-input"
                  :class="{ 'error': profileErrors.email }"
                />
                <span v-if="profileErrors.email" class="error-message">{{ profileErrors.email }}</span>
              </div>

              <button type="submit" class="btn btn-primary" :disabled="profileLoading">
                <span v-if="profileLoading" class="loading-spinner"></span>
                {{ profileLoading ? 'Сохранение...' : 'Сохранить изменения' }}
              </button>
            </form>
          </div>

          <!-- Account Info Section -->
          <div class="section">
            <h2>Информация об аккаунте</h2>
            <div class="account-info">
              <div class="info-item">
                <span class="info-label">Дата регистрации:</span>
                <span class="info-value">{{ formatDate(user?.createdAt) }}</span>
              </div>
              <div class="info-item">
                <span class="info-label">Последний вход:</span>
                <span class="info-value">{{ formatDate(user?.lastLogin) || 'Недавно' }}</span>
              </div>
              <div class="info-item">
                <span class="info-label">ID аккаунта:</span>
                <span class="info-value">{{ user?.id }}</span>
              </div>
              <div class="info-item">
                <span class="info-label">Статус email:</span>
                <span class="info-value" :class="user?.emailVerified ? 'verified' : 'unverified'">
                  {{ user?.emailVerified ? 'Подтвержден' : 'Не подтвержден' }}
                </span>
              </div>
              <div class="info-item" v-if="user?.emailVerified">
                <span class="info-label">Токен пользователя:</span>
                <span class="info-value">{{ user?.userToken }}</span>
              </div>
              <div class="info-item" v-else>
                <span class="info-label">Токен пользователя:</span>
                <span class="info-value unverified">Доступен после подтверждения email</span>
              </div>
            </div>
          </div>

          <!-- Password Change Section -->
          <div class="section">
            <h2>Изменить пароль</h2>
            <form @submit.prevent="changePassword" class="password-form">
              <div class="form-group">
                <label for="currentPassword">Текущий пароль</label>
                <div class="password-input">
                  <input
                    :type="showCurrentPassword ? 'text' : 'password'"
                    id="currentPassword"
                    v-model="passwordForm.currentPassword"
                    required
                    placeholder="Введите текущий пароль"
                    class="form-input"
                    :class="{ 'error': passwordErrors.currentPassword }"
                  />
                  <button
                    type="button"
                    @click="showCurrentPassword = !showCurrentPassword"
                    class="password-toggle"
                  >
                    <svg v-if="showCurrentPassword" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                      <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/>
                      <circle cx="12" cy="12" r="3"/>
                    </svg>
                    <svg v-else width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                      <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"/>
                      <line x1="1" y1="1" x2="23" y2="23"/>
                    </svg>
                  </button>
                </div>
                <span v-if="passwordErrors.currentPassword" class="error-message">{{ passwordErrors.currentPassword }}</span>
              </div>

              <div class="form-group">
                <label for="newPassword">Новый пароль</label>
                <div class="password-input">
                  <input
                    :type="showNewPassword ? 'text' : 'password'"
                    id="newPassword"
                    v-model="passwordForm.newPassword"
                    required
                    placeholder="Введите новый пароль"
                    class="form-input"
                    :class="{ 'error': passwordErrors.newPassword }"
                  />
                  <button
                    type="button"
                    @click="showNewPassword = !showNewPassword"
                    class="password-toggle"
                  >
                    <svg v-if="showNewPassword" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                      <path d="M1 12s4-8 11-8 11 8 11 8-4 8-11 8-11-8-11-8z"/>
                      <circle cx="12" cy="12" r="3"/>
                    </svg>
                    <svg v-else width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                      <path d="M17.94 17.94A10.07 10.07 0 0 1 12 20c-7 0-11-8-11-8a18.45 18.45 0 0 1 5.06-5.94M9.9 4.24A9.12 9.12 0 0 1 12 4c7 0 11 8 11 8a18.5 18.5 0 0 1-2.16 3.19m-6.72-1.07a3 3 0 1 1-4.24-4.24"/>
                      <line x1="1" y1="1" x2="23" y2="23"/>
                    </svg>
                  </button>
                </div>
                <span v-if="passwordErrors.newPassword" class="error-message">{{ passwordErrors.newPassword }}</span>
              </div>

              <div class="password-strength" v-if="passwordForm.newPassword">
                <div class="strength-bar">
                  <div 
                    class="strength-fill" 
                    :class="passwordStrength.class"
                    :style="{ width: passwordStrength.percentage + '%' }"
                  ></div>
                </div>
                <span class="strength-text" :class="passwordStrength.class">{{ passwordStrength.text }}</span>
              </div>

              <button type="submit" class="btn btn-secondary" :disabled="passwordLoading">
                <span v-if="passwordLoading" class="loading-spinner"></span>
                {{ passwordLoading ? 'Изменение...' : 'Изменить пароль' }}
              </button>
            </form>
          </div>

          <!-- Email Verification Section -->
          <div class="section" v-if="!user?.emailVerified">
            <h2>Подтверждение Email</h2>
            <div class="verification-info">
              <p>Для доступа к токену пользователя необходимо подтвердить ваш email адрес.</p>
              <div class="verification-actions">
                <button @click="resendVerification" class="btn btn-secondary" :disabled="verificationLoading">
                  <span v-if="verificationLoading" class="loading-spinner"></span>
                  {{ verificationLoading ? 'Отправка...' : 'Отправить письмо повторно' }}
                </button>
              </div>
            </div>
          </div>

          <!-- Danger Zone -->
          <div class="section danger-zone">
            <div class="danger-actions">
              <button @click="handleLogout" class="btn btn-danger">
                <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                  <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4"/>
                  <polyline points="16,17 21,12 16,7"/>
                  <line x1="21" y1="12" x2="9" y2="12"/>
                </svg>
                Выйти из аккаунта
              </button>
            </div>
          </div>
        </div>

        <!-- Success/Error messages -->
        <div v-if="message" class="message" :class="messageType">
          {{ message }}
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted, onUnmounted } from 'vue'
import { createUnifiedBackgroundText } from '../utils/backgroundText.js';
import apiService from '../services/api.js'

const emit = defineEmits(['navigate', 'logout'])

const loading = ref(false)
const profileLoading = ref(false)
const passwordLoading = ref(false)
const verificationLoading = ref(false)
const showCurrentPassword = ref(false)
const showNewPassword = ref(false)
const message = ref('')
const messageType = ref('success')
const user = ref(null)

const profileForm = reactive({
  firstName: '',
  lastName: '',
  email: ''
})

const profileErrors = reactive({
  firstName: '',
  lastName: '',
  email: ''
})

const passwordForm = reactive({
  currentPassword: '',
  newPassword: ''
})

const passwordErrors = reactive({
  currentPassword: '',
  newPassword: ''
})

const passwordStrength = computed(() => {
  const password = passwordForm.newPassword
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

const showMessage = (text, type = 'success') => {
  message.value = text
  messageType.value = type
  setTimeout(() => {
    message.value = ''
  }, 5000)
}

const clearErrors = (errorObj) => {
  Object.keys(errorObj).forEach(key => errorObj[key] = '')
}

const validateProfileForm = () => {
  clearErrors(profileErrors)
  let isValid = true
  
  if (!profileForm.firstName.trim()) {
    profileErrors.firstName = 'Имя обязательно'
    isValid = false
  }
  
  if (!profileForm.lastName.trim()) {
    profileErrors.lastName = 'Фамилия обязательна'
    isValid = false
  }
  
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/
  if (!profileForm.email.trim()) {
    profileErrors.email = 'Email обязателен'
    isValid = false
  } else if (!emailRegex.test(profileForm.email)) {
    profileErrors.email = 'Введите корректный email'
    isValid = false
  }
  
  return isValid
}

const validatePasswordForm = () => {
  clearErrors(passwordErrors)
  let isValid = true
  
  if (!passwordForm.currentPassword) {
    passwordErrors.currentPassword = 'Текущий пароль обязателен'
    isValid = false
  }
  
  if (!passwordForm.newPassword) {
    passwordErrors.newPassword = 'Новый пароль обязателен'
    isValid = false
  } else if (passwordForm.newPassword.length < 8) {
    passwordErrors.newPassword = 'Пароль должен содержать минимум 8 символов'
    isValid = false
  }
  
  return isValid
}

const updateProfile = async () => {
  if (!validateProfileForm()) return
  
  profileLoading.value = true
  
  try {
    const response = await apiService.updateProfile({
      firstName: profileForm.firstName,
      lastName: profileForm.lastName,
      email: profileForm.email
    })
    
    user.value = response.user
    apiService.setCurrentUser(response.user)
    showMessage('Профиль успешно обновлен')
    
  } catch (error) {
    showMessage(error.message, 'error')
  } finally {
    profileLoading.value = false
  }
}

const changePassword = async () => {
  if (!validatePasswordForm()) return
  
  passwordLoading.value = true
  
  try {
    await apiService.changePassword({
      currentPassword: passwordForm.currentPassword,
      newPassword: passwordForm.newPassword
    })
    
    // Clear password form
    passwordForm.currentPassword = ''
    passwordForm.newPassword = ''
    
    showMessage('Пароль успешно изменен')
    
  } catch (error) {
    showMessage(error.message, 'error')
  } finally {
    passwordLoading.value = false
  }
}

const handleLogout = async () => {
  try {
    await apiService.logout()
    emit('logout')
    closeAccount()
  } catch (error) {
    console.error('Logout error:', error)
  }
}

const resendVerification = async () => {
  verificationLoading.value = true
  
  try {
    const response = await apiService.resendVerification()
    showMessage('Письмо с подтверждением отправлено на ваш email', 'success')
    
    // In a real app, you would show the verification token
    // For demo purposes, we'll show it in the console
    console.log('Verification token:', response.verificationToken)
    
  } catch (error) {
    showMessage(error.message || 'Ошибка отправки письма', 'error')
  } finally {
    verificationLoading.value = false
  }
}

const closeAccount = () => {
  emit('navigate', 'home')
}

const formatDate = (dateString) => {
  if (!dateString) return 'Неизвестно'
  return new Date(dateString).toLocaleDateString('ru-RU', {
    year: 'numeric',
    month: 'long',
    day: 'numeric',
    hour: '2-digit',
    minute: '2-digit'
  })
}

const loadUserProfile = async () => {
  loading.value = true
  
  try {
    const response = await apiService.getProfile()
    user.value = response.user
    
    // Populate form with current user data
    profileForm.firstName = user.value.firstName
    profileForm.lastName = user.value.lastName
    profileForm.email = user.value.email
    
  } catch (error) {
    showMessage('Ошибка загрузки профиля', 'error')
    console.error('Profile load error:', error)
  } finally {
    loading.value = false
  }
}

onMounted(() => {
  loadUserProfile()
  
  // Unified background text effect
  const accountPage = document.querySelector('.account-page')
  if (!accountPage) return

  const background = createUnifiedBackgroundText(accountPage)

  onUnmounted(() => {
    if (background && typeof background.remove === 'function') {
      background.remove();
    }
  })
})
</script>

<style scoped>
.account-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 2rem 1rem;
  background: transparent;
  position: relative;
  overflow: hidden;
}

.account-container {
  width: 100%;
  max-width: 600px;
}

.account-card {
  position: relative;
  z-index: 2;
  background: #161616;
  border: 1px solid #1e1e1e;
  border-radius: 16px;
  padding: 2.5rem;
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.6), 0 0 120px rgba(56,183,161,0.20), 0 0 240px rgba(56,183,161,0.10);
}

.account-header {
  text-align: center;
  margin-bottom: 2rem;
}

.account-logo {
  height: 42px;
  margin-bottom: 1.5rem;
  margin-left: auto;
  margin-right: auto;
}

.account-header h1 {
  font-size: 1.75rem;
  font-weight: 700;
  margin-bottom: 0.5rem;
  color: var(--text);
}

.account-header p {
  color: var(--text-muted);
  font-size: 0.95rem;
}

.loading-container {
  text-align: center;
  padding: 3rem 0;
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

.section {
  margin-bottom: 2.5rem;
  padding-bottom: 2rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.section:last-child {
  border-bottom: none;
  margin-bottom: 0;
}

.section h2 {
  font-size: 1.25rem;
  font-weight: 600;
  margin-bottom: 1.5rem;
  color: var(--text);
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
  border-color: #ef4444;
}

.form-input::placeholder {
  color: var(--text-muted);
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

.error-message {
  display: block;
  color: #ef4444;
  font-size: 0.85rem;
  margin-top: 0.25rem;
}

.password-strength {
  margin-top: 0.75rem;
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
  transition: width 0.3s ease, background-color 0.3s ease;
}

.strength-fill.weak { background: #ef4444; }
.strength-fill.medium { background: #f59e0b; }
.strength-fill.good { background: #10b981; }
.strength-fill.strong { background: #059669; }

.strength-text {
  font-size: 0.85rem;
  font-weight: 500;
}

.strength-text.weak { color: #ef4444; }
.strength-text.medium { color: #f59e0b; }
.strength-text.good { color: #10b981; }
.strength-text.strong { color: #059669; }

.account-info {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.info-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 0.75rem;
  background: rgba(255, 255, 255, 0.05);
  border-radius: 8px;
}

.info-label {
  font-weight: 500;
  color: var(--text-muted);
}

.info-value {
  color: var(--text);
  font-family: monospace;
  font-size: 0.9rem;
}

.info-value.verified {
  color: #10b981;
}

.info-value.unverified {
  color: #f59e0b;
}

.verification-info {
  background: rgba(255, 255, 255, 0.05);
  border-radius: 8px;
  padding: 1rem;
  margin-bottom: 1rem;
}

.verification-info p {
  color: var(--text-muted);
  margin-bottom: 1rem;
}

.verification-actions {
  display: flex;
  gap: 1rem;
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

.danger-zone {
  border-color: rgba(239, 68, 68, 0.3);
}

.danger-zone h2 {
  color: #ef4444;
}

.danger-actions {
  display: flex;
  gap: 1rem;
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

.btn-danger {
  background: #dc2626;
  color: white;
}

.btn-danger:hover {
  background: #b91c1c;
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

.message {
  padding: 1rem;
  border-radius: 8px;
  margin-bottom: 1rem;
  font-weight: 500;
}

.message.success {
  background: rgba(16, 185, 129, 0.1);
  color: #10b981;
  border: 1px solid rgba(16, 185, 129, 0.3);
}

.message.error {
  background: rgba(239, 68, 68, 0.1);
  color: #ef4444;
  border: 1px solid rgba(239, 68, 68, 0.3);
}

@media (max-width: 768px) {
  .form-row {
    grid-template-columns: 1fr;
  }
  
  .account-card {
    padding: 1.5rem;
  }
  
  .danger-actions {
    flex-direction: column;
  }
}
</style> 