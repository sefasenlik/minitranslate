<script setup>
import { ref } from 'vue'
import TheHeader from './components/TheHeader.vue'
import HeroSection from './components/HeroSection.vue'
import FeaturesSection from './components/FeaturesSection.vue'
import ServicesSection from './components/ServicesSection.vue'
import HowItWorksSection from './components/HowItWorksSection.vue'
import CallToActionSection from './components/CallToActionSection.vue'
import TheFooter from './components/TheFooter.vue'
import LoginPage from './components/LoginPage.vue'
import RegisterPage from './components/RegisterPage.vue'
import MyAccountPage from './components/MyAccountPage.vue'
import EmailVerificationPage from './components/EmailVerificationPage.vue'
import DownloadPage from './components/DownloadPage.vue' // Import the new component
import apiService from './services/api.js'

const currentPage = ref('home')
const isAuthenticated = ref(false)
const currentUser = ref(null)

// Check authentication status on app start
const checkAuthStatus = () => {
  isAuthenticated.value = apiService.isAuthenticated()
  currentUser.value = apiService.getCurrentUser()
}

const handleNavigation = (page) => {
  currentPage.value = page
}

// Check for verification URL on app load
const checkVerificationUrl = () => {
  const path = window.location.pathname
  if (path.startsWith('/verify-email/')) {
    const token = path.split('/').pop()
    if (token && token.length > 10) {
      currentPage.value = 'verify-email'
      return
    }
  }
}

// Initialize verification URL check
checkVerificationUrl()

const handleLogout = () => {
  // Clear authentication state
  isAuthenticated.value = false
  currentUser.value = null
  currentPage.value = 'home'
  
  // Force a re-check of authentication status
  checkAuthStatus()
  
  // Force component re-render by updating reactive state
  setTimeout(() => {
    checkAuthStatus()
  }, 100)
}

// Initialize auth status
checkAuthStatus()
</script>

<template>
  <div v-if="currentPage === 'home'">
    <TheHeader @navigate="handleNavigation" @logout="handleLogout" />
    <main>
      <HeroSection @navigate="handleNavigation" />
      <FeaturesSection />
      <ServicesSection />
      <HowItWorksSection />
      <CallToActionSection @navigate="handleNavigation" />
    </main>
    <TheFooter />
  </div>
  
  <div v-else-if="currentPage === 'login'">
    <LoginPage @navigate="handleNavigation" />
  </div>
  
  <div v-else-if="currentPage === 'register'">
    <RegisterPage @navigate="handleNavigation" />
  </div>
  
  <div v-else-if="currentPage === 'account'">
    <MyAccountPage @navigate="handleNavigation" @logout="handleLogout" />
  </div>
  
  <div v-else-if="currentPage === 'verify-email'">
    <EmailVerificationPage @navigate="handleNavigation" />
  </div>
  
  <div v-else-if="currentPage === 'download'">
    <DownloadPage @close="handleNavigation('home')" />
  </div>
</template>

<style scoped>
/* We can add component-specific styles here if needed */
</style>
