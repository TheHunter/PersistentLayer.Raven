using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PersistentLayer.Raven
{
    /// <summary>
    /// 
    /// </summary>
    public interface IDocumentStoreInfo
    {
        /// <summary>
        /// 
        /// </summary>
        string IdentityPartsSeparator { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <returns></returns>
        string GetTagName<TDocument>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <returns></returns>
        string GetTagName(Type typeDocument);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <returns></returns>
        string GetDocumentKeyTagName<TDocument>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <returns></returns>
        string GetDocumentKeyTagName(Type typeDocument);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="identifier"></param>
        /// <returns></returns>
        string MakeDocumentKey<TDocument>(ValueType identifier);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="identifier"></param>
        /// <returns></returns>
        string MakeDocumentKey<TDocument>(string identifier);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="typeDocument"></param>
        /// <returns></returns>
        string MakeDocumentKey(string identifier, Type typeDocument);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        string GetIdentifier<TDocument, TKey>(TKey identifier);
    }
}
