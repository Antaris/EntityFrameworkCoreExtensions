// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions
{
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a cached completed task.
    /// </summary>
    internal static class TaskCache
    {
        /// <summary>
        /// A completed task.
        /// </summary>
        public static readonly Task CompletedTask = Task.FromResult(true);
    }
}