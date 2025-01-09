using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

public static class ReflectiveEnumerator {
    static ReflectiveEnumerator() { }

    public static IEnumerable<T> GetEnumerableOfType<T>(params object[] constructorArgs) where T : class {
        List<T> objects = new List<T>();
        Type[] listOfTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(domainAssembly => domainAssembly.GetTypes())
            .Where(type => typeof(T).IsAssignableFrom(type)).ToArray();

        foreach (Type type in listOfTypes) {
            DebugC.Instance.LogAlways(type.Name);
            objects.Add((T)Activator.CreateInstance(type));
        }
        DebugC.Instance.LogAlways("something 2");
        objects.Sort();
        return objects;
    }
}

/**
Assembly.GetAssembly(typeof(T)).GetTypes()
.Where(myType => myType.IsClass && myType.IsAbstract && myType.IsSubclassOf(typeof(T)))
        
*/