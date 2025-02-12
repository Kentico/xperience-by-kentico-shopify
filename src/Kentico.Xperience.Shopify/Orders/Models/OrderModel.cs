namespace Kentico.Xperience.Shopify.Orders.Models
{
    /// <summary>
    /// Shopify order model.
    /// </summary>
    public class OrderModel
    {
        /// <summary>
        /// Order ID.
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// Order name.
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Shopify order status page URL.
        /// </summary>
        public required string StatusPageUrl { get; set; }

        /// <summary>
        /// Customer first name.
        /// </summary>
        public required string FirstName { get; set; }

        /// <summary>
        /// Customer last name.
        /// </summary>
        public required string LastName { get; set; }

        /// <summary>
        /// Customer email.
        /// </summary>
        public required string Email { get; set; }

        /// <summary>
        /// Order total price.
        /// </summary>
        public required decimal Amount { get; set; }

        /// <summary>
        /// Order currency code.
        /// </summary>
        public required string CurrencyCode { get; set; }

        /// <summary>
        /// Customer default address.
        /// </summary>
        public required OrderAddressModel DefaultAddress { get; set; }
    }

    /// <summary>
    /// Order address model.
    /// </summary>
    public class OrderAddressModel
    {
        /// <summary>
        /// Address line 1.
        /// </summary>
        public required string Address1 { get; set; }

        /// <summary>
        /// Address city.
        /// </summary>
        public required string City { get; set; }

        /// <summary>
        /// Address ZIP.
        /// </summary>
        public required string Zip { get; set; }

        /// <summary>
        /// Address two-letter country code.
        /// </summary>
        public required string CountryCode { get; set; }
    }
}
