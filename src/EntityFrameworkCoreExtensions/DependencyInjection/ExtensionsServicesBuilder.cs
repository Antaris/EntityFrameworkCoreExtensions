// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions
{
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;

    using EntityFrameworkCoreExtensions.Builder;

    /// <summary>
    /// A builder utility for applying service descriptors to a service collection.
    /// </summary>
    public class ExtensionsServicesBuilder
    {
        private readonly IServiceCollection _services;

        /// <summary>
        /// Initialises a new instance of <see cref="ExtensionsServicesBuilder"/>.
        /// </summary>
        /// <param name="services">The services collection.</param>
        public ExtensionsServicesBuilder(IServiceCollection services)
        {
            _services = Ensure.NotNull(services, nameof(services));
        }

        /// <summary>
        /// Gets the set of services.
        /// </summary>
        public IServiceCollection Services => _services;

        /// <summary>
        /// Adds the auto-model feature to the services collection.
        /// </summary>
        /// <returns>The service builder.</returns>
        public ExtensionsServicesBuilder AddAutoModel()
        {
            _services.AddSingleton<IModelBuilderService, ModelBuilderService>();
            _services.AddScoped<IDbContextHook, AutoModelDbContextHook>();

            return this;
        }

        /// <summary>
        /// Adds the model builders from the given assembly.
        /// </summary>
        /// <param name="assembly">The assembly instance.</param>
        /// <returns>The service builder.</returns>
        public ExtensionsServicesBuilder AddModelBuildersFromAssembly(Assembly assembly)
        {
            Ensure.NotNull(assembly, nameof(assembly));

            var modelBuilderInterfaceType = typeof(IModelBuilder);

            var modelBuilderTypes = assembly.GetExportedTypes()
                .Select(t => t.GetTypeInfo())
                .Where(
                    ti => !ti.IsAbstract
                          && ti.IsClass
                          && ti.IsPublic
                          && ti.ImplementedInterfaces.Any(i => modelBuilderInterfaceType.Equals(i)));

            foreach (var modelBuilderType in modelBuilderTypes)
            {
                _services.AddSingleton(modelBuilderInterfaceType, modelBuilderType.AsType());
            }

            return this;
        }
    }
}
