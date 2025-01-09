# PaddleNet.SDK

The main SDK library for interacting with the Paddle payment system API.

## Project Structure

```
PaddleNet.SDK/
├── PaddleClient.cs        # Main client implementation
├── Models/                # Response and request models
├── Exceptions/            # Custom exceptions
└── Utils/                # Utility classes
```

## Features

### API Client
- Environment support (Sandbox/Production)
- Async/await operations
- Proper error handling
- Strong typing for all operations

### Product Management
- Get product information
- Handle product pricing
- Manage product metadata

### Order Management
- Retrieve order details
- Track order status
- Process order information

### Subscription Management
- List subscriptions
- Update subscription plans
- Cancel subscriptions
- Handle subscription states

### Coupon Management
- Create discount coupons
- List active coupons
- Delete coupons
- Track coupon usage

### License Management
- Validate license keys
- Activate licenses
- Track license usage
- Handle license limits

### Webhook Support
- Validate webhook signatures
- Process webhook data
- Secure webhook handling

## Implementation Details

### PaddleClient

The main client class that handles all API interactions:

```csharp
public class PaddleClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _vendorId;
    private readonly PaddleEnvironment _environment;

    public PaddleClient(
        string apiKey, 
        string vendorId, 
        PaddleEnvironment environment = PaddleEnvironment.Production,
        HttpClient? httpClient = null)
    {
        // Initialization
    }

    // API methods
}
```

### Response Models

All API responses are strongly typed using C# 9.0 records:

```csharp
public record ProductResponse(
    bool Success,
    ProductResponseData Response
);

public record ProductResponseData(
    string ProductId,
    string Name,
    string Description,
    decimal BasePrice,
    string Currency
);
```

### Error Handling

The SDK provides comprehensive error handling:

```csharp
try
{
    var product = await client.GetProductAsync("PRODUCT_ID");
}
catch (HttpRequestException ex)
{
    // Handle API communication errors
}
catch (InvalidOperationException ex)
{
    // Handle response deserialization errors
}
```

## Best Practices

1. **API Key Management**
   - Store API keys securely
   - Use different keys for sandbox and production
   - Never commit API keys to source control

2. **Environment Usage**
   - Use sandbox for development and testing
   - Use production only for live transactions
   - Keep environments strictly separated

3. **Error Handling**
   - Always handle potential exceptions
   - Provide meaningful error messages
   - Log errors appropriately

4. **Performance**
   - Reuse HttpClient instances
   - Implement proper cancellation
   - Handle rate limiting

## Dependencies

- .NET 8.0
- System.Net.Http.Json
- System.Text.Json

## Building

```bash
dotnet build
```

## Testing

```bash
dotnet test
```

## Additional Resources

- [Example Projects](../PaddleNet.Examples/README.md)
- [Test Documentation](../PaddleNet.Tests/README.md)
- [Contributing Guidelines](../CONTRIBUTING.md)
- [Paddle API Documentation](https://developer.paddle.com/api-reference/overview) 