using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Redirectr
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddRedirectr(this IServiceCollection serviceCollection)
        {
            serviceCollection.TryAddSingleton<KeyGenerator>();
            serviceCollection.TryAddSingleton<IRedirectrStore, InMemoryRedirectrStore>();
            return serviceCollection;
        }
    }
}