$(document).ready(function () {

    let attrIndex = $("#addProductModal #attributes-container .attribute-row").length;

    function cartesian(arr) {
        return arr.reduce((a, b) => a.flatMap(d => b.map(e => d.concat([e]))), [[]]);
    }

    // ➕ Add new attribute row
    $("#addProductModal").on("click", "#add-attribute", function () {
        let firstOptions = $("#addProductModal #attributes-container .attribute-row:first-child select").html();
        let row = `
            <div class="attribute-row mb-2" data-attr-index="${attrIndex}">
                <select name="Attributes[${attrIndex}].AttributeId" 
                        class="attribute-select form-control-modal">
                    ${firstOptions}
                </select>

                <div class="attribute-values-container mt-2"></div>

                <button type="button" 
                    class="btn btn-sm btn-outline-danger remove-attribute mt-1">x</button>
            </div>
        `;
        $("#addProductModal #attributes-container").append(row);
        attrIndex++;
    });

    // ❌ Remove attribute row
    $("#addProductModal").on("click", ".remove-attribute", function () {
        $(this).closest(".attribute-row").remove();
        generateVariants();
    });

    // 🔄 Load attribute values
    $("#addProductModal").on("change", ".attribute-select", function () {
        let row = $(this).closest(".attribute-row");
        let container = row.find(".attribute-values-container");
        container.html("");

        let attributeId = $(this).val();
        if (!attributeId) return;

        $.get("/product/get-attribute-values", { attributeId: attributeId }, function (data) {
            data.forEach(v => {
                container.append(`
                    <label class="d-block">
                        <input type="checkbox" 
                               class="attribute-value-checkbox" 
                               value="${v.id}" 
                               data-attrname="${v.value}" />
                        ${v.value}
                    </label>
                `);
            });
        });
    });

    // 🔁 Refresh variants when clicking attribute value checkbox
    $("#addProductModal").on("change", ".attribute-value-checkbox", generateVariants);

    // 🚀 Generate product variants
    function generateVariants() {
        let variantsContainer = $("#addProductModal #variants-container");
        variantsContainer.html("");

        let selectedPerAttribute = [];
        $("#addProductModal .attribute-row").each(function () {
            let checked = $(this).find(".attribute-value-checkbox:checked");
            if (checked.length > 0) {
                let values = [];
                checked.each(function () {
                    values.push({
                        id: $(this).val(),
                        label: $(this).data("attrname")
                    });
                });
                selectedPerAttribute.push(values);
            }
        });

        if (selectedPerAttribute.length === 0) return;

        let combos = cartesian(selectedPerAttribute);

        let productName = $("input[name='ProductName']").val() || "";
        let basePrice = $("#addProductModal input[name='BasePrice']").val() || 0;

        combos.forEach((combo, idx) => {

            let label = productName + " - " + combo.map(v => v.label).join(" - ");

            let hiddenInputs = combo.map((v, i) =>
                `<input type="hidden" 
                        name="Variants[${idx}].AttributeValueIds[${i}]"
                        value="${v.id}" />`
            ).join("");

            variantsContainer.append(`
    <div class="variant-row mb-2">
        ${hiddenInputs}
        <span class="d-inline-block w-50">${label}</span>

        <input type="number" 
               name="Variants[${idx}].Price"
               class="form-control-modal d-inline-block w-25"
               value="${basePrice}"
               required />
    </div>
`);

        });
    }

    // 🔁 Refresh variants when product name is typed
    $("#addProductModal input[name='ProductName']").on("input", generateVariants);

});
