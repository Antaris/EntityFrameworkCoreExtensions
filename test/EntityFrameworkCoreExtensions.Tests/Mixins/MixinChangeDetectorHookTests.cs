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
            public CatalogDbContext(IEnumerable<IDbContextHook> hooks, DbContextOptions options) : base(hooks, options)
            {
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                var entityBuilder = modelBuilder.Entity<Product>()
                    .HasKey(p => p.Id);

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
