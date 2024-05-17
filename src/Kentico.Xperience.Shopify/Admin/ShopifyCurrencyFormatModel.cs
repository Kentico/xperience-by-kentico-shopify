using System.Configuration;

using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace Kentico.Xperience.Shopify.Admin
{
    /// <summary>
    /// Model for Shopify currency format.
    /// </summary>
    public class ShopifyCurrencyFormatModel
    {
        /// <summary>
        /// The currency code (ISO 4217).
        /// </summary>
        [TextInputComponent(Label = "Currency code", ExplanationText = "ISO 4217 currency code")]
        [RegexStringValidator(@"[A-Z]{3}")]
        [RequiredValidationRule]
        public string? CurrencyCode { get; set; }


        /// <summary>
        /// The currency price format.
        /// </summary>
        [TextInputComponent(Label = "Currency price format", WatermarkText = "$0.00")]
        [RequiredValidationRule]
        public string? CurrencyPriceFormat { get; set; }
    }
}
