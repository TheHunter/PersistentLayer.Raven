using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentLayer.Raven
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRavenRootPagedDAO<in TRootEntity>
        : IRootPagedDAO<TRootEntity>, IRavenPersisterDAO<TRootEntity>, IRavenRootQueryableDAO<TRootEntity>
        where TRootEntity : class
    {
        
    }
}
