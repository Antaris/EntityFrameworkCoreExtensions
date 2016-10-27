// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions
{
    using System;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// Provides extensions for the <see cref="IServiceCollection"/> type.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the entity framework core extensions to the services collection.
        /// </summary>
        /// <param name="services">The services collection.</param>
        /// <param name="builderAction">The builder action.</param>
        /// <returns>The services collection.</returns>
        public static IServiceCollection AddEntityFrameworkCoreExtensions(this IServiceCollection services, Action<ExtensionsServicesBuilder> builderAction)
        {
            Ensure.NotNull(services, nameof(services));
            Ensure.NotNull(builderAction, nameof(builderAction));

            var builder = new ExtensionsServicesBuilder(services);
            builderAction(builder);

            return services;
        }
    }
}
