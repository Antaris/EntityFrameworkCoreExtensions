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
    using Microsoft.EntityFrameworkCore.Infrastructure;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.EntityFrameworkCore.Internal;

    /// <summary>
    /// Represents an extended database context with support for hooks.
    /// </summary>
    public class ExtendedDbContext : DbContext
    {
        private readonly Type _dbContextType;
        private ActionExecutor<IDbContextHook> _executor;
        private DbContextOptions _options;

        /// <summary>
        /// Initialises a new instance of <see cref="ExtendedDbContext"/>
        /// </summary>
        public ExtendedDbContext()
            : this(new DbContextOptions<ExtendedDbContext>())
        { }

        /// <summary>
        /// Initialises a new instance of <see cref="ExtendedDbContext"/>
        /// </summary>
        /// <param name="options">The database context options.</param>
        public ExtendedDbContext(DbContextOptions options)
            : base(options)
        {
            _dbContextType = GetType();

            _options = Ensure.NotNull(options, nameof(options));

            // MA - Flow the database context type to the hooks.
            Executor.Execute(h => h.SetDbContextType(_dbContextType));
        }
        
        private ActionExecutor<IDbContextHook> Executor
        {
            get
            {
                return _executor ?? (_executor = CreateExecutor(InternalServiceProvider.GetServices<IDbContextHook>()));
            }
        }

        private IServiceProvider InternalServiceProvider
        {
            get
            {
                return ServiceProviderCache.Instance.GetOrAdd(_options);
            }
        }

        /// <inheritdoc />
        public override EntityEntry<TEntity> Add<TEntity>(TEntity entity)
        {
            Executor.Execute(hook => hook.AddingEntry<TEntity>(entity));

            var entry = base.Add<TEntity>(entity);

            Executor.Execute(hook => hook.AddedEntry(entity, entry));

            return entry;
        }

        /// <inheritdoc />
        public override EntityEntry<TEntity> Attach<TEntity>(TEntity entity)
        {
            Executor.Execute(hook => hook.AttachingEntry(entity));

            var entry = base.Attach(entity);

            Executor.Execute(hook => hook.AttachedEntry(entity, entry));

            return entry;
        }

        /// <inheritdoc />
        protected sealed override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            Executor.Execute(hook => hook.OnConfiguring(optionsBuilder));
        }

        /// <inheritdoc />
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            Executor.Execute(hook => hook.OnModelCreating(modelBuilder));
        }

        /// <inheritdoc />
        public override EntityEntry<TEntity> Remove<TEntity>(TEntity entity)
        {
            Executor.Execute(hook => hook.RemovingEntry(entity));

            var entry = base.Remove(entity);

            Executor.Execute(hook => hook.RemovedEntry(entity, entry));

            return entry;
        }

        /// <inheritdoc />
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            Executor.Execute(hook => hook.SavingChanges(acceptAllChangesOnSuccess));

            int result = base.SaveChanges(acceptAllChangesOnSuccess);

            Executor.Execute(hook => hook.SavedChanges(result, acceptAllChangesOnSuccess));

            return result;
        }

        /// <inheritdoc />
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            Executor.Execute(async hook => await hook.SavingChangesAsync(acceptAllChangesOnSuccess, cancellationToken));

            int results = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

            Executor.Execute(async hook => await hook.SavedChangesAsync(results, acceptAllChangesOnSuccess, cancellationToken));

            return results;
        }

        /// <inheritdoc />
        public override EntityEntry<TEntity> Update<TEntity>(TEntity entity)
        {
            Executor.Execute(hook => hook.UpdatingEntry(entity));

            var entry = base.Update(entity);

            Executor.Execute(hook => hook.UpdatedEntry(entity, entry));

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
