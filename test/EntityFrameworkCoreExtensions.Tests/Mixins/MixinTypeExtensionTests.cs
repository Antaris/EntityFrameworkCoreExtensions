// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Tests.Mixins
{
    using System;
    using Xunit;

    using EntityFrameworkCoreExtensions.Mixins;

    /// <summary>
    /// Provides tests for the <see cref="MixinTypeExtensions"/> type.
    /// </summary>
    public class MixinTypeExtensionTests
    {
        [Fact]
        public void GetMixinPrefix_ValidatesParameters()
        {
            // Arrange

            // Act

            // Assert
            Assert.Throws<ArgumentNullException>(() => MixinTypeExtensions.GetMixinPrefix(null /* type */));
        }

        [Fact]
        public void GetMixinPrefix_ReturnsMixinPrefix_FromAttribute()
        {
            // Arrange
            var mixinType = typeof(PrefixedMixin);

            // Act
            string prefix = MixinTypeExtensions.GetMixinPrefix(mixinType);

            // Assert
            Assert.Equal("Superman", prefix);
        }


        [Fact]
        public void GetMixinPrefix_ReturnsMixinPrefix_FromTypeName()
        {
            // Arrange
            var mixinType = typeof(Mixin);

            // Act
            string prefix = MixinTypeExtensions.GetMixinPrefix(mixinType);

            // Assert
            Assert.Equal("Mixin", prefix);
        }

        [MixinPrefix("Superman")]
        private class PrefixedMixin
        {

        }

        private class Mixin
        {

        }
    }
}
