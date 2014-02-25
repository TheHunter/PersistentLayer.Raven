using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using PersistentLayer.Exceptions;
using PersistentLayer.Raven;
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

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TDocument"></typeparam>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public string GetIdentifier<TDocument>(dynamic identifier)
        {
            Type docType = typeof(TDocument);
            var identifierProperty = this.store
                                         .Conventions
                                         .GetIdentityProperty(docType);

            if (identifier == null)
                throw new InvalidIdentifierException("The identifier value cannot be null");

            if (identifierProperty != null)
            {
                Type keyParamType = identifier.GetType();
                Type docIdType = identifierProperty.PropertyType;

                if (!docIdType.IsAssignableFrom(keyParamType))
                {
                    // only numerical identifiers can be accepted at this point.
                    if (!(keyParamType.IsNumeric() || keyParamType.IsNullableNumericType()))
                        throw new InvalidIdentifierException("The indentifier is not compatible with identifier property, so It's not possible to make any kind of conversion.");

                    var converter = this.GetConverterOf(keyParamType);
                    if (!converter.CanConvertTo(docIdType))
                        throw new InvalidIdentifierException("The indentifier cannot be converted into identifier property.");

                    dynamic converted = converter.ConvertTo(identifier, docIdType);

                    if (converted == null)
                        throw new InvalidIdentifierException("The indentifier is not compatible with identifier property document because any converter can convert its value into adequate value.");

                    dynamic res = identifier - converted;       // verify if original value was changed.
                    if (res != 0)
                        throw new InvalidIdentifierException("The indentifier was converted by a suitable converter, but Its original value was altered.");
                }

                if (identifier is string)
                {
                    if (string.IsNullOrWhiteSpace(identifier))
                        throw new InvalidIdentifierException("The identifier cannot be an empty string.");
                    return identifier;
                }
                
                return identifier.ToString();
            }

            if (!(identifier is string))
                throw new InvalidIdentifierException("The given identifier must be a string type because there's no identity property for the given document type.");
            return identifier;
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
