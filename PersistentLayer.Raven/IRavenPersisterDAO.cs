using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Raven.Abstractions.Data;

namespace PersistentLayer.Raven
{
    public interface IRavenPersisterDAO
        : IPersisterDAO
    {
        
        //TEntity MakePersistent<TEntity>(TEntity entity, string id)
        //    where TEntity : class;


        TEntity MakePersistent<TEntity>(TEntity entity, RavenEtag etag)
            where TEntity : class;


        TEntity MakePersistent<TEntity, TKey>(TEntity entity, TKey identifier, RavenEtag etag)
            where TEntity : class;
    }
}
