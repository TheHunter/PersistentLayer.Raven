using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentLayer.Raven
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRavenPersisterDAO
        : IPersisterDAO
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        dynamic MakePersistent(dynamic entity);

        /// <summary>
        /// /
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="etag"></param>
        /// <returns></returns>
        TEntity MakePersistent<TEntity>(TEntity entity, RavenEtag etag)
            where TEntity : class;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="entity"></param>
        /// <param name="identifier"></param>
        /// <param name="etag"></param>
        /// <returns></returns>
        TEntity MakePersistent<TEntity, TKey>(TEntity entity, TKey identifier, RavenEtag etag)
            where TEntity : class;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntity MakePersistentUsingIdentity<TEntity>(TEntity entity)
            where TEntity : class;
    }
}
