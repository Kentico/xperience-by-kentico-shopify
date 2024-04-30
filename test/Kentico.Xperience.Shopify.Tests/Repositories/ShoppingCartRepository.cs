using Kentico.Xperience.Shopify.ShoppingCart;
using Kentico.Xperience.Shopify.ShoppingCart.GraphQLModels;
using ShopifySharp.GraphQL;

namespace Kentico.Xperience.Shopify.Tests.Repositories
{
    internal class ShoppingCartRepository
    {
        public IEnumerable<CartObjectModel> Carts { get; set; }

        public ShoppingCartRepository()
        {
            Carts =
            [
                GenerateCart1(),
                GenerateCart2(),
                GenerateCart3(),
                GenerateCart4(),
                GenerateCartWithoutLines()
            ];
        }

        private CartObjectModel GenerateCart1()
        {
            // Create VariantProduct instances
            var product1 = new VariantProduct { Title = "Panama Los Lajones Honey" };
            var product2 = new VariantProduct { Title = "Kenya Gakuyuni AA" };

            // Create Merchandise instances
            var merchandise1 = new Merchandise { Id = "gid://shopify/ProductVariant/47401942581544", Title = "2 lb", Product = product1 };
            var merchandise2 = new Merchandise { Id = "gid://shopify/ProductVariant/47401957785896", Title = "Default Title", Product = product2 };

            // Create CartCost instances
            var totalAmount1 = new PriceDto { Amount = 3267.0m, CurrencyCode = CurrencyCode.CZK };
            var totalAmount2 = new PriceDto { Amount = 1272.0m, CurrencyCode = CurrencyCode.CZK };
            var subtotalAmount = new PriceDto { Amount = 4539.0m, CurrencyCode = CurrencyCode.CZK };

            var cost1 = new CartCost { TotalAmount = totalAmount1 };
            var cost2 = new CartCost { TotalAmount = totalAmount2 };

            // Create CartLineNode instances
            var node1 = new CartLineNode { Id = "gid://shopify/CartLine/244da069-6219-4943-8741-a0c8690d461c?cart=Z2NwLXVzLWVhc3QxOjAxSFM5NUFDWUI4OTQ0N05HUVdKU0hXVFlN", Quantity = 3, Cost = cost1, Merchandise = merchandise1 };
            var node2 = new CartLineNode { Id = "gid://shopify/CartLine/efb4b873-08b8-48dc-94d4-b026fdebfa07?cart=Z2NwLXVzLWVhc3QxOjAxSFM5NUFDWUI4OTQ0N05HUVdKU0hXVFlN", Quantity = 3, Cost = cost2, Merchandise = merchandise2 };

            // Create CartLineEdge instances
            var edge1 = new CartLineEdge { Node = node1 };
            var edge2 = new CartLineEdge { Node = node2 };

            // Create CartLines instance
            var lines = new CartLines { Edges = [edge1, edge2] };

            // Create CartCost instance
            var cost = new CartCost { TotalAmount = totalAmount1, SubtotalAmount = subtotalAmount };

            // Create CartObjectModel instance
            return new CartObjectModel
            {
                Id = "gid://shopify/Cart/Z2NwLXVzLWVhc3QxOjAxSFM5NUFDWUI4OTQ0N05HUVdKU0hXVFlN",
                TotalQuantity = 6,
                CheckoutUrl = "https://quickstart-3b0f1a15.myshopify.com/cart/c/Z2NwLXVzLWVhc3QxOjAxSFM5NUFDWUI4OTQ0N05HUVdKU0hXVFlN?key=ca0127810e60f064542c8a70ee304b76",
                Cost = cost,
                Lines = lines
            };
        }


        private CartObjectModel GenerateCart2()
        {
            // Create VariantProduct instances
            var product1 = new VariantProduct { Title = "Another Product Title" };
            var product2 = new VariantProduct { Title = "Yet Another Product Title" };

            // Create Merchandise instances
            var merchandise1 = new Merchandise { Id = "gid://shopify/ProductVariant/123456789", Title = "Another Variant Title", Product = product1 };
            var merchandise2 = new Merchandise { Id = "gid://shopify/ProductVariant/987654321", Title = "Yet Another Variant Title", Product = product2 };

            // Create CartCost instances
            var totalAmount1 = new PriceDto { Amount = 1000.0m, CurrencyCode = CurrencyCode.USD };
            var totalAmount2 = new PriceDto { Amount = 500.0m, CurrencyCode = CurrencyCode.USD };
            var subtotalAmount = new PriceDto { Amount = 1500.0m, CurrencyCode = CurrencyCode.USD };

            var cost1 = new CartCost { TotalAmount = totalAmount1 };
            var cost2 = new CartCost { TotalAmount = totalAmount2 };

            // Create CartLineNode instances
            var node1 = new CartLineNode { Id = "gid://shopify/CartLine/11111111-1111-1111-1111-111111111111", Quantity = 2, Cost = cost1, Merchandise = merchandise1 };
            var node2 = new CartLineNode { Id = "gid://shopify/CartLine/22222222-2222-2222-2222-222222222222", Quantity = 1, Cost = cost2, Merchandise = merchandise2 };

            // Create CartLineEdge instances
            var edge1 = new CartLineEdge { Node = node1 };
            var edge2 = new CartLineEdge { Node = node2 };

            // Create CartLines instance
            var lines = new CartLines { Edges = new List<CartLineEdge> { edge1, edge2 } };

            // Create CartCost instance
            var cost = new CartCost { TotalAmount = totalAmount1, SubtotalAmount = subtotalAmount };

            // Create CartObjectModel instance
            return new CartObjectModel
            {
                Id = "gid://shopify/Cart/987654321",
                TotalQuantity = 3,
                CheckoutUrl = "https://example.com/cart/checkout",
                Cost = cost,
                Lines = lines
            };
        }


        private CartObjectModel GenerateCart3()
        {
            // Create VariantProduct instances
            var product1 = new VariantProduct { Title = "Product X" };
            var product2 = new VariantProduct { Title = "Product Y" };

            // Create Merchandise instances
            var merchandise1 = new Merchandise { Id = "gid://shopify/ProductVariant/1234567890", Title = "Variant X", Product = product1 };
            var merchandise2 = new Merchandise { Id = "gid://shopify/ProductVariant/9876543210", Title = "Variant Y", Product = product2 };

            // Create CartCost instances
            var totalAmount1 = new PriceDto { Amount = 500.0m, CurrencyCode = CurrencyCode.USD };
            var totalAmount2 = new PriceDto { Amount = 200.0m, CurrencyCode = CurrencyCode.USD };
            var subtotalAmount = new PriceDto { Amount = 700.0m, CurrencyCode = CurrencyCode.USD };

            var cost1 = new CartCost { TotalAmount = totalAmount1 };
            var cost2 = new CartCost { TotalAmount = totalAmount2 };

            // Create CartLineNode instances
            var node1 = new CartLineNode { Id = "gid://shopify/CartLine/33333333-3333-3333-3333-333333333333", Quantity = 1, Cost = cost1, Merchandise = merchandise1 };
            var node2 = new CartLineNode { Id = "gid://shopify/CartLine/44444444-4444-4444-4444-444444444444", Quantity = 2, Cost = cost2, Merchandise = merchandise2 };

            // Create CartLineEdge instances
            var edge1 = new CartLineEdge { Node = node1 };
            var edge2 = new CartLineEdge { Node = node2 };

            // Create CartLines instance
            var lines = new CartLines { Edges = new List<CartLineEdge> { edge1, edge2 } };

            // Create CartCost instance
            var cost = new CartCost { TotalAmount = totalAmount1, SubtotalAmount = subtotalAmount };

            // Create CartObjectModel instance
            return new CartObjectModel
            {
                Id = "gid://shopify/Cart/1234567890",
                TotalQuantity = 3,
                CheckoutUrl = "https://example.com/cart/checkout",
                Cost = cost,
                Lines = lines
            };
        }


        private CartObjectModel GenerateCart4()
        {
            // Create VariantProduct instances
            var product1 = new VariantProduct { Title = "Product A" };
            var product2 = new VariantProduct { Title = "Product B" };

            // Create Merchandise instances
            var merchandise1 = new Merchandise { Id = "gid://shopify/ProductVariant/1111111111", Title = "Variant A", Product = product1 };
            var merchandise2 = new Merchandise { Id = "gid://shopify/ProductVariant/2222222222", Title = "Variant B", Product = product2 };

            // Create CartCost instances
            var totalAmount1 = new PriceDto { Amount = 700.0m, CurrencyCode = CurrencyCode.USD };
            var totalAmount2 = new PriceDto { Amount = 300.0m, CurrencyCode = CurrencyCode.USD };
            var subtotalAmount = new PriceDto { Amount = 1000.0m, CurrencyCode = CurrencyCode.USD };

            var cost1 = new CartCost { TotalAmount = totalAmount1 };
            var cost2 = new CartCost { TotalAmount = totalAmount2 };

            // Create CartLineNode instances
            var node1 = new CartLineNode { Id = "gid://shopify/CartLine/55555555-5555-5555-5555-555555555555", Quantity = 2, Cost = cost1, Merchandise = merchandise1 };
            var node2 = new CartLineNode { Id = "gid://shopify/CartLine/66666666-6666-6666-6666-666666666666", Quantity = 1, Cost = cost2, Merchandise = merchandise2 };

            // Create CartLineEdge instances
            var edge1 = new CartLineEdge { Node = node1 };
            var edge2 = new CartLineEdge { Node = node2 };

            // Create CartLines instance
            var lines = new CartLines { Edges = new List<CartLineEdge> { edge1, edge2 } };

            // Create CartCost instance
            var cost = new CartCost { TotalAmount = totalAmount1, SubtotalAmount = subtotalAmount };

            // Create CartObjectModel instance
            return new CartObjectModel
            {
                Id = "gid://shopify/Cart/6789012345",
                TotalQuantity = 3,
                CheckoutUrl = "https://example.com/cart/checkout",
                Cost = cost,
                Lines = lines
            };
        }

        private CartObjectModel GenerateCartWithoutLines()
        {
            // Create CartCost instance
            var totalAmount = new PriceDto { Amount = 0.0m, CurrencyCode = CurrencyCode.USD };
            var cost = new CartCost { TotalAmount = totalAmount };

            // Create CartObjectModel instance without lines
            return new CartObjectModel
            {
                Id = "gid://shopify/Cart/0000000000",
                TotalQuantity = 0,
                CheckoutUrl = "https://example.com/cart/checkout",
                Cost = cost,
                Lines = null // No cart lines
            };
        }
    }
}
