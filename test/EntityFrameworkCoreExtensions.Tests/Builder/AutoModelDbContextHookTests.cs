// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Tests.Builder
{
    using System;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Conventions;
    using Xunit;

    using EntityFrameworkCoreExtensions.Builder;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Provides tests for the <see cref="AutoModelDbContextHook"/> type.
    /// </summary>
    public class AutoModelDbContextHookTests
    {
        [Fact]
        public void Constructor_ValidatesParameters()
        {
            // Arrange

            // Act

            // Assert
            Assert.Throws<ArgumentNullException>(() => new AutoModelDbContextHook(null /* modelBuilderService */));
        }

        [Fact]
        public void OnModelCreating_DoesNothing_IfNoDbContextType()
        {
            // Arrange
            var modelBuilderService = new ModelBuilderService(new IModelBuilder[0]);
            var hook = new AutoModelDbContextHook(modelBuilderService);
            var modelBuilder = new ModelBuilder(new ConventionSet());

            // Act
            // MA - the DbContextType property is set by the ExtendedDbContext constructor, so its currently null.
            hook.OnModelCreating(modelBuilder);

            // Assert
            var entityTypes = modelBuilder.Model.GetEntityTypes().ToArray();
            Assert.Equal(0, entityTypes.Length);
        }

        [Fact]
        public void OnModelCreating_DoesNothing_IfNoAutoBuildAttribute()
        {
            // Arrange
            var modelBuilderService = new ModelBuilderService(new IModelBuilder[0]);
            var hook = new AutoModelDbContextHook(modelBuilderService);
            var modelBuilder = new ModelBuilder(new ConventionSet());

            // Act
            ((IDbContextHook)hook).SetDbContextType(typeof(NoAutoBuildCatalogDbContext));
            hook.OnModelCreating(modelBuilder);

            // Assert
            var entityTypes = modelBuilder.Model.GetEntityTypes().ToArray();
            Assert.Equal(0, entityTypes.Length);
        }

        [Fact]
        public void OnModelCreating_BuildsModel()
        {
            // Arrange
            var productEntityTypeBuilder = new ProductEntityTypeBuilder();
            var productVariantEntityTypeBuilder = new ProductVariantEntityTypeBuilder();
            var inputBuilders = new IModelBuilder[]
            {
                productEntityTypeBuilder,
                productVariantEntityTypeBuilder
            };
            var modelBuilderService = new ModelBuilderService(inputBuilders);
            var hook = new AutoModelDbContextHook(modelBuilderService);
            var modelBuilder = new ModelBuilder(new ConventionSet());

            // Act
            ((IDbContextHook)hook).SetDbContextType(typeof(AutoBuildCatalogDbContext));
            hook.OnModelCreating(modelBuilder);

            // Assert
            var entityTypes = modelBuilder.Model.GetEntityTypes().ToArray();
            Assert.Equal(2, entityTypes.Length);
            Assert.Equal(true, productEntityTypeBuilder.BuildEntityCalled);
            Assert.Contains(entityTypes, et => et.ClrType == typeof(Product));
            Assert.Equal(true, productVariantEntityTypeBuilder.BuildEntityCalled);
            Assert.Contains(entityTypes, et => et.ClrType == typeof(ProductVariant));
        }

        [Fact]
        public void OnModelCreating_BuildsModel_CanIgnoreNavigations()
        {
            // Arrange
            var productEntityTypeBuilder = new ProductEntityTypeBuilder();
            var productVariantEntityTypeBuilder = new ProductVariantEntityTypeBuilder();
            var inputBuilders = new IModelBuilder[]
            {
                productEntityTypeBuilder,
                productVariantEntityTypeBuilder
            };
            var modelBuilderService = new ModelBuilderService(inputBuilders);
            var hook = new AutoModelDbContextHook(modelBuilderService);
            var modelBuilder = new ModelBuilder(new ConventionSet());

            // Act
            ((IDbContextHook)hook).SetDbContextType(typeof(NoNavigationsAutoBuildCatalogDbContext));
            hook.OnModelCreating(modelBuilder);

            // Assert
            var entityTypes = modelBuilder.Model.GetEntityTypes().ToArray();
            Assert.Equal(1, entityTypes.Length);
            Assert.Equal(true, productEntityTypeBuilder.BuildEntityCalled);
            Assert.Contains(entityTypes, et => et.ClrType == typeof(Product));
        }

        [Fact]
        public void OnModelCreating_BuildsModel_UsingTypedContextModelBuilder()
        {
            // Arrange
            var contextModelBuilder = new ContextModelBuilder();
            var inputBuilders = new IModelBuilder[]
            {
                contextModelBuilder
            };
            var modelBuilderService = new ModelBuilderService(inputBuilders);
            var hook = new AutoModelDbContextHook(modelBuilderService);
            var modelBuilder = new ModelBuilder(new ConventionSet());

            // Act
            ((IDbContextHook)hook).SetDbContextType(typeof(AutoBuildCatalogDbContext));
            hook.OnModelCreating(modelBuilder);

            // Assert
            var entityTypes = modelBuilder.Model.GetEntityTypes().ToArray();
            Assert.Equal(1, entityTypes.Length);
            Assert.Equal(true, contextModelBuilder.BuildModelCalled);
            Assert.Contains(entityTypes, et => et.ClrType == typeof(Vendor));
        }

        private class Vendor
        {

        }

        private class ContextModelBuilder : ModelBuilderBase<AutoBuildCatalogDbContext>
        {
            public bool BuildModelCalled { get; private set; }
            public override void BuildModel(ModelBuilder builder)
            {
                BuildModelCalled = true;

                builder.Entity<Vendor>();
            }
        }

        private class Product
        {
            public ProductVariant ProductVariant { get; set; }
        }

        private class ProductEntityTypeBuilder : EntityTypeBuilderBase<Product>
        {
            public bool BuildEntityCalled { get; set; }
            public override void BuildEntity(EntityTypeBuilder<Product> builder)
                => BuildEntityCalled = true;
        }

        private class ProductVariant
        {

        }

        private class ProductVariantEntityTypeBuilder : EntityTypeBuilderBase<ProductVariant>
        {
            public bool BuildEntityCalled { get; set; }
            public override void BuildEntity(EntityTypeBuilder<ProductVariant> builder)
                => BuildEntityCalled = true;
        }

        private class NoAutoBuildCatalogDbContext : DbContext
        {
            public DbSet<Product> Products { get; set; }
        }

        [AutoModel]
        private class AutoBuildCatalogDbContext : DbContext
        {
            public DbSet<Product> Products { get; set; }
        }

        [AutoModel(IncludeNavigations = false)]
        private class NoNavigationsAutoBuildCatalogDbContext : DbContext
        {
            public DbSet<Product> Products { get; set; }
        }
    }
}
