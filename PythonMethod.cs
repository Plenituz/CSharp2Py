using Motio.CSharp2Py.Doc;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Motio.CSharp2Py
{
    public class PythonMethod : PythonMember
    {
        protected readonly MethodInfo method;

        public PythonMethod(PythonClass parent, MethodInfo method) : base(parent)
        {
            this.method = method;
        }

        protected virtual string Args()
        {
            var par = method.GetParameters().Select(p => p.Name).ToList();
            par.Insert(0, "self");
            return String.Join(", ", par);
        }

        protected virtual string GenericArguments()
        {
            if (method.GetGenericArguments().Length == 0)
                return "";
            return $"[{String.Join(", ", method.GetGenericArguments().Select((m) => m.Namespace + "." + Utils.CleanupNamePython(m)).ToArray())}]";
        }

        public override void StringRepr(PythonFileBuilder builder)
        {
            MemberDocumentation doc = parent.xmlDoc?.GetDocs(method);

            builder.Append($"def {method.Name}{GenericArguments()}({Args()}):");
            builder.AddIndent();
            builder.LineBreak();
            //generate doc string for type hints

            if(doc != null || method.GetParameters().Length != 0)
            {
                builder.Append("\"\"\"");
                builder.LineBreak();
            }   

            if (doc != null)
            {
                MemberDocExporter exporter = new PythonDocCommentExporter(doc);
                
                string docstring = exporter.Export();
                string line;
                StringReader reader = new StringReader(docstring);
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Trim();
                    builder.Append(line);
                    builder.LineBreak();
                }
            }
            if (method.GetParameters().Length != 0)
            {
                foreach (ParameterInfo param in method.GetParameters())
                {
                    Type paramType = param.ParameterType;
                    builder.Append($":type {param.Name}: {paramType.Namespace}.{Utils.CleanupNamePython(paramType)}");
                    builder.LineBreak();
                }
                if (method.ReturnType != typeof(void))
                {
                    builder.Append($":rtype: {method.ReturnType.Namespace}.{Utils.CleanupNamePython(method.ReturnType)}");
                    builder.LineBreak();
                }
            }

            if (doc != null || method.GetParameters().Length != 0)
            {
                builder.Append("\"\"\"");
                builder.LineBreak();
            }

            builder.Append("pass");
            builder.RemoveIndent();
        }
    }
}
