<template>
  <div v-if="show" class="popup-overlay" @click="closePopup">
    <div class="popup-content" @click.stop>
      <div class="popup-header">
        <h3>Функция недоступна</h3>
        <button @click="closePopup" class="popup-close-btn">
          <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <line x1="18" y1="6" x2="6" y2="18"></line>
            <line x1="6" y1="6" x2="18" y2="18"></line>
          </svg>
        </button>
      </div>
      <div class="popup-body">
        <div class="popup-icon">
          <svg width="48" height="48" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <circle cx="12" cy="12" r="10"/>
            <path d="M16 16s-1.5-1.5-4-1.5-4 1.5-4 1.5"/>
            <line x1="9" y1="9" x2="9.01" y2="9"/>
            <line x1="15" y1="9" x2="15.01" y2="9"/>
          </svg>
        </div>
        <p class="popup-message">
          {{ message || 'Эта функция пока недоступна. Мы работаем над её реализацией.' }}
        </p>
        <p class="popup-subtitle">
          Пожалуйста, используйте стандартную регистрацию или вход.
        </p>
      </div>
      <div class="popup-footer">
        <button @click="closePopup" class="popup-btn">
          Понятно
        </button>
      </div>
    </div>
  </div>
</template>

<script setup>
import { defineProps, defineEmits } from 'vue'

const props = defineProps({
  show: {
    type: Boolean,
    default: false
  },
  message: {
    type: String,
    default: ''
  }
})

const emit = defineEmits(['close'])

const closePopup = () => {
  emit('close')
}
</script>

<style scoped>
.popup-overlay {
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  bottom: 0;
  background: rgba(0, 0, 0, 0.75);
  backdrop-filter: blur(8px);
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: 1000;
  padding: 1rem;
}

.popup-content {
  background: rgba(255, 255, 255, 0.05);
  border: 1px solid rgba(255, 255, 255, 0.1);
  border-radius: 12px;
  backdrop-filter: blur(10px);
  box-shadow: 0 8px 32px rgba(0, 0, 0, 0.3);
  max-width: 400px;
  width: 100%;
  animation: popupSlideIn 0.3s ease-out;
}

@keyframes popupSlideIn {
  from {
    opacity: 0;
    transform: translateY(-10px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.popup-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  padding: 1rem 1.25rem 0.75rem;
  border-bottom: 1px solid rgba(255, 255, 255, 0.1);
}

.popup-header h3 {
  margin: 0;
  color: var(--text);
  font-size: 0.9rem;
  font-weight: 600;
}

.popup-close-btn {
  background: none;
  border: none;
  color: var(--text-muted);
  cursor: pointer;
  padding: 0.5rem;
  border-radius: 8px;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.popup-close-btn:hover {
  background: rgba(255, 255, 255, 0.1);
  color: var(--text);
}

.popup-body {
  padding: 0.75rem 1.25rem;
  text-align: center;
}

.popup-icon {
  margin-bottom: 1rem;
  color: var(--accent);
}

.popup-message {
  color: var(--text);
  font-size: 1rem;
  line-height: 1.5;
  margin-bottom: 0.75rem;
}

.popup-subtitle {
  color: var(--text-muted);
  font-size: 0.9rem;
  line-height: 1.4;
  margin: 0;
}

.popup-footer {
  padding: 0.75rem 0;
  display: flex;
  justify-content: center;
}

.popup-btn {
  width: 100%;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.75rem;
  padding: 0.75rem 1.25rem;
  background: none;
  border: none;
  color: var(--text);
  font-size: 0.9rem;
  font-weight: 500;
  cursor: pointer;
  transition: all 0.2s ease;
  text-align: center;
}

.popup-btn:hover {
  background: rgba(255, 255, 255, 0.05);
  color: var(--accent);
}

@media (max-width: 480px) {
  .popup-content {
    margin: 1rem;
  }
  
  .popup-header,
  .popup-body,
  .popup-footer {
    padding-left: 1rem;
    padding-right: 1rem;
  }
}
</style> 