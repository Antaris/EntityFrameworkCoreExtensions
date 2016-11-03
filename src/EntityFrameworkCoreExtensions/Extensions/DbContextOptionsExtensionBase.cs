// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions
{
    using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using Microsoft.Extensions.DependencyInjection;
    
    using EntityFrameworkCoreExtensions.ChangeTracking;
    using EntityFrameworkCoreExtensions.Materialization;

    /// <summary>
    /// Provides an extension that registers services with the internal services provider.
    /// </summary>
    public abstract class DbContextOptionsExtensionBase : IDbContextOptionsExtension
    {
        private static readonly ServiceDescriptor _markerServiceDescriptor = ServiceDescriptor.Singleton<ExtensionsMarkerService, ExtensionsMarkerService>();

        /// <inheritdoc />
        public void ApplyServices(IServiceCollection services)
        {
            // MA - Ensure the core services are applied.
            EnsureCoreServices(services);

            // MA - Apply the extension specific services.
            ApplyServicesCore(services);
        }

        /// <summary>
        /// Applies the extension specific services.
        /// </summary>
        /// <param name="services">The service collection.</param>
        public abstract void ApplyServicesCore(IServiceCollection services);

        private void EnsureCoreServices(IServiceCollection services)
        {
            if (!services.Contains(_markerServiceDescriptor))
            {
                services.AddScoped<IChangeDetector, ExtendedChangeDetector>();
                services.AddSingleton<IEntityMaterializerSource, ExtendedEntityMaterializerSource>();

                services.Add(_markerServiceDescriptor);
            }
        }

        private class ExtensionsMarkerService { }
    }
}
