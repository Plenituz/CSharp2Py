using System.Reflection;

namespace Motio.CSharp2Py
{
    public class PythonEvent : PythonMember
    {
        protected readonly EventInfo ev;

        public PythonEvent(EventInfo ev)
        {
            this.ev = ev;
        }

        public override void StringRepr(PythonFileBuilder builder)
        {
            builder.Append($"self.{ev.Name} = None");
        }
    }
}
