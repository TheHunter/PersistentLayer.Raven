using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;

namespace PersistentLayer.Raven
{
    /// <summary>
    /// 
    /// </summary>
    public interface ISessionContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IDocumentSession GetCurrentSession();

        /// <summary>
        /// 
        /// </summary>
        bool HasSessionBinded { get; }
    }
}
