using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Softfluent.Asapp.Core.Context;
using System.Linq.Expressions;

namespace Softfluent.Asapp.Core.Data
{
    public class BaseRepository<Context> : IBaseRepository<Context> where Context : DbContext
    {
        protected readonly Context DbContext;

        public BaseRepository(Context dbContext, CallContext callContext)
        {
            this.DbContext = dbContext;
            this.CallContext = callContext;
        }

        protected CallContext CallContext { get; private set; }

        public virtual IDbContextTransaction BeginTransaction()
        {
            return this.DbContext.Database.BeginTransaction();
        }

        public virtual int BulkInsert<TEntity>(IEnumerable<TEntity> entities, bool truncateTable) where TEntity : class
        {
            if (truncateTable)
            {
                Microsoft.EntityFrameworkCore.Metadata.IEntityType? entityType = this.DbContext.Model.FindEntityType(typeof(TEntity));
                string? schemaName = entityType?.GetSchema();
                string? tableName = entityType?.GetTableName();
                this.DbContext.Database.ExecuteSqlRaw(string.Format(@"TRUNCATE TABLE ""{0}"".""{1}""", schemaName, tableName));
            }

            this.DbContext.Set<TEntity>().AddRange(entities);

            return this.DbContext.SaveChanges();
        }

        public virtual void Commit()
        {
            this.DbContext.Database.CommitTransaction();
        }

        public virtual Task CommitAsync(CancellationToken cancellationToken = default)
        {
            return this.DbContext.Database.CommitTransactionAsync(cancellationToken);
        }

        public int Count<TEntity>() where TEntity : class
        {
            return this.DbContext.Set<TEntity>().Count();
        }

        public int Count<TEntity>(IQuerySpecification<TEntity> querySpecification) where TEntity : class
        {
            return this.ApplyWhere(this.DbContext.Set<TEntity>(), querySpecification).Count();
        }

        public int Count<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return this.DbContext.Set<TEntity>().Where(predicate).Count();
        }

        public int Count<TEntity>(IQuerySpecification<TEntity> querySpecification, Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return this.ApplyWhere(this.DbContext.Set<TEntity>(), querySpecification)
                .Where(predicate)
                .Count();
        }

        public async Task<int> CountAsync<TEntity>(CancellationToken cancellationToken = default) where TEntity : class
        {
            return await this.DbContext.Set<TEntity>().CountAsync(cancellationToken);
        }

        public async Task<int> CountAsync<TEntity>(IQuerySpecification<TEntity> querySpecification
                                                    , CancellationToken cancellationToken = default) where TEntity : class
        {
            return await this.ApplyWhere(this.DbContext.Set<TEntity>(), querySpecification).CountAsync(cancellationToken);
        }

        public async Task<int> CountAsync<TEntity>(Expression<Func<TEntity, bool>> predicate
                                                    , CancellationToken cancellationToken = default) where TEntity : class
        {
            return await this.DbContext.Set<TEntity>().Where(predicate).CountAsync(cancellationToken);
        }

        public async Task<int> CountAsync<TEntity>(IQuerySpecification<TEntity> querySpecification, Expression<Func<TEntity, bool>> predicate
                                                    , CancellationToken cancellationToken = default) where TEntity : class
        {
            return await this.ApplyWhere(this.DbContext.Set<TEntity>(), querySpecification)
                .Where(predicate)
                .CountAsync(cancellationToken);
        }

        public virtual int Create<TEntity>(TEntity model) where TEntity : class, IEntity
        {
            EntityEntry<TEntity> entityEntry = this.DbContext.Set<TEntity>().Add(model);
            int result = this.SaveChanges();
            return result;
        }

        public virtual int Create<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity
        {
            this.DbContext.Set<TEntity>().AddRange(entities);
            return this.SaveChanges();
        }

        public virtual async Task<int> CreateAsync<TEntity>(TEntity model, CancellationToken cancellationToken = default) where TEntity : class, IEntity
        {
            EntityEntry<TEntity> entityEntry = await this.DbContext.Set<TEntity>().AddAsync(model, cancellationToken);
            int result = await this.SaveChangesAsync(cancellationToken);
            return result;
        }

        public virtual async Task<int> CreateAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class, IEntity
        {
            await this.DbContext.Set<TEntity>().AddRangeAsync(entities, cancellationToken);
            return await this.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task<int> DeleteAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default) where TEntity : class, IEntity
        {
            DbSet<TEntity> dbSet = this.DbContext.Set<TEntity>();
            TEntity? item = await dbSet.FindAsync(key, cancellationToken);
            if (item != null)
            {
                EntityEntry<TEntity> entityEntry = dbSet.Remove(item);
            }
            int count = await this.SaveChangesAsync(cancellationToken);
            return count;
        }

        public virtual async Task<int> DeleteAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class, IEntity
        {
            foreach (TEntity entity in entities)
            {
                EntityEntry<TEntity> entry = this.DbContext.Entry(entity);
                if (entry == null)
                {
                    continue;
                }
                entry.State = EntityState.Deleted;
            }

            int count = await this.SaveChangesAsync(cancellationToken);
            return count;
        }

        public virtual async Task<int> DeleteAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : class, IEntity
        {
            IQueryable<TEntity> entities = this.DbContext.Set<TEntity>().Where(predicate);
            this.DbContext.Set<TEntity>().RemoveRange(entities);
            int count = await this.SaveChangesAsync(cancellationToken);
            return count;
        }

        public virtual int ExecuteStoredProcedure(string storedProcedureFormat, params object[] args)
        {
            string callFormat = string.Format("CALL {0}", storedProcedureFormat);
            return this.DbContext.Database.ExecuteSqlRaw(callFormat, args ?? new object[0]);
        }

        public virtual Task<int> ExecuteStoredProcedureAsync(string storedProcedureFormat, CancellationToken cancellationToken, params object[] args)
        {
            string callFormat = string.Format("CALL {0}", storedProcedureFormat);
            return this.DbContext.Database.ExecuteSqlRawAsync(callFormat, args ?? new object[0], cancellationToken);
        }

        public virtual bool Exists<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return this.DbContext.Set<TEntity>().Any(predicate);
        }

        public virtual Task<bool> ExistsAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : class
        {
            return this.DbContext.Set<TEntity>().AnyAsync(predicate, cancellationToken);
        }

        public virtual TEntity? Get<TEntity, TKey>(TKey key) where TEntity : Entity<TKey>
                                                                where TKey : notnull
        {
            return this.DbContext.Set<TEntity>().FirstOrDefault(x => x.Identifier.Equals(key));
        }

        public virtual TEntity? Get<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return this.DbContext.Set<TEntity>().FirstOrDefault(predicate);
        }

        public virtual TEntity? Get<TEntity>(IQuerySpecification<TEntity> querySpecification) where TEntity : class
        {
            return this.ApplyQuerySpecification(this.DbContext.Set<TEntity>(), querySpecification).FirstOrDefault();
        }

        public virtual Task<TEntity?> GetAsync<TEntity, TKey>(TKey key, CancellationToken cancellationToken = default) where TEntity : Entity<TKey>
                                                                        where TKey : notnull
        {
            return this.DbContext.Set<TEntity>().FirstOrDefaultAsync(x => x.Identifier.Equals(key), cancellationToken);
        }

        public virtual Task<TEntity?> GetAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : class
        {
            return this.DbContext.Set<TEntity>().FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public virtual Task<TEntity?> GetAsync<TEntity>(IQuerySpecification<TEntity> querySpecification, CancellationToken cancellationToken = default) where TEntity : class
        {
            return this.ApplyQuerySpecification(this.DbContext.Set<TEntity>(), querySpecification).FirstOrDefaultAsync(cancellationToken);
        }

        public virtual List<TResult> GetDistinctValues<TEntity, TResult>(IQuerySpecification<TEntity> querySpecification, Expression<Func<TEntity, TResult>> predicate) where TEntity : class
        {
            IQueryable<TResult> query = this.ApplyWhere(this.DbContext.Set<TEntity>().AsQueryable(), querySpecification)
                                        .Select(predicate).Distinct();
            return query.ToList();
        }

        public virtual Task<List<TResult>> GetDistinctValuesAsync<TEntity, TResult>(IQuerySpecification<TEntity> querySpecification, Expression<Func<TEntity, TResult>> predicate, CancellationToken cancellationToken = default) where TEntity : class
        {
            IQueryable<TResult> query = this.ApplyWhere(this.DbContext.Set<TEntity>().AsQueryable(), querySpecification)
                                        .Select(predicate).Distinct();
            return query.ToListAsync(cancellationToken);
        }

        public virtual IEnumerable<TEntity> GetEnumerable<TEntity>(IQuerySpecification<TEntity> querySpecification, Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            IQueryable<TEntity> query = this.DbContext.Set<TEntity>().Where(predicate);
            return this.ApplyQuerySpecification(query, querySpecification).AsEnumerable();
        }

        public virtual IEnumerable<TEntity> GetEnumerable<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return this.DbContext.Set<TEntity>().Where(predicate).AsEnumerable();
        }

        public virtual IEnumerable<TEntity> GetEnumerable<TEntity>(IQuerySpecification<TEntity> querySpecification) where TEntity : class
        {
            return this.ApplyQuerySpecification(this.DbContext.Set<TEntity>().AsQueryable(), querySpecification).AsEnumerable();
        }

        public virtual GridPage<TEntity> GetGridPage<TEntity>(GridCriteria gridCriteria) where TEntity : class
        {
            Tuple<IQueryable<TEntity>, GridPage<TEntity>> queryAndPage = GetQueryAndGridPage<TEntity>(gridCriteria);
            queryAndPage.Item2.Items = queryAndPage.Item1.ToList();
            return queryAndPage.Item2;
        }

        public virtual async Task<GridPage<TEntity>> GetGridPageAsync<TEntity>(GridCriteria gridCriteria, CancellationToken cancellationToken = default) where TEntity : class
        {
            Tuple<IQueryable<TEntity>, GridPage<TEntity>> queryAndPage = GetQueryAndGridPage<TEntity>(gridCriteria);
            queryAndPage.Item2.Items = await queryAndPage.Item1.ToListAsync(cancellationToken);
            return queryAndPage.Item2;
        }

        public TOutput Max<TEntity, TOutput>(Expression<Func<TEntity, TOutput>> predicate, TOutput defaultValue) where TEntity : class
        {
            if (this.DbContext.Set<TEntity>().Count() == 0)
            {
                return defaultValue;
            }
            TOutput? maxVal = this.DbContext.Set<TEntity>().Max(predicate);
            if (maxVal == null)
                return defaultValue;
            return maxVal;
        }

        public async Task<TOutput> MaxAsync<TEntity, TOutput>(Expression<Func<TEntity, TOutput>> predicate, TOutput defaultValue, CancellationToken cancellationToken = default) where TEntity : class
        {
            if (await this.DbContext.Set<TEntity>().CountAsync(cancellationToken) == 0)
            {
                return defaultValue;
            }
            return await this.DbContext.Set<TEntity>().MaxAsync(predicate, cancellationToken);
        }

        public virtual void Rollback()
        {
            this.DbContext.Database.RollbackTransaction();
        }

        public virtual async Task RollbackAsync()
        {
            await this.DbContext.Database.RollbackTransactionAsync();
        }

        public virtual int Update<TEntity>(TEntity entity) where TEntity : class, IEntity
        {
            this.DbContext.Set<TEntity>().Attach(entity);
            this.DbContext.Entry(entity).State = EntityState.Modified;

            return this.SaveChanges();
        }

        public virtual int Update<TEntity>(IEnumerable<TEntity> entities) where TEntity : class, IEntity
        {
            this.DbContext.Set<TEntity>().AttachRange(entities);
            this.DbContext.UpdateRange(entities);
            int result = this.SaveChanges();
            return result;
        }

        public virtual int Update<TEntity>(TEntity entity, IQuerySpecification<TEntity> querySpecification) where TEntity : class, IEntity
        {
            TEntity? dbEntity = this.GetQueryable(querySpecification).FirstOrDefault();
            int result = 0;

            if (dbEntity != null)
            {
                this.DbContext.Entry(dbEntity).CurrentValues.SetValues(entity);
                result = this.SaveChanges();
            }

            return result;
        }

        /// <summary>
        /// Update single row with predicate
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="entity"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual int Update<TEntity>(TEntity entity, Expression<Func<TEntity, bool>> predicate) where TEntity : class, IEntity
        {
            TEntity? dbEntity = this.DbContext.Set<TEntity>().FirstOrDefault(predicate);
            int result = 0;

            if (dbEntity != null)
            {
                this.DbContext.Entry(dbEntity).CurrentValues.SetValues(entity);
                result = this.SaveChanges();
            }

            return result;
        }

        public virtual int Update<TEntity>(TEntity entity, IQuerySpecification<TEntity> querySpecification, Expression<Func<TEntity, bool>> predicate) where TEntity : class, IEntity
        {
            TEntity? dbEntity = this.ApplyWhere(this.DbContext.Set<TEntity>(), querySpecification)
                                    .Where(predicate)
                                    .FirstOrDefault();
            int result = 0;

            if (dbEntity != null)
            {
                this.DbContext.Entry(dbEntity).CurrentValues.SetValues(entity);
                result = this.SaveChanges();
            }

            return result;
        }

        public virtual async Task<int> UpdateAsync<TEntity>(TEntity entity, CancellationToken cancellationToken = default) where TEntity : class, IEntity
        {
            this.DbContext.Set<TEntity>().Attach(entity);
            this.DbContext.Entry(entity).State = EntityState.Modified;

            return await this.SaveChangesAsync(cancellationToken);
        }

        public virtual async Task<int> UpdateAsync<TEntity>(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default) where TEntity : class, IEntity
        {
            this.DbContext.Set<TEntity>().AttachRange(entities);
            this.DbContext.UpdateRange(entities);
            int result = await this.SaveChangesAsync(cancellationToken);
            return result;
        }

        public virtual async Task<int> UpdateAsync<TEntity>(TEntity entity, IQuerySpecification<TEntity> querySpecification, CancellationToken cancellationToken = default) where TEntity : class, IEntity
        {
            TEntity? dbEntity = await this.GetQueryable(querySpecification).FirstOrDefaultAsync(cancellationToken);
            int result = 0;

            if (dbEntity != null)
            {
                this.DbContext.Entry(dbEntity).CurrentValues.SetValues(entity);
                result = await this.SaveChangesAsync(cancellationToken);
            }

            return result;
        }

        /// <summary>
        /// Update single row with predicate
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="entity"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual async Task<int> UpdateAsync<TEntity>(TEntity entity, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : class, IEntity
        {
            TEntity? dbEntity = await this.DbContext.Set<TEntity>().FirstOrDefaultAsync(predicate, cancellationToken);
            int result = 0;

            if (dbEntity != null)
            {
                this.DbContext.Entry(dbEntity).CurrentValues.SetValues(entity);
                result = await this.SaveChangesAsync(cancellationToken);
            }

            return result;
        }

        public virtual async Task<int> UpdateAsync<TEntity>(TEntity entity, IQuerySpecification<TEntity> querySpecification, Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TEntity : class, IEntity
        {
            TEntity? dbEntity = await this.ApplyWhere(this.DbContext.Set<TEntity>(), querySpecification)
                                    .Where(predicate)
                                    .FirstOrDefaultAsync(cancellationToken);
            int result = 0;

            if (dbEntity != null)
            {
                this.DbContext.Entry(dbEntity).CurrentValues.SetValues(entity);
                result = await this.SaveChangesAsync(cancellationToken);
            }

            return result;
        }

        protected virtual IQueryable<TEntity> ApplyQuerySpecification<TEntity>(IQueryable<TEntity> inputQuery, IQuerySpecification<TEntity> querySpecification) where TEntity : class
        {
            IQueryable<TEntity> query = this.ApplyWhere(inputQuery, querySpecification);

            // Apply ordering if expressions are set
            bool first = true;
            foreach (Tuple<SortDirection, Expression<Func<TEntity, object>>> order in querySpecification.OrderBy)
            {
                if (first)
                {
                    if (order.Item1 == SortDirection.Descending)
                    {
                        query = query.OrderByDescending(order.Item2);
                    }
                    else
                    {
                        query = query.OrderBy(order.Item2);
                    }
                    first = false;
                }
                else
                {
                    if (order.Item1 == SortDirection.Descending)
                    {
                        query = ((IOrderedQueryable<TEntity>)query).ThenByDescending(order.Item2);
                    }
                    else
                    {
                        query = ((IOrderedQueryable<TEntity>)query).ThenBy(order.Item2);
                    }
                }
            }

            // Apply paging if enabled
            if (querySpecification.IsPagingEnabled)
            {
                query = query.Skip(querySpecification.Skip)
                             .Take(querySpecification.Take);
            }

            // Apply includes
            if (querySpecification.Includes.Any())
            {
                // Includes all expression-based includes
                query = querySpecification.Includes.Aggregate(query, (current, include) => current.Include(include));
            }

            if (querySpecification.IncludeStrings.Any())
            {
                // Include any string-based include statements
                query = querySpecification.IncludeStrings.Aggregate(query, (current, include) => current.Include(include));
            }

            return query;
        }

        /// <summary>
        /// This fonction will be used to return the effective number of elements in database
        /// corresponding to the filter
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="inputQuery"></param>
        /// <param name="querySpecification"></param>
        /// <returns></returns>
        protected virtual IQueryable<TEntity> ApplyWhere<TEntity>(IQueryable<TEntity> inputQuery, IQuerySpecification<TEntity> querySpecification) where TEntity : class
        {
            IQueryable<TEntity> query = inputQuery;

            // Apply filter
            if (querySpecification.Criteria != null)
            {
                query = query.Where(querySpecification.Criteria);
            }
            return query;
        }

        protected virtual IQueryable<TEntity> GetQueryable<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return this.DbContext.Set<TEntity>().Where(predicate);
        }

        protected IQueryable<TEntity> GetQueryable<TEntity>(Expression<Func<TEntity, bool>> predicate, IQuerySpecification<TEntity> querySpecification) where TEntity : class
        {
            IQueryable<TEntity> query = this.DbContext.Set<TEntity>().Where(predicate);
            return this.ApplyQuerySpecification(query, querySpecification);
        }

        protected virtual IQueryable<TEntity> GetQueryable<TEntity>(IQuerySpecification<TEntity> querySpecification) where TEntity : class
        {
            return this.ApplyQuerySpecification(this.DbContext.Set<TEntity>().AsQueryable(), querySpecification);
        }

        protected IQueryable<TEntity> GetQueryable<TEntity>() where TEntity : class
        {
            return this.DbContext.Set<TEntity>().AsQueryable();
        }

        private Tuple<IQueryable<TEntity>, GridPage<TEntity>> GetQueryAndGridPage<TEntity>(GridCriteria gridCriteria) where TEntity : class
        {
            GridPage<TEntity> result = new();

            IQuerySpecification<TEntity> querySpecification = new BaseQuerySpecification<TEntity>();
            querySpecification.ApplyCriteria(gridCriteria);

            result.TotalCount = this.Count(querySpecification); // Count does not take into account paging
            result.Page = gridCriteria.Page ?? 0;
            result.PageSize = gridCriteria.PageSize ?? 0;

            IQueryable<TEntity> entities = this.GetQueryable(querySpecification);
            return new Tuple<IQueryable<TEntity>, GridPage<TEntity>>(entities, result);
        }

        private int SaveChanges()
        {
            this.UpdateEntitiesEntry();
            return this.DbContext.SaveChanges();
        }

        private async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            this.UpdateEntitiesEntry();
            return await this.DbContext.SaveChangesAsync(cancellationToken);
        }

        private void UpdateEntitiesEntry()
        {
            IEnumerable<EntityEntry> entries = this.DbContext.ChangeTracker.Entries().Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (EntityEntry entityEntry in entries)
            {
                if (entityEntry.Entity is IEntity entity)
                {
                    entity.TrackLastWriteTime = DateTime.UtcNow;
                    entity.TrackLastWriteUser = CallContext.ExecutionIdentity;

                    if (entityEntry.State == EntityState.Modified)
                    {
                        this.DbContext.Entry(entity).Property(x => x.TrackCreationUser).IsModified = false;
                        this.DbContext.Entry(entity).Property(x => x.TrackCreationTime).IsModified = false;
                    }
                    else if (entityEntry.State == EntityState.Added)
                    {
                        entity.TrackCreationUser = CallContext.ExecutionIdentity;
                        entity.TrackCreationTime = DateTime.UtcNow;
                    }
                }
            }
        }
    }
}
