using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentLayer.Raven
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRavenPersisterDAO<in TRootEntity>
        : IRootPersisterDAO<TRootEntity>
        where TRootEntity : class
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="useIdentity"></param>
        /// <returns></returns>
        dynamic MakePersistent(dynamic entity, bool useIdentity);

        /// <summary>
        /// /
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="etag"></param>
        /// <returns></returns>
        TEntity MakePersistent<TEntity>(TEntity entity, RavenEtag etag)
            where TEntity : class, TRootEntity;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="identifier"></param>
        /// <param name="etag"></param>
        /// <returns></returns>
        TEntity MakePersistent<TEntity>(TEntity entity, object identifier, RavenEtag etag)
            where TEntity : class, TRootEntity;

        
        //TEntity MakePersistentUsingIdentity<TEntity>(TEntity entity)
        //    where TEntity : class, TRootEntity;
    }
}
