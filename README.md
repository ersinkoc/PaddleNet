# PaddleNet SDK

A .NET SDK for the Paddle payment system API, built with .NET 8.

## Features

- Product information retrieval
- Order details retrieval
- Subscription management
  - List subscriptions
  - Update subscription plans
  - Cancel subscriptions
- Coupon management
  - Create coupons
  - List coupons
  - Delete coupons
- License management
  - Validate licenses
  - Activate licenses
- Webhook signature validation
- Environment support
  - Sandbox for testing
  - Production for real transactions
- Async/await support
- Unit tested
- Example project included

## Installation

Clone the repository and build the solution:

```bash
git clone https://github.com/ersinkoc/PaddleNet.git
cd PaddleNet
dotnet build
```

## Usage

First, initialize the PaddleClient with your API credentials. You can choose between Sandbox and Production environments:

```csharp
// For testing (sandbox environment)
var sandboxClient = new PaddleClient(
    apiKey: "YOUR_SANDBOX_API_KEY",
    vendorId: "YOUR_SANDBOX_VENDOR_ID",
    environment: PaddleEnvironment.Sandbox
);

// For real transactions (production environment)
var productionClient = new PaddleClient(
    apiKey: "YOUR_PRODUCTION_API_KEY",
    vendorId: "YOUR_PRODUCTION_VENDOR_ID",
    environment: PaddleEnvironment.Production
);
```

### Get Product Information

```csharp
var product = await client.GetProductAsync("PRODUCT_ID");
Console.WriteLine($"Product name: {product.Response.Name}");
Console.WriteLine($"Price: {product.Response.BasePrice} {product.Response.Currency}");
```

### Get Order Details

```csharp
var order = await client.GetOrderAsync("ORDER_ID");
Console.WriteLine($"Order status: {order.Response.Status}");
Console.WriteLine($"Total amount: {order.Response.Total} {order.Response.Currency}");
```

### Subscription Management

List subscriptions for a plan:
```csharp
var subscriptions = await client.ListSubscriptionsAsync("PLAN_ID", state: "active");
foreach (var sub in subscriptions.Response)
{
    Console.WriteLine($"Subscription ID: {sub.SubscriptionId}");
    Console.WriteLine($"Next payment: {sub.NextPaymentDate}");
}
```

Update a subscription's plan:
```csharp
var updatedSub = await client.UpdateSubscriptionPlanAsync(
    "SUBSCRIPTION_ID",
    "NEW_PLAN_ID",
    prorate: true
);
Console.WriteLine($"Next payment amount: {updatedSub.Response.NewAmount}");
```

Cancel a subscription:
```csharp
var cancelledSub = await client.CancelSubscriptionAsync("SUBSCRIPTION_ID");
Console.WriteLine($"Cancellation effective: {cancelledSub.Response.CancellationEffectiveDate}");
```

### Coupon Management

Create a new coupon:
```csharp
var couponRequest = new CreateCouponRequest(
    DiscountType: "percentage",
    DiscountAmount: 20,
    CouponType: "single",
    CouponPrefix: "TEST",
    NumberOfCoupons: 1,
    Description: "20% off for testing",
    ExpiryDate: DateTime.UtcNow.AddDays(30),
    ProductIds: new List<string> { "PRODUCT_ID" },
    AllowedUses: 100,
    Currency: "USD"
);

var newCoupon = await client.CreateCouponAsync(couponRequest);
Console.WriteLine($"Created coupon code: {newCoupon.Response.CouponCode}");
```

List coupons:
```csharp
var coupons = await client.ListCouponsAsync();
foreach (var coupon in coupons.Response)
{
    Console.WriteLine($"Coupon code: {coupon.CouponCode}");
    Console.WriteLine($"Times used: {coupon.TimesUsed}/{coupon.AllowedUses}");
}
```

### License Management

Validate a license:
```csharp
var validation = await client.ValidateLicenseAsync("PRODUCT_ID", "LICENSE_KEY");
Console.WriteLine($"License is valid: {validation.Response.IsValid}");
Console.WriteLine($"Activations: {validation.Response.TimesActivated}/{validation.Response.ActivationsLimit}");
```

Activate a license:
```csharp
var activation = await client.ActivateLicenseAsync("PRODUCT_ID", "LICENSE_KEY");
Console.WriteLine($"License activated: {activation.Response.Activated}");
Console.WriteLine($"Message: {activation.Response.Message}");
```

### Webhook Handling

Validate webhook signatures:
```csharp
var webhookData = new Dictionary<string, string>
{
    ["alert_id"] = "123456789",
    ["event_time"] = "2024-01-01 12:00:00"
    // Add other webhook fields here
};

var isValid = client.ValidateWebhookSignature(
    "WEBHOOK_SIGNATURE",
    webhookData,
    "YOUR_PUBLIC_KEY"
);

if (isValid)
{
    // Process the webhook
}
```

## Environments

The SDK supports two environments:

### Sandbox Environment
Used for testing and evaluation. All transactions are simulated and no real money is involved.
```csharp
var client = new PaddleClient(
    apiKey: "YOUR_SANDBOX_API_KEY",
    vendorId: "YOUR_SANDBOX_VENDOR_ID",
    environment: PaddleEnvironment.Sandbox
);
```

### Production Environment
Used for real transactions where customers can make actual purchases.
```csharp
var client = new PaddleClient(
    apiKey: "YOUR_PRODUCTION_API_KEY",
    vendorId: "YOUR_PRODUCTION_VENDOR_ID",
    environment: PaddleEnvironment.Production
);
```

## Project Structure

- `PaddleNet.SDK`: The main SDK library
- `PaddleNet.Tests`: Unit tests for the SDK
- `PaddleNet.Examples`: Example usage of the SDK

## Running Tests

```bash
dotnet test
```

## Contributing

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## Acknowledgments

- [Paddle API Documentation](https://developer.paddle.com/api-reference/overview) 