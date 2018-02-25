using System.Reflection;

namespace Motio.CSharp2Py
{
    public class PythonField : PythonMember
    {
        protected readonly string name;

        public PythonField(PythonClass parent, PropertyInfo property) : base(parent)
        {
            this.name = property.Name;
        }

        public PythonField(PythonClass parent, FieldInfo field) : base(parent)
        {
            this.name = field.Name;
        }

        public override void StringRepr(PythonFileBuilder builder)
        {
            builder.Append($"self.{name} = None");
        }
    }
}
