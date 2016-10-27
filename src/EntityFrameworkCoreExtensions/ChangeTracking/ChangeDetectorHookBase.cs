// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.ChangeTracking
{
    using System;
    using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

    /// <summary>
    /// Provides a base implementation of a <see cref="IChangeDetectorHook"/>.
    /// </summary>
    public abstract class ChangeDetectorHookBase : IChangeDetectorHook
    {
        /// <inheritdoc />
        public virtual void DetectedChanges(IChangeDetector changeDetector, IStateManager stateManager)
        {
        }

        /// <inheritdoc />
        public virtual void DetectedEntryChanges(IChangeDetector changeDetector, IStateManager stateManager, InternalEntityEntry entry)
        {
        }

        /// <inheritdoc />
        public virtual void DetectingChanges(IChangeDetector changeDetector, IStateManager stateManager)
        {
        }

        /// <inheritdoc />
        public virtual void DetectingEntryChanges(IChangeDetector changeDetector, IStateManager stateManager, InternalEntityEntry entry)
        {
        }
    }
}
