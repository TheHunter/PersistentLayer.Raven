using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentLayer.Raven.Test.Domain
{
    public class Teacher
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public int BoardNumber { get; set; }
    }

    public class TeacherV2
    {
        public int? Id { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public int BoardNumber { get; set; }
    }
}
