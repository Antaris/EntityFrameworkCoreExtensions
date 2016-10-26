// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Tests.ChangeTracking
{
    using System;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    using EntityFrameworkCoreExtensions.ChangeTracking;

    /// <summary>
    /// Provides tests for the <see cref="ExtendedChangeDetector"/> type.
    /// </summary>
    public class ExtendedChangeDetectorTests
    {
        [Fact]
        public void Constructor_ValidatesParameters()
        {
            // Arrange

            // Act

            // Assert
            Assert.Throws<ArgumentNullException>(() => new ExtendedChangeDetector(null /* hooks */));
        }

        [Fact]
        public void DetectionHooks_AreCalled()
        {
            // Arrange
            var hook = new TestChangeDetectorHook();
            var hooks = new IChangeDetectorHook[] { hook };
            var changeDetector = new ExtendedChangeDetector(hooks);
            var services = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .AddSingleton<IChangeDetector>(sp => changeDetector);
            var serviceProvider = services.BuildServiceProvider();

            using (var context = new CatalogDBContext(serviceProvider))
            {
                // MA - We have to add a entity, otherwise no change tracking entries will be tested.
                context.Add(new Product());

                // Act
                context.ChangeTracker.DetectChanges();

                // Assert
                Assert.Equal(true, hook.DetectingChangesCalled);
                Assert.Equal(true, hook.DetectingEntryChangesCalled);
                Assert.Equal(true, hook.DetectedEntryChangesCalled);
                Assert.Equal(true, hook.DetectedChangesCalled);
            }
        }

        private class TestChangeDetectorHook : ChangeDetectorHook
        {
            public bool DetectedChangesCalled { get; private set; }

            public override void DetectedChanges(IChangeDetector changeDetector, IStateManager stateManager)
                => DetectedChangesCalled = true;

            public bool DetectingChangesCalled { get; private set; }

            public override void DetectingChanges(IChangeDetector changeDetector, IStateManager stateManager)
                => DetectingChangesCalled = true;

            public bool DetectedEntryChangesCalled { get; private set; }

            public override void DetectedEntryChanges(IChangeDetector changeDetector, IStateManager stateManager, InternalEntityEntry entry)
                => DetectedEntryChangesCalled = true;

            public bool DetectingEntryChangesCalled { get; private set; }

            public override void DetectingEntryChanges(IChangeDetector changeDetector, IStateManager stateManager, InternalEntityEntry entry)
                => DetectingEntryChangesCalled = true;
        }

        private class Product
        {
            public int Id { get; set; }
        }

        private class CatalogDBContext : DbContext
        {
            private IServiceProvider _serviceProvider;

            public CatalogDBContext(IServiceProvider serviceProvider = null)
            {
                _serviceProvider = serviceProvider;
            }

            public DbSet<Product> Products { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<Product>().HasKey(p => p.Id);
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder.UseInMemoryDatabase();

                if (_serviceProvider != null)
                {
                    optionsBuilder.UseInternalServiceProvider(_serviceProvider);
                }
            }
        }
    }
}
