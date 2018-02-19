using System.Reflection;

namespace Motio.CSharp2Py
{
    public class PythonStaticField : PythonField
    {
        public PythonStaticField(PropertyInfo property) : base(property)
        {
        }

        public PythonStaticField(FieldInfo field) : base(field)
        {
        }

        public override void StringRepr(PythonFileBuilder builder)
        {
            builder.Append(name + " = None");
        }
    }
}
