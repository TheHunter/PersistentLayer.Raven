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
    public interface ISessionBinder
        : ISessionContext
    {
        /// <summary>
        /// 
        /// </summary>
        void Bind(IDocumentSession session);

        /// <summary>
        /// 
        /// </summary>
        IDocumentSession UnBind();

    }
}
