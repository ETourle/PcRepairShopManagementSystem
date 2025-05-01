
// --- Utility to read/save
function getCart() {
    return JSON.parse(localStorage.getItem('pcCart') || '[]');
}
function saveCart(cart) {
    localStorage.setItem('pcCart', JSON.stringify(cart));
}

// --- Expose addToCart globally so inline onclicks work
function addToCart(componentId, componentName, unitPrice, qty) {
    let cart = getCart();
    const existing = cart.find(i => i.componentId === componentId);
    if (existing) {
        existing.quantity = qty;
    } else {
        cart.push({ componentId, componentName, unitPrice, quantity: qty });
    }
    saveCart(cart);
    renderCartBadge(cart.length);
}
// make it available on window
window.addToCart = addToCart;

// --- Render the little red badge
function renderCartBadge(count) {
    const el = document.getElementById('cartCount');
    if (el) el.textContent = count;
}

// --- Render mini-cart dropdown contents
function renderMiniCart() {
    const cart = getCart();
    const container = document.getElementById('miniCartItems');
    if (!container) return;

    container.innerHTML = '';
    if (cart.length === 0) {
        container.innerHTML = '<p class="text-center text-muted mb-0">Your cart is empty.</p>';
        const totalEl = document.getElementById('miniCartTotal');
        if (totalEl) totalEl.textContent = '£0.00';
        return;
    }

    let total = 0;

    cart.forEach(item => {
        total += item.unitPrice * item.quantity;

        // Build a small row with delete button
        const row = document.createElement('div');
        row.className = 'd-flex align-items-center mb-2 justify-content-between';
        row.innerHTML = `
      <div class="d-flex align-items-center flex-grow-1">
        <div class="me-2">
          <div>${item.componentName}</div>
          <div class="small text-muted">
            ${(item.unitPrice).toLocaleString(undefined, { style: 'currency', currency: 'GBP' })}
          </div>
        </div>
        <div class="input-group input-group-sm" style="width:6rem;">
          <button class="btn btn-outline-secondary minus" type="button">−</button>
          <input type="text" class="form-control text-center qty" value="${item.quantity}" readonly />
          <button class="btn btn-outline-secondary plus" type="button">+</button>
        </div>
      </div>
      <button class="btn btn-sm btn-danger ms-2 delete-btn">
        <i class="bi bi-trash"></i>
      </button>
    `;

        // + / − logic
        const minusBtn = row.querySelector('button.minus');
        const plusBtn = row.querySelector('button.plus');
        const qtyInput = row.querySelector('input.qty');

        minusBtn.addEventListener('click', () => {
            if (item.quantity > 1) {
                item.quantity--;
                qtyInput.value = item.quantity;
                saveCart(cart);
                renderCartBadge(cart.length);
                renderMiniCart();
            }
        });
        plusBtn.addEventListener('click', () => {
            item.quantity++;
            qtyInput.value = item.quantity;
            saveCart(cart);
            renderCartBadge(cart.length);
            renderMiniCart();
        });

        // Delete single item
        const delBtn = row.querySelector('button.delete-btn');
        delBtn.addEventListener('click', () => {
            const newCart = getCart().filter(i => i.componentId !== item.componentId);
            saveCart(newCart);
            renderCartBadge(newCart.length);
            renderMiniCart();
        });

        container.appendChild(row);
    });

    // Update subtotal
    const totalEl = document.getElementById('miniCartTotal');
    if (totalEl) {
        totalEl.textContent = total.toLocaleString(undefined, { style: 'currency', currency: 'GBP' });
    }
}

// --- Initialize on page load
document.addEventListener('DOMContentLoaded', () => {
    const cart = getCart();
    renderCartBadge(cart.length);

    // Populate dropdown every time it's shown
    const cartLink = document.getElementById('cartDropdownLink');
    if (cartLink) {
        cartLink.addEventListener('show.bs.dropdown', renderMiniCart);
    }
});
