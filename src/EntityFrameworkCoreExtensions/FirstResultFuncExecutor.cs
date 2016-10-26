// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions
{
    using System;
    using System.Linq;

    /// <summary>
    /// Converts an array enumeration into a delegated action flow that returns the first non-null result.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    public class FirstResultFuncExecutor<T>
    {
        private readonly Func<Func<T, object>, object> _flow;

        /// <summary>
        /// Initialises a new instance of <see cref="ActionExecutor{T}"/>
        /// </summary>
        /// <param name="items">The source array.</param>
        public FirstResultFuncExecutor(T[] items)
        {
            _flow = CreateFlow(Ensure.NotNull(items, nameof(items)));
        }

        /// <summary>
        /// Executes the given action over all items in the source array.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        public object Execute(Func<T, object> action)
            => _flow?.Invoke(Ensure.NotNull(action, nameof(action)));

        /// <summary>
        /// Executes the given action over all items, halting at the first non-null result.
        /// </summary>
        /// <typeparam name="R">The result type.</typeparam>
        /// <param name="action">The action to execute.</param>
        /// <returns>The result instance, or null.</returns>
        public R Execute<R>(Func<T, R> action) where R : class
            => _flow?.Invoke(t => Ensure.NotNull(action, nameof(action))(t)) as R;

        private static Func<Func<T, object>, object> CreateFlow(T[] items)
            => items
                .Reverse()
                .Aggregate((Func<Func<T, object>, object>)null, (prev, item) => CreateFlowCore(item, prev));

        private static Func<Func<T, object>, object> CreateFlowCore(T item, Func<Func<T, object>, object> prev)
            => (act) =>
            {
                var result = act(item);
                if (result != null)
                {
                    return result;
                }
                return prev?.Invoke(act);
            };
    }
}
