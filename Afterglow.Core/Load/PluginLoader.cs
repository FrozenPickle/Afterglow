using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Collections;
using Afterglow.Core.Plugins;

namespace Afterglow.Core.Load
{
    public static class PluginLoader
    {
        public static DynamicPluginLoader Loader;
    }

    public class DynamicPluginLoader
    {

        static List<Type> _types = null;

        //Help found at http://blogs.msdn.com/b/abhinaba/archive/2005/11/14/492458.aspx
        public Type[] GetObjectsTypes<T>(string folder)
        {
            string[] files = Directory.GetFiles(folder, "*.dll");

            if (_types == null)
            {
                _types = new System.Collections.Generic.List<Type>();

                foreach (string file in files)
                {
                    try
                    {
                        Assembly assembly = Assembly.LoadFile(file);

                        var assemblyTypes = (from assemblyType in assembly.GetTypes()
                                             where assemblyType.IsClass && assemblyType.IsPublic &&
                                                 (from i in assemblyType.GetInterfaces()
                                                  where i == typeof(T)
                                                  select i).Any()
                                             select assemblyType).ToArray();

                        if (assemblyTypes != null && assemblyTypes.Any())
                        {
                            _types.AddRange(assemblyTypes);
                        }
                    }
                    catch (Exception)
                    {
                        //LogError(ex);
                    }
                }
            }
            return _types.ToArray();
        }

        public Type GetObjectType(string typeName)
        {
            System.Type type = System.Type.GetType(typeName);

            if (type == null)
            {
                Type[] plugins = GetPlugins<IAfterglowPlugin>();

                //Get Exact match (fully qualified)
                var exactTypes = from t in plugins
                                 where t.FullName == typeName
                                 select t;

                type = exactTypes.FirstOrDefault();

                //If Exact match fails get best match (object name only)
                if (type == null)
                {
                    var bestTypes = from t in plugins
                                    where t.FullName.EndsWith(typeName)
                                    select t;

                    type = bestTypes.FirstOrDefault();
                }

            }

            return type;
        }
        
        public Type[] GetPlugins<T>()
        {
            string folder = Path.Combine(Environment.CurrentDirectory, "Plugins");

            Type[] list = GetObjectsTypes<T>(folder);
            return list;
        }

    }
}

