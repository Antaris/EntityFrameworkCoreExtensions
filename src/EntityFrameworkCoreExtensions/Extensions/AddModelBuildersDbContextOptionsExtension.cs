// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;

    using EntityFrameworkCoreExtensions.Builder;

    /// <summary>
    /// Adds any discovered model builders to the internal services provider.
    /// </summary>
    public class AddModelBuildersDbContextOptionsExtension : DbContextOptionsExtensionBase
    {
        private Assembly[] _assemblies;
        private const string ExtensionsAssemblyNamePrefix = "EntityFrameworkCoreExtensions";

        /// <summary>
        /// Initialises a new instance of <see cref="AddModelBuildersDbContextOptionsExtension"/>
        /// </summary>
        /// <param name="assemblies">The set of assemblies.</param>
        public AddModelBuildersDbContextOptionsExtension(Assembly[] assemblies)
        {
            _assemblies = Ensure.NotNull(assemblies, nameof(assemblies));
        }

        /// <summary>
        /// Initialises a new instance of <see cref="AddModelBuildersDbContextOptionsExtension"/> from an existing <see cref="AddModelBuildersDbContextOptionsExtension"/>
        /// </summary>
        /// <param name="other">The other extension.</param>
        /// <param name="assemblies">The set of assemblies.</param>
        public AddModelBuildersDbContextOptionsExtension(AddModelBuildersDbContextOptionsExtension other, Assembly[] assemblies)
        {
            Ensure.NotNull(other, nameof(other));

            _assemblies = other._assemblies.Concat(Ensure.NotNull(assemblies, nameof(assemblies))).ToArray();
        }

        /// <inheritdoc />
        public override void ApplyServicesCore(IServiceCollection services)
        {
            var modelBuilderInterfaceType = typeof(IModelBuilder);

            foreach (var assembly in _assemblies)
            {
                // MA - Skip the extensions assemblies
                if (assembly.GetName().Name.StartsWith(ExtensionsAssemblyNamePrefix, StringComparison.Ordinal))
                {
                    continue;
                }

                var modelBuilderTypes = assembly.GetExportedTypes()
                    .Select(t => t.GetTypeInfo())
                    .Where(
                        ti => !ti.IsAbstract
                              && ti.IsClass
                              && ti.IsPublic
                              && ti.ImplementedInterfaces.Any(i => modelBuilderInterfaceType.Equals(i)));

                foreach (var modelBuilderType in modelBuilderTypes)
                {
                    services.AddSingleton(modelBuilderInterfaceType, modelBuilderType.AsType());
                }
            }
        }
    }
}
