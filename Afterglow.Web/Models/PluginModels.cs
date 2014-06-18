using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Afterglow.Web.Models
{
    [DataContract]
    public class PluginRequest
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }
        [DataMember(Name = "profileId")]
        public int ProfileId { get; set; }
        [DataMember(Name = "pluginType")]
        public int PluginType { get; set; }
        [DataMember(Name = "type")]
        public string Type { get; set; }
    }
    [DataContract]
    public class PluginResponse
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }
        [DataMember(Name = "profileId")]
        public int ProfileId { get; set; }
        [DataMember(Name = "pluginType")]
        public int PluginType { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "description")]
        public string Description { get; set; }
        [DataMember(Name = "author")]
        public string Author { get; set; }
        [DataMember(Name = "version")]
        public Version Version { get; set; }
        [DataMember(Name = "website")]
        public string Website { get; set; }
        [DataMember(Name = "properties")]
        public IEnumerable<PluginProperty> Properties { get; set; }
        [DataMember(Name = "title")]
        public string Title { get; set; }
    }
    [DataContract]
    public class PluginProperty
    {
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "type")]
        public string Type { get; set; }
        [DataMember(Name = "required")]
        public bool Required { get; set; }
        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }
        [DataMember(Name = "description")]
        public string Description { get; set; }
        [DataMember(Name = "minValue")]
        public int? MinValue { get; set; }
        [DataMember(Name = "maxValue")]
        public int? MaxValue { get; set; }
        [DataMember(Name = "value")]
        public object Value { get; set; }
        [DataMember(Name = "options")]
        public IEnumerable<SelectOption> Options { get; set; }
    }

    [DataContract]
    public class SelectOption
    {
        [DataMember(Name = "id")]
        public object Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
    }

    [DataContract]
    public class UpdatePluginRequest
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }
        [DataMember(Name = "profileId")]
        public int ProfileId { get; set; }
        [DataMember(Name = "pluginType")]
        public int PluginType { get; set; }
        [DataMember(Name = "type")]
        public string Type { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "properties")]
        public IEnumerable<PluginProperty> Properties { get; set; }
    }

    [DataContract]
    public class DeletePluginRequest
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }
        [DataMember(Name = "profileId")]
        public int ProfileId { get; set; }
        [DataMember(Name = "pluginType")]
        public int PluginType { get; set; }
    }

    [DataContract]
    public class LightSetup
    {
        [DataMember(Name = "clockwise")]
        public bool Clockwise { get; set; }

        [DataMember(Name = "firstRowIndex")]
        public int FirstRowIndex { get; set; }

        [DataMember(Name = "firstColumnIndex")]
        public int FirstColumnIndex { get; set; }

        [DataMember(Name = "lightRows")]
        public List<LightRow> LightRows { get; set; }
    }
    [DataContract]
    public class LightRow
    {
        [DataMember(Name = "rowIndex")]
        public int RowIndex { get; set; }

        [DataMember(Name = "lightColumns")]
        public List<LightColumn> LightColumns { get; set; }
    }
    [DataContract]
    public class LightColumn
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "index")]
        public string Index { get; set; }

        [DataMember(Name = "columnIndex")]
        public int ColumnIndex { get; set; }

        [DataMember(Name = "colourClass")]
        public string ColourClass { get; set; }

        [DataMember(Name = "enabled")]
        public bool Enabled { get; set; }
    }
}
