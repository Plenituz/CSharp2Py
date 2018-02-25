namespace Motio.CSharp2Py
{
    public abstract class PythonMember
    {
        protected PythonClass parent;

        public PythonMember(PythonClass parent)
        {
            this.parent = parent;
        }

        public abstract void StringRepr(PythonFileBuilder builder);
    }
}
