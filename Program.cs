using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Motio.CSharp2Py
{
    public class Program
    {
        static void Main(string[] args)
        {
            if(args.Length < 2)
            {
                Console.WriteLine("This CLI is used to read a folder (and subfolder) full of .dll files and create python interfaces from the type found in each dll");
                Console.WriteLine("Usage:");
                Console.WriteLine("CSharp2Py [directory with dlls] [output directory] [regex match for each dll file]");
                return;
            }
            string dir = args[0];
            HierarchyBuilder hierarchy = new HierarchyBuilder()
            {
                BaseDir = args[1]
            };
            string arg = args.Length > 2 ? args[2] : "*";
            Regex regex = new Regex(arg);

            foreach(string dll in Directory.GetFiles(dir, "*.dll"))
            {
                if(!regex.IsMatch(Path.GetFileName(dll)))
                    continue;
                Assembly assembly = Assembly.LoadFrom(dll);//typeof(int).Assembly;
                foreach (Type type in assembly.GetTypes())
                {
                    hierarchy.Add(new PythonClass(type));
                }
                //break;
            }

            hierarchy.CreateDirectories();
            hierarchy.CreateFiles();
        }
    }
}
