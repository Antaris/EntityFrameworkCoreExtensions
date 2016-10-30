// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Query
{
    using Microsoft.EntityFrameworkCore.Query;
    using Remotion.Linq;

    /// <summary>
    /// Provides a base implemenation of a query model visitor hook.
    /// </summary>
    public abstract class QueryModelVisitorHookBase : IQueryModelVisitorHook
    {
        /// <inhertidoc />
        public virtual void VisitedQueryModel(EntityQueryModelVisitor visitor, QueryModel queryModel)
        { }

        /// <inhertidoc />
        public virtual void VisitingQueryModel(EntityQueryModelVisitor visitor, QueryModel queryModel)
        { }
    }
}
