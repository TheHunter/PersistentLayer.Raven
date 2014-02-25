using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PersistentLayer.Exceptions;
using PersistentLayer.Raven.Test.Domain;
using Raven.Abstractions.Data;
using Raven.Client.Converters;

namespace PersistentLayer.Raven.Test
{
    public class PagedDaoTest
        : DataAccessor
    {
        [Test]
        public void ExistsTest()
        {
            var res1=this.DAO.Exists<Person, int>(1);
            Assert.IsTrue(res1);

            var res2 = this.DAO.Exists<Person, string>("1");
            Assert.IsTrue(res2);

            var res3 = this.DAO.Exists<Person, string>("1001");
            Assert.IsFalse(res3);

        }

        [Test]
        public void ExistsIDsTest()
        {
            var res1 = this.DAO.Exists<Person, int>(new[]{1, 100});
            Assert.IsTrue(res1);

            var res2 = this.DAO.Exists<Person, int>(new[] { 1, 100, 5 });
            Assert.IsFalse(res2);

        }

        [Test]
        public void ExistsWithExpressionTest()
        {
            var res1 = this.DAO.Exists<Person>(person => person.ID > 1);
            Assert.IsTrue(res1);

            var res2 = this.DAO.Exists<Person>(person => person.ID > 1000);
            Assert.IsFalse(res2);

        }

        [Test]
        public void FindByTest()
        {
            var person1 = this.DAO.FindBy<Person, string>("1");
            var person2 = this.DAO.FindBy<Person, int>(1);

            Assert.IsNotNull(person1);
            Assert.IsNotNull(person2);

            var person3 = this.DAO.FindBy<Person, string>("2");
            var person4 = this.DAO.FindBy<Person, int>(2);

            Assert.IsNull(person3);
            Assert.IsNull(person4);

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
            var res = this.DAO.Exists<Student, string>("mykey");
            Assert.IsTrue(res);

            try
            {
                tranProvider.BeginTransaction();
                this.DAO.MakeTransient(st);
                tranProvider.CommitTransaction();
            }
            catch (Exception ex)
            {
                tranProvider.RollbackTransaction(ex);
                throw;
            }
            Assert.IsFalse(this.DAO.Exists<Student, string>(st.Key));
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

            var ret = this.DAO.FindBy<Person, string>("210");
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
    }
}
