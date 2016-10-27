// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Mixins
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Provides a base implementation of a mixin host.
    /// </summary>
    public abstract class MixinHostBase : IMixinHost
    {
        private Lazy<Dictionary<Type, object>> _dictThunk = new Lazy<Dictionary<Type, object>>(() => new Dictionary<Type, object>());

        /// <inheritdoc />
        public virtual TMixin Mixin<TMixin>() where TMixin : class
        {
            object instance;
            if (_dictThunk.Value.TryGetValue(typeof(TMixin), out instance))
            {
                return (TMixin)instance;
            }

            return default(TMixin);
        }

        /// <inheritdoc />
        public virtual void SetMixin<TMixin>(TMixin mixin) where TMixin : class
            => _dictThunk.Value[typeof(TMixin)] = mixin;

        object IMixinHost.GetMixin(Type mixinType)
        {
            object instance;
            if (_dictThunk.Value.TryGetValue(mixinType, out instance))
            {
                return instance;
            }

            return null;
        }
 
        void IMixinHost.SetMixin(Type mixinType, object mixin)
        {
            Ensure.NotNull(mixinType, nameof(mixinType));

            _dictThunk.Value[mixinType] = mixin;
        }
    }
}
