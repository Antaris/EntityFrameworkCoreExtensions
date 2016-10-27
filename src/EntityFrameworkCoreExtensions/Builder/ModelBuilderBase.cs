// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Builder
{
    using System;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Provides a base implementation of a model builder.
    /// </summary>
    public abstract class ModelBuilderBase : IModelBuilder
    {
        /// <inheritdoc />
        public virtual Type DbContextType => null;

        /// <inheritdoc />
        public virtual Type EntityType => null;

        /// <inheritdoc />
        public virtual bool IsTyped => false;

        /// <inheritdoc />
        public abstract void BuildModel(ModelBuilder builder);
    }

    /// <summary>
    /// Provides a base implementation of a model builder.
    /// </summary>
    /// <typeparam name="TContext">The context type.</typeparam>
    public abstract class ModelBuilderBase<TContext> : ModelBuilderBase
        where TContext : DbContext
    {
        /// <inheritdoc />
        public override Type DbContextType => typeof(TContext);

        /// <inheritdoc />
        public override bool IsTyped => true;
    }
}
