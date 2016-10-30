// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Query
{
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;
    using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors.Internal;
    using Microsoft.EntityFrameworkCore.Query.Internal;
    using Remotion.Linq;

    /// <summary>
    /// A query model visitor with supports for hooks.
    /// </summary>
    public class ExtendedSqlServerQueryModelVisitor : SqlServerQueryModelVisitor
    {
        private readonly ActionExecutor<IQueryModelVisitorHook> _executor;

        /// <summary>
        /// Initialises a new instance of <see cref="ExtendedSqlServerQueryModelVisitor" />
        /// </summary>
        public ExtendedSqlServerQueryModelVisitor(
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
            RelationalQueryCompilationContext queryCompilationContext, 
            SqlServerQueryModelVisitor parentQueryModelVisitor,
            ActionExecutor<IQueryModelVisitorHook> hookActionExecutor) 
            : base(queryOptimizer, navigationRewritingExpressionVisitorFactory, subQueryMemberPushDownExpressionVisitor, querySourceTracingExpressionVisitorFactory, 
                   entityResultFindingExpressionVisitorFactory, taskBlockingExpressionVisitor, memberAccessBindingExpressionVisitorFactory, orderingExpressionVisitorFactory, 
                   projectionExpressionVisitorFactory, entityQueryableExpressionVisitorFactory, queryAnnotationExtractor, resultOperatorHandler, entityMaterializerSource, 
                   expressionPrinter, relationalAnnotationProvider, includeExpressionVisitorFactory, sqlTranslatingExpressionVisitorFactory, 
                   compositePredicateExpressionVisitorFactory, conditionalRemovingExpressionVisitorFactory, queryFlattenerFactory, contextOptions, queryCompilationContext, 
                   parentQueryModelVisitor)
        {
            _executor = Ensure.NotNull(hookActionExecutor, nameof(hookActionExecutor));
        }

        /// <inheritdoc />
        public override void VisitQueryModel(QueryModel queryModel)
        {
            _executor.Execute(hook => hook.VisitingQueryModel(this, queryModel));

            base.VisitQueryModel(queryModel);

            _executor.Execute(hook => hook.VisitingQueryModel(this, queryModel));
        }
    }
}
