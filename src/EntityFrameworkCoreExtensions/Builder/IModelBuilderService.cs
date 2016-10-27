// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Builder
{
    using System;

    /// <summary>
    /// Defines the required contract for implementing a model builder service.
    /// </summary>
    public interface IModelBuilderService
    {
        /// <summary>
        /// Gets the set of model builders for the given database context type.
        /// </summary>
        /// <param name="dbContextType">The database context type.</param>
        /// <param name="includeNavigations">Indicates whether navigation entities should be included.</param>
        /// <returns>The set of model builders.</returns>
        IModelBuilder[] GetModelBuilders(Type dbContextType, bool includeNavigations);
    }
}
