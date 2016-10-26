// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions.ChangeTracking
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;

    /// <summary>
    /// An extended change detector with support for hooks.
    /// </summary>
    public class ExtendedChangeDetector : ChangeDetector
    {
        private readonly ActionExecutor<IChangeDetectorHook> _executor;

        /// <summary>
        /// Initialises a new instance of <see cref="ExtendedChangeDetector"/>
        /// </summary>
        /// <param name="hooks">The set of change detector hooks.</param>
        public ExtendedChangeDetector(IEnumerable<IChangeDetectorHook> hooks)
        {
            _executor = new ActionExecutor<IChangeDetectorHook>(Ensure.NotNull(hooks, nameof(hooks)).ToArray());
        }

        /// <inheritdoc />
        public override void DetectChanges(InternalEntityEntry entry)
        {
            _executor.Execute(hook => hook.DetectingEntryChanges(this, entry.StateManager, entry));

            base.DetectChanges(entry);

            _executor.Execute(hook => hook.DetectedEntryChanges(this, entry.StateManager, entry));
        }

        /// <inheritdoc />
        public override void DetectChanges(IStateManager stateManager)
        {
            _executor.Execute(hook => hook.DetectingChanges(this, stateManager));

            base.DetectChanges(stateManager);

            _executor.Execute(hook => hook.DetectedChanges(this, stateManager));
        }
    }
}
