// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Builder
{
    using System;
    using System.Linq.Expressions;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.EntityFrameworkCore.Internal;

    using EntityFrameworkCoreExtensions.Mixins;

    /// <summary>
    /// Provides model building services for defining mixins.
    /// </summary>
    /// <typeparam name="TMixin">The mixin type.</typeparam>
    public class MixinTypeBuilder<TMixin> where TMixin : class
    {
        private readonly EntityTypeBuilder _entityTypeBuilder;

        /// <summary>
        /// Initialises a new instance of <see cref="MixinTypeBuilder{TMixin}"/>
        /// </summary>
        /// <param name="entityTypBuilder">The parent entity type builder.</param>
        public MixinTypeBuilder(EntityTypeBuilder entityTypBuilder)
        {
            _entityTypeBuilder = Ensure.NotNull(entityTypBuilder, nameof(entityTypBuilder));
        }

        /// <summary>
        /// Defines a property for the mixin.
        /// </summary>
        /// <typeparam name="TProperty">The property type.</typeparam>
        /// <param name="propertyExpression">The property selector expression.</param>
        /// <returns>The property builder.</returns>
        public PropertyBuilder<TProperty> Property<TProperty>(Expression<Func<TMixin, TProperty>> propertyExpression)
        {
            Ensure.NotNull(propertyExpression, nameof(propertyExpression));

            // MA - Get the property name.
            var name = propertyExpression.GetPropertyAccess().Name;
            // MA - Define the shadow property name.
            string shadowPropertyName = $"{typeof(TMixin).GetMixinPrefix()}_{name}";

            // MA - Create and return a property builder from the parent entity type builder.
            return _entityTypeBuilder.Property<TProperty>(shadowPropertyName);
        }
    }
}
