document.addEventListener("DOMContentLoaded", function () {

    /****************** Showing Password ******************/

    const togglePasswordIcons = document.querySelectorAll('.togglePassword');
    const passwordFields = document.querySelectorAll('.password');

    togglePasswordIcons.forEach((icon, index) => {
        icon.addEventListener('click', function () {
            // Toggle the type attribute
            const passwordField = passwordFields[index]; // Get the corresponding password field
            const type = passwordField.getAttribute('type') === 'password' ? 'text' : 'password';
            passwordField.setAttribute('type', type);

            // Toggle the eye icon
            this.classList.toggle('bi-eye-fill');
            this.classList.toggle('bi-eye-slash-fill');
        });
    });


});

