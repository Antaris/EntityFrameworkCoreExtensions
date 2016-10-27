// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Tests.Builder
{
    using System;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Xunit;

    using EntityFrameworkCoreExtensions.Builder;

    /// <summary>
    /// Provides tests for the <see cref="ModelBuilderService"/> type.
    /// </summary>
    public class ModelBuilderServiceTests
    {
        [Fact]
        public void Constructor_ValidatesParameters()
        {
            // Arrange

            // Act

            // Assert
            Assert.Throws<ArgumentNullException>(() => new ModelBuilderService(null /* builders */));
        }

        [Fact]
        public void GetModelBuilders_ValidatesParameters()
        {
            // Arrange
            var dbContextType = typeof(CatalogDbContext);
            var inputBuilders = new IModelBuilder[0];
            var service = new ModelBuilderService(inputBuilders);

            // Act

            // Assert
            Assert.Throws<ArgumentNullException>(() => service.GetModelBuilders(null /* dbContextType */, false));
        }

        [Fact]
        public void GetModelBuilders_SupportsUntypedModelBuilders()
        {
            // Arrange
            var dbContextType = typeof(CatalogDbContext);
            var inputBuilders = new IModelBuilder[]
            {
                new UntypedModelBuilder()
            };
            var service = new ModelBuilderService(inputBuilders);

            // Act
            var outputBuilders = service.GetModelBuilders(dbContextType, false);

            // Assert
            Assert.Equal(1, outputBuilders.Length);
            Assert.Contains(inputBuilders[0], outputBuilders);
        }

        [Fact]
        public void GetModelBuilders_SupportsContextTypedModelBuilders()
        {
            // Arrange
            var dbContextType = typeof(CatalogDbContext);
            var inputBuilders = new IModelBuilder[]
            {
                new CatalogDbContextModelBuilder(),
                new OtherDbContextModelBuilder()
            };
            var service = new ModelBuilderService(inputBuilders);

            // Act
            var outputBuilders = service.GetModelBuilders(dbContextType, false);

            // Assert
            Assert.Equal(1, outputBuilders.Length);
            Assert.Contains(inputBuilders[0], outputBuilders);
        }

        [Fact]
        public void GetModelBuilders_SupportsEntityTypedModelBuilders_FromDbSetProperties()
        {
            // Arrange
            var dbContextType = typeof(CatalogDbContext);
            var inputBuilders = new IModelBuilder[]
            {
                new ProductEntityTypeBuilder(),
                new ProductVariantEntityTypeBuilder(),
                new VendorEntityTypeBuilder(),
                new AttributeEntityTypeBuilder()
            };
            var service = new ModelBuilderService(inputBuilders);

            // Act
            var outputBuilders = service.GetModelBuilders(dbContextType, false /* includeNavigations */);

            // Assert
            Assert.Equal(3, outputBuilders.Length);
            Assert.Contains(inputBuilders[0], outputBuilders);
            Assert.Contains(inputBuilders[1], outputBuilders);
            Assert.Contains(inputBuilders[2], outputBuilders);
        }

        [Fact]
        public void GetModelBuilders_SupportsEntityTypeModelBuilders_FromSingularNavigation()
        {
            // Arrange
            var dbContextType = typeof(CatalogDbContext);
            var inputBuilders = new IModelBuilder[]
            {
                new ProductOptionEntityTypeBuilder()
            };
            var service = new ModelBuilderService(inputBuilders);

            // Act
            var outputBuilders = service.GetModelBuilders(dbContextType, true /* includeNavigations */);

            // Assert
            Assert.Equal(1, outputBuilders.Length);
            Assert.Contains(inputBuilders[0], outputBuilders);
        }

        [Fact]
        public void GetModelBuilders_SupportsEntityTypeModelBuilders_FromICollectionNavigation()
        {
            // Arrange
            var dbContextType = typeof(CatalogDbContext);
            var inputBuilders = new IModelBuilder[]
            {
                new AttributeEntityTypeBuilder()
            };
            var service = new ModelBuilderService(inputBuilders);

            // Act
            var outputBuilders = service.GetModelBuilders(dbContextType, true /* includeNavigations */);

            // Assert
            Assert.Equal(1, outputBuilders.Length);
            Assert.Contains(inputBuilders[0], outputBuilders);
        }

        [Fact]
        public void GetModelBuilders_SupportsEntityTypeModelBuilders_FromListNavigation()
        {
            // Arrange
            var dbContextType = typeof(CatalogDbContext);
            var inputBuilders = new IModelBuilder[]
            {
                new VendorOptionEntityTypeBuilder()
            };
            var service = new ModelBuilderService(inputBuilders);

            // Act
            var outputBuilders = service.GetModelBuilders(dbContextType, true /* includeNavigations */);

            // Assert
            Assert.Equal(1, outputBuilders.Length);
            Assert.Contains(inputBuilders[0], outputBuilders);
        }

        [Fact]
        public void GetModelBuilders_SupportsEntityTypeModelBuilders_FromIListNavigation()
        {
            // Arrange
            var dbContextType = typeof(CatalogDbContext);
            var inputBuilders = new IModelBuilder[]
            {
                new ProductVariantOptionEntityTypeBuilder()
            };
            var service = new ModelBuilderService(inputBuilders);

            // Act
            var outputBuilders = service.GetModelBuilders(dbContextType, true /* includeNavigations */);

            // Assert
            Assert.Equal(1, outputBuilders.Length);
            Assert.Contains(inputBuilders[0], outputBuilders);
        }

        private class Product
        {
            public Vendor Vendor { get; set; }

            public ICollection<Product> Products { get; set; }

            public ICollection<Attribute> Attributes { get; set; }

            public ProductOption ProductOption { get; set; }

            public string Name { get; set; }

            public decimal Cost { get; set; }
        }

        private class ProductEntityTypeBuilder : EntityTypeBuilderBase<Product>
        {
            public override void BuildEntity(EntityTypeBuilder<Product> builder)
            {

            }
        }

        private class ProductVariant
        {
            public Product Product { get; set; }

            public IList<ProductVariantOption> Options { get; set; }
        }

        private class ProductVariantEntityTypeBuilder : EntityTypeBuilderBase<ProductVariant>
        {
            public override void BuildEntity(EntityTypeBuilder<ProductVariant> builder)
            {

            }
        }

        private class Vendor
        {
            public List<Product> Products { get; set; }

            public List<VendorOption> Options { get; set; }
        }

        private class VendorOption
        {

        }

        private class VendorOptionEntityTypeBuilder : EntityTypeBuilderBase<VendorOption>
        {
            public override void BuildEntity(EntityTypeBuilder<VendorOption> builder)
            {
            }
        }

        private class VendorEntityTypeBuilder : EntityTypeBuilderBase<Vendor>
        {
            public override void BuildEntity(EntityTypeBuilder<Vendor> builder)
            {

            }
        }

        private class Attribute
        {

        }

        private class AttributeEntityTypeBuilder : EntityTypeBuilderBase<Attribute>
        {
            public override void BuildEntity(EntityTypeBuilder<Attribute> builder)
            {

            }
        }

        private class ProductOption
        {

        }

        private class ProductOptionEntityTypeBuilder : EntityTypeBuilderBase<ProductOption>
        {
            public override void BuildEntity(EntityTypeBuilder<ProductOption> builder)
            {
            }
        }

        private class ProductVariantOption
        {

        }

        private class ProductVariantOptionEntityTypeBuilder : EntityTypeBuilderBase<ProductVariantOption>
        {
            public override void BuildEntity(EntityTypeBuilder<ProductVariantOption> builder)
            {
            }
        }

        private class CatalogDbContextModelBuilder : ModelBuilderBase<CatalogDbContext>
        {
            public override void BuildModel(ModelBuilder builder)
            {

            }
        }

        private class UntypedModelBuilder : ModelBuilderBase
        {
            public override void BuildModel(ModelBuilder builder)
            {

            }
        }

        private class CatalogDbContext : DbContext
        {
            public DbSet<Vendor> Vendors { get; set; }

            public DbSet<Product> Products { get; set; }

            public DbSet<ProductVariant> ProductVariants { get; set; }
        }

        private class OtherDbContext : DbContext { }

        private class OtherDbContextModelBuilder : ModelBuilderBase<OtherDbContext>
        {
            public override void BuildModel(ModelBuilder builder)
            {

            }
        }
    }
}
