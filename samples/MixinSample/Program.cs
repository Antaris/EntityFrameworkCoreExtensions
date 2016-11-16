// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace MixinSample
{
    using System.Linq;
    using System.Reflection;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;

    using EntityFrameworkCoreExtensions;
    using EntityFrameworkCoreExtensions.Builder;

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
                .AddDbContext<CatalogDbContext>(options => options
                    .UseInMemoryDatabase()
                    .AddAutoModel()
                    .AddModelBuilders(assemblies)
                    .AddMixins()
                    .AddHooks(assemblies)
                );
            var provider = services.BuildServiceProvider();

            var context = provider.GetService<CatalogDbContext>();

            var product = new Product()
            {
                Name = "Hello"
            };
            product.SetMixin(new Option()
            {
                Value = "World"
            });

            context.Products.Add(product);
            context.SaveChanges();

            var product2 = context.Products.SingleOrDefault(p => p.Name == "Hello");
            var option2 = product2.Mixin<Option>();
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
