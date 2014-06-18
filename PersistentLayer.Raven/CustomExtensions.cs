using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Raven.Abstractions.Data;

namespace PersistentLayer.Raven
{
    /// <summary>
    /// 
    /// </summary>
    public static class CustomExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="etag"></param>
        /// <returns></returns>
        public static RavenEtag ToRavenEtag(this Etag etag)
        {
            return new RavenEtag(etag);
        }

    }
}
