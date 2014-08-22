using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using PersistentLayer.Exceptions;
using PersistentLayer.Raven.Impl;
using PersistentLayer.Raven.Test.Domain;
using Raven.Abstractions.Data;
using Raven.Client.Converters;
using Raven.Client.Linq;

namespace PersistentLayer.Raven.Test
{
    public class PagedDaoTest
        : DataAccessor
    {
        [Test]
        public void ExistsTest()
        {
            var res1=this.DAO.Exists<Person>(1);
            Assert.IsTrue(res1);

            var res2 = this.DAO.Exists<Person>("1");
            Assert.IsFalse(res2);

            var res3 = this.DAO.Exists<Person>(1001);
            Assert.IsFalse(res3);

        }

        [Test]
        public void ExistsIDsTest()
        {
            var res1 = this.DAO.Exists<Person>(new[] { 1, 100 });
            Assert.IsTrue(res1);

            var res2 = this.DAO.Exists<Person>(new[] { 1, 100, 1005 });
            Assert.IsFalse(res2);

            var res3 = this.DAO.Exists<Person>(new[] { 1, 2, 3 });
            Assert.IsTrue(res3);

            var res4 = this.DAO.Exists<Person>(new object[] { 1, "2", 3 });
            Assert.IsFalse(res4);
        }

        [Test]
        public void ExistsIDsTest2()
        {
            List<int> l1 = new List<int>
                {
                    1, 2
                };

            var res = this.DAO.Exists<Person>(l1);
            Assert.IsTrue(res);
        }

        [Test]
        public void ExistsWithExpressionTest()
        {
            //var res2 = this.DAO.Exists<Person>(person => person.Name != null && person.ID > 10000 && person.ID < 10010);
            //var res1 = this.DAO.Exists<Person>(person => person.Name != null);
            //Assert.IsTrue(res1);

            //var res2 = this.DAO.Exists<Person>(person => person.Name != null && person.ID > 10000);
            //var res2 = this.DAO.Exists<Person>(person => person.Name != null && person.ID > 1);

            var res0 = this.DAO.Exists<Person>(person => person.Name != null && person.Surname != null);
            Assert.IsTrue(res0);

            // error whenever It tries to make an query with the own id
            //var res1 = this.DAO.FindAll<Person>(person => person.ID == 1);
            //Assert.IsNotNull(res1);

            // error whenever It tries to make an query with the own id
            //var res2 = this.DAO.Exists<Person>(person => person.ID == 1);
            //Assert.IsTrue(res2);


        }

        [Test]
        public void FindByTest()
        {
            var person1 = this.DAO.FindBy<Person>(1);
            Assert.IsNotNull(person1);

            var person2 = this.DAO.FindBy<Person>(2);
            Assert.IsNotNull(person2);
        }

        [Test]
        [ExpectedException(typeof(ExecutionQueryException))]
        public void WrongFindByTest()
        {
            this.DAO.FindBy<Person>("1");
        }

        [Test]
        public void FindByIDTest()
        {
            //students/1
            var res = this.DAO.FindBy<Student>("1");
            Assert.IsNull(res);

            var res1 = this.DAO.FindBy<Student>("mykey");
            Assert.IsNotNull(res1);

            var res2 = this.DAO.Exists<Student>(student => student.Key == "mykey");
            Assert.IsTrue(res2);
        }

        [Test]
        public void FindAllTest()
        {
            var persons = this.DAO.FindAll<Person>();
            Assert.IsNotNull(persons);

            var students = this.DAO.FindAll<Student>();
            Assert.IsNotNull(students);

            var persons2 = this.DAO.FindAll<PersonV2>();
            Assert.IsNotNull(persons2);
        }

        [Test]
        public void FindAllWithExpressionTest()
        {
            var persons = this.DAO.FindAll<Person>(person => person.ID > 1);
            Assert.IsNotNull(persons);

            var students = this.DAO.FindAll<Student>(student => student.Key != null);
            Assert.IsNotNull(students);
        }

        [Test]
        public void GetTransactionProviderTest()
        {
            Assert.IsNotNull(this.DAO.GetTransactionProvider());
        }

        [Test]
        public void TestMetadata()
        {
            //var session = this.Accessor.Session;
            
            ////var metadata = session.Advanced.GetMetadataFor(new Person(1){ Name = "Manu", Surname = "lur" });
            ////var id = session.Advanced.StoreIdentifier;
            //Type type = typeof (Person);
            ////type = typeof (Student);

            //Dictionary<string, string> conventions = new Dictionary<string, string>();
            //conventions.Add("GetTypeTagName(typeof (Person)", this.DocStore.Conventions.GetTypeTagName(type));
            //conventions.Add("GetClrTypeName(typeof (Person)", this.DocStore.Conventions.GetClrTypeName(type));
            //conventions.Add("GetIdentityProperty(typeof(Person)).Name", this.DocStore.Conventions.GetIdentityProperty(type).Name);
            //conventions.Add("IdentityPartsSeparator", this.DocStore.Conventions.IdentityPartsSeparator);
            //conventions.Add("SaveEnumsAsIntegers", this.DocStore.Conventions.SaveEnumsAsIntegers.ToString());

            //conventions.Add("FindTypeTagName.Invoke(type)", this.DocStore.Conventions.FindTypeTagName.Invoke(type));

            //DocStore.Conventions.TransformTypeTagNameToDocumentKeyPrefix("People");
            

            //Assert.IsNotNull(conventions);
        }

        [Test]
        public void MakePersistentTest()
        {
            Student st = new Student {Key = "mykey", Matricola = "121254842M"};
            var tranProvider = this.DAO.GetTransactionProvider();

            //try
            //{
            //    tranProvider.BeginTransaction();
            //    st.Key = "mykey";
            //    st.Matricola = "121254842M";

            //    this.DAO.MakePersistent<Student>(st);
            //    tranProvider.CommitTransaction();
            //}
            //catch (Exception ex)
            //{
            //    tranProvider.RollbackTransaction(ex);
            //    throw;
            //}

            var all = this.DAO.FindAll<Student>();
            Assert.IsNotNull(all);
            //var res = this.DAO.Exists<Student, string>("mykey");
            var res1 = this.DAO.Exists<Student>(student => student.Key == "mykey");
            Assert.IsTrue(res1);

            
        }

        [Test]
        public void TestCreatePerson2()
        {
            Person p = new Person { Name = "James", Surname = "Brown6" };

            //var person = this.Accessor.MakePersistent(p, "100");      //error
            //var person = this.Accessor.MakePersistent(p, "");         // la sequenza non contiene errori..
            //var person = this.Accessor.MakePersistent(p, "/");       // la sequenza non contiene errori..
            //var person = this.Accessor.MakePersistent(p, "people/");    // richiede un converter della stringa vuota..
            //var person = this.DAO.MakePersistent<Person, string>(p, "people/201");     //ok, ID = 100
            var person = this.DAO.MakePersistent(p, "210");     //ok, ID = 100
            //var person = this.Accessor.MakePersistent(p, "anotherCls/101");     //ok, ID = 100
            Assert.IsNotNull(person);

            var ret = this.DAO.FindBy<Person>("210");
            Assert.IsNotNull(ret);
        }

        [Test]
        [Description("Whenever It uses identity as document key, the key property must be nullable, otherwise underlaying system throws an exception.")]
        [ExpectedException(typeof(CommitFailedException))]
        public void CreatePerson()
        {
            var tran = this.DAO.GetTransactionProvider();
            try
            {
                tran.BeginTransaction();

                Person p = new Person { Name = "James", Surname = "Brown11" };
                Person person = this.DAO.MakePersistent(p, true);                   //uses identity algorithm
                Assert.IsNotNull(person);

                tran.CommitTransaction();

                Assert.AreNotEqual(person.ID, 0);
            }
            catch (Exception ex)
            {
                tran.RollbackTransaction(ex);
                throw ex;
            }
        }

        [Test]
        public void CreatePersonV2WithIdentity()
        {
            var tran = this.DAO.GetTransactionProvider();
            try
            {
                PersonV2 person;

                tran.BeginTransaction();

                PersonV2 p = new PersonV2 { Name = "James", Surname = "Brown12" };
                
                // ok uses identity
                // but The identity is not get by 
                person = this.DAO.MakePersistent(p, true);
                
                Assert.IsNotNull(person);

                // if commit operation wasn't executed, so no identity value has been computed by server side.
                Assert.IsNull(person.ID);

                tran.CommitTransaction();

                // afetr comit operation, the identity was computed by server, so after that identity is avaible
                Assert.IsNotNull(person.ID);
            }
            catch (Exception ex)
            {
                tran.RollbackTransaction(ex);
                throw ex;
            }
        }

        [Test]
        public void CreatePersonV2WithHILO()
        {
            var tran = this.DAO.GetTransactionProvider();
            try
            {
                PersonV2 person;

                tran.BeginTransaction();

                PersonV2 p = new PersonV2 { Name = "James", Surname = "Brown12" };

                // ok uses identity
                person = this.DAO.MakePersistent(p, false);

                Assert.IsNotNull(person);

                // if commit operation wasn't executed, so no identity value has been computed by server side.
                Assert.IsNotNull(person.ID);

                tran.CommitTransaction();

                // afetr comit operation, the identity was computed by server, so after that identity is avaible
                Assert.IsNotNull(person.ID);
            }
            catch (Exception ex)
            {
                tran.RollbackTransaction(ex);
                throw ex;
            }
        }

        [Test]
        public void TestTeacher1()
        {
            var tran = this.DAO.GetTransactionProvider();
            try
            {
                tran.BeginTransaction("CreateTeacher");
                List<Teacher> list = new List<Teacher>
                    {
                        new Teacher
                        {
                            Name = "Name1",
                            Surname = "Surname1",
                            BoardNumber = 1
                        },
                        new Teacher
                        {
                            Name = "Name2",
                            Surname = "Surname2",
                            BoardNumber = 101
                        },
                        new Teacher
                        {
                            Name = "Name1",
                            Surname = "Surname1",
                            BoardNumber = 2
                        }
                    };

                this.DAO.MakePersistent(list.AsEnumerable());

                tran.CommitTransaction();
            }
            catch (Exception ex)
            {
                tran.RollbackTransaction(ex);
                throw ex;
            }
        }

        [Test]
        public void TestAllPersons()
        {
            dynamic a = 5L;
            Assert.IsTrue(a is long);
            Assert.IsTrue(a is dynamic);

            //var persons = this.DAO.FindAll<Person>();
            //Assert.IsNotNull(persons);

            var personsV2 = this.DAO.FindAll<PersonV2>();
            Assert.IsNotNull(personsV2);
            
        }

        
        [Test]
        public void TestDocStoreInfo2()
        {
            List<string> buffer = new List<string>();
            buffer.Add(this.DocStoreInfo.GetIdentifier<Person>((byte)1));
            buffer.Add(this.DocStoreInfo.GetIdentifier<Person>((short)10));
            buffer.Add(this.DocStoreInfo.GetIdentifier<Person>(100));
            buffer.Add(this.DocStoreInfo.GetIdentifier<Person>(225m));

            try
            {
                buffer.Add(this.DocStoreInfo.GetIdentifier<Person>(string.Empty));
                Assert.IsFalse(true);
            }
            catch (InvalidIdentifierException ex)
            {
                Assert.IsTrue(true);
            }

            buffer.Add(this.DocStoreInfo.GetIdentifier<Person>(10D));
            buffer.Add(this.DocStoreInfo.GetIdentifier<Person>(200L));

            try
            {
                buffer.Add(this.DocStoreInfo.GetIdentifier<Person>(1.5F));
                Assert.IsFalse(true);
            }
            catch (InvalidIdentifierException ex)
            {
                Assert.IsTrue(true);
            }

            try
            {
                buffer.Add(this.DocStoreInfo.GetIdentifier<Person>(-2.9D));
                Assert.IsFalse(true);
            }
            catch (InvalidIdentifierException ex)
            {
                Assert.IsTrue(true);
            }
        }

        [Test]
        public void TestExternalizeExpression()
        {
            var customDAO = this.DAO as RavenRootEnterpriseDAO<object>;
            if (customDAO == null)
                Assert.IsTrue(false, "No DAO is avaible..");

            Expression<Func<IQueryable<Person>, Person>> queryExpr
                = persons => (from a in persons
                             where a.ID == 1
                             select a)
                             .FirstOrDefault()
                ;

            var person = customDAO.ExecuteExpression(queryExpr);
            Assert.IsNotNull(person);
        }

        [Test]
        public void TestExternalizeExpressionWithProjection()
        {
            var customDAO = this.DAO as RavenRootEnterpriseDAO<object>;
            if (customDAO == null)
                Assert.IsTrue(false, "No DAO is avaible..");

            Expression<Func<IQueryable<Person>, IEnumerable<string>>> queryExpr
                = persons => (from a in persons
                              select a.Name
                              )
                              .ToList()
                ;

            var peopleName = customDAO.ExecuteExpression(queryExpr);
            Assert.IsNotNull(peopleName);
        }

        [Test]
        public void TestExternalizeExpressionWithAnonymusType()
        {
            var customDAO = this.DAO as RavenRootEnterpriseDAO<object>;
            if (customDAO == null)
                Assert.IsTrue(false, "No DAO is avaible..");

            Expression<Func<IQueryable<Person>, IEnumerable<dynamic>>> queryExpr
                = persons => (from a in persons
                              select new { a.Name, a.Surname})
                ;

            var peopleName = customDAO.ExecuteExpression(queryExpr);
            Assert.IsNotNull(peopleName);

            Type tt = peopleName.GetType();
            Assert.IsNotNull(tt);

            string nome = peopleName.FirstOrDefault().Name;
            Assert.IsNotNull(nome);
        }

        [Test]
        public void TestExternalizeExpression2()
        {
            var customDAO = this.DAO as RavenRootEnterpriseDAO<object>;
            if (customDAO == null)
                Assert.IsTrue(false, "No DAO is avaible..");

            // taking full advantage of inference !!
            var result1 = customDAO.ExecuteExpression((IQueryable<Person> entities) => entities.FirstOrDefault());
            Assert.IsNotNull(result1);

            var result2 = customDAO.ExecuteExpression((IQueryable<Person> entities) => entities.Where(person1 => person1.ID < 5));
            Assert.IsNotNull(result2);

            var result3 = customDAO.ExecuteExpression((IQueryable<Person> entities) => entities.Select(person => new { person.Name, person.Surname }));
            Assert.IsNotNull(result3);

            var result4 = customDAO.ExecuteExpression((IQueryable<Person> entities) => entities.Where(person => person.ID > 1).Select(person => new { person.Name, person.Surname }));
            Assert.IsNotNull(result4);

            var result5 = customDAO.ExecuteExpression((IQueryable<Person> entities) => entities.Select(person => new PersonPrj { Name = person.Name, Surname = person.Surname }));
            Assert.IsNotNull(result5);

            var result6 = customDAO.ExecuteExpression((IQueryable<Person> entities) => entities.Select<Person, dynamic>(person => new { Name = person.Name, Surname = person.Surname }));
            Assert.IsNotNull(result6);
        }

        [Test]
        public void Test()
        {
            //double aa = 4.5f;
            Assert.IsTrue(typeof(bool?).IsAssignableFrom(typeof(bool)));
            Assert.IsTrue(typeof(byte?).IsAssignableFrom(typeof(byte)));
            Assert.IsTrue(typeof(int?).IsAssignableFrom(typeof(int)));
            Assert.IsTrue(typeof(long?).IsAssignableFrom(typeof(long)));
            Assert.IsTrue(typeof(float?).IsAssignableFrom(typeof(float)));
            Assert.IsTrue(typeof(double?).IsAssignableFrom(typeof(double)));
            Assert.IsTrue(typeof(decimal?).IsAssignableFrom(typeof(decimal)));

            Assert.IsFalse(typeof(int?).IsAssignableFrom(typeof(byte)));
            Assert.IsFalse(typeof(long?).IsAssignableFrom(typeof(byte)));
            Assert.IsFalse(typeof(long?).IsAssignableFrom(typeof(int)));

            Assert.IsFalse(typeof(byte).IsAssignableFrom(typeof(byte?)));
        }

        [Test]
        public void Test2()
        {
            var t = typeof (string[]);
            Assert.IsTrue(t.IsArray);

            var j = typeof (List<string>);
            Assert.IsTrue(j.IsClass && j.Implements(typeof(IEnumerable<string>)));

            Assert.IsTrue(j.Implements(typeof(IEnumerable)));
            Assert.IsTrue(j.Implements(typeof(IEnumerable<>)));

            Assert.IsFalse(j.Implements(typeof(List<string>)));


            MethodInfo method = typeof (Enumerable).GetMethod("ToList");
            Assert.IsNotNull(method);
        }
    }
}
