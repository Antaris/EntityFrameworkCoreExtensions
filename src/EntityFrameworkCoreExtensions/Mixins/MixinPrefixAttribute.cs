// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Mixins
{
    using System;

    /// <summary>
    /// Decorates a mixin class with a name prefix.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class MixinPrefixAttribute : Attribute
    {
        /// <summary>
        /// Initialises a new instance of <see cref="MixinPrefixAttribute"/>
        /// </summary>
        /// <param name="prefix">The prefix.</param>
        public MixinPrefixAttribute(string prefix)
        {
            Prefix = Ensure.NotNullOrEmpty(prefix, nameof(prefix));
        }

        /// <summary>
        /// Gets the prefix.
        /// </summary>
        public string Prefix { get; }
    }
}
