// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Provides argument checking services.
    /// </summary>
    [DebuggerStepThrough]
    internal static class Ensure
    {
        /// <summary>
        /// Ensures the given value is not null.
        /// </summary>
        /// <typeparam name="T">The parameter type</typeparam>
        /// <param name="value">The parameter value</param>
        /// <param name="parameterName">The parameter name</param>
        /// <exception cref="ArgumentNullException">If the given argument is null.</exception>
        /// <returns>The parameter value</returns>
        public static T NotNull<T>(T value, string parameterName) where T : class
        {
            if (value == null)
            {
                throw new ArgumentNullException(parameterName);
            }

            return value;
        }
    }
}