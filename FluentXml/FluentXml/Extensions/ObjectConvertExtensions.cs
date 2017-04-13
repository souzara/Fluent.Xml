using Fluent.Xml.Handlers;
using System;

namespace Fluent.Xml.Extensions
{
    public static class ObjectConvertExtensions
    {
        internal static event ResolveUnconvertibleTypeHandler resolveUnconvertibleType;

        public static object ChangeType(this object value, Type destinationType)
        {
            try
            {
                if (value == null)
                    return value;
                if (value.GetType() == destinationType)
                    return value;

                return Convert.ChangeType(value, destinationType);
            }
            catch (Exception ex)
            {
                if (resolveUnconvertibleType != null)
                    return resolveUnconvertibleType(new ResolveUnconvertibleTypeArgs(value, destinationType));
                throw new Exception($"Can't convert {value.GetType().FullName} to {destinationType.FullName}", ex);
            }
        }

    }
}
