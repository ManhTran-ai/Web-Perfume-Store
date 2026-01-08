// Toast Notification System
class ToastManager {
    constructor() {
        this.container = null;
        this.init();
    }

    init() {
        // Create toast container
        this.container = document.createElement('div');
        this.container.id = 'toast-container';
        this.container.className = 'toast-container position-fixed top-0 end-0 p-3';
        this.container.style.zIndex = '9999';
        document.body.appendChild(this.container);

        // Add CSS
        this.addStyles();
    }

    addStyles() {
        const style = document.createElement('style');
        style.textContent = `
            .toast-container {
                z-index: 9999;
            }

            .toast {
                background: white;
                border-radius: 8px;
                box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
                margin-bottom: 10px;
                min-width: 300px;
                max-width: 500px;
                opacity: 0;
                transform: translateX(100%);
                transition: all 0.3s ease;
                border-left: 4px solid;
            }

            .toast.show {
                opacity: 1;
                transform: translateX(0);
            }

            .toast.success {
                border-left-color: #198754;
            }

            .toast.error {
                border-left-color: #dc3545;
            }

            .toast.warning {
                border-left-color: #ffc107;
            }

            .toast.info {
                border-left-color: #0dcaf0;
            }

            .toast-header {
                display: flex;
                justify-content: space-between;
                align-items: center;
                padding: 12px 16px;
                border-bottom: 1px solid #dee2e6;
                background: #f8f9fa;
                border-radius: 8px 8px 0 0;
            }

            .toast-body {
                padding: 12px 16px;
            }

            .toast .btn-close {
                background: none;
                border: none;
                font-size: 1.2rem;
                opacity: 0.7;
                cursor: pointer;
            }

            .toast .btn-close:hover {
                opacity: 1;
            }
        `;
        document.head.appendChild(style);
    }

    show(message, type = 'info', duration = 5000) {
        const toast = document.createElement('div');
        toast.className = `toast ${type}`;

        const header = document.createElement('div');
        header.className = 'toast-header';

        const title = document.createElement('strong');
        title.className = 'me-auto';
        title.textContent = this.getTitle(type);

        const closeBtn = document.createElement('button');
        closeBtn.type = 'button';
        closeBtn.className = 'btn-close';
        closeBtn.innerHTML = '×';
        closeBtn.onclick = () => this.hide(toast);

        header.appendChild(title);
        header.appendChild(closeBtn);

        const body = document.createElement('div');
        body.className = 'toast-body';
        body.textContent = message;

        toast.appendChild(header);
        toast.appendChild(body);

        this.container.appendChild(toast);

        // Trigger animation
        setTimeout(() => toast.classList.add('show'), 10);

        // Auto hide
        if (duration > 0) {
            setTimeout(() => this.hide(toast), duration);
        }

        return toast;
    }

    hide(toast) {
        toast.classList.remove('show');
        setTimeout(() => {
            if (toast.parentNode) {
                toast.parentNode.removeChild(toast);
            }
        }, 300);
    }

    getTitle(type) {
        switch (type) {
            case 'success': return 'Success';
            case 'error': return 'Error';
            case 'warning': return 'Warning';
            case 'info':
            default: return 'Info';
        }
    }
}

// Global toast manager
const toastManager = new ToastManager();

// Global function for easy access
function showToast(message, type = 'info', duration = 5000) {
    return toastManager.show(message, type, duration);
}
