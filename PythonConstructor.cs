using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Motio.CSharp2Py
{
    public class PythonConstructor : PythonMember
    {
        protected readonly ConstructorInfo ctr;

        public PythonConstructor(PythonClass parent, ConstructorInfo constructor) : base(parent)
        {
            this.ctr = constructor;
        }

        public PythonConstructor(PythonClass parent) : base(parent)
        {

        }

        protected virtual string Args()
        {
            var par = ctr?.GetParameters().Select(p => p.Name).ToList() ?? new List<string>();
            par.Insert(0, "self");
            return String.Join(", ", par);
        }

        public override void StringRepr(PythonFileBuilder builder)
        {
            builder.Append($"def __init__({Args()}):");
        }
    }
}
