# Upgrading PaddleNet SDK

This guide provides information about breaking changes and how to upgrade from one version to another.

## Upgrading to 1.0.0

### Breaking Changes

No breaking changes as this is the initial release.

### New Features

- Initial implementation of Paddle API client
- Environment support (Sandbox/Production)
- Async/await support for all operations
- Comprehensive error handling
- XML documentation
- Type-safe response models

### Deprecations

No deprecations as this is the initial release.

## API Changes

### Client Initialization

The client can now be initialized with environment selection:

```csharp
// Before (not applicable - new feature)

// After
var client = new PaddleClient(
    apiKey: "YOUR_API_KEY",
    vendorId: "YOUR_VENDOR_ID",
    environment: PaddleEnvironment.Sandbox // or PaddleEnvironment.Production
);
```

### Response Types

All response types are now strongly typed records:

```csharp
// Example response type
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

Improved error handling with specific exceptions:

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

## Future Changes

We plan to maintain backward compatibility in future releases. Any breaking changes will be clearly documented in this guide. 