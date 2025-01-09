using PaddleNet.SDK;

Console.WriteLine("Paddle SDK Example Application");
Console.WriteLine("=============================\n");

// Initialize clients for both environments
var sandboxClient = new PaddleClient(
    apiKey: "YOUR_SANDBOX_API_KEY",
    vendorId: "YOUR_SANDBOX_VENDOR_ID",
    environment: PaddleEnvironment.Sandbox
);

var productionClient = new PaddleClient(
    apiKey: "YOUR_PRODUCTION_API_KEY",
    vendorId: "YOUR_PRODUCTION_VENDOR_ID",
    environment: PaddleEnvironment.Production
);

// Select which environment to use
Console.WriteLine("Select environment:");
Console.WriteLine("1. Sandbox (for testing)");
Console.WriteLine("2. Production (for real transactions)");
Console.Write("\nEnter your choice (1 or 2): ");

var choice = Console.ReadLine()?.Trim();
var client = choice == "1" ? sandboxClient : productionClient;
var environmentName = choice == "1" ? "Sandbox" : "Production";

Console.WriteLine($"\nUsing {environmentName} environment");
Console.WriteLine("--------------------------------\n");

try
{
    // Get product information
    Console.WriteLine("Fetching product information...");
    var product = await client.GetProductAsync("PRODUCT_ID");
    Console.WriteLine($"Product name: {product.Response.Name}");
    Console.WriteLine($"Price: {product.Response.BasePrice} {product.Response.Currency}");
    
    // Get order details
    Console.WriteLine("\nFetching order details...");
    var order = await client.GetOrderAsync("ORDER_ID");
    Console.WriteLine($"Order status: {order.Response.Status}");
    Console.WriteLine($"Total amount: {order.Response.Total} {order.Response.Currency}");

    // List active subscriptions for a plan
    Console.WriteLine("\nListing active subscriptions...");
    var subscriptions = await client.ListSubscriptionsAsync("PLAN_ID", state: "active");
    foreach (var sub in subscriptions.Response)
    {
        Console.WriteLine($"Subscription ID: {sub.SubscriptionId}");
        Console.WriteLine($"Next payment date: {sub.NextPaymentDate}");
        Console.WriteLine($"Amount: {sub.Amount} {sub.Currency}");
    }

    // Create a new coupon
    Console.WriteLine("\nCreating a new coupon...");
    var couponRequest = new CreateCouponRequest(
        DiscountType: "percentage",
        DiscountAmount: 20,
        CouponType: "single",
        CouponPrefix: "TEST",
        NumberOfCoupons: 1,
        Description: $"20% off for testing ({environmentName})",
        ExpiryDate: DateTime.UtcNow.AddDays(30),
        ProductIds: new List<string> { "PRODUCT_ID" },
        AllowedUses: 100,
        Currency: "USD"
    );
    
    var newCoupon = await client.CreateCouponAsync(couponRequest);
    Console.WriteLine($"Created coupon code: {newCoupon.Response.CouponCode}");
    Console.WriteLine($"Discount: {newCoupon.Response.DiscountAmount}%");
    Console.WriteLine($"Expires: {newCoupon.Response.ExpiryDate}");

    // List all coupons
    Console.WriteLine("\nListing all coupons...");
    var coupons = await client.ListCouponsAsync();
    foreach (var coupon in coupons.Response)
    {
        Console.WriteLine($"Coupon code: {coupon.CouponCode}");
        Console.WriteLine($"Times used: {coupon.TimesUsed}/{coupon.AllowedUses}");
    }

    // Validate a license
    Console.WriteLine("\nValidating license...");
    var licenseValidation = await client.ValidateLicenseAsync("PRODUCT_ID", "LICENSE_KEY");
    Console.WriteLine($"License is valid: {licenseValidation.Response.IsValid}");
    Console.WriteLine($"Activations: {licenseValidation.Response.TimesActivated}/{licenseValidation.Response.ActivationsLimit}");
    if (licenseValidation.Response.ExpiryDate.HasValue)
    {
        Console.WriteLine($"Expires: {licenseValidation.Response.ExpiryDate}");
    }

    // Activate a license
    Console.WriteLine("\nActivating license...");
    var licenseActivation = await client.ActivateLicenseAsync("PRODUCT_ID", "LICENSE_KEY");
    Console.WriteLine($"License activated: {licenseActivation.Response.Activated}");
    Console.WriteLine($"Message: {licenseActivation.Response.Message}");

    // Example of webhook signature validation
    Console.WriteLine("\nValidating webhook signature...");
    var webhookData = new Dictionary<string, string>
    {
        ["alert_id"] = "123456789",
        ["event_time"] = "2024-01-01 12:00:00",
        ["subscription_id"] = "123",
        ["status"] = "active"
    };

    var isValid = client.ValidateWebhookSignature(
        "WEBHOOK_SIGNATURE",
        webhookData,
        "YOUR_PUBLIC_KEY"
    );

    Console.WriteLine($"Webhook signature is valid: {isValid}");

    // Example of cancelling a subscription
    Console.WriteLine("\nCancelling subscription...");
    var cancelledSub = await client.CancelSubscriptionAsync("SUBSCRIPTION_ID");
    Console.WriteLine($"Subscription status: {cancelledSub.Response.Status}");
    Console.WriteLine($"Cancellation effective date: {cancelledSub.Response.CancellationEffectiveDate}");
}
catch (Exception ex)
{
    Console.WriteLine($"\nError occurred: {ex.Message}");
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();
