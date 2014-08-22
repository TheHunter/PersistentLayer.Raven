using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using PersistentLayer.Raven.Test.Domain;

namespace PersistentLayer.Raven.Test
{
    public class RavenDBTester
        : DataAccessor
    {
        [Test]
        public void SaveInstanceWithHILO()
        {
            try
            {
                //var query = this.DAO.MakeQuery<PersonV2>();
                var session = this.StoreCached.OpenSession();

                Teacher p = new Teacher { Name = "James", Surname = "Brown13" };

                //person = this.DAO.MakePersistent(p, false);
                session.Store(p);

                Assert.IsNotNull(p);

                session.SaveChanges();

                // if commit operation wasn't executed, so no identity value has been computed by server side.
                Assert.IsNotNull(p.Id);
            }
            catch (Exception ex)
            {
                throw new Exception("New instance using HILO.", ex);
            }
        }

    }
}
