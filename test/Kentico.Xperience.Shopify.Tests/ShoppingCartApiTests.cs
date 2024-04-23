using Moq.AutoMock;

using Kentico.Xperience.Shopify.ShoppingCart;
using Microsoft.AspNetCore.Http;

using Kentico.Xperience.Shopify.Tests.Mocks;
using GraphQL.Client.Abstractions;
using ShopifySharp.GraphQL;
using Kentico.Xperience.Shopify.Tests.Repositories;

namespace Kentico.Xperience.Shopify.Tests
{
    [TestFixture]
    public class ShoppingCartApiTests
    {
        private readonly AutoMocker mocker;

        public ShoppingCartApiTests()
        {
            mocker = new AutoMocker();
        }


        [OneTimeSetUp]
        public void Setup()
        {
            mocker.Setup<IGraphQLHttpClientFactory, IGraphQLClient>(c => c.CreateGraphQLHttpClient()).Returns(new GraphQLHttpClientMock());
            mocker.Use<IShoppingCartCacheService>(new ShoppingCartCacheServiceMock());
        }


        [Test]
        public async Task GetCurrentShoppingCart_Should_Return_Cart()
        {
            var shoppingCart = new ShoppingCartInfo(new ShoppingCartRepository().Carts.First());
            mocker.Setup<IHttpContextAccessor, HttpContext?>(s => s.HttpContext).Returns(new HttpContextMock(shoppingCart.CartId));

            var shoppingService = mocker.CreateInstance<ShoppingService>();
            var returnedCart = await shoppingService.GetCurrentShoppingCart();

            Assert.Multiple(() =>
            {
                Assert.That(returnedCart is not null);
                Assert.That(returnedCart!.CartId, Is.EqualTo(shoppingCart!.CartId));
            });
        }


        [Test]
        public async Task GetCurrentShoppingCart_Should_Retrun_Null()
        {
            mocker.Setup<IHttpContextAccessor, HttpContext?>(s => s.HttpContext).Returns(new HttpContextMock(string.Empty));
            var shoppingService = mocker.CreateInstance<ShoppingService>();
            var returnedCart = await shoppingService.GetCurrentShoppingCart();

            Assert.That(returnedCart, Is.Null);
        }


        [Test]
        public async Task ShoppingCartItemUpdateNegativeQuantity_Should_Remove_CartItem()
        {
            var shoppingCart = new ShoppingCartInfo(new ShoppingCartRepository().Carts.First());
            mocker.Setup<IHttpContextAccessor, HttpContext?>(s => s.HttpContext).Returns(new HttpContextMock(shoppingCart.CartId));

            var shoppingService = mocker.CreateInstance<ShoppingService>();

            var cartItemsList = shoppingCart.Items.ToList();
            var itemToUpdate = cartItemsList[0];
            cartItemsList.Remove(itemToUpdate);

            var retrievedCart = (await shoppingService.UpdateCartItem(new ShoppingCartItemParameters()
            {
                Country = CountryCode.CZ,
                Quantity = -1,
                MerchandiseID = itemToUpdate.ShopifyCartItemId
            })).Cart;

            Assert.Multiple(() =>
            {
                Assert.That(retrievedCart is not null);

                var retrievedCartItems = retrievedCart!.Items.ToList();
                Assert.That(retrievedCartItems, Has.Count.EqualTo(cartItemsList.Count));

                foreach (var cartItem in retrievedCartItems)
                {
                    int index = retrievedCartItems.IndexOf(cartItem);
                    Assert.That(cartItem.ShopifyCartItemId, Is.EqualTo(cartItemsList[index].ShopifyCartItemId));
                }
            });
        }


        [Test]
        public async Task NonExistingShoppingCartRemoveItem_Should_Retrun_Null()
        {
            mocker.Setup<IHttpContextAccessor, HttpContext?>(s => s.HttpContext).Returns(new HttpContextMock(string.Empty));
            var itemToRemove = new ShoppingCartInfo(new ShoppingCartRepository().Carts.First()).Items.First();
            var shoppingService = mocker.CreateInstance<ShoppingService>();

            var result = await shoppingService.RemoveCartItem(itemToRemove.VariantGraphQLId);
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Cart, Is.Null);
            });
        }


        [Test]
        public async Task ShoppingCartNonExistingItemUpdate_Should_Add_Item_To_Cart()
        {
            var shoppingCart = new ShoppingCartInfo(new ShoppingCartRepository().Carts.First());
            mocker.Setup<IHttpContextAccessor, HttpContext?>(s => s.HttpContext).Returns(new HttpContextMock(shoppingCart.CartId));
            var itemToAdd = new ShoppingCartItemParameters()
            {
                Country = CountryCode.CZ,
                Quantity = 9,
                MerchandiseID = "BrandNewNonExistingMerchandiseID"
            };
            var shoppingService = mocker.CreateInstance<ShoppingService>();

            var cart = (await shoppingService.UpdateCartItem(itemToAdd)).Cart;

            if (cart is null)
            {
                Assert.Fail("Returned shopping cart was null");
                return;
            }

            Assert.Multiple(() =>
            {
                Assert.That(cart, Is.Not.Null);
                var cartItems = cart.Items.ToList();
                Assert.That(cartItems, Has.Count.EqualTo(shoppingCart.Items.Count() + 1));

                var addedItem = cartItems.Find(x => x.VariantGraphQLId == itemToAdd.MerchandiseID);
                Assert.That(addedItem, Is.Not.Null);
                Assert.That(addedItem?.Quantity, Is.EqualTo(itemToAdd.Quantity));
            });
        }


        [Test]
        public async Task ShoppingCartItemUpdateQuantity_Should_Update_CartItem()
        {
            var shoppingCart = new ShoppingCartInfo(new ShoppingCartRepository().Carts.First());
            mocker.Setup<IHttpContextAccessor, HttpContext?>(s => s.HttpContext).Returns(new HttpContextMock(shoppingCart.CartId));
            var shoppingService = mocker.CreateInstance<ShoppingService>();

            var cartItemsList = shoppingCart.Items.ToList();
            var itemToUpdate = cartItemsList[0];
            int newQuantity = itemToUpdate.Quantity + 2;

            var retrievedCart = (await shoppingService.UpdateCartItem(new ShoppingCartItemParameters()
            {
                Country = CountryCode.CZ,
                Quantity = newQuantity,
                MerchandiseID = itemToUpdate.ShopifyCartItemId
            })).Cart;

            Assert.Multiple(() =>
            {
                Assert.That(retrievedCart, Is.Not.Null);

                var retrievedCartItems = retrievedCart!.Items.ToList();
                Assert.That(retrievedCartItems, Has.Count.EqualTo(cartItemsList.Count));

                var retrievedCartItem = retrievedCartItems.Find(x => x.ShopifyCartItemId == itemToUpdate.ShopifyCartItemId);
                Assert.That(retrievedCartItem, Is.Not.Null);
                Assert.That(retrievedCartItem!.Quantity, Is.EqualTo(newQuantity));
            });
        }


        [Test]
        public async Task ShoppingCartExistingItemRemove_Should_Remove_Item()
        {
            var shoppingCart = new ShoppingCartInfo(new ShoppingCartRepository().Carts.First());
            mocker.Setup<IHttpContextAccessor, HttpContext?>(s => s.HttpContext).Returns(new HttpContextMock(shoppingCart.CartId));
            var shoppingService = mocker.CreateInstance<ShoppingService>();
            var cartItemsList = shoppingCart.Items.ToList();
            var itemToRemove = cartItemsList[0];

            var retrievedCart = (await shoppingService.RemoveCartItem(itemToRemove.VariantGraphQLId)).Cart;
            cartItemsList.Remove(itemToRemove);

            Assert.Multiple(() =>
            {
                Assert.That(retrievedCart, Is.Not.Null);

                var retrievedCartItems = retrievedCart!.Items.ToList();
                Assert.That(retrievedCartItems, Has.Count.EqualTo(cartItemsList.Count));
                Assert.That(retrievedCartItems.Exists(x => x.ShopifyCartItemId == itemToRemove.ShopifyCartItemId), Is.False);
            });
        }


        [Test]
        public async Task ShoppingCartRemoveNonExistingItem_Should_Return_Unmodified_Cart()
        {
            var shoppingCart = new ShoppingCartInfo(new ShoppingCartRepository().Carts.First());
            mocker.Setup<IHttpContextAccessor, HttpContext?>(s => s.HttpContext).Returns(new HttpContextMock(shoppingCart.CartId));
            var shoppingService = mocker.CreateInstance<ShoppingService>();
            var cart = (await shoppingService.RemoveCartItem("NonExistingMerchandiseID")).Cart;

            if (cart is null)
            {
                Assert.Fail("Returned cart cannot be null");
                return;
            }
            var cartItemsList = cart.Items.ToList();

            Assert.Multiple(() =>
            {
                Assert.That(cart.CartId, Is.EqualTo(shoppingCart.CartId));
                Assert.That(cartItemsList, Has.Count.EqualTo(shoppingCart.Items.Count()));

                foreach (var cartItem in cartItemsList)
                {
                    Assert.That(shoppingCart.Items, Has.Some.Property(nameof(ShoppingCartItem.VariantGraphQLId)).EqualTo(cartItem.VariantGraphQLId));
                }
            });
        }


        [Test]
        public async Task ShoppingCartAddNewItem_Should_Add_Item()
        {
            var shoppingCart = new ShoppingCartInfo(new ShoppingCartRepository().Carts.First());
            mocker.Setup<IHttpContextAccessor, HttpContext?>(s => s.HttpContext).Returns(new HttpContextMock(shoppingCart.CartId));
            var shoppingService = mocker.CreateInstance<ShoppingService>();
            var itemToAdd = new ShoppingCartItem()
            {
                Name = "New added product",
                Quantity = 2,
                Price = 10m,
                ShopifyCartItemId = "TestCartItemId",
                VariantGraphQLId = "TestVariantGraphQLId"
            };

            var retrievedCart = (await shoppingService.AddItemToCart(new ShoppingCartItemParameters()
            {
                Country = CountryCode.CZ,
                MerchandiseID = itemToAdd.VariantGraphQLId,
                Quantity = itemToAdd.Quantity
            })).Cart;

            Assert.Multiple(() =>
            {
                Assert.That(retrievedCart, Is.Not.Null);
                var retrievedCartItems = retrievedCart!.Items.ToList();

                Assert.That(retrievedCartItems.Exists(x => x.VariantGraphQLId == itemToAdd.VariantGraphQLId), Is.True);
            });
        }


        [Test]
        public async Task ShoppingCartAddItemWithoutExistingCart_Should_Return_New_Cart()
        {
            mocker.Setup<IHttpContextAccessor, HttpContext?>(s => s.HttpContext).Returns(new HttpContextMock(string.Empty));
            string merchandiseID = "TestMerchandiseID";
            var shoppingService = mocker.CreateInstance<ShoppingService>();
            var itemParams = new ShoppingCartItemParameters()
            {
                Country = CountryCode.CZ,
                Quantity = 2,
                MerchandiseID = merchandiseID
            };

            var cart = (await shoppingService.AddItemToCart(itemParams)).Cart;

            Assert.Multiple(() =>
            {
                Assert.That(cart, Is.Not.Null);
                Assert.That(cart!.Items.Count(), Is.EqualTo(1));

                var cartItem = cart.Items.First();
                Assert.That(cartItem.Quantity, Is.EqualTo(itemParams.Quantity));
                Assert.That(cartItem.VariantGraphQLId, Is.EqualTo(itemParams.MerchandiseID));
            });
        }
    }
}
