using PaddleNet.SDK;

namespace PaddleNet.Examples.Examples;

public static class SubscriptionExample
{
    public static async Task RunAsync(PaddleClient client)
    {
        Console.WriteLine("\nRunning Subscription Examples");
        Console.WriteLine("============================\n");

        try
        {
            // List all active subscriptions
            Console.WriteLine("1. Listing active subscriptions...");
            var subscriptions = await client.ListSubscriptionsAsync("PLAN_ID", state: "active");
            foreach (var sub in subscriptions.Response)
            {
                Console.WriteLine($"Subscription ID: {sub.SubscriptionId}");
                Console.WriteLine($"Plan ID: {sub.PlanId}");
                Console.WriteLine($"Status: {sub.Status}");
                Console.WriteLine($"Next Payment: {sub.NextPaymentDate}");
                Console.WriteLine($"Amount: {sub.Amount} {sub.Currency}");
                Console.WriteLine($"Customer ID: {sub.CustomerId}\n");
            }

            // Update subscription plan
            if (subscriptions.Response.Any())
            {
                var firstSub = subscriptions.Response[0];
                Console.WriteLine($"\n2. Updating subscription {firstSub.SubscriptionId} to new plan...");
                
                var updatedSub = await client.UpdateSubscriptionPlanAsync(
                    firstSub.SubscriptionId,
                    "NEW_PLAN_ID",
                    prorate: true
                );

                Console.WriteLine($"Updated to plan: {updatedSub.Response.NewPlanId}");
                Console.WriteLine($"Next payment amount: {updatedSub.Response.NewAmount}");
                Console.WriteLine($"Next payment date: {updatedSub.Response.NextPaymentDate}");

                // Cancel subscription
                Console.WriteLine($"\n3. Cancelling subscription {firstSub.SubscriptionId}...");
                var cancelledSub = await client.CancelSubscriptionAsync(firstSub.SubscriptionId);
                Console.WriteLine($"Status: {cancelledSub.Response.Status}");
                Console.WriteLine($"Cancellation effective: {cancelledSub.Response.CancellationEffectiveDate}");
            }
            else
            {
                Console.WriteLine("\nNo active subscriptions found to update or cancel.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError occurred: {ex.Message}");
        }
    }
} 