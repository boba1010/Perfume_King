using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.WebUtilities;
using System.Globalization;

namespace Perfume_King.Blazor.Extensions
{
    public static class CultureExtensions
    {
        public static WebAssemblyHost ApplyUrlCulture(this WebAssemblyHost host)
        {
            var navManager = host.Services.GetRequiredService<NavigationManager>();
            var uri = new Uri(navManager.Uri);

            CultureInfo culture;

            // Check if url has ?culture=ar-EG or ?culture=en-US
            if (QueryHelpers.ParseQuery(uri.Query).TryGetValue("culture", out var cultureValue))
            {
                culture = new CultureInfo(cultureValue.ToString());
            }
            else
            {
                culture = new CultureInfo("en-US"); // Default fallback
            }

            CultureInfo.DefaultThreadCurrentCulture = culture;
            CultureInfo.DefaultThreadCurrentUICulture = culture;

            return host;
        }
    }
}
