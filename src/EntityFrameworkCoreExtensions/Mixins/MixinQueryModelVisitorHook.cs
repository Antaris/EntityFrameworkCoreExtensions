// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Mixins
{
    using Microsoft.EntityFrameworkCore.Query;
    using Remotion.Linq;

    using EntityFrameworkCoreExtensions.Query;

    /// <summary>
    /// Transoforms mixin methods and member calls in the query model.
    /// </summary>
    public class MixinQueryModelVisitorHook : QueryModelVisitorHookBase
    {
        /// <inheritdoc />
        public override void VisitingQueryModel(EntityQueryModelVisitor visitor, QueryModel queryModel)
            => queryModel.TransformExpressions(e =>
                new MixinExpressionVisitor(visitor.QueryCompilationContext.Model).Visit(e));
    }
}
