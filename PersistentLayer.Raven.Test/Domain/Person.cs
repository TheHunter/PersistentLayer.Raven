using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentLayer.Raven.Test.Domain
{
    public class Person
    {
        public Person()
        {
        }

        public Person(int id)
        {
            this.ID = id;
        }

        public int ID { get; protected set; }

        public string Name { get; set; }

        public string Surname { get; set; }
    }
}
