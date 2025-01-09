using PaddleNet.SDK;
using PaddleNet.Examples.Examples;

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
    // Run subscription examples
    await SubscriptionExample.RunAsync(client);

    // Run coupon examples
    await CouponExample.RunAsync(client);

    // Run license examples
    await LicenseExample.RunAsync(client);
}
catch (Exception ex)
{
    Console.WriteLine($"\nError occurred: {ex.Message}");
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();
