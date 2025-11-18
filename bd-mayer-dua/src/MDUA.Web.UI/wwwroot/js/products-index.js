$(document).ready(function () {

    // 1. Global Variables
    // We grab the token from the DOM
    let token = $('input[name="__RequestVerificationToken"]').val();
    let modalBody = $('#modal-variants-content');

    // Access the URLs we defined in the View
    const urls = window.productConfig.urls;

    // ============================================================
    //  HELPER FUNCTION: Load Values for Attributes
    // ============================================================
    function loadAttributeValues() {
        $('.variant-attr-select').each(function () {
            let $select = $(this);
            let attrId = $select.data('attribute-id');

            if (attrId) {
                $select.html('<option value="">Loading...</option>');

                $.get(urls.getAttributeValues, { attributeId: attrId }, function (data) {
                    let opts = '<option value="">-- Select Value --</option>';
                    if (data && data.length > 0) {
                        data.forEach(v => {
                            opts += `<option value="${v.id}">${v.value}</option>`;
                        });
                    } else {
                        opts = '<option value="">No values found</option>';
                    }
                    $select.html(opts);
                });
            }
        });
    }

    // ============================================================
    //  VIEW VARIANTS HANDLER
    // ============================================================
    $('.btn-view-variants').on('click', function () {
        var productId = $(this).data('product-id');
        var productName = $(this).closest('tr').find('td[data-product-name]').data('product-name');

        $('#modal-product-name').text(productName);
        $('#modal-variants-content').html('<div class="loading-spinner"></div>');

        $.get(urls.getVariantsPartial, { productId: productId }, function (data) {
            $('#modal-variants-content').html(data);
            loadAttributeValues();
        }).fail(function () {
            $('#modal-variants-content').html('<div class="error-message">Failed to load variants.</div>');
        });
    });

    // ============================================================
    //  ADD NEW VARIANT LOGIC
    // ============================================================

    // Update Hidden Input when Dropdown Changes
    modalBody.on('change', '.variant-attr-select', function () {
        let val = $(this).val();
        $(this).siblings('.final-value-id').val(val);
    });

    // Save New Variant
    modalBody.on('click', '#btn-save-new-variant', function () {
        let $form = $('#form-add-single-variant');
        let productName = $('#modal-product-name').text().trim();

        let parts = [productName];
        let allSelected = true;

        $('.variant-attr-select').each(function () {
            let val = $(this).val();
            let text = $(this).find('option:selected').text();

            if (!val) {
                allSelected = false;
                $(this).addClass('is-invalid');
            } else {
                $(this).removeClass('is-invalid');
                parts.push(text);
            }
        });

        if (!allSelected) {
            alert("Please select a value for all attributes.");
            return;
        }

        $('#new-variant-name').val(parts.join(' - '));

        let $btn = $(this);
        $btn.prop('disabled', true).text('Saving...');

        $.ajax({
            url: urls.addSingleVariant,
            type: 'POST',
            headers: { 'RequestVerificationToken': token },
            data: $form.serialize(),
            success: function (data) {
                if (data.success) {
                    let productId = $form.find('input[name="ProductId"]').val();
                    $.get(urls.getVariantsPartial, { productId: productId }, function (html) {
                        $('#modal-variants-content').html(html);
                        loadAttributeValues();
                    });
                } else {
                    alert('Error: ' + data.message);
                    $btn.prop('disabled', false).text('Save New Variant');
                }
            },
            error: function () {
                alert('Server Error');
                $btn.prop('disabled', false).text('Save New Variant');
            }
        });
    });

    // ============================================================
    //  EXISTING HANDLERS
    // ============================================================

    // Toggle Status
    $('.js-toggle-status').on('click', function () {
        let $button = $(this);
        let productId = $button.data('product-id');
        $button.prop('disabled', true);

        $.ajax({
            url: urls.toggleStatus,
            type: 'POST',
            headers: { 'RequestVerificationToken': token },
            data: { productId: productId },
            success: function (data) {
                if (data.success) {
                    $button.text(data.newIsActive ? "Active" : "Inactive");
                    $button.removeClass("status-active status-inactive");
                    $button.addClass(data.newIsActive ? "status-active" : "status-inactive");
                } else { alert(data.message); }
            },
            error: function () { alert('Error.'); },
            complete: function () { $button.prop('disabled', false); }
        });
    });

    // View Details
    $('.js-view-details').on('click', function () {
        let $button = $(this);
        let productId = $button.data('product-id');
        let productName = $button.closest('tr').find('td[data-product-name]').data('product-name');

        $('#productDetailsModalLabel').text("Details for " + productName);
        $('#modal-details-content').html('<div class="loading-spinner"></div>');

        $.get(urls.getProductDetailsPartial, { productId: productId }, function (data) {
            $('#modal-details-content').html(data);
        }).fail(function () {
            $('#modal-details-content').html('<div class="error-message">Failed.</div>');
        });
    });

    // Prepare Delete Product
    $('.js-delete-product').on('click', function () {
        let productId = $(this).data('product-id');
        let productName = $(this).data('product-name');
        $('#modal-delete-product-name').text(productName);
        $('#confirm-delete-button').data('product-id', productId);
    });

    // Checkbox Logic
    let deleteCheckbox = document.getElementById('confirm-delete-checkbox');
    if (deleteCheckbox) {
        deleteCheckbox.addEventListener('change', function () {
            document.getElementById('confirm-delete-button').disabled = !this.checked;
        });
    }
    $('#deleteProductModal').on('hidden.bs.modal', function () {
        if (deleteCheckbox) deleteCheckbox.checked = false;
        document.getElementById('confirm-delete-button').disabled = true;
    });

    // Execute Delete Product
    $('#confirm-delete-button').on('click', function () {
        let $button = $(this);
        let productId = $button.data('product-id');
        $button.prop('disabled', true).text('Deleting...');

        $.ajax({
            url: urls.deleteConfirmed,
            type: 'POST',
            headers: { 'RequestVerificationToken': token },
            data: { productId: productId },
            success: function (data) {
                if (data.success) {
                    $('#deleteProductModal').modal('hide');
                    $('tr[data-product-row-id="' + productId + '"]').fadeOut(500, function () { $(this).remove(); });
                } else { alert('Error: ' + data.message); }
            },
            error: function () { alert('Server error.'); },
            complete: function () { $button.prop('disabled', false).text('Delete Product'); }
        });
    });

    // Edit Product Modal
    $('.js-edit-product').on('click', function () {
        let $button = $(this);
        let productId = $button.data('product-id');
        let productName = $button.data('product-name');
        $('#editProductModalLabel').text("Edit: " + productName);
        $('#modal-edit-content').html('<div class="loading-spinner"></div>');

        $.get(urls.getEditPartial, { productId: productId }, function (data) {
            $('#modal-edit-content').html(data);
        }).fail(function () {
            $('#modal-edit-content').html('<div class="error-message">Failed to load.</div>');
        });
    });

    // ============================================================
    //  MANAGE VARIANTS (Inline Edit/Delete)
    // ============================================================

    // 1. CLICK "Delete" on the row -> OPENS MODAL
    modalBody.on('click', '.js-delete-variant', function () {
        let $button = $(this);
        let $row = $button.closest('tr');
        let variantId = $row.data('variant-id');

        let variantName = $row.find('td:first').text().trim();
        $('#modal-delete-variant-name').text(variantName);
        $('#confirm-delete-variant-button').data('variant-id', variantId);

        $('#deleteVariantModal').modal('show');
    });

    // 2. CLICK "Confirm Delete"
    $('#confirm-delete-variant-button').on('click', function () {
        let $button = $(this);
        let variantId = $button.data('variant-id');
        $button.prop('disabled', true).text('Deleting...');

        $.ajax({
            url: urls.deleteVariant,
            type: 'POST',
            headers: { 'RequestVerificationToken': token },
            data: { variantId: variantId },
            success: function (data) {
                if (data.success) {
                    $('#deleteVariantModal').modal('hide');
                    $('tr[data-variant-id="' + variantId + '"]').fadeOut(300, function () {
                        $(this).remove();
                        if ($('#modal-variants-content tbody tr').length === 0) {
                            $('#modal-variants-content').html('<div class="empty-state"><h3>No Variants Found</h3></div>');
                        }
                    });
                } else { alert('Error: ' + data.message); }
            },
            error: function () { alert('Server error.'); },
            complete: function () { $button.prop('disabled', false).text('Delete Variant'); }
        });
    });

    // 3. Manage Variants: Edit/Save
    modalBody.on('click', '.js-edit-variant', function () {
        let $button = $(this);
        let $row = $button.closest('tr');
        let variantId = $row.data('variant-id');
        let isEditMode = $button.text().trim() === 'Edit';

        if (isEditMode) {
            $row.find('td[data-col="price"] .display-value').hide();
            $row.find('td[data-col="price"] .edit-input').show();
            $button.text('Save').removeClass('btn-warning').addClass('btn-success');
        } else {
            let newPrice = $row.find('td[data-col="price"] .edit-input').val();
            $button.prop('disabled', true).text('Saving...');

            $.ajax({
                url: urls.updateVariantPrice,
                type: 'POST',
                headers: { 'RequestVerificationToken': token },
                data: { variantId: variantId, newPrice: newPrice },
                success: function (data) {
                    if (data.success) {
                        $row.find('td[data-col="price"] .display-value').text('Tk. ' + parseFloat(newPrice).toFixed(2));
                        $row.find('.display-value').show();
                        $row.find('.edit-input').hide();
                        $button.text('Edit').removeClass('btn-success').addClass('btn-warning');
                    } else { alert('Error: ' + data.message); }
                },
                error: function () { alert('Server error.'); },
                complete: function () { $button.prop('disabled', false); }
            });
        }
    });
});