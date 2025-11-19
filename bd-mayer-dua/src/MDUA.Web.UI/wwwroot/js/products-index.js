$(document).ready(function () {

    // 1. Global Variables
    let token = $('input[name="__RequestVerificationToken"]').val();
    let modalBody = $('#modal-variants-content');
    const urls = window.productConfig.urls;

    // ============================================================
    //  DYNAMIC ATTRIBUTE LOGIC (For "Add New Variant" section)
    // ============================================================

    // Helper: Prevent selecting the same attribute twice across rows
    function updateDropdownAvailability() {
        let selectedValues = [];

        // Gather currently selected attribute IDs
        $('.attr-name-select').each(function () {
            let v = $(this).val();
            if (v) selectedValues.push(v);
        });

        // Disable those IDs in other dropdowns
        $('.attr-name-select').each(function () {
            let myVal = $(this).val();
            $(this).find('option').each(function () {
                let optVal = $(this).val();
                // If option is selected elsewhere (and not by me), disable it
                if (optVal && selectedValues.includes(optVal) && optVal !== myVal) {
                    $(this).prop('disabled', true);
                } else {
                    $(this).prop('disabled', false);
                }
            });
        });
    }

    // Function to Add a New Attribute Row
    // ============================================================
    //  ADD NEW VARIANT LOGIC (Dynamic Rows)
    // ============================================================
    // ============================================================
    //  HELPER: Update Main Grid Price without Reload
    // ============================================================
    function updateMainGridPrice(productId) {
        // Find the specific row in the background table
        let $row = $(`tr[data-product-row-id="${productId}"]`);
        let $cell = $row.find('.price-cell'); // The td we added the class to

        if ($row.length === 0) return;

        // Fetch new calculated price
        $.get(urls.getUpdatedPrice, { productId: productId }, function (data) {
            if (data.success) {
                let html = '';
                if (data.hasDiscount) {
                    html = `<span class="original-price">${data.originalPrice}</span><br/>
                            <span class="discounted-price">${data.sellingPrice}</span>`;
                } else {
                    html = `<span>${data.originalPrice}</span>`;
                }
                // Update the HTML of the cell
                $cell.html(html);
            }
        });
    }
    // 1. Function to Add a Row
    function addAttributeRow() {
        let index = $('#dynamic-attributes-container .dynamic-attr-row').length;

        // Get options from the hidden template
        let optionsHtml = $('#attribute-template').html();

        // DEBUG: Check if attributes are loaded
        if (!optionsHtml || optionsHtml.trim() === "") {
            console.error("Attribute Template is empty. Ensure Facade loads Attributes.");
            optionsHtml = '<option value="">Error: No Attributes Found</option>';
        }

        let rowHtml = `
            <div class="dynamic-attr-row mb-2 d-flex gap-2 align-items-center">
                <select class="form-control form-control-sm attr-name-select" style="width: 45%;">
                    ${optionsHtml}
                </select>
                
                <select class="form-control form-control-sm attr-value-select" style="width: 45%;" disabled>
                    <option value="">Select Value</option>
                </select>
                
                <input type="hidden" name="AttributeValueIds[${index}]" class="final-value-id" />

                <button type="button" class="btn btn-sm btn-outline-danger remove-attr-row" style="width: 10%;">&times;</button>
            </div>
        `;

        $('#dynamic-attributes-container').append(rowHtml);
        updateDropdownAvailability();
    }

    // 2. Helper: Prevent selecting same attribute twice
    function updateDropdownAvailability() {
        let selectedValues = [];
        $('.attr-name-select').each(function () {
            let v = $(this).val();
            if (v) selectedValues.push(v);
        });

        $('.attr-name-select').each(function () {
            let myVal = $(this).val();
            $(this).find('option').each(function () {
                let optVal = $(this).val();
                if (optVal && selectedValues.includes(optVal) && optVal !== myVal) {
                    $(this).prop('disabled', true);
                } else {
                    $(this).prop('disabled', false);
                }
            });
        });
    }

    // 3. HANDLER: Click "+ Add Attribute"
    // ✅ Using $(document) ensures we catch the click even if modal reloads
    $(document).on('click', '#btn-add-dynamic-attr', function () {
        console.log("Add Attribute Button Clicked"); // Check Console (F12) if this prints
        addAttributeRow();
    });

    // 4. HANDLER: Remove Row
    $(document).on('click', '.remove-attr-row', function () {
        $(this).closest('.dynamic-attr-row').remove();

        // Re-index inputs
        $('#dynamic-attributes-container .dynamic-attr-row').each(function (idx) {
            $(this).find('.final-value-id').attr('name', `AttributeValueIds[${idx}]`);
        });
        updateDropdownAvailability();
    });

    // 5. HANDLER: Attribute Name Changed -> Load Values
    $(document).on('change', '.attr-name-select', function () {
        let $select = $(this);
        let $row = $select.closest('.dynamic-attr-row');
        let $valSelect = $row.find('.attr-value-select');
        let attrId = $select.val();

        updateDropdownAvailability();

        if (attrId) {
            $valSelect.html('<option>Loading...</option>').prop('disabled', true);

            $.get(urls.getAttributeValues, { attributeId: attrId }, function (data) {
                let html = '<option value="">Select Value</option>';
                if (data && data.length > 0) {
                    data.forEach(v => {
                        html += `<option value="${v.id}">${v.value}</option>`;
                    });
                } else {
                    html = '<option value="">No values found</option>';
                }
                $valSelect.html(html).prop('disabled', false);
            });
        } else {
            $valSelect.html('<option value="">Select Value</option>').prop('disabled', true);
        }
    });

    // 6. HANDLER: Attribute Value Changed -> Update Hidden Input
    $(document).on('change', '.attr-value-select', function () {
        let val = $(this).val();
        $(this).siblings('.final-value-id').val(val);
    });

    // 7. HANDLER: Save New Variant
    // HANDLER: Save New Variant
    modalBody.on('click', '#btn-save-new-variant', function () {
        let $form = $('#form-add-single-variant');
        let productName = $('#modal-product-name').text().trim();

        let parts = [productName];
        let isValid = true;

        // 1. Validate: Must have at least one attribute row
        if ($('.dynamic-attr-row').length === 0) {
            alert("Please add at least one attribute.");
            return;
        }

        // 2. Validate: All dropdowns must be selected
        $('.attr-value-select').each(function () {
            if (!$(this).val()) {
                isValid = false;
                $(this).addClass('is-invalid');
            } else {
                $(this).removeClass('is-invalid');
                parts.push($(this).find('option:selected').text());
            }
        });

        if (!isValid) {
            alert("Please select values for all attributes.");
            return;
        }

        // 3. Construct the proposed name
        let proposedName = parts.join(' - ');

        // 4. ✅ DUPLICATE CHECK: Loop through existing table rows
        // 4. DUPLICATE CHECK: Loop through existing table rows
        let isDuplicate = false;
        $('.variants-table-modal tbody tr').each(function () {
            let existingName = $(this).find('td:first').text().trim();
            if (existingName === proposedName) {
                isDuplicate = true;
                return false; // Break loop
            }
        });

        if (isDuplicate) {
            // ✅ FIX: Show Beautiful Modal instead of Alert
            $('#duplicate-variant-name').text(proposedName);
            $('#duplicateVariantModal').modal('show');
            return; // STOP here.
        }

        // 5. Set Name and Submit
        $('#new-variant-name').val(proposedName);

        let $btn = $(this);
        $btn.prop('disabled', true).text('Saving...');

        $.ajax({
            url: urls.addSingleVariant,
            type: 'POST',
            headers: { 'RequestVerificationToken': token },
            data: $form.serialize(),
            success: function (data) {
                if (data.success) {
                    // Reload Modal Content
                    let productId = $form.find('input[name="ProductId"]').val();
                    $.get(urls.getVariantsPartial, { productId: productId }, function (html) {
                        $('#modal-variants-content').html(html);
                        addAttributeRow(); // Start fresh
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
    //  MAIN PAGE HANDLERS
    // ============================================================

    // 1. View Variants Button (Opens the Modal)
    $('.btn-view-variants').on('click', function () {
        var productId = $(this).data('product-id');
        var productName = $(this).closest('tr').find('td[data-product-name]').data('product-name');

        $('#modal-product-name').text(productName);
        $('#modal-variants-content').html('<div class="loading-spinner"></div>');

        $.get(urls.getVariantsPartial, { productId: productId }, function (data) {
            $('#modal-variants-content').html(data);

            // ✅ Auto-add one empty row so the form is ready to use
            addAttributeRow();

        }).fail(function () {
            $('#modal-variants-content').html('<div class="error-message">Failed to load variants.</div>');
        });
    });

    // 2. Toggle Status
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

    // 3. View Details
    $('.js-view-details').on('click', function () {
        let $button = $(this);
        let productId = $button.data('product-id');
        let productName = $button.closest('tr').find('td[data-product-name]').data('product-name');

        $('#productDetailsModalLabel').text("Details for " + productName);
        $('#modal-details-content').html('<div class="loading-spinner"></div>');

        $.get(urls.getProductDetailsPartial, { productId: productId }, function (data) {
            $('#modal-details-content').html(data);
        }).fail(function () {
            $('#modal-details-content').html('<div class="error-message">Failed to load details.</div>');
        });
    });

    // 4. Edit Product (Main)
    $('.js-edit-product').on('click', function () {
        let $button = $(this);
        let productId = $button.data('product-id');
        let productName = $button.data('product-name');
        $('#editProductModalLabel').text("Edit: " + productName);
        $('#modal-edit-content').html('<div class="loading-spinner"></div>');

        $.get(urls.getEditPartial, { productId: productId }, function (data) {
            $('#modal-edit-content').html(data);
        }).fail(function () {
            $('#modal-edit-content').html('<div class="error-message">Failed to load form.</div>');
        });
    });

    // 5. Delete Product (Preparation)
    $('.js-delete-product').on('click', function () {
        let productId = $(this).data('product-id');
        let productName = $(this).data('product-name');
        $('#modal-delete-product-name').text(productName);
        $('#confirm-delete-button').data('product-id', productId);
    });

    // Checkbox Logic for Delete
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


    // ============================================================
    //  MANAGE VARIANTS LIST (Inline Edit & Delete)
    // ============================================================

    // 1. Delete Variant (Opens Confirm Modal)
    modalBody.on('click', '.js-delete-variant', function () {
        let $button = $(this);
        let $row = $button.closest('tr');
        let variantId = $row.data('variant-id');
        let variantName = $row.find('td:first').text().trim();

        $('#modal-delete-variant-name').text(variantName);
        $('#confirm-delete-variant-button').data('variant-id', variantId);

        $('#deleteVariantModal').modal('show');
    });

    // 2. Execute Delete Variant
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
                            $('#modal-variants-content').html('<div class="empty-state"><p>No variants found.</p></div>');
                        }
                    });
                } else { alert('Error: ' + data.message); }
            },
            error: function () { alert('Server error.'); },
            complete: function () { $button.prop('disabled', false).text('Delete Variant'); }
        });
    });

    // 3. Inline Edit Variant
    // 3. Manage Variants: Edit/Save (Updated for SKU)
    modalBody.on('click', '.js-edit-variant', function () {
        let $button = $(this);
        let $row = $button.closest('tr');
        let variantId = $row.data('variant-id');

        // Use .trim() to ensure correct comparison
        let isEditMode = $button.hasClass('text-warning');
        if (isEditMode) {
            // --- ENTER EDIT MODE ---

            // 1. Toggle Price: Hide Text, Show Input
            $row.find('td[data-col="price"] .display-value').hide();
            $row.find('td[data-col="price"] .edit-input').show();

            // 2. Toggle SKU: Hide Text, Show Input (NEW)
            $row.find('td[data-col="sku"] .display-value').hide();
            $row.find('td[data-col="sku"] .edit-input').show();

            // 3. Show the [+ Add Attr] button in the name column
            let nameCell = $row.find('td:first');
            if (nameCell.find('.btn-add-attr-inline').length === 0) {
                nameCell.append(`
                    <button type="button" class="btn btn-xs btn-outline-primary ms-2 btn-add-attr-inline" 
                            title="Add missing attribute" style="padding: 0px 4px; font-size: 10px;">
                        + Add Attr
                    </button>
                `);
            }
            nameCell.find('.btn-add-attr-inline').show();

            // 4. Change button to Save
            $button.removeClass('text-warning').addClass('text-success')
                .html('<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><polyline points="20 6 9 17 4 12"></polyline></svg>');
        } else {
            // --- SAVE MODE ---

            // Get values from inputs
            let newPrice = $row.find('td[data-col="price"] .edit-input').val();
            let newSku = $row.find('td[data-col="sku"] .edit-input').val(); // (NEW)

            $button.prop('disabled', true).text('Saving...');

            $.ajax({
                url: urls.updateVariantPrice, // Ensure this URL maps to 'UpdateVariantData' in your View Config
                type: 'POST',
                headers: { 'RequestVerificationToken': token },
                data: {
                    variantId: variantId,
                    newPrice: newPrice,
                    newSku: newSku // Send SKU
                },
                success: function (data) {
                    if (data.success) {
                        let priceHtml = `<span class="fw-bold text-dark">Tk. ${parseFloat(newPrice).toFixed(2)}</span>`;
                        // 1. Update Price UI
                        $row.find('td[data-col="price"] .display-value').text('Tk. ' + parseFloat(newPrice).toFixed(2));

                        // 2. Update SKU UI (NEW)
                        $row.find('td[data-col="sku"] .display-value').text(newSku ? newSku : "N/A");

                        // 3. Switch inputs back to hidden
                        $row.find('.display-value').show();
                        $row.find('.edit-input').hide();
                        $row.find('.btn-add-attr-inline').hide(); // Hide add attr button

                        // 4. Change button back to Edit
                        $button.removeClass('text-success').addClass('text-warning')
                            .html('<svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2"><path d="M11 4H4a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h14a2 2 0 0 0 2-2v-7"></path><path d="M18.5 2.5a2.121 2.121 0 0 1 3 3L12 15l-4 1 1-4 9.5-9.5z"></path></svg>');                    } else {
                        alert('Error: ' + data.message);
                    }
                },
                error: function () { alert('Server error.'); },
                complete: function () { $button.prop('disabled', false); }
            });
        }
    });

    // 4. HANDLER: Click "[+ Add Attr]" (Opens Small Modal)
    modalBody.on('click', '.btn-add-attr-inline', function(e) {
        e.stopPropagation(); // Prevent row clicks
        
        let $row = $(this).closest('tr');
        let variantId = $row.data('variant-id');
        let productId = $('input[name="ProductId"]').val(); // Get from hidden field in main modal

        // Reset & Open Modal
        $('#target-variant-id').val(variantId);
        $('#missing-attr-select').html('<option>Loading...</option>');
        $('#missing-value-select').html('<option>Select Value</option>').prop('disabled', true);
        $('#addVariantAttributeModal').modal('show');

        // Load Missing Attributes
        $.get(urls.getMissingAttributes, { productId: productId, variantId: variantId }, function(data) {
            let html = '<option value="">Select Attribute</option>';
            if(data && data.length > 0) {
                data.forEach(a => { html += `<option value="${a.id}">${a.name}</option>`; });
            } else {
                html = '<option value="">No missing attributes!</option>';
            }
            $('#missing-attr-select').html(html);
        });
    });

    // 5. HANDLER: Missing Attribute Selected -> Load Values
    $('#missing-attr-select').on('change', function() {
        let attrId = $(this).val();
        let $valSelect = $('#missing-value-select');
        
        if(attrId) {
            $valSelect.html('<option>Loading...</option>').prop('disabled', true);
            $.get(urls.getAttributeValues, { attributeId: attrId }, function(data) {
                let html = '<option value="">Select Value</option>';
                data.forEach(v => { html += `<option value="${v.id}">${v.value}</option>`; });
                $valSelect.html(html).prop('disabled', false);
            });
        }
    });

    // 6. HANDLER: Confirm Add Attribute
    $('#btn-confirm-add-attr').on('click', function() {
        let variantId = $('#target-variant-id').val();
        let valId = $('#missing-value-select').val();
        let productId = $('input[name="ProductId"]').val();

        if(!valId) { alert("Select a value"); return; }

        $(this).prop('disabled', true).text('Adding...');

        $.ajax({
            url: urls.addAttributeToVariant,
            type: 'POST',
            headers: { 'RequestVerificationToken': token },
            data: { variantId: variantId, attributeValueId: valId },
            success: function(data) {
                if(data.success) {
                    // Close small modal
                    $('#addVariantAttributeModal').modal('hide');
                    // Reload the big list to show updated name
                    $.get(urls.getVariantsPartial, { productId: productId }, function (html) {
                        $('#modal-variants-content').html(html);
                        addAttributeRow(); // Re-init dynamic rows
                    });
                } else {
                    alert("Failed");
                }
            },
            complete: function() { 
                $('#btn-confirm-add-attr').prop('disabled', false).text('Add & Update'); 
            }
        });
    });

    // ============================================================
    //  DISCOUNT MODAL LOGIC
    // ============================================================

    // 1. Open Modal
    $('.js-manage-discounts').on('click', function () {
        let productId = $(this).data('product-id');
        let name = $(this).data('product-name');
        $('#modal-discount-product-name').text(name);
        $('#modal-discounts-content').html('<div class="loading-spinner"></div>');

        $.get(urls.getDiscounts, { productId: productId }, function (html) {
            $('#modal-discounts-content').html(html);
        });
    });

    // 2. Add Discount
    $(document).on('click', '#btn-save-discount', function () {
        let $form = $('#form-add-discount');
        let productId = $form.find('input[name="ProductId"]').val();
        let $btn = $(this);

        $btn.prop('disabled', true).text('Adding...');

        $.ajax({
            url: urls.addDiscount,
            type: 'POST',
            headers: { 'RequestVerificationToken': token },
            data: $form.serialize(),
            success: function (data) {
                if (data.success) {
                    // Reload
                    $.get(urls.getDiscounts, { productId: productId }, function (html) {
                        $('#modal-discounts-content').html(html);
                    });
                    updateMainGridPrice(productId);
                } else {
                    alert("Error: " + data.message);
                    $btn.prop('disabled', false).text('Add Discount');
                }
            },
            error: function () { alert("Server Error"); $btn.prop('disabled', false).text('Add Discount'); }
        });
    });

    // 3. Delete Discount
    // 3. Delete Discount (Opens Modal)
    $(document).on('click', '.js-delete-discount', function () {
        let $row = $(this).closest('tr');
        let id = $row.data('id');

        // Get some info to show in the modal (e.g. "Flat - 50.00")
        let type = $row.find('td[data-col="type"] .display-value').text();
        let value = $row.find('td[data-col="value"] .display-value').text();
        let infoText = `${type} : ${value}`;

        // Update Modal Text
        $('#modal-delete-discount-info').text(infoText);

        // Attach ID to the confirm button
        $('#confirm-delete-discount-button').data('discount-id', id);

        // Show Modal (Stacked on top)
        $('#deleteDiscountModal').modal('show');
    });

    // 3b. Confirm Delete Discount (Executes AJAX)
    $(document).on('click', '#confirm-delete-discount-button', function () {
        let $btn = $(this);
        let discountId = $btn.data('discount-id');

        $btn.prop('disabled', true).text('Deleting...');

        $.ajax({
            url: urls.deleteDiscount,
            type: 'POST',
            headers: { 'RequestVerificationToken': token },
            data: { id: discountId },
            success: function (data) {
                if (data.success) {
                    // Close the confirmation modal
                    $('#deleteDiscountModal').modal('hide');

                    // Find the row inside the Discount Modal and remove it
                    let $row = $(`.discounts-table tr[data-id="${discountId}"]`);
                    $row.fadeOut(300, function () {
                        $(this).remove();
                        // Show "No discounts" if table is empty
                        if ($('.discounts-table tbody tr').length === 0) {
                            $('.discounts-table tbody').html('<tr><td colspan="7" class="text-center text-muted">No discounts found.</td></tr>');
                        }
                    });
                    let productId = $('#form-add-discount input[name="ProductId"]').val();
                    updateMainGridPrice(productId);
                } else {
                    alert("Error: " + data.message);
                }
            },
            error: function () { alert("Server Error"); },
            complete: function () {
                $btn.prop('disabled', false).text('Delete Discount');
            }
        });
    });

    // 4. Inline Edit Discount
    $(document).on('click', '.js-edit-discount', function () {
        let $btn = $(this);
        let $row = $btn.closest('tr');
        let isEdit = $btn.text().trim() === 'Edit';

        if (isEdit) {
            // Enter Edit Mode
            $row.find('.display-value').hide();
            $row.find('.edit-input').show();
            $btn.text('Save').removeClass('btn-warning').addClass('btn-success');
        } else {
            // Save Mode
            let id = $row.data('id');
            let productId = $('#form-add-discount input[name="ProductId"]').val();

            // Gather data manually
            let discount = {
                Id: id,
                ProductId: productId,
                DiscountType: $row.find('td[data-col="type"] select').val(),
                DiscountValue: $row.find('td[data-col="value"] input').val(),
                MinQuantity: $row.find('td[data-col="qty"] input').val(),
                EffectiveFrom: $row.find('td[data-col="from"] input').val(),
                EffectiveTo: $row.find('td[data-col="to"] input').val(),
                IsActive: $row.find('td[data-col="active"] select').val(),

                // Fake values required by model binder (dates/strings)
                CreatedBy: "",
                CreatedAt: new Date().toISOString()
            };

            $btn.prop('disabled', true).text('Saving...');

            $.ajax({
                url: urls.updateDiscount,
                type: 'POST',
                headers: { 'RequestVerificationToken': token },
                data: discount,
                success: function (data) {
                    if (data.success) {
                        // Reload to refresh display values (dates/badges) cleanly
                        $.get(urls.getDiscounts, { productId: productId }, function (html) {
                            $('#modal-discounts-content').html(html);
                        });
                        updateMainGridPrice(productId);
                    } else { alert("Error saving"); $btn.prop('disabled', false).text('Save'); }
                },
                error: function () { alert("Server Error"); $btn.prop('disabled', false).text('Save'); }
            });
        }
    });

}); // End of Document Ready