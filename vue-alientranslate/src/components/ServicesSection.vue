<template>
  <section id="services" class="services">
    <div class="container">
      <h2>Сервисы переводов</h2>
      <div class="services-grid">
        <div class="service-card">
          <h3>Google Translate</h3>
          <p>
            Быстрые и надёжные переводы с поддержкой 100+ языков. Идеально для повседневного
            использования.
          </p>
          <ul>
            <li>100+ языков</li>
            <li>Быстрая обработка</li>
            <li>Бесплатный доступ</li>
          </ul>
        </div>
        <div class="service-card">
          <h3>Yandex Translate</h3>
          <p>Высокое качество переводов с отличной поддержкой русского языка. 90+ языков.</p>
          <ul>
            <li>90+ языков</li>
            <li>Отличный русский</li>
            <li>Бесплатный доступ</li>
          </ul>
        </div>
        <div class="service-card">
          <h3>ChatGPT Translator</h3>
          <p>ИИ-переводы с превосходным качеством и пониманием контекста. 35+ языков.</p>
          <ul>
            <li>35+ языков</li>
            <li>Контекстные переводы</li>
            <li>Высокое качество</li>
          </ul>
          <div class="card-requirement">Требует свой API ключ OpenAI</div>
        </div>
        <div class="service-card service-card-featured">
          <div class="diagonal-banner">Рекомендуется</div>
          <h3>Translation Server</h3>
          <p>
            Конфиденциальная серверная обработка с качеством DeepL. Работает во всех регионах.
          </p>
          <ul>
            <li>Качество DeepL</li>
            <li>Конфиденциальность</li>
            <li>Все регионы</li>
          </ul>
          <div class="card-requirement">Доступен при регистрации</div>
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
  initAutoScroll('#services .services-grid');
});

onUnmounted(() => {
  cancelAnimationFrame(animationFrameId);
});
</script> 