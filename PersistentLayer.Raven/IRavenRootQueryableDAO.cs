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
        /// <param name="indexName"></param>
        /// <param name="isMapReduce"></param>
        /// <returns></returns>
        IEnumerable<TEntity> FindAll<TEntity>(string indexName, bool isMapReduce)
            where TEntity : class, TRootEntity;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="isMapReduce"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<TEntity> FindAll<TEntity>(string indexName, bool isMapReduce, Expression<Func<TEntity, bool>> predicate)
            where TEntity : class, TRootEntity;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="identifiers"></param>
        /// <returns></returns>
        IEnumerable<TEntity> FindAll<TEntity>(IEnumerable<object> identifiers)
            where TEntity : class, TRootEntity;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="identifiers"></param>
        /// <returns></returns>
        IEnumerable<TEntity> FindAll<TEntity>(params object[] identifiers)
            where TEntity : class, TRootEntity;
    }
}
