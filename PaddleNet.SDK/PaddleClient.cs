using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;

namespace PaddleNet.SDK;

/// <summary>
/// Specifies the Paddle environment to use.
/// </summary>
public enum PaddleEnvironment
{
    /// <summary>
    /// Sandbox environment for testing (https://sandbox-api.paddle.com/)
    /// </summary>
    Sandbox,

    /// <summary>
    /// Production environment for real transactions (https://api.paddle.com/)
    /// </summary>
    Production
}

/// <summary>
/// Client for interacting with the Paddle payment system API.
/// </summary>
public class PaddleClient
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _vendorId;
    private readonly PaddleEnvironment _environment;
    private const string SandboxBaseUrl = "https://sandbox-api.paddle.com/";
    private const string ProductionBaseUrl = "https://api.paddle.com/";

    /// <summary>
    /// Initializes a new instance of the PaddleClient class.
    /// </summary>
    /// <param name="apiKey">Your Paddle API key.</param>
    /// <param name="vendorId">Your Paddle vendor ID.</param>
    /// <param name="environment">The Paddle environment to use (defaults to Production).</param>
    /// <param name="httpClient">Optional HttpClient instance. If not provided, a new instance will be created.</param>
    /// <exception cref="ArgumentNullException">Thrown when apiKey or vendorId is null.</exception>
    public PaddleClient(
        string apiKey, 
        string vendorId, 
        PaddleEnvironment environment = PaddleEnvironment.Production,
        HttpClient? httpClient = null)
    {
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        _vendorId = vendorId ?? throw new ArgumentNullException(nameof(vendorId));
        _environment = environment;

        var baseUrl = environment == PaddleEnvironment.Sandbox
            ? "https://sandbox-vendors.paddle.com/api/2.0"
            : "https://vendors.paddle.com/api/2.0";

        _httpClient = httpClient ?? new HttpClient();
        if (_httpClient.BaseAddress == null)
        {
            _httpClient.BaseAddress = new Uri(baseUrl);
        }
    }

    /// <summary>
    /// Retrieves product information from Paddle.
    /// </summary>
    /// <param name="productId">The ID of the product to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A ProductResponse containing the product information.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the response cannot be deserialized.</exception>
    public async Task<ProductResponse> GetProductAsync(string productId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/product/get_products?product_id={productId}&vendor_id={_vendorId}&vendor_auth_code={_apiKey}", cancellationToken);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<ProductResponse>(cancellationToken: cancellationToken) 
               ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Retrieves order details from Paddle.
    /// </summary>
    /// <param name="orderId">The ID of the order to retrieve.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An OrderResponse containing the order details.</returns>
    /// <exception cref="HttpRequestException">Thrown when the API request fails.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the response cannot be deserialized.</exception>
    public async Task<OrderResponse> GetOrderAsync(string orderId, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetAsync($"/order/details?order_id={orderId}&vendor_id={_vendorId}&vendor_auth_code={_apiKey}", cancellationToken);
        response.EnsureSuccessStatusCode();
        
        return await response.Content.ReadFromJsonAsync<OrderResponse>(cancellationToken: cancellationToken) 
               ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Lists all subscriptions for a specific plan.
    /// </summary>
    /// <param name="planId">The ID of the subscription plan.</param>
    /// <param name="state">Optional subscription state filter (active, past_due, trialing, paused, deleted).</param>
    /// <param name="page">Page number for pagination.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A SubscriptionListResponse containing the list of subscriptions.</returns>
    public async Task<SubscriptionListResponse> ListSubscriptionsAsync(
        string planId,
        string? state = null,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var url = $"/subscription/users?plan={planId}&page={page}&vendor_id={_vendorId}&vendor_auth_code={_apiKey}";
        if (!string.IsNullOrEmpty(state))
        {
            url += $"&state={state}";
        }

        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<SubscriptionListResponse>(cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Updates a subscription's plan.
    /// </summary>
    /// <param name="subscriptionId">The ID of the subscription to update.</param>
    /// <param name="newPlanId">The ID of the new plan.</param>
    /// <param name="prorate">Whether to prorate the change.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A SubscriptionUpdateResponse containing the updated subscription details.</returns>
    public async Task<SubscriptionUpdateResponse> UpdateSubscriptionPlanAsync(
        string subscriptionId,
        string newPlanId,
        bool prorate = true,
        CancellationToken cancellationToken = default)
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["subscription_id"] = subscriptionId,
            ["plan_id"] = newPlanId,
            ["prorate"] = prorate.ToString().ToLower(),
            ["vendor_id"] = _vendorId,
            ["vendor_auth_code"] = _apiKey
        });

        var response = await _httpClient.PostAsync("/subscription/users/update", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<SubscriptionUpdateResponse>(cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Cancels a subscription.
    /// </summary>
    /// <param name="subscriptionId">The ID of the subscription to cancel.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A SubscriptionCancelResponse indicating the success of the operation.</returns>
    public async Task<SubscriptionCancelResponse> CancelSubscriptionAsync(
        string subscriptionId,
        CancellationToken cancellationToken = default)
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["subscription_id"] = subscriptionId,
            ["vendor_id"] = _vendorId,
            ["vendor_auth_code"] = _apiKey
        });

        var response = await _httpClient.PostAsync("/subscription/users/cancel", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<SubscriptionCancelResponse>(cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Validates a webhook signature.
    /// </summary>
    /// <param name="signature">The p_signature from the webhook payload.</param>
    /// <param name="webhookData">The complete webhook data excluding the p_signature.</param>
    /// <param name="publicKey">Your Paddle public key (from the dashboard).</param>
    /// <returns>True if the webhook signature is valid, false otherwise.</returns>
    public bool ValidateWebhookSignature(string signature, Dictionary<string, string> webhookData, string publicKey)
    {
        var serializedData = SerializeWebhookData(webhookData);
        return VerifySignature(serializedData, signature, publicKey);
    }

    private static string SerializeWebhookData(Dictionary<string, string> data)
    {
        return string.Join("&",
            data.OrderBy(x => x.Key)
                .Select(x => $"{x.Key}={Uri.EscapeDataString(x.Value)}"));
    }

    private static bool VerifySignature(string data, string signature, string publicKey)
    {
        try
        {
            using var rsa = RSA.Create();
            rsa.ImportFromPem(publicKey);

            var dataBytes = Encoding.UTF8.GetBytes(data);
            var signatureBytes = Convert.FromBase64String(signature);

            return rsa.VerifyData(dataBytes, signatureBytes, HashAlgorithmName.SHA1, RSASignaturePadding.Pkcs1);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Creates a new coupon code.
    /// </summary>
    /// <param name="request">The coupon creation request details.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A CouponResponse containing the created coupon details.</returns>
    public async Task<CouponResponse> CreateCouponAsync(CreateCouponRequest request, CancellationToken cancellationToken = default)
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["discount_type"] = request.DiscountType,
            ["discount_amount"] = request.DiscountAmount.ToString(),
            ["coupon_type"] = request.CouponType,
            ["coupon_prefix"] = request.CouponPrefix,
            ["num_coupons"] = request.NumberOfCoupons.ToString(),
            ["description"] = request.Description,
            ["expires"] = request.ExpiryDate?.ToString("yyyy-MM-dd"),
            ["product_ids"] = string.Join(",", request.ProductIds),
            ["allowed_uses"] = request.AllowedUses.ToString(),
            ["currency"] = request.Currency,
            ["vendor_id"] = _vendorId,
            ["vendor_auth_code"] = _apiKey
        });

        var response = await _httpClient.PostAsync("/product/create_coupon", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CouponResponse>(cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Lists all active coupons.
    /// </summary>
    /// <param name="productId">Optional product ID to filter coupons.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A CouponListResponse containing the list of coupons.</returns>
    public async Task<CouponListResponse> ListCouponsAsync(string? productId = null, CancellationToken cancellationToken = default)
    {
        var url = $"/product/list_coupons?vendor_id={_vendorId}&vendor_auth_code={_apiKey}";
        if (!string.IsNullOrEmpty(productId))
        {
            url += $"&product_id={productId}";
        }

        var response = await _httpClient.GetAsync(url, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CouponListResponse>(cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Deletes a coupon.
    /// </summary>
    /// <param name="couponCode">The code of the coupon to delete.</param>
    /// <param name="productId">The ID of the product the coupon is associated with.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A CouponDeleteResponse indicating the success of the operation.</returns>
    public async Task<CouponDeleteResponse> DeleteCouponAsync(string couponCode, string productId, CancellationToken cancellationToken = default)
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["coupon_code"] = couponCode,
            ["product_id"] = productId,
            ["vendor_id"] = _vendorId,
            ["vendor_auth_code"] = _apiKey
        });

        var response = await _httpClient.PostAsync("/product/delete_coupon", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<CouponDeleteResponse>(cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Validates a license key.
    /// </summary>
    /// <param name="productId">The ID of the product.</param>
    /// <param name="licenseKey">The license key to validate.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A LicenseValidationResponse containing the validation result.</returns>
    public async Task<LicenseValidationResponse> ValidateLicenseAsync(string productId, string licenseKey, CancellationToken cancellationToken = default)
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["product_id"] = productId,
            ["license_code"] = licenseKey,
            ["vendor_id"] = _vendorId,
            ["vendor_auth_code"] = _apiKey
        });

        var response = await _httpClient.PostAsync("/license/verify", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<LicenseValidationResponse>(cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("Failed to deserialize response");
    }

    /// <summary>
    /// Activates a license key.
    /// </summary>
    /// <param name="productId">The ID of the product.</param>
    /// <param name="licenseKey">The license key to activate.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A LicenseActivationResponse containing the activation result.</returns>
    public async Task<LicenseActivationResponse> ActivateLicenseAsync(string productId, string licenseKey, CancellationToken cancellationToken = default)
    {
        var content = new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["product_id"] = productId,
            ["license_code"] = licenseKey,
            ["vendor_id"] = _vendorId,
            ["vendor_auth_code"] = _apiKey
        });

        var response = await _httpClient.PostAsync("/license/activate", content, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<LicenseActivationResponse>(cancellationToken: cancellationToken)
               ?? throw new InvalidOperationException("Failed to deserialize response");
    }
}

/// <summary>
/// Represents a response from the Paddle product API.
/// </summary>
/// <param name="Success">Whether the API request was successful.</param>
/// <param name="Response">The product data returned by the API.</param>
public record ProductResponse(
    bool Success,
    ProductResponseData Response
);

/// <summary>
/// Contains detailed product information from Paddle.
/// </summary>
/// <param name="ProductId">The unique identifier of the product.</param>
/// <param name="Name">The name of the product.</param>
/// <param name="Description">The description of the product.</param>
/// <param name="BasePrice">The base price of the product.</param>
/// <param name="Currency">The currency of the base price.</param>
public record ProductResponseData(
    string ProductId,
    string Name,
    string Description,
    decimal BasePrice,
    string Currency
);

/// <summary>
/// Represents a response from the Paddle order API.
/// </summary>
/// <param name="Success">Whether the API request was successful.</param>
/// <param name="Response">The order data returned by the API.</param>
public record OrderResponse(
    bool Success,
    OrderResponseData Response
);

/// <summary>
/// Contains detailed order information from Paddle.
/// </summary>
/// <param name="OrderId">The unique identifier of the order.</param>
/// <param name="Status">The current status of the order.</param>
/// <param name="Total">The total amount of the order.</param>
/// <param name="Currency">The currency of the order total.</param>
/// <param name="CreatedAt">The date and time when the order was created.</param>
public record OrderResponseData(
    string OrderId,
    string Status,
    decimal Total,
    string Currency,
    DateTime CreatedAt
);

/// <summary>
/// Response for subscription list operations.
/// </summary>
public record SubscriptionListResponse(
    bool Success,
    List<SubscriptionData> Response
);

/// <summary>
/// Detailed subscription information.
/// </summary>
public record SubscriptionData(
    string SubscriptionId,
    string PlanId,
    string Status,
    DateTime NextPaymentDate,
    decimal Amount,
    string Currency,
    string CustomerId
);

/// <summary>
/// Response for subscription update operations.
/// </summary>
public record SubscriptionUpdateResponse(
    bool Success,
    SubscriptionUpdateData Response
);

/// <summary>
/// Updated subscription details.
/// </summary>
public record SubscriptionUpdateData(
    string SubscriptionId,
    string NewPlanId,
    DateTime NextPaymentDate,
    decimal NewAmount
);

/// <summary>
/// Response for subscription cancellation operations.
/// </summary>
public record SubscriptionCancelResponse(
    bool Success,
    SubscriptionCancelData Response
);

/// <summary>
/// Cancelled subscription details.
/// </summary>
public record SubscriptionCancelData(
    string SubscriptionId,
    string Status,
    DateTime? CancellationEffectiveDate
);

/// <summary>
/// Request model for creating a coupon.
/// </summary>
public record CreateCouponRequest(
    string DiscountType,
    decimal DiscountAmount,
    string CouponType,
    string CouponPrefix,
    int NumberOfCoupons,
    string Description,
    DateTime? ExpiryDate,
    List<string> ProductIds,
    int AllowedUses,
    string Currency
);

/// <summary>
/// Response for coupon creation operations.
/// </summary>
public record CouponResponse(
    bool Success,
    CouponData Response
);

/// <summary>
/// Detailed coupon information.
/// </summary>
public record CouponData(
    string CouponCode,
    string DiscountType,
    decimal DiscountAmount,
    DateTime? ExpiryDate,
    int AllowedUses,
    int TimesUsed
);

/// <summary>
/// Response for coupon list operations.
/// </summary>
public record CouponListResponse(
    bool Success,
    List<CouponData> Response
);

/// <summary>
/// Response for coupon deletion operations.
/// </summary>
public record CouponDeleteResponse(
    bool Success,
    string Message
);

/// <summary>
/// Response for license validation operations.
/// </summary>
public record LicenseValidationResponse(
    bool Success,
    LicenseValidationData Response
);

/// <summary>
/// Detailed license validation information.
/// </summary>
public record LicenseValidationData(
    string LicenseCode,
    bool IsValid,
    DateTime? ExpiryDate,
    int ActivationsLimit,
    int TimesActivated
);

/// <summary>
/// Response for license activation operations.
/// </summary>
public record LicenseActivationResponse(
    bool Success,
    LicenseActivationData Response
);

/// <summary>
/// Detailed license activation information.
/// </summary>
public record LicenseActivationData(
    string LicenseCode,
    bool Activated,
    string Message
); 