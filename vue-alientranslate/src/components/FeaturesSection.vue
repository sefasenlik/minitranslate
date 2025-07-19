<template>
  <section id="features" class="features">
    <div class="container">
      <h2>Возможности</h2>
      <div class="grid">
        <div class="feature-card">
          <h3>Глобальные горячие клавиши</h3>
          <p>
            Работает по всему Windows с настраиваемыми сочетаниями клавиш. По умолчанию
            <em>Ctrl+Q</em>, также поддерживает <em>Ctrl+C+C</em> как DeepL.
          </p>
        </div>
        <div class="feature-card">
          <h3>Множественные сервисы</h3>
          <p>
            Выбирайте между Google Translate, Yandex Translate, ChatGPT или конфиденциальным
            Translation Server. Качество на уровне DeepL.
          </p>
        </div>
        <div class="feature-card">
          <h3>Умное определение языка</h3>
          <p>
            Автоматически определяет кириллицу, латиницу, арабский, китайский, японский,
            корейский и другие письменности.
          </p>
        </div>
        <div class="feature-card">
          <h3>Контекстные переводы</h3>
          <p>
            Указывайте контекст перевода: формальность, стиль, специализация. Получайте более
            точные результаты с ChatGPT и Translation Server.
          </p>
        </div>
        <div class="feature-card">
          <h3>Региональная доступность</h3>
          <p>
            Translation Server работает во всех регионах, включая Россию, где заблокирован
            ChatGPT. Конфиденциальная обработка без хранения данных.
          </p>
        </div>
        <div class="feature-card">
          <h3>Полная кастомизация</h3>
          <p>
            Настройте горячие клавиши, размеры окна, браузер (Chrome/Edge), автозапуск и даже
            собственный сервер переводов.
          </p>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { onMounted, onUnmounted } from 'vue';

let animationFrameId;

function initAutoScroll(containerSelector, speed = 0.3) {
  const container = document.querySelector(containerSelector);
  if (!container) return;

  const originalCards = Array.from(container.children);
  if (originalCards.length === 0) return;

  for (let i = 0; i < 3; i++) {
    originalCards.forEach(card => {
      container.appendChild(card.cloneNode(true));
    });
  }

  let isScrolling = true;
  let scrollAmount = 0;

  function scroll() {
    if (isScrolling) {
      scrollAmount += speed;
      if (scrollAmount >= container.scrollWidth / 4) {
        scrollAmount = 0;
      }
      container.scrollLeft = scrollAmount;
    }
    animationFrameId = requestAnimationFrame(scroll);
  }
  
  const startScroll = () => { isScrolling = true; };
  const stopScroll = () => { isScrolling = false; };

  container.addEventListener('mouseenter', stopScroll);
  container.addEventListener('mouseleave', startScroll);
  container.addEventListener('touchstart', stopScroll, { passive: true });
  container.addEventListener('touchend', startScroll, { passive: true });

  scroll();
}

onMounted(() => {
  initAutoScroll('#features .grid');
});

onUnmounted(() => {
  cancelAnimationFrame(animationFrameId);
});
</script> 