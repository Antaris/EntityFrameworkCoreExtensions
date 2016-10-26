// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Storage
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore.Metadata;
    using Microsoft.EntityFrameworkCore.Storage;
    using Microsoft.EntityFrameworkCore.Storage.Internal;

    /// <summary>
    /// An extended Sql Server type mapper with support for hooks.
    /// </summary>
    public class ExtendedSqlServerTypeMapper : SqlServerTypeMapper
    {
        private readonly FirstResultFuncExecutor<IRelationalTypeMapperHook> _hooks;

        /// <summary>
        /// Initialises a new instance of <see cref="ExtendedSqlServerTypeMapper"/>.
        /// </summary>
        /// <param name="hooks">The set of relational type mapper hooks.</param>
        public ExtendedSqlServerTypeMapper(IEnumerable<IRelationalTypeMapperHook> hooks)
        {
            _hooks = new FirstResultFuncExecutor<IRelationalTypeMapperHook>(Ensure.NotNull(hooks.ToArray(), nameof(hooks)));
        }

        /// <inheritdoc />
        public override RelationalTypeMapping FindMapping(IProperty property)
        {
            return _hooks.Execute(r => r.FindMapping(property))
                ?? base.FindMapping(property);
        }
    }
}
