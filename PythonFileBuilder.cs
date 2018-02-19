using System;
using System.Text;

namespace Motio.CSharp2Py
{
    public class PythonFileBuilder
    {
        private string currentIndentation = "";
        /// <summary>
        /// an indent (4 spaces by default)
        /// </summary>
        private string indent = "    ";
        /// <summary>
        /// new line
        /// </summary>
        private string nl = Environment.NewLine;

        private StringBuilder builder = new StringBuilder();

        public void Append(string str)
        {
            builder.Append(str);
        }

        public void LineBreak()
        {
            builder.Append(nl);
            builder.Append(currentIndentation);
        }

        public void AddIndent()
        {
            currentIndentation += indent;
        }

        public void RemoveIndent()
        {
            currentIndentation = currentIndentation.Substring(0, currentIndentation.Length - indent.Length);
        }

        public override string ToString()
        {
            return builder.ToString();
        }
    }
}
