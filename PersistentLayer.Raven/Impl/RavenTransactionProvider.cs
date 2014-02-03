﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.Text;
using System.Transactions;
using PersistentLayer.Exceptions;
using PersistentLayer.Impl;
using Raven.Client;

namespace PersistentLayer.Raven.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public class RavenTransactionProvider
        : IRavenTransactionProvider
    {
        private readonly Stack<ITransactionInfo> transactions;
        private readonly ISessionContext sessionContext;
        private TransactionScope transactionScope;
        private const string DefaultNaming = "anonymous";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionContext"></param>
        public RavenTransactionProvider(ISessionContext sessionContext)
        {
            if (sessionContext == null)
                throw new BusinessLayerException("The IDocumentStore instance cannot be null.", "ctr RavenTransactionProvider");

            this.sessionContext = sessionContext;
            this.transactions =new Stack<ITransactionInfo>();
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
        public void BeginTransaction()
        {
            int index = transactions.Count;
            this.BeginTransaction(string.Format("{0}_{1}", DefaultNaming, index));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void BeginTransaction(string name)
        {
            if (name == null || name.Trim().Equals(string.Empty))
                throw new BusinessLayerException("The transaction name cannot be null or empty", "BeginTransaction");

            if (this.Exists(name))
                throw new BusinessLayerException(string.Format("The transaction name ({0}) to add is used by another one..", name), "BeginTransaction");

            int index = transactions.Count;

            if (this.transactions.Count == 0)
            {
                if (!sessionContext.HasSessionBinded)
                    throw new BusinessLayerException("Error on beginning a new transaction because of missing binded document session.", "BeginTransaction");

                this.transactionScope = new TransactionScope();
            }
            this.transactions.Push(new TransactionInfo(name, index));
        }

        /// <summary>
        /// 
        /// </summary>
        public void CommitTransaction()
        {
            if (transactions.Count > 0)
            {
                ITransactionInfo info = transactions.Pop();
                if (transactions.Count == 0)
                {
                    if (!sessionContext.HasSessionBinded)
                        throw new BusinessLayerException("Error on Commiting the current transaction because of missing binded document session.", "CommitTransaction");
                    
                    IDocumentSession session = this.sessionContext.GetCurrentSession();

                    try
                    {
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
                        this.CleanTransactions();
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void RollbackTransaction()
        {
            this.RollbackTransaction(null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cause"></param>
        public void RollbackTransaction(Exception cause)
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
                    this.CleanTransactions();
                }
            }
        }

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
        public ISessionContext SessionContext
        {
            get { return this.sessionContext; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected virtual void CleanTransactions()
        {
            if (transactionScope == null)
                return;

            this.transactionScope.Dispose();
            this.transactions.Clear();
            this.transactionScope = null;
        }
    }
}
