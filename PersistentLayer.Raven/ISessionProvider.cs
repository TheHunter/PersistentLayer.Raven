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
    public interface ISessionProvider
        : IDisposable
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IDocumentSession GetCurrentSession();
    }
}
