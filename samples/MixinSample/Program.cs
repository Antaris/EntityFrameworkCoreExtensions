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
    using EntityFrameworkCoreExtensions.ChangeTracking;
    using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
    using Microsoft.EntityFrameworkCore.Infrastructure;

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
                .AddEntityFrameworkSqlServer()
                .AddDbContext<CatalogDbContext>(options => options
                    .UseSqlServer(@"SERVER=(localdb)\MSSQLLocalDb;Database=MixinSample;Integrated Security=true;")
                    .AddAutoModel()
                    .AddModelBuilders(assemblies)
                    .AddMixins()
                    .AddHooks(assemblies)
                );
            var provider = services.BuildServiceProvider();

            var context = provider.GetService<CatalogDbContext>();

            /* FIRST RUN */
            //var product = new Product()
            //{
            //    Name = "Hello"
            //};
            //product.SetMixin(new Option()
            //{
            //    Value = "World"
            //});

            //context.Products.Add(product);
            //context.SaveChanges();


            var product2 = context.Products.SingleOrDefault(p => p.Name == "Hello");
            product2.SetMixin<Option>(null);

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

    public class Option2
    {
        public string Value { get; set; }
    }

    public class Option2MixinEntityTypeBuilder : MixinTypeBuilderBase<Product, Option2>
    {
        public override void BuildMixin(MixinTypeBuilder<Option2> builder)
        {
            builder.Property(o => o.Value).HasMaxLength(200);
        }
    }


    // This is to support migrations :-/
    public class CatalogDbContextFactory : IDbContextFactory<CatalogDbContext>
    {
        public CatalogDbContext Create(DbContextFactoryOptions opts)
        {
            var assemblies = new[]
            {
                typeof(Program).GetTypeInfo().Assembly,
                typeof(CatalogDbContext).GetTypeInfo().Assembly
            };

            var services = new ServiceCollection();
            services
                .AddEntityFrameworkSqlServer()
                .AddDbContext<CatalogDbContext>(options => options
                    .UseSqlServer(@"SERVER=(localdb)\MSSQLLocalDb;Database=MixinSample;Integrated Security=true;", b => b.MigrationsAssembly("MixinSample"))
                    .AddAutoModel()
                    .AddModelBuilders(assemblies)
                    .AddMixins()
                    .AddHooks(assemblies)
                );
            var provider = services.BuildServiceProvider();

            return provider.GetService<CatalogDbContext>();
        }
    }
}
