namespace Kentico.Xperience.Shopify.Tests.Repositories
{
    internal class DiscountCodesRepository
    {
        /// <summary>
        /// Discount code that is not included in <see cref="DiscountCodes"/>.
        /// </summary>
        public static string NonExistingDiscountCode = "NonExistingDiscountCode";


        public IEnumerable<string> DiscountCodes { get; set; } = [
                "SAVE10",
                "SALE20",
                "25OFF",
                "FREESHIP",
                "WELCOME",
                "VIP15",
                "DEAL25",
                "SPRING30",
                "SUMMER40",
                "FALL50"
            ];
    }
}
