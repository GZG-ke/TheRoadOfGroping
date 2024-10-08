﻿using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using RoadOfGroping.Common.Pager;

namespace RoadOfGroping.Common.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> PageBy<T>(
            this IQueryable<T> query,
            int skipCount,
            int maxResultCount
        )
        {
            return query.Skip(skipCount).Take(maxResultCount);
        }

        public static TQueryable PageBy<T, TQueryable>(
            this TQueryable query,
            int skipCount,
            int maxResultCount
        )
            where TQueryable : IQueryable<T>
        {
            return (TQueryable)query.Skip(skipCount).Take(maxResultCount);
        }

        public static IQueryable<T> WhereIf<T>(
            this IQueryable<T> query,
            bool condition,
            Expression<Func<T, bool>> predicate
        )
        {
            return !condition ? query : query.Where(predicate);
        }

        public static TQueryable WhereIf<T, TQueryable>(
            this TQueryable query,
            bool condition,
            Expression<Func<T, bool>> predicate
        )
            where TQueryable : IQueryable<T>
        {
            return !condition ? query : (TQueryable)query.Where<T>(predicate);
        }

        public static IQueryable<T> WhereIf<T>(
            this IQueryable<T> query,
            bool condition,
            Expression<Func<T, int, bool>> predicate
        )
        {
            return !condition ? query : query.Where(predicate);
        }

        public static TQueryable WhereIf<T, TQueryable>(
            this TQueryable query,
            bool condition,
            Expression<Func<T, int, bool>> predicate
        )
            where TQueryable : IQueryable<T>
        {
            return !condition ? query : (TQueryable)query.Where<T>(predicate);
        }

        public static IQueryable<T> Count<T>(this IQueryable<T> queryable, out long count)
        {
            count = queryable.Count();
            return queryable;
        }

        public static IQueryable<T> Page<T>(this IQueryable<T> queryable, int pageNumber, int pageSize)
        {
            queryable = queryable.Skip(Math.Max(0, pageNumber - 1) * pageSize).Take(pageSize);
            return queryable;
        }

        /// <summary>
        /// 分页拓展
        /// </summary>
        /// <param name="queryable"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public static async Task<PageResult<T>> ToPagedListAsync<T>(
            this IQueryable<T> queryable,
            IPagination input
        )
        {
            var items = await queryable
                .Skip((input.PageNo - 1) * input.PageSize)
                .Take(input.PageSize)
                .ToListAsync();
            var totalCount = await queryable.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)input.PageSize);
            return new PageResult<T>()
            {
                PageNo = input.PageNo,
                PageSize = input.PageSize,
                Rows = items,
                Total = (int)totalCount,
                Pages = totalPages,
            };
        }
    }
}