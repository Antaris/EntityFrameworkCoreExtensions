// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Storage
{
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Storage;

    /// <summary>
    /// Defines the required contract for implementing a type mapper hook.
    /// </summary>
    public interface IRelationalTypeMapperHook
    {
        /// <summary>
        /// Gets the relational database type for the given property.
        /// </summary>
        /// <param name="property">The property to get the mapping for.</param>
        /// <returns>The type mapping to be used.</returns>
        RelationalTypeMapping FindMapping(IProperty property);
    }
}
