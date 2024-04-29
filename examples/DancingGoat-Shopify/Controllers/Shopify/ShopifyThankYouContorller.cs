using DancingGoat.Controllers.Shopify;
using DancingGoat.Models;
using DancingGoat;
using Kentico.Content.Web.Mvc.Routing;
using Microsoft.AspNetCore.Mvc;
using Kentico.Xperience.Shopify.Orders;
using CMS.ContactManagement;
using CMS.Globalization;

[assembly: RegisterWebPageRoute(ThankYouPage.CONTENT_TYPE_NAME, typeof(ShopifyThankYouContorller), WebsiteChannelNames = new[] { DancingGoatConstants.WEBSITE_CHANNEL_NAME }, ActionName = "Index")]

namespace DancingGoat.Controllers.Shopify
{
    public class ShopifyThankYouContorller : Controller
    {
        private readonly IShopifyOrderService orderService;
        private readonly ICountryInfoProvider countryInfoProvider;


        public ShopifyThankYouContorller(IShopifyOrderService orderService, ICountryInfoProvider countryInfoProvider)
        {
            this.orderService = orderService;
            this.countryInfoProvider = countryInfoProvider;
        }

        public async Task<IActionResult> Index()
        {
            string cartToken = "test";
            ViewData["PageClass"] = "inverted";
            var customerDetails = await orderService.GetOrderCustomerDetails(cartToken);
            if (customerDetails != null)
            {
                UpdateCurrentContact(customerDetails);
            }

            return View("~/Views/ShopifyThankYou/Index.cshtml", "asdads");
        }


        private void UpdateCurrentContact(OrderCustomerDetails customerDetails)
        {
            var currentContact = ContactManagementContext.CurrentContact;
            if (currentContact == null || customerDetails == null)
            {
                return;
            }

            currentContact.ContactFirstName = customerDetails.FirstName;
            currentContact.ContactLastName = customerDetails.LastName;
            currentContact.ContactEmail = customerDetails.Email;
            currentContact.ContactAddress1 = customerDetails.Address1;
            currentContact.ContactCity = customerDetails.City;
            currentContact.ContactZIP = customerDetails.Zip;

            if (!string.IsNullOrEmpty(customerDetails.CountryCode))
            {
                var country = countryInfoProvider.Get()
                .TopN(1)
                .WhereEquals(nameof(CountryInfo.CountryTwoLetterCode), customerDetails.CountryCode)
                .FirstOrDefault();

                currentContact.ContactCountryID = country?.CountryID ?? 0;
            }

            currentContact.Update();
        }
    }
}
