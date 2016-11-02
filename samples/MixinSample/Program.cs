// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace MixinSample
{
    using System.Reflection;
    using System.Collections.Generic;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.Extensions.DependencyInjection;

    using EntityFrameworkCoreExtensions;
    using EntityFrameworkCoreExtensions.Builder;
    using EntityFrameworkCoreExtensions.Mixins;

    using MixinSampleModule;

    public class Program
    {
        public static void Main(string[] args)
        {
            var assemblies = new[]
            {
                typeof(Program).GetTypeInfo().Assembly,
                typeof(CatalogDbContext).GetTypeInfo().Assembly
            };

            var services = new ServiceCollection();
            services
                .AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<CatalogDbContext>(options => options.UseInMemoryDatabase())
                .AddEntityFrameworkCoreExtensions(b => b
                    .AddHooksFromAssemblies(assemblies)
                    .AddModelBuildersFromAssemblies(assemblies)
                    .AddAutoModel()
                    .AddMixins()
                );
            var provider = services.BuildServiceProvider();

            var context = provider.GetService<CatalogDbContext>();

            var product = new Product()
            {
                Name = "Hello"
            };

            context.Products.Add(product);
            context.SaveChanges();
        }
    }

    public class Option
    {
        public string Value { get; set; }
    }

    public class OptionMixinEntityTypeBuilder : MixinTypeBuilderBase<Product, Option>
    {
        public override void BuildMixin(MixinTypeBuilder<Option> builder)
        {
            builder.Property(o => o.Value).HasMaxLength(200);
        }
    }
}
