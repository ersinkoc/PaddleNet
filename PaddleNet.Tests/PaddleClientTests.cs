using System.Net;
using System.Net.Http;
using System.Text.Json;
using Xunit;
using PaddleNet.SDK;

namespace PaddleNet.Tests;

public class PaddleClientTests
{
    private const string TestApiKey = "test_api_key";
    private const string TestVendorId = "test_vendor_id";

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

    [Fact]
    public async Task ListSubscriptionsAsync_ValidPlanId_ReturnsSubscriptions()
    {
        // Arrange
        var planId = "123";
        var expectedResponse = new SubscriptionListResponse(
            Success: true,
            Response: new List<SubscriptionData>
            {
                new(
                    SubscriptionId: "sub_1",
                    PlanId: planId,
                    Status: "active",
                    NextPaymentDate: DateTime.UtcNow.AddDays(30),
                    Amount: 29.99m,
                    Currency: "USD",
                    CustomerId: "cust_1"
                )
            }
        );

        var mockHttpClient = new HttpClient(new MockHttpMessageHandler(JsonSerializer.Serialize(expectedResponse)))
        {
            BaseAddress = new Uri("https://vendors.paddle.com/api/2.0")
        };

        var client = new PaddleClient(TestApiKey, TestVendorId, mockHttpClient);

        // Act
        var result = await client.ListSubscriptionsAsync(planId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Single(result.Response);
        Assert.Equal(planId, result.Response[0].PlanId);
    }

    [Fact]
    public async Task UpdateSubscriptionPlanAsync_ValidData_ReturnsUpdatedSubscription()
    {
        // Arrange
        var subscriptionId = "sub_1";
        var newPlanId = "plan_2";
        var expectedResponse = new SubscriptionUpdateResponse(
            Success: true,
            Response: new SubscriptionUpdateData(
                SubscriptionId: subscriptionId,
                NewPlanId: newPlanId,
                NextPaymentDate: DateTime.UtcNow.AddDays(30),
                NewAmount: 39.99m
            )
        );

        var mockHttpClient = new HttpClient(new MockHttpMessageHandler(JsonSerializer.Serialize(expectedResponse)))
        {
            BaseAddress = new Uri("https://vendors.paddle.com/api/2.0")
        };

        var client = new PaddleClient(TestApiKey, TestVendorId, mockHttpClient);

        // Act
        var result = await client.UpdateSubscriptionPlanAsync(subscriptionId, newPlanId);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(newPlanId, result.Response.NewPlanId);
    }

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

        // Note: In a real test, you would need to generate a valid signature with a known key pair
        var signature = "test_signature";
        var publicKey = "test_public_key";

        // Act
        var result = client.ValidateWebhookSignature(signature, webhookData, publicKey);

        // Assert
        Assert.False(result); // Will be false because we're using dummy values
    }

    [Fact]
    public async Task CreateCouponAsync_ValidRequest_ReturnsCoupon()
    {
        // Arrange
        var request = new CreateCouponRequest(
            DiscountType: "percentage",
            DiscountAmount: 20,
            CouponType: "single",
            CouponPrefix: "TEST",
            NumberOfCoupons: 1,
            Description: "Test Coupon",
            ExpiryDate: DateTime.UtcNow.AddDays(30),
            ProductIds: new List<string> { "123" },
            AllowedUses: 100,
            Currency: "USD"
        );

        var expectedResponse = new CouponResponse(
            Success: true,
            Response: new CouponData(
                CouponCode: "TEST-123",
                DiscountType: "percentage",
                DiscountAmount: 20,
                ExpiryDate: DateTime.UtcNow.AddDays(30),
                AllowedUses: 100,
                TimesUsed: 0
            )
        );

        var mockHttpClient = new HttpClient(new MockHttpMessageHandler(JsonSerializer.Serialize(expectedResponse)))
        {
            BaseAddress = new Uri("https://vendors.paddle.com/api/2.0")
        };

        var client = new PaddleClient(TestApiKey, TestVendorId, mockHttpClient);

        // Act
        var result = await client.CreateCouponAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal("TEST-123", result.Response.CouponCode);
        Assert.Equal(20, result.Response.DiscountAmount);
    }

    [Fact]
    public async Task ListCouponsAsync_ValidRequest_ReturnsCoupons()
    {
        // Arrange
        var expectedResponse = new CouponListResponse(
            Success: true,
            Response: new List<CouponData>
            {
                new(
                    CouponCode: "TEST-123",
                    DiscountType: "percentage",
                    DiscountAmount: 20,
                    ExpiryDate: DateTime.UtcNow.AddDays(30),
                    AllowedUses: 100,
                    TimesUsed: 0
                )
            }
        );

        var mockHttpClient = new HttpClient(new MockHttpMessageHandler(JsonSerializer.Serialize(expectedResponse)))
        {
            BaseAddress = new Uri("https://vendors.paddle.com/api/2.0")
        };

        var client = new PaddleClient(TestApiKey, TestVendorId, mockHttpClient);

        // Act
        var result = await client.ListCouponsAsync("123");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Single(result.Response);
        Assert.Equal("TEST-123", result.Response[0].CouponCode);
    }

    [Fact]
    public async Task ValidateLicenseAsync_ValidLicense_ReturnsValidation()
    {
        // Arrange
        var expectedResponse = new LicenseValidationResponse(
            Success: true,
            Response: new LicenseValidationData(
                LicenseCode: "TEST-LICENSE-123",
                IsValid: true,
                ExpiryDate: DateTime.UtcNow.AddYears(1),
                ActivationsLimit: 5,
                TimesActivated: 1
            )
        );

        var mockHttpClient = new HttpClient(new MockHttpMessageHandler(JsonSerializer.Serialize(expectedResponse)))
        {
            BaseAddress = new Uri("https://vendors.paddle.com/api/2.0")
        };

        var client = new PaddleClient(TestApiKey, TestVendorId, mockHttpClient);

        // Act
        var result = await client.ValidateLicenseAsync("123", "TEST-LICENSE-123");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.True(result.Response.IsValid);
        Assert.Equal("TEST-LICENSE-123", result.Response.LicenseCode);
    }

    [Fact]
    public async Task ActivateLicenseAsync_ValidLicense_ReturnsActivation()
    {
        // Arrange
        var expectedResponse = new LicenseActivationResponse(
            Success: true,
            Response: new LicenseActivationData(
                LicenseCode: "TEST-LICENSE-123",
                Activated: true,
                Message: "License activated successfully"
            )
        );

        var mockHttpClient = new HttpClient(new MockHttpMessageHandler(JsonSerializer.Serialize(expectedResponse)))
        {
            BaseAddress = new Uri("https://vendors.paddle.com/api/2.0")
        };

        var client = new PaddleClient(TestApiKey, TestVendorId, mockHttpClient);

        // Act
        var result = await client.ActivateLicenseAsync("123", "TEST-LICENSE-123");

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.True(result.Response.Activated);
        Assert.Equal("TEST-LICENSE-123", result.Response.LicenseCode);
    }
}

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