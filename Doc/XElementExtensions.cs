using System.Xml.Linq;

namespace Motio.CSharp2Py.Doc
{
    public static class XElementExtensions
    {
        public static string GetMemberName(this XElement self)
        {
            return self.Find("name")?.Value;
        }

        public static XAttribute Find(this XElement self, string attrName)
        {
            foreach (XAttribute attr in self.Attributes())
            {
                if (attr.Name.ToString().Equals(attrName))
                    return attr;
            }
            return null;
        }
    }
}
