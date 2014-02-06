using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PersistentLayer.Exceptions;
using Raven.Client;

namespace PersistentLayer.Raven.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public class SessionBinder
        : ISessionBinder
    {
        private IDocumentSession currentSession;

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="SessionNotBindedException"></exception>
        /// <returns></returns>
        public IDocumentSession GetCurrentSession()
        {
            if (this.currentSession == null)
                throw new SessionNotBindedException("No IDocumentSession has binded.", "GetCurrentSession");

            return this.currentSession;
        }

        /// <summary>
        /// 
        /// </summary>
        public bool HasSessionBinded
        {
            get { return currentSession != null; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <exception cref="BusinessLayerException"></exception>
        public void Bind(IDocumentSession session)
        {
            if (session == null)
                throw new BusinessLayerException("IDocumentSession instance to bind cannot be null", "Bind");

            this.currentSession = session;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IDocumentSession UnBind()
        {
            return this.currentSession;
        }

    }
}
