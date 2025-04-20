document.addEventListener('DOMContentLoaded', function () {
    const darkModeToggle = document.getElementById('dark-mode-toggle');
    const htmlElement = document.documentElement;

    // Check for saved user preference
    const savedDarkMode = localStorage.getItem('darkMode');

    // Check if user has a system preference
    const prefersDarkMode = window.matchMedia('(prefers-color-scheme: dark)').matches;

    // Apply dark mode if saved preference exists or system prefers dark
    if (savedDarkMode === 'true' || (savedDarkMode === null && prefersDarkMode)) {
        htmlElement.classList.add('dark-mode');
        updateDarkModeIcons(true);
    }

    // Toggle dark mode on button click
    if (darkModeToggle) {
        darkModeToggle.addEventListener('click', function () {
            const isDarkMode = htmlElement.classList.toggle('dark-mode');
            updateDarkModeIcons(isDarkMode);

            // Save user preference
            localStorage.setItem('darkMode', isDarkMode);
        });
    }

    // Update icons based on dark mode state
    function updateDarkModeIcons(isDarkMode) {
        if (darkModeToggle) {
            const moonIcon = darkModeToggle.querySelector('.icon-moon');
            const sunIcon = darkModeToggle.querySelector('.icon-sun');

            if (isDarkMode) {
                moonIcon.style.display = 'none';
                sunIcon.style.display = 'block';
            }
            else {
                moonIcon.style.display = 'block';
                sunIcon.style.display = 'none';
            }
        }
    }
});