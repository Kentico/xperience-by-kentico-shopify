# Usage Guide
## Table of contents
1. [Shopify integration overview](#shopify-integration)
2. [Shopify integration setup](#setup)

## Shopify integration
For the communication between Shopify and XByK, GraphQL Storefront API and REST Admin API is used. For the REST Admin API communication,  [ShopifySharp](https://www.nuget.org/packages/ShopifySharp/) NuGet package is used as it provides all the necessary methods used in this integration. All the available Shopify APIs are documented [here](https://shopify.dev/docs/api).
Class library consists of 2 main parts - Shopify products synchronization and the e-commerce integration. In this repository, there is also an example of standalone product listing widget.

### Product listing widget
The **Product listing** widget is a standalone widget that displays Shopify products using the Shopify Admin REST API. The Shopify integration must be set up in `appsettings.json` or in the `Shopify integration` application for this widget to work (no products need to be synchronized). The widget can be used to display products from your Shopify store outside of the [content type structure](#integration-specific-content-types) the integration provides.

#### Limitations
- Shopify API can return maximum of 250 items in one API request. For larger number of products, pagination needs to be implemented. Detailed information on Shopify API pagination can be found [here](https://shopify.dev/docs/api/usage/pagination-rest). The `ShopifyProductService` wraps result in [ListResultWrapper](../src/Kentico.Xperience.Shopify/Products/Models/ListResultWrapper.cs). This wrapper returns retrieved items along with next page and previous page [PagingFilterParams](../src/Kentico.Xperience.Shopify/Products/Models/PagingFilterParams.cs). These filters can then be used in the `GetProductsAsync` method parameters to retrieve next or previous page from Shopify API. Due to this limitation, the maximum number of retrieved results in the [product listing widget](#product-listing-widget) is 250. To increase this limit, pagination must be implemented in the widget. This limitation also affects product synchronization, where only first 250 products are synchronized, and shopping cart, which can have maximum of 250 cart items.
- Only one currency per website channel is supported.

### Shopify products synchronization
Synchronization running in background thread worker periodically every 15 minutes and all synchronization items are stored as following content items:
- `Shopify Image`: image asset that belongs to either a product or a product variant.
- `Shopify Product Variant`: representation of a product variant containing the title, SKU number, weight and a collection of `Shopify Image` content items related to this product variant.
- `Shopify Product`: represents the product synchronized from Shopify. Contains the product title, description, a collection of product images (`Shopify Image` content items), and a list of `Shopify Product Variant` content items. This content type also includes the `product parameters` field that is not overwritten by the synchronization procedure and can be used by site content editors for custom content.
Note: Price data and product availability are not synchronized since these values change frequently and must be up to date. They are retrieved directly from Shopify using the Shopify Admin REST API every time the product detail page is loaded.
With the exception of the `product parameters` field of the "Shopify Product" content type, we don't recommend updating or modifying the values of any fields in the synchronized content items. The Shopify synchronization procedure overrides all fields in existing content items when it is executed.

#### Limitations
Currently, product synchronization creates content items (products, variants, images) only in English language. Multicultural synchronization is not implemented. If the English language is not registered in the [Languages](https://docs.kentico.com/x/OxT_Cw) application, synchronization won't work.

### Benefits of products synchronization
Constantly querying all product data directly from Shopify can lead to performance bottlenecks, especially when there is a large product catalog. Synchronizing the data to Xperience by Kentico application reduces the load on Shopify's API and improves the application's overall performance.

Having your products stored as content items in Xperience also allows you to add project-specific information (using [custom content type fields](https://docs.kentico.com/x/RIXWCQ)) to the synchronized products without modifying their description in Shopify.

### Integration API Overview
The Shopify integration is implemented via the `IShoppingService` interface. The interface provides methods for all common e-commerce actions (for example, updating shopping cart items). Each action is sent to Shopify using the [GraphQL Storefront API](https://shopify.dev/docs/api/storefront).

#### Shopping Cart Management
When a user adds their first item to the shopping cart, a shopping cart instance is created. Each shopping cart is assigned a unique identifier known as the `CartId`. This `CartId` is generated within Shopify upon cart creation. To maintain session persistence, the `CartId` is stored both in cookies and the HTTP session, utilizing the key `CMSShoppingCart`. 

The `IShoppingService` interface supports the following cart operations:
- Add/update/remove cart items. For usage examples, see the `Update` method in [ShopifyShoppingCartController](../examples/DancingGoat-Shopify/Controllers/Shopify/ShopifyShoppingCartController.cs) and the `Index` method in [ShopifyProductDetailController](../examples/DancingGoat-Shopify/Controllers/Shopify/ShopifyProductDetailController.cs).
- Add/remove discount code. This operation works with all discount types supported by Shopify. For usage examples, see the `AddDiscountCode` and `RemoveDiscountCode` methods in [ShopifyShoppingCartController](../examples/DancingGoat-Shopify/Controllers/Shopify/ShopifyShoppingCartController.cs).
- Get the current shopping cart. For a usage example, see the `InvokeAsync` method in [ShopifyCartViewComponent](../examples/DancingGoat-Shopify/Components/ViewComponents/ShopifyCart/ShopifyCartViewComponent.cs)

#### Integration-specific content types
- `Shopify.ProductDetailPage`: Each product content item that is listed in the store has its own detail page. This page can be then linked to category page(one product can be in multiple categories). Related product content item is defined in `Related product` field.
- `Shopify.CategoryPage`: Dedicated pages for specific product categories. The page displays a list of products that are assigned to this category. Products can be added to a category using the page's `Products` field.
- `Shopify.StorePage`: The store home page showcasing a list of product categories and preselected products. The page displays a list of all existing child category pages and preselected products marked as **bestsellers** and **hot tips** (if selected for products).
- `Shopify.ShoppingCartPage`: A preview page displaying the contents of the shopping cart, providing users with an overview of their selected items before proceeding to checkout. Users are also able to modify shopping cart items and add or remove coupon codes from shopping cart.
- `Shopify.ThankYouPage`: Upon the successful completion of an order, users are redirected to the thank-you page. This page is used for tracking purchases using [custom activities](https://docs.kentico.com/x/xoouCw).

Step-by-step tutorial to create new pages:
1. Firstly, start with creating the product detail pages. Fill the "Related product" field with product content item that the page should represent.
2. Create store page and, optionaly, fill "Bestsellers" and "Hot tips" fields with relevant product pages. The filled products will be display in on the page in 2 separate listings - for "Bestsellers" and "Hot tips".
3. Create new category page and in "Products" field, select all the product detail pages that should be included in the category. The selected products will be displayed as a products listing in the category page.

#### Checkout
The next step from shopping cart preview page(`Shopify.ShoppingCartPage`) is redirection to official Shopify store checkout page where user can complete the checkout and create the order. Checkout completion using Shopify API is not possible as whole checkout API will be removed from Shopify.

#### Redirection back to DancingGoat site
After order is completed, users need to be redirected back to DancingGoat. To do this, [web pixels](https://shopify.dev/docs/apps/build/marketing-analytics/pixels) in combination with [liquid templates](https://shopify.dev/docs/api/liquid) needs to be used. 

Step-by-step tutorial:
1. Create custom web pixel in `Settings` -> `Customer events` -> `Add custom pixel`. In the web pixel, [checkout completed](https://shopify.dev/docs/api/web-pixels-api/standard-events/checkout_completed) event will be used to set `orderId` cookie. In `customer privacy` section, set `Permission` to `Not required` so the pixel will always run. Then, insert following code:
	```javascript
	analytics.subscribe('checkout_completed', (event) => {
		const orderId = event.data.checkout.order?.id;
		document.cookie = "orderId=" + orderId + ";path=/";
	});
	```
	Redirect inside the web pixel cannot be used since it runs inside an iframe that does not allow redirects.

2. Add script to check for `orderId` cookie and redirect back to DancingGoat. Go to `Online store` -> `Themes`. Tap on three dots at your current theme and select `Edit code`. Go to `assets` folder and click `+ Add a new asset` -> `Create a blank file`. Select js extension and create new asset. To the new javascript file, add this code:
	```javascript
	function getCookie(cookieName) {
		let cookies = document.cookie;
		let cookieArray = cookies.split("; ");

		for (let i = 0; i < cookieArray.length; i++) {
			let cookie = cookieArray[i];
			let [name, value] = cookie.split("=");

			if (name === cookieName) {
				return decodeURIComponent(value);
			}
		}

		return null;
	}

	function deleteCookie(cookieName) {
		document.cookie = cookieName + "=; expires=Thu, 01 Jan 1970 00:00:00 UTC; path=/;";
	}

	const orderCookieName = "orderId";
	const orderId = getCookie(orderCookieName);

	if (orderId) {
		deleteCookie(orderCookieName);

		/// Replace with absolute URL of your XByK thank you page
		const thankYouPageUrl = "https://my-dancing-goat.com/thank-you";
		window.location.href = thankYouPageUrl + "?orderId=" + orderId;
	}
	```
	When the order is completed and user will go back to the store(for example by pressing the 'Continue shopping' button), this javascript will be executed and if `orderId` cookie exists, user will be redirected back to the DancingGoat site and the cookie will be removed(to prevent further redirections). Query parameter `orderId` is then used to retrieve created order, update XByK contact information based on the order information and log purchase activity.

3. Last step is to add this javascript to the layout. Find `theme.liquid` file inside `layout` folder and add file created in previous step to the `head` HTML element.
	```html
	<!-- Change redirect.js to filename created in previous step -->
	<script src="{{ 'redirect.js' | asset_url }}" defer="defer"></script>
	```


### Store activity tracking
The integration allows you to log product-related [activities](https://docs.kentico.com/x/oYPWCQ) via the `IEcommerceActivityLogger` service. The service provides tracking methods for the following events:
- `Product added to shopping cart`
	```csharp
	// Log product was added to shopping cart.
	// Old quantity was higher than new quantity.
	activityLogger.LogProductAddedToShoppingCartActivity(cartItemToUpdate, newQuantity - cartItemToUpdate.Quantity);
	```
- `Product removed from shopping cart`
	```csharp
	// Log product was removed from shopping cart.
	// Old quantity was lower than new quantity
	activityLogger.LogProductRemovedFromShoppingCartActivity(cartItemToUpdate, cartItemToUpdate.Quantity - newQuantity);
	```
- `Product purchased` and `Made a purchase` after new order was created
	```csharp
	// Log purchase activity and purchased products after order was created.
	activityLogger.LogPurchaseActivity(order.TotalPriceSet.PresentmentMoney.Amount, order.Id, order.PresentmentCurrency);
	foreach (var lineItem in cart.Items)
	{
		activityLogger.LogPurchasedProductActivity(lineItem);
	}
	```
More examples can be found in [ShoppingService.cs](../src/Kentico.Xperience.Shopify/ShoppingCart/ShoppingService.cs) and [ShopifyThankYouController.cs](../examples/DancingGoat-Shopify/Controllers/Shopify/ShopifyThankYouController.cs).

#### Limitations
Since Shopify identifiers use the `long` data type, `ActivityItemID` is impossible to use. The activity itself is tracked correctly but any tracking or analytics features that rely on `ActivityItemID` will not work as expected.
If storing `ActivityItemID` value is necessary, there can be implemented generation of a custom identifier within the Xperience by Kentico application that maps to the Shopify long identifier. Then, this custom identifier could be used as `ActivityItemID`.

## Setup

### Generating shopify API credentials
1. Log in to your Shopify store administration.
2. Go to `Settings` -> `Apps and sales channels`.
3. Click on `Develop apps`.
4. Click on `Create an app` and fill the app name.
5. Configure Admin API scopes.
	- Go to the `Configuration` tab.
	- Add the following Shopify Admin API access scopes: `read_product_listings`, `read_products`, `read_inventory`, `read_orders`, `read_draft_orders`.
6. In the `Apps and sales channels` install the Headless channel from the [Shopify App Store](https://apps.shopify.com/headless).
7. In the Headless channel app, create new Storefront. After that, storefront `Private access token` should be available.

### Dancing Goat example - setup
1. Create a blank database using following command:
	```powershell
	dotnet kentico-xperience-dbmanager -- -s "<sql_server_name>" -d "<database_name>" -a "<admin_password>" --hash-string-salt "<hash_string_salt>" --license-file "<license_file_path>"
	```
	and, connect it to the sample project (CMSConnectionString).
	
2. Go to `./examples/DancingGoat-Shopify` folder and run CI restore for content types, product related pages and shopping cart page
	```powershell
	dotnet run --kxp-ci-restore
	```
	All content types and custom activities for e-ecommerce events are created.

3. Start Xperience By Kentico Dancing Goat application (`DancingGoat` in `.\examples`) configured with your own database and wait for product synchronization finish. Check `Shopify product synchronization done.` message in event log.

4. Create pages for Store using page types from this [this list](#e-commerce-content-types).
