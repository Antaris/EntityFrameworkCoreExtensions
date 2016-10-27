// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Tests.Builder
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Conventions;
    using Xunit;

    using EntityFrameworkCoreExtensions.Builder;

    /// <summary>
    /// Provides tests for the <see cref="MixinTypeBuilder{TMixin}"/> type.
    /// </summary>
    public class MixinTypeBuilderTests
    {
        [Fact]
        public void Constructor_ValidatesParameters()
        {
            // Arrange

            // Act

            // Assert
            Assert.Throws<ArgumentNullException>(() => new MixinTypeBuilder<Author>(null /* entityTypeBuilder */));
        }

        [Fact]
        public void Property_DefinesShadowProperty_OnParentBuilder()
        {
            // Arrange
            var modelBuilder = new ModelBuilder(new ConventionSet());
            var entityBuilder = modelBuilder.Entity<User>();
            var mixinBuilder = new MixinTypeBuilder<Author>(entityBuilder);

            // Act
            mixinBuilder.Property(a => a.GooglePlusProfile).HasMaxLength(200);

            // Assert
            var property = entityBuilder.Metadata.FindProperty("Author_GooglePlusProfile");
            Assert.NotNull(property);
            Assert.Equal(typeof(string), property.ClrType);
            Assert.Equal(200, property.GetMaxLength());
        }

        private class Author
        {
            public string GooglePlusProfile { get; set; }
        }

        public class User
        {

        }
    }
}
