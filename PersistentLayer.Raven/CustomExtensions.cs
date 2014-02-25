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
        private static readonly HashSet<Type> NumericTypes;
        private static readonly HashSet<Type> NullableNumericTypes;
        private static readonly HashSet<Type> FloatingNumericTypes;

        /// <summary>
        /// 
        /// </summary>
        static CustomExtensions()
        {
            NumericTypes = new HashSet<Type>
                {
                    typeof(Byte),
                    typeof(SByte),
                    typeof(Int16),
                    typeof(UInt16),
                    typeof(Int32),
                    typeof(UInt32),
                    typeof(Int64),
                    typeof(UInt64),
                    typeof(Decimal),
                    typeof(Single),
                    typeof(Double)
                };

            NullableNumericTypes = new HashSet<Type>
                {
                    typeof(Byte?),
                    typeof(SByte?),
                    typeof(Int16?),
                    typeof(UInt16?),
                    typeof(Int32?),
                    typeof(UInt32?),
                    typeof(Int64?),
                    typeof(UInt64?),
                    typeof(Decimal?),
                    typeof(Single?),
                    typeof(Double?)
                };

            FloatingNumericTypes = new HashSet<Type>
                {
                    typeof(Single),
                    typeof(Double)
                };
        }

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
        public static bool IsFloatingNumericType(this Type type)
        {
            return FloatingNumericTypes.Contains(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullable(this Type type)
        {
            //return type.IsClass || type.IsInterface || type.IsAbstract || type.IsEquivalentTo()
            return type.IsClass || type.IsInterface || type.IsAbstract || type.GetInterface("Nullable`1", true) != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsReferenceType(this Type type)
        {
            return type.IsClass || type.IsInterface || type.IsAbstract;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNumeric(this Type type)
        {
            return NumericTypes.Contains(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullableNumericType(this Type type)
        {
            return NullableNumericTypes.Contains(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static bool Implements(this Type type, string namingInterface)
        {
            return type.GetInterface(namingInterface, true) != null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsNullableValueType(this Type type)
        {
            return type.GetInterface("Nullable`1", true) != null;
        }
    }
}
