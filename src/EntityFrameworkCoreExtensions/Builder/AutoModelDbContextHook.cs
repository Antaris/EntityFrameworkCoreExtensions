// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Builder
{
    using System.Reflection;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// Automatically builds the model for a database context using the <see cref="DbSet{T}"/> properties.
    /// </summary>
    public class AutoModelDbContextHook : DbContextHookBase
    {
        private readonly IModelBuilderService _modelBuilderService;

        /// <summary>
        /// Initialises a new instance of <see cref="AutoModelDbContextHook"/>
        /// </summary>
        /// <param name="modelBuilderService">The model builder service.</param>
        public AutoModelDbContextHook(IModelBuilderService modelBuilderService)
        {
            _modelBuilderService = Ensure.NotNull(modelBuilderService, nameof(modelBuilderService));
        }

        /// <inheritdoc />
        public override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (DbContextType == null)
            {
                return;
            }

            var attribute = DbContextType.GetTypeInfo().GetCustomAttribute<AutoModelAttribute>(true);
            if (attribute == null)
            {
                return;
            }

            // MA - Resolve the model builders that can be applied to the model.
            var builders = _modelBuilderService.GetModelBuilders(DbContextType, attribute.IncludeNavigations);

            // Apply each builder.
            foreach (var builder in builders)
            {
                builder.BuildModel(modelBuilder);
            }
        }
    }
}
