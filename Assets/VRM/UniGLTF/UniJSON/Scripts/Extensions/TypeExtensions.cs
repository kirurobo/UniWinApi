using System;
using System.Collections.Generic;


namespace UniJSON
{
    public static class TypeExtensions
    {
        public static bool GetIsGenericList(this Type t)
        {
            return t.IsGenericType && (t.GetGenericTypeDefinition() == typeof(List<>));
        }
    }
}
