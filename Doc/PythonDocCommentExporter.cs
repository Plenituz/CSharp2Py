using System.Text;

namespace Motio.CSharp2Py.Doc
{
    public class PythonDocCommentExporter : MemberDocExporter
    {
        public PythonDocCommentExporter(MemberDocumentation doc) : base(doc)
        {
        }

        public override string Export()
        {
            StringBuilder builder = new StringBuilder();
            if(!string.IsNullOrEmpty(memberDoc.Summary))
            {
                builder.Append(ProcessNodes(memberDoc.Summary));
                builder.Append("\n\n");
            }
            if (!string.IsNullOrEmpty(memberDoc.Example))
            {
                builder.Append(":Example:\n\n");
                builder.Append(ProcessNodes(memberDoc.Example));
                builder.Append("\n\n");
            }
            if(!string.IsNullOrEmpty(memberDoc.Value))
            {
                builder.Append(":Value: ");
                builder.Append(ProcessNodes(memberDoc.Value));
                builder.Append("\n");
            }
            foreach(var pair in memberDoc.Params)
            {
                builder.Append(":param " + pair.Key + ": ");
                builder.Append(ProcessNodes(pair.Value));
                builder.Append("\n");
            }
            foreach(var pair in memberDoc.TypeParams)
            {
                builder.Append(":typeparam " + pair.Key + ": ");
                builder.Append(ProcessNodes(pair.Value));
                builder.Append("\n");
            }
            if (!string.IsNullOrEmpty(memberDoc.Returns))
            {
                builder.Append(":return: ");
                builder.Append(ProcessNodes(memberDoc.Returns));
                builder.Append("\n");
            }
            if (memberDoc.Exceptions.Count != 0)
            {
                builder.Append(":raises: ");
                builder.Append(ProcessNodes(string.Join(", ", memberDoc.Exceptions.Keys)));
            }
            if (!string.IsNullOrEmpty(memberDoc.Remarks))
            {
                builder.Append(".. note::\n");
                builder.Append(ProcessNodes(memberDoc.Remarks));
                builder.Append("\n\n");
            }
            if (memberDoc.SeeAlso.Length != 0)
            {
                builder.Append(".. seealso:: ");
                builder.Append(ProcessNodes(string.Join(", ", memberDoc.SeeAlso)));
            }
            return builder.ToString();
        }

        private string Sanitize(string name)
        {
            if (name[1] == ':')
                name = name.Substring(2);
            return name;
        }

        protected override string ReplaceCodeTag(XmlTag tag)
        {
            return "\n\n" + tag.content + "\n\n";
        }

        protected override string ReplaceCTag(XmlTag tag)
        {
            return tag.content;
        }

        protected override string ReplaceParamRefTag(XmlTag tag)
        {
            return ":paramref:`" + Sanitize(tag.attrs["name"]) + "`";
        }

        protected override string ReplaceParaTag(XmlTag tag)
        {
            return "\n\n" + tag.content + "\n\n";
        }

        protected override string ReplaceSeeTag(XmlTag tag)
        {
            return ":see:`" + Sanitize(tag.attrs["cref"]) + "`";
        }

        protected override string ReplaceTypeParamRefTag(XmlTag tag)
        {
            return Sanitize(tag.attrs["name"]);
        }
    }
}
