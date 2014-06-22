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
        //private readonly EmbeddableDocumentStore storeCached;
        private readonly DocumentStore storeCached;
        private readonly RavenEnterpriseDAO dao;
        private readonly IDocumentStoreInfo docStoreInfo;
        private readonly ISessionProvider sessionProvider;

        public DataAccessor()
        {
            //storeCached = new EmbeddableDocumentStore
            //{
            //    DataDirectory =
            //        @"C:\Users\Diego\Documents\visual studio 2012\Projects\PersistentLayer.Raven\PersistentLayer.Raven.Test\App_Data\RavenDB"
            //};


            storeCached = new DocumentStore
            {
                Url = "https://kiwi.ravenhq.com/databases/TheHunter-salesarea",
                ApiKey = "9994a2f8-fd78-418f-9490-9ff0e757fe6c"
            };
            storeCached.Initialize();

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
            sessionProvider = new SessionContextProvider(() => storeCached.OpenSession());

            docStoreInfo = new DocumentStoreInfo(storeCached);
            dao = new RavenEnterpriseDAO(sessionProvider, docStoreInfo);
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
            //sessionContext.Bind(storeCached.OpenSession());
        }

        [TearDown]
        public void UnBindSession()
        {
            //var session = sessionContext.UnBind();
            //if (session != null)
            //{
            //    // What happens when a Dispose method is called twice ??
            //    session.Dispose();
            //}
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
