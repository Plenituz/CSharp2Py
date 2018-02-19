using System.Reflection;

namespace Motio.CSharp2Py
{
    public class PythonField : PythonMember
    {
        protected readonly string name;

        public PythonField(PropertyInfo property)
        {
            this.name = property.Name;
        }

        public PythonField(FieldInfo field)
        {
            this.name = field.Name;
        }

        public override void StringRepr(PythonFileBuilder builder)
        {
            builder.Append($"self.{name} = None");
        }
    }
}
