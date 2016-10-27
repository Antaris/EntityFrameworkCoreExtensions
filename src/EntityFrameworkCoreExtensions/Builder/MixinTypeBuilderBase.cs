// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Builder
{
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Provides a base implementation of a mixin type builder.
    /// </summary>
    public abstract class MixinTypeBuilderBase<TEntity, TMixin> : EntityTypeBuilderBase<TEntity>
        where TEntity : class
        where TMixin : class
    {
        /// <inheritdoc />
        public override void BuildEntity(EntityTypeBuilder<TEntity> builder)
        {
            Ensure.NotNull(builder, nameof(builder));

            // MA - Create a mixin type builder.
            var mixinBuilder = new MixinTypeBuilder<TMixin>(builder);

            // MA - Configure the mixin.
            BuildMixin(mixinBuilder);
        }

        /// <summary>
        /// Builds the mixin model.
        /// </summary>
        /// <param name="builder">The mixin type builder.</param>
        public abstract void BuildMixin(MixinTypeBuilder<TMixin> builder);
    }
}
