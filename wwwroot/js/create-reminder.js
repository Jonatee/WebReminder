document.addEventListener('DOMContentLoaded', function () {
    // Form elements
    const titleInput = document.getElementById('Title');
    const descriptionInput = document.getElementById('Description');
    const dueDateInput = document.getElementById('DueDate');
    const imageInput = document.getElementById('Image');

    // Preview elements
    const previewTitle = document.getElementById('preview-title');
    const previewDescription = document.getElementById('preview-description');
    const previewDate = document.getElementById('preview-date');
    const previewImage = document.querySelector('.preview-image img');

    // Update preview on input changes
    titleInput.addEventListener('input', function () {
        previewTitle.textContent = this.value || 'Reminder Title';
        previewTitle.classList.add('preview-update');
        setTimeout(() => previewTitle.classList.remove('preview-update'), 1000);
    });

    descriptionInput.addEventListener('input', function () {
        previewDescription.textContent = this.value || 'Reminder description will appear here...';
        previewDescription.classList.add('preview-update');
        setTimeout(() => previewDescription.classList.remove('preview-update'), 1000);
    });

    dueDateInput.addEventListener('input', function () {
        if (this.value) {
            const date = new Date(this.value);
            const options = {
                year: 'numeric',
                month: 'short',
                day: 'numeric',
                hour: '2-digit',
                minute: '2-digit'
            };
            previewDate.textContent = date.toLocaleDateString('en-US', options);
        } else {
            previewDate.textContent = 'Select a due date';
        }
        previewDate.classList.add('preview-update');
        setTimeout(() => previewDate.classList.remove('preview-update'), 1000);
    });

    // Image preview
    imageInput.addEventListener('change', function () {
        if (this.files && this.files[0]) {
            const reader = new FileReader();

            reader.onload = function (e) {
                previewImage.src = e.target.result;
                previewImage.classList.add('preview-update');
                setTimeout(() => previewImage.classList.remove('preview-update'), 1000);
            }

            reader.readAsDataURL(this.files[0]);
        }
    });

    // Mobile navigation toggle (if needed)
    const mobileToggle = document.getElementById('mobile-toggle');
    const navMenu = document.getElementById('nav-menu');

    if (mobileToggle && navMenu) {
        mobileToggle.addEventListener('click', function () {
            navMenu.classList.toggle('nav-hidden');
        });
    }
});