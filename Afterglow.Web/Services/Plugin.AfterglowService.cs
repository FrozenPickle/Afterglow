using Afterglow.Core;
using Afterglow.Core.Configuration;
using Afterglow.Core.Plugins;
using Afterglow.Web.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace Afterglow.Web
{
    public partial class AfterglowServiceController : ApiController
    {
        [Route("plugin")]
        public PluginResponse Post(PluginRequest request)
        {
            PluginResponse response = new PluginResponse();
            Profile profile = (from p in Program.Runtime.Setup.Profiles
                               where p.Id == request.ProfileId
                               select p).FirstOrDefault();
            if (profile == null)
            {
                return response;
            }

            bool newPlugin = request.Id == 0;
            Type type = null;

            IAfterglowPlugin plugin = null;

            switch (request.PluginType)
            {
                case 1:
                    if (newPlugin)
                    {
                        type = Program.Runtime.Setup.AvailableLightSetupPlugins.Where(a => a.Name == request.Type).FirstOrDefault();
                    }
                    else
                    {
                        plugin = profile.LightSetupPlugins.Where(l => l.Id == request.Id).FirstOrDefault();
                    }
                    break;
                case 2:
                    if (newPlugin)
                    {
                        type = Program.Runtime.Setup.AvailableCapturePlugins.Where(a => a.Name == request.Type).FirstOrDefault();
                    }
                    else
                    {
                        plugin = profile.CapturePlugins.Where(l => l.Id == request.Id).FirstOrDefault();
                    }
                    break;
                case 3:
                    if (newPlugin)
                    {
                        type = Program.Runtime.Setup.AvailableColourExtractionPlugins.Where(a => a.Name == request.Type).FirstOrDefault();
                    }
                    else
                    {
                        plugin = profile.ColourExtractionPlugins.Where(l => l.Id == request.Id).FirstOrDefault();
                    }
                    break;
                case 4:
                    if (newPlugin)
                    {
                        type = Program.Runtime.Setup.AvailablePostProcessPlugins.Where(a => a.Name == request.Type).FirstOrDefault();
                    }
                    else
                    {
                        plugin = profile.PostProcessPlugins.Where(l => l.Id == request.Id).FirstOrDefault();
                    }
                    break;
                case 5:
                    if (newPlugin)
                    {
                        type = Program.Runtime.Setup.AvailablePreOutputPlugins.Where(a => a.Name == request.Type).FirstOrDefault();
                    }
                    else
                    {
                        plugin = profile.PreOutputPlugins.Where(l => l.Id == request.Id).FirstOrDefault();
                    }
                    break;
                case 6:
                    if (newPlugin)
                    {
                        type = Program.Runtime.Setup.AvailableOutputPlugins.Where(a => a.Name == request.Type).FirstOrDefault();
                    }
                    else
                    {
                        plugin = profile.OutputPlugins.Where(l => l.Id == request.Id).FirstOrDefault();
                    }
                    break;
                default:
                    plugin = null;
                    break;
            }

            if (newPlugin && type != null)
            {
                plugin = Activator.CreateInstance(type) as IAfterglowPlugin;
            }
            else if (plugin == null)
            {
                return response;
            }

            response.Id = plugin.Id;
            response.Name = plugin.Name;
            response.Title = string.Format("{0} - {1}", profile.Name, plugin.Name);
            response.Description = plugin.Description;
            response.Author = plugin.Author;
            response.Version = plugin.Version;
            response.Website = plugin.Website;
            response.PluginType = request.PluginType;
            response.ProfileId = profile.Id;

            response.Properties = GetPluginProperties(plugin);

            return response;
        }

        private PluginProperty[] GetPluginProperties(object plugin)
        {
            List<PluginProperty> pluginProperties = new List<PluginProperty>();

            if (plugin == null)
            {
                return pluginProperties.ToArray();
            }

            Type pluginObjectType = plugin.GetType();

            foreach (System.Reflection.PropertyInfo objectProperty in pluginObjectType.GetProperties())
            {
                PluginProperty pluginProperty = new PluginProperty();
                bool exclude = false;
                bool dataMember = false;

                if (!objectProperty.CanWrite)
                {
                    continue;
                }

                pluginProperty.Name = objectProperty.Name;

                pluginProperty.Value = objectProperty.GetValue(plugin, null);

                Type propertyType = objectProperty.PropertyType;
                if (propertyType == typeof(string))
                {
                    pluginProperty.Type = "text";
                }
                else if (propertyType == typeof(int))
                {
                    pluginProperty.Type = "number";
                }
                else if (propertyType == typeof(bool))
                {
                    pluginProperty.Type = "boolean";
                }
                else if (propertyType == typeof(List<Core.Light>))
                {
                    pluginProperty.Type = "lights";

                    string[] colourClasses = new string[] { "btn-primary", "btn-success", "btn-info", "btn-warning", "btn-danger" };
                    int colourPosition = 0;

                    List<Core.Light> lights = pluginProperty.Value as List<Core.Light>;
                    LightSetup lightSetup = new LightSetup();

                    if (lights != null && lights.Any())
                    {
                        //convert back to 2d array for setup
                        lightSetup.LightRows = new List<LightRow>();

                        int numberOfRows = lights.Max(l => l.Top);
                        int numberOfColumns = lights.Max(l => l.Left);

                        for (int row = 0; row <= numberOfRows; row++)
                        {
                            LightRow lightRow = new LightRow();
                            lightRow.RowIndex = row;
                            lightRow.LightColumns = new List<LightColumn>();
                            for (int column = 0; column <= numberOfColumns; column++)
                            {
                                LightColumn lightColumn = new LightColumn();
                                lightColumn.ColumnIndex = column;
                                if (column == 0 || column == numberOfColumns
                                    || row == 0 || row == numberOfRows)
                                {
                                    
                                    Light light = (from l in lights
                                                  where l.Top == row
                                                      && l.Left == column
                                                  select l).FirstOrDefault();
                                    if (light != null)
                                    {
                                        lightColumn.Id = light.Id.ToString();
                                        lightColumn.Index = light.Index.ToString();

                                        lightColumn.ColourClass = colourClasses[colourPosition++];

                                        lightColumn.Enabled = !string.IsNullOrEmpty(lightColumn.Id);

                                        if (lightColumn.Id == "1")
                                        {
                                            lightSetup.FirstColumnIndex = column;
                                            lightSetup.FirstRowIndex = row;
                                        }
                                    }
                                    else
                                    {
                                        lightColumn.ColourClass = colourClasses[colourPosition++];
                                    }
                                }
                                else
                                {
                                    lightColumn.ColourClass = "disabled";
                                    lightColumn.Enabled = false;
                                }
                                if (colourPosition >= colourClasses.Length)
                                {
                                    colourPosition = 0;
                                }
                                lightRow.LightColumns.Add(lightColumn);
                            }
                            lightSetup.LightRows.Add(lightRow);
                        }
                    }
                    pluginProperty.Value = lightSetup;
                }

                foreach (System.Attribute attr in objectProperty.GetCustomAttributes(true))
                {
                    if ((attr as DisplayAttribute) != null)
                    {
                        DisplayAttribute displayName = (DisplayAttribute)attr;
                        pluginProperty.DisplayName = displayName.Name;
                        pluginProperty.Description = displayName.Description;
                    }
                    else if ((attr as RangeAttribute) != null)
                    {
                        RangeAttribute range = (RangeAttribute)attr;
                        pluginProperty.MinValue = range.Minimum as int?;
                        pluginProperty.MaxValue = range.Maximum as int?;
                    }
                    else if ((attr as RequiredAttribute) != null)
                    {
                        RequiredAttribute required = (RequiredAttribute)attr;
                        pluginProperty.Required = required.AllowEmptyStrings;
                    }
                    else if ((attr as DataMemberAttribute) != null)
                    {
                        dataMember = true;
                    }
                    else if ((attr as KeyAttribute) != null)
                    {
                        exclude = true;
                        break;
                    }
                    else if ((attr as ConfigLookupAttribute) != null)
                    {
                        ConfigLookupAttribute lookup = (ConfigLookupAttribute)attr;
                        pluginProperty.Type = "lookup";

                        PropertyInfo optionsProperty = pluginObjectType.GetProperty(lookup.RetrieveValuesFrom);

                        IEnumerable<object> options = optionsProperty.GetValue(plugin, null) as IEnumerable<object>;
                        List<SelectOption> parsedOptions = new List<SelectOption>();
                        if (options.Any())
                        {
                            foreach (object option in options)
                            {
                                LookupItem lookupItem = option as LookupItem;
                                LookupItemString lookupItemString = option as LookupItemString;
                                SelectOption pluginOption = new SelectOption();

                                if (lookupItem != null)
                                {
                                    pluginOption.Id = lookupItem.Id;
                                    pluginOption.Name = lookupItem.Name;
                                }
                                else if (lookupItemString != null)
                                {
                                    pluginOption.Id = lookupItemString.Id;
                                    pluginOption.Name = lookupItemString.Name;
                                }
                                else
                                {
                                    pluginOption.Id = option;
                                    pluginOption.Name = option.ToString();
                                }
                                parsedOptions.Add(pluginOption);
                            }
                        }
                        pluginProperty.Options = parsedOptions;
                    }
                }

                if (dataMember && !exclude)
                {
                    if (pluginProperty.Type == "lookup" && pluginProperty.Value != null)
                    {
                        var currentValue = (from o in pluginProperty.Options
                                            where o.Id.ToString() == pluginProperty.Value.ToString()
                                            select o).FirstOrDefault();
                        if (currentValue == null)
                        {
                            List<SelectOption> options = pluginProperty.Options as List<SelectOption>;
                            options.Add(new SelectOption()
                            {
                                Id = pluginProperty.Value,
                                Name = "Invalid! " + pluginProperty.Value.ToString()
                            });
                            pluginProperty.Options = options;
                        }
                    }
                    pluginProperties.Add(pluginProperty);
                }
            }

            return pluginProperties.ToArray();
        }

        [Route("updatePlugin")]
        public PluginResponse Post(UpdatePluginRequest request)
        {
            PluginResponse response = new PluginResponse();
            Profile profile = (from p in Program.Runtime.Setup.Profiles
                               where p.Id == request.ProfileId
                               select p).FirstOrDefault();

            bool newPlugin = request.Id == 0;
            int newId = 0;
            IAfterglowPlugin plugin = null;
            Type type = null;

            switch (request.PluginType)
            {
                case 1:
                    if (newPlugin)
                    {
                        type = Program.Runtime.Setup.AvailableLightSetupPlugins.Where(a => a.Name == request.Type).FirstOrDefault();
                        newId = Program.Runtime.Setup.GetNewId<ILightSetupPlugin>();
                    }
                    else
                    {
                        plugin = profile.LightSetupPlugins.Where(l => l.Id == request.Id).FirstOrDefault();
                    }
                    break;
                case 2:
                    if (newPlugin)
                    {
                        type = Program.Runtime.Setup.AvailableCapturePlugins.Where(a => a.Name == request.Type).FirstOrDefault();
                        newId = Program.Runtime.Setup.GetNewId<ICapturePlugin>();
                    }
                    else
                    {
                        plugin = profile.CapturePlugins.Where(l => l.Id == request.Id).FirstOrDefault();
                    }
                    break;
                case 3:
                    if (newPlugin)
                    {
                        type = Program.Runtime.Setup.AvailableColourExtractionPlugins.Where(a => a.Name == request.Type).FirstOrDefault();
                        newId = Program.Runtime.Setup.GetNewId<IColourExtractionPlugin>();
                    }
                    else
                    {
                        plugin = profile.ColourExtractionPlugins.Where(l => l.Id == request.Id).FirstOrDefault();
                    }
                    break;
                case 4:
                    if (newPlugin)
                    {
                        type = Program.Runtime.Setup.AvailablePostProcessPlugins.Where(a => a.Name == request.Type).FirstOrDefault();
                        newId = Program.Runtime.Setup.GetNewId<IPostProcessPlugin>();
                    }
                    else
                    {
                        plugin = profile.PostProcessPlugins.Where(l => l.Id == request.Id).FirstOrDefault();
                    }
                    break;
                case 5:
                    if (newPlugin)
                    {
                        type = Program.Runtime.Setup.AvailablePreOutputPlugins.Where(a => a.Name == request.Type).FirstOrDefault();
                        newId = Program.Runtime.Setup.GetNewId<IPreOutputPlugin>();
                    }
                    else
                    {
                        plugin = profile.PreOutputPlugins.Where(l => l.Id == request.Id).FirstOrDefault();
                    }
                    break;
                case 6:
                    if (newPlugin)
                    {
                        type = Program.Runtime.Setup.AvailableOutputPlugins.Where(a => a.Name == request.Type).FirstOrDefault();
                        newId = Program.Runtime.Setup.GetNewId<IOutputPlugin>();
                    }
                    else
                    {
                        plugin = profile.OutputPlugins.Where(l => l.Id == request.Id).FirstOrDefault();
                    }
                    break;
                default:
                    plugin = null;
                    break;
            }

            if (newPlugin && type != null)
            {
                plugin = Activator.CreateInstance(type) as IAfterglowPlugin;
            }

            if (SetPluginProperties(plugin, request.Properties))
            {
                if (newPlugin)
                {
                    plugin.Id = newId;
                    switch (request.PluginType)
                    {
                        case 1:
                            profile.LightSetupPlugins[0] = plugin as ILightSetupPlugin;
                            break;
                        case 2:
                            profile.CapturePlugins[0] = plugin as ICapturePlugin;
                            break;
                        case 3:
                            profile.ColourExtractionPlugins[0] = plugin as IColourExtractionPlugin;
                            break;
                        case 4:
                            profile.PostProcessPlugins.Add(plugin as IPostProcessPlugin);
                            break;
                        case 5:
                            profile.PreOutputPlugins.Add(plugin as IPreOutputPlugin);
                            break;
                        case 6:
                            profile.OutputPlugins.Add(plugin as IOutputPlugin);
                            break;
                        default:
                            plugin = null;
                            break;
                    }
                }

                Program.Runtime.Save();

                // Load the saved version
                PluginRequest newRequest = new PluginRequest();
                newRequest.Id = plugin.Id;
                newRequest.PluginType = request.PluginType;
                newRequest.ProfileId = profile.Id;

                return Post(newRequest);
            }
            return null;
        }

        private bool SetPluginProperties(object plugin, IEnumerable<PluginProperty> pluginProperties)
        {
            Type pluginObjectType = plugin.GetType();

            foreach (PluginProperty pluginProperty in pluginProperties)
            {
                PropertyInfo objectProperty = pluginObjectType.GetProperty(pluginProperty.Name);

                if (pluginProperty.Type == "text")
                {
                    objectProperty.SetValue(plugin, pluginProperty.Value, null);
                }
                else if (pluginProperty.Type == "number")
                {
                    int value = 0;
                    if (pluginProperty.Value != null
                        && int.TryParse(pluginProperty.Value.ToString(), out value))
                    {
                        objectProperty.SetValue(plugin, value, null);
                    }
                }
                else if (pluginProperty.Type == "boolean")
                {
                    bool value = false;
                    if (pluginProperty.Value != null
                        && bool.TryParse(pluginProperty.Value.ToString(), out value))
                    {
                        objectProperty.SetValue(plugin, value, null);
                    }
                }
                else if (pluginProperty.Type == "text" || pluginProperty.Type == "lookup")
                {
                    int value = 0;
                    Type propertyType = objectProperty.PropertyType;
                    if (propertyType == typeof(string))
                    {
                        objectProperty.SetValue(plugin, pluginProperty.Value, null);
                    }
                    else if (propertyType == typeof(int)
                        && pluginProperty.Value != null
                        && int.TryParse(pluginProperty.Value.ToString(), out value))
                    {
                        objectProperty.SetValue(plugin, value, null);
                    }

                }
                else if (pluginProperty.Type == "lights")
                {
                    LightSetup lightSetup = null;
                    if (pluginProperty.Value != null)
                    {
                        lightSetup = JsonConvert.DeserializeObject<LightSetup>(pluginProperty.Value.ToString());
                    }

                    List<Core.Light> lights = new List<Core.Light>();

                    if (lightSetup != null)
                    {
                        lights = (from lightRow in lightSetup.LightRows
                                  from lightColumn in lightRow.LightColumns
                                  where !string.IsNullOrEmpty(lightColumn.Id)
                                  orderby Convert.ToInt32(lightColumn.Id)
                                  select new Core.Light()
                                  {
                                      Id = Convert.ToInt32(lightColumn.Id),
                                      Index = Convert.ToInt32(lightColumn.Index),
                                      Left = lightColumn.ColumnIndex,
                                      Top = lightRow.RowIndex
                                  }).ToList();
                    }
                    else
                    {
                        AfterglowRuntime.Logger.Warn("Saving light setup failed");
                    }
                    objectProperty.SetValue(plugin, lights, null);
                }
            }

            return true;
        }

        [Route("deletePlugin")]
        public bool Post(DeletePluginRequest request)
        {
            PluginResponse response = new PluginResponse();
            Profile profile = (from p in Program.Runtime.Setup.Profiles
                               where p.Id == request.ProfileId
                               select p).FirstOrDefault();

            if (request.Id == 0)
            {
                return true;
            }

            bool updated = false;

            switch (request.PluginType)
            {
                case 4:
                    IPostProcessPlugin postProcessPlugin = profile.PostProcessPlugins.Where(l => l.Id == request.Id).FirstOrDefault() as IPostProcessPlugin;
                    if (postProcessPlugin != null)
                    {
                        profile.PostProcessPlugins.Remove(postProcessPlugin);
                        updated = true;
                    }
                    break;
                case 5:
                    IPreOutputPlugin preOutputPlugin = profile.PreOutputPlugins.Where(l => l.Id == request.Id).FirstOrDefault() as IPreOutputPlugin;
                    if (preOutputPlugin != null)
                    {
                        profile.PreOutputPlugins.Remove(preOutputPlugin);
                        updated = true;
                    }
                    break;
                case 6:
                    IOutputPlugin outputPlugin = profile.OutputPlugins.Where(l => l.Id == request.Id).FirstOrDefault() as IOutputPlugin;
                    if (outputPlugin != null)
                    {
                        profile.OutputPlugins.Remove(outputPlugin);
                        updated = true;
                    }
                    break;
                default:
                    break;
            }
            if (updated)
            {
                Program.Runtime.Save();
            }
            return true;
        }
    }
}
