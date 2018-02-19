using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Motio.CSharp2Py
{
    public class HierarchyBuilder
    {
        Dictionary<string, List<PythonClass>> namespaces = new Dictionary<string, List<PythonClass>>();

        public string BaseDir { get; set; } = "";

        public void Add(PythonClass pyClass)
        {
            List<PythonClass> classes;
            if (pyClass.Namespace == null)
                return;
            if (!namespaces.TryGetValue(pyClass.Namespace, out classes))
            {
                classes = new List<PythonClass>();
                namespaces.Add(pyClass.Namespace, classes);
            }
            classes.Add(pyClass);
        }

        public void CreateDirectories()
        {
            foreach (var pair in namespaces)
            {
                string path = Path.Combine(BaseDir, Path.Combine(pair.Key.Split('.').ToArray()));
                Directory.CreateDirectory(path);

                //create __init__.py file in all sub directories so they are concidered modules
                pair.Key.Split('.').Aggregate((p, n) =>
                {
                    string subDirPath = Path.Combine(BaseDir, p);
                    subDirPath = Path.Combine(subDirPath, "__init__.py");
                    if (!File.Exists(subDirPath))
                        File.WriteAllText(subDirPath, "");
                    return Path.Combine(p, n);
                });
            }
        }

        public void CreateFiles()
        {
            foreach (var pair in namespaces)
            {
                string path = Path.Combine(BaseDir, Path.Combine(pair.Key.Split('.').ToArray()), "__init__.py");
                StringBuilder content = new StringBuilder();

                foreach (PythonClass pyClass in pair.Value)
                {
                    content.Append(pyClass.StringRepr());
                    content.Append(Environment.NewLine);
                }
                try
                {
                    File.WriteAllText(path, content.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("couldn't write file " + path + ex);
                }
            }
        }

    }
}
