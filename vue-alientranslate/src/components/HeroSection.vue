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
          <a href="translate.html" class="btn secondary">Попробовать онлайн</a>
          <div class="download-group">
            <a href="#download" class="btn primary">Скачать бесплатно</a>
            <div class="hero-small">для Windows</div>
          </div>
        </div>
      </div>
    </div>
  </section>
</template>

<script setup>
import { onMounted } from 'vue';

onMounted(() => {
  const heroSection = document.querySelector('.hero');
  if (!heroSection) return;

  const translations = [
    "Hello, I am your alien translator!", "Привет, я твой переводчик-чужик!", 
    "¡Hola, soy tu traductor alienígena!", "Bonjour, je suis votre traducteur extraterrestre!",
    "Hallo, ich bin Ihr außerirdischer Übersetzer!", "Ciao, sono il tuo traduttore alieno!",
    "Olá, sou seu tradutor alienígena!", "你好，我是你的外星翻译！",
    "こんにちは、私はあなたのエイリアン翻訳者です！", "안녕하세요, 저는 당신의 외계인 번역가입니다!",
    "مرحبًا، أنا مترجمك الفضائي!", "नमस्ते, मैं आपका एलियन अनुवादक हूँ!",
    "Merhaba, ben senin uzaylı çevirmeninim!", "Witaj, jestem twoim kosmicznym tłumaczem!",
    "Hallo, ik ben jouw buitenaardse vertaler!", "Hallå, jag är din utomjordiska översättare!",
    "Hej, jeg er din udenjordiske oversætter!", "Hei, jeg er din utenomjordiske oversetter!",
    "Hei, olen avaruustulkki!", "Ahoj, jsem tvůj mimozemský překladatel!",
    "Привіт, я твій інопланетний перекладач!", "Здравей, аз съм твоят извънземен преводач!",
    "Bok, ja sam tvoj izvanzemaljski prevoditelj!", "Ahoj, som tvoj mimozemský prekladateľ!",
    "Živjo, jaz sem tvoj vesoljski prevajalec!", "Tere, ma olen sinu tulnukast tõlkija!",
    "Здраво, ја сам твој ванземаљски преводилац!", "Sveiki, aš esu jūsų ateivių vertėjas!",
    "Helló, én vagyok az idegen fordítód!", "Salut, sunt traducătorul tău extraterestru!",
    "Γεια, είμαι ο εξωγήινος μεταφραστής σου!", "שלום, אני המתרגם החייזרי שלך!",
    "สวัสดี ฉันคือโปรแกรมแปลภาษาต่างดาวของคุณ!", "Xin chào, tôi là phiên dịch viên ngoài hành tinh của bạn!",
    "Halo, saya penerjemah alien Anda!", "Hai, saya penterjemah makhluk asing anda!"
  ];

  function generateBackgroundLines() {
    const lines = [];
    const repetitions = 60;
    const minLineLength = 500;
    for (let i = 0; i < repetitions; i++) {
      let line = '';
      while (line.length < minLineLength) {
        const randomTranslation = translations[Math.floor(Math.random() * translations.length)];
        line += randomTranslation + ' ';
      }
      lines.push(line.trim());
    }
    return lines;
  }

  function createBackgroundText() {
    const existingBg = heroSection.querySelector('.background-text');
    if (existingBg) existingBg.remove();

    const backgroundTextElement = document.createElement('pre');
    backgroundTextElement.className = 'background-text';
    Object.assign(backgroundTextElement.style, {
      position: 'absolute',
      top: '-50px', left: '-50px', right: '-50px', bottom: '-50px',
      fontSize: '12px', lineHeight: '1.1', color: 'rgba(255, 255, 255, 0.05)',
      fontWeight: '400', zIndex: '1', pointerEvents: 'none', userSelect: 'none',
      textAlign: 'left', padding: '20px', overflow: 'hidden',
      width: 'calc(100vw + 100px)', whiteSpace: 'pre', transform: 'rotate(-3deg)',
    });

    backgroundTextElement.textContent = generateBackgroundLines().join('\n');
    heroSection.insertBefore(backgroundTextElement, heroSection.firstChild);
  }

  createBackgroundText();
});
</script> 