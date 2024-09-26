using CMS.ContactManagement;
using CMS.DataEngine;
using CMS.Globalization;

using DancingGoat;
using DancingGoat.Controllers.Shopify;
using DancingGoat.Models;

using Kentico.Content.Web.Mvc.Routing;
using Kentico.Xperience.Shopify.Activities;
using Kentico.Xperience.Shopify.Orders;
using Kentico.Xperience.Shopify.ShoppingCart;

using Microsoft.AspNetCore.Mvc;

using ShopifySharp;

[assembly: RegisterWebPageRoute(ThankYouPage.CONTENT_TYPE_NAME, typeof(ShopifyThankYouController), WebsiteChannelNames = new[] { DancingGoatConstants.WEBSITE_CHANNEL_NAME })]

namespace DancingGoat.Controllers.Shopify
{
    public class ShopifyThankYouController : Controller
    {
        private readonly IShopifyOrderService orderService;
        private readonly IInfoProvider<CountryInfo> countryInfoProvider;
        private readonly IShoppingService shoppingService;
        private readonly IEcommerceActivityLogger activityLogger;


        public ShopifyThankYouController(
            IShopifyOrderService orderService,
            IInfoProvider<CountryInfo> countryInfoProvider,
            IShoppingService shoppingService,
            IEcommerceActivityLogger activityLogger)
        {
            this.orderService = orderService;
            this.countryInfoProvider = countryInfoProvider;
            this.shoppingService = shoppingService;
            this.activityLogger = activityLogger;
        }


        public async Task<IActionResult> Index(long orderId)
        {
            var cart = await shoppingService.GetCurrentShoppingCart();
            var order = await orderService.GetOrder(orderId);

            if (order != null && cart != null)
            {
                UpdateCurrentContact(order);
                LogPurchaseActivity(order, cart);
            }

            if (order == null)
            {
                return NotFound();
            }

            shoppingService.RemoveCurrentShoppingCart();

            var model = ThankYouPageViewModel.GetModel(order);

            return View(model);
        }


        private void LogPurchaseActivity(Order order, ShoppingCartInfo cart)
        {
            activityLogger.LogPurchaseActivity(order.TotalPriceSet.PresentmentMoney.Amount ?? 0, order?.Id ?? 0, order.PresentmentCurrency);
            foreach (var lineItem in cart.Items)
            {
                activityLogger.LogPurchasedProductActivity(lineItem);
            }
        }


        private void UpdateCurrentContact(Order order)
        {
            var currentContact = ContactManagementContext.CurrentContact;
            if (currentContact == null || order == null)
            {
                return;
            }

            var customerDetails = order.Customer;

            currentContact.ContactFirstName = customerDetails.FirstName;
            currentContact.ContactLastName = customerDetails.LastName;
            currentContact.ContactEmail = customerDetails.Email;
            currentContact.ContactAddress1 = customerDetails.DefaultAddress.Address1;
            currentContact.ContactCity = customerDetails.DefaultAddress.City;
            currentContact.ContactZIP = customerDetails.DefaultAddress.Zip;

            if (!string.IsNullOrEmpty(customerDetails.DefaultAddress.CountryCode))
            {
                var country = countryInfoProvider.Get()
                .TopN(1)
                .WhereEquals(nameof(CountryInfo.CountryTwoLetterCode), customerDetails.DefaultAddress.CountryCode)
                .FirstOrDefault();

                currentContact.ContactCountryID = country?.CountryID ?? 0;
            }

            currentContact.Update();
        }
    }
}
