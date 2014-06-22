using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client;

namespace PersistentLayer.Raven.Impl
{
    public class SessionCacheProvider
        : SessionContextProvider
    {
        private readonly bool newSessionAfterCommit;
        private readonly bool newSessionAfterRollback;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionSupplier"></param>
        /// <param name="newSessionAfterCommit"></param>
        /// <param name="newSessionAfterRollback"></param>
        public SessionCacheProvider(Func<IDocumentSession> sessionSupplier, bool newSessionAfterCommit, bool newSessionAfterRollback)
            : this(sessionSupplier, DefaultContext, newSessionAfterCommit, newSessionAfterRollback)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionSupplier"></param>
        /// <param name="keyContext"></param>
        /// <param name="newSessionAfterCommit"></param>
        /// <param name="newSessionAfterRollback"></param>
        public SessionCacheProvider(Func<IDocumentSession> sessionSupplier, object keyContext, bool newSessionAfterCommit, bool newSessionAfterRollback)
            : base(sessionSupplier, keyContext)
        {
            this.newSessionAfterCommit = newSessionAfterCommit;
            this.newSessionAfterRollback = newSessionAfterRollback;
        }

        /// <summary>
        /// 
        /// </summary>
        public override void CommitTransaction()
        {
            base.CommitTransaction();
            if (this.newSessionAfterCommit && !this.InProgress)
                this.Reset();
        }

        /// <summary>
        /// 
        /// </summary>
        public override void RollbackTransaction()
        {
            this.RollbackTransaction(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cause"></param>
        public override void RollbackTransaction(Exception cause)
        {
            base.RollbackTransaction(cause);
            if (this.newSessionAfterRollback)
                this.Reset();
        }
    }
}
