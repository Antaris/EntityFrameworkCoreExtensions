// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Storage
{
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Storage;

    /// <summary>
    /// Provides a base implementation of a relational type mapper hook.
    /// </summary>
    public abstract class RelationalTypeMapperHook : IRelationalTypeMapperHook
    {
        /// <inheritdoc />
        public virtual RelationalTypeMapping FindMapping(IProperty property)
            => null;
    }
}
