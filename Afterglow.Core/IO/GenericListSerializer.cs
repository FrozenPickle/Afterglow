using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace Afterglow.Core.IO
{
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
        public GenericListSerializer(List<T> interfaceList, Type[] extraTypes = null)
        {
            _interfaceList = interfaceList;

            serializers = SerializationHelper.GetSerializers;
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

            //Return the retrived XmlSerializer
            return returnSerializer;
        }
    }
}
