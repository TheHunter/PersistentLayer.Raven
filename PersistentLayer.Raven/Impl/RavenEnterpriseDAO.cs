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
    /// <summary>
    /// 
    /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="isMapReduce"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> FindAll<TEntity>(string indexName, bool isMapReduce) where TEntity : class
        {
            RavenQueryStatistics stats;
            return this.Session.Query<TEntity>(indexName, isMapReduce)
                       .Statistics(out stats)
                       .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="indexName"></param>
        /// <param name="isMapReduce"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> FindAll<TEntity>(string indexName, bool isMapReduce, Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            RavenQueryStatistics stats;
            return this.Session.Query<TEntity>(indexName, isMapReduce)
                       .Statistics(out stats)
                       .Where(predicate)
                       .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="identifiers"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> FindAll<TEntity, TKey>(IEnumerable<TKey> identifiers) where TEntity : class
        {
            if (identifiers == null || !identifiers.Any())
                throw new QueryArgumentException("Identifiers to load cannot be null or empty", "FindAll", "identifiers");

            return this.LoadDocuments<TEntity, TKey>(identifiers);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="identifiers"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> FindAll<TEntity, TKey>(params TKey[] identifiers) where TEntity : class
        {
            if (identifiers == null || !identifiers.Any())
                throw new QueryArgumentException("Identifiers to load cannot be null or empty", "FindAll", "identifiers");


            return this.LoadDocuments<TEntity, TKey>(identifiers);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="identifiers"></param>
        /// <returns></returns>
        private IEnumerable<TEntity> LoadDocuments<TEntity, TKey>(IEnumerable<TKey> identifiers) where TEntity : class
        {
            Type keyType = typeof (TKey);

            if (keyType.IsValueType)
                return this.Session.Load<TEntity>(identifiers.Cast<ValueType>());

            IEnumerable<string> keys = keyType.Name == "String"
                                           ? identifiers.Select(
                                               n => this.storeInfo.MakeDocumentKey<TEntity>(n as string))
                                           : identifiers.Select(
                                               n => this.storeInfo.MakeDocumentKey<TEntity>(n.ToString()));

            return this.Session.Load<TEntity>(keys);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ITransactionProvider GetTransactionProvider()
        {
            return this.transactionProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public dynamic MakePersistent(dynamic entity)
        {
            this.Session.Store(entity);
            return entity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TEntity MakePersistentUsingIdentity<TEntity>(TEntity entity) where TEntity : class
        {
            string key = this.storeInfo.MakeDocumentKey<TEntity>(string.Empty);
            this.Session.Store(entity, key);
            return entity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TEntity MakePersistent<TEntity>(TEntity entity) where TEntity : class
        {
            this.Session.Store(entity);
            return entity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <param name="etag"></param>
        /// <returns></returns>
        public TEntity MakePersistent<TEntity>(TEntity entity, RavenEtag etag) where TEntity : class
        {
            this.Session.Store(entity, etag.Etag);
            return entity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="entity"></param>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public TEntity MakePersistent<TEntity, TKey>(TEntity entity, TKey identifier) where TEntity : class
        {
            

            string key = this.storeInfo.MakeDocumentKey<TEntity>(identifier.ToString());
            this.Session.Store(entity, key);
            return entity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="entity"></param>
        /// <param name="identifier"></param>
        /// <param name="etag"></param>
        /// <returns></returns>
        public TEntity MakePersistent<TEntity, TKey>(TEntity entity, TKey identifier, RavenEtag etag) where TEntity : class
        {
            string key = this.storeInfo.MakeDocumentKey<TEntity>(identifier.ToString());
            this.Session.Store(entity, etag.Etag, key);
            return entity;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> MakePersistent<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            if (entities == null || !entities.Any())
                return null;

            return entities.Select(n => this.MakePersistent(n))
                           .ToList();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        public void MakeTransient<TEntity>(TEntity entity) where TEntity : class
        {
            this.Session.Delete(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        public void MakeTransient<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            if (entities == null || !entities.Any())
                return;

            foreach (var entity in entities)
            {
                this.MakeTransient(entity);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="startIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
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

    }
}
