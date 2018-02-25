using System.Reflection;

namespace Motio.CSharp2Py
{
    public class PythonEvent : PythonMember
    {
        protected readonly EventInfo ev;

        public PythonEvent(PythonClass parent, EventInfo ev) : base(parent)
        {
            this.ev = ev;
        }

        public override void StringRepr(PythonFileBuilder builder)
        {
            builder.Append($"self.{ev.Name} = None");
        }
    }
}
