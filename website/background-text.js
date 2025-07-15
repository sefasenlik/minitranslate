// Background text generator for the hero section
// Improved: Use a single <pre> for background text, restore tilt, prevent overlap

document.addEventListener('DOMContentLoaded', function() {
    const heroSection = document.querySelector('.hero');
    if (!heroSection) return;

    const translations = [
        // English
        "Hello, I am your alien translator!",
        // Russian
        "Привет, я твой переводчик-чужик!",
        // Spanish
        "¡Hola, soy tu traductor alienígena!",
        // French
        "Bonjour, je suis votre traducteur extraterrestre!",
        // German
        "Hallo, ich bin Ihr außerirdischer Übersetzer!",
        // Italian
        "Ciao, sono il tuo traduttore alieno!",
        // Portuguese
        "Olá, sou seu tradutor alienígena!",
        // Chinese (Simplified)
        "你好，我是你的外星翻译！",
        // Japanese
        "こんにちは、私はあなたのエイリアン翻訳者です！",
        // Korean
        "안녕하세요, 저는 당신의 외계인 번역가입니다!",
        // Arabic
        "مرحبًا، أنا مترجمك الفضائي!",
        // Hindi
        "नमस्ते, मैं आपका एलियन अनुवादक हूँ!",
        // Turkish
        "Merhaba, ben senin uzaylı çevirmeninim!",
        // Polish
        "Witaj, jestem twoim kosmicznym tłumaczem!",
        // Dutch
        "Hallo, ik ben jouw buitenaardse vertaler!",
        // Swedish
        "Hallå, jag är din utomjordiska översättare!",
        // Danish
        "Hej, jeg er din udenjordiske oversætter!",
        // Norwegian
        "Hei, jeg er din utenomjordiske oversetter!",
        // Finnish
        "Hei, olen avaruustulkki!",
        // Czech
        "Ahoj, jsem tvůj mimozemský překladatel!",
        // Ukrainian
        "Привіт, я твій інопланетний перекладач!",
        // Bulgarian
        "Здравей, аз съм твоят извънземен преводач!",
        // Croatian
        "Bok, ja sam tvoj izvanzemaljski prevoditelj!",
        // Slovak
        "Ahoj, som tvoj mimozemský prekladateľ!",
        // Slovenian
        "Živjo, jaz sem tvoj vesoljski prevajalec!",
        // Estonian
        "Tere, ma olen sinu tulnukast tõlkija!",
        // Serbian
        "Здраво, ја сам твој ванземаљски преводилац!",
        // Lithuanian
        "Sveiki, aš esu jūsų ateivių vertėjas!",
        // Hungarian
        "Helló, én vagyok az idegen fordítód!",
        // Romanian
        "Salut, sunt traducătorul tău extraterestru!",
        // Greek
        "Γεια, είμαι ο εξωγήινος μεταφραστής σου!",
        // Hebrew
        "שלום, אני המתרגם החייזרי שלך!",
        // Thai
        "สวัสดี ฉันคือโปรแกรมแปลภาษาต่างดาวของคุณ!",
        // Vietnamese
        "Xin chào, tôi là phiên dịch viên ngoài hành tinh của bạn!",
        // Indonesian
        "Halo, saya penerjemah alien Anda!",
        // Malay
        "Hai, saya penterjemah makhluk asing anda!"
    ];

    function generateBackgroundLines() {
        const lines = [];
        const repetitions = 60; // Number of lines
        const minLineLength = 500; // Minimum characters per line
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
            top: '-50px',
            left: '-50px',
            right: '-50px',
            bottom: '-50px',
            fontSize: '12px',
            lineHeight: '1.1',
            color: 'rgba(255, 255, 255, 0.05)',
            fontWeight: '400',
            zIndex: '1',
            pointerEvents: 'none',
            userSelect: 'none',
            textAlign: 'left',
            padding: '20px',
            overflow: 'hidden',
            width: 'calc(100vw + 100px)',
            whiteSpace: 'pre',
            transform: 'rotate(-3deg)',
        });

        // Join all lines with newlines
        backgroundTextElement.textContent = generateBackgroundLines().join('\n');

        heroSection.insertBefore(backgroundTextElement, heroSection.firstChild);
    }

    createBackgroundText();

    // Auto-scroll functionality for cards
    function initAutoScroll() {
        const featuresGrid = document.querySelector('.features .grid');
        const servicesGrid = document.querySelector('.services-grid');
        
        function createInfiniteScroller(container, speed = 1) {
            if (!container) return;
            
            // Store original cards before cloning
            const originalCards = Array.from(container.children);
            const cardCount = originalCards.length;
            
            // Create multiple copies for smoother infinite scroll
            for (let copy = 0; copy < 3; copy++) {
                originalCards.forEach(card => {
                    const clone = card.cloneNode(true);
                    container.appendChild(clone);
                });
            }
            
            let isScrolling = true;
            let animationId;
            let startTime = null;
            let pausedTime = 0;
            
            function scroll(currentTime) {
                if (!isScrolling) return;
                
                if (startTime === null) {
                    startTime = currentTime - pausedTime;
                }
                
                // Calculate position based on time for consistent speed
                const elapsed = currentTime - startTime;
                const position = (elapsed * speed * 0.1) % (container.scrollWidth * 0.25); // Reset at 25% to avoid lag
                
                container.scrollLeft = position;
                
                animationId = requestAnimationFrame(scroll);
            }
            
            // Start scrolling
            function startScroll() {
                if (animationId) {
                    cancelAnimationFrame(animationId);
                }
                isScrolling = true;
                animationId = requestAnimationFrame(scroll);
            }
            
            // Stop scrolling
            function stopScroll() {
                isScrolling = false;
                if (animationId) {
                    cancelAnimationFrame(animationId);
                    animationId = null;
                }
            }
            
            // Initialize after layout is ready
            setTimeout(() => {
                startScroll();
            }, 100);
            
            // Pause on hover for any card (including clones)
            container.addEventListener('mouseenter', () => {
                pausedTime = performance.now() - (startTime || 0);
                stopScroll();
            });
            
            container.addEventListener('mouseleave', () => {
                startTime = performance.now() - pausedTime;
                startScroll();
            });
            
            // Pause on touch/interaction
            container.addEventListener('touchstart', () => {
                pausedTime = performance.now() - (startTime || 0);
                stopScroll();
            });
            
            container.addEventListener('touchend', () => {
                setTimeout(() => {
                    startTime = performance.now() - pausedTime;
                    startScroll();
                }, 2000); // Resume after 2 seconds
            });
            
            // Reset on resize
            window.addEventListener('resize', () => {
                pausedTime = performance.now() - (startTime || 0);
                stopScroll();
                setTimeout(() => {
                    startTime = performance.now() - pausedTime;
                    startScroll();
                }, 100);
            });
        }
        
        // Initialize infinite scroll for both sections
        createInfiniteScroller(featuresGrid, 0.3);
        createInfiniteScroller(servicesGrid, 0.3);
    }
    
    // Initialize auto-scroll after a short delay to ensure elements are loaded
    setTimeout(initAutoScroll, 1000);
}); 