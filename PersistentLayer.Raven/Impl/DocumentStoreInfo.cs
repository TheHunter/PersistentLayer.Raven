using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using PersistentLayer.Exceptions;
using Raven.Client;

namespace PersistentLayer.Raven.Impl
{
    /// <summary>
    /// 
    /// </summary>
    public class DocumentStoreInfo
        : IDocumentStoreInfo
    {
        private readonly IDocumentStore store;
        private readonly Dictionary<Type, TypeConverter> converters;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="store"></param>
        public DocumentStoreInfo(IDocumentStore store)
        {
            if (store == null)
                throw new BusinessLayerException("The document store cannot be null.");

            this.store = store;
            this.converters = new Dictionary<Type, TypeConverter>();
        }

        /// <summary>
        /// 
        /// </summary>
        public string IdentityPartsSeparator
        {
            get { return store.Conventions.IdentityPartsSeparator; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <returns></returns>
        public string GetTagName<TDocument>()
        {
            return this.GetTagName(typeof (TDocument));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <returns></returns>
        public string GetTagName(Type typeDocument)
        {
            return this.store.Conventions.GetTypeTagName(typeDocument);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <returns></returns>
        public string GetDocumentKeyTagName<TDocument>()
        {
            return this.GetDocumentKeyTagName(typeof (TDocument));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeDocument"></param>
        /// <returns></returns>
        public string GetDocumentKeyTagName(Type typeDocument)
        {
            return this.store.Conventions
                       .TransformTypeTagNameToDocumentKeyPrefix
                       .Invoke(this.GetTagName(typeDocument));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public string MakeDocumentKey<TDocument>(ValueType identifier)
        {
            return this.MakeDocumentKey(identifier.ToString(), typeof (TDocument));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public string MakeDocumentKey<TDocument>(string identifier)
        {
            return this.MakeDocumentKey(identifier, typeof (TDocument));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="identifier"></param>
        /// <param name="typeDocument"></param>
        /// <returns></returns>
        public string MakeDocumentKey(string identifier, Type typeDocument)
        {
            return string.Format("{0}{1}{2}",
                                 this.GetDocumentKeyTagName(typeDocument),
                                 this.IdentityPartsSeparator,
                                 identifier
                );
        }


        public bool IsValidIdentifier<TDocument>(object identifier)
        {
            var propertyInfo = this.store.Conventions.GetIdentityProperty(typeof (TDocument));

            if (identifier == null)
            {
                //System.Nullable`1
                return false;
            }
            return this.GetConverterOf(identifier.GetType())
                       .CanConvertTo(propertyInfo.PropertyType);
            
        }


        private TypeConverter GetConverterOf(Type type)
        {
            TypeConverter converter;
            if (this.converters.Keys.Contains(type))
            {
                converter = this.converters[type];
            }
            else
            {
                converter = TypeDescriptor.GetConverter(type);
                this.converters.Add(type, converter);
            }
            return converter;
        }

    }
}
