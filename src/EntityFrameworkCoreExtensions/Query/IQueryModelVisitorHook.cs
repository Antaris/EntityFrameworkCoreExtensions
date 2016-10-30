// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Query
{
    using Microsoft.EntityFrameworkCore.Query;
    using Remotion.Linq;

    /// <summary>
    /// Defines the required contract for implementing a query model visitor hook.
    /// </summary>
    public interface IQueryModelVisitorHook
    {
        /// <summary>
        /// Fired before the visit model operation. 
        /// </summary>
        /// <param name="visitor">The query model visitor.</param>
        /// <param name="queryModel">The query model.</param>
        void VisitingQueryModel(EntityQueryModelVisitor visitor, QueryModel queryModel);

        /// <summary>
        /// Fired after the visit model operation.
        /// </summary>
        /// <param name="visitor">The query model visitor.</param>
        /// <param name="queryModel">The query model.</param>
        void VisitedQueryModel(EntityQueryModelVisitor visitor, QueryModel queryModel);
    }
}
