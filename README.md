# Xperiene by Kentico Shopify integration

[![Kentico Labs](https://img.shields.io/badge/Kentico_Labs-grey?labelColor=orange&logo=data:image/svg+xml;base64,PHN2ZyBjbGFzcz0ic3ZnLWljb24iIHN0eWxlPSJ3aWR0aDogMWVtOyBoZWlnaHQ6IDFlbTt2ZXJ0aWNhbC1hbGlnbjogbWlkZGxlO2ZpbGw6IGN1cnJlbnRDb2xvcjtvdmVyZmxvdzogaGlkZGVuOyIgdmlld0JveD0iMCAwIDEwMjQgMTAyNCIgdmVyc2lvbj0iMS4xIiB4bWxucz0iaHR0cDovL3d3dy53My5vcmcvMjAwMC9zdmciPjxwYXRoIGQ9Ik05NTYuMjg4IDgwNC40OEw2NDAgMjc3LjQ0VjY0aDMyYzE3LjYgMCAzMi0xNC40IDMyLTMycy0xNC40LTMyLTMyLTMyaC0zMjBjLTE3LjYgMC0zMiAxNC40LTMyIDMyczE0LjQgMzIgMzIgMzJIMzg0djIxMy40NEw2Ny43MTIgODA0LjQ4Qy00LjczNiA5MjUuMTg0IDUxLjIgMTAyNCAxOTIgMTAyNGg2NDBjMTQwLjggMCAxOTYuNzM2LTk4Ljc1MiAxMjQuMjg4LTIxOS41MnpNMjQxLjAyNCA2NDBMNDQ4IDI5NS4wNFY2NGgxMjh2MjMxLjA0TDc4Mi45NzYgNjQwSDI0MS4wMjR6IiAgLz48L3N2Zz4=)](https://github.com/Kentico/.github/blob/main/SUPPORT.md#labs-limited-support) [![CI: Build and Test](https://github.com/Kentico/xperience-by-kentico-shopify/actions/workflows/ci.yml/badge.svg?branch=main)](https://github.com/Kentico/xperience-by-kentico-shopify/actions/workflows/ci.yml)

## Description
Repository contains solution with Xperience By Kentico integration to Shopify. It shows the connection to the Shopify headless API and shows the implementation of a simple e-shop on XByK (extended Dancing Goat sample site). The solution consists of these parts:
- Kentico.Xperience.Shopify - class library that contains all services necessary for this integration.
- Kentico.Xperience.Shopify.Rcl - razor class library for selector components(used in standalone product listing widget).
- DancingGoat - Sample Dancing Goat site.
- [Kentico.Xperience.Ecommerce.Common](https://github.com/Kentico/xperience-by-kentico-ecommerce-common) - common library for e-commerce integrations.

## Screenshots

![Products in content hub](./images/screenshots/products_content_hub.jpg "Products in content hub")

## Library Version Matrix

Summary of libraries which are supported by the following versions Xperince by Kentico / Kentico Xperience 13

### Shopify integration

| Library                            | Xperience Version | Library Version |
|----------------------------------- |-------------------| --------------- |
| Kentico.Xperience.Ecommerce.Common | \>= 28.2.1        | 1.0.0           |
| Kentico.Xperience.Shopify          | \>= 29.0.2        | 1.0.0           |
| Kentico.Xperience.Shopify.Rcl      | \>= 29.0.2        | 1.0.0           |

### Dependencies
- [ASP.NET Core 8.0](https://dotnet.microsoft.com/en-us/download)
- [Xperience by Kentico](https://docs.xperience.io/xp/changelog)

## Package Installation
Add these packages to your XbyK application using the .NET CLI

```powershell
dotnet add package Kentico.Xperience.Shopify
dotnet add package Kentico.Xperience.Shopify.Rcl
```

## Quick Start
1. Fill settings to connect your Shopify instance(optional)
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
2. Add library to the application services
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
## Full Instructions

View the [Usage Guide](./docs/Usage-Guide.md) for more detailed instructions.

## Contributing

To see the guidelines for Contributing to Kentico open source software, please see [Kentico's `CONTRIBUTING.md`](https://github.com/Kentico/.github/blob/main/CONTRIBUTING.md) for more information and follow the [Kentico's `CODE_OF_CONDUCT`](https://github.com/Kentico/.github/blob/main/CODE_OF_CONDUCT.md).

Instructions and technical details for contributing to **this** project can be found in [Contributing Setup](./docs/Contributing-Setup.md).

## License

Distributed under the MIT License. See [`LICENSE.md`](./LICENSE.md) for more information.

## Support

This contribution has __Kentico Labs limited support__.

See [`SUPPORT.md`](https://github.com/Kentico/.github/blob/main/SUPPORT.md#labs-limited-support) for more information.

For any security issues see [`SECURITY.md`](https://github.com/Kentico/.github/blob/main/SECURITY.md).
