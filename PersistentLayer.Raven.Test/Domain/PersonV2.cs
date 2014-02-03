using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentLayer.Raven.Test.Domain
{
    public class PersonV2
    {
        public PersonV2()
        {
        }

        public PersonV2(int id)
        {
            this.ID = id;
        }

        public int? ID { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }
    }
}
