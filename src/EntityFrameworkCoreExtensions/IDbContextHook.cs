// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

namespace EntityFrameworkCoreExtensions
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.ChangeTracking;

    /// <summary>
    /// Defines the required contract for implementing a database context hook.
    /// </summary>
    public interface IDbContextHook
    {
        /// <summary>
        /// Gets the database context type.
        /// </summary>
        Type DbContextType { get; }

        /// <summary>
        /// Gets whether this is a type database context hook.
        /// </summary>
        bool IsTyped { get; }

        /// <summary>
        /// Fired when the database context is adding an entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="entity">The entity instance.</param>
        void AddingEntry<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// Fired when the database context has added an entry.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="entity">The entity instance.</param>
        /// <param name="entry">The entity entry.</param>
        void AddedEntry<TEntity>(TEntity entity, EntityEntry<TEntity> entry) where TEntity : class;

        /// <summary>
        /// Fired when the database context is attaching an entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="entity">The entity instance.</param>
        void AttachingEntry<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// Fired when the database context has attached an entry.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="entity">The entity instance.</param>
        /// <param name="entry">The entity entry.</param>
        void AttachedEntry<TEntity>(TEntity entity, EntityEntry<TEntity> entry) where TEntity : class;

        /// <summary>
        /// Fired when the database context is configuring.
        /// </summary>
        /// <param name="optionsBuilder">The options builder.</param>
        void OnConfiguring(DbContextOptionsBuilder optionsBuilder);

        /// <summary>
        /// Fired when the model is being created for the database context.
        /// </summary>
        /// <param name="modelBuilder"></param>
        void OnModelCreating(ModelBuilder modelBuilder);

        /// <summary>
        /// Fired when the database context is removing an entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="entity">The entity instance.</param>
        void RemovingEntry<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// Fired when the database context has removed an entry.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="entity">The entity instance.</param>
        /// <param name="entry">The entity entry.</param>
        void RemovedEntry<TEntity>(TEntity entity, EntityEntry<TEntity> entry) where TEntity : class;

        /// <summary>
        /// Fired when the database context is saving changes.
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">Indicates whether the change tracker should accept changes after the changes have been sent successfully to the database.</param>
        void SavingChanges(bool acceptAllChangesOnSuccess);

        /// <summary>
        /// Fired when the database context has saved changes.
        /// </summary>
        /// <param name="persistedStateEntries">The number of state entries persisted to the database.</param>
        /// <param name="acceptedAllChangesOnSuccess">Indicates whether the change tracker accepted changes after the changes had been sent successfully to the database.</param>
        void SavedChanges(int persistedStateEntries, bool acceptedAllChangesOnSuccess);

        /// <summary>
        /// Fired when the database context is saving changes asynchronously.
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">Indicates whether the change tracker should accept changes after the changes have been sent successfully to the database.</param>
        /// <param name="cancellationToken">The cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task instance used to await this method.</returns>
        Task SavingChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken);

        /// <summary>
        /// Fired when the database context has saved changes asynchronously.
        /// </summary>
        /// <param name="persistedStateEntries">The number of state entries persisted to the database.</param>
        /// <param name="acceptedAllChangesOnSuccess">Indicates whether the change tracker accepted changes after the changes had been sent successfully to the database.</param>
        /// <param name="cancellationToken">The cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task instance used to await this method.</returns>
        Task SavedChangesAsync(int persistedStateEntries, bool acceptedAllChangesOnSuccess, CancellationToken cancellationToken);
        
        /// <summary>
        /// Sets the database context type for this hook.
        /// </summary>
        /// <param name="type">The database context type.</param>
        void SetDbContextType(Type type);

        /// <summary>
        /// Fired when the database context is updating an entity.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="entity">The entity instance.</param>
        void UpdatingEntry<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// Fired when the database context has updated an entry.
        /// </summary>
        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <param name="entity">The entity instance.</param>
        /// <param name="entry">The entity entry.</param>
        void UpdatedEntry<TEntity>(TEntity entity, EntityEntry<TEntity> entry) where TEntity : class;
    }

    /// <summary>
    /// Defines the required contract for implementing a database context hook.
    /// </summary>
    /// <typeparam name="T">The database context type.</typeparam>
    public interface IDbContextHook<T> : IDbContextHook
        where T : DbContext
    {
    }
}
