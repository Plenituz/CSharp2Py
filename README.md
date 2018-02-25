# What?

Generate the equivalent python interface for a given C# dll/assembly.
If you generate the XML description of your dll while compiling, it will be taken in account when creating the python class.

For example if you have the following C# class

```c#
namespace Motio.Meshing
{
	using EdgeTrain = System.Collections.Generic.IList<int>;

    public struct EdgeSet
    {
        public int indexFirst, indexSecond;

        public EdgeSet(int indexFirst, int indexSecond)
        {
            this.indexFirst = indexFirst;
            this.indexSecond = indexSecond;
        }

        public bool Contains(int index)
        {
            ...
        }

        public int Other(int index)
        {
            ...
        }

        public static IList<EdgeTrain> UnfoldAllEdges(HashSet<EdgeSet> edges)
        {
            ...
        }

        public static EdgeTrain UnfoldEdges(HashSet<EdgeSet> edges)
        {
            ...
        }
    }
}
```

A folder structure will be created to emulate the namespaces
```
Motio/
├── Meshing/
│   └── __init__.py
└── __init__.py
```

`Motio/__init__.py` will be empty but `Motio/Meshing/__init__.py` will contain the following class definition

```python
from System import ValueType
class EdgeSet(ValueType):
    @staticmethod
    def UnfoldAllEdges(edges):
        """
        :type edges: System.Collections.Generic.HashSet[EdgeSet]
        :rtype: System.Collections.Generic.IList[IList[Int32]]
        """
        pass
    @staticmethod
    def UnfoldEdges(edges):
        """
        :type edges: System.Collections.Generic.HashSet[EdgeSet]
        :rtype: System.Collections.Generic.IList[Int32]
        """
        pass
    
    def __init__(self, indexFirst, indexSecond):
        self.indexFirst = None
        self.indexSecond = None
        
    def Contains(self, index):
        """
        :type index: System.Int32
        :rtype: System.Boolean
        """
        pass
    def Other(self, index):
        """
        :type index: System.Int32
        :rtype: System.Int32
        """
        pass
```

The idea is to drop the generated folders in your IDE python path.
The main use is to have your python IDE's auto complete work when you are working on the python side of an IronPython project.
This has only been tested on PyCharm, so feel free to send a PR or ask if you need a change for it to work on your IDE.

This is a module extracted from [Water Motion](https://plenicorp.com) our motion graphics software.

# How?

To use it you have to build it yourself using Visual Studio. 
 - Open the project on Visual Studio
 - Go to Build>Build Solution (Ctrl+Shift+B)
 - Open a terminal on in the output directory
 - `.\Motio.CSharp2Py.exe [directory with dlls to read] [output directory] [filter regex on the dll name]`

Example:
```
                      Read from bin output of our project    output          only keep dlls that contain "Motio"
.\Motio.CSharp2Py.exe D:\C#\Motio2\Motio2\bin\Debug          D:\python_ref   "Motio"
```

Alternatively you can put the command line arguments in the project's `Properties` (in Debug>Command line arguments)

You can also point it directly to any Assembly known at runtime by changing the code. For example to generate the python
interfaces for the classes in the `System` namespace change these lines

```c#
Assembly assembly = Assembly.LoadFrom(dll);//typeof(int).Assembly;
foreach (Type type in assembly.GetTypes())
{
    hierarchy.Add(new PythonClass(type));
}
//break;
```

By

```c#
Assembly assembly = typeof(int).Assembly;
foreach (Type type in assembly.GetTypes())
{
    hierarchy.Add(new PythonClass(type));
}
break;
```
