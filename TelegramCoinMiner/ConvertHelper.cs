using System;
using System.Collections.Generic;
using System.Linq;

namespace TelegramCoinMiner
{
    public static class ConvertHelper
    {
        public static T[] GetArrayFromObjectList<T>(object obj)
        {
            return ((IEnumerable<object>)obj)
                .Cast<T>()
                .ToArray();
        }

        public static List<T> GetListFromObjectList<T>(object obj)
        {
            return ((IEnumerable<object>)obj)
                .Cast<T>()
                .ToList();
        }

        public static T ToTypedVariable<T>(object obj)
        {
            if (obj == null)
            {
                dynamic dynamicResult = null;
                return dynamicResult;
            }

            Type type = typeof(T);
            if (type.IsArray)
            {
                dynamic dynamicResult = typeof(ConvertHelper).GetMethod(nameof(GetArrayFromObjectList))
                    .MakeGenericMethod(type.GetElementType())
                    .Invoke(null, new[] { obj });
                return dynamicResult;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                dynamic dynamicResult = typeof(ConvertHelper).GetMethod(nameof(GetListFromObjectList))
                    .MakeGenericMethod(type.GetGenericArguments().Single())
                    .Invoke(null, new[] { obj });
                return dynamicResult;
            }

            return (T)obj;
        }
    }
}
