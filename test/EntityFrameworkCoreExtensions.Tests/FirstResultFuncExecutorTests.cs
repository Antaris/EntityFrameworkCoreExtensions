// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Tests
{
    using System;
    using System.Collections.Generic;
    using Xunit;

    /// <summary>
    /// Provides tests for the <see cref="ActionExecutor{T}"/> type.
    /// </summary>
    public class FirstResultFuncExecutorTests
    {
        [Fact]
        public void Constructor_ValidatesParameters()
        {
            // Arrange

            // Act

            // Assert
            Assert.Throws<ArgumentNullException>(() => new FirstResultFuncExecutor<string>(null /* items */));
        }

        [Fact]
        public void Execute_ValidatesParameters()
        {
            // Arrange
            var products = new[]
            {
                new Product(), new Product()
            };
            var executor = new FirstResultFuncExecutor<Product>(products);

            // Act

            // Assert
            Assert.Throws<ArgumentNullException>(() => executor.Execute(null /* action */));
        }

        [Fact]
        public void Execute_ReturnsFirstItem()
        {
            // Arrange
            var products = new[]
            {
                new Product() { Name = "Product A" },
                new Product() { Name = "Product B" }
            };
            var executor = new FirstResultFuncExecutor<Product>(products);

            // Act
            string name = executor.Execute(p => p.Name);

            // Assert
            Assert.Equal("Product A", name);
        }

        [Fact]
        public void Accepts_EmptyArray()
        {
            // Arrange
            var products = new Product[0];
            var executor = new FirstResultFuncExecutor<Product>(products);

            // Act
            string name = executor.Execute(p => p.Name);

            // Assert
            Assert.Equal(null, name);
        }

        private class Product
        {
            public string Name { get; set; }
        }
    }
}
