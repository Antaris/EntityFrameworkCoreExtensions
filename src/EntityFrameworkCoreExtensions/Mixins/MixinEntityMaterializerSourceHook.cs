// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Mixins
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using Microsoft.EntityFrameworkCore.Storage;

    using EntityFrameworkCoreExtensions.Materialization;

    /// <summary>
    /// Hooks into the entity materialialization to enable mixins to be hydrated 
    /// at the same time as the parent entity.
    /// </summary>
    public class MixinEntityMaterializerSourceHook : EntityMaterializerSourceHookBase
    {
        private static readonly Type _mixinHostType = typeof(IMixinHost);
        private static readonly Type _valueBufferType = typeof(ValueBuffer);
        private static readonly MethodInfo _setMixinMethod = _mixinHostType.GetRuntimeMethod(nameof(IMixinHost.SetMixin), new[] { typeof(Type), typeof(object) });
        private static readonly MethodInfo _readValueMethod = _valueBufferType.GetTypeInfo().DeclaredProperties.Single(p => p.GetIndexParameters().Any()).GetMethod;

        /// <inheritdoc />
        public override Expression CreateMaterializeExpression(IEntityType entityType, Expression valueBuffer, int[] indexMap, Func<IEntityType, Expression, int[], Expression> defaultFactory)
        {
            if (!entityType.ClrType.GetTypeInfo().ImplementedInterfaces.Contains(_mixinHostType))
            {
                return null;
            }

            // MA - Get the mixin types.
            var mixinTypes = GetMixinTypes(entityType).ToList();

            if (mixinTypes.Count == 0)
            {
                return null;
            }

            // MA - Get the entity expression
            var entity = defaultFactory(entityType, valueBuffer, indexMap);

            // MA - Create input parameters
            var instance = Expression.Variable(_mixinHostType);

            // MA - Collect the expressions that will form a new block.
            var blockExpressions = new List<Expression>();

            // MA - Assign the instance variable.
            blockExpressions.Add(
                Expression.Assign(instance,
                    Expression.Convert(entity, _mixinHostType)));

            foreach (var mixinType in mixinTypes)
            {
                // MA - Create the mixin materialiser.
                blockExpressions.Add(CreateMixinMaterializeExpression(instance, valueBuffer, mixinType, entityType));
            }

            // MA - Add the instance variable as the implicit return.
            blockExpressions.Add(
                Expression.Convert(instance, typeof(object)));

            return Expression.Block(typeof(object), new[] { instance }, blockExpressions);
        }

        private Expression CreateMixinMaterializeExpression(ParameterExpression instance, Expression valueBuffer, Type mixinType, IEntityType entityType)
        {
            string prefix = $"{mixinType.GetMixinPrefix()}_";

            // MA - Create a new mixin variable
            var mixin = Expression.Variable(mixinType);

            // MA - Collect the expressions that form a block.
            var blockExpressions = new List<Expression>();

            // MA - Get the properties
            var properties = entityType.GetProperties()
                .Where(p => p.IsShadowProperty && p.Name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
                .ToArray();

            // MA - Collect the bindings.
            var bindings = new MemberBinding[properties.Length];
            
            for (int i = 0; i < properties.Length; i++)
            {
                var property = properties[i];
                string propertyName = property.Name.Substring(prefix.Length + 1);
                var member = mixinType.GetRuntimeProperty(propertyName);
                int index = property.GetIndex();

                // MA - Create a binding between the mixin property and the value buffer.
                var value = Expression.Convert(
                    Expression.Call(valueBuffer, _readValueMethod, Expression.Constant(index)),
                    property.ClrType);

                bindings[i] = Expression.Bind(member, value);
            }

            // MA - Add a new object initializer which initializes the mixin and sets the property values.
            blockExpressions.Add(
                Expression.Assign(
                    mixin,
                    Expression.MemberInit(
                        Expression.New(mixinType),
                        bindings)));

            // MA - Call the SetMixin method on the mixin host.
            blockExpressions.Add(
                Expression.Call(
                    instance,
                    _setMixinMethod,
                    Expression.Constant(mixinType),
                    mixin));

            // MA - Return a new block.
            return Expression.Block(
                new[] { mixin },
                blockExpressions);
        }

        private IEnumerable<Type> GetMixinTypes(IEntityType entityType)
        {
            foreach (var annotation in entityType.GetAnnotations().Where(a => a.Name.StartsWith("mixin-", StringComparison.Ordinal)))
            {
                yield return (Type)annotation.Value;
            }
        }
    }
}
