// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Tests.Mixins
{
    using System;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    using EntityFrameworkCoreExtensions.Builder;
    using EntityFrameworkCoreExtensions.Mixins;

    /// <summary>
    /// Provides tests for the <see cref="MixinChangeDetectorHook"/> type.
    /// </summary>
    public class MixinChangeDetectorHookTests
    {
        [Fact]
        public void NullMixinValue_ClearsEntityMixinProperties()
        {
            // Arrange
            
            // Act

            // Assert
        }

        private class CatalogDbContext : ExtendedDbContext
        {
            public CatalogDbContext(DbContextOptions options) : base(options)
            {
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                var entityBuilder = modelBuilder.Entity<Product>();
                entityBuilder.HasKey(p => p.Id);

                var mixinBuilder = entityBuilder.Mixin<Option>();
                mixinBuilder.Property(p => p.Name).HasMaxLength(200);
                mixinBuilder.Property(p => p.Cost).HasDefaultValue(100);
            }
        }

        private class Product : MixinHostBase
        {
            public int Id { get; set; }
        }

        public class Option
        {
            public string Name { get; set; }

            public int Cost { get; set; }
        }
    }
}
