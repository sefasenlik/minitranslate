// Common translations used across the application
export const translations = [
  "Hello, I am your alien translator!", "Привет, я твой переводчик-чужик!",
  "¡Hola, soy tu traductor alienígena!", "Bonjour, je suis votre traducteur extraterrestre!",
  "Hallo, ich bin Ihr außerirdischer Übersetzer!", "Ciao, sono il tuo traduttore alieno!",
  "Olá, sou seu tradutor alienígena!", "你好，我是你的外星翻译！",
  "こんにちは、私はあなたのエイリアン翻訳者です！", "안녕하세요, 저는 당신의 외계인 번역가입니다!",
  "مرحبًا، أنا مترجمك الفضائي!", "नमस्ते, मैं आपका एलियन अनुवादк हूँ!",
  "Merhaba, ben senin uzaylı çevirmeninim!", "Witaj, jestem twoim kosmicznym tłumaczem!",
  "Hallo, ik ben jouw buitenaardse vertaler!", "Hallå, jag är din utomjordiska översättare!",
  "Hej, jeg er din udenjordiske oversætter!", "Hei, jeg er din utenomjordiske oversetter!",
  "Hei, olen avaruustulkki!", "Ahoj, jsem tvůj mimozemský překladatel!",
  "Привіт, я твій інопланетний перекладач!", "Здравей, аз съм твоят извънземен преводач!",
  "Bok, ja sam tvoj izvanzemaljski prevoditelj!", "Ahoj, som tvoj mimozemský prekladateľ!",
  "Živjo, jaz sem tvoj vesoljski prevajalec!", "Tere, ma olen sinu tulnukast tõlkija!",
  "Здраво, ја сам твој ванземаљски преводилац!", "Sveiki, aš esu jūsų ateivių vertėjas!",
  "Helló, én vagyok az idegen fordítód!", "Salut, sunt traducătorul tău extraterestru!",
  "Γεια, είμαι ο εξωγήινος μεταφραсτής σου!", "שלום, אני המתרגם החייזרי שלך!",
  "สวัสดี ฉันคือโปรแกรมแปลภาษาต่างดาวของคุณ!", "Xin chào, tôi là phiên dịch viên ngoài hành tinh của bạn!",
  "Halo, saya penerjemah alien Anda!", "Hai, saya penterjemah makhluk asing anda!"
];

// Debounce function to limit how often a function can run
function debounce(func, wait) {
  let timeout;
  return function executedFunction(...args) {
    const later = () => {
      clearTimeout(timeout);
      func(...args);
    };
    clearTimeout(timeout);
    timeout = setTimeout(later, wait);
  };
}

let activeBackgroundUpdater = null;

/**
 * Creates a stylish, animated background text element that covers the entire container,
 * accounting for scrollable area and zoom levels.
 *
 * @param {HTMLElement} container - The container element to append the background text to.
 * @returns {object} An object with the element and a manual update function.
 */
export function createUnifiedBackgroundText(container) {
  if (!container) {
    console.error('A container must be provided for the background text.');
    return;
  }

  const existingBg = container.querySelector('.background-text');
  if (existingBg) existingBg.remove();

  const backgroundTextElement = document.createElement('pre');
  backgroundTextElement.className = 'background-text';

  function generateLines(numLines, charsPerLine) {
    const lines = [];
    for (let i = 0; i < numLines; i++) {
      let line = '';
      while (line.length < charsPerLine) {
        line += translations[Math.floor(Math.random() * translations.length)] + ' ';
      }
      lines.push(line.substring(0, charsPerLine));
    }
    return lines.join('\n');
  }

  function updateBackground() {
    const contentWidth = document.documentElement.scrollWidth;
    const contentHeight = document.documentElement.scrollHeight;
    
    const buffer = 500;
    
    const requiredWidth = contentWidth + buffer;
    const requiredHeight = contentHeight + buffer;

    const fontSize = 12;
    const lineHeight = fontSize * 1.2;
    const numLines = Math.ceil(requiredHeight / lineHeight);
    const charsPerLine = Math.ceil(requiredWidth / (fontSize * 0.5)); // Adjusted for ~15-20% longer lines

    Object.assign(backgroundTextElement.style, {
      position: 'absolute',
      top: `-${buffer / 2}px`,
      left: `-${buffer / 2}px`,
      width: `${requiredWidth}px`,
      height: `${requiredHeight}px`,
      fontSize: `${fontSize}px`,
      lineHeight: `${lineHeight}px`,
      color: 'rgba(255, 255, 255, 0.05)',
      fontWeight: '400',
      zIndex: '1',
      pointerEvents: 'none',
      userSelect: 'none',
      overflow: 'hidden',
      whiteSpace: 'pre',
      transform: 'rotate(-3deg)',
    });
    
    backgroundTextElement.textContent = generateLines(numLines, charsPerLine);
  }

  // Initial setup
  updateBackground();
  if (getComputedStyle(container).position === 'static') {
    container.style.position = 'relative';
  }
  container.insertBefore(backgroundTextElement, container.firstChild);

  // Debounced resize handler for performance
  const debouncedUpdate = debounce(updateBackground, 150);
  window.addEventListener('resize', debouncedUpdate);

  // Cleanup function
  const remove = () => {
    window.removeEventListener('resize', debouncedUpdate);
    if (backgroundTextElement.parentElement) {
      backgroundTextElement.parentElement.removeChild(backgroundTextElement);
    }
    activeBackgroundUpdater = null;
  };

  activeBackgroundUpdater = updateBackground;

  return {
    element: backgroundTextElement,
    remove,
    update: updateBackground // Expose the update function
  };
}

/**
 * Manually trigger a refresh of the active background text.
 * Useful for when content changes without a window resize event.
 */
export function refreshBackground() {
  if (activeBackgroundUpdater) {
    activeBackgroundUpdater();
  }
}
