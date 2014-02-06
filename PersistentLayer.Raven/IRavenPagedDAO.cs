using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentLayer.Raven
{
    /// <summary>
    /// 
    /// </summary>
    public interface IRavenPagedDAO
        : IPagedDAO, IRavenPersisterDAO, IRavenQueryableDAO
    {
        
    }
}
