using System;
using System.Linq;
using System.Reflection;

namespace Motio.CSharp2Py
{
    public class PythonStaticMethod : PythonMethod
    {
        public PythonStaticMethod(PythonClass parent, MethodInfo method) : base(parent, method)
        {
        }

        protected override string Args()
        {
            var par = method.GetParameters().Select(p => p.Name);
            return String.Join(", ", par);
        }

        public override void StringRepr(PythonFileBuilder builder)
        {
            builder.Append("@staticmethod");
            builder.LineBreak();
            base.StringRepr(builder);
        }
    }
}
