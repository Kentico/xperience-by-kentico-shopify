# Usage Guide
## Table of contents
1. [Shopify integration overview](#shopify-integration)
2. [Shopify integration setup](#setup)

## Shopify integration
Main part of the integration is in `Kentico.Xperience.Shopify` class library. For the communication between Shopify and XByK, GraphQL Storefront API and REST Admin API is used. For the REST Admin API communication,    [ShopifySharp](https://www.nuget.org/packages/ShopifySharp/) NuGet package is used as it provides all the necessary methods used in this example. All the available Shopify APIs are documented [here](https://shopify.dev/docs/api).
Class library consists of 3 main parts - Product listing widget, Shopify products synchronization and the e-commerce integration.

### Product listing widget
Product listing widget is standalone widget that displays Shopify products using the Admin REST API. Products displayed in this widget can be filtered by collection(Shopify equivalent to product category). Widget properties also contain currency selector and maximum number of retrieved products. Each product has a button that will redirect user to the product detail page on the Shopify store website.

#### Limitations
Shopify API can return maximum of 250 items in one API request. For larger number of products, pagination needs to be implemented.


### Shopify products synchronization
Synchronization running in background thread worker periodically and is implemented  in `ShopifySynchronizationWorker` class. It runs every 15 minutes but this can be changed by modifying `_defaultInterval` property. The time between executions needs to be in milliseconds. All synchronization items are stored as content items:
- `Shopify Image`: image asset that belongs either to product or product variant
- `Shopify Product Variant`: representation of product variant containing the title, SKU number, weight and collection of `Shopify Image` content items related to this product variant.
- `Shopify Product`: the product itself containing title, description, collection of related images as `Shopify Image` content items and list of `Shopify Product Variant` content items. This content type also includes field Product parameters that editors can modify and it won't be overwritten by synchronization.

Price data and product availibility is not synchronized. These values are always retrieved directly from Shopify.

It is not recommended to update any fields of these content items as it will be always overwritten when the synchronization is executed.

#### Limitations
Currently products are synchronized only in default content culture.


### E-commerce Integration Overview
E-commerce integration is implemented via `IShoppingService` interface, that provides methods for all e-commerce actions. Each action is then sent to Shopify using GraphQL Storefront API.

#### Shopping Cart Management
When a user adds their first item to the shopping cart, a shopping cart instance is created. Each shopping cart is assigned a unique identifier known as the `CartId`. This `CartId` is generated within Shopify upon cart creation. To maintain session persistence, the `CartId` is stored both in cookies and the HTTP session, utilizing the key `CMSShoppingCart`. 

The `IShoppingService` interface supports the following cart operations:
- Add/update/remove cart items
- Add/remove discount code
- Get current shopping cart

#### E-commerce page types
- `Shopify.StorePage`: The main page showcasing a list of product categories.
- `Shopify.CategoryPage`: Dedicated pages for specific product categories.
- `Shopify.ProductDetailPage`: Each product listed in the store has its own detail page. This page is then linked to category page(one product can be in multiple categories).
- `Shopify.ShoppingCartPage`: A preview page displaying the contents of the shopping cart, providing users with an overview of their selected items before proceeding to checkout. Users are also able to modify shopping cart items and add or remove coupon codes from shopping cart.
- `Shopify.ThankYouPage`: Upon successful completion of an order, users are redirected to the thank-you page.


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
This script will redirect the user to the XByK thank you page after 5 seconds. You can adjust the timespan and URL by modifying the `redirectionDelay` and `thankYouPageUrl` constants, respectively. The redirection will occur exclusively from the Shopify thank you page, ensuring users can still check their order status in the future. Query parameter `sourceId` is then used to retrieve created order, update XByK contact information based on the order information and log purchase activity.

### E-commerce activity tracking
`IEcommerceActivityLogger` interface provides tracking of these e-commerce activities:
- `Product added to shopping cart`
- `Product removed from shopping cart`
- `Product purchased`
- `Made a purchase`

#### Limitations
Since Shopify identifiers are using `long` data type,  `ActivityItemID` is impossible to use.


## Setup

### Generating shopify API credentials
1. Log in to your shopify store administration.
2. Go to `Settings` ->`Apps and sales channels`.
3. Click on `Develop apps`.
4. Click on `Create an app` and fill the app name.
5. Configure Admin API scopes.
	- Go to `configuration` tab.
	- Add following Admin API access scopes: `write_product_listings`, `read_product_listings`, `write_products`, `read_products`, `read_inventory`, `write_orders`, `read_orders`.
6. In the `Apps and sales channels` install the Headless channel from the [Shopify App Store](https://apps.shopify.com/headless).
7. In the Headless channel app, create new Storefront. After that, storefront `Private access token` should be available.

### XByK set up
1.  (optional)Setup your own settings to connect your Shopify instance. Use API tokens generated in the [generating shopify API credentials](#generating-shopify-api-credentials) section.
```json
{  
  "CMSShopifyConfig": {
    "ShopifyUrl": "https://quickstart-xxxxxxxxxx.myshopify.com/",
    "AdminApiToken": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
    "StorefrontApiToken": "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
    "StorefrontApiVersion": "YYYY-MM"
  }
}
```

**Setting description**
| Setting              | Description                                                                      |
| -------------------- | -------------------------------------------------------------------------------- |
| ShopifyUrl           | URL of the Shopify store                                                         |
| AdminApiToken        | Access token for the Admin API calls                                             |
| StorefrontApiToken   | Access token for the Storefront API calls                                        |
| StorefrontApiVersion | Storefront API version that will be used in API calls. Must be in format YYYY-MM |
2. Add library to the application services.
```csharp
// Program.cs

// Registers Shopify services
builder.Services.RegisterShopifyServices(builder.Configuration);
```
3. Enable session state for the application
```csharp
// Program.cs

// Enable session state for appliation
app.UseSession();
```
4. Restore CI repository files to database (reusable content types, custom activities). CI files are located in  `.\examples\DancingGoat-Shopify\App_Data\CIRepository\`  and copy these files to your application.
```powershell
dotnet run --no-build --kxp-ci-restore
```
5.  Copy product listing widget from Dancing Goat example project to your project. Sample widget is located in  [here](https://github.com/Kentico/xperience-by-kentico-shopify/blob/feat/XbyK_Shopify_integration/examples/DancingGoat-Shopify/Components/Widgets/Shopify/ProductListWidget).
6. Start your livesite
7. If `CMSShopifyConfig` is not filled in the step 1, go to Shopify configuration module in Xperience by Kentico admin page and fill the credentials. Note that this method should only be used for development purposes. It is recommended to fill in the credentials using User Secrets, as shown in Step 1.
8. Add currency formats in the Shopify configuration module. It is recommended to use [custom numberic format strings](https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings).


### Dancing Goat example - setup
1. Go to `./examples/DancingGoat-Shopify` folder and run CI restore for content types, product related pages and shopping cart page
```powershell
dotnet run --kxp-ci-restore
```
All content types and custom activities for e-ecommerce events are created.

2. Start Xperience By Kentico Dancing Goat application (`DancingGoat` in `.\examples`) configured with your own database and wait for product synchronization finish. Check `Shopify product synchronization done.` message in event log.

3. Create pages for Store using page types from this [this list](#e-commerce-page-types).
