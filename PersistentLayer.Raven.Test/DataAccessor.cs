using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using PersistentLayer.Raven.Converters;
using PersistentLayer.Raven.Impl;
using PersistentLayer.Raven.Test.Domain;
using PersistentLayer.Raven.Test.Extra;
using Raven.Abstractions.Indexing;
using Raven.Client;
using Raven.Client.Connection;
using Raven.Client.Converters;
using Raven.Client.Document;
//using Raven.Client.Embedded;
using Raven.Client.Indexes;
using Raven.Imports.Newtonsoft.Json;
using Raven.Imports.Newtonsoft.Json.Serialization;

namespace PersistentLayer.Raven.Test
{
    [TestFixture]
    public class DataAccessor
    {
        //private readonly EmbeddableDocumentStore storeCached;
        private readonly DocumentStore storeCached;
        private readonly RavenRootEnterpriseDAO<object> dao;
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
                ApiKey = "8a780943-6aca-484a-9f30-6004b00836ac"
            };
            
            //storeCached.Conventions.FindIdentityPropertyNameFromEntityName = FindIdentityPropertyNameFromEntityName;
            storeCached.Conventions.FindIdentityProperty = info => IsKeyPropertyinfo(info);

            storeCached.Initialize();

            storeCached.Conventions.DefaultQueryingConsistency = ConsistencyOptions.AlwaysWaitForNonStaleResultsAsOfLastWrite;
            storeCached.Conventions.IdentityTypeConvertors.Add(new NumericNullableConverter<int>());
            storeCached.Conventions.IdentityTypeConvertors.Add(new NumericNullableConverter<long>());
            
            storeCached.Conventions.IdentityTypeConvertors.RemoveAll(converter => converter is Int32Converter);
            //Int32Converter

            storeCached.Conventions.AllowQueriesOnId = true;
            //storeCached.DatabaseCommands.NextIdentityFor()
            
            //<storeCached.Conventions.GetIdentityProperty()
            //storeCached.RegisterListener(new UpdateIdentifierListener());
            //storeCached.DatabaseCommands.PutIndex("Ciao",
            //                                      new IndexDefinitionBuilder<Person>()
            //                                          {
            //                                              Map = persons =>
            //                                                  from person in persons
            //                                                  select new
            //                                                      {
            //                                                          person.ID,
            //                                                          person.Name
            //                                                      }
            //                                                      ,
            //                                              SortOptions = { {person => person.Name, SortOptions.String} }
            //                                          }
            //    );

            //storeCached.Conventions.GetIdentityProperty()

            //storeCached.Conventions.CustomizeJsonSerializer =
            //    serializer => serializer.TypeNameHandling = TypeNameHandling.All;

            //storeCached.Conventions.RegisterIdConvention<Person>(Func);
            //storeCached.Initialize();
            sessionProvider = new SessionContextProvider(() => storeCached.OpenSession());

            docStoreInfo = new DocumentStoreInfo(storeCached);
            dao = new RavenRootEnterpriseDAO<object>(sessionProvider, docStoreInfo);
        }

        private bool IsKeyPropertyinfo(PropertyInfo info)
        {
            return info.Name == "Key" || info.Name == "ID" || info.Name == "Id";
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
            
        }

        [TearDown]
        public void UnBindSession()
        {
            
        }

        /// <summary>
        /// 
        /// </summary>
        public RavenRootEnterpriseDAO<object> DAO
        {
            get { return this.dao; }
        }

        public IDocumentStoreInfo DocStoreInfo
        {
            //get {return this.IDocumentStoreInfo docStoreInfo}
            get { return this.docStoreInfo; }
        }


        public DocumentStore StoreCached
        {
            get { return this.storeCached; }
        }
    }
}
