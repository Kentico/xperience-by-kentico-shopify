using System.Configuration;

using Kentico.Xperience.Admin.Base.FormAnnotations;

namespace Kentico.Xperience.Shopify.Admin
{
    public class ShopifyCurrencyFormatModel
    {
        [TextInputComponent(Label = "Currency code", ExplanationText = "ISO 4217 currency code")]
        [RegexStringValidator(@"[A-Z]{3}")]
        [RequiredValidationRule]
        public string? CurrencyCode { get; set; }


        [TextInputComponent(Label = "Currency price format", WatermarkText = "$0.00")]
        [RequiredValidationRule]
        public string? CurrencyPriceFormat { get; set; }
    }
}
