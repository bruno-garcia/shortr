using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace Redirectr.Web
{
    internal static class ConfigurationExtension
    {
        public static string GetBaseAddress(this IConfiguration configuration)
        {
            var baseAddress = configuration.GetValue<string>("BaseAddress");
            if (baseAddress is null)
            {
                baseAddress = configuration.GetValue<string>("URLS").Split(";").FirstOrDefault();
                if (baseAddress is null)
                {
                    throw new InvalidOperationException("No base address for redirect.");
                }
            }

            return baseAddress;
        }
    }
}