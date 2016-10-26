// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions
{
    using System;
    using System.Linq;

    /// <summary>
    /// Converts an array enumeration into a delegated action flow.
    /// </summary>
    /// <typeparam name="T">The element type.</typeparam>
    public class ActionExecutor<T>
    {
        private readonly Action<Action<T>> _flow;

        /// <summary>
        /// Initialises a new instance of <see cref="ActionExecutor{T}"/>
        /// </summary>
        /// <param name="items">The source array.</param>
        public ActionExecutor(T[] items)
        {
            _flow = CreateFlow(Ensure.NotNull(items, nameof(items)));
        }

        /// <summary>
        /// Executes the given action over all items in the source array.
        /// </summary>
        /// <param name="action">The action to execute.</param>
        public void Execute(Action<T> action)
            => _flow?.Invoke(Ensure.NotNull(action, nameof(action)));

        private static Action<Action<T>> CreateFlow(T[] items)
            => items
                .Reverse()
                .Aggregate((Action<Action<T>>)null, (prev, item) => CreateFlowCore(item, prev));

        private static Action<Action<T>> CreateFlowCore(T item, Action<Action<T>> prev)
            => (act) =>
            {
                act(item);
                prev?.Invoke(act);
            };
    }
}
