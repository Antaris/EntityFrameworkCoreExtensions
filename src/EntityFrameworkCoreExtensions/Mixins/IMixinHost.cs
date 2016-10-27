// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Mixins
{
    using System;

    /// <summary>
    /// Defines the required contract for implementing a mixin host.
    /// </summary>
    public interface IMixinHost
    {
        /// <summary>
        /// Gets the mixin of the given type that is attached to the current entity.
        /// </summary>
        /// <param name="mixinType">The mixin type.</param>
        /// <returns>The mixin instance.</returns>
        object GetMixin(Type mixinType);

        /// <summary>
        /// Gets the mixin of the given type that is attached to the current entity.
        /// </summary>
        /// <typeparam name="TMixin">The mixin type.</typeparam>
        /// <returns>The mixin instance.</returns>
        TMixin Mixin<TMixin>() where TMixin : class;

        /// <summary>
        /// Sets the mixin for the current entity.
        /// </summary>
        /// <typeparam name="TMixin">The mixin type.</typeparam>
        void SetMixin<TMixin>(TMixin mixin) where TMixin : class;

        /// <summary>
        /// Sets the mixin for the current object.
        /// </summary>
        /// <param name="mixinType">The mixin type.</param>
        /// <param name="mixin">The mixin instance.</param>
        void SetMixin(Type mixinType, object mixin);
    }
}
