// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions
{
    using Microsoft.Extensions.DependencyInjection;
    
    using EntityFrameworkCoreExtensions.ChangeTracking;
    using EntityFrameworkCoreExtensions.Materialization;
    using EntityFrameworkCoreExtensions.Mixins;
    using EntityFrameworkCoreExtensions.Query;

    /// <summary>
    /// Adds the mixins feature to the internal service provider.
    /// </summary>
    public class MixinsDbContextOptionsExtension : DbContextOptionsExtensionBase
    {
        /// <inheritdoc />
        public override void ApplyServicesCore(IServiceCollection services)
        {
            services.AddScoped<IChangeDetectorHook, MixinChangeDetectorHook>();
            services.AddScoped<IQueryModelVisitorHook, MixinQueryModelVisitorHook>();
            services.AddScoped<IEntityMaterializerSourceHook, MixinEntityMaterializerSourceHook>();
        }
    }
}
