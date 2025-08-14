// Consulting Group SRL - Gestione Studio
// Site JavaScript Functions

$(document).ready(function() {
    console.log('Consulting Group - Gestione Studio loaded successfully!');
    
    // Initialize sidebar toggle
    initializeSidebar();
    
    // Initialize form validation enhancements
    initializeFormValidation();
    
    // Initialize confirmation dialogs
    initializeConfirmationDialogs();
    
    // Initialize tooltips
    initializeTooltips();
    
    // Auto-hide alerts after 5 seconds
    autoHideAlerts();
});

// Sidebar Toggle Functionality
function initializeSidebar() {
    $('#sidebarCollapse').on('click', function() {
        $('#sidebar').toggleClass('active');
        $('#content').toggleClass('active');
        
        // Store sidebar state in localStorage
        const isActive = $('#sidebar').hasClass('active');
        localStorage.setItem('sidebarCollapsed', isActive);
    });
    
    // Restore sidebar state from localStorage
    const sidebarCollapsed = localStorage.getItem('sidebarCollapsed');
    if (sidebarCollapsed === 'true') {
        $('#sidebar').addClass('active');
        $('#content').addClass('active');
    }
    
    // Highlight active menu item
    highlightActiveMenuItem();
}

// Highlight Active Menu Item
function highlightActiveMenuItem() {
    const currentPath = window.location.pathname.toLowerCase();
    
    $('.sidebar .nav-link').each(function() {
        const linkPath = $(this).attr('href');
        if (linkPath && currentPath.includes(linkPath.toLowerCase())) {
            $(this).addClass('active');
        }
    });
}

// Form Validation Enhancements
function initializeFormValidation() {
    // Add custom validation styles
    $('form').on('submit', function() {
        const form = this;
        if (form.checkValidity() === false) {
            event.preventDefault();
            event.stopPropagation();
        }
        $(form).addClass('was-validated');
    });
    
    // Real-time validation feedback
    $('.form-control').on('blur', function() {
        if (this.checkValidity()) {
            $(this).removeClass('is-invalid').addClass('is-valid');
        } else {
            $(this).removeClass('is-valid').addClass('is-invalid');
        }
    });
    
    // Password confirmation validation
    $('input[name="ConfirmPassword"]').on('keyup', function() {
        const password = $('input[name="Password"]').val();
        const confirmPassword = $(this).val();
        
        if (password !== confirmPassword) {
            this.setCustomValidity('Le password non corrispondono');
            $(this).removeClass('is-valid').addClass('is-invalid');
        } else {
            this.setCustomValidity('');
            $(this).removeClass('is-invalid').addClass('is-valid');
        }
    });
}

// Confirmation Dialogs
function initializeConfirmationDialogs() {
    // Delete confirmation
    $('[data-confirm-delete]').on('click', function(e) {
        e.preventDefault();
        
        const message = $(this).data('confirm-delete') || 'Sei sicuro di voler eliminare questo elemento?';
        const title = 'Conferma Eliminazione';
        
        showConfirmationModal(title, message, 'danger', () => {
            // If confirmed, submit the form or follow the link
            if ($(this).is('button') && $(this).closest('form').length) {
                $(this).closest('form').submit();
            } else if ($(this).is('a')) {
                window.location.href = $(this).attr('href');
            }
        });
    });
    
    // General confirmation
    $('[data-confirm]').on('click', function(e) {
        e.preventDefault();
        
        const message = $(this).data('confirm') || 'Sei sicuro di voler continuare?';
        const title = 'Conferma Azione';
        
        showConfirmationModal(title, message, 'primary', () => {
            if ($(this).is('button') && $(this).closest('form').length) {
                $(this).closest('form').submit();
            } else if ($(this).is('a')) {
                window.location.href = $(this).attr('href');
            }
        });
    });
}

// Show Confirmation Modal
function showConfirmationModal(title, message, type, onConfirm) {
    const modalId = 'confirmationModal';
    
    // Remove existing modal
    $(`#${modalId}`).remove();
    
    const typeClass = type === 'danger' ? 'btn-danger' : 'btn-primary';
    const iconClass = type === 'danger' ? 'fa-exclamation-triangle' : 'fa-question-circle';
    
    const modalHtml = `
        <div class="modal fade" id="${modalId}" tabindex="-1" aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title">
                            <i class="fas ${iconClass} me-2"></i>
                            ${title}
                        </h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <p>${message}</p>
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">
                            <i class="fas fa-times me-1"></i> Annulla
                        </button>
                        <button type="button" class="btn ${typeClass}" id="confirmButton">
                            <i class="fas fa-check me-1"></i> Conferma
                        </button>
                    </div>
                </div>
            </div>
        </div>
    `;
    
    $('body').append(modalHtml);
    
    const modal = new bootstrap.Modal(document.getElementById(modalId));
    modal.show();
    
    $(`#${modalId} #confirmButton`).on('click', function() {
        modal.hide();
        onConfirm();
    });
    
    // Clean up after modal is hidden
    $(`#${modalId}`).on('hidden.bs.modal', function() {
        $(this).remove();
    });
}

// Initialize Tooltips
function initializeTooltips() {
    const tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function(tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });
}

// Auto-hide Alerts
function autoHideAlerts() {
    $('.alert:not(.alert-permanent)').each(function() {
        const alert = $(this);
        setTimeout(function() {
            alert.fadeOut('slow', function() {
                alert.remove();
            });
        }, 5000);
    });
}

// Show Loading State
function showLoading(element) {
    const $element = $(element);
    const originalText = $element.text();
    
    $element.data('original-text', originalText);
    $element.prop('disabled', true);
    $element.html('<span class="loading me-2"></span>Caricamento...');
}

// Hide Loading State
function hideLoading(element) {
    const $element = $(element);
    const originalText = $element.data('original-text');
    
    $element.prop('disabled', false);
    $element.html(originalText);
}

// Show Success Message
function showSuccessMessage(message) {
    showMessage(message, 'success');
}

// Show Error Message
function showErrorMessage(message) {
    showMessage(message, 'danger');
}

// Show Info Message
function showInfoMessage(message) {
    showMessage(message, 'info');
}

// Show Warning Message
function showWarningMessage(message) {
    showMessage(message, 'warning');
}

// Generic Show Message Function
function showMessage(message, type) {
    const iconMap = {
        'success': 'fa-check-circle',
        'danger': 'fa-exclamation-circle',
        'info': 'fa-info-circle',
        'warning': 'fa-exclamation-triangle'
    };
    
    const icon = iconMap[type] || 'fa-info-circle';
    
    const alertHtml = `
        <div class="alert alert-${type} alert-dismissible fade show" role="alert">
            <i class="fas ${icon} me-2"></i>
            ${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `;
    
    // Find the first container to append the alert
    const container = $('.container-fluid').first();
    if (container.length) {
        container.prepend(alertHtml);
        
        // Auto-hide after 5 seconds
        setTimeout(function() {
            container.find('.alert').first().fadeOut('slow', function() {
                $(this).remove();
            });
        }, 5000);
    }
}

// AJAX Error Handler
$(document).ajaxError(function(event, xhr, settings, thrownError) {
    console.error('AJAX Error:', thrownError);
    
    let message = 'Si è verificato un errore durante il caricamento.';
    
    if (xhr.status === 404) {
        message = 'Risorsa non trovata.';
    } else if (xhr.status === 500) {
        message = 'Errore interno del server.';
    } else if (xhr.status === 403) {
        message = 'Accesso negato.';
    }
    
    showErrorMessage(message);
});

// Global AJAX Setup
$.ajaxSetup({
    beforeSend: function(xhr, settings) {
        // Add anti-forgery token to AJAX requests
        const token = $('input[name="__RequestVerificationToken"]').val();
        if (token) {
            xhr.setRequestHeader('RequestVerificationToken', token);
        }
    }
});

// Export functions for global use
window.ConsultingGroup = {
    showLoading: showLoading,
    hideLoading: hideLoading,
    showSuccessMessage: showSuccessMessage,
    showErrorMessage: showErrorMessage,
    showInfoMessage: showInfoMessage,
    showWarningMessage: showWarningMessage,
    showConfirmationModal: showConfirmationModal
};