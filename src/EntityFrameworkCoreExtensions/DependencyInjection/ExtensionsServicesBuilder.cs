// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using Microsoft.Extensions.DependencyInjection;

    using EntityFrameworkCoreExtensions.Builder;
    using EntityFrameworkCoreExtensions.ChangeTracking;
    using EntityFrameworkCoreExtensions.Materialization;
    using EntityFrameworkCoreExtensions.Mixins;
    using EntityFrameworkCoreExtensions.Query;
    using EntityFrameworkCoreExtensions.Storage;

    /// <summary>
    /// A builder utility for applying service descriptors to a service collection.
    /// </summary>
    public class ExtensionsServicesBuilder
    {
        private readonly IServiceCollection _services;
        
        private static readonly Dictionary<Type, ServiceLifetime> _hookTypes = new Dictionary<Type, ServiceLifetime>
        {
            [typeof(IDbContextHook)] = ServiceLifetime.Scoped,
            [typeof(IChangeDetectorHook)] = ServiceLifetime.Scoped,
            [typeof(IEntityMaterializerSourceHook)] = ServiceLifetime.Singleton,
            [typeof(IQueryModelVisitorHook)] = ServiceLifetime.Scoped,
            [typeof(IRelationalTypeMapperHook)] = ServiceLifetime.Singleton
        };
        private const string ExtensionsAssemblyNamePrefix = "EntityFrameworkCoreExtensions";
        private static readonly ServiceDescriptor _markerServiceDescriptor = ServiceDescriptor.Singleton<ExtensionsMarkerService, ExtensionsMarkerService>();

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
            EnsureCoreServices();

            _services.AddSingleton<IModelBuilderService, ModelBuilderService>();
            _services.AddScoped<IDbContextHook, AutoModelDbContextHook>();

            return this;
        }

        /// <summary>
        /// Adds the mixins feature to the services collection.
        /// </summary>
        /// <returns>The service builder.</returns>
        public ExtensionsServicesBuilder AddMixins()
        {
            EnsureCoreServices();

            _services.AddScoped<IChangeDetectorHook, MixinChangeDetectorHook>();
            _services.AddScoped<IQueryModelVisitorHook, MixinQueryModelVisitorHook>();
            _services.AddScoped<IEntityMaterializerSourceHook, MixinEntityMaterializerSourceHook>();

            return this;
        }

        /// <summary>
        /// Adds the model builders from the given assemblies.
        /// </summary>
        /// <param name="assemblies">The assembly instances.</param>
        /// <returns>The service builder.</returns>
        public ExtensionsServicesBuilder AddModelBuildersFromAssemblies(params Assembly[] assemblies)
        {
            EnsureCoreServices();

            if (assemblies == null || assemblies.Length == 0)
            {
                return this;
            }

            var modelBuilderInterfaceType = typeof(IModelBuilder);

            foreach (var assembly in assemblies)
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
                    _services.AddSingleton(modelBuilderInterfaceType, modelBuilderType.AsType());
                }
            }

            return this;
        }

        /// <summary>
        /// Adds the hooks from the given assemblies.
        /// </summary>
        /// <param name="assemblies">The assembly instances.</param>
        /// <returns>The service builder.</returns>
        public ExtensionsServicesBuilder AddHooksFromAssemblies(params Assembly[] assemblies)
        {
            EnsureCoreServices();

            if (assemblies == null || assemblies.Length == 0)
            {
                return this;
            }

            // MA - Resolve all the hook implementation types.
            var implementationTypes = assemblies
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
                        _services.Add(ServiceDescriptor.Describe(interfaceType, implementationType.AsType(), lifetime));
                    }
                }
            }

            return this;
        }

        private void EnsureCoreServices()
        {
            if (!_services.Contains(_markerServiceDescriptor))
            {
                _services.AddScoped<IChangeDetector, ExtendedChangeDetector>();
                _services.AddSingleton<IEntityMaterializerSource, ExtendedEntityMaterializerSource>();

                _services.Add(_markerServiceDescriptor);
            }
        }

        private class ExtensionsMarkerService { }
    }
}
