using Moq.AutoMock;

using Kentico.Xperience.Shopify.ShoppingCart;
using Microsoft.AspNetCore.Http;

using Kentico.Xperience.Shopify.Tests.Mocks;
using GraphQL.Client.Abstractions;
using ShopifySharp.GraphQL;
using Kentico.Xperience.Shopify.Tests.Repositories;
using Kentico.Xperience.Shopify.Activities;

namespace Kentico.Xperience.Shopify.Tests
{
    /// <summary>
    /// <see cref="ShoppingService"/> unit tests.
    /// </summary>
    [TestFixture]
    public class ShoppingServiceTests
    {
        private readonly AutoMocker mocker;
        private IShoppingCartRepository CartRepository { get; set; }
        private DiscountCodesRepository DiscountCodesRepository { get; set; }


        /// <summary>
        /// Create <see cref="ShoppingServiceTests"/> instance.
        /// </summary>
        public ShoppingServiceTests()
        {
            mocker = new AutoMocker();
        }


        /// <summary>
        /// One time tests setup. Add mocks to dependency injection container.
        /// </summary>
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            mocker.Use<IShoppingCartCacheService>(new ShoppingCartCacheServiceMock());
            mocker.Use<IEcommerceActivityLogger>(new EcommerceActivityLoggerMock());
        }


        /// <summary>
        /// Set up before each test.
        /// Create shopping cart and discount codes repository mocks.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            CartRepository = new ShoppingCartRepository();
            DiscountCodesRepository = new DiscountCodesRepository();
            mocker.Use(CartRepository);
            mocker.Setup<IGraphQLHttpClientFactory, IGraphQLClient>(c => c.CreateGraphQLHttpClient()).Returns(new GraphQLHttpClientMock(CartRepository));
        }


        /// <summary>
        /// Get existing shopping cart stored in session.
        /// </summary>
        [Test]
        public async Task GetCurrentShoppingCart_ExistingCartIdInSession_ShouldReturnCart()
        {
            var shoppingCart = new ShoppingCartInfo(CartRepository.Carts.First());
            SetHttpContext(shoppingCart);

            var shoppingService = mocker.CreateInstance<ShoppingService>();
            var returnedCart = await shoppingService.GetCurrentShoppingCart();

            if (returnedCart is null)
            {
                AssertCartIsNull();
                return;
            }


            Assert.Multiple(() =>
            {
                Assert.That(returnedCart is not null);
                Assert.That(returnedCart!.CartId, Is.EqualTo(shoppingCart.CartId));
            });
        }


        /// <summary>
        /// Get shopping cart without any data stored in session.
        /// </summary>
        [Test]
        public async Task GetCurrentShoppingCart_NoCartIdInSession_ShouldReturnNull()
        {
            SetHttpContext();
            var shoppingService = mocker.CreateInstance<ShoppingService>();
            var returnedCart = await shoppingService.GetCurrentShoppingCart();

            Assert.That(returnedCart, Is.Null);
        }


        /// <summary>
        /// Set negative quantity to shopping cart item.
        /// </summary>
        [Test]
        public async Task UpdateCartItem_SetNegativeQuantity_ShouldRemoveCartItem()
        {
            var shoppingCart = new ShoppingCartInfo(CartRepository.Carts.First());
            SetHttpContext(shoppingCart);

            var shoppingService = mocker.CreateInstance<ShoppingService>();

            var cartItemsList = shoppingCart.Items.ToList();
            var itemToUpdate = cartItemsList[0];
            cartItemsList.Remove(itemToUpdate);

            var retrievedCart = (await shoppingService.UpdateCartItem(new ShoppingCartItemParameters()
            {
                Country = CountryCode.CZ,
                Quantity = -1,
                ShoppingCartItemID = itemToUpdate.ShopifyCartItemId
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


        /// <summary>
        /// Try remove shopping cart item when no shopping cart is stored in session.
        /// </summary>
        [Test]
        public async Task RemoveCartItem_NoCartIdInSession_ShouldRetrunNull()
        {
            SetHttpContext();
            var itemToRemove = new ShoppingCartInfo(CartRepository.Carts.First()).Items.First();
            var shoppingService = mocker.CreateInstance<ShoppingService>();

            var result = await shoppingService.RemoveCartItem(itemToRemove.VariantGraphQLId);
            Assert.Multiple(() =>
            {
                Assert.That(result.Success, Is.True);
                Assert.That(result.Cart, Is.Null);
            });
        }


        /// <summary>
        /// Update non existing shopping cart item.
        /// </summary>
        [Test]
        public async Task UpdateCartItem_AddNewCartItem_ShouldAddItemToCart()
        {
            var shoppingCart = new ShoppingCartInfo(CartRepository.Carts.First());
            SetHttpContext(shoppingCart);
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
                AssertCartIsNull();
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


        /// <summary>
        /// Update existing shopping cart item.
        /// </summary>
        [Test]
        public async Task UpdateCartItem_UpdatedProductAlreadyInCart_ShouldUpdateExistingCartItem()
        {
            var shoppingCart = new ShoppingCartInfo(CartRepository.Carts.First());
            SetHttpContext(shoppingCart);
            var shoppingService = mocker.CreateInstance<ShoppingService>();

            var cartItemsList = shoppingCart.Items.ToList();
            var itemToUpdate = cartItemsList[0];
            int newQuantity = itemToUpdate.Quantity + 2;

            var retrievedCart = (await shoppingService.UpdateCartItem(new ShoppingCartItemParameters()
            {
                Country = CountryCode.CZ,
                Quantity = newQuantity,
                ShoppingCartItemID = itemToUpdate.ShopifyCartItemId
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


        /// <summary>
        /// Remove existing shopping cart item.
        /// </summary>
        [Test]
        public async Task RemoveCartItem_CartContainsItem_ShouldRemoveCartItem()
        {
            var shoppingCart = new ShoppingCartInfo(CartRepository.Carts.First());
            SetHttpContext(shoppingCart);
            var shoppingService = mocker.CreateInstance<ShoppingService>();
            var cartItemsList = shoppingCart.Items.ToList();
            var itemToRemove = cartItemsList[0];

            var retrievedCart = (await shoppingService.RemoveCartItem(itemToRemove.ShopifyCartItemId)).Cart;
            cartItemsList.Remove(itemToRemove);

            Assert.Multiple(() =>
            {
                Assert.That(retrievedCart, Is.Not.Null);

                var retrievedCartItems = retrievedCart!.Items.ToList();
                Assert.That(retrievedCartItems, Has.Count.EqualTo(cartItemsList.Count));
                Assert.That(retrievedCartItems.Exists(x => x.ShopifyCartItemId == itemToRemove.ShopifyCartItemId), Is.False);
            });
        }


        /// <summary>
        /// Remove cart items with the same product variant GraphQL ID.
        /// </summary>
        [Test]
        public async Task RemoveCartProductVariant_CartContainsVariant_ShouldRemoveCartItem()
        {
            var shoppingCart = new ShoppingCartInfo(CartRepository.CartWithSameProductVariant);
            SetHttpContext(shoppingCart);
            var shoppingService = mocker.CreateInstance<ShoppingService>();
            var cartItemsList = shoppingCart.Items.ToList();
            var duplicitItem = cartItemsList[0];

            var retrievedCart = (await shoppingService.RemoveProductVariantFromCart(duplicitItem.VariantGraphQLId)).Cart;
            cartItemsList.RemoveAll(x => x.VariantGraphQLId == duplicitItem.VariantGraphQLId);

            Assert.Multiple(() =>
            {
                Assert.That(retrievedCart, Is.Not.Null);

                var retrievedCartItems = retrievedCart!.Items.ToList();
                Assert.That(retrievedCartItems, Has.Count.EqualTo(cartItemsList.Count));
                Assert.That(retrievedCartItems.Exists(x => x.VariantGraphQLId == duplicitItem.VariantGraphQLId), Is.False);
            });
        }


        /// <summary>
        /// Remove non existing shopping cart item.
        /// </summary>
        [Test]
        public async Task RemoveCartItem_CartDoesNotContainItem_ShouldReturnUnmodifiedCart()
        {
            var shoppingCart = new ShoppingCartInfo(CartRepository.Carts.First());
            SetHttpContext(shoppingCart);
            var shoppingService = mocker.CreateInstance<ShoppingService>();
            var cart = (await shoppingService.RemoveCartItem("NonExistingMerchandiseID")).Cart;

            if (cart is null)
            {
                AssertCartIsNull();
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


        /// <summary>
        /// Add new shopping cart item to existing shopping cart.
        /// </summary>
        [Test]
        public async Task AddItemToCart_ExistingCartIdStoredInSession_ShouldAddItemToCart()
        {
            var shoppingCart = new ShoppingCartInfo(CartRepository.Carts.First());
            SetHttpContext(shoppingCart);
            var shoppingService = mocker.CreateInstance<ShoppingService>();
            var itemToAdd = new ShoppingCartItem()
            {
                Name = "New added product",
                Quantity = 2,
                Price = 10m,
                DiscountedAmount = 0m,
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


        /// <summary>
        /// Add product to shopping cart when no shopping cart is stored in session.
        /// </summary>
        [Test]
        public async Task AddItemToCart_NoExistingCartIdInSession_ShouldReturnNewCartWithProduct()
        {
            SetHttpContext();
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


        /// <summary>
        /// Add discount code to existing shopping cart.
        /// </summary>
        [Test]
        public async Task AddDiscountCode_ExistingCartIdInSession_ShouldAddDiscountCode()
        {
            string discountCode = DiscountCodesRepository.DiscountCodes.First();
            var shoppingCart = new ShoppingCartInfo(CartRepository.Carts.First());
            SetHttpContext(shoppingCart);
            var shoppingService = mocker.CreateInstance<ShoppingService>();
            var result = await shoppingService.AddDiscountCode(discountCode);
            var cart = result?.Cart;

            if (cart is null)
            {
                AssertCartIsNull();
                return;
            }

            Assert.Multiple(() =>
            {
                Assert.That(cart, Is.Not.Null);
                string[] discountCodes = cart.DiscountCodes.ToArray();
                Assert.That(discountCodes, Has.Length.EqualTo(1));
                Assert.That(discountCodes[0], Is.EqualTo(discountCode));
            });
        }


        /// <summary>
        /// Add invalid discount code to existing shopping cart.
        /// </summary>
        [Test]
        public async Task AddDiscountCode_NonExistingDiscountCode_ShouldIgnoreDiscountCode()
        {
            string discountCode = DiscountCodesRepository.NonExistingDiscountCode;
            var shoppingCart = new ShoppingCartInfo(CartRepository.Carts.First());
            SetHttpContext(shoppingCart);
            var shoppingService = mocker.CreateInstance<ShoppingService>();
            var result = await shoppingService.AddDiscountCode(discountCode);
            var cart = result?.Cart;

            if (cart is null)
            {
                AssertCartIsNull();
                return;
            }

            Assert.Multiple(() =>
            {
                Assert.That(cart, Is.Not.Null);
                Assert.That(cart.DiscountCodes.ToArray(), Is.Empty);
            });
        }


        /// <summary>
        /// Add valid discount code to the cart that already contains the same discount code.
        /// </summary>
        [Test]
        public async Task AddDiscountCode_CartAlreadyContainsAnotherCode_ShouldAddDiscountCode()
        {
            var shoppingCart = new ShoppingCartInfo(CartRepository.CartWithDiscountCode);
            string discountCode = DiscountCodesRepository.DiscountCodes.First(x => !shoppingCart.DiscountCodes.Contains(x));
            int originalDiscountsCount = shoppingCart.DiscountCodes.Count();

            SetHttpContext(shoppingCart);
            var shoppingService = mocker.CreateInstance<ShoppingService>();
            var result = await shoppingService.AddDiscountCode(discountCode);
            var cart = result?.Cart;

            if (cart is null)
            {
                AssertCartIsNull();
                return;
            }

            Assert.Multiple(() =>
            {
                Assert.That(cart, Is.Not.Null);
                string[] discountCodes = cart.DiscountCodes.ToArray();
                Assert.That(discountCodes, Has.Length.EqualTo(originalDiscountsCount + 1));
                Assert.That(discountCodes, Has.Member(discountCode));
            });
        }


        /// <summary>
        /// Add invalid discount code to shopping cart.
        /// </summary>
        [Test]
        public async Task AddNonExistingDiscountCode_CartHasAnotherDiscountCode_ShouldIgnoreCode()
        {
            string discountCode = DiscountCodesRepository.NonExistingDiscountCode;
            var shoppingCart = new ShoppingCartInfo(CartRepository.CartWithDiscountCode);
            int originalDiscountsCount = shoppingCart.DiscountCodes.Count();

            SetHttpContext(shoppingCart);
            var shoppingService = mocker.CreateInstance<ShoppingService>();
            var result = await shoppingService.AddDiscountCode(discountCode);
            var cart = result?.Cart;

            if (cart is null)
            {
                AssertCartIsNull();
                return;
            }

            Assert.Multiple(() =>
            {
                Assert.That(cart, Is.Not.Null);
                Assert.That(cart.DiscountCodes.ToArray(), Has.Length.EqualTo(originalDiscountsCount));
            });
        }


        /// <summary>
        /// Remove valid discount code from existing shopping cart.
        /// </summary>
        [Test]
        public async Task RemoveExistingDiscountCode_CartContainsDiscountCode_ShouldRemoveCode()
        {
            var shoppingCart = new ShoppingCartInfo(CartRepository.CartWithDiscountCode);
            int originalDiscountCodesLength = shoppingCart.DiscountCodes.Count();
            SetHttpContext(shoppingCart);

            var shoppingService = mocker.CreateInstance<ShoppingService>();
            string discountCodeToRemove = shoppingCart.DiscountCodes.First();
            var result = await shoppingService.RemoveDiscountCode(discountCodeToRemove);

            var cart = result?.Cart;

            if (cart is null)
            {
                AssertCartIsNull();
                return;
            }

            Assert.Multiple(() =>
            {
                Assert.That(cart.DiscountCodes, Has.No.Member(discountCodeToRemove));
                Assert.That(cart.DiscountCodes.ToArray(), Has.Length.EqualTo(originalDiscountCodesLength - 1));
            });
        }


        /// <summary>
        /// Remove non-existing discount code from shopping cart.
        /// </summary>
        [Test]
        public async Task RemoveNonExistingDiscountCode_CartContainsAnotherDiscountCode_ShouldIgnoreCode()
        {
            var shoppingCart = new ShoppingCartInfo(CartRepository.CartWithDiscountCode);
            int originalDiscountCodesLength = shoppingCart.DiscountCodes.Count();
            SetHttpContext(shoppingCart);

            var shoppingService = mocker.CreateInstance<ShoppingService>();
            string discountCodeToRemove = DiscountCodesRepository.NonExistingDiscountCode;
            var result = await shoppingService.RemoveDiscountCode(discountCodeToRemove);

            var cart = result?.Cart;

            if (cart is null)
            {
                AssertCartIsNull();
                return;
            }

            Assert.Multiple(() =>
            {
                Assert.That(cart.DiscountCodes, Has.No.Member(discountCodeToRemove));
                Assert.That(cart.DiscountCodes.ToArray(), Has.Length.EqualTo(originalDiscountCodesLength));
            });
        }


        /// <summary>
        /// Remove whole shopping cart from session.
        /// </summary>
        [Test]
        public async Task RemoveCurrentShoppingCart_CartExists_ShouldRemoveCart()
        {
            SetHttpContext(new ShoppingCartInfo(CartRepository.Carts.First()));
            var shoppingService = mocker.CreateInstance<ShoppingService>();

            var shoppingCart = await shoppingService.GetCurrentShoppingCart();
            if (shoppingCart == null)
            {
                AssertCartIsNull();
                return;
            }

            shoppingService.RemoveCurrentShoppingCart();
            shoppingCart = await shoppingService.GetCurrentShoppingCart();

            Assert.That(shoppingCart, Is.Null);
        }


        private void AssertCartIsNull() => Assert.Fail("Returned shopping cart was null");


        private void SetHttpContext(ShoppingCartInfo shoppingCart) =>
            mocker.Setup<IHttpContextAccessor, HttpContext?>(s => s.HttpContext).Returns(new HttpContextMock(shoppingCart.CartId));

        private void SetHttpContext() =>
            mocker.Setup<IHttpContextAccessor, HttpContext?>(s => s.HttpContext).Returns(new HttpContextMock(string.Empty));
    }
}
