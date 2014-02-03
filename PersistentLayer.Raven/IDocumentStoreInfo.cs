using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentLayer.Raven
{
    public interface IDocumentStoreInfo
    {

        string IdentityPartsSeparator { get; }


        string GetTagName<TDocument>();


        string GetTagName(Type typeDocument);


        string GetDocumentKeyTagName<TDocument>();

        
        string GetDocumentKeyTagName(Type typeDocument);


        string MakeDocumentKey<TDocument>(ValueType identifier);


        string MakeDocumentKey<TDocument>(string identifier);


        string MakeDocumentKey(string identifier, Type typeDocument);


        bool IsValidIdentifier<TDocument>(object identifier);
    }
}
