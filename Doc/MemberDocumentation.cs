using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace Motio.CSharp2Py.Doc
{
    /// <summary>
    /// documentation concerning a member
    /// </summary>
    public class MemberDocumentation
    {
        //https://docs.microsoft.com/en-us/dotnet/csharp/codedoc 
        //summary remarks return exceptions params typeparams value(like return but for props) example 
        //to html: code c para see seealso paramref typeparamref list/item

        private readonly XElement element;

        public string Summary { get; private set; } = "";
        public string Remarks { get; private set; } = "";
        public string Returns { get; private set; } = "";
        public string Value { get; private set; } = "";
        public string Example { get; private set; } = "";
        public Dictionary<string, string> Params { get; private set; } = new Dictionary<string, string>();
        public Dictionary<string, string> TypeParams { get; private set; } = new Dictionary<string, string>();
        public Dictionary<string, string> Exceptions { get; private set; } = new Dictionary<string, string>();
        public string[] SeeAlso { get; private set; } = new string[0];

        public MemberDocumentation(XElement element)
        {
            List<string> seeAlso = new List<string>();

            this.element = element;
            foreach (XElement node in element.Descendants())
            {
                string paramName;
                XmlReader reader = node.CreateReader();
                reader.MoveToContent();
                switch (node.Name.ToString())
                {
                    case "summary":
                        Check(Summary, "summary");
                        Summary = reader.ReadInnerXml();
                        break;
                    case "remarks":
                        Check(Remarks, "remarks");
                        Remarks = reader.ReadInnerXml();
                        break;
                    case "returns":
                        Check(Returns, "returns");
                        Returns = reader.ReadInnerXml();
                        break;
                    case "value":
                        Check(Value, "value");
                        Value = reader.ReadInnerXml();
                        break;
                    case "example":
                        Check(Example, "example");
                        Example = reader.ReadInnerXml();
                        break;
                    case "param":
                        paramName = node.GetMemberName();
                        if (Params.ContainsKey(paramName))
                        {
                            Console.WriteLine("found more than one param description for " + paramName + " in " + element.GetMemberName());
                            break;
                        }
                        Params.Add(paramName, reader.ReadInnerXml());
                        break;
                    case "typeparam":
                        paramName = node.GetMemberName();
                        if (TypeParams.ContainsKey(paramName))
                        {
                            Console.WriteLine("found more than one typeparam description for " + paramName + " in " + element.GetMemberName());
                            break;
                        }
                        TypeParams.Add(paramName, reader.ReadInnerXml());
                        break;
                    case "exception":
                        paramName = node.Find("cref").Value;
                        if (paramName.StartsWith("T:"))
                            paramName = paramName.Substring(2);
                        if (Exceptions.ContainsKey(paramName))
                        {
                            Console.WriteLine("found more than one exception description for " + paramName + " in " + element.GetMemberName());
                            break;
                        }
                        Exceptions.Add(paramName, reader.ReadInnerXml());
                        break;
                    case "seealso":
                        paramName = node.Find("cref").Value;
                        seeAlso.Add(paramName);
                        break;
                }
            }
            SeeAlso = seeAlso.ToArray();
        }

        private void Check(string member, string name)
        {
            //commented out for speed
            //if (!string.IsNullOrEmpty(member))
            //    Console.WriteLine("found more than one " + name + " tag in " + element.GetMemberName());
        }
    }
}
