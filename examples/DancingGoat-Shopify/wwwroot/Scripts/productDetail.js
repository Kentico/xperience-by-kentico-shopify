const PRICES_URL = "/api/shopify/product";
const container = document.querySelector('.js-productDetail');
const discountPriceElem = container.querySelector('.discount-price');
var variants = {};

function fetchVariants(select) {
    var productId = container.getAttribute('data-product-id');
    var url = new URL(PRICES_URL, window.location.origin);
    const cartSection = container.querySelector('.js-cartSection');
    url.searchParams.append('productId', productId);

    fetch(url)
        .then(function (response) {
            if (!response.ok) {
                if (cartSection) {
                    cartSection.classList.remove('hidden');
                    cartSection.innerHTML = '<p>Could not fetch variant prices.</p>'
                }
                throw new Error('Could not fetch variant prices.');
            }
            return response.json();
        })
        .then(function (data) {
            variants = data;
            if (Object.keys(variants).length <= 1) {
                select.disabled = true
            }
            changePricesAndStock(variants[select.value]);
            if (cartSection) {
                cartSection.classList.remove('hidden');
            }
        });
}

function selectChanged(event) {
    var value = event.target.value;
    var selectedPrices = variants[value];
    if (!selectedPrices) {
        return;
    }

    container.setAttribute('data-selected-variant', value);

    changePricesAndStock(selectedPrices);
}

function changePricesAndStock(selectedVariant) {
    var priceElem = container.querySelector('#js-price');
    var listPriceElem = container.querySelector('#js-listPrice');
    var stockMessage = container.querySelector('#stockMessage');
    var addToCartBtn = container.querySelector('.js-addToCartBtn');
    var merchandiseIdElem = container.querySelector('.js-merchandiseId');
    var cartQuantityElem = container.querySelector('.js-cartItemQuantity');
    var removeFromCartBtn = container.querySelector('.js-removeFromCartBtn');

    if (priceElem) {
        priceElem.innerHTML = selectedVariant.priceFormatted;
    }
    if (listPriceElem) {
        listPriceElem.innerHTML = selectedVariant.listPriceFormatted;
    }

    if (selectedVariant.listPriceFormatted) {
        discountPriceElem.classList.remove('hidden');
    } else {
        discountPriceElem.classList.add('hidden');
    }

    if (merchandiseIdElem) {
        merchandiseIdElem.value = selectedVariant.merchandiseID;
    }

    if (stockMessage) {
        stockMessage.className = 'stock ' + selectedVariant.stockCssClass;
        stockMessage.innerHTML = selectedVariant.stockStatusText;
    }

    if (cartQuantityElem) {
        cartQuantityElem.innerHTML = selectedVariant.itemsInCart;
    }
    
    const disabledClass = 'btn-disabled';
    if (addToCartBtn) {
        if (selectedVariant.available) {
            addToCartBtn.classList.remove(disabledClass);
            addToCartBtn.removeAttribute('disabled');
        } else {
            addToCartBtn.classList.add(disabledClass);
            addToCartBtn.setAttribute('disabled', true);
        }
    }
    if (removeFromCartBtn) {
        if (selectedVariant.available && selectedVariant.itemsInCart > 0) {
            removeFromCartBtn.classList.remove('hidden');
            removeFromCartBtn.removeAttribute('disabled');
        } else {
            removeFromCartBtn.classList.add('hidden');
            removeFromCartBtn.setAttribute('disabled', true);
        }
    }
}

if (container && container.getAttribute('data-product-id') != null) {
    const select = container.querySelector('.js-variantSelector');
    if (select) {
        select.onchange = selectChanged
    }

    fetchVariants(select);
}


