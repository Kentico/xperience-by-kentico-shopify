
using CMS.Core;

using GraphQL;

using Microsoft.AspNetCore.Http;

namespace Kentico.Xperience.Shopify.ShoppingCart;

internal class ShoppingService : ShopifyStorefrontServiceBase, IShoppingService
{
    private const string CART_ID_KEY = "CMSShoppingCart";

    private readonly IEventLogService eventLogService;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IShoppingCartCacheService shoppingCartCacheService;

    public ShoppingService(
        IEventLogService eventLogService,
        IHttpContextAccessor httpContextAccessor,
        IGraphQLHttpClientFactory clientFactory,
        IShoppingCartCacheService shoppingCartCacheService) : base(clientFactory)
    {
        this.eventLogService = eventLogService;
        this.httpContextAccessor = httpContextAccessor;
        this.shoppingCartCacheService = shoppingCartCacheService;
    }


    public async Task<CartOperationResult> UpdateCartItem(ShoppingCartItemParameters parameters)
    {
        var cart = await GetCurrentShoppingCart();

        var cartItemToUpdate = cart?.Items.FirstOrDefault(x => x.ShopifyCartItemId == parameters.MerchandiseID);
        if (cart == null || cartItemToUpdate == null)
        {
            return await AddItemToCart(parameters);
        }

        int quantity = Math.Max(parameters.Quantity, 0);
        var result = await UpdateCartItemInternal(cart.CartId, cartItemToUpdate, quantity);
        if (result.Success && result.Cart != null)
        {
            UpdateCartCache(result.Cart);
        }

        return result;
    }


    public async Task<CartOperationResult> RemoveCartItem(string merchandiseId)
    {
        var cart = await GetCurrentShoppingCart();
        if (cart == null)
        {
            return new CartOperationResult(null, true);
        }

        var shopifyCartLine = cart.Items.FirstOrDefault(x => x.VariantGraphQLId == merchandiseId);
        if (shopifyCartLine == null)
        {
            return new CartOperationResult(cart, true);
        }
        var result = await RemoveCartItemInternal(cart.CartId, shopifyCartLine.ShopifyCartItemId);
        if (result.Success && result.Cart != null)
        {
            UpdateCartCache(result.Cart);
        }

        return result;
    }


    public async Task<ShoppingCartInfo?> GetCurrentShoppingCart()
    {
        string? cartId = GetCurrentShoppingCartId();
        if (string.IsNullOrEmpty(cartId))
        {
            return null;
        }

        return await shoppingCartCacheService.LoadAsync(cartId, GetCurrentShoppingCartInternal);
    }


    private async Task<CartOperationResult> CreateShoppingCart(ShoppingCartItemParameters parameters)
    {
        // Minified GraphQL query
        string query = $"mutation createCart($cartInput: CartInput, $country: CountryCode) @inContext(country: $country) {{ cartCreate(input: $cartInput) {{ cart {CartObjectModel.MutationObjectScheme} userErrors {{ field message }} }} }}";

        var createCart = new CreateCartParameters()
        {
            BuyerIdentity = new CreateCartBuyer()
            {
                CountryCode = parameters.Country.ToString()
            },
            Lines = [new CartLine()
            {
                Quantity = parameters.Quantity,
                MerchandiseId = parameters.MerchandiseID
            }]
        };

        var cartResponse = await PostMutationAsync<CreateCartResponse>(query, new { CartInput = createCart, parameters.Country });
        var cart = cartResponse.Data.CartCreate?.Cart;
        var userErrors = cartResponse.Data.CartCreate?.UserErrors;

        bool success = ResponseIsOk(cartResponse.Errors, cart, userErrors);

        if (cart == null)
        {
            return new CartOperationResult(null, success);
        }

        return new CartOperationResult(new ShoppingCartInfo(cart), success);
    }


    public async Task<CartOperationResult> AddItemToCart(ShoppingCartItemParameters parameters)
    {
        bool newCartCreated = false;
        string? cartId = GetCurrentShoppingCartId();

        CartOperationResult result;

        if (string.IsNullOrEmpty(cartId))
        {
            result = await CreateShoppingCart(parameters);
            newCartCreated = result.Success;
        }
        else
        {
            result = await ExecuteAddItemMutation(parameters, cartId);

            // Shopping cart might have been deleted in the meantime.
            if (!result.Success)
            {
                result = await CreateShoppingCart(parameters);
                newCartCreated = result.Success;
            }
        }

        if (result.Success && result.Cart != null)
        {
            var cart = result.Cart;
            UpdateCartCache(cart);

            if (newCartCreated)
            {
                StoreCartToCookiesAndSession(cart.CartId);
            }
        }
        return result;
    }


    private async Task<CartOperationResult> ExecuteAddItemMutation(ShoppingCartItemParameters parameters, string cartId)
    {
        string query = $"mutation addCartLines($cartId: ID!, $lines: [CartLineInput!]!) {{ cartLinesAdd(cartId: $cartId, lines: $lines) {{ cart {CartObjectModel.MutationObjectScheme} userErrors {{ field message }} }} }}";

        var addToCartParams = new AddToCartParameters()
        {
            CartId = cartId,
            Lines = new AddToCartLines()
            {
                MerchandiseId = parameters.MerchandiseID,
                Quantity = parameters.Quantity
            }
        };

        var cartResponse = await PostMutationAsync<AddToCartResponse>(query, addToCartParams);
        var cart = cartResponse.Data?.CartLinesAdd?.Cart;
        bool success = ResponseIsOk(cartResponse.Errors, cart, cartResponse.Data?.CartLinesAdd?.UserErrors);

        if (cart == null)
        {
            return new CartOperationResult(null, success);
        }

        return new CartOperationResult(new ShoppingCartInfo(cart), success);
    }


    private async Task<CartOperationResult> UpdateCartItemInternal(string cartId, ShoppingCartItem cartItem, int newQuantity)
    {
        string query = $"mutation updateCartLines($cartId: ID!, $lines: [CartLineUpdateInput!]!) {{ cartLinesUpdate(cartId: $cartId, lines: $lines) {{ cart {CartObjectModel.MutationObjectScheme} userErrors {{ field message }} }} }}";
        var parameters = new UpdateCartLineParameters()
        {
            CartId = cartId,
            Lines = new UpdateCartLine()
            {
                Id = cartItem.ShopifyCartItemId,
                Quantity = newQuantity
            }
        };
        var cartResponse = await PostMutationAsync<UpdateCartLinesResponse>(query, parameters);

        var cart = cartResponse.Data?.CartLinesUpdate?.Cart;
        bool success = ResponseIsOk(cartResponse.Errors, cart, cartResponse.Data?.CartLinesUpdate?.UserErrors);

        if (cart == null)
        {
            return new CartOperationResult(null, success);
        }

        return new CartOperationResult(new ShoppingCartInfo(cart), success);
    }


    private async Task<CartOperationResult> RemoveCartItemInternal(string shoppingCartId, string shopifyCartLineId)
    {
        string query = $"mutation cartLinesRemove($cartId: ID!, $lineIds: [ID!]!) {{ cartLinesRemove(cartId: $cartId, lineIds: $lineIds) {{ cart {CartObjectModel.MutationObjectScheme} userErrors {{ field message }} }} }}";
        var cartResponse = await PostMutationAsync<RemoveCartItemResponse>(query, new { CartId = shoppingCartId, LineIds = new string[] { shopifyCartLineId } });

        var cart = cartResponse.Data?.CartLinesRemove?.Cart;
        bool success = ResponseIsOk(cartResponse.Errors, cart, cartResponse.Data?.CartLinesRemove?.UserErrors);

        if (cart == null)
        {
            return new CartOperationResult(null, success);
        }

        return new CartOperationResult(new ShoppingCartInfo(cart), success);

    }


    private async Task<ShoppingCartInfo?> GetCurrentShoppingCartInternal(string shoppingCartId)
    {
        string query = $"query cartQuery($cartId: ID!) {{ cart(id: $cartId) {CartObjectModel.QueryObjectScheme} }}";
        var response = await PostQueryAsync<GetCartResponse>(query, new { cartId = shoppingCartId });

        if (response.Data?.Cart == null)
        {
            return null;
        }

        return new ShoppingCartInfo(response.Data.Cart);
    }


    /// <summary>
    /// Check and log response errors into kentico event log.
    /// </summary>
    /// <param name="responseErrors"></param>
    /// <param name="cart"></param>
    /// <param name="cartUserErrors"></param>
    /// <returns>True if no error occured, otherwise false.</returns>
    private bool ResponseIsOk(GraphQLError[]? responseErrors, CartObjectModel? cart, IEnumerable<CartUserError>? cartUserErrors)
    {
        var userErrors = cartUserErrors?.Select(x => x.Message);
        string message = string.Empty;
        if (cart == null ||
            (responseErrors != null && responseErrors.Length != 0) ||
            (userErrors != null && userErrors.Any()))
        {
            var errors = responseErrors?.Select(x => x.Message);
            message = userErrors != null
                ? string.Join("\n", userErrors)
                : string.Join("\n", errors ?? ["Cannot create shopping cart"]);

            eventLogService.LogError(nameof(ShoppingService), nameof(AddItemToCart), message);
            return false;
        }

        return true;
    }


    private string? GetCurrentShoppingCartId()
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return null;
        }

        string? cartId = httpContext.Session.GetString(CART_ID_KEY);
        if (string.IsNullOrEmpty(cartId))
        {
            httpContext.Request.Cookies.TryGetValue(CART_ID_KEY, out cartId);
        }

        return cartId;
    }


    private void StoreCartToCookiesAndSession(string cartId)
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext != null && !string.IsNullOrEmpty(cartId))
        {
            var cookieOptions = new CookieOptions()
            {
                Expires = DateTime.Now.AddDays(30),
                HttpOnly = true
            };

            httpContext.Response.Cookies.Append(CART_ID_KEY, cartId, cookieOptions);
            httpContext.Session.SetString(CART_ID_KEY, cartId);
        }
    }


    private void UpdateCartCache(ShoppingCartInfo cart) => shoppingCartCacheService.UpdateCartCache(cart);
}

