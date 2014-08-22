using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentLayer.Raven
{
    /// <summary>
    /// 
    /// </summary>
    public class IndexParameter
    {
        private readonly string name;
        private readonly bool isMapReduce;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="isMapReduce"></param>
        public IndexParameter(string name, bool isMapReduce = false)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("The given parameter cannot be empty or null.", "name");

            this.name = name.Trim();
            this.isMapReduce = isMapReduce;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get { return this.name; }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsMapReduce
        {
            get { return this.isMapReduce; }
        }
    }
}
