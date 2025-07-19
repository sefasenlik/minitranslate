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
          </li>
        </ul>
      </nav>
    </div>
  </header>
</template>

<script setup>
import { ref } from 'vue'

const emit = defineEmits(['navigate'])

const showProfileMenu = ref(false)

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
})

onUnmounted(() => {
  document.removeEventListener('click', handleClickOutside)
})
</script>

<style scoped>
/* Scoped styles for the header could go here if needed */
</style> 