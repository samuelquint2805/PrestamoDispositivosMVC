document.addEventListener('DOMContentLoaded', function () {
    const loginToggle = document.getElementById('loginToggle');
    const loginDropdown = document.getElementById('loginDropdown');
    const loginForm = document.getElementById('loginForm');
    const loginError = document.getElementById('loginError');
    const loginBtn = document.getElementById('loginBtn');

    // Toggle dropdown
    loginToggle.addEventListener('click', function (e) {
        e.preventDefault();
        e.stopPropagation();
        loginDropdown.classList.toggle('show');
    });

    // Cerrar dropdown al hacer clic fuera
    document.addEventListener('click', function (e) {
        if (!loginDropdown.contains(e.target) && e.target !== loginToggle) {
            loginDropdown.classList.remove('show');
        }
    });

    // Prevenir que el dropdown se cierre al hacer clic dentro
    loginDropdown.addEventListener('click', function (e) {
        e.stopPropagation();
    });

    // Envío del formulario con AJAX
    loginForm.addEventListener('submit', async function (e) {
        e.preventDefault();

        loginError.classList.add('d-none');
        loginBtn.disabled = true;
        loginBtn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span> Entrando...';

        const formData = new FormData(loginForm);

        try {
            const response = await fetch('/Account/Login', {
                method: 'POST',
                body: formData,
                headers: {
                    'X-Requested-With': 'XMLHttpRequest'
                }
            });

            if (response.ok) {
                const contentType = response.headers.get('content-type');
                if (contentType && contentType.includes('text/html')) {
                    window.location.reload();
                    return;
                }

                const result = await response.json();
                if (result.success) {
                    window.location.href = result.redirectUrl || '/';
                } else {
                    showError(result.message || 'Error al iniciar sesión');
                }
            } else {
                const text = await response.text();
                const parser = new DOMParser();
                const doc = parser.parseFromString(text, 'text/html');
                const errorMsg = doc.querySelector('.validation-summary-errors, .text-danger, .alert-danger');

                if (errorMsg) {
                    showError(errorMsg.textContent.trim());
                } else {
                    showError('Credenciales inválidas. Verifica tus datos.');
                }
            }
        } catch (error) {
            console.error('Error:', error);
            showError('Error de conexión. Intenta nuevamente.');
        } finally {
            loginBtn.disabled = false;
            loginBtn.innerHTML = '<i class="bi bi-box-arrow-in-right me-1"></i> Entrar';
        }
    });

    function showError(message) {
        loginError.querySelector('small').textContent = message;
        loginError.classList.remove('d-none');
    }
});