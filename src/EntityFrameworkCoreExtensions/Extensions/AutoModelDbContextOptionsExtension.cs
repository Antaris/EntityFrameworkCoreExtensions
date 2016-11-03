// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions
{
    using Microsoft.Extensions.DependencyInjection;

    using EntityFrameworkCoreExtensions.Builder;

    /// <summary>
    /// Adds the auto-model extension.
    /// </summary>
    public class AutoModelDbContextOptionsExtension : DbContextOptionsExtensionBase
    {
        /// <inheritdoc />
        public override void ApplyServicesCore(IServiceCollection services)
        {
            services.AddSingleton<IModelBuilderService, ModelBuilderService>();
            services.AddScoped<IDbContextHook, AutoModelDbContextHook>();
        }
    }
}
