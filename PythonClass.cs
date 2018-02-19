using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Motio.CSharp2Py
{
    public class PythonClass
    {
        Type className;
        List<Type> parentClasses = new List<Type>();

        PythonConstructor ctr = new PythonConstructor();
        List<PythonEvent> events = new List<PythonEvent>();
        List<PythonField> fields = new List<PythonField>();
        List<PythonStaticField> staticFields = new List<PythonStaticField>();
        List<PythonMethod> methods = new List<PythonMethod>();
        List<PythonStaticMethod> staticMethods = new List<PythonStaticMethod>();

        public string Namespace => className.Namespace;
        public string Name => CleanupName(className);

        public PythonClass(Type type)
        {
            className = type;
            if (type.BaseType != null)
                parentClasses.Add(type.BaseType);
            parentClasses.AddRange(type.GetInterfaces());

            foreach (MemberInfo member in type.GetMembers(BindingFlags.Static | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                Add(member, true);
            }

            foreach (MemberInfo member in type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly))
            {
                Add(member, false);
            }
        }

        public void Set(PythonConstructor constructor)
        {
            ctr = constructor;
        }

        public void Add(MemberInfo member, bool isStatic = false)
        {
            if (member is FieldInfo || member is PropertyInfo)
            {
                if (member is FieldInfo field)
                {
                    if (isStatic)
                        staticFields.Add(new PythonStaticField(field));
                    else
                        fields.Add(new PythonField(field));
                }
                if (member is PropertyInfo prop)
                {
                    if (isStatic)
                        staticFields.Add(new PythonStaticField(prop));
                    else
                        fields.Add(new PythonField(prop));
                }
            }
            else if (member is MethodInfo method)
            {
                if (!method.Name.StartsWith("set_")
                    && !method.Name.StartsWith("get_")
                    && !method.Name.StartsWith("add_")
                    && !method.Name.StartsWith("remove_"))
                {
                    if (isStatic)
                        staticMethods.Add(new PythonStaticMethod(method));
                    else
                        methods.Add(new PythonMethod(method));
                }
            }
            else if (member is ConstructorInfo ctr)
            {
                this.ctr = new PythonConstructor(ctr);
            }
            else if (member is EventInfo ev)
            {
                events.Add(new PythonEvent(ev));
            }
            else if (member is Type)
            {
                //TODO add child classes
                Console.WriteLine("child class found " + member);
            }
            else
            {
                throw new Exception("unsupported member " + member);
            }
        }

        public string StringRepr()
        {
            PythonFileBuilder builder = new PythonFileBuilder();

            //this creates an import line above each python class,
            //there might be duplicate import line after merging all the classes
            //in a single file but since it'll never never anyway
            Imports(builder);

            builder.Append($"class {CleanupName(className)}({ParentClasses()}):");
            builder.AddIndent();
            builder.LineBreak();

            StaticFields(builder);
            StaticMethods(builder);
            Constructor(builder);
            Methods(builder);

            return builder.ToString();
        }

        public void Imports(PythonFileBuilder builder)
        {
            Dictionary<string, HashSet<string>> namespaces = new Dictionary<string, HashSet<string>>();

            foreach (Type type in parentClasses)
            {
                string ns = type.Namespace;
                string name = CleanupName(type);

                HashSet<string> inNs;
                if (!namespaces.TryGetValue(ns, out inNs))
                {
                    inNs = new HashSet<string>();
                    namespaces.Add(ns, inNs);
                }
                inNs.Add(name);
            }

            foreach (var pair in namespaces)
            {
                builder.Append("from ");
                builder.Append(pair.Key);
                builder.Append(" import ");
                builder.Append(String.Join(", ", pair.Value));
                builder.LineBreak();
            }
        }

        public static string CleanupName(Type type)
        {
            string name = type.Name;
            if (!name.Contains("`"))
                return name;

            string[] split = name.Split(new char[] { '`' }, 2);
            name = split[0] + "[" + String.Join(", ", type.GetGenericArguments().Select(p => CleanupName(p)).ToArray()) + "]";
            return name;
        }

        public string ParentClasses()
        {
            return String.Join(", ", parentClasses.Select(p => CleanupName(p)));
        }

        public void StaticFields(PythonFileBuilder builder)
        {
            foreach (PythonStaticField field in staticFields)
            {
                field.StringRepr(builder);
                builder.LineBreak();
            }
            if(staticFields.Count != 0)
                builder.LineBreak();
        }

        public void StaticMethods(PythonFileBuilder builder)
        {
            foreach (PythonStaticMethod method in staticMethods)
            {
                method.StringRepr(builder);
                builder.LineBreak();
            }
            if(staticMethods.Count != 0)
                builder.LineBreak();
        }

        public void Constructor(PythonFileBuilder builder)
        {
            ctr.StringRepr(builder);
            builder.AddIndent();
            builder.LineBreak();

            if (fields.Count == 0 && events.Count == 0)
            {
                builder.Append("pass");
                builder.RemoveIndent();
                builder.LineBreak();
                return;
            }

            foreach (PythonField field in fields)
            {
                field.StringRepr(builder);
                builder.LineBreak();
            }
            foreach (PythonEvent ev in events)
            {
                ev.StringRepr(builder);
                builder.LineBreak();
            }
            builder.RemoveIndent();
            builder.LineBreak();
        }

        public void Methods(PythonFileBuilder builder)
        {
            foreach (PythonMethod method in methods)
            {
                method.StringRepr(builder);
                builder.LineBreak();
            }
        }
    }
}
