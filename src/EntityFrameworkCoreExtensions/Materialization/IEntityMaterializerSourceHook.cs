// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Materialization
{
    using System;
    using System.Linq.Expressions;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Storage;

    /// <summary>
    /// Defines the required contract for implementing an entity materializer source hook.
    /// </summary>
    public interface IEntityMaterializerSourceHook
    {
        /// <summary>
        /// Creates the materialize expression
        /// </summary>
        /// <param name="entityType">The entity type</param>
        /// <param name="valueBuffer">The value buffer expression.</param>
        /// <param name="indexMap">The index map</param>
        /// <param name="defaultFactory">The factory used to create a default read value expression.</param>
        /// <returns>The materialize expression.</returns>
        Expression CreateMaterializeExpression(IEntityType entityType, Expression valueBuffer, int[] indexMap, Func<IEntityType, Expression, int[], Expression> defaultFactory);

        /// <summary>
        /// Creates a read value expression that allows a value to be read from the value buffer.
        /// </summary>
        /// <param name="valueBuffer">The value buffer expression.</param>
        /// <param name="type">The property type.</param>
        /// <param name="index">The value index within the buffer.</param>
        /// <param name="defaultFactory">The factory used to create a default read value expression.</param>
        /// <returns>The read value expression.</returns>
        Expression CreateReadValueExpression(Expression valueBuffer, Type type, int index, Func<Expression, Type, int, Expression> defaultFactory);

        /// <summary>
        /// Creates a read value call expression for the <see cref="ValueBuffer"/> indexer getter.
        /// </summary>
        /// <param name="valueBuffer">The value buffer expression.</param>
        /// <param name="index">The value index within the buffer.</param>
        /// <param name="defaultFactory">The factory used to create a default read value call expression.</param>
        /// <returns>The read value call expression.</returns>
        Expression CreateReadValueCallExpression(Expression valueBuffer, int index, Func<Expression, int, Expression> defaultFactory);
    }
}
