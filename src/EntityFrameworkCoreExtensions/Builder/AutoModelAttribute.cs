// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Builder
{
    using System;

    /// <summary>
    /// Marks a database context to enable auto-building of the model.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class AutoModelAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets whether to include navigations when building the model.
        /// </summary>
        public bool IncludeNavigations { get; set; } = true;
    }
}
