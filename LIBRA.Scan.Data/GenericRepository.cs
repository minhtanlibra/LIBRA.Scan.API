using LIBRA.Scan.Data.Extensions;
using LIBRA.Scan.Data.Repositories.Constracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace LIBRA.Scan.Data
{
    public class GenericRepository<TEntity> : IEfRepository<TEntity> where TEntity : class
    {
        private readonly DbContext _context;
        private readonly DbSet<TEntity> _dbSet;

        public GenericRepository(DbContext context)
        {
            DbContext dbContext = context;
            _context = dbContext ?? throw new ArgumentNullException(nameof(context));
            _dbSet = _context.Set<TEntity>();
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity, bool ensureTransaction)
        {
            EntityEntry<TEntity> entityEntry = null;
            if (ensureTransaction)
            {
                using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
                {
                    _context.Entry(entity);
                    entityEntry = await _dbSet.AddAsync(entity, new CancellationToken());
                    try
                    {
                        await _context.SaveChangesAsync(new CancellationToken());
                        await transaction.CommitAsync();
                    }
                    catch (Exception e)
                    {
                        await transaction.RollbackAsync();
                        throw e;
                    }
                }
                return entityEntry.Entity;
            }
            else
            {
                _context.Entry(entity);
                entityEntry = await _dbSet.AddAsync(entity, new CancellationToken());
                try
                {
                    await _context.SaveChangesAsync(new CancellationToken());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            return entityEntry.Entity;
        }

        public async Task<bool> AddRangeAsync(IEnumerable<TEntity> entity)
        {
            bool result = true;
            using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (TEntity item in entity)
                    {
                        _context.Entry(item);
                        EntityEntry<TEntity> entityEntry = await _dbSet.AddAsync(item, new CancellationToken());
                        await _context.SaveChangesAsync(new CancellationToken());
                        if(entityEntry.Entity == null)
                        {
                            await transaction.RollbackAsync();
                            result = false;
                            break;
                        }
                    }
                    if(result)
                    {
                        await transaction.CommitAsync();
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    //throw ex;
                    return false;
                }
            }
        }

        public virtual async Task<TEntity> RemoveAsync(TEntity entity, bool ensureTransaction)
        {
            EntityEntry<TEntity> entityEntry = null;
            if (ensureTransaction)
            {
                using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
                {
                    _context.Entry(entity);
                    entityEntry = _dbSet.Remove(entity);
                    try
                    {
                        await _context.SaveChangesAsync(new CancellationToken());
                        await transaction.CommitAsync();
                    }
                    catch (Exception e)
                    {
                        await transaction.RollbackAsync();
                        throw e;
                    }
                }
                return entityEntry.Entity;
            }
            else
            {
                _context.Entry(entity);
                entityEntry = _dbSet.Remove(entity);
                try
                {
                    await _context.SaveChangesAsync(new CancellationToken());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
            return entityEntry.Entity;
        }

        public async Task<bool> RemoveRangeAsync(IEnumerable<TEntity> entity)
        {
            bool result = true;
            using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    foreach (TEntity item in entity)
                    {
                        _context.Entry(item);
                        EntityEntry<TEntity> entityEntry = _dbSet.Remove(item);
                        await _context.SaveChangesAsync(new CancellationToken());
                        if (entityEntry.Entity == null)
                        {
                            await transaction.RollbackAsync();
                            result = false;
                            break;
                        }
                    }
                    if (result)
                    {
                        await transaction.CommitAsync();
                    }
                    return result;
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    throw ex;
                }
            }
        }

        public virtual async Task<TEntity> DeleteAsync(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Detached;
            _context.Entry(entity).State = EntityState.Deleted;
            int num = await _context.SaveChangesAsync();
            return num > 0 ? entity : null;
        }

        public IQueryable<TEntity> GetWithInclude(Expression<Func<TEntity, bool>> predicate, params string[] include)
        {
            IQueryable<TEntity> query = _dbSet;
            query = include.Aggregate(query, (current, inc) => current.Include(inc));
            return query.Where(predicate);
        }

        public virtual Task<TEntity> Find(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeProperties = null)
        {
            IQueryable<TEntity> source = _dbSet;
            IEnumerable<TEntity> b = _context.Set<TEntity>();
            var c = b.FirstOrDefault();
            if (filter != null)
            {
                source = source.Where(filter);
            }

            if (includeProperties != null)
            {
                source = includeProperties(source);
            }
            return source.FirstOrDefaultAsync();
        }

        public async Task<bool> Any(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeProperties = null)
        {
            IQueryable<TEntity> source = _dbSet.AsNoTracking();
            if (includeProperties != null)
            {
                source = includeProperties(source);
            }

            if (filter != null)
            {
                return await source.AnyAsync(filter);
            }

            return await source.AnyAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> ListAsync(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy, Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeProperties)
        {
            IQueryable<TEntity> source = _dbSet.AsNoTracking();
            if (filter != null)
            {
                source = source.Where(filter);
            }

            if (includeProperties != null)
            {
                source = includeProperties(source);
            }

            if (orderBy != null)
            {
                return await orderBy(source).ToListAsync();
            }

            return await source.ToListAsync();
        }

        public virtual async Task<IReadOnlyList<TEntity>> ListAsync(Expression<Func<TEntity, bool>> filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        {
            IQueryable<TEntity> source = _dbSet.AsNoTracking();
            if (filter != null)
            {
                source = source.Where(filter);
            }

            if (orderBy != null)
            {
                return await orderBy(source).ToListAsync();
            }
            return await source.ToListAsync();
        }
                
        public virtual async Task<int> CountAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> source = _dbSet.AsNoTracking();
            if (filter != null)
            {
                source = source.Where(filter);
            } 
            return await source.CountAsync();
        }

        public virtual async Task<TEntity> UpdateAsync(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            //_context.Entry(entity).Property(x => x.Id).IsModified = false;
            int num = await _context.SaveChangesAsync();
            return num > 0 ? entity : null;
        }

        public virtual async Task<IEnumerable<TEntity>> UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            _context.UpdateRange(entities);
            int num = await _context.SaveChangesAsync();
            return num > 0 ? entities : null;
        } 
        public virtual async Task<TEntity> FindLast(Expression<Func<TEntity, bool>> filter,
             Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includeProperties = null)
        {
            IQueryable<TEntity> source = _dbSet.AsNoTracking();
            if (filter != null)
            {
                source = source.Where(filter);
            }
            if (includeProperties != null)
            {
                source = includeProperties(source);
            }
            if (orderBy != null)
            {
                return await orderBy(source).FirstOrDefaultAsync();
            }
            return await source.FirstOrDefaultAsync();
        }

        /// <summary>
        /// Creates a LINQ query for the query type based on a raw SQL query
        /// </summary>
        /// <typeparam name="TQuery">Query type</typeparam>
        /// <param name="sql">The raw SQL query</param>
        /// <param name="parameters">The values to be assigned to parameters</param>
        /// <returns>An IQueryable representing the raw SQL query</returns>
        public virtual IQueryable<TQuery> QueryFromSql<TQuery>(string sql, params object[] parameters) where TQuery : class
        {
            return _context.Set<TQuery>().FromSqlRaw(CreateSqlWithParameters(sql, parameters), parameters);
        }

        /// <summary>
        /// Modify the input SQL query by adding passed parameters
        /// </summary>
        /// <param name="sql">The raw SQL query</param>
        /// <param name="parameters">The values to be assigned to parameters</param>
        /// <returns>Modified raw SQL query</returns>
        public virtual string CreateSqlWithParameters(string sql, params object[] parameters)
        {
            //add parameters to sql
            for (int i = 0; i <= (parameters?.Length ?? 0) - 1; i++)
            {
                if (!(parameters[i] is DbParameter parameter))
                {
                    continue;
                }
                sql = $"{sql}{(i > 0 ? "," : string.Empty)} @{parameter.ParameterName}";
                //whether parameter is output
                if (parameter.Direction == ParameterDirection.InputOutput || parameter.Direction == ParameterDirection.Output)
                {
                    sql = $"{sql} output";
                }
            }
            return sql;
        }

        public virtual DataTable QueryByStoredProcedure(string storedProcedureName, params Microsoft.Data.SqlClient.SqlParameter[] parameters)
        {
            return _context.ExecuteDataTable(storedProcedureName, parameters);
        }

        /// <summary>
        /// Executes the given SQL against the database
        /// </summary>
        /// <param name="sql">The SQL to execute</param>
        /// <param name="doNotEnsureTransaction">true - the transaction creation is not ensured; false - the transaction creation is ensured.</param>
        /// <param name="timeout">The timeout to use for command. Note that the command timeout is distinct from the connection timeout, which is commonly set on the database connection string</param>
        /// <param name="parameters">Parameters to use with the SQL</param>
        /// <returns>The number of rows affected</returns>
        [Obsolete]
        public virtual int ExecuteSqlCommand(string sql, bool doNotEnsureTransaction = false, int? timeout = null, params object[] parameters)
        {
            //set specific command timeout
            int? previousTimeout = _context.Database.GetCommandTimeout();

            _context.Database.SetCommandTimeout(timeout);

            int result = 0;
            if (!doNotEnsureTransaction)
            {
                //use with transaction
                using (IDbContextTransaction transaction = _context.Database.BeginTransaction())
                {
                    result = _context.Database.ExecuteSqlRaw(sql, parameters);
                    transaction.Commit();
                }
            }
            else
            {
                result = _context.Database.ExecuteSqlRaw(sql, parameters);
            }

            //return previous timeout back
            _context.Database.SetCommandTimeout(previousTimeout);

            return result;
        }
    }
}