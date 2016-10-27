// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Builder
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Provides a base implementation of an entity type builder.
    /// </summary>
    /// <typeparam name="TEntity">The entity type.</typeparam>
    public abstract class EntityTypeBuilderBase<TEntity> : ModelBuilderBase, IEntityTypeBuilder<TEntity>
        where TEntity : class
    {
        /// <inheritdoc />
        public override Type EntityType => typeof(TEntity);

        /// <inheritdoc />
        public override bool IsTyped => true;

        /// <inheritdoc />
        public override void BuildModel(ModelBuilder builder)
        {
            Ensure.NotNull(builder, nameof(builder));

            BuildEntity(builder.Entity<TEntity>());
        }

        /// <inheritdoc />
        public abstract void BuildEntity(EntityTypeBuilder<TEntity> builder);
    }
}
