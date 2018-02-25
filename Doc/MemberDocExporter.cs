using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Motio.CSharp2Py.Doc
{
    public abstract class MemberDocExporter
    {
        protected readonly MemberDocumentation memberDoc;
        private static Regex inlineTagRegex = new Regex("<(\\w+)(?:\\s*(\\w+)=\"([^\"]*?)\"\\s*)*\\/>", RegexOptions.Compiled);
        private static Regex fullTagRegex = new Regex("<(\\w+)(?:\\s*(\\w+)=\"([^\"]*?)\"\\s*)*>(.*?)<\\/\\1>", RegexOptions.Compiled | RegexOptions.Singleline);

        public MemberDocExporter(MemberDocumentation doc)
        {
            this.memberDoc = doc;
        }

        public abstract string Export();

        /// <summary>
        /// replace all the xml doc nodes by their equivalent in the current exporter
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public virtual string ProcessNodes(string str)
        {
            str = ReplaceInlineNodes(str);
            str = ReplaceContentNodes(str);
            return str;
        }

        private XmlTag CreateTag(Match match)
        {
            //groups[1] = tag name
            //groups[2] = attr name
            //groups[3] = attr value
            Group nameGroup = match.Groups[1];
            Group attrGroup = match.Groups[2];
            Group valueGroup = match.Groups[3];

            if (attrGroup.Captures.Count != valueGroup.Captures.Count)
                throw new System.Exception("desync of attr/value pairs");

            Dictionary<string, string> attrs = new Dictionary<string, string>();
            for (int k = 0; k < attrGroup.Captures.Count; k++)
            {
                attrs.Add(attrGroup.Captures[k].Value, valueGroup.Captures[k].Value);
            }

            XmlTag tag = new XmlTag(nameGroup.Value, attrs);

            if(match.Groups.Count > 4)
            {
                Group contentGroup = match.Groups[4];
                tag.content = contentGroup.Value;
            }
            return tag;
        }

        protected virtual string ReplaceContentNodes(string xml)
        {
            return fullTagRegex.Replace(xml, RegexMatcher);
        }

        protected virtual string ReplaceInlineNodes(string xml)
        {
            return inlineTagRegex.Replace(xml, RegexMatcher);
        }

        private string RegexMatcher(Match match)
        {
            XmlTag tag = CreateTag(match);
            return ReplaceTag(tag);
        }

        protected virtual string ReplaceTag(XmlTag tag)
        {
            //code c para see seealso paramref typeparamref list/item
            switch (tag.name)
            {
                case "c":
                    return ReplaceCTag(tag);
                case "code":
                    return ReplaceCodeTag(tag);
                case "para":
                    return ReplaceParaTag(tag);
                case "see":
                    return ReplaceSeeTag(tag);
                case "paramref":
                    return ReplaceParamRefTag(tag);
                case "typeparamref":
                    return ReplaceTypeParamRefTag(tag);
                default:
                    System.Console.WriteLine("unexpected inline tag:" + tag.name);
                    break;
            }
            return "<" + tag.name + "/>";
        }

        protected abstract string ReplaceCTag(XmlTag tag);
        protected abstract string ReplaceCodeTag(XmlTag tag);
        protected abstract string ReplaceParaTag(XmlTag tag);
        protected abstract string ReplaceSeeTag(XmlTag tag);
        protected abstract string ReplaceParamRefTag(XmlTag tag);
        protected abstract string ReplaceTypeParamRefTag(XmlTag tag);
    }
}
