using System.Collections.Generic;

namespace Motio.CSharp2Py.Doc
{
    public class XmlTag
    {
        public string name;
        public Dictionary<string, string> attrs;
        public string content;

        public XmlTag(string name, Dictionary<string, string> attrs)
        {
            this.name = name;
            this.attrs = attrs;
        }
    }
}
