# PaddleNet Tests

This directory contains unit tests for the PaddleNet SDK.

## Test Structure

- `PaddleClientTests.cs`: Main test suite for the PaddleClient class
  - Product API tests
  - Order API tests
  - Subscription management tests
  - Coupon management tests
  - License management tests
  - Webhook validation tests

## Running Tests

1. Clone the repository:
```bash
git clone https://github.com/ersinkoc/PaddleNet.git
cd PaddleNet
```

2. Run all tests:
```bash
dotnet test
```

3. Run specific test project:
```bash
dotnet test PaddleNet.Tests/PaddleNet.Tests.csproj
```

4. Run tests with specific filter:
```bash
dotnet test --filter "FullyQualifiedName~PaddleNet.Tests.PaddleClientTests"
```

## Test Categories

### Product Tests
- Get product information
- Handle invalid product IDs
- Verify response deserialization

### Order Tests
- Get order details
- Handle invalid order IDs
- Verify response data

### Subscription Tests
- List subscriptions
- Update subscription plans
- Cancel subscriptions
- Handle subscription state changes

### Coupon Tests
- Create coupons
- List active coupons
- Delete coupons
- Verify coupon operations

### License Tests
- Validate licenses
- Activate licenses
- Handle invalid licenses
- Check activation limits

### Webhook Tests
- Validate webhook signatures
- Handle invalid signatures
- Process webhook data

## Mock HTTP Client

The tests use a mock HTTP client to simulate API responses:

```csharp
public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly string _response;
    private readonly HttpStatusCode _statusCode;

    public MockHttpMessageHandler(string response, HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        _response = response;
        _statusCode = statusCode;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        return Task.FromResult(new HttpResponseMessage
        {
            StatusCode = _statusCode,
            Content = new StringContent(_response)
        });
    }
}
```

## Test Examples

### Testing Product Retrieval

```csharp
[Fact]
public async Task GetProductAsync_ValidProductId_ReturnsProduct()
{
    // Arrange
    var productId = "123";
    var expectedProduct = new ProductResponse(
        Success: true,
        Response: new ProductResponseData(
            ProductId: productId,
            Name: "Test Product",
            Description: "Test Description",
            BasePrice: 99.99m,
            Currency: "USD"
        )
    );

    var mockHttpClient = new HttpClient(new MockHttpMessageHandler(JsonSerializer.Serialize(expectedProduct)))
    {
        BaseAddress = new Uri("https://vendors.paddle.com/api/2.0")
    };

    var client = new PaddleClient(TestApiKey, TestVendorId, mockHttpClient);

    // Act
    var result = await client.GetProductAsync(productId);

    // Assert
    Assert.NotNull(result);
    Assert.True(result.Success);
    Assert.Equal(productId, result.Response.ProductId);
    Assert.Equal("Test Product", result.Response.Name);
}
```

### Testing Webhook Validation

```csharp
[Fact]
public void ValidateWebhookSignature_ValidSignature_ReturnsTrue()
{
    // Arrange
    var client = new PaddleClient(TestApiKey, TestVendorId);
    var webhookData = new Dictionary<string, string>
    {
        ["alert_id"] = "123456789",
        ["event_time"] = "2024-01-01 12:00:00"
    };

    var signature = "test_signature";
    var publicKey = "test_public_key";

    // Act
    var result = client.ValidateWebhookSignature(signature, webhookData, publicKey);

    // Assert
    Assert.False(result);
}
```

## Best Practices

1. **Test Organization**
   - Group related tests together
   - Use clear and descriptive test names
   - Follow Arrange-Act-Assert pattern

2. **Mock External Dependencies**
   - Use mock HTTP client for API calls
   - Simulate different response scenarios
   - Test error conditions

3. **Test Coverage**
   - Test all public methods
   - Include positive and negative cases
   - Test edge cases and error handling

4. **Test Maintenance**
   - Keep tests simple and focused
   - Update tests when API changes
   - Document test requirements

## Additional Resources

- [Main SDK Documentation](../README.md)
- [Example Projects](../PaddleNet.Examples/README.md)
- [Contributing Guidelines](../CONTRIBUTING.md) 