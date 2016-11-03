// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace MixinSampleModule
{
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    using EntityFrameworkCoreExtensions;
    using EntityFrameworkCoreExtensions.Builder;
    using EntityFrameworkCoreExtensions.Mixins;

    [AutoModel]
    public class CatalogDbContext : ExtendedDbContext
    {
        public CatalogDbContext(IEnumerable<IDbContextHook> hooks, DbContextOptions options) : base(hooks, options)
        {
        }

        public DbSet<Product> Products { get; set; }
    }

    public class Product : MixinHostBase
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class ProductEntityTypeBuilder : EntityTypeBuilderBase<Product>
    {
        public override void BuildEntity(EntityTypeBuilder<Product> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        }
    }
}
