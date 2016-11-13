// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Tests
{
    using System;
    using System.Collections.Concurrent;
    using System.Reflection;
    using Microsoft.EntityFrameworkCore.Internal;

    /// <summary>
    /// Provides helper methods for unit tests.
    /// </summary>
    public static class TestHelpers
    {
        public static void ClearServiceProviderCache()
        {
            var type = typeof(ServiceProviderCache);
            var instance = ServiceProviderCache.Instance;
            var field = type.GetTypeInfo().GetField("_configurations", BindingFlags.NonPublic | BindingFlags.Instance);
            var cache = field.GetValue(instance) as ConcurrentDictionary<long, IServiceProvider>;

            cache.Clear();
        }
    }
}
