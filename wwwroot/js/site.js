// site.js - updated mobile toggle with inline SVG

// Mobile navigation toggle functionality
// (expects inline SVGs with IDs `hamburger-icon` and `close-icon` in the HTML)
document.addEventListener('DOMContentLoaded', function () {
    const mobileToggle = document.getElementById('mobile-toggle');
    const navMenu = document.getElementById('nav-menu');
    const hamIcon = document.getElementById('hamburger-icon');
    const closeIcon = document.getElementById('close-icon');

    if (mobileToggle && navMenu && hamIcon && closeIcon) {
        // Initial state: hide nav menu on mobile, show hamburger, hide close icon
        navMenu.classList.add('nav-hidden');
        hamIcon.style.display = 'block';
        closeIcon.style.display = 'none';

        // Toggle menu visibility
        mobileToggle.addEventListener('click', function () {
            navMenu.classList.toggle('nav-hidden');
            if (navMenu.classList.contains('nav-hidden')) {
                hamIcon.style.display = 'block';
                closeIcon.style.display = 'none';
            } else {
                hamIcon.style.display = 'none';
                closeIcon.style.display = 'block';
            }
        });

        // Close menu when clicking outside
        document.addEventListener('click', function (event) {
            const isClickInsideNav = navMenu.contains(event.target);
            const isClickOnToggle = mobileToggle.contains(event.target);

            if (!isClickInsideNav && !isClickOnToggle && !navMenu.classList.contains('nav-hidden')) {
                navMenu.classList.add('nav-hidden');
                hamIcon.style.display = 'block';
                closeIcon.style.display = 'none';
            }
        });

        // Handle window resize to switch between mobile and desktop views
        window.addEventListener('resize', function () {
            if (window.innerWidth > 768) {
                navMenu.classList.remove('nav-hidden');
                hamIcon.style.display = 'block';
                closeIcon.style.display = 'none';
            } else {
                navMenu.classList.add('nav-hidden');
                hamIcon.style.display = 'block';
                closeIcon.style.display = 'none';
            }
        });

        // Initial screen size check
        if (window.innerWidth > 768) {
            navMenu.classList.remove('nav-hidden');
            hamIcon.style.display = 'block';
            closeIcon.style.display = 'none';
        }
    }

    // Form validation logic (unchanged)
    const forms = document.querySelectorAll('form');
    forms.forEach(form => {
        form.addEventListener('submit', function (event) {
            let isValid = true;
            const inputs = form.querySelectorAll('input[required]');

            inputs.forEach(input => {
                if (!input.value.trim()) {
                    isValid = false;

                    // Remove existing error message
                    const existingError = input.parentNode.querySelector('.error-message');
                    if (existingError) existingError.remove();

                    // Add error message
                    const errorMessage = document.createElement('p');
                    errorMessage.className = 'error-message';
                    errorMessage.textContent = 'This field is required';
                    input.parentNode.appendChild(errorMessage);

                    // Highlight invalid input
                    input.style.borderColor = 'red';
                }
            });

            if (!isValid) event.preventDefault();
        });
    });
});

// Pinterest embed responsive handling (unchanged)
document.addEventListener('DOMContentLoaded', function () {
    function adjustPinterestEmbeds() {
        const pinterestContainers = document.querySelectorAll('.auth-card-pinterest, .pinterest-embed-container');
        pinterestContainers.forEach(container => {
            const iframe = container.querySelector('iframe');
            if (iframe) {
                iframe.style.width = '100%';
                iframe.style.height = '100%';
            }
        });
    }

    adjustPinterestEmbeds();
    window.addEventListener('resize', adjustPinterestEmbeds);
});
