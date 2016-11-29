// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.Mixins
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
    using Microsoft.EntityFrameworkCore.Metadata;

    using EntityFrameworkCoreExtensions.ChangeTracking;

    /// <summary>
    /// Hooks into the change detector to set entity entry status for mixin properties.
    /// </summary>
    public class MixinChangeDetectorHook : ChangeDetectorHookBase
    {
        /// <inheritdoc />
        public override void DetectedEntryChanges(IChangeDetector changeDetector, IStateManager stateManager, InternalEntityEntry entry)
        {
            var mixinHost = entry.Entity as IMixinHost;
            if (mixinHost == null)
            {
                // MA - Not a mixin, so nothing to do.
                return;
            }

            foreach (var mixinType in GetMixinTypes(entry.EntityType))
            {
                var mixinTypeInfo = mixinType.GetTypeInfo();

                // MA - Get the mixin from the entity.
                var mixin = mixinHost.GetMixin(mixinType);
                // MA - Generate the mixin prefix.
                string prefix = $"{mixinType.GetMixinPrefix()}_";
                // MA - Get the properties
                var properties = GetMixinProperties(entry.EntityType, prefix);

                if (mixin == null)
                {
                    // MA - Mixin is set to null, so we'll need to determine if any of the mixin properties are not null, and set them accordingly.
                    foreach (var property in properties)
                    {
                        if (entry.GetCurrentValue(property) != null)
                        {
                            // MA - Override the value and set it to null.
                            entry.SetCurrentValue(property, null);
                        }
                    }
                }
                else
                {
                    // MA - Mixin is not null, so compare values to the current values.
                    foreach (var property in properties)
                    {
                        string propertyName = property.Name.Substring(prefix.Length);

                        // MA - Get the property info.
                        var propertyInfo = mixinTypeInfo.GetDeclaredProperty(propertyName);
                        // MA - Get the current value.
                        var value = propertyInfo.GetValue(mixin);

                        if (!Equals(entry.GetCurrentValue(property), value))
                        {
                            // MA - Replace the current value.
                            entry.SetCurrentValue(property, value);
                        }
                    }
                }
            }
        }

        private IEnumerable<Type> GetMixinTypes(IEntityType entityType)
        {
            foreach (var annotation in entityType.GetAnnotations().Where(a => a.Name.StartsWith("mixin-", StringComparison.Ordinal)))
            {
                // MA - The mixin type is stored in the annotation.
                //yield return (Type)annotation.Value;
                yield return Type.GetType((string)annotation.Value);
            }
        }

        private IEnumerable<IProperty> GetMixinProperties(IEntityType entityType, string prefix)
        {
            foreach (var property in entityType.GetProperties().Where(p => p.Name.StartsWith(prefix, StringComparison.Ordinal)))
            {
                yield return property;
            }
        }
    }
}
