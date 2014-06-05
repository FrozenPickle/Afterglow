using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Afterglow.Core.IO
{
    /// <summary>
    /// Generic list Deserializer
    /// </summary>
    public class GenericListDeSerializer<T>
    {
        /// <summary>
        /// Serializers collection stored with type name
        /// </summary>
        private Dictionary<string, XmlSerializer> serializers;

        /// <summary>
        /// Creates an instance of GenericListDeSerializer
        /// </summary>
        public GenericListDeSerializer(Type[] extraTypes = null)
        {
            serializers = SerializationHelper.GetSerializers;
        }

        /// <summary>
        /// Deserializes list
        /// </summary>
        /// <param name="outputStream">Ouput stream to write the serialized data</param>
        public void Deserialize(System.Xml.XmlReader inputStream, List<T> interfaceList)
        {
            //Get the depth of the initial generic list node
            int parentDepth = inputStream.Depth;

            //check for empty element if empty then return
            if (inputStream.IsEmptyElement) return;

            //Move to first child
            inputStream.Read();
            //loop through child nodes until you reach the parent depth
            while (inputStream.Depth > parentDepth)
            {
                //read the input stream name
                string assemblyQualifiedName = inputStream.Name;

                XmlSerializer slzr = null;
                
                if (!serializers.TryGetValue(assemblyQualifiedName, out slzr))
                {
                    AfterglowRuntime.Logger.Fatal("Could not deserialize plugin of type: {0}", assemblyQualifiedName);
                    throw new Exception();
                }

                object item = slzr.Deserialize(inputStream);
                interfaceList.Add((T)item);

                //read next node if it is an end element
                if (inputStream.NodeType == XmlNodeType.EndElement) inputStream.Read();
            }
        }
    }
}
