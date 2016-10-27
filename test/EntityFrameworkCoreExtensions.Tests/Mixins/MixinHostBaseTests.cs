// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Tests.Mixins
{
    using System;
    using Xunit;

    using EntityFrameworkCoreExtensions.Mixins;

    /// <summary>
    /// Provides tests for the <see cref="MixinHostBase"/> type.
    /// </summary>
    public class MixinHostBaseTests
    {
        [Fact]
        public void SetMixin_SetsMixin_WhenNotNull()
        {
            // Arrange
            var product = new Product();
            var option = new Option();

            // Act
            product.SetMixin(option);

            // Assert
            var option2 = product.Mixin<Option>();
            Assert.NotNull(option2);
            Assert.Equal(option, option2);
        }

        [Fact]
        public void SetMixin_ClearsMixin_WhenNull()
        {
            // Arrange
            var product = new Product();
            var option = new Option();

            // Act
            product.SetMixin(option);
            product.SetMixin<Option>(null);

            // Assert
            var option2 = product.Mixin<Option>();
            Assert.Equal(null, option2);
        }

        [Fact]
        public void Mixin_ReturnsMixin_WhenSet()
        {
            // Arrange
            var product = new Product();
            var option = new Option();

            // Act
            product.SetMixin(option);
            var option2 = product.Mixin<Option>();

            // Assert
            Assert.NotNull(option2);
            Assert.Equal(option, option2);
        }

        [Fact]
        public void Mixin_ReturnsDefault_WhenNotSet()
        {
            // Arrange
            var product = new Product();

            // Act
            var option = product.Mixin<Option>();

            // Assert
            Assert.Equal(null, option);
        }

        private class Option
        {

        }

        private class Product : MixinHostBase
        {

        }
    }
}
