using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Listeners;
using Raven.Json.Linq;

namespace PersistentLayer.Raven.Test.Extra
{
    public class UpdateIdentifierListener
        : IDocumentStoreListener
    {
        public bool BeforeStore(string key, object entityInstance, RavenJObject metadata, RavenJObject original)
        {
            return true;
        }

        public void AfterStore(string key, object entityInstance, RavenJObject metadata)
        {
            return;
        }
    }
}
