using PaddleNet.SDK;

namespace PaddleNet.Examples.Examples;

public static class CouponExample
{
    public static async Task RunAsync(PaddleClient client)
    {
        Console.WriteLine("\nRunning Coupon Examples");
        Console.WriteLine("======================\n");

        try
        {
            // Create a new coupon
            Console.WriteLine("1. Creating a new coupon...");
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
            Console.WriteLine($"Discount: {newCoupon.Response.DiscountAmount}%");
            Console.WriteLine($"Expires: {newCoupon.Response.ExpiryDate}");
            Console.WriteLine($"Allowed uses: {newCoupon.Response.AllowedUses}");

            // List all coupons
            Console.WriteLine("\n2. Listing all coupons...");
            var coupons = await client.ListCouponsAsync();
            foreach (var coupon in coupons.Response)
            {
                Console.WriteLine($"\nCoupon code: {coupon.CouponCode}");
                Console.WriteLine($"Discount type: {coupon.DiscountType}");
                Console.WriteLine($"Discount amount: {coupon.DiscountAmount}");
                Console.WriteLine($"Times used: {coupon.TimesUsed}/{coupon.AllowedUses}");
                if (coupon.ExpiryDate.HasValue)
                {
                    Console.WriteLine($"Expires: {coupon.ExpiryDate}");
                }
            }

            // Delete the coupon we just created
            Console.WriteLine($"\n3. Deleting coupon {newCoupon.Response.CouponCode}...");
            var deleteResponse = await client.DeleteCouponAsync(newCoupon.Response.CouponCode, "PRODUCT_ID");
            Console.WriteLine($"Delete response: {deleteResponse.Message}");

            // Verify the coupon was deleted
            Console.WriteLine("\n4. Listing coupons after deletion...");
            var updatedCoupons = await client.ListCouponsAsync();
            var deletedCoupon = updatedCoupons.Response.FirstOrDefault(c => c.CouponCode == newCoupon.Response.CouponCode);
            Console.WriteLine(deletedCoupon == null 
                ? "Coupon was successfully deleted" 
                : "Coupon still exists");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError occurred: {ex.Message}");
        }
    }
} 