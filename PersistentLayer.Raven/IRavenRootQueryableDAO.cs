using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Raven.Client.Linq;

namespace PersistentLayer.Raven
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRavenRootQueryableDAO<in TRootEntity>
        : IRootQueryableDAO<TRootEntity>
        where TRootEntity : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="indexParameter"></param>
        /// <returns></returns>
        IEnumerable<TEntity> FindAll<TEntity>(IndexParameter indexParameter)
            where TEntity : class, TRootEntity;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="indexParameter"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<TEntity> FindAll<TEntity>(IndexParameter indexParameter, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class, TRootEntity;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="identifiers"></param>
        /// <returns></returns>
        IEnumerable<TEntity> FindAll<TEntity>(IEnumerable<object> identifiers)
            where TEntity : class, TRootEntity;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="indexParameter"></param>
        /// <param name="queryExpr"></param>
        /// <returns></returns>
        TResult ExecuteExpression<TEntity, TResult>(IndexParameter indexParameter, Expression<Func<IQueryable<TEntity>, TResult>> queryExpr)
            where TEntity : class, TRootEntity;
    }
}
