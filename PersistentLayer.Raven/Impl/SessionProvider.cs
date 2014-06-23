using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Transactions;
using PersistentLayer.Exceptions;
using PersistentLayer.Impl;
using Raven.Client;
using IsolationLevel = System.Data.IsolationLevel;

namespace PersistentLayer.Raven.Impl
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]    
    public abstract class SessionProvider
        : ISessionProvider
    {
        private readonly Stack<ITransactionInfo> transactions;
        private TransactionScope transactionScope;
        private const string DefaultNaming = "anonymous";

        /// <summary>
        /// 
        /// </summary>
        protected SessionProvider()
        {
            transactions = new Stack<ITransactionInfo>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public abstract IDocumentSession GetCurrentSession();

        /// <summary>
        /// 
        /// </summary>
        public bool InProgress
        {
            get { return this.transactions.Count > 0; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool Exists(string name)
        {
            if (name == null)
                return false;

            return this.transactions.Count(info => info.Name == name) > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void BeginTransaction()
        {
            this.BeginTransaction((IsolationLevel?)null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public virtual void BeginTransaction(string name)
        {
            this.BeginTransaction(name, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        public virtual void BeginTransaction(IsolationLevel? level)
        {
            int index = transactions.Count;
            this.BeginTransaction(string.Format("{0}_{1}", DefaultNaming, index));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="level"></param>
        public virtual void BeginTransaction(string name, IsolationLevel? level)
        {
            if (name == null || name.Trim().Equals(string.Empty))
                throw new BusinessLayerException("The transaction name cannot be null or empty", "BeginTransaction");

            if (this.Exists(name))
                throw new BusinessLayerException(string.Format("The transaction name ({0}) to add is used by another one..", name), "BeginTransaction");

            int index = transactions.Count;

            if (this.transactions.Count == 0)
            {
                //if (!sessionProvider.HasSessionBinded)
                //    throw new BusinessLayerException("Error on beginning a new transaction because of missing binded document session.", "BeginTransaction");

                this.transactionScope = new TransactionScope();
            }
            this.transactions.Push(new TransactionInfo(name, index));
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void CommitTransaction()
        {
            if (transactions.Count > 0)
            {
                ITransactionInfo info = transactions.Pop();
                if (transactions.Count == 0)
                {
                    //if (!sessionProvider.HasSessionBinded)
                    //    throw new BusinessLayerException("Error on Commiting the current transaction because of missing binded document session.", "CommitTransaction");

                    try
                    {
                        IDocumentSession session = this.GetCurrentSession();

                        session.SaveChanges();
                        this.transactionScope.Complete();
                    }
                    catch (Exception ex)
                    {
                        throw new CommitFailedException(
                            string.Format(
                                "Error when the current session tries to commit the current transaction (name: {0}).",
                                info.Name), "CommitTransaction", ex);
                    }
                    finally
                    {
                        this.Reset();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void RollbackTransaction()
        {
            this.RollbackTransaction(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cause"></param>
        public virtual void RollbackTransaction(Exception cause)
        {
            if (this.transactions.Count > 0)
            {
                ITransactionInfo info = transactions.Pop();
                int innerTransactions = transactions.Count;

                try
                {
                    if (innerTransactions > 0)
                        throw new InnerRollBackException("An inner rollback transaction has occurred.", cause, info);
                }
                finally
                {
                    this.Reset();
                }
            }
        }

        /// <summary>
        /// Clear all internal transactions.
        /// </summary>
        protected virtual void Reset()
        {
            if (transactionScope == null)
                return;

            this.transactionScope.Dispose();
            this.transactions.Clear();
            this.transactionScope = null;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual void Dispose()
        {
            try
            {
                if (this.InProgress)
                    this.RollbackTransaction();
            }
            catch
            {
            }
        }
    }
}
