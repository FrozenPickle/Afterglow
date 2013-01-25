using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Afterglow.Core.Plugins;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using Afterglow.Core.Load;
using System.Collections;

//http://www.codeproject.com/Articles/38930/XML-Serialization-of-a-Class-Inherited-from-Generi

namespace Afterglow.Core
{
    /// <summary>
    /// A List that can be easily saved to XML
    /// </summary>
    /// <typeparam name="T">Type of objects in the list</typeparam>
    public class SerializableInterfaceList<T> : List<T>, IXmlSerializable
    {
        #region IXmlSerializable Members

        /// <summary>
        /// As per MSDN this does nothing
        /// </summary>
        /// <returns>null</returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader">The System.Xml.XmlReader stream from which the object is deserialized.</param>
        public void ReadXml(XmlReader reader)
        {
            GenericListDeSerializer<T> dslzr = new GenericListDeSerializer<T>();
            dslzr.Deserialize(reader, this);
        }
        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer">The System.Xml.XmlWriter stream to which the object is serialized.</param>
        public void WriteXml(XmlWriter writer)
        {
            GenericListSerializer<T> serializers = new GenericListSerializer<T>(this);
            serializers.Serialize(writer);
        }

        #endregion
    }

    /// <summary>
    /// Generic list serializer 
    /// </summary>
    public class GenericListSerializer<T>
    {
        /// <summary>
        /// To retain the serializer list that the T contains
        /// </summary>
        private List<T> _interfaceList;

        /// <summary>
        /// Serializers collection stored with type name
        /// </summary>
        private Dictionary<string, XmlSerializer> serializers;

        /// <summary/>
        /// Creates a new instance of generic list serializer 
        /// </summary/>
        /// <param name="interfaceList">Generic list of interface type</param>
        public GenericListSerializer(List<T> interfaceList)
        {
            _interfaceList = interfaceList;
            InitializeSerializers();
        }


        /// <summary>
        /// Initializes all the type of XML serializers the
        /// generic list requires 
        /// </summary>
        private void InitializeSerializers()
        {
            serializers = new Dictionary<string, XmlSerializer>();
            for (int index = 0; index < _interfaceList.Count; index++)
            {

                //Add new serializer to the serializers list by
                //passing the class name 
                //Ignore if already added
                if (_interfaceList[index] != null)
                    GetSerializerByTypeName(_interfaceList[index].GetType().FullName);
            }
        }

        /// <summary>
        /// Serializes list 
        /// </summary>
        /// <param name="outputStream">Ouput stream to write the serialized data</param>
        public void Serialize(XmlWriter outputStream)
        {
            for (int index = 0; index < _interfaceList.Count; index++)
            {
                //Get appropriate serializer
                if (_interfaceList[index] == null)
                    continue;
                GetSerializerByTypeName(
                                       _interfaceList[index].GetType().FullName).Serialize
                (outputStream, _interfaceList[index]);
            }
        }

        /// <summary>
        /// Gets serializer by type name from internal XML serializers list 
        /// If specific serializers doesn't exists adds it and returns it 
        /// </summary>
        /// <param name="typeName">Class type name</param>
        /// <returns>XmlSerializer</returns>
        private XmlSerializer GetSerializerByTypeName(string typeName)
        {
            XmlSerializer returnSerializer = null;

            //Check if existing already

            if (serializers.ContainsKey(typeName))
            {
                returnSerializer = serializers[typeName];
            }
            //If doesn't exist in list create a new one and add it to list
            if (returnSerializer == null)
            {
                returnSerializer = new XmlSerializer(PluginLoader.Loader.GetObjectType(typeName));
                serializers.Add(typeName, returnSerializer);
            }

            //Return the retrived XmlSerializer
            return returnSerializer;
        }
    }

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
        public GenericListDeSerializer()
        {
            serializers = new Dictionary<string, XmlSerializer>();
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

                XmlSerializer slzr = GetSerializerByTypeName(assemblyQualifiedName);
                object item = slzr.Deserialize(inputStream);
                interfaceList.Add((T)item);

                //read next node if it is an end element
                if (inputStream.NodeType == XmlNodeType.EndElement) inputStream.Read();
            }
        }

        /// <summary>
        /// Gets serializer by type name from internal XML serializers list
        /// If specific serializers doesn't exists adds it and returns it
        /// </summary>
        /// <param name="typeName">Class type name</param>
        /// <returns>XmlSerializer</returns>
        private XmlSerializer GetSerializerByTypeName(string typeName)
        {
            XmlSerializer returnSerializer = null;

            //Check if existing already
            if (serializers.ContainsKey(typeName))
            {
                returnSerializer = serializers[typeName];
            }

            //If doesn't exist in list create a new and add it to list
            if (returnSerializer == null)
            {
                Type type = PluginLoader.Loader.GetObjectType(typeName);
                returnSerializer = new XmlSerializer(type);
                serializers.Add(typeName, returnSerializer);
            }

            //Return the retrived XmlSerializer
            return returnSerializer;
        }
    }
}
