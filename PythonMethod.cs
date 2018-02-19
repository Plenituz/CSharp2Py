using System;
using System.Linq;
using System.Reflection;

namespace Motio.CSharp2Py
{
    public class PythonMethod : PythonMember
    {
        protected readonly MethodInfo method;

        public PythonMethod(MethodInfo method)
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
            return $"[{String.Join(", ", method.GetGenericArguments().Select((m) => m.Namespace + "." + PythonClass.CleanupName(m)).ToArray())}]";
        }

        public override void StringRepr(PythonFileBuilder builder)
        {
            builder.Append($"def {method.Name}{GenericArguments()}({Args()}):");
            builder.AddIndent();
            builder.LineBreak();
            //generate doc string for type hints
            if(method.GetParameters().Length != 0)
            {
                builder.Append("\"\"\"");
                builder.LineBreak();

                foreach (ParameterInfo param in method.GetParameters())
                {
                    Type paramType = param.ParameterType;
                    builder.Append($":type {param.Name}: {paramType.Namespace}.{PythonClass.CleanupName(paramType)}");
                    builder.LineBreak();
                }
                if (method.ReturnType != typeof(void))
                {
                    builder.Append($":rtype: {method.ReturnType.Namespace}.{PythonClass.CleanupName(method.ReturnType)}");
                    builder.LineBreak();
                }

                builder.Append("\"\"\"");
                builder.LineBreak();
            }
            
            builder.Append("pass");
            builder.RemoveIndent();
        }
    }
}
