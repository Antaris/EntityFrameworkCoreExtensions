// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Tests.Materialization
{
    using System;
    using System.Linq.Expressions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using Microsoft.EntityFrameworkCore.Storage;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using Xunit;

    using EntityFrameworkCoreExtensions.Materialization;

    public class ExtendedEntityMaterializerSourceTests
    {
        [Fact]
        public void Constructor_ValidatesParameters()
        {
            // Arrange
            var memberMapper = new MemberMapper(new FieldMatcher());

            // Act

            // Assert
            Assert.Throws<ArgumentNullException>(() => new ExtendedEntityMaterializerSource(null /* memberMapper */, null /* hooks */));
            Assert.Throws<ArgumentNullException>(() => new ExtendedEntityMaterializerSource(memberMapper, null /* hooks */));
        }

        [Fact]
        public void CreateReadValueCallExpression_CallsHook()
        {
            // Arrange
            var memberMapper = new MemberMapper(new FieldMatcher());
            var testExpression = Expression.Constant(1);
            var hook = new TestEntityMaterializeSourceHook()
            {
                TestExpression = testExpression
            };
            var hooks = new IEntityMaterializerSourceHook[] { hook };
            var source = new ExtendedEntityMaterializerSource(memberMapper, hooks);

            // Act
            var expression = source.CreateReadValueCallExpression(_readerParameter, 0);

            // Assert
            Assert.Equal(true, hook.CreateReadValueCallExpressionCalled);
            Assert.Equal(testExpression, expression);
        }

        [Fact]
        public void CreateReadValueExpression_CallsHook()
        {
            // Arrange
            var memberMapper = new MemberMapper(new FieldMatcher());
            var testExpression = Expression.Constant(1);
            var hook = new TestEntityMaterializeSourceHook()
            {
                TestExpression = testExpression
            };
            var hooks = new IEntityMaterializerSourceHook[] { hook };
            var source = new ExtendedEntityMaterializerSource(memberMapper, hooks);

            // Act
            var expression = source.CreateReadValueExpression(_readerParameter, typeof(string), 0);

            // Assert
            Assert.Equal(true, hook.CreateReadValueExpressionCalled);
            Assert.Equal(testExpression, expression);
        }

        [Fact]
        public void CreateMaterializeExpression_CallsHook()
        {
            // Arrange
            var memberMapper = new MemberMapper(new FieldMatcher());
            var entityType = new Model().AddEntityType(typeof(Product));
            var testExpression = Expression.Constant(1);
            var hook = new TestEntityMaterializeSourceHook()
            {
                TestExpression = testExpression
            };
            var hooks = new IEntityMaterializerSourceHook[] { hook };
            var source = new ExtendedEntityMaterializerSource(memberMapper, hooks);

            // Act
            var expression = source.CreateMaterializeExpression(entityType, _readerParameter);

            // Assert
            Assert.Equal(true, hook.CreateMaterializeExpressionCalled);
            Assert.Equal(testExpression, expression);
        }

        private static readonly ParameterExpression _readerParameter = Expression.Parameter(typeof(ValueBuffer), "valueBuffer");

        private Func<ValueBuffer, object> GetMaterializer(IEntityMaterializerSource source, IEntityType entityType)
            => Expression.Lambda<Func<ValueBuffer, object>>(
                source.CreateMaterializeExpression(entityType, _readerParameter),
                _readerParameter).Compile();

        private class Product
        {
        }

        private class TestEntityMaterializeSourceHook : EntityMaterializerSourceHookBase
        {
            public Expression TestExpression { get; set; }

            public bool CreateMaterializeExpressionCalled { get; private set; }

            public override Expression CreateMaterializeExpression(IEntityType entityType, Expression valueBuffer, int[] indexMap, Func<IEntityType, Expression, int[], Expression> defaultFactory)
            {
                CreateMaterializeExpressionCalled = true;

                return TestExpression;
            }

            public bool CreateReadValueCallExpressionCalled { get; private set; }
            
            public override Expression CreateReadValueCallExpression(Expression valueBuffer, int index, Func<Expression, int, Expression> defaultFactory)
            {
                CreateReadValueCallExpressionCalled = true;

                return TestExpression;
            }

            public bool CreateReadValueExpressionCalled { get; private set; }

            public override Expression CreateReadValueExpression(Expression valueBuffer, Type type, int index, Func<Expression, Type, int, Expression> defaultFactory)
            {
                CreateReadValueExpressionCalled = true;

                return TestExpression;
            }
        }
    }
}
