// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Builder
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Provides services for resolving model builders for a database context.
    /// </summary>
    public class ModelBuilderService : IModelBuilderService
    {
        private static readonly Type[] _supportedCollectionTypes = new[]
        {
            typeof(List<>),
            typeof(IList<>),
            typeof(ICollection<>)
        };

        private IModelBuilder[] _builders;

        /// <summary>
        /// Initialises a new instance of <see cref="ModelBuilderService"/>
        /// </summary>
        /// <param name="builders">The set of model builders.</param>
        public ModelBuilderService(IEnumerable<IModelBuilder> builders)
        {
            _builders = Ensure.NotNull(builders, nameof(builders)).ToArray();
        }

        /// <inheritdoc />
        public IModelBuilder[] GetModelBuilders(Type dbContextType, bool includeNavigations)
        {
            Ensure.NotNull(dbContextType, nameof(dbContextType));

            // MA - Get the untyped builders - these apply to all database contexts.
            var untypedBuilders = _builders.Where(b => !b.IsTyped);
            // MA - Get the typed builders for compatible database contexts.
            var contextTypedBuilders = _builders.Where(b => b.IsTyped && b.DbContextType != null && b.DbContextType.IsAssignableFrom(dbContextType));

            // MA - Get the entity types from DbSet<T> properties
            var setTypes = FindSets(dbContextType);
            // MA - Get the typed builders for properties on the database context.
            var setTypedBuilders = _builders.Where(b => b.IsTyped && b.EntityType != null && setTypes.Any(t => b.EntityType.Equals(t)));

            IEnumerable<IModelBuilder> navigationTypedBuilders = Enumerable.Empty<IModelBuilder>();
            if (includeNavigations)
            {
                // MA - Gets the navigations for the given entity types.
                var navigationTypes = FindNavigations(setTypes);
                // MA - Get the typed builders for navigations.
                navigationTypedBuilders = _builders.Where(b => b.IsTyped && b.EntityType != null && navigationTypes.Any(t => b.EntityType.Equals(t)));
            }

            return untypedBuilders
                .Concat(contextTypedBuilders)
                .Concat(setTypedBuilders)
                .Concat(navigationTypedBuilders)
                .ToArray();
        }

        private Type[] FindNavigations(Type[] setTypes)
        {
            var knownTypes = new HashSet<Type>(setTypes);
            var resultTypes = new HashSet<Type>();

            foreach (var setType in setTypes)
            {
                foreach (var resultType in FindNavigationsCore(setType, knownTypes))
                {
                    resultTypes.Add(resultType);
                }
            }

            return resultTypes.ToArray();
        }

        private IEnumerable<Type> FindNavigationsCore(Type type, HashSet<Type> knownTypes)
        {
            foreach (var navigationType in FindNavigationCandidates(type))
            {
                if (!knownTypes.Contains(navigationType))
                {
                    knownTypes.Add(navigationType);

                    yield return navigationType;

                    foreach (var subNavigationType in FindNavigationsCore(navigationType, knownTypes))
                    {
                        yield return subNavigationType;
                    }
                }
            }
        }

        private Type[] FindNavigationCandidates(Type entityType)
            => entityType.GetRuntimeProperties()
                .Where(
                    p => !IsStatic(p)
                         && !p.PropertyType.GetTypeInfo().IsValueType)
                .Select(p => ExtractNavigationType(p.PropertyType))
                .Where(p => p != null)
                .ToArray();
                    

        private Type[] FindSets(Type dbContextType)
            => dbContextType.GetRuntimeProperties()
                .Where(
                    p => !IsStatic(p)
                         && !p.GetIndexParameters().Any()
                         && p.DeclaringType != typeof(DbContext)
                         && p.DeclaringType != typeof(ExtendedDbContext)
                         && p.PropertyType.GetTypeInfo().IsGenericType
                         && p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>))
                .Select(p => p.PropertyType.GetGenericArguments()[0])
                .ToArray();

        private bool IsStatic(PropertyInfo property)
            => (property.GetMethod ?? property.SetMethod).IsStatic;

        private Type ExtractNavigationType(Type propertyType)
        {
            var typeInfo = propertyType.GetTypeInfo();

            if (typeInfo.IsValueType
                || propertyType == typeof(string))
            {
                return null;
            }

            if (typeInfo.IsGenericType && IsSupportedCollectionType(typeInfo))
            {
                return propertyType.GetGenericArguments()[0];
            }
            else if (!typeInfo.IsGenericType)
            {
                return propertyType;
            }

            return null;
        }

        private bool IsSupportedCollectionType(TypeInfo typeInfo)
            => _supportedCollectionTypes.Any(t => t.Equals(typeInfo.GetGenericTypeDefinition()));
    }
}
