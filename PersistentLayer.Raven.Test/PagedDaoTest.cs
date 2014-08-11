using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
            Assert.IsTrue(res2);

            var res3 = this.DAO.Exists<Person>("1001");
            Assert.IsFalse(res3);

        }

        [Test]
        public void ExistsIDsTest()
        {
            var res1 = this.DAO.Exists<Person>(new[]{1, 100});
            Assert.IsTrue(res1);

            var res2 = this.DAO.Exists<Person>(new[] { 1, 100, 5 });
            Assert.IsFalse(res2);

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
            //var res1 = this.DAO.Exists<Person>(person => person.ID > 1);
            //Assert.IsTrue(res1);

            //var res2 = this.DAO.Exists<Person>(person => person.ID == 10000);
            var res2 = this.DAO.Exists<Person>(person => person.Name != null && person.ID > 10000 && person.ID < 10010);
            Assert.IsFalse(res2);

        }

        [Test]
        public void FindByTest()
        {
            var person1 = this.DAO.FindBy<Person>("1");
            var person2 = this.DAO.FindBy<Person>(1);

            Assert.IsNotNull(person1);
            Assert.IsNotNull(person2);

            var person3 = this.DAO.FindBy<Person>("2");
            var person4 = this.DAO.FindBy<Person>(2);

            Assert.IsNull(person3);
            Assert.IsNull(person4);


            IRavenQueryable<Person> a;
            
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
            Student st = new Student();
            st.Key = "mykey";
            st.Matricola = "121254842M";
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

            //try
            //{
            //    tranProvider.BeginTransaction();
            //    this.DAO.MakeTransient(st);
            //    tranProvider.CommitTransaction();
            //}
            //catch (Exception ex)
            //{
            //    tranProvider.RollbackTransaction(ex);
            //    throw;
            //}
            //Assert.IsFalse(this.DAO.Exists<Student, string>(st.Key));
        }

        //[Test]
        //public void TestCreatePerson()
        //{
        //    Person p = new Person();
        //    p.Name = "James";
        //    p.Surname = "Brown";

        //    var person = this.Accessor.MakePersistent(p);
        //    this.Save();
            
        //    Assert.IsNotNull(person);
        //}

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
        public void CreatePerson()
        {
            var tran = this.DAO.GetTransactionProvider();
            try
            {
                tran.BeginTransaction();

                Person p = new Person { Name = "James", Surname = "Brown7" };
                //Person person = this.DAO.MakePersistent<Person, int>(p, 210);     //ok, ID
                //Person person = this.DAO.MakePersistent<Person, string>(p, "220");     //ok, ID
                //Person person = this.DAO.MakePersistent(p, "221");     //ok
                //Person person = this.DAO.MakePersistent(p, "ciao");     //ko
                Person person = this.DAO.MakePersistent(p, "");     //ko
                Assert.IsNotNull(person);

                tran.CommitTransaction();

            }
            catch (Exception ex)
            {
                tran.RollbackTransaction(ex);
                throw ex;
            }
        }

        [Test]
        public void CreatePersonV2()
        {
            var tran = this.DAO.GetTransactionProvider();
            try
            {
                PersonV2 person;

                tran.BeginTransaction();

                PersonV2 p = new PersonV2 { Name = "James", Surname = "Brown8" };
                //person = this.DAO.MakePersistent<Person, int>(p, 210);     //ok, ID
                //person = this.DAO.MakePersistent<Person, string>(p, "220");     //ok, ID
                //person = this.DAO.MakePersistent(p, "221");     //ok
                //person = this.DAO.MakePersistent(p, "ciao");     //ko format exception
                //person = this.DAO.MakePersistent(p, "");     //ok  uses identity
                person = this.DAO.MakePersistent(p, (string)null);     //ok  uses identity
                Assert.IsNotNull(person);

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
            //var persons = this.DAO.FindAll<Person>();
            //Assert.IsNotNull(persons);

            var personsV2 = this.DAO.FindAll<PersonV2>();
            Assert.IsNotNull(personsV2);
            
        }

        //[Test]
        //public void TestCreatePersonV2()
        //{
        //    var person = new PersonV2(1);
        //    person.Name = "Name1";
        //    person.Surname = "Surname1";

        //    this.Accessor.MakePersistent(person);
        //    this.Save();

        //    Assert.IsNotNull(person);
        //}

        //[Test]
        //public void TestCreatePersonV2_2()
        //{
        //    var person = new PersonV2();
        //    person.Name = "Name2";
        //    person.Surname = "Surname2";

        //    //this.Accessor.MakePersistent(person);
        //    this.Accessor.MakePersistent(person, "personV2s/");
        //    this.Save();

        //    Assert.IsNotNull(person);
        //}

        //[Test]
        //public void TestCreateStudent()
        //{
        //    Student st = new Student();
        //    st.Key = "first";           //Raven keeps the assign identifier
        //    st.Matricola = "12121A";

        //    var student = this.Accessor.MakePersistent(st);

        //    //newone without ID
        //    // in this case Raven generates an unique identifier composed by class naming + incremental Id
        //    // generated by HILO algorithm
        //    this.Accessor.MakePersistent(new Student {Matricola = "451263B"});

        //    this.Save();

        //    Assert.IsNotNull(student);
        //}

        

        //[Test]
        //public void TestCreateStudent2()
        //{
            
        //    List<Student> list = new List<Student>();
        //    //newone without ID
        //    // in this case Raven generates an unique identifier composed by class naming + incremental Id
        //    // generated by HILO algorithm
            
        //    //list.Add(this.Accessor.MakePersistent(new Student { Key = "/", Matricola = "011263B" }));  // errore nella generazione della chiave.
        //    //list.Add(this.Accessor.MakePersistent(new Student { Key = "students/", Matricola = "011263B" }));
        //    list.Add(this.Accessor.MakePersistent(new Student { Key = "students/", Matricola = "021263B" }));
        //    //list.Add(this.Accessor.MakePersistent(new Student { Key = "students/", Matricola = "031263B" }));

        //    this.Save();

        //    Assert.IsNotNull(list);
        //}

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
