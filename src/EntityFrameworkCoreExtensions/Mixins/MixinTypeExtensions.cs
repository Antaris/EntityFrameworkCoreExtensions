// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Mixins
{
    using System;
    using System.Reflection;

    /// <summary>
    /// Provides mixin extensions for the <see cref="Type"/> type.
    /// </summary>
    public static class MixinTypeExtensions
    {
        /// <summary>
        /// Gets the mixin prefix for the given type.
        /// </summary>
        /// <param name="type">The mixin type.</param>
        /// <returns>The mixin prefix.</returns>
        public static string GetMixinPrefix(this Type type)
            => Ensure.NotNull(type, nameof(type))
                .GetTypeInfo()
                .GetCustomAttribute<MixinPrefixAttribute>()?.Prefix
                ?? type.Name;
    }
}
