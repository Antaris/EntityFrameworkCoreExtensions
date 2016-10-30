// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Mixins
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Query;
    using Microsoft.EntityFrameworkCore.Query.ExpressionVisitors;

    /// <summary>
    /// Transforms mixin expressions
    /// </summary>
    public class MixinExpressionVisitor : ExpressionVisitorBase
    {
        private readonly IModel _model;

        /// <summary>
        /// Initialises a new instance of <see cref="MixinExpressionVisitor"/>
        /// </summary>
        /// <param name="model">The entity model</param>
        public MixinExpressionVisitor(IModel model)
        {
            _model = Ensure.NotNull(model, nameof(model));
        }

        /// <inheritdoc />
        protected override Expression VisitMember(MemberExpression node)
        {
            /* MA - We'll transform calls to e.Mixin<T>().Property to
             *      EF.Property<Type>(e"<prefix>_<name>")
             */
            var method = node.Expression as MethodCallExpression;
            if (method != null && method.Method.IsGenericMethod && method.Method.Name == nameof(IMixinHost.Mixin))
            {
                return MixinPropertyToShadowProperty(node);
            }

            return base.VisitMember(node);
        }

        /// <inheritdoc />
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            /* MA - We'll transform calls to e.Mixin<T>() to
             *      new T { Property = EF.Property<Type>(e, "<prefix>_<name>") }
             */
            if (node.Method.IsGenericMethod && node.Method.Name == nameof(IMixinHost.Mixin))
            {
                return MixinMethodToObject(node);
            }
            return base.VisitMethodCall(node);
        }

        private Expression MixinPropertyToShadowProperty(MemberExpression expression)
        {
            // MA - Mixin<Type>()
            var method = (MethodCallExpression)expression.Expression;

            // MA - [e]
            var target = method.Object;

            // MA - MixinType_PropertyName
            string shadowPropertyName = $"{target.Type.GetMixinPrefix()}_{expression.Member.Name}";

            // MA - Get the entity type from the model.
            var entityType = _model.FindEntityType(target.Type);

            if (entityType == null)
            {
                throw new InvalidOperationException($"The entity type '{target.Type}' is not supported by the entity model.");
            }

            // MA - Get the property from the entity type.
            var property = entityType.FindProperty(shadowPropertyName);

            if (property == null)
            {
                throw new InvalidOperationException($"The property '{shadowPropertyName}' is not supported by the entity model for entity '{target.Type}'.");
            }

            // MA - EF.Property<Type>([e], <shadowPropertyName>)
            return EntityQueryModelVisitor.CreatePropertyExpression(target, property);
        }

        private Expression MixinMethodToObject(MethodCallExpression expression)
        {
            // MA - Get the mixin type and entity types.
            var mixinType = expression.Type;
            var entityType = expression.Object.Type;

            string prefix = $"{mixinType.GetMixinPrefix()}_";

            // MA - Get the entity from the model.
            var entity = _model.FindEntityType(entityType);

            if (entityType == null)
            {
                throw new InvalidOperationException($"The entity type '{entityType}' is not supported by the entity model.");
            }

            // MA - Get the available shadow properties.
            var properties = entity.GetProperties()
                .Where(p => p.IsShadowProperty && p.Name.StartsWith(prefix, StringComparison.Ordinal))
                .ToList();

            // MA - new Mixin()
            var ctor = Expression.New(mixinType);
            var bindings = new MemberBinding[properties.Count];
            for (int i = 0; i < properties.Count; i++)
            {
                var property = properties[i];
                string shadowPropertyName = property.Name.Substring(prefix.Length + 1);

                // MA - Get the property from the mixin.
                var mixinProperty = mixinType.GetRuntimeProperty(shadowPropertyName);

                // MA - EF.Property<Type>([e], <shadowPropertyName>)
                var value = EntityQueryModelVisitor.CreatePropertyExpression(expression.Object, property);

                bindings[i] = Expression.Bind(mixinProperty, value);
            }

            // MA - new Mixin { Property = EF.Property<Type>([e], <shadowPropertyName>) }
            return Expression.MemberInit(ctor, bindings);
        }
    }
}
