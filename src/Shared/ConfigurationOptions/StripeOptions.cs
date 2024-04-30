namespace Shared.ConfigurationOptions
{
    public class StripeOptions
    {
        public static string SectionName { get; } = "Stripe";
        public string WebhookSigningSecret { get; set; }
        public PricingTable PricingTable { get; set; }
    }

    public class PricingTable
    {
        public string PricingTableId { get; set; }
        public string PublishableKey { get; set; }
    }
}
