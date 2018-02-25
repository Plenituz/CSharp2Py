using System.Reflection;

namespace Motio.CSharp2Py
{
    public class PythonStaticField : PythonField
    {
        public PythonStaticField(PythonClass parent, PropertyInfo property) : base(parent, property)
        {
        }

        public PythonStaticField(PythonClass parent, FieldInfo field) : base(parent, field)
        {
        }

        public override void StringRepr(PythonFileBuilder builder)
        {
            builder.Append(name + " = None");
        }
    }
}
