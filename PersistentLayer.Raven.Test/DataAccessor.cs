using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using PersistentLayer.Raven.Converters;
using PersistentLayer.Raven.Impl;
using PersistentLayer.Raven.Test.Domain;
using Raven.Client;
using Raven.Client.Connection;
using Raven.Client.Converters;
using Raven.Client.Document;
using Raven.Client.Embedded;
using Raven.Imports.Newtonsoft.Json;
using Raven.Imports.Newtonsoft.Json.Serialization;

namespace PersistentLayer.Raven.Test
{
    [TestFixture]
    public class DataAccessor
    {
        private readonly EmbeddableDocumentStore storeCached;
        private readonly RavenEnterpriseDAO dao;
        private readonly ISessionBinder sessionContext;
        private readonly IDocumentStoreInfo docStoreInfo;
        private readonly IRavenTransactionProvider transactionProvider;

        public DataAccessor()
        {
            storeCached = new EmbeddableDocumentStore
            {
                DataDirectory =
                    @"C:\Users\Diego\Documents\visual studio 2012\Projects\PersistentLayer.Raven\PersistentLayer.Raven.Test\App_Data\RavenDB"
            };

            //storeCached.Conventions.FindIdentityPropertyNameFromEntityName = FindIdentityPropertyNameFromEntityName;
            storeCached.Conventions.FindIdentityProperty = (info => info.Name == "ID" || info.Name == "Key");
            storeCached.Conventions.DefaultQueryingConsistency = ConsistencyOptions.AlwaysWaitForNonStaleResultsAsOfLastWrite;
            storeCached.Conventions.IdentityTypeConvertors.Add(new NumericNullableConverter<int>());
            storeCached.Conventions.IdentityTypeConvertors.Add(new NumericNullableConverter<long>());

            storeCached.Conventions.AllowQueriesOnId = true;

            //storeCached.Conventions.GetIdentityProperty()

            //storeCached.Conventions.CustomizeJsonSerializer =
            //    serializer => serializer.TypeNameHandling = TypeNameHandling.All;

            //storeCached.Conventions.RegisterIdConvention<Person>(Func);
            storeCached.Initialize();
            sessionContext = new SessionBinder();
            transactionProvider = new RavenTransactionProvider(sessionContext);
            docStoreInfo = new DocumentStoreInfo(storeCached);
            dao = new RavenEnterpriseDAO(transactionProvider, docStoreInfo);
        }

        [TestFixtureSetUp]
        protected void OnStartUp()
        {
            
        }

        [TestFixtureTearDown]
        protected void OnTearDown()
        {
            
        }

        [SetUp]
        protected void BindSession()
        {
            sessionContext.Bind(storeCached.OpenSession());
        }

        [TearDown]
        public void UnBindSession()
        {
            var session = sessionContext.UnBind();
            if (session != null)
            {
                // What happens when a Dispose method is called twice ??
                session.Dispose();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IRavenPagedDAO DAO
        {
            get { return this.dao; }
        }

        public IDocumentStoreInfo DocStoreInfo
        {
            //get {return this.IDocumentStoreInfo docStoreInfo}
            get { return this.docStoreInfo; }
        }
    }
}
