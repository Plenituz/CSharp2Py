using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace Motio.CSharp2Py.Doc
{
    public class DocParser
    {
        Dictionary<string, MemberDocumentation> docs = new Dictionary<string, MemberDocumentation>();

        public DocParser(XDocument xml)
        {
            foreach(XElement element in xml.Descendants())
            {
                if (element.Name.ToString().Equals("member"))
                {
                    string name = element.GetMemberName();
                    docs.Add(name, new MemberDocumentation(element));
                }
            }
        }

        public MemberDocumentation GetDocs(MemberInfo member)
        {
            string id = DocId(member);
            if (docs.TryGetValue(id, out var doc))
            {
                return doc;
            }
            Console.WriteLine("couldn't find documentation for member " + id);
            return null;
        }

        /// <summary>
        /// create the xml name of this member
        /// </summary>
        /// <param name="member"></param>
        /// <returns></returns>
        public string DocId(MemberInfo member)
        {
            string typeStr = MemberTypeString(member);
            string fullName = FullName(member);
            return typeStr + ":" + fullName;
        }

        private string ListArguments(MethodBase method)
        {
            return string.Join(",", method.GetParameters().Select(p => Utils.CleanupNameCSharp(p.ParameterType) + ExtraArgument(p)));
        }

        private string ExtraArgument(ParameterInfo param)
        {
            if (param.IsOut || param.ParameterType.IsByRef)
                return "@";
            return "";
        }

        private string ListTypeArguments(Type type)
        {
            return string.Join(",", type.GetGenericArguments().Select(t => Utils.CleanupNameCSharp(t)));
        }

        private string FullName(MemberInfo member)
        {
            Type parent = member.DeclaringType;
            string baseName = $"{parent.Namespace}.{parent.Name}.{member.Name}";

            if (member is FieldInfo || member is PropertyInfo || member is EventInfo)
            {
                return baseName;
            }
            else if (member is MethodInfo)
            {
                return baseName + "(" + ListArguments((MethodBase)member) + ")";
            }
            else if(member is ConstructorInfo)
            {
                return baseName + ".#ctor(" + ListArguments((MethodBase)member) + ")";

            }
            else if (member is Type type)
            {
                if (type.ContainsGenericParameters)
                    baseName = Utils.CleanupNameCSharp(type);
                    //baseName += "{" + ListTypeArguments(type) + "}";
                return baseName;
            }
            else
            {
                throw new Exception("unsupported member " + member);
            }
        }

        private string MemberTypeString(MemberInfo member)
        {
            // T=type, M=method/constructor, P=property, E=event, F=field
            if (member is FieldInfo)
            {
                return "F";
            }
            else if(member is PropertyInfo)
            {
                return "P";
            }
            else if (member is MethodInfo || member is ConstructorInfo)
            {
                return "M";
            }
            else if (member is EventInfo)
            {
                return "E";
            }
            else if (member is Type)
            {
                return "T";
            }
            else
            {
                throw new Exception("unsupported member " + member);
            }
        }
    }
}
