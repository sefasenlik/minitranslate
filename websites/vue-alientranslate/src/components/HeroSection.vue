<template>
  <section class="hero">
    <div class="hero-content">
      <div class="hero-image-box">
        <img src="/main_image.png" alt="Демонстрация переводчужик" class="hero-image" />
      </div>
      <div class="hero-text">
        <h1>Перевод со скоростью света</h1>
        <p>
          переводчужик - это не просто переводчик, а пришелец, знающий все языки вселенной.
          выдели текст, нажми <em>Ctrl+Q</em> - космически точный перевод готов! <br /><br />
          <i> Поддерживает 35+ языков и работает во всех регионах мира.</i>
        </p>
        <div class="hero-buttons">
          <a href="#" @click.prevent="$emit('navigate', 'download')" class="btn secondary">Попробовать онлайн</a>
          <div class="download-group">
            <a href="#" @click.prevent="$emit('navigate', 'download')" class="btn primary">Скачать бесплатно</a>
            <div class="hero-small">для Windows</div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { onMounted, onUnmounted } from 'vue';
import { createUnifiedBackgroundText } from '../utils/backgroundText.js';

onMounted(() => {
  const heroSection = document.querySelector('.hero');
  if (!heroSection) return;

  const background = createUnifiedBackgroundText(heroSection);

  onUnmounted(() => {
    if (background && typeof background.remove === 'function') {
      background.remove();
    }
  });
});
</script>
