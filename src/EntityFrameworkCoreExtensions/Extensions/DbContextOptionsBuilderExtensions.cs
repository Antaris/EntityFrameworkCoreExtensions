// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions
{
    using System.Reflection;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Infrastructure;

    /// <summary>
    /// Adds extensions to the <see cref="DbContextOptions"/> type.
    /// </summary>
    public static class DbContextOptionsBuilderExtensions
    {
        /// <summary>
        /// Adds support for the auto-model feature.
        /// </summary>
        /// <param name="builder">The options builder.</param>
        /// <returns>The options builder.</returns>
        public static DbContextOptionsBuilder AddAutoModel(this DbContextOptionsBuilder builder)
        {
            Ensure.NotNull(builder, nameof(builder));

            AddOrUpdateExtension(builder, new AutoModelDbContextOptionsExtension());

            return builder;
        }

        /// <summary>
        /// Adds the model builders from the given assemblies.
        /// </summary>
        /// <param name="builder">The options builder.</param>
        /// <param name="assemblies">The source set of assembies.</param>
        /// <returns>The options builder.</returns>
        public static DbContextOptionsBuilder AddHooks(this DbContextOptionsBuilder builder, params Assembly[] assemblies)
        {
            Ensure.NotNull(builder, nameof(builder));

            var extension = builder.Options.FindExtension<AddHooksDbContextOptionsExtension>();
            extension = (extension == null)
                ? new AddHooksDbContextOptionsExtension(assemblies)
                : new AddHooksDbContextOptionsExtension(extension, assemblies);

            AddOrUpdateExtension(builder, extension);

            return builder;
        }

        /// <summary>
        /// Adds support for the mixins feature.
        /// </summary>
        /// <param name="builder">The options builder.</param>
        /// <returns>The options builder.</returns>
        public static DbContextOptionsBuilder AddMixins(this DbContextOptionsBuilder builder)
        {
            Ensure.NotNull(builder, nameof(builder));

            AddOrUpdateExtension(builder, new MixinsDbContextOptionsExtension());

            return builder;
        }

        /// <summary>
        /// Adds the model builders from the given assemblies.
        /// </summary>
        /// <param name="builder">The options builder.</param>
        /// <param name="assemblies">The source set of assembies.</param>
        /// <returns>The options builder.</returns>
        public static DbContextOptionsBuilder AddModelBuilders(this DbContextOptionsBuilder builder, params Assembly[] assemblies)
        {
            Ensure.NotNull(builder, nameof(builder));

            var extension = builder.Options.FindExtension<AddModelBuildersDbContextOptionsExtension>();
            extension = (extension == null)
                ? new AddModelBuildersDbContextOptionsExtension(assemblies)
                : new AddModelBuildersDbContextOptionsExtension(extension, assemblies);

            AddOrUpdateExtension(builder, extension);

            return builder;
        }

        /// <summary>
        /// Adds a custom extension to the options.
        /// </summary>
        /// <typeparam name="T">The extensions type.</typeparam>
        /// <param name="builder">The options builder.</param>
        /// <param name="extension">The extension instance.</param>
        /// <returns>The options builder.</returns>
        public static DbContextOptionsBuilder AddExtension<T>(this DbContextOptionsBuilder builder, T extension)
            where T : class, IDbContextOptionsExtension
        {
            Ensure.NotNull(extension, nameof(extension));

            AddOrUpdateExtension<T>(builder, extension);

            return builder;
        }

        private static void AddOrUpdateExtension<T>(DbContextOptionsBuilder builder, T extension)
            where T : class, IDbContextOptionsExtension
            => ((IDbContextOptionsBuilderInfrastructure)builder).AddOrUpdateExtension(extension);
    }
}
