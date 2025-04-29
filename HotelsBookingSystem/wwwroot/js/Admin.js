document.addEventListener('DOMContentLoaded', function () {
    // Sidebar toggle functionality
    const toggleBtn = document.getElementById('toggleSidebar');
    const sidebar = document.getElementById('sidebar');
    const overlay = document.getElementById('overlay');
    const isMobile = window.innerWidth <= 1024;

    toggleBtn.addEventListener('click', function () {
        sidebar.classList.toggle('active');
        if (isMobile) {
            overlay.classList.toggle('active');
        } else {
            sidebar.classList.toggle('collapsed');
        }
    });

    overlay.addEventListener('click', function () {
        sidebar.classList.remove('active');
        overlay.classList.remove('active');
    });

    // Search and filter functionality
    const searchInputs = document.querySelectorAll('.search-input');
    const filterButtons = document.querySelectorAll('.filter-btn');

    searchInputs.forEach(input => {
        input.addEventListener('input', function () {
            console.log('Search input:', this.value);
            // Search logic would go here
        });
    });

    filterButtons.forEach(button => {
        button.addEventListener('click', function () {
            console.log('Filter button clicked');
            // Filtering logic would go here
        });
    });

    // Action buttons
    const actionButtons = document.querySelectorAll('.action-btn');

    actionButtons.forEach(button => {
        button.addEventListener('click', function () {
            console.log('Action button clicked:', this.textContent.trim());
            // Action handling logic would go here
        });
    });

    // Responsive sidebar behavior
    window.addEventListener('resize', function () {
        const newIsMobile = window.innerWidth <= 1024;

        if (newIsMobile !== isMobile) {
            // Refresh page to apply correct sidebar state
            // For a real app, you'd handle this more gracefully
            if (sidebar.classList.contains('active') || sidebar.classList.contains('collapsed')) {
                sidebar.classList.remove('active', 'collapsed');
                overlay.classList.remove('active');
            }
        }
    });
});

const currentPath = window.location.pathname;
const navLinks = document.querySelectorAll('.sidebar .nav-link');

navLinks.forEach(link => {
    link.classList.remove('active');

    const href = link.getAttribute('href');

    if (currentPath.endsWith(href) || (href !== '/' && currentPath.includes(href))) {
        link.classList.add('active');
    }
});