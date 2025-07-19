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

const currentPage = ref('home')

const handleNavigation = (page) => {
  currentPage.value = page
}
</script>

<template>
  <div v-if="currentPage === 'home'">
    <TheHeader @navigate="handleNavigation" />
    <main>
      <HeroSection />
      <FeaturesSection />
      <ServicesSection />
      <HowItWorksSection />
      <CallToActionSection />
    </main>
    <TheFooter />
  </div>
  
  <div v-else-if="currentPage === 'login'">
    <LoginPage @navigate="handleNavigation" />
  </div>
  
  <div v-else-if="currentPage === 'register'">
    <RegisterPage @navigate="handleNavigation" />
  </div>
</template>

<style scoped>
/* We can add component-specific styles here if needed */
</style>