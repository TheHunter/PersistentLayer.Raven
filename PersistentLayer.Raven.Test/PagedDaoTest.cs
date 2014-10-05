using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using NUnit.Framework;
using PersistentLayer.Exceptions;
using PersistentLayer.Raven.Impl;
using PersistentLayer.Raven.Test.Domain;
using Raven.Abstractions.Data;
using Raven.Client;
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
            Assert.IsNull(res1);

            var res2 = this.DAO.Exists<Student>(student => student.Key == "mykey");
            Assert.IsFalse(res2);
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
        public void MakePersistentTest()
        {
            Student st = null;
            var tranProvider = this.DAO.GetTransactionProvider();

            try
            {
                tranProvider.BeginTransaction();

                st = new Student { Key = "mykey", Matricola = "121254842M" };

                this.DAO.MakePersistent(st);
                tranProvider.CommitTransaction();
            }
            catch (Exception ex)
            {
                tranProvider.RollbackTransaction(ex);
                throw;
            }

            var all = this.DAO.FindAll<Student>();
            Assert.IsNotNull(all);
            //var res = this.DAO.Exists<Student, string>("mykey");
            var res1 = this.DAO.Exists<Student>(student => student.Key == "mykey");
            Assert.IsTrue(res1);

            
        }

        [Test]
        [ExpectedException(typeof(BusinessPersistentException))]
        public void TestCreatePerson2()
        {
            var tranProvider = this.DAO.GetTransactionProvider();
            try
            {
                tranProvider.BeginTransaction();

                Person p = new Person { Name = "James", Surname = "Brown6" };
                var person = this.DAO.MakePersistent(p, "210");
                //Assert.IsNotNull(person);

                //var ret = this.DAO.FindBy<Person>("210");
                //Assert.IsNotNull(ret);

                tranProvider.CommitTransaction();
            }
            catch (Exception ex)
            {
                tranProvider.RollbackTransaction(ex);
                throw;
            }
        }

        [Test]
        [Description("Whenever It uses identity as document key, the key property must be nullable, otherwise underlaying system throws an exception.")]
        //[ExpectedException(typeof(CommitFailedException))]
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
        public void TestTeachers()
        {
            //IEnumerable<Teacher> rr;
            //rr = this.DAO.FindAll<Teacher>(teacher => teacher.BoardNumber >= 1 && teacher.BoardNumber <= 2);
            //Assert.IsNotNull(rr);
            //Assert.IsTrue(rr.Count() == 2);

            //rr = this.DAO.FindAll<Teacher>(teacher => teacher.BoardNumber >= 2 && teacher.BoardNumber <= 13);
            //Assert.IsNotNull(rr);
            //Assert.IsTrue(rr.Count() == 3);
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
            var customDAO = this.DAO;
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
            var customDAO = this.DAO;
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
            var customDAO = this.DAO;
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
            var customDAO = this.DAO;
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
        public void Test1()
        {
            var customDAO = this.DAO;
            var qq = customDAO.MakeLuceneQuery<Person>();
            //var r = qq.WhereBetweenOrEqual("Id", "people/1", "people/2").ToList();
            //r = qq.WhereBetween("Id", "people/1", "people/2").ToList();
            var r = qq.WhereGreaterThan("Id", "people/1").ToList();
            

            //Assert.AreEqual(r.Count, 2);
            Console.WriteLine(r.Count);
        }

        [Test]
        public void Test2()
        {
            var customDAO = this.DAO;
            var res = customDAO.FindAll<Teacher>(teacher => teacher.BoardNumber > 1 && teacher.BoardNumber < 3);

            Assert.IsTrue(res != null);
            Assert.AreEqual(res.Count(), 4);

            foreach (var teacher in res)
            {
                Assert.IsTrue(customDAO.IsCached<Teacher>(teacher.Id));
            }

            Assert.IsFalse(customDAO.IsCached<Teacher>(5));
        }

        [Test]
        public void Test3()
        {
            var customDAO = this.DAO;
            var index = new IndexParameter("Teachers/TeachersById");

            var res = customDAO.FindAll<Teacher>(index, teacher => teacher.Id > 1 && teacher.Id < 3);
            Assert.IsNotNull(res);
            Console.WriteLine(res);
        }


    }


    
}
