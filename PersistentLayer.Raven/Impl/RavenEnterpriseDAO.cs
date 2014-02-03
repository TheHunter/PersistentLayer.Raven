using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using PersistentLayer.Exceptions;
using PersistentLayer.Impl;
using Raven.Abstractions.Data;
using Raven.Client;
using Raven.Client.Linq;

namespace PersistentLayer.Raven.Impl
{
    public class RavenEnterpriseDAO
        : IRavenPagedDAO
    {
        private readonly IRavenTransactionProvider transactionProvider;
        private readonly IDocumentStoreInfo storeInfo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="transactionProvider"></param>
        /// <param name="storeInfo"></param>
        public RavenEnterpriseDAO(IRavenTransactionProvider transactionProvider, IDocumentStoreInfo storeInfo)
        {
            if (transactionProvider == null)
                throw new BusinessObjectException("The ITransactionProvider instance cannot be null.");

            if (storeInfo == null)
                throw new BusinessObjectException("The IDocumentStoreInfo cannot be null.");

            this.transactionProvider = transactionProvider;
            this.storeInfo = storeInfo;
        }

        /// <summary>
        /// Gets the current session binded.
        /// </summary>
        protected IDocumentSession Session
        {
            get
            {
                return this.transactionProvider
                           .SessionContext
                           .GetCurrentSession();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public bool Exists<TEntity, TKey>(TKey identifier) where TEntity : class
        {
            string key = this.storeInfo
                             .MakeDocumentKey<TEntity>(identifier.ToString());

            return this.Session.Advanced.LuceneQuery<TEntity>()
                       .Search("Id", key)
                       .Any();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="identifiers"></param>
        /// <returns></returns>
        public bool Exists<TEntity, TKey>(IEnumerable<TKey> identifiers) where TEntity : class
        {
            int total = identifiers.Count();

            return this.Session.Advanced.LuceneQuery<TEntity>()
                        .SelectFields<TEntity>("Id")
                        .WhereIn("Id",
                                 identifiers.Select(key => this.storeInfo.MakeDocumentKey<TEntity>(key.ToString()) as object))
                        .ToArray()
                        .Length == total
                        ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool Exists<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            return this.Session.Query<TEntity>()
                       .Any(predicate);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public TEntity FindBy<TEntity, TKey>(TKey identifier)
            where TEntity : class
        {
            var session = this.Session;

            if (identifier is ValueType)
                return session.Load<TEntity>(identifier as ValueType);

            string key = this.storeInfo.MakeDocumentKey<TEntity>
                (
                    identifier is string ? identifier as string : identifier.ToString()
                );
            return session.Load<TEntity>(key);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public IEnumerable<TEntity> FindAll<TEntity>() where TEntity : class
        {
            RavenQueryStatistics stats;
            return this.Session.Query<TEntity>()
                       .Statistics(out stats)
                       //.Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(10)))
                       .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> FindAll<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            RavenQueryStatistics stats;
            return this.Session.Query<TEntity>()
                       .Statistics(out stats)
                       .Where(predicate)
                       .ToList();
        }

        
        public ITransactionProvider GetTransactionProvider()
        {
            return this.transactionProvider;
        }

        
        public TEntity MakePersistent<TEntity>(TEntity entity) where TEntity : class
        {
            this.Session.Store(entity);
            return entity;
        }


        public TEntity MakePersistent<TEntity>(TEntity entity, RavenEtag etag) where TEntity : class
        {
            this.Session.Store(entity, etag.Etag);
            return entity;
        }


        public TEntity MakePersistent<TEntity, TKey>(TEntity entity, TKey identifier) where TEntity : class
        {
            string key = this.storeInfo.MakeDocumentKey<TEntity>(identifier.ToString());
            this.Session.Store(entity, key);
            return entity;
        }


        public TEntity MakePersistent<TEntity, TKey>(TEntity entity, TKey identifier, RavenEtag etag) where TEntity : class
        {
            string key = this.storeInfo.MakeDocumentKey<TEntity>(identifier.ToString());
            this.Session.Store(entity, etag.Etag, key);
            return entity;
        }
        

        public IEnumerable<TEntity> MakePersistent<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            if (entities == null || !entities.Any())
                return null;

            return entities.Select(n => this.MakePersistent(n)).ToList();
        }

        
        public void MakeTransient<TEntity>(TEntity entity) where TEntity : class
        {
            this.Session.Delete(entity);
        }

        
        public void MakeTransient<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            if (entities == null || !entities.Any())
                return;

            foreach (var entity in entities)
            {
                this.MakeTransient(entity);
            }
        }

        
        public IPagedResult<TEntity> GetPagedResult<TEntity>(int startIndex, int pageSize, Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            RavenQueryStatistics stats;
            var result = this.Session.Query<TEntity>()
                             .Statistics(out stats)
                             .Where(predicate)
                             .Skip(startIndex)
                             .Take(pageSize)
                             .ToArray();

            return new PagedResult<TEntity>(startIndex, pageSize, stats.TotalResults, result);
        }


        public IPagedResult<TEntity> GetPagedResult<TEntity>(int startIndex, int pageSize, IQueryable<TEntity> query) where TEntity : class
        {
            if (!(query is IRavenQueryable<TEntity>))
                return null;

            RavenQueryStatistics stats;
            IRavenQueryable<TEntity> exp = query as IRavenQueryable<TEntity>;
            var result = exp.Statistics(out stats)
                            .Skip(startIndex)
                            .Take(pageSize)
                            .ToArray();

            return new PagedResult<TEntity>(startIndex, pageSize, stats.TotalResults, result);
        }

        public IPagedResult<TEntity> GetIndexPagedResult<TEntity>(int pageIndex, int pageSize, IQueryable<TEntity> query) where TEntity : class
        {
            if (!(query is IRavenQueryable<TEntity>))
                return null;

            RavenQueryStatistics stats;
            IRavenQueryable<TEntity> exp = query as IRavenQueryable<TEntity>;
            var result = exp.Statistics(out stats)
                            .Skip(pageIndex * pageSize)
                            .Take(pageSize)
                            .ToArray();

            return new PagedResult<TEntity>(pageIndex * pageSize, pageSize, stats.TotalResults, result);
        }
    }
}
