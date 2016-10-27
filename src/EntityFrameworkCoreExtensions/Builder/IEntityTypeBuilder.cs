// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Builder
{
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Defines the required contract for implementing an entity type builder.
    /// </summary>
    public interface IEntityTypeBuilder<T> : IModelBuilder
        where T : class
    {
        /// <summary>
        /// Builds the entity model.
        /// </summary>
        /// <param name="builder">The entity type builder.</param>
        void BuildEntity(EntityTypeBuilder<T> builder);
    }
}
