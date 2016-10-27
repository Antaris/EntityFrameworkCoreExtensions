// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;
    
    /// <summary>
    /// Provides a base implementation of a database context hook.
    /// </summary>
    public abstract class DbContextHookBase : IDbContextHook
    {
        private Type _dbContextType = null;

        /// <inheritdoc />
        public virtual Type DbContextType => _dbContextType;

        /// <inheritdoc />
        public virtual bool IsTyped => false;

        /// <inheritdoc />
        public virtual void AddedEntry<TEntity>(TEntity entity, EntityEntry<TEntity> entry) where TEntity : class
        {
        }

        /// <inheritdoc />
        public virtual void AddingEntry<TEntity>(TEntity entity) where TEntity : class
        {
        }

        /// <inheritdoc />
        public virtual void AttachedEntry<TEntity>(TEntity entity, EntityEntry<TEntity> entry) where TEntity : class
        {
        }

        /// <inheritdoc />
        public virtual void AttachingEntry<TEntity>(TEntity entity) where TEntity : class
        {
        }

        /// <inheritdoc />
        public virtual void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        /// <inheritdoc />
        public virtual void OnModelCreating(ModelBuilder modelBuilder)
        {
        }

        /// <inheritdoc />
        public virtual void RemovedEntry<TEntity>(TEntity entity, EntityEntry<TEntity> entry) where TEntity : class
        {
        }

        /// <inheritdoc />
        public virtual void RemovingEntry<TEntity>(TEntity entity) where TEntity : class
        {
        }

        /// <inheritdoc />
        public virtual void SavedChanges(int persistedStateEntries, bool acceptedAllChangesOnSuccess)
        {
        }

        /// <inheritdoc />
        public virtual Task SavedChangesAsync(int persistedStateEntries, bool acceptedAllChangesOnSuccess, CancellationToken cancellationToken)
            => TaskCache.CompletedTask;

        /// <inheritdoc />
        public virtual void SavingChanges(bool acceptAllChangesOnSuccess)
        {
        }

        /// <inheritdoc />
        public virtual Task SavingChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken)
            => TaskCache.CompletedTask;

        /// <inheritdoc />
        public virtual void UpdatedEntry<TEntity>(TEntity entity, EntityEntry<TEntity> entry) where TEntity : class
        {
        }

        /// <inheritdoc />
        public virtual void UpdatingEntry<TEntity>(TEntity entity) where TEntity : class
        {
        }

        /// <inheritdoc />
        void IDbContextHook.SetDbContextType(Type type)
            => _dbContextType = Ensure.NotNull(type, nameof(type));
    }

    /// <summary>
    /// Provides a base implementation of a typed database context hook.
    /// </summary>
    /// <typeparam name="T">The database context type.</typeparam>
    public abstract class DbContextHookBase<T> : DbContextHookBase, IDbContextHook<T>
        where T : DbContext
    {
        /// <inheritdoc />
        public override Type DbContextType => typeof(T);

        /// <inheritdoc />
        public override bool IsTyped => true;

        /// <inheritdoc />
        void IDbContextHook.SetDbContextType(Type type)
        { }
    }
}
