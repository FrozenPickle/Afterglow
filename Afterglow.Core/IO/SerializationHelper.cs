using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Afterglow.Core.IO
{
    public static class SerializationHelper
    {
        public static Dictionary<string, XmlSerializer> GetSerializers
        {
            get
            {
                //Gets all possibly used IAfterglowPlugins
                Type[] extraTypes = PluginLoader.Loader.AllPlugins;

                Dictionary<string, XmlSerializer> serializers = new Dictionary<string, XmlSerializer>();

                if (extraTypes != null)
                {
                    foreach (Type type in extraTypes)
                    {
                        XmlSerializer returnSerializer = new XmlSerializer(type);
                        serializers.Add(type.Name, returnSerializer);
                        serializers.Add(type.FullName, returnSerializer);
                    }
                }
                return serializers;
            }
        }
    }
}
