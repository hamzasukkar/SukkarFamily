// Islamic Family Website JavaScript
document.addEventListener('DOMContentLoaded', function() {
    
    // Initialize page animations
    initializeAnimations();
    
    // Setup navigation enhancements
    enhanceNavigation();
    
    // Setup Islamic patterns animation
    animateIslamicPatterns();
    
    // Setup responsive features
    handleResponsiveFeatures();
    
    // Setup accessibility improvements
    improveAccessibility();
    
    // Setup smooth scrolling
    setupSmoothScrolling();
});

// Initialize fade-in animations
function initializeAnimations() {
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };
    
    const observer = new IntersectionObserver((entries) => {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.classList.add('animate__animated', 'animate__fadeInUp');
                observer.unobserve(entry.target);
            }
        });
    }, observerOptions);
    
    // Observe elements with fade-in classes
    document.querySelectorAll('.fade-in-up, .fade-in-up-delay').forEach(el => {
        observer.observe(el);
    });
}

// Enhance navigation with active states and smooth transitions
function enhanceNavigation() {
    const navLinks = document.querySelectorAll('.navbar-nav .nav-link');
    const currentPath = window.location.pathname;
    
    // Set active navigation state
    navLinks.forEach(link => {
        if (link.getAttribute('href') === currentPath) {
            link.classList.add('active');
            link.style.color = 'var(--islamic-gold)';
        }
    });
    
    // Add hover effects
    navLinks.forEach(link => {
        link.addEventListener('mouseenter', function() {
            if (!this.classList.contains('active')) {
                this.style.transform = 'translateY(-2px)';
                this.style.color = 'var(--islamic-gold)';
            }
        });
        
        link.addEventListener('mouseleave', function() {
            if (!this.classList.contains('active')) {
                this.style.transform = 'translateY(0)';
                this.style.color = '';
            }
        });
    });
    
    // Enhance dropdown menus
    const dropdowns = document.querySelectorAll('.dropdown-menu');
    dropdowns.forEach(dropdown => {
        dropdown.addEventListener('show.bs.dropdown', function() {
            this.style.animation = 'fadeIn 0.3s ease';
        });
    });
}

// Animate Islamic geometric patterns
function animateIslamicPatterns() {
    const patterns = document.querySelectorAll('.islamic-pattern-bg, .islamic-pattern-overlay');
    
    patterns.forEach(pattern => {
        let rotation = 0;
        setInterval(() => {
            rotation += 0.5;
            pattern.style.transform = `rotate(${rotation}deg)`;
        }, 100);
    });
}

// Handle responsive features
function handleResponsiveFeatures() {
    let resizeTimer;
    
    window.addEventListener('resize', function() {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(function() {
            adjustLayoutForDevice();
            recalculateAnimations();
        }, 250);
    });
    
    // Initial adjustment
    adjustLayoutForDevice();
}

function adjustLayoutForDevice() {
    const isMobile = window.innerWidth <= 768;
    const isTablet = window.innerWidth <= 1024 && window.innerWidth > 768;
    
    // Adjust card spacing for mobile
    const cards = document.querySelectorAll('.card-islamic');
    cards.forEach(card => {
        if (isMobile) {
            card.style.marginBottom = '1rem';
        } else {
            card.style.marginBottom = '1.5rem';
        }
    });
    
    // Adjust hero section height
    const heroSection = document.querySelector('.hero-section');
    if (heroSection) {
        if (isMobile) {
            heroSection.style.minHeight = '50vh';
        } else if (isTablet) {
            heroSection.style.minHeight = '60vh';
        } else {
            heroSection.style.minHeight = '70vh';
        }
    }
}

// Improve accessibility
function improveAccessibility() {
    // Add ARIA labels to interactive elements
    const buttons = document.querySelectorAll('button, .btn');
    buttons.forEach(button => {
        if (!button.getAttribute('aria-label') && button.textContent.trim()) {
            button.setAttribute('aria-label', button.textContent.trim());
        }
    });
    
    // Improve keyboard navigation
    const focusableElements = document.querySelectorAll(
        'a, button, input, select, textarea, [tabindex]:not([tabindex="-1"])'
    );
    
    focusableElements.forEach(element => {
        element.addEventListener('focus', function() {
            this.style.outline = '3px solid var(--islamic-gold)';
            this.style.outlineOffset = '2px';
        });
        
        element.addEventListener('blur', function() {
            this.style.outline = '';
            this.style.outlineOffset = '';
        });
    });
    
    // Add screen reader support for Arabic content
    document.documentElement.setAttribute('lang', 'ar');
    
    // Improve image alt texts
    const images = document.querySelectorAll('img:not([alt])');
    images.forEach(img => {
        img.setAttribute('alt', 'صورة من تراث عائلة سكر');
    });
}

// Setup smooth scrolling
function setupSmoothScrolling() {
    const links = document.querySelectorAll('a[href^="#"]');
    
    links.forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            
            const targetId = this.getAttribute('href').substring(1);
            const targetElement = document.getElementById(targetId);
            
            if (targetElement) {
                const offsetTop = targetElement.offsetTop - 100; // Account for fixed navbar
                
                window.scrollTo({
                    top: offsetTop,
                    behavior: 'smooth'
                });
            }
        });
    });
}

function recalculateAnimations() {
    // Recalculate intersection observer thresholds for different screen sizes
    const isMobile = window.innerWidth <= 768;
    const threshold = isMobile ? 0.05 : 0.1;
    
    // Re-initialize animations with new thresholds
    initializeAnimations();
}

// Islamic Date Display
function displayIslamicDate() {
    const islamicMonths = [
        'محرم', 'صفر', 'ربيع الأول', 'ربيع الثاني', 'جمادى الأولى', 'جمادى الثانية',
        'رجب', 'شعبان', 'رمضان', 'شوال', 'ذو القعدة', 'ذو الحجة'
    ];
    
    // This is a simplified Islamic date - in production, use a proper Islamic calendar library
    const today = new Date();
    const islamicYear = 1445; // This should be calculated properly
    const islamicMonth = islamicMonths[today.getMonth()];
    const islamicDay = today.getDate();
    
    const islamicDateElements = document.querySelectorAll('.islamic-date');
    islamicDateElements.forEach(element => {
        element.textContent = `${islamicDay} ${islamicMonth} ${islamicYear}هـ`;
    });
}

// Card hover effects
document.addEventListener('DOMContentLoaded', function() {
    const cards = document.querySelectorAll('.card-islamic');
    
    cards.forEach(card => {
        card.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-8px)';
            this.style.boxShadow = '0 20px 40px rgba(0, 0, 0, 0.15)';
        });
        
        card.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0)';
            this.style.boxShadow = '';
        });
    });
});

// Loading state management
function showLoading(element) {
    const loadingHTML = `
        <div class="d-flex justify-content-center align-items-center p-4">
            <div class="loading-spinner me-3"></div>
            <span>جاري التحميل...</span>
        </div>
    `;
    element.innerHTML = loadingHTML;
}

function hideLoading(element, content) {
    element.innerHTML = content;
}

// Form enhancements for Arabic inputs
function enhanceArabicForms() {
    const arabicInputs = document.querySelectorAll('input[type="text"], textarea');
    
    arabicInputs.forEach(input => {
        // Auto-detect Arabic text and adjust direction
        input.addEventListener('input', function() {
            const arabicPattern = /[\u0600-\u06FF]/;
            if (arabicPattern.test(this.value)) {
                this.style.direction = 'rtl';
                this.style.textAlign = 'right';
            } else {
                this.style.direction = 'ltr';
                this.style.textAlign = 'left';
            }
        });
        
        // Enhanced validation for Arabic names
        input.addEventListener('blur', function() {
            if (this.classList.contains('arabic-name')) {
                validateArabicName(this);
            }
        });
    });
}

function validateArabicName(input) {
    const arabicNamePattern = /^[\u0600-\u06FF\s]+$/;
    const value = input.value.trim();
    
    if (value && !arabicNamePattern.test(value)) {
        input.classList.add('is-invalid');
        showValidationMessage(input, 'يرجى إدخال الاسم باللغة العربية فقط');
    } else {
        input.classList.remove('is-invalid');
        hideValidationMessage(input);
    }
}

function showValidationMessage(input, message) {
    let feedback = input.nextElementSibling;
    if (!feedback || !feedback.classList.contains('invalid-feedback')) {
        feedback = document.createElement('div');
        feedback.className = 'invalid-feedback';
        input.parentNode.insertBefore(feedback, input.nextSibling);
    }
    feedback.textContent = message;
}

function hideValidationMessage(input) {
    const feedback = input.nextElementSibling;
    if (feedback && feedback.classList.contains('invalid-feedback')) {
        feedback.remove();
    }
}

// Search functionality enhancement
function enhanceSearch() {
    const searchInputs = document.querySelectorAll('input[type="search"]');
    
    searchInputs.forEach(input => {
        let searchTimeout;
        
        input.addEventListener('input', function() {
            clearTimeout(searchTimeout);
            const query = this.value.trim();
            
            if (query.length >= 2) {
                searchTimeout = setTimeout(() => {
                    performSearch(query);
                }, 500);
            }
        });
    });
}

function performSearch(query) {
    // Implement search functionality
    console.log('بحث عن:', query);
    // This would typically make an AJAX call to search the family database
}

// Print functionality
function setupPrint() {
    const printButtons = document.querySelectorAll('.print-btn');
    
    printButtons.forEach(button => {
        button.addEventListener('click', function() {
            window.print();
        });
    });
    
    // Optimize for printing
    const mediaQueryList = window.matchMedia('print');
    mediaQueryList.addListener(function(mql) {
        if (mql.matches) {
            document.body.classList.add('print-mode');
        } else {
            document.body.classList.remove('print-mode');
        }
    });
}

// Export data functionality
function setupExport() {
    const exportButtons = document.querySelectorAll('.export-btn');
    
    exportButtons.forEach(button => {
        button.addEventListener('click', function() {
            const format = this.dataset.format;
            const data = this.dataset.content;
            
            switch(format) {
                case 'pdf':
                    exportToPDF(data);
                    break;
                case 'excel':
                    exportToExcel(data);
                    break;
                default:
                    console.log('تنسيق التصدير غير مدعوم');
            }
        });
    });
}

function exportToPDF(data) {
    // Implement PDF export
    console.log('تصدير إلى PDF');
}

function exportToExcel(data) {
    // Implement Excel export
    console.log('تصدير إلى Excel');
}

// Initialize all enhancements when DOM is loaded
document.addEventListener('DOMContentLoaded', function() {
    displayIslamicDate();
    enhanceArabicForms();
    enhanceSearch();
    setupPrint();
    setupExport();
});

// Utility functions
const IslamicSite = {
    showToast: function(message, type = 'success') {
        const toast = document.createElement('div');
        toast.className = `toast align-items-center text-white bg-${type} border-0`;
        toast.innerHTML = `
            <div class="d-flex">
                <div class="toast-body">${message}</div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        `;
        
        document.body.appendChild(toast);
        const bsToast = new bootstrap.Toast(toast);
        bsToast.show();
        
        toast.addEventListener('hidden.bs.toast', function() {
            toast.remove();
        });
    },
    
    formatArabicDate: function(date) {
        const options = { 
            year: 'numeric', 
            month: 'long', 
            day: 'numeric',
            calendar: 'islamic'
        };
        return new Intl.DateTimeFormat('ar-SA-u-ca-islamic', options).format(date);
    },
    
    debounce: function(func, wait) {
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
};