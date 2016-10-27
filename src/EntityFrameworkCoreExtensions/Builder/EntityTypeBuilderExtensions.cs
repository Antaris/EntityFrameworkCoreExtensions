// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Builder
{
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Provides extensions for the <see cref="EntityTypeBuilder"/> type.
    /// </summary>
    public static class EntityTypeBuilderExtensions
    {
        /// <summary>
        /// Returns a <see cref="MixinTypeBuilder{TMixin}"/> for the given type.
        /// </summary>
        /// <typeparam name="TMixin">The mixin type.</typeparam>
        /// <param name="entityTypeBuilder">The entity type builder.</param>
        /// <returns>The mixin type builder.</returns>
        public static MixinTypeBuilder<TMixin> Mixin<TMixin>(this EntityTypeBuilder entityTypeBuilder)
            where TMixin : class
            => new MixinTypeBuilder<TMixin>(Ensure.NotNull(entityTypeBuilder, nameof(entityTypeBuilder)));
    }
}
