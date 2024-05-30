using Kentico.Xperience.Shopify.ShoppingCart;

namespace Kentico.Xperience.Shopify.Tests.Repositories
{
    internal interface IShoppingCartRepository
    {
        public IEnumerable<CartObjectModel> Carts { get; set; }

        public CartObjectModel CartWithDiscountCode { get; set; }

        public CartObjectModel CartWithSameProductVariant { get; set; }
    }
}
