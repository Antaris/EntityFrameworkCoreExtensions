// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Tests
{
    using System;
    using System.Collections.Generic;
    using Xunit;

    /// <summary>
    /// Provides tests for the <see cref="ActionExecutor{T}"/> type.
    /// </summary>
    public class ActionExecutorTests
    {
        [Fact]
        public void Constructor_ValidatesParameters()
        {
            // Arrange

            // Act

            // Assert
            Assert.Throws<ArgumentNullException>(() => new ActionExecutor<string>(null /* items */));
        }

        [Fact]
        public void Execute_ValidatesParameters()
        {
            // Arrange
            var items = new[] { "Hello", "World" };
            var executor = new ActionExecutor<string>(items);

            // Act

            // Assert
            Assert.Throws<ArgumentNullException>(() => executor.Execute(null /* action */));
        }

        [Fact]
        public void Execute_FlowsCorrectOrder()
        {
            // Arrange
            var items = new[] { "Hello", "World" };
            var executor = new ActionExecutor<string>(items);
            var results = new List<string>();

            // Act
            executor.Execute(s => results.Add(s));

            // Assert
            Assert.Equal(items, results);
        }

        [Fact]
        public void Accepts_EmptyArray()
        {
            // Arrange
            var items = new string[0];
            var executor = new ActionExecutor<string>(items);
            bool called = false;

            // Act
            executor.Execute(s => called = true);

            // Assert
            Assert.Equal(false, called);
        }
    }
}
