// Admin Panel JavaScript

$(document).ready(function() {
    // Set active menu item based on current URL
    var currentPath = window.location.pathname;
    $('.menu-link').each(function() {
        var href = $(this).attr('href');
        if (href && currentPath.startsWith(href.replace('~', ''))) {
            $(this).addClass('active');
        }
    });

    // Confirm delete actions
    $('.btn-delete').on('click', function(e) {
        var result = confirm('Are you sure you want to delete this item? This action cannot be undone.');
        if (!result) {
            e.preventDefault();
        }
    });

    // Status change confirmation
    $('.status-change').on('change', function() {
        var newStatus = $(this).val();
        var itemName = $(this).data('item-name') || 'item';
        var result = confirm('Are you sure you want to change the status of ' + itemName + ' to ' + newStatus + '?');

        if (!result) {
            // Reset to original value
            var originalValue = $(this).data('original-value');
            $(this).val(originalValue);
        }
    });

    // Auto-hide alerts after 5 seconds
    setTimeout(function() {
        $('.alert').fadeOut('slow');
    }, 5000);

    // Initialize tooltips
    $('[data-toggle="tooltip"]').tooltip();

    // Table sorting
    $('.table-sortable th').on('click', function() {
        var table = $(this).parents('table').eq(0);
        var rows = table.find('tr:gt(0)').toArray().sort(comparer($(this).index()));
        this.asc = !this.asc;
        if (!this.asc) { rows = rows.reverse(); }
        for (var i = 0; i < rows.length; i++) { table.append(rows[i]); }
    });

    function comparer(index) {
        return function(a, b) {
            var valA = getCellValue(a, index), valB = getCellValue(b, index);
            return $.isNumeric(valA) && $.isNumeric(valB) ? valA - valB : valA.toString().localeCompare(valB);
        };
    }

    function getCellValue(row, index) {
        return $(row).children('td').eq(index).text();
    }

    // Bulk actions
    $('#select-all').on('change', function() {
        $('.item-checkbox').prop('checked', $(this).prop('checked'));
        updateBulkActions();
    });

    $('.item-checkbox').on('change', function() {
        updateBulkActions();
    });

    function updateBulkActions() {
        var checkedCount = $('.item-checkbox:checked').length;
        if (checkedCount > 0) {
            $('#bulk-actions').show();
            $('#selected-count').text(checkedCount + ' items selected');
        } else {
            $('#bulk-actions').hide();
        }
    }

    // Search functionality
    $('#search-input').on('keyup', function() {
        var value = $(this).val().toLowerCase();
        $('.table tbody tr').filter(function() {
            $(this).toggle($(this).text().toLowerCase().indexOf(value) > -1);
        });
    });

    // Pagination
    $('.page-link').on('click', function(e) {
        e.preventDefault();
        var page = $(this).data('page');
        $('#page-input').val(page);
        $('#pagination-form').submit();
    });

    // Form validation
    $('form').on('submit', function(e) {
        var isValid = true;
        $(this).find('input[required], select[required], textarea[required]').each(function() {
            if (!$(this).val()) {
                $(this).addClass('is-invalid');
                isValid = false;
            } else {
                $(this).removeClass('is-invalid');
            }
        });

        if (!isValid) {
            e.preventDefault();
            alert('Please fill in all required fields.');
        }
    });

    // Image preview
    $('.image-input').on('change', function() {
        var file = this.files[0];
        if (file) {
            var reader = new FileReader();
            reader.onload = function(e) {
                $('#image-preview').attr('src', e.target.result).show();
            };
            reader.readAsDataURL(file);
        }
    });
});
