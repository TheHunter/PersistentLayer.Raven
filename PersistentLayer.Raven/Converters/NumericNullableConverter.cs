using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Raven.Client.Converters;

namespace PersistentLayer.Raven.Converters
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class NumericNullableConverter<TValue>
        : ITypeConverter
        where TValue: struct
    {
        
        private readonly Type type;

        /// <summary>
        /// 
        /// </summary>
        public NumericNullableConverter()
        {
            this.type = typeof (TValue?);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceType"></param>
        /// <returns></returns>
        public bool CanConvertFrom(Type sourceType)
        {
            //return this.type == sourceType;
            return this.type.IsAssignableFrom(sourceType);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="value"></param>
        /// <param name="allowNull"></param>
        /// <returns></returns>
        public string ConvertFrom(string tag, object value, bool allowNull)
        {
            if (value == null)
                return null;

            Type valType = value.GetType();

            if (!this.type.IsAssignableFrom(valType) || default(TValue).Equals(value))
                return null;

            return tag + value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public object ConvertTo(string value)
        {
            try
            {
                return Convert.ChangeType(value, typeof(TValue));
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
