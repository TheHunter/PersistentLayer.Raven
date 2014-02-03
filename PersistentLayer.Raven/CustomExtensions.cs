using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Abstractions.Data;

namespace PersistentLayer.Raven
{
    public static class CustomExtensions
    {
        public static RavenEtag ToRavenEtag(this Etag etag)
        {
            return new RavenEtag(etag);
        }
    }
}
