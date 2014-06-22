using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
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
        private readonly ISessionProvider sessionProvider;
        private readonly IDocumentStoreInfo storeInfo;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessionProvider"></param>
        /// <param name="storeInfo"></param>
        public RavenEnterpriseDAO(ISessionProvider sessionProvider, IDocumentStoreInfo storeInfo)
        {
            if (sessionProvider == null)
                throw new BusinessObjectException("The ITransactionProvider instance cannot be null.");

            if (storeInfo == null)
                throw new BusinessObjectException("The IDocumentStoreInfo cannot be null.");

            this.sessionProvider = sessionProvider;
            this.storeInfo = storeInfo;
        }

        /// <summary>
        /// Gets the current session binded.
        /// </summary>
        protected IDocumentSession Session
        {
            get
            {
                return this.sessionProvider
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
            try
            {
                string key = this.storeInfo
                             .MakeDocumentKey<TEntity>(identifier.ToString());
                
                return this.Session.Advanced.LuceneQuery<TEntity>()
                           .Search("Id", key)
                           .Any();
            }
            catch (Exception ex)
            {
                throw new ExecutionQueryException("Error on invoking the Exists method when the caller tried to find an instance with the given identifier, see innerException for details.", MakeNamingMethod<TEntity, TKey>("Exists"), ex);
            }
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
            try
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
            catch (Exception ex)
            {
                throw new ExecutionQueryException("Error on invoking the Exists method when the caller tried to verify input identifiers, see innerException for details.", MakeNamingMethod<TEntity, TKey>("Exists"), ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public bool Exists<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            try
            {
                return this.Session.Query<TEntity>()
                           .Any(predicate);
            }
            catch (Exception ex)
            {
                throw new ExecutionQueryException("Error on executing the Exists method when the caller tried to evaluate the parameter expression, see innerException for details", MakeNamingMethod<TEntity>("Exists"), ex);
            }
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
            try
            {
                var session = this.Session;

                if (identifier is ValueType)
                    return session.Load<TEntity>(identifier as ValueType);
              
                return session.Load<TEntity>(identifier.ToString());
            }
            catch (Exception ex)
            {
                throw new ExecutionQueryException("Error on executing the FindBy method when the caller tried to load the instance for the input identifier, see innerException for details.", MakeNamingMethod<TEntity, TKey>("FindBy"), ex);
            }
        }

        public TEntity UniqueResult<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            try
            {
                return this.Session.Query<TEntity>()
                           .Where(predicate)
                           .SingleOrDefault();
            }
            catch (Exception ex)
            {
                throw new ExecutionQueryException("Error on executing the FindBy method when the caller tried to load the instance for the input identifier, see innerException for details.", MakeNamingMethod<TEntity>("UniqueResult"), ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <returns></returns>
        public IEnumerable<TEntity> FindAll<TEntity>() where TEntity : class
        {
            try
            {
                RavenQueryStatistics stats;
                return this.Session.Query<TEntity>()
                           .Statistics(out stats)
                            //.Customize(x => x.WaitForNonStaleResults(TimeSpan.FromSeconds(10)))
                           .ToList();
            }
            catch (Exception ex)
            {
                throw new ExecutionQueryException("Error on executing the FindAll method.", MakeNamingMethod<TEntity>("FindAll"), ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> FindAll<TEntity>(Expression<Func<TEntity, bool>> predicate) where TEntity : class
        {
            try
            {
                //RavenQueryStatistics stats;
                return this.Session.Query<TEntity>()
                           //.Statistics(out stats)
                           .Where(predicate)
                           .ToList();
            }
            catch (Exception ex)
            {
                throw new ExecutionQueryException("Error on executing the FindAll method when the caller tried to evaluate the input expression.", MakeNamingMethod<TEntity>("FindAll"), ex);
            }
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
            try
            {
                RavenQueryStatistics stats;
                return this.Session.Query<TEntity>(indexName, isMapReduce)
                           .Statistics(out stats)
                           .ToList();
            }
            catch (Exception ex)
            {
                throw new ExecutionQueryException("Error on executing the FindAll method when the caller tried to evaluate the input index name, see innerException for details.", MakeNamingMethod<TEntity>("FindAll"), ex);
            }
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
            try
            {
                RavenQueryStatistics stats;
                return this.Session.Query<TEntity>(indexName, isMapReduce)
                           .Statistics(out stats)
                           .Where(predicate)
                           .ToList();
            }
            catch (Exception ex)
            {
                throw new ExecutionQueryException("Error on executing the FindAll method when the caller tried to evaluate the input index name with the given expression to evaluate., see innerException for details.", MakeNamingMethod<TEntity>("FindAll"), ex);
            }
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
            try
            {
                if (identifiers == null || !identifiers.Any())
                    throw new QueryArgumentException("Identifiers to load cannot be null or empty", MakeNamingMethod<TEntity, TKey>("FindAll"), "identifiers");

                return this.LoadDocuments<TEntity, TKey>(identifiers);
            }
            catch (Exception ex)
            {
                throw new ExecutionQueryException("Error on executing the FindAll method when the caller tried to load instances with the input identifiers.", MakeNamingMethod<TEntity, TKey>("FindAll"), ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="identifiers"></param>
        /// <returns></returns>
        public IEnumerable<TEntity> FindAll<TEntity, TKey>(params TKey[] identifiers)
            where TEntity : class
        {
            try
            {
                if (identifiers == null || !identifiers.Any())
                    throw new QueryArgumentException("Identifiers to load cannot be null or empty", MakeNamingMethod<TEntity, TKey>("FindAll"), "identifiers");

                return this.LoadDocuments<TEntity, TKey>(identifiers);
            }
            catch (Exception ex)
            {
                throw new ExecutionQueryException("Error on executing the FindAll method when the caller tried to load instances with the input identifiers.", MakeNamingMethod<TEntity, TKey>("FindAll"), ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="queryExpr"></param>
        /// <returns></returns>
        public TResult ExecuteExpression<TEntity, TResult>(Expression<Func<IEnumerable<TEntity>, TResult>> queryExpr)
            where TEntity : class
        {
            var ret = queryExpr.Compile()
                            .Invoke(this.Session.Query<TEntity>());

            Type resultType = typeof(TResult);
            if (resultType.IsInterface && resultType.Implements(typeof (IEnumerable)))
            {
                if (resultType.IsGenericType)
                {
                    // for version net40
                    Type t1 = resultType.GetGenericArguments()[0];
                    if (t1.IsAnonymous())
                    {
                        Delegate del = ReflectionExtension.ToListEnumerableDelegate(resultType);
                        object res = del.DynamicInvoke(ret);
                        return (TResult)res;
                    }
                    return Enumerable.ToList(ret as dynamic);

                    // for version net.35
                    //Delegate del = CustomExtensions.ToListEnumerableDelegate(resultType);
                    //object res = del.DynamicInvoke(ret);
                    //return (TResult)res;
                }
            }
            return ret;
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
            return this.sessionProvider;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public dynamic MakePersistent(dynamic entity)
        {
            try
            {
                this.Session.Store(entity);
                return entity;
            }
            catch (Exception ex)
            {
                throw new BusinessPersistentException("Error on making persistent the given dynamic instace.", "MakePersistent", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TEntity MakePersistentUsingIdentity<TEntity>(TEntity entity) where TEntity : class
        {
            try
            {
                string key = this.storeInfo.MakeDocumentKey<TEntity>(string.Empty);
                this.Session.Store(entity, key);
                return entity;
            }
            catch (Exception ex)
            {
                throw new BusinessPersistentException("Error on making persistent the given instance.", MakeNamingMethod<TEntity>("MakePersistentUsingIdentity"), ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TEntity MakePersistent<TEntity>(TEntity entity) where TEntity : class
        {
            try
            {
                this.Session.Store(entity);
                return entity;
            }
            catch (Exception ex)
            {
                throw new BusinessPersistentException("Error on making persistent the given instance.", MakeNamingMethod<TEntity>("MakePersistent"), ex);
            }
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
            try
            {
                this.Session.Store(entity, etag.Etag);
                return entity;
            }
            catch (Exception ex)
            {
                throw new BusinessPersistentException("Error on making persistent the given instance using the given Etag instance.", MakeNamingMethod<TEntity>("MakePersistent"), ex);
            }
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
            try
            {
                string key = this.storeInfo.MakeDocumentKey<TEntity>(this.storeInfo.GetIdentifier<TEntity>(identifier));
                this.Session.Store(entity, key);
                return entity;
            }
            catch (Exception ex)
            {
                throw new BusinessPersistentException("Error on making persistent the given instance using the given identifier.", MakeNamingMethod<TEntity>("MakePersistent"), ex);
            }
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
            try
            {
                string key = this.storeInfo.MakeDocumentKey<TEntity>(this.storeInfo.GetIdentifier<TEntity>(identifier));
                this.Session.Store(entity, etag.Etag, key);
                return entity;
            }
            catch (Exception ex)
            {
                throw new BusinessPersistentException("Error on making persistent the given instance using the given identifier and Etag instance.", MakeNamingMethod<TEntity>("MakePersistent"), ex);
            }
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
                return entities;

            try
            {
                return entities.Select(n => this.MakePersistent(n))
                           .ToList();
            }
            catch (Exception ex)
            {
                throw new BusinessPersistentException("Error on making persistent the given collection of instances.", MakeNamingMethod<TEntity>("MakePersistent"), ex);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entity"></param>
        public void MakeTransient<TEntity>(TEntity entity) where TEntity : class
        {
            try
            {
                this.Session.Delete(entity);
            }
            catch (Exception ex)
            {
                throw new BusinessPersistentException("Error on making transient the given instance.", MakeNamingMethod<TEntity>("MakeTransient"), ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="entities"></param>
        public void MakeTransient<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            try
            {
                if (entities == null || !entities.Any())
                    return;

                foreach (var entity in entities)
                {
                    this.MakeTransient(entity);
                }
            }
            catch (Exception ex)
            {
                throw new BusinessPersistentException("Error on making transient the given collection of instances.", MakeNamingMethod<TEntity>("MakeTransient"), ex);
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
            try
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
            catch (Exception ex)
            {
                throw new ExecutionQueryException("Error on executing the given instance", MakeNamingMethod<TEntity>("GetPagedResult"), ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <param name="namingMethod"></param>
        /// <returns></returns>
        protected static string MakeNamingMethod<TLeft>(string namingMethod)
        {
            if (string.IsNullOrWhiteSpace(namingMethod))
                return string.Empty;

            return string.Format("{0}<{1}>", namingMethod, typeof(TLeft).Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TLeft"></typeparam>
        /// <typeparam name="TRight"></typeparam>
        /// <param name="namingMethod"></param>
        /// <returns></returns>
        protected static string MakeNamingMethod<TLeft, TRight>(string namingMethod)
        {
            if (string.IsNullOrWhiteSpace(namingMethod))
                return string.Empty;

            return string.Format("{0}<{1}, {2}>", namingMethod, typeof(TLeft).Name, typeof(TRight).Name);
        }
    }
}
