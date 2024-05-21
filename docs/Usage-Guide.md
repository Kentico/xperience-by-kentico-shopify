# Usage Guide
## Table of contents
1. [Shopify integration overview](#shopify-integration)
2. [Shopify integration setup](#setup)

## Shopify integration
Main part of the integration is in `Kentico.Xperience.Shopify` class library. For the communication between Shopify and XByK, GraphQL Storefront API and REST Admin API is used. For the REST Admin API communication,    [ShopifySharp](https://www.nuget.org/packages/ShopifySharp/) NuGet package is used as it provides all the necessary methods used in this example. All the available Shopify APIs are documented [here](https://shopify.dev/docs/api).
Class library consists of 3 main parts - Product listing widget, Shopify products synchronization and the e-commerce integration.

### Product listing widget
Product listing widget is standalone widget that displays Shopify products using the Admin REST API. This widget requires only to have Shopify configuration set up in `appsettings.json`,`user secrets` or `Shopify configuration module`(no products need to be synchronized). Everything is loaded directly from the Shopify store. Products displayed in this widget can be filtered by collection(Shopify equivalent to product category). Widget properties also contain currency selector and maximum number of retrieved products. Each product has a button that will redirect user to the product detail page on the Shopify store website.

#### Limitations
Shopify API can return maximum of 250 items in one API request. For larger number of products, pagination needs to be implemented.


### Shopify products synchronization
Synchronization running in background thread worker periodically and is implemented  in [ShopifySynchronizationWorker.cs](../src/Kentico.Xperience.Shopify/Synchronization/ShopifySynchronizationWorker.cs) class. It runs every 15 minutes but this can be changed by modifying `_defaultInterval` property of the `ShopifySynchronizationWorker` class. The time between executions needs to be specified in milliseconds. To change interfal between synchronizations, change value of this property. All synchronization items are stored as content items:
- `Shopify Image`: image asset that belongs either to product or product variant
- `Shopify Product Variant`: representation of product variant containing the title, SKU number, weight and collection of `Shopify Image` content items related to this product variant.
- `Shopify Product`: the product itself containing title, description, collection of related images as `Shopify Image` content items and list of `Shopify Product Variant` content items. This content type also includes field Product parameters that editors can modify and it won't be overwritten by synchronization.
Note: Price data and product availability is not synchronized since these values are changed very often and it is necessary to have them up to date. These values are always retrieved directly from Shopify using Admin REST API everytime product detail page is loaded.
It is not recommended to update any fields of these content items as it will be always overwritten when the synchronization is executed.

#### Limitations
Currently products are synchronized only in default content culture.

### Benefits of products synchronization
Constantly querying all product data directly from Shopify can lead to performance bottlenecks, especially when there is a large product catalog. Synchronizing the data to Xperience by Kentico application reduces the load on Shopify's API and improves the overall performance of the application. Having products stored as content items allows to create additional properties without modifying the products in Shopify.

### E-commerce Integration Overview
E-commerce integration is implemented via `IShoppingService` interface, that provides methods for all e-commerce actions(for example updating shopping cart items). Each action is then sent to Shopify using [GraphQL Storefront API](https://shopify.dev/docs/api/storefront).

#### Shopping Cart Management
When a user adds their first item to the shopping cart, a shopping cart instance is created. Each shopping cart is assigned a unique identifier known as the `CartId`. This `CartId` is generated within Shopify upon cart creation. To maintain session persistence, the `CartId` is stored both in cookies and the HTTP session, utilizing the key `CMSShoppingCart`. 

The `IShoppingService` interface supports the following cart operations:
- Add/update/remove cart items
- Add/remove discount code(works with all discount types)
- Get current shopping cart

#### E-commerce content types
- `Shopify.ProductDetailPage`: Each product content item that is listed in the store has its own detail page. This page can be then linked to category page(one product can be in multiple categories). Related product content item is defined in `Related product` field.
- `Shopify.CategoryPage`: Dedicated pages for specific product categories. Page displays list of products that are assigned to this category. Product can be added into category using `Products` field of the category page.
- `Shopify.StorePage`: The main page showcasing a list of product categories and preselected products. Page displays list of all the child category pages and also preselected products marked as Bestsellers and Hot tips(if any selected).
- `Shopify.ShoppingCartPage`: A preview page displaying the contents of the shopping cart, providing users with an overview of their selected items before proceeding to checkout. Users are also able to modify shopping cart items and add or remove coupon codes from shopping cart.
- `Shopify.ThankYouPage`: Upon successful completion of an order, users are redirected to the thank-you page. This page is used for tracking purchase custom activities.


#### Checkout
The next step from shopping cart preview page(`Shopify.ShoppingCartPage`) is redirection to official Shopify store checkout page where user can complete the checkout and create the order. Checkout completion using Shopify API is not possible as whole checkout API will be removed from Shopify. To redirect user from Shopify thank you page to DancingGoat, following javascript tag for redirection needs to be inserted into Shopify order status page(you can set it in Shopify administration via `Settings` -> `Checkout` -> `Order status page` ->`Additional scripts`):
```html
<script>
	/// Replace with delay in ms
	const redirectionDelay = 5000;
	/// Replace with absolute URL of your XByK thank you page
	const thankYouPageUrl = "https://my-dancing-goat.com/thank-you";
    window.setTimeout(function(){
            var urlPart = "/checkouts/";
            var currentUrl = window.location.href;
            if (!currentUrl.includes(urlPart)) {
                return;
            }
            var sourceId= currentUrl.split(urlPart)[1].split("/")[1];
            window.location.href = thankYouPageUrl + "?sourceId=" + sourceId;
        }, redirectionDelay);
</script>
```
This script will redirect the user to the Xperience by Kentico Thank you page after 5 seconds. You can adjust the timespan and URL by modifying the `redirectionDelay` and `thankYouPageUrl` constants, respectively. The redirection will occur exclusively from the Shopify thank you page, ensuring users can still check their order status in the future. Query parameter `sourceId` is then used to retrieve created order, update XByK contact information based on the order information and log purchase activity.

### E-commerce activity tracking
`IEcommerceActivityLogger` interface provides tracking of these e-commerce activities:
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
Since Shopify identifiers are using `long` data type, `ActivityItemID` is impossible to use. The activity itself is tracked correctly but any tracking or analytics features that rely on `ActivityItemID` will not work as expected.
If storing `ActivityItemID` value is necessary, there can be implemented generation of a custom identifier within the Xperience by Kentico application that maps to the Shopify long identifier. Then, this custom identifier could be used as `ActivityItemID`.

## Setup

### Generating shopify API credentials
1. Log in to your shopify store administration.
2. Go to `Settings` -> `Apps and sales channels`.
3. Click on `Develop apps`.
4. Click on `Create an app` and fill the app name.
5. Configure Admin API scopes.
	- Go to `configuration` tab.
	- Add following Admin API access scopes: `write_product_listings`, `read_product_listings`, `write_products`, `read_products`, `read_inventory`, `write_orders`, `read_orders`.
6. In the `Apps and sales channels` install the Headless channel from the [Shopify App Store](https://apps.shopify.com/headless).
7. In the Headless channel app, create new Storefront. After that, storefront `Private access token` should be available.

### Dancing Goat example - setup
1. Go to `./examples/DancingGoat-Shopify` folder and run CI restore for content types, product related pages and shopping cart page
```powershell
dotnet run --kxp-ci-restore
```
All content types and custom activities for e-ecommerce events are created.

2. Start Xperience By Kentico Dancing Goat application (`DancingGoat` in `.\examples`) configured with your own database and wait for product synchronization finish. Check `Shopify product synchronization done.` message in event log.

3. Create pages for Store using page types from this [this list](#e-commerce-page-types).
