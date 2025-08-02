<template>
  <section id="download" class="cta">
    <h2>Готовы начать?</h2>
    <a href="#" @click.prevent="$emit('navigate', 'download')" class="btn primary btn-large">Скачать для Windows</a>
    <p class="muted">Версии для macOS и Linux — скоро.</p>
    <div class="cta-features">
      <div class="cta-feature">
        <span class="feature-icon">🔒</span>
        <span>Конфиденциальность</span>
      </div>
      <div class="cta-feature">
        <span class="feature-icon">🌍</span>
        <span>Все регионы</span>
      </div>
      <div class="cta-feature">
        <span class="feature-icon">⚡</span>
        <span>Мгновенно</span>
      </div>
    </div>
  </section>
</template>

<script setup></script> 