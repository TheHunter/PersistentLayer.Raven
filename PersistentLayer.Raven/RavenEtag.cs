using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Abstractions.Data;

namespace PersistentLayer.Raven
{
    /// <summary>
    /// 
    /// </summary>
    public class RavenEtag
    {
        private readonly Etag etag;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="etag"></param>
        public RavenEtag(Etag etag)
        {
            this.etag = etag;
        }

        /// <summary>
        /// 
        /// </summary>
        public Etag Etag { get { return this.etag; } }
    }
}
