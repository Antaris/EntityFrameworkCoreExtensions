// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.Extensions.DependencyInjection;
    
    using EntityFrameworkCoreExtensions.ChangeTracking;
    using EntityFrameworkCoreExtensions.Materialization;
    using EntityFrameworkCoreExtensions.Query;
    using EntityFrameworkCoreExtensions.Storage;

    /// <summary>
    /// Adds any discovered hooks to the internal service provider.
    /// </summary>
    public class AddHooksDbContextOptionsExtension : DbContextOptionsExtensionBase
    {
        private static readonly Dictionary<Type, ServiceLifetime> _hookTypes = new Dictionary<Type, ServiceLifetime>
        {
            [typeof(IDbContextHook)] = ServiceLifetime.Scoped,
            [typeof(IChangeDetectorHook)] = ServiceLifetime.Scoped,
            [typeof(IEntityMaterializerSourceHook)] = ServiceLifetime.Singleton,
            [typeof(IQueryModelVisitorHook)] = ServiceLifetime.Scoped,
            [typeof(IRelationalTypeMapperHook)] = ServiceLifetime.Singleton
        };
        private Assembly[] _assemblies;
        private const string ExtensionsAssemblyNamePrefix = "EntityFrameworkCoreExtensions";

        /// <summary>
        /// Initialises a new instance of <see cref="AddHooksDbContextOptionsExtension"/>
        /// </summary>
        /// <param name="assemblies">The set of assemblies.</param>
        public AddHooksDbContextOptionsExtension(Assembly[] assemblies)
        {
            _assemblies = Ensure.NotNull(assemblies, nameof(assemblies));
        }

        /// <summary>
        /// Initialises a new instance of <see cref="AddHooksDbContextOptionsExtension"/> from an existing <see cref="AddHooksDbContextOptionsExtension"/>
        /// </summary>
        /// <param name="other">The other extension.</param>
        /// <param name="assemblies">The set of assemblies.</param>
        public AddHooksDbContextOptionsExtension(AddHooksDbContextOptionsExtension other, Assembly[] assemblies)
        {
            Ensure.NotNull(other, nameof(other));

            _assemblies = other._assemblies.Concat(Ensure.NotNull(assemblies, nameof(assemblies))).ToArray();
        }

        /// <inheritdoc />
        public override void ApplyServicesCore(IServiceCollection services)
        {
            // MA - Resolve all the hook implementation types.
            var implementationTypes = _assemblies
                .Where(a => a.GetName().Name.StartsWith(ExtensionsAssemblyNamePrefix, StringComparison.Ordinal))
                .SelectMany(a => a.GetExportedTypes())
                .Select(t => t.GetTypeInfo())
                .Where(
                    ti => !ti.IsAbstract
                          && ti.IsClass
                          && ti.IsPublic
                          && ti.ImplementedInterfaces.Any(i => _hookTypes.Keys.Contains(i)));

            foreach (var implementationType in implementationTypes)
            {
                foreach (var interfaceType in implementationType.ImplementedInterfaces)
                {
                    ServiceLifetime lifetime;
                    if (_hookTypes.TryGetValue(interfaceType, out lifetime))
                    {
                        services.Add(ServiceDescriptor.Describe(interfaceType, implementationType.AsType(), lifetime));
                    }
                }
            }
        }
    }
}
