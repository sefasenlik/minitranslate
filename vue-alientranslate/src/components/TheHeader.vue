<template>
  <header class="navbar">
    <div class="container">
      <a href="#" @click.prevent="goHome"><img src="/logo.png" alt="логотип переводчужик" class="logo" /></a>
      <nav>
        <ul class="nav-links">
          <li><a href="#features">Возможности</a></li>
          <li><a href="#services">Сервисы</a></li>
          <li><a href="#how">Как работает</a></li>
          <li><a href="#download">Скачать</a></li>
          <li>
            <div class="profile-section">
              <!-- Authenticated user -->
              <div v-if="isAuthenticated" class="user-info">
                <span class="user-name">{{ currentUser?.firstName || 'Пользователь' }}</span>
                <button @click="toggleProfileMenu" class="profile-icon" :class="{ 'active': showProfileMenu }">
                  <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/>
                    <circle cx="12" cy="7" r="4"/>
                  </svg>
                </button>
                
                <div v-if="showProfileMenu" class="profile-menu">
                  <div class="profile-menu-header">
                    <h3>{{ currentUser?.firstName }} {{ currentUser?.lastName }}</h3>
                    <p class="user-email">{{ currentUser?.email }}</p>
                  </div>
                  <div class="profile-menu-actions">
                    <button @click="goToAccount" class="profile-menu-item">
                      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/>
                        <circle cx="12" cy="7" r="4"/>
                      </svg>
                      Мой аккаунт
                    </button>
                    <button @click="handleLogout" class="profile-menu-item logout">
                      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M9 21H5a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2h4"/>
                        <polyline points="16,17 21,12 16,7"/>
                        <line x1="21" y1="12" x2="9" y2="12"/>
                      </svg>
                      Выйти
                    </button>
                  </div>
                </div>
              </div>
              
              <!-- Guest user -->
              <div v-else>
                <button @click="toggleProfileMenu" class="profile-icon" :class="{ 'active': showProfileMenu }">
                  <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                    <path d="M20 21v-2a4 4 0 0 0-4-4H8a4 4 0 0 0-4 4v2"/>
                    <circle cx="12" cy="7" r="4"/>
                  </svg>
                </button>
                
                <div v-if="showProfileMenu" class="profile-menu">
                  <div class="profile-menu-header">
                    <h3>Аккаунт</h3>
                  </div>
                  <div class="profile-menu-actions">
                    <button @click="goToLogin" class="profile-menu-item">
                      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M15 3h4a2 2 0 0 1 2 2v14a2 2 0 0 1-2 2h-4"/>
                        <polyline points="10,17 15,12 10,7"/>
                        <line x1="15" y1="12" x2="3" y2="12"/>
                      </svg>
                      Войти
                    </button>
                    <button @click="goToRegister" class="profile-menu-item">
                      <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                        <path d="M16 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"/>
                        <circle cx="8.5" cy="7" r="4"/>
                        <line x1="20" y1="8" x2="20" y2="14"/>
                        <line x1="23" y1="11" x2="17" y2="11"/>
                      </svg>
                      Регистрация
                    </button>
                  </div>
                </div>
              </div>
            </div>
          </li>
        </ul>
      </nav>
    </div>
  </header>
</template>

<script setup>
import { ref } from 'vue'
import apiService from '../services/api.js'

const emit = defineEmits(['navigate', 'logout'])

const showProfileMenu = ref(false)
const isAuthenticated = ref(false)
const currentUser = ref(null)

// Check authentication status
const checkAuthStatus = () => {
  isAuthenticated.value = apiService.isAuthenticated()
  currentUser.value = apiService.getCurrentUser()
}

const goHome = () => {
  emit('navigate', 'home')
  showProfileMenu.value = false
}

const goToLogin = () => {
  emit('navigate', 'login')
  showProfileMenu.value = false
}

const goToRegister = () => {
  emit('navigate', 'register')
  showProfileMenu.value = false
}

const goToAccount = () => {
  emit('navigate', 'account')
  showProfileMenu.value = false
}

const handleLogout = async () => {
  try {
    await apiService.logout()
    // Refresh authentication status
    checkAuthStatus()
    emit('logout')
    showProfileMenu.value = false
  } catch (error) {
    console.error('Logout error:', error)
  }
}

const toggleProfileMenu = () => {
  showProfileMenu.value = !showProfileMenu.value
}

// Close menu when clicking outside
const handleClickOutside = (event) => {
  const profileSection = event.target.closest('.profile-section')
  if (!profileSection) {
    showProfileMenu.value = false
  }
}

// Add event listener when component mounts
import { onMounted, onUnmounted } from 'vue'

onMounted(() => {
  document.addEventListener('click', handleClickOutside)
  checkAuthStatus()
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})
</script>

<style scoped>
.user-info {
  display: flex;
  align-items: center;
  gap: 0.75rem;
}

.user-name {
  color: var(--text);
  font-weight: 500;
  font-size: 0.9rem;
}

.user-email {
  color: var(--text-muted);
  font-size: 0.85rem;
  margin: 0;
}

.profile-menu-item.logout {
  color: #ef4444;
}

.profile-menu-item.logout:hover {
  background: rgba(239, 68, 68, 0.1);
}
</style> 