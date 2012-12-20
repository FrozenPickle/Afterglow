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
    /// <summary>
    /// Holds the a Dynamic Plugin Loader object
    /// </summary>
    public static class PluginLoader
    {
        private static DynamicPluginLoader _loader;
        /// <summary>
        /// Gets a DynamicPluginLoader object
        /// </summary>
        public static DynamicPluginLoader Loader 
        {
            get
            {
                if (_loader == null)
                {
                    _loader = new DynamicPluginLoader();
                }
                return _loader;
            }
        }
    }

    /// <summary>
    /// Loads all IAfterglowPlugin's from the applicaion folder \Plugins
    /// </summary>
    public class DynamicPluginLoader
    {
        /// <summary>
        /// All Loaded object Types
        /// </summary>
        static List<Type> _types = null;

        //Help found at http://blogs.msdn.com/b/abhinaba/archive/2005/11/14/492458.aspx

        /// <summary>
        /// Gets all Object Types from a specific folder by loading all dll's
        /// </summary>
        /// <typeparam name="T">The type of Object to return</typeparam>
        /// <param name="folder">The folder path</param>
        /// <returns>An array of object types</returns>
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

        /// <summary>
        /// Gets an IAfterglowPlugin object based on the type name, trys a few different methods
        /// </summary>
        /// <param name="typeName">The object Type Name</param>
        /// <returns>An object type</returns>
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
        
        /// <summary>
        /// Gets all objects of a specific Type
        /// </summary>
        /// <typeparam name="T">Object type</typeparam>
        /// <returns>An array of object types</returns>
        public Type[] GetPlugins<T>()
        {
            string folder = Path.Combine(Environment.CurrentDirectory, "Plugins");

            Type[] list = GetObjectsTypes<T>(folder);
            return list;
        }

    }
}

