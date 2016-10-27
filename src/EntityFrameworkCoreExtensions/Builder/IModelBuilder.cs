// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Builder
{
    using System;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Defines the required contract for implementing a model builder.
    /// </summary>
    public interface IModelBuilder
    {
        /// <summary>
        /// Gets the database context type.
        /// </summary>
        Type DbContextType { get; }

        /// <summary>
        /// Gets the entity type.
        /// </summary>
        Type EntityType { get; }

        /// <summary>
        /// Gets whether this is a typed model builder.
        /// </summary>
        bool IsTyped { get; }

        /// <summary>
        /// Builds the model.
        /// </summary>
        /// <param name="builder">The <see cref="ModelBuilder" /> instance.</param>
        void BuildModel(ModelBuilder builder);
    }
}
