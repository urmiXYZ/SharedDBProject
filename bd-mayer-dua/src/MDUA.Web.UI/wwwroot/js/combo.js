let currentSlide = 0;

function showSlide(i) {
    const slides = document.querySelectorAll(".slide");
    if (slides.length === 0) return;
    currentSlide = (i + slides.length) % slides.length;
    slides.forEach((s, idx) => s.classList.toggle("active", idx === currentSlide));
}

function changeSlide(dir) {
    showSlide(currentSlide + dir);
}

document.addEventListener("DOMContentLoaded", () => {

    // global data provided by Razor
    const globalVariantPrices = (typeof variantPrices !== "undefined") ? variantPrices : {};
    let currentPricePerCombo = (typeof pricePerCombo !== "undefined") ? parseFloat(pricePerCombo) : 0;
    const delivery = (typeof deliveryCharges !== "undefined") ? deliveryCharges : { dhaka: 0, outside: 0 };

    const slides = document.querySelectorAll(".slide");
    if (slides.length) showSlide(0);

    const qty = document.getElementById("quantity");
    const summaryQty = document.getElementById("summary-qty");
    const summarySize = document.getElementById("summary-size");
    const summaryTotal = document.getElementById("summary-total");
    const receiptTotal = document.getElementById("receipt-total");
    const sizeSelect = document.getElementById("size-select");
    const deliverySelect = document.getElementById("delivery-select");
    const decrease = document.getElementById("decrease");
    const increase = document.getElementById("increase");

    function getNumeric(value) {
        const n = parseFloat(value);
        return Number.isFinite(n) ? n : 0;
    }

    function updateSummary() {
        const q = Math.max(1, parseInt(qty.value) || 1);
        const size = sizeSelect?.value || "-";
        const deliveryKey = deliverySelect?.value || "dhaka";

        if (globalVariantPrices && globalVariantPrices[size] != null) {
            currentPricePerCombo = getNumeric(globalVariantPrices[size]);
        }

        const subtotal = q * getNumeric(currentPricePerCombo);
        const total = subtotal + (getNumeric(delivery[deliveryKey]) || 0);

        summaryQty.textContent = q;
        summarySize.textContent = size;
        summaryTotal.textContent = `Tk. ${Math.round(total).toLocaleString()}`;
        receiptTotal.textContent = `Tk. ${Math.round(total).toLocaleString()}`;
    }

    increase?.addEventListener("click", () => {
        qty.value = (parseInt(qty.value) || 1) + 1;
        updateSummary();
    });

    decrease?.addEventListener("click", () => {
        if ((parseInt(qty.value) || 1) > 1) {
            qty.value = (parseInt(qty.value) || 1) - 1;
        }
        updateSummary();
    });

    sizeSelect?.addEventListener("change", updateSummary);
    deliverySelect?.addEventListener("change", updateSummary);

    document.getElementById("order-form").addEventListener("submit", e => {
        e.preventDefault();

        const name = document.getElementById("name").value.trim();
        const phone = document.getElementById("phone").value.trim();
        const address = document.getElementById("address").value.trim();

        if (!name || !phone || !address) {
            alert("⚠️ Please fill out all fields before submitting.");
            return;
        }

        alert("✅ Order placed successfully! (Future: Save to DB)");
        e.target.reset();
        qty.value = 1;
        updateSummary();
    });

    // Initial sync
    updateSummary();
});
