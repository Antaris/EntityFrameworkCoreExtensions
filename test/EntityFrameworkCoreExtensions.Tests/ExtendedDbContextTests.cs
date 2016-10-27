// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    /// <summary>
    /// Provides tests for the <see cref="ExtendedDbContext"/> type.
    /// </summary>
    public class ExtendedDbContextTests
    {
        [Fact]
        public void Constructor_ValidatesParameters()
        {
            // Arrange
            var hooks = new IDbContextHook[0];

            // Act

            // Assert
            Assert.Throws<ArgumentNullException>(() => new ExtendedDbContext(null /* hooks */, null /* options */));
            Assert.Throws<ArgumentNullException>(() => new ExtendedDbContext(hooks, null /* options */));
        }

        [Fact]
        public void Supports_UntypedHooks()
        {
            // Arrange
            var untypedHook = new UntypedDbContextHook();
            var hooks = new IDbContextHook[]
            {
                untypedHook
            };
            var services = new ServiceCollection().AddEntityFrameworkInMemoryDatabase();
            var serviceProvider = services.BuildServiceProvider();
            var options = new DbContextOptionsBuilder<ExtendedDbContext>()
                .UseInMemoryDatabase()
                .UseInternalServiceProvider(serviceProvider)
                .Options;
            var context = new TestExtendedDbContext(hooks, options);

            // Act
            // MA - Services are lazily initiated so we need to perform an operation to set them.
            context.SaveChanges();


            // Assert
            Assert.Equal(true, untypedHook.OnConfiguringCalled);
        }

        [Fact]
        public void DbContextType_AppliedToHooks()
        {
            // Arrange
            var untypedHook = new UntypedDbContextHook();
            var hooks = new IDbContextHook[]
            {
                untypedHook
            };
            var services = new ServiceCollection().AddEntityFrameworkInMemoryDatabase();
            var serviceProvider = services.BuildServiceProvider();
            var options = new DbContextOptionsBuilder<ExtendedDbContext>()
                .UseInMemoryDatabase()
                .UseInternalServiceProvider(serviceProvider)
                .Options;

            // Act
            var context = new TestExtendedDbContext(hooks, options);
            
            // Assert
            Assert.Equal(typeof(TestExtendedDbContext), untypedHook.DbContextType);
        }

        [Fact]
        public void Supports_TypedHooks()
        {
            // Arrange
            var typedHook = new TypedDbContextHook();
            var otherTypedHook = new OtherTypedDbContextHook();
            var hooks = new IDbContextHook[]
            {
                typedHook, otherTypedHook
            };
            var services = new ServiceCollection().AddEntityFrameworkInMemoryDatabase();
            var serviceProvider = services.BuildServiceProvider();
            var options = new DbContextOptionsBuilder<ExtendedDbContext>()
                .UseInMemoryDatabase()
                .UseInternalServiceProvider(serviceProvider)
                .Options;
            var context = new TestExtendedDbContext(hooks, options);

            // Act
            // MA - Services are lazily initiated so we need to perform an operation to set them.
            context.SaveChanges();


            // Assert
            Assert.Equal(true, typedHook.OnConfiguringCalled);
            // MA - THis should be false, because otherTypedHook is a typed hook for a different DbContext type.
            Assert.Equal(false, otherTypedHook.OnConfiguringCalled);
        }

        [Fact]
        public void AddEntity_CallsHooks()
        {
            // Arrange
            var hook = new UntypedDbContextHook();
            var hooks = new IDbContextHook[]
            {
                hook
            };
            var services = new ServiceCollection().AddEntityFrameworkInMemoryDatabase();
            var serviceProvider = services.BuildServiceProvider();
            var options = new DbContextOptionsBuilder<ExtendedDbContext>()
                .UseInMemoryDatabase()
                .UseInternalServiceProvider(serviceProvider)
                .Options;
            var context = new TestExtendedDbContext(hooks, options);

            // Act
            var product = new Product { };
            context.Add(product);

            // Assert
            Assert.Equal(true, hook.AddingEntryCalled);
            Assert.Equal(true, hook.AddedEntryCalled);
        }

        [Fact]
        public void AttachEntity_CallsHooks()
        {
            // Arrange
            var hook = new UntypedDbContextHook();
            var hooks = new IDbContextHook[]
            {
                hook
            };
            var services = new ServiceCollection().AddEntityFrameworkInMemoryDatabase();
            var serviceProvider = services.BuildServiceProvider();
            var options = new DbContextOptionsBuilder<ExtendedDbContext>()
                .UseInMemoryDatabase()
                .UseInternalServiceProvider(serviceProvider)
                .Options;
            var context = new TestExtendedDbContext(hooks, options);

            // Act
            var product = new Product { Id = 1 };
            context.Attach(product);

            // Assert
            Assert.Equal(true, hook.AttachingEntryCalled);
            Assert.Equal(true, hook.AttachedEntryCalled);
        }
        
        [Fact]
        public void OnConfiguring_CallsHooks()
        {
            // Arrange
            var hook = new UntypedDbContextHook();
            var hooks = new IDbContextHook[]
            {
                hook
            };
            var services = new ServiceCollection().AddEntityFrameworkInMemoryDatabase();
            var serviceProvider = services.BuildServiceProvider();
            var options = new DbContextOptionsBuilder<ExtendedDbContext>()
                .UseInMemoryDatabase()
                .UseInternalServiceProvider(serviceProvider)
                .Options;
            var context = new TestExtendedDbContext(hooks, options);

            // Act
            var product = new Product { };
            // MA - Services are lazily initiated so we need to perform an operation to set them.
            context.Add(product);

            // Assert
            Assert.Equal(true, hook.OnConfiguringCalled);
        }

        [Fact]
        public void OnModelCreating_CallsHooks()
        {
            // Arrange
            var hook = new UntypedDbContextHook();
            var hooks = new IDbContextHook[]
            {
                hook
            };
            var services = new ServiceCollection().AddEntityFrameworkInMemoryDatabase();
            var serviceProvider = services.BuildServiceProvider();
            var options = new DbContextOptionsBuilder<ExtendedDbContext>()
                .UseInMemoryDatabase()
                .UseInternalServiceProvider(serviceProvider)
                .Options;
            var context = new TestExtendedDbContext(hooks, options);

            // Act
            var product = new Product { };
            // MA - Services are lazily initiated so we need to perform an operation to set them.
            context.Add(product);

            // Assert
            Assert.Equal(true, hook.OnModelCreatingCalled);
        }

        [Fact]
        public void RemoveEntity_CallsHooks()
        {
            // Arrange
            var hook = new UntypedDbContextHook();
            var hooks = new IDbContextHook[]
            {
                hook
            };
            var services = new ServiceCollection().AddEntityFrameworkInMemoryDatabase();
            var serviceProvider = services.BuildServiceProvider();
            var options = new DbContextOptionsBuilder<ExtendedDbContext>()
                .UseInMemoryDatabase()
                .UseInternalServiceProvider(serviceProvider)
                .Options;
            var context = new TestExtendedDbContext(hooks, options);

            // Act
            var product = new Product { Id = 1 };
            context.Remove(product);

            // Assert
            Assert.Equal(true, hook.RemovingEntryCalled);
            Assert.Equal(true, hook.RemovedEntryCalled);
        }

        [Fact]
        public void SavingChanges_CallsHooks()
        {
            // Arrange
            var hook = new UntypedDbContextHook();
            var hooks = new IDbContextHook[]
            {
                hook
            };
            var services = new ServiceCollection().AddEntityFrameworkInMemoryDatabase();
            var serviceProvider = services.BuildServiceProvider();
            var options = new DbContextOptionsBuilder<ExtendedDbContext>()
                .UseInMemoryDatabase()
                .UseInternalServiceProvider(serviceProvider)
                .Options;
            var context = new TestExtendedDbContext(hooks, options);

            // Act
            var product = new Product { };
            context.Add(product);
            context.SaveChanges();

            // Assert
            Assert.Equal(true, hook.SavingChangesCalled);
            Assert.Equal(true, hook.SavedChangesCalled);
        }

        [Fact]
        public async Task SavingChangesAsync_CallsHooks()
        {
            // Arrange
            var hook = new UntypedDbContextHook();
            var hooks = new IDbContextHook[]
            {
                hook
            };
            var services = new ServiceCollection().AddEntityFrameworkInMemoryDatabase();
            var serviceProvider = services.BuildServiceProvider();
            var options = new DbContextOptionsBuilder<ExtendedDbContext>()
                .UseInMemoryDatabase()
                .UseInternalServiceProvider(serviceProvider)
                .Options;
            var context = new TestExtendedDbContext(hooks, options);

            // Act
            var product = new Product { };
            context.Add(product);
            await context.SaveChangesAsync();

            // Assert
            Assert.Equal(true, hook.SavingChangesAsyncCalled);
            Assert.Equal(true, hook.SavedChangesAsyncCalled);
        }

        [Fact]
        public void UpdateEntity_CallsHooks()
        {
            // Arrange
            var hook = new UntypedDbContextHook();
            var hooks = new IDbContextHook[]
            {
                hook
            };
            var services = new ServiceCollection().AddEntityFrameworkInMemoryDatabase();
            var serviceProvider = services.BuildServiceProvider();
            var options = new DbContextOptionsBuilder<ExtendedDbContext>()
                .UseInMemoryDatabase()
                .UseInternalServiceProvider(serviceProvider)
                .Options;
            var context = new TestExtendedDbContext(hooks, options);

            // Act
            var product = new Product { Id = 1 };
            context.Update(product);

            // Assert
            Assert.Equal(true, hook.UpdatingEntryCalled);
            Assert.Equal(true, hook.UpdatedEntryCalled);
        }

        public class Product
        {
            public int Id { get; set; }
        }

        private class UntypedDbContextHook : DbContextHookBase
        {
            public bool AddingEntryCalled { get; private set; }
            public override void AddingEntry<TEntity>(TEntity entity)
                => AddingEntryCalled = true;

            public bool AddedEntryCalled { get; private set; }
            public override void AddedEntry<TEntity>(TEntity entity, EntityEntry<TEntity> entry)
                => AddedEntryCalled = true;

            public bool AttachingEntryCalled { get; private set; }
            public override void AttachingEntry<TEntity>(TEntity entity)
                => AttachingEntryCalled = true;
            
            public bool AttachedEntryCalled { get; private set; }
            public override void AttachedEntry<TEntity>(TEntity entity, EntityEntry<TEntity> entry)
                => AttachedEntryCalled = true;

            public bool OnConfiguringCalled { get; private set; }
            public override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => OnConfiguringCalled = true;

            public bool OnModelCreatingCalled { get; private set; }
            public override void OnModelCreating(ModelBuilder modelBuilder)
                => OnModelCreatingCalled = true;

            public bool RemovingEntryCalled { get; private set; }
            public override void RemovingEntry<TEntity>(TEntity entity)
                => RemovingEntryCalled = true;

            public bool RemovedEntryCalled { get; private set; }
            public override void RemovedEntry<TEntity>(TEntity entity, EntityEntry<TEntity> entry)
                => RemovedEntryCalled = true;

            public bool SavingChangesCalled { get; private set; }
            public override void SavingChanges(bool acceptAllChangesOnSuccess)
                => SavingChangesCalled = true;

            public bool SavedChangesCalled { get; private set; }
            public override void SavedChanges(int persistedStateEntries, bool acceptedAllChangesOnSuccess)
                => SavedChangesCalled = true;

            public bool SavingChangesAsyncCalled { get; private set; }
            public override Task SavingChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken)
            {
                SavingChangesAsyncCalled = true;

                return Task.FromResult(true);
            }

            public bool SavedChangesAsyncCalled { get; private set; }
            public override Task SavedChangesAsync(int persistedStateEntries, bool acceptedAllChangesOnSuccess, CancellationToken cancellationToken)
            {
                SavedChangesAsyncCalled = true;

                return Task.FromResult(true);
            }

            public bool UpdatingEntryCalled { get; private set; }
            public override void UpdatingEntry<TEntity>(TEntity entity)
                => UpdatingEntryCalled = true;

            public bool UpdatedEntryCalled { get; private set; }
            public override void UpdatedEntry<TEntity>(TEntity entity, EntityEntry<TEntity> entry)
                => UpdatedEntryCalled = true;
        }

        private class TypedDbContextHook : DbContextHookBase<TestExtendedDbContext>
        {
            public bool AddingEntryCalled { get; private set; }
            public override void AddingEntry<TEntity>(TEntity entity)
                => AddingEntryCalled = true;

            public bool AddedEntryCalled { get; private set; }
            public override void AddedEntry<TEntity>(TEntity entity, EntityEntry<TEntity> entry)
                => AddedEntryCalled = true;

            public bool AttachingEntryCalled { get; private set; }
            public override void AttachingEntry<TEntity>(TEntity entity)
                => AttachingEntryCalled = true;

            public bool AttachedEntryCalled { get; private set; }
            public override void AttachedEntry<TEntity>(TEntity entity, EntityEntry<TEntity> entry)
                => AttachedEntryCalled = true;

            public bool OnConfiguringCalled { get; private set; }
            public override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => OnConfiguringCalled = true;

            public bool OnModelCreatingCalled { get; private set; }
            public override void OnModelCreating(ModelBuilder modelBuilder)
                => OnModelCreatingCalled = true;

            public bool RemovingEntryCalled { get; private set; }
            public override void RemovingEntry<TEntity>(TEntity entity)
                => RemovingEntryCalled = true;

            public bool RemovedEntryCalled { get; private set; }
            public override void RemovedEntry<TEntity>(TEntity entity, EntityEntry<TEntity> entry)
                => RemovedEntryCalled = true;

            public bool SavingChangesCalled { get; private set; }
            public override void SavingChanges(bool acceptAllChangesOnSuccess)
                => SavingChangesCalled = true;

            public bool SavedChangesCalled { get; private set; }
            public override void SavedChanges(int persistedStateEntries, bool acceptedAllChangesOnSuccess)
                => SavedChangesCalled = true;

            public bool SavingChangesAsyncCalled { get; private set; }
            public override Task SavingChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken)
            {
                SavingChangesAsyncCalled = true;

                return Task.FromResult(true);
            }

            public bool SavedChangesAsyncCalled { get; private set; }
            public override Task SavedChangesAsync(int persistedStateEntries, bool acceptedAllChangesOnSuccess, CancellationToken cancellationToken)
            {
                SavedChangesAsyncCalled = true;

                return Task.FromResult(true);
            }

            public bool UpdatingEntryCalled { get; private set; }
            public override void UpdatingEntry<TEntity>(TEntity entity)
                => UpdatingEntryCalled = true;

            public bool UpdatedEntryCalled { get; private set; }
            public override void UpdatedEntry<TEntity>(TEntity entity, EntityEntry<TEntity> entry)
                => UpdatedEntryCalled = true;
        }

        private class OtherTypedDbContextHook : DbContextHookBase<OtherTestExtendedDbContext>
        {
            public bool AddingEntryCalled { get; private set; }
            public override void AddingEntry<TEntity>(TEntity entity)
                => AddingEntryCalled = true;

            public bool AddedEntryCalled { get; private set; }
            public override void AddedEntry<TEntity>(TEntity entity, EntityEntry<TEntity> entry)
                => AddedEntryCalled = true;

            public bool AttachingEntryCalled { get; private set; }
            public override void AttachingEntry<TEntity>(TEntity entity)
                => AttachingEntryCalled = true;

            public bool AttachedEntryCalled { get; private set; }
            public override void AttachedEntry<TEntity>(TEntity entity, EntityEntry<TEntity> entry)
                => AttachedEntryCalled = true;

            public bool OnConfiguringCalled { get; private set; }
            public override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
                => OnConfiguringCalled = true;

            public bool OnModelCreatingCalled { get; private set; }
            public override void OnModelCreating(ModelBuilder modelBuilder)
                => OnModelCreatingCalled = true;

            public bool RemovingEntryCalled { get; private set; }
            public override void RemovingEntry<TEntity>(TEntity entity)
                => RemovingEntryCalled = true;

            public bool RemovedEntryCalled { get; private set; }
            public override void RemovedEntry<TEntity>(TEntity entity, EntityEntry<TEntity> entry)
                => RemovedEntryCalled = true;

            public bool SavingChangesCalled { get; private set; }
            public override void SavingChanges(bool acceptAllChangesOnSuccess)
                => SavingChangesCalled = true;

            public bool SavedChangesCalled { get; private set; }
            public override void SavedChanges(int persistedStateEntries, bool acceptedAllChangesOnSuccess)
                => SavedChangesCalled = true;

            public bool SavingChangesAsyncCalled { get; private set; }
            public override Task SavingChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken)
            {
                SavingChangesAsyncCalled = true;

                return Task.FromResult(true);
            }

            public bool SavedChangesAsyncCalled { get; private set; }
            public override Task SavedChangesAsync(int persistedStateEntries, bool acceptedAllChangesOnSuccess, CancellationToken cancellationToken)
            {
                SavedChangesAsyncCalled = true;

                return Task.FromResult(true);
            }

            public bool UpdatingEntryCalled { get; private set; }
            public override void UpdatingEntry<TEntity>(TEntity entity)
                => UpdatingEntryCalled = true;

            public bool UpdatedEntryCalled { get; private set; }
            public override void UpdatedEntry<TEntity>(TEntity entity, EntityEntry<TEntity> entry)
                => UpdatedEntryCalled = true;
        }

        private class TestExtendedDbContext : ExtendedDbContext
        {
            public TestExtendedDbContext(IEnumerable<IDbContextHook> hooks, DbContextOptions options) : base(hooks, options)
            {
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                modelBuilder.Entity<Product>().HasKey(p => p.Id);
            }
        }

        private class OtherTestExtendedDbContext : ExtendedDbContext
        {
            public OtherTestExtendedDbContext(IEnumerable<IDbContextHook> hooks, DbContextOptions options) : base(hooks, options)
            {
            }
        }
    }
}
