using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Motio.CSharp2Py
{
    public class Utils
    {
        public static string CleanupName(Type type, string separator, 
            string delimiterStart, string delimiterEnd, Func<Type, string, string> doReturn)
        {
            string name = type.Name;
            if (name.Contains("&"))
                name = name.Replace("&", "");
            if (!name.Contains("`"))
                return doReturn(type, name);

            string[] split = name.Split(new char[] { '`' }, 2);
            name = split[0] + delimiterStart 
                + String.Join(separator, type.GetGenericArguments().Select(p => CleanupName(p, separator, delimiterStart, delimiterEnd, doReturn))
                .ToArray()) + delimiterEnd;
            return name;
        }

        public static string CleanupNamePython(Type type)
        {
            return CleanupName(type, ", ", "[", "]", (t, s) => s);
        }

        public static string CleanupNameCSharp(Type type)
        {
            return CleanupName(type, ",", "{", "}", (t, s) => t.Namespace + "." + s);
        }

    }
}
