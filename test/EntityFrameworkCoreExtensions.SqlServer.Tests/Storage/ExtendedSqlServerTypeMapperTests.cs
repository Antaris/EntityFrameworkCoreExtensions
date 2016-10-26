// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Tests.Storage
{
    using System;
    using System.Data;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using Microsoft.EntityFrameworkCore.Storage;
    using Xunit;

    using EntityFrameworkCoreExtensions.Storage;

    /// <summary>
    /// Provides tests for the <see cref="ExtendedSqlServerTypeMapper"/> type.
    /// </summary>
    public class ExtendedSqlServerTypeMapperTests
    {
        [Fact]
        public void Constructor_ValidatesParameters()
        {
            // Arrange

            // Act

            // Assert
            Assert.Throws<ArgumentNullException>(() => new ExtendedSqlServerTypeMapper(null /* hooks */));
        }

        [Fact]
        public void FindMapping_WithProperty_ReturnsCustomMappingFromHook()
        {
            // Arrange
            var model = new Model();
            var property = model.AddEntityType("Test").AddProperty("Value", typeof(CustomType));
            var hook = new TestRelationalTypeMapperHook();
            var hooks = new IRelationalTypeMapperHook[] { hook };
            var typeMapper = new ExtendedSqlServerTypeMapper(hooks);

            // Act
            var typeMapping = typeMapper.FindMapping(property);

            // Assert
            Assert.NotNull(typeMapping);
            Assert.Equal(typeMapping.ClrType, typeof(CustomType));
        }

        [Fact]
        public void FindMapping_WithProperty_ReturnsStandardMappingFromBase()
        {
            // Arrange
            var model = new Model();
            var property = model.AddEntityType("Test").AddProperty("Value", typeof(byte));
            var hook = new TestRelationalTypeMapperHook();
            var hooks = new IRelationalTypeMapperHook[] { hook };
            var typeMapper = new ExtendedSqlServerTypeMapper(hooks);

            // Act
            var typeMapping = typeMapper.FindMapping(property);

            // Assert
            Assert.NotNull(typeMapping);
            Assert.Equal(typeMapping.ClrType, typeof(byte));
        }

        private struct CustomType
        {

        }

        private class TestRelationalTypeMapperHook : RelationalTypeMapperHook
        {
            private readonly RelationalTypeMapping _customType
                = new RelationalTypeMapping("varchar(100)", typeof(CustomType), dbType: DbType.String, unicode: false, size: 100);

            public override RelationalTypeMapping FindMapping(IProperty property)
            {
                return (_customType.ClrType.Equals(property.ClrType))
                    ? _customType
                    : null;
            }
        }
    }
}
