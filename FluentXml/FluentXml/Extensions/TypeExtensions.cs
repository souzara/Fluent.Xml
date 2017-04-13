using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fluent.Xml.Extensions
{
    public static class TypeExtensions
    {

        public static bool IsEnumerable(this Type type)
        {
            return typeof(IEnumerable).IsAssignableFrom(type);
        }

        public static Type GetGenericArgument(this Type type)
        {

            if (type.IsEnumerable())
                return type.GetGenericArguments().FirstOrDefault();
            return null;
        }
        public static IList<object> CreateEnumerable(this Type type)
        {
            var listType = typeof(List<>);
            var genericType = listType.MakeGenericType(type);

            return ((IEnumerable<object>)Activator.CreateInstance(genericType)).ToList();
        }
    }
}
