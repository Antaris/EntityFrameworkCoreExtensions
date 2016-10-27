// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Materialization
{
    using System;
    using System.Linq.Expressions;
    using Microsoft.EntityFrameworkCore.Metadata;

    /// <summary>
    /// Provides a base implementation of a <see cref="IEntityMaterializerSourceHook"/>.
    /// </summary>
    public class EntityMaterializerSourceHookBase : IEntityMaterializerSourceHook
    {
        /// <inheritdoc />
        public virtual Expression CreateMaterializeExpression(IEntityType entityType, Expression valueBuffer, int[] indexMap, Func<IEntityType, Expression, int[], Expression> defaultFactory)
            => null;

        /// <inheritdoc />
        public virtual Expression CreateReadValueCallExpression(Expression valueBuffer, int index, Func<Expression, int, Expression> defaultFactory)
            => null;

        /// <inheritdoc />
        public virtual Expression CreateReadValueExpression(Expression valueBuffer, Type type, int index, Func<Expression, Type, int, Expression> defaultFactory)
            => null;
    }
}
