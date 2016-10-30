// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Query
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
    using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
    using Microsoft.EntityFrameworkCore.Query.Internal;

    /// <summary>
    /// A factory used to create instances of <see cref="ExtendedSqlServerQueryModelVisitor"/>
    /// </summary>
    public class ExtendedSqlServerQueryModelVisitorFactory : SqlServerQueryModelVisitorFactory
    {
        private ActionExecutor<IQueryModelVisitorHook> _executor;

        /// <summary>
        /// Initialises a new instance of <see cref="ExtendedSqlServerQueryModelVisitorFactory"/>.
        /// </summary>
        public ExtendedSqlServerQueryModelVisitorFactory(
            IQueryOptimizer queryOptimizer, 
            INavigationRewritingExpressionVisitorFactory navigationRewritingExpressionVisitorFactory, 
            ISubQueryMemberPushDownExpressionVisitor subQueryMemberPushDownExpressionVisitor, 
            IQuerySourceTracingExpressionVisitorFactory querySourceTracingExpressionVisitorFactory, 
            IEntityResultFindingExpressionVisitorFactory entityResultFindingExpressionVisitorFactory, 
            ITaskBlockingExpressionVisitor taskBlockingExpressionVisitor, 
            IMemberAccessBindingExpressionVisitorFactory memberAccessBindingExpressionVisitorFactory, 
            IOrderingExpressionVisitorFactory orderingExpressionVisitorFactory, 
            IProjectionExpressionVisitorFactory projectionExpressionVisitorFactory, 
            IEntityQueryableExpressionVisitorFactory entityQueryableExpressionVisitorFactory, 
            IQueryAnnotationExtractor queryAnnotationExtractor, 
            IResultOperatorHandler resultOperatorHandler, 
            IEntityMaterializerSource entityMaterializerSource, 
            IExpressionPrinter expressionPrinter, 
            IRelationalAnnotationProvider relationalAnnotationProvider, 
            IIncludeExpressionVisitorFactory includeExpressionVisitorFactory, 
            ISqlTranslatingExpressionVisitorFactory sqlTranslatingExpressionVisitorFactory, 
            ICompositePredicateExpressionVisitorFactory compositePredicateExpressionVisitorFactory, 
            IConditionalRemovingExpressionVisitorFactory conditionalRemovingExpressionVisitorFactory, 
            IQueryFlattenerFactory queryFlattenerFactory, 
            IDbContextOptions contextOptions,
            IEnumerable<IQueryModelVisitorHook> hooks) 
            : base(queryOptimizer, navigationRewritingExpressionVisitorFactory, subQueryMemberPushDownExpressionVisitor, querySourceTracingExpressionVisitorFactory, 
                  entityResultFindingExpressionVisitorFactory, taskBlockingExpressionVisitor, memberAccessBindingExpressionVisitorFactory, orderingExpressionVisitorFactory, 
                  projectionExpressionVisitorFactory, entityQueryableExpressionVisitorFactory, queryAnnotationExtractor, resultOperatorHandler, entityMaterializerSource, 
                  expressionPrinter, relationalAnnotationProvider, includeExpressionVisitorFactory, sqlTranslatingExpressionVisitorFactory, 
                  compositePredicateExpressionVisitorFactory, conditionalRemovingExpressionVisitorFactory, queryFlattenerFactory, contextOptions)
        {
            _executor = new ActionExecutor<IQueryModelVisitorHook>(Ensure.NotNull(hooks, nameof(hooks)).ToArray());
        }

        /// <inheritdoc />
        public override EntityQueryModelVisitor Create(QueryCompilationContext queryCompilationContext, EntityQueryModelVisitor parentEntityQueryModelVisitor)
            => new ExtendedSqlServerQueryModelVisitor(
                QueryOptimizer, NavigationRewritingExpressionVisitorFactory, SubQueryMemberPushDownExpressionVisitor,
                QuerySourceTracingExpressionVisitorFactory, EntityResultFindingExpressionVisitorFactory,
                TaskBlockingExpressionVisitor, MemberAccessBindingExpressionVisitorFactory,
                OrderingExpressionVisitorFactory, ProjectionExpressionVisitorFactory, EntityQueryableExpressionVisitorFactory,
                QueryAnnotationExtractor, ResultOperatorHandler, EntityMaterializerSource, ExpressionPrinter, RelationalAnnotationProvider,
                IncludeExpressionVisitorFactory, SqlTranslatingExpressionVisitorFactory, CompositePredicateExpressionVisitorFactory, ConditionalRemovingExpressionVisitorFactory,
                QueryFlattenerFactory, ContextOptions, (RelationalQueryCompilationContext)queryCompilationContext, 
                (SqlServerQueryModelVisitor)parentEntityQueryModelVisitor, _executor);
    }
}
