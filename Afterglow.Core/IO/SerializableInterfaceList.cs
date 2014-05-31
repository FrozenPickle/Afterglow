using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Afterglow.Core.Plugins;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
using System.Collections;

//http://www.codeproject.com/Articles/38930/XML-Serialization-of-a-Class-Inherited-from-Generi

namespace Afterglow.Core.IO
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
}
