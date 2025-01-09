# PaddleNet Examples

This directory contains example projects demonstrating how to use the PaddleNet SDK.

## Structure

- `Examples/SubscriptionExample.cs`: Subscription management examples
- `Examples/CouponExample.cs`: Coupon management examples
- `Examples/LicenseExample.cs`: License management examples

## Running the Examples

1. Clone the repository:
```bash
git clone https://github.com/ersinkoc/PaddleNet.git
cd PaddleNet
```

2. Update API credentials:
Edit `Program.cs` and replace the placeholder values with your Paddle API credentials:
```csharp
var sandboxClient = new PaddleClient(
    apiKey: "YOUR_SANDBOX_API_KEY",    // Replace with your sandbox API key
    vendorId: "YOUR_SANDBOX_VENDOR_ID", // Replace with your sandbox vendor ID
    environment: PaddleEnvironment.Sandbox
);

var productionClient = new PaddleClient(
    apiKey: "YOUR_PRODUCTION_API_KEY",    // Replace with your production API key
    vendorId: "YOUR_PRODUCTION_VENDOR_ID", // Replace with your production vendor ID
    environment: PaddleEnvironment.Production
);
```

3. Build and run:
```bash
dotnet build
dotnet run
```

## Example Scenarios

### Subscription Management
- List active subscriptions
- Update subscription plans
- Cancel subscriptions

```csharp
// Example: List active subscriptions
var subscriptions = await client.ListSubscriptionsAsync("PLAN_ID", state: "active");
foreach (var sub in subscriptions.Response)
{
    Console.WriteLine($"Subscription ID: {sub.SubscriptionId}");
    Console.WriteLine($"Next payment: {sub.NextPaymentDate}");
}
```

### Coupon Management
- Create discount coupons
- List active coupons
- Delete coupons
- Verify coupon deletion

```csharp
// Example: Create a coupon
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
```

### License Management
- Validate license keys
- Activate licenses
- Handle invalid licenses
- Check activation limits

```csharp
// Example: Validate and activate a license
var validation = await client.ValidateLicenseAsync("PRODUCT_ID", "LICENSE_KEY");
if (validation.Response.IsValid)
{
    var activation = await client.ActivateLicenseAsync("PRODUCT_ID", "LICENSE_KEY");
    Console.WriteLine($"License activated: {activation.Response.Activated}");
}
```

## Environment Selection

The example application allows you to choose between Sandbox and Production environments at runtime:

```csharp
Console.WriteLine("Select environment:");
Console.WriteLine("1. Sandbox (for testing)");
Console.WriteLine("2. Production (for real transactions)");
```

## Error Handling

All examples include proper error handling:

```csharp
try
{
    // API calls
}
catch (Exception ex)
{
    Console.WriteLine($"\nError occurred: {ex.Message}");
}
```

## Best Practices

1. **Environment Separation**
   - Use sandbox environment for testing
   - Keep sandbox and production credentials separate
   - Never use production credentials in development

2. **API Keys**
   - Store API keys securely
   - Use environment variables or secure configuration
   - Never commit API keys to source control

3. **Error Handling**
   - Always implement proper error handling
   - Log errors appropriately
   - Provide meaningful error messages

4. **Testing**
   - Test all features in sandbox first
   - Verify webhook signatures
   - Test error scenarios

## Additional Resources

- [Main SDK Documentation](../README.md)
- [Contributing Guidelines](../CONTRIBUTING.md)
- [Paddle API Documentation](https://developer.paddle.com/api-reference/overview) 