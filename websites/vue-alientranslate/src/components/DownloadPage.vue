<template>
  <div class="auth-page" @click.self="closePage">
    <div class="auth-container">
      <div class="auth-card">
        <button @click="closePage" class="close-btn">
          <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
            <line x1="18" y1="6" x2="6" y2="18"></line>
            <line x1="6" y1="6" x2="18" y2="18"></line>
          </svg>
        </button>
        <div class="auth-header">
          <img src="/logo.png" alt="логотип переводчужик" class="auth-logo" />
          <h1>Загрузки</h1>
          <p>Выберите версию для загрузки</p>
        </div>

        <div v-if="loading" class="loading-container">
          <div class="loading-spinner"></div>
          <p>Загрузка списка файлов...</p>
        </div>

        <div v-else-if="error" class="error-container">
          <p>{{ error }}</p>
        </div>

        <div v-else class="download-list">
          <div v-for="file in files" :key="file.name" class="download-item">
            <div class="file-info">
              <span class="file-name">{{ file.name }}</span>
              <div class="file-details">
                <span class="file-size">{{ file.size }}</span>
                <span class="file-date">{{ formatDate(file.lastModified) }}</span>
              </div>
            </div>
            <a :href="file.url" class="btn btn-primary" download>
              <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2">
                <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"></path>
                <polyline points="7 10 12 15 17 10"></polyline>
                <line x1="12" y1="15" x2="12" y2="3"></line>
              </svg>
              Скачать
            </a>
          </div>
        </div>

        <div class="auth-footer">
          <p>Все файлы проверены на наличие вирусов</p>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted, onUnmounted } from 'vue';
import { createUnifiedBackgroundText } from '../utils/backgroundText.js';
import apiService from '../services/api.js';

const emit = defineEmits(['close']);

const loading = ref(true);
const error = ref(null);
const files = ref([]);

const fetchReleaseFiles = async () => {
  try {
    files.value = await apiService.getReleaseFiles();
  } catch (err) {
    error.value = 'Не удалось загрузить список файлов.';
    console.error(err);
  } finally {
    loading.value = false;
  }
};

onMounted(() => {
  fetchReleaseFiles();
  const authPage = document.querySelector('.auth-page');
  if (!authPage) return;

  const background = createUnifiedBackgroundText(authPage);

  onUnmounted(() => {
    if (background && typeof background.remove === 'function') {
      background.remove();
    }
  });
});

const closePage = () => {
  emit('close');
};

const formatDate = (dateString) => {
  const date = new Date(dateString);
  return date.toLocaleDateString('ru-RU', {
    year: 'numeric',
    month: 'short',
    day: 'numeric'
  });
};
</script>

<style scoped>
/* Reusing auth styles for consistency */
.auth-page {
  min-height: 100vh;
  display: flex;
  align-items: center;
  justify-content: center;
  padding: 2rem 1rem;
  background: transparent;
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

.loading-container,
.error-container {
  text-align: center;
  padding: 2rem 0;
  color: var(--text-muted);
}

.download-list {
  display: flex;
  flex-direction: column;
  gap: 1rem;
}

.download-item {
  display: flex;
  justify-content: space-between;
  align-items: center;
  background: rgba(255, 255, 255, 0.05);
  padding: 1rem;
  border-radius: 8px;
  border: 1px solid rgba(255, 255, 255, 0.1);
  transition: all 0.2s ease;
}

.download-item:hover {
    transform: translateY(-2px);
    border-color: rgba(255, 255, 255, 0.2);
}

.file-info {
  display: flex;
  flex-direction: column;
  flex-grow: 1;
}

.file-name {
  font-weight: 500;
  color: var(--text);
  margin-bottom: 0.25rem;
}

.file-details {
  display: flex;
  gap: 1rem;
  font-size: 0.85rem;
  color: var(--text-muted);
}

.file-size {
  font-weight: 500;
}

.file-date {
  opacity: 0.8;
}

.auth-footer {
  text-align: center;
  margin-top: 2rem;
  font-size: 0.85rem;
  color: var(--text-muted);
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

.btn {
  display: inline-flex;
  align-items: center;
  gap: 0.5rem;
  color: var(--accent);
}
</style>
