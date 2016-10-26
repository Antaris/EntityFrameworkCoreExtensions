// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Materialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;

    /// <summary>
    /// An extended entity materializer source with support for hooks.
    /// </summary>
    public class ExtendedEntityMaterializerSource : EntityMaterializerSource
    {
        private readonly FirstResultFuncExecutor<IEntityMaterializerSourceHook> _executor;

        /// <summary>
        /// Initialises a new instance of <see cref="ExtendedEntityMaterializerSource"/>.
        /// </summary>
        /// <param name="memberMapper">The member mapper.</param>
        /// <param name="hooks">The set of entity materializer source hooks.</param>
        public ExtendedEntityMaterializerSource(IMemberMapper memberMapper, IEnumerable<IEntityMaterializerSourceHook> hooks)
            : base(memberMapper)
        {
            _executor = new FirstResultFuncExecutor<IEntityMaterializerSourceHook>(Ensure.NotNull(hooks, nameof(hooks)).ToArray());
        }

        /// <inheritdoc />
        public override Expression CreateMaterializeExpression(IEntityType entityType, Expression valueBufferExpression, int[] indexMap = null)
            => _executor.Execute(hook => hook.CreateMaterializeExpression(
                entityType,
                valueBufferExpression,
                indexMap,
                (e, v, im) => base.CreateMaterializeExpression(e, v, im)))
            ?? base.CreateMaterializeExpression(entityType, valueBufferExpression, indexMap);

        /// <inheritdoc />
        public override Expression CreateReadValueCallExpression(Expression valueBuffer, int index)
            => _executor.Execute(hook => hook.CreateReadValueCallExpression(
                valueBuffer,
                index,
                (v, i) => base.CreateReadValueCallExpression(v, i)))
            ?? base.CreateReadValueCallExpression(valueBuffer, index);

        /// <inheritdoc />
        public override Expression CreateReadValueExpression(Expression valueBuffer, Type type, int index)
            => _executor.Execute(hook => hook.CreateReadValueExpression(
                valueBuffer,
                type,
                index,
                (v, t, i) => base.CreateReadValueExpression(v, t, i)))
            ?? base.CreateReadValueExpression(valueBuffer, type, index);
    }
}
