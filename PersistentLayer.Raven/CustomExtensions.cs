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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullable(this Type type)
        {
            //return type.IsClass || type.IsInterface || type.IsAbstract || type.IsEquivalentTo()
            return type.IsClass || type.IsInterface || type.IsAbstract || type.GetInterface("Nullable`1") != null;
        }
    }
}
