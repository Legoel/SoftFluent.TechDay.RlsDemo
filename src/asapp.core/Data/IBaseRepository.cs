using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;

namespace Softfluent.Asapp.Core.Data
{
    public interface IBaseRepository<Context> where Context : DbContext
    {
        /// <summary>
        /// Start a transaction
        /// </summary>
        /// <returns></returns>
        IDbContextTransaction BeginTransaction();

        /// <summary>
        /// Add a list of entities
        /// </summary>
        /// <typeparam name="TEntity">Entity Type</typeparam>
        /// <param name="entities"></param>
        /// <returns>number of records</returns>
        int BulkInsert<TEntity>(IEnumerable<TEntity> entities, bool truncateTable) where TEntity : class;

        /// <summary>
        /// Commit all save changes in your transaction
        /// </summary>
        /// <returns></returns>
        void Commit();

        /// <summary>
        /// Commit all save changes in your transaction
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task CommitAsync(CancellationToken cancellationToken = default);

        int Count<TEntity>() where TEntity : class;

        int Count<TEntity>(IQuerySpecification<TEntity> querySpecification) where TEntity : class;

        int Count<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;

        int Count<TEntity>(IQuerySpecification<TEntity> querySpecification, Expression<Func<TEntity, bool>> predicate) where TEntity : class;

        Task<int> CountAsync<TEntity>(CancellationToken cancellationToken = default) where TEntity : class;

        /// <summary>
        /// Get number of TEntity specification Paging specification is ignored
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="querySpecification"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> CountAsync<TEntity>(IQuerySpecification<TEntity> querySpecification, CancellationToken cancellationToken = default) where TEntity : class;

        /// <summary>
        /// Get number of TEntity from a predicate
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<int> CountAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : class;

        /// <summary>
        /// Get number of TEntity from specification and predicate Paging specification is ignored
        /// </summary>
        /// <typeparam name="TEntity">Entity Type</typeparam>
        /// <param name="predicate">Set of criteria</param>
        /// <param name="querySpecification"></param>
        /// <returns>IQuerable of entity</returns>
        Task<int> CountAsync<TEntity>(IQuerySpecification<TEntity> querySpecification, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : class;

        int Create<TEntity>(TEntity model) where TEntity : class, IEntity;

        int Create<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity;

        /// <summary>
        /// Create an TEntity
        /// </summary>
        /// <typeparam name="TEntity">Entity Type</typeparam>
        /// <typeparam name="TKey">The type of the entity identifier</typeparam>
        /// <param name="model"></param>
        /// <returns>number of records</returns>
        Task<int> CreateAsync<TEntity>(TEntity model, CancellationToken cancellationToken = default) where TEntity : class, IEntity;

        /// <summary>
        /// Create an list of entities
        /// </summary>
        /// <typeparam name="TEntity">Entity Type</typeparam>
        /// <typeparam name="TKey">The type of the entity identifier</typeparam>
        /// <param name="entities"></param>
        /// <returns>number of records</returns>
        Task<int> CreateAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class, IEntity;

        /// <summary>
        /// Delete an entity
        /// </summary>
        /// <typeparam name="TEntity">Entity Type</typeparam>
        /// <typeparam name="TKey">The type of the entity identifier</typeparam>
        /// <param name="key">Entity identifier</param>
        /// <returns>number of records</returns>
        Task<int> DeleteAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default) where TEntity : class, IEntity;

        /// <summary>
        /// Delete a list of entities
        /// </summary>
        /// <typeparam name="TEntity">Entity Type</typeparam>
        /// <param name="entities"></param>
        /// <returns>number of records</returns>
        Task<int> DeleteAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class, IEntity;

        /// <summary>
        /// Delete entities which correspond to predicate
        /// </summary>
        /// <typeparam name="TEntity">Entity Type</typeparam>
        /// <param name="predicate">Set of criteria</param>
        /// <returns>number of records</returns>
        Task<int> DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : class, IEntity;

        int ExecuteStoredProcedure(string storedProcedureFormat, params object[] args);

        /// <summary>
        /// Interface Stored procedure
        /// </summary>
        /// <param name="storedProcedureFormat"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        Task<int> ExecuteStoredProcedureAsync(string storedProcedureFormat, CancellationToken cancellationToken, params object[] args);

        bool Exists<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;

        /// <summary>
        /// Check if the collection of an Entity is not Empty
        /// </summary>
        /// <typeparam name="TEntity">Entity Type</typeparam>
        /// <param name="predicate">Set of criteria</param>
        /// <returns>Return True if at least one Element Exist</returns>
        Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : class;

        public TEntity? Get<TEntity, TKey>(TKey key) where TEntity : Entity<TKey>
                                                        where TKey : notnull;

        public TEntity? Get<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;

        public TEntity? Get<TEntity>(IQuerySpecification<TEntity> querySpecification) where TEntity : class;

        /// <summary>
        /// Get entity to correspond with his identifier.
        /// </summary>
        /// <typeparam name="TEntity">Entity Type</typeparam>
        /// <typeparam name="TKey">The type of the entity identifier</typeparam>
        /// <param name="key">Entity identifier</param>
        /// <returns>Entity or null if his identifier isn't found</returns>
        Task<TEntity?> GetAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default) where TEntity : Entity<TKey>
                                                        where TKey : notnull;

        /// <summary>
        /// Get entities which correspond to predicate
        /// </summary>
        /// <typeparam name="TEntity">Entity Type</typeparam>
        /// <param name="predicate">Set of criteria</param>
        /// <returns>Entity or null if no matches</returns>
        Task<TEntity?> GetAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : class;

        /// <summary>
        /// Get entity which correspond to specification
        /// </summary>
        /// <typeparam name="TEntity">Entity Type</typeparam>
        /// <param name="querySpecification"></param>
        /// <returns>Entity or null if no matches</returns>
        Task<TEntity?> GetAsync<TEntity>(IQuerySpecification<TEntity> querySpecification, CancellationToken cancellationToken = default) where TEntity : class;

        List<TResult> GetDistinctValues<TEntity, TResult>(IQuerySpecification<TEntity> querySpecification, Expression<Func<TEntity, TResult>> predicate) where TEntity : class;

        /// <summary>
        /// Retreive a full List of Distinct values
        /// </summary>
        /// <typeparam name="TEntity">Entity Type</typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="querySpecification"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        Task<List<TResult>> GetDistinctValuesAsync<TEntity, TResult>(IQuerySpecification<TEntity> querySpecification, Expression<Func<TEntity, TResult>> predicate, CancellationToken cancellationToken = default) where TEntity : class;

        /// <summary>
        /// Get entities wich coresspond to predicate and specification
        /// </summary>
        /// <typeparam name="TEntity">Entity Type</typeparam>
        /// <param name="predicate">Set of criteria</param>
        /// <param name="querySpecification"></param>
        /// <returns>list of entities or null if no matches</returns>
        IEnumerable<TEntity> GetEnumerable<TEntity>(IQuerySpecification<TEntity> querySpecification, Expression<Func<TEntity, bool>> predicate) where TEntity : class;

        /// <summary>
        /// Get entities which correspond to predicate
        /// </summary>
        /// <typeparam name="TEntity">Entity Type</typeparam>
        /// <param name="predicate">Set of criteria</param>
        /// <returns>list of entities or null if no matches</returns>
        IEnumerable<TEntity> GetEnumerable<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class;

        /// <summary>
        /// Get entities which correspond to specification
        /// </summary>
        /// <typeparam name="TEntity">Entity Type</typeparam>
        /// <param name="querySpecification"></param>
        /// <returns>list of entities or null if no matches</returns>
        IEnumerable<TEntity> GetEnumerable<TEntity>(IQuerySpecification<TEntity> querySpecification) where TEntity : class;

        GridPage<TEntity> GetGridPage<TEntity>(GridCriteria gridCriteria) where TEntity : class;

        Task<GridPage<TEntity>> GetGridPageAsync<TEntity>(GridCriteria gridCriteria, CancellationToken cancellationToken = default) where TEntity : class;

        TOutput Max<TEntity, TOutput>(Expression<Func<TEntity, TOutput>> predicate, TOutput defaultValue) where TEntity : class;

        /// <summary>
        /// Retreive the Max value of an Entity property
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TOutput"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        Task<TOutput> MaxAsync<TEntity, TOutput>(Expression<Func<TEntity, TOutput>> predicate, TOutput defaultValue, CancellationToken cancellationToken = default) where TEntity : class;

        /// <summary>
        /// Remove all save changes in your transaction.
        /// </summary>
        /// <returns></returns>
        void Rollback();

        /// <summary>
        /// Remove all save changes in your transaction.
        /// </summary>
        /// <returns></returns>
        Task RollbackAsync();

        int Update<TEntity>(TEntity entity) where TEntity : class, IEntity;

        int Update<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity;

        int Update<TEntity>(TEntity entity, IQuerySpecification<TEntity> querySpecification) where TEntity : class, IEntity;

        int Update<TEntity>(TEntity entity, Expression<Func<TEntity, bool>> predicate) where TEntity : class, IEntity;

        int Update<TEntity>(TEntity entity, IQuerySpecification<TEntity> querySpecification, Expression<Func<TEntity, bool>> predicate) where TEntity : class, IEntity;

        /// <summary>
        /// Update an entity
        /// </summary>
        /// <typeparam name="TEntity">Entity Type</typeparam>
        /// <typeparam name="TKey">The type of the entity identifier</typeparam>
        /// <param name="entity"></param>
        /// <returns>number of records</returns>
        Task<int> UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class, IEntity;

        /// <summary>
        /// Update a list of entities
        /// </summary>
        /// <typeparam name="TEntity">Entity Type</typeparam>
        /// <typeparam name="TKey">The type of the entity identifier</typeparam>
        /// <param name="entities"></param>
        /// <returns>number of records</returns>
        Task<int> UpdateAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class, IEntity;

        /// <summary>
        /// Update a list entities which correspond to specification
        /// </summary>
        /// <typeparam name="TEntity">Entity Type</typeparam>
        /// <typeparam name="TKey">The type of the entity identifier</typeparam>
        /// <param name="entity"></param>
        /// <param name="querySpecification"></param>
        /// <returns>number of records</returns>
        Task<int> UpdateAsync<TEntity>(TEntity entity, IQuerySpecification<TEntity> querySpecification, CancellationToken cancellationToken = default) where TEntity : class, IEntity;

        /// <summary>
        /// Update a list entities which correspond to predicate
        /// </summary>
        /// <typeparam name="TEntity">Entity Type</typeparam>
        /// <typeparam name="TKey">The type of the entity identifier</typeparam>
        /// <param name="entity"></param>
        /// <param name="predicate"></param>
        /// <returns>number of records</returns>
        Task<int> UpdateAsync<TEntity>(TEntity entity, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : class, IEntity;

        Task<int> UpdateAsync<TEntity>(TEntity entity, IQuerySpecification<TEntity> querySpecification, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : class, IEntity;
    }
}
