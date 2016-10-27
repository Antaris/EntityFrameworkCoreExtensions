// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;

    /// <summary>
    /// Represents an extended database context with support for hooks.
    /// </summary>
    public class ExtendedDbContext : DbContext
    {
        private readonly Type _dbContextType;
        private ActionExecutor<IDbContextHook> _executor;

        /// <summary>
        /// Initialises a new instance of <see cref="ExtendedDbContext"/>
        /// </summary>
        /// <param name="hooks">The set of database context hooks.</param>
        /// <param name="options">The database context options.</param>
        public ExtendedDbContext(IEnumerable<IDbContextHook> hooks, DbContextOptions options)
            : base(options)
        {
            _dbContextType = GetType();
            _executor = CreateExecutor(Ensure.NotNull(hooks, nameof(hooks)));

            // MA - Flow the database context type to the hooks.
            _executor.Execute(h => h.SetDbContextType(_dbContextType));
        }

        /// <inheritdoc />
        public override EntityEntry<TEntity> Add<TEntity>(TEntity entity)
        {
            _executor.Execute(hook => hook.AddingEntry<TEntity>(entity));

            var entry = base.Add<TEntity>(entity);

            _executor.Execute(hook => hook.AddedEntry(entity, entry));

            return entry;
        }

        /// <inheritdoc />
        public override EntityEntry<TEntity> Attach<TEntity>(TEntity entity)
        {
            _executor.Execute(hook => hook.AttachingEntry(entity));

            var entry = base.Attach(entity);

            _executor.Execute(hook => hook.AttachedEntry(entity, entry));

            return entry;
        }

        /// <inheritdoc />
        protected sealed override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            _executor.Execute(hook => hook.OnConfiguring(optionsBuilder));
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            _executor.Execute(hook => hook.OnModelCreating(modelBuilder));
        }

        /// <inheritdoc />
        public override EntityEntry<TEntity> Remove<TEntity>(TEntity entity)
        {
            _executor.Execute(hook => hook.RemovingEntry(entity));

            var entry = base.Remove(entity);

            _executor.Execute(hook => hook.RemovedEntry(entity, entry));

            return entry;
        }

        /// <inheritdoc />
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            _executor.Execute(hook => hook.SavingChanges(acceptAllChangesOnSuccess));

            int result = base.SaveChanges(acceptAllChangesOnSuccess);

            _executor.Execute(hook => hook.SavedChanges(result, acceptAllChangesOnSuccess));

            return result;
        }

        /// <inheritdoc />
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            _executor.Execute(async hook => await hook.SavingChangesAsync(acceptAllChangesOnSuccess, cancellationToken));

            int results = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

            _executor.Execute(async hook => await hook.SavedChangesAsync(results, acceptAllChangesOnSuccess, cancellationToken));

            return results;
        }

        /// <inheritdoc />
        public override EntityEntry<TEntity> Update<TEntity>(TEntity entity)
        {
            _executor.Execute(hook => hook.UpdatingEntry(entity));

            var entry = base.Update(entity);

            _executor.Execute(hook => hook.UpdatedEntry(entity, entry));

            return entry;
        }

        private ActionExecutor<IDbContextHook> CreateExecutor(IEnumerable<IDbContextHook> hooks)
        {
            // MA - Get the untyped hooks.
            var untyped = hooks.Where(h => !h.IsTyped);

            // MA - Get the typed hooks.
            var typed = hooks.Where(h => h.IsTyped && h.DbContextType != null && h.DbContextType.IsAssignableFrom(_dbContextType));

            // MA - Create the action executor.
            return new ActionExecutor<IDbContextHook>(untyped.Concat(typed).ToArray());
        }
    }
}
