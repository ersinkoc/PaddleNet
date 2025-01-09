using PaddleNet.SDK;

namespace PaddleNet.Examples.Examples;

public static class LicenseExample
{
    public static async Task RunAsync(PaddleClient client)
    {
        Console.WriteLine("\nRunning License Examples");
        Console.WriteLine("=======================\n");

        try
        {
            // Validate a license
            Console.WriteLine("1. Validating license...");
            var licenseValidation = await client.ValidateLicenseAsync("PRODUCT_ID", "LICENSE_KEY");
            Console.WriteLine($"License code: {licenseValidation.Response.LicenseCode}");
            Console.WriteLine($"Is valid: {licenseValidation.Response.IsValid}");
            Console.WriteLine($"Activations: {licenseValidation.Response.TimesActivated}/{licenseValidation.Response.ActivationsLimit}");
            
            if (licenseValidation.Response.ExpiryDate.HasValue)
            {
                Console.WriteLine($"Expires: {licenseValidation.Response.ExpiryDate}");
            }

            // Only try to activate if the license is valid
            if (licenseValidation.Response.IsValid)
            {
                // Check if we can activate (haven't reached limit)
                if (licenseValidation.Response.TimesActivated < licenseValidation.Response.ActivationsLimit)
                {
                    Console.WriteLine("\n2. Activating license...");
                    var licenseActivation = await client.ActivateLicenseAsync("PRODUCT_ID", "LICENSE_KEY");
                    Console.WriteLine($"Activation status: {licenseActivation.Response.Activated}");
                    Console.WriteLine($"Message: {licenseActivation.Response.Message}");

                    // Validate again to see updated activation count
                    Console.WriteLine("\n3. Validating license after activation...");
                    var updatedValidation = await client.ValidateLicenseAsync("PRODUCT_ID", "LICENSE_KEY");
                    Console.WriteLine($"Updated activations: {updatedValidation.Response.TimesActivated}/{updatedValidation.Response.ActivationsLimit}");
                }
                else
                {
                    Console.WriteLine("\nCannot activate: License has reached its activation limit.");
                }
            }
            else
            {
                Console.WriteLine("\nCannot activate: License is not valid.");
            }

            // Example of handling an invalid license
            Console.WriteLine("\n4. Trying to validate an invalid license...");
            try
            {
                var invalidValidation = await client.ValidateLicenseAsync("PRODUCT_ID", "INVALID_LICENSE_KEY");
                Console.WriteLine($"Invalid license check result: {invalidValidation.Response.IsValid}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating invalid license: {ex.Message}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError occurred: {ex.Message}");
        }
    }
} 