﻿using System.Linq.Expressions;
using RoadOfGroping.Repository.Entities;

namespace RoadOfGroping.Repository.Repository
{
    public interface IAnotherBaseRepository<TEntity, TPrimaryKey> where TEntity : class, IEntity<TPrimaryKey>
    {
        IQueryable<TEntity> GetAll();

        IQueryable<TEntity> GetAllIncluding(params Expression<Func<TEntity, object>>[] propertySelectors);

        List<TEntity> GetAllList();

        Task<List<TEntity>> GetAllListAsync();

        List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate);

        Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate);

        T Query<T>(Func<IQueryable<TEntity>, T> queryMethod);

        TEntity Get(TPrimaryKey id);

        Task<TEntity> GetAsync(TPrimaryKey id);

        TEntity Single(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> SingleAsync(Expression<Func<TEntity, bool>> predicate);

        TEntity FirstOrDefault(TPrimaryKey id);

        Task<TEntity> FirstOrDefaultAsync(TPrimaryKey id);

        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);

        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        TEntity Load(TPrimaryKey id);

        TEntity Insert(TEntity entity);

        Task<TEntity> InsertAsync(TEntity entity);

        TPrimaryKey InsertAndGetId(TEntity entity);

        Task<TPrimaryKey> InsertAndGetIdAsync(TEntity entity);

        TEntity Update(TEntity entity);

        Task<TEntity> UpdateAsync(TEntity entity);

        TEntity Update(TPrimaryKey id, Action<TEntity> updateAction);

        Task<TEntity> UpdateAsync(TPrimaryKey id, Func<TEntity, Task> updateAction);

        void Delete(TEntity entity);

        Task DeleteAsync(TEntity entity);

        void Delete(TPrimaryKey id);

        Task DeleteAsync(TPrimaryKey id);

        void Delete(Expression<Func<TEntity, bool>> predicate);

        Task DeleteAsync(Expression<Func<TEntity, bool>> predicate);

        int Count();

        Task<int> CountAsync();

        int Count(Expression<Func<TEntity, bool>> predicate);

        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);

        long LongCount();

        Task<long> LongCountAsync();

        long LongCount(Expression<Func<TEntity, bool>> predicate);

        Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate);
    }
}