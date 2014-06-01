using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceHost;
using System.Runtime.Serialization;
using Afterglow.Core;
using Afterglow.Core.Plugins;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Afterglow.Core.Configuration;

namespace Afterglow.Web
{
    [Route("/runtime")]
    public class Runtime
    {
        public bool? Start { get; set; }
    }

    public class RuntimeResponse
    {
        public bool Active { get; set; }
        public int NumberOfLightsHigh { get; set; }
        public int NumberOfLightsWide { get; set; }
    }

    [Route("/setup")]
    public class Setup
    {
        public Afterglow.Core.AfterglowSetup UpdatedSetup { get; set; }
    }

    public class SetupResponse
    {
        public Afterglow.Core.AfterglowSetup Setup { get; set; }
    }
    
    [Route("/preview")]
    public class Preview
    {

    }

    public class PreviewResponse
    {
        public List<LightPreview> Lights { get; set; }

        public double CaptureFPS { get; set; }
        
        public double CaptureFrameTime { get; set; }
        
        public double OutputFPS { get; set; }

        public double OutputFrameTime { get; set; }
            
    }

    [DataContract]
    public class LightPreview
    {
        [DataMember]
        public int Top { get; set; }
        [DataMember]
        public int Left { get; set; }
        [DataMember]
        public Color Colour { get; set; }
    }

    public class Color
    {
        public byte R { get; set; }
        public byte G { get; set; }
        public byte B { get; set; }

        public static implicit operator System.Drawing.Color(Color c)
        {
            return System.Drawing.Color.FromArgb(c.R, c.G, c.B);
        }

        public static implicit operator Color(System.Drawing.Color c)
        {
            return new Color() { R = c.R, G = c.G, B = c.B };
        }
    }

    #region Index Controller
    [Route("/isRunning")]
    public class IsRunningRequest
    {
    }
    [Route("/start")]
    public class StartRequest
    {
    }
    [Route("/stop")]
    public class StopRequest
    {
    }
    [Route("/toggleStartStop")]
    public class ToggleStartStopRequest
    {
    }
    [DataContract]
    public class AfterglowActiveResponse
    {
        [DataMember(Name="active")]
        public bool Active { get; set; }
    }
    [Route("/menuSetup")]
    public class MenuSetupRequest
    {
    }
    [DataContract]
    public class MenuSetupResponse
    {
        [DataMember(Name = "profiles")]
        public IEnumerable<ProfilesProfileResponse> Profiles { get; set; }

        [DataMember(Name = "currentProfile")]
        public ProfilesProfileResponse CurrentProfile { get; set; }
    }
    [Route("/setProfile")]
    [DataContract]
    public class SetProfileRequest
    {
        [DataMember(Name="profileId")]
        public int ProfileId { get; set; }
    }
    #endregion

    #region Plugins Controller
    [Route("/availablePlugins")]
    [DataContract]
    public class AvailablePluginsRequest
    {
    }
    [DataContract]
    public class AvailablePluginsResult
    {
        [DataMember(Name = "availableLightSetupPlugins")]
        public IEnumerable<AvailablePluginDetails> AvailableLightSetupPlugins { get; set; }

        [DataMember(Name = "availableCapturePlugins")]
        public IEnumerable<AvailablePluginDetails> AvailableCapturePlugins { get; set; }

        [DataMember(Name = "availableColourExtractionPlugins")]
        public IEnumerable<AvailablePluginDetails> AvailableColourExtractionPlugins { get; set; }

        [DataMember(Name = "availablePostProcessPlugins")]
        public IEnumerable<AvailablePluginDetails> AvailablePostProcessPlugins { get; set; }

        [DataMember(Name = "availablePreOutputPlugins")]
        public IEnumerable<AvailablePluginDetails> AvailablePreOutputPlugins { get; set; }

        [DataMember(Name = "availableOutputPlugins")]
        public IEnumerable<AvailablePluginDetails> AvailableOutputPlugins { get; set; }
    }
    #endregion

    #region Profiles Controller

    [Route("/profiles")]
    [DataContract]
    public class ProfilesRequest
    {
    }
    [DataContract]
    public class ProfilesResponse
    {
        [DataMember(Name="profiles")]
        public IEnumerable<ProfilesProfileResponse> Profiles { get; set; }
    }
    [DataContract]
    public class ProfilesProfileResponse
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "description")]
        public string Description { get; set; }
    }

    [Route("/addProfile")]
    [DataContract]
    public class AddProfileRequest
    {
    }

    #endregion

    #region Profile Controller
    [Route("/profile")]
    [DataContract]
    public class ProfileRequest
    {
        [DataMember(Name="id")]
        public int Id { get; set; }
    }
    [DataContract]
    public class ProfileResponse
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "description")]
        public string Description { get; set; }
        [DataMember(Name = "captureFrequency")]
        public int CaptureFrequency { get; set; }
        [DataMember(Name = "outputFrequency")]
        public int OutputFrequency { get; set; }
        [DataMember(Name = "lightSetupPlugins")]
        public IEnumerable<SavedPlugin> LightSetupPlugins { get; set; }
        [DataMember(Name = "capturePlugins")]
        public IEnumerable<SavedPlugin> CapturePlugins { get; set; }
        [DataMember(Name = "colourExtractionPlugins")]
        public IEnumerable<SavedPlugin> ColourExtractionPlugins { get; set; }
        [DataMember(Name = "postProcessPlugins")]
        public IEnumerable<SavedPlugin> PostProcessPlugins { get; set; }
        [DataMember(Name = "preOutputPlugins")]
        public IEnumerable<SavedPlugin> PreOutputPlugins { get; set; }
        [DataMember(Name = "outputPlugins")]
        public IEnumerable<SavedPlugin> OutputPlugins { get; set; }
    }
    [DataContract]
    public class SavedPlugin
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "description")]
        public string Description { get; set; }
    }
    [Route("/updateProfile")]
    [DataContract]
    public class UpdateProfileRequest
    {
        [DataMember(Name = "id")]
        public int Id { get; set; }
        [DataMember(Name = "name")]
        public string Name { get; set; }
        [DataMember(Name = "description")]
        public string Description { get; set; }
        [DataMember(Name = "captureFrequency")]
        public int CaptureFrequency { get; set; }
        [DataMember(Name = "outputFrequency")]
        public int OutputFrequency { get; set; }
    }

    [Route("/pluginTypes")]
    [DataContract]
    public class PluginTypesRequest
    {
        [DataMember(Name = "pluginType")]
        public int PluginType { get; set; }
    }

    [DataContract]
    public class PluginTypesResponse
    {
        [DataMember(Name = "plugins")]
        public IEnumerable<AvailablePluginDetails> Plugins { get; set; }
    }
    #endregion

    #region Settings Controller
    [Route("/settings")]
    public class SettingsRequest { }
    [DataContract]
    public class SettingsResponse
    {
        [DataMember(Name = "port")]
        public int Port { get; set; }
        [DataMember(Name = "userName")]
        public string UserName { get; set; }
        [DataMember(Name = "password")]
        public string Password { get; set; }
        [DataMember(Name = "logLevel")]
        public int LogLevel { get; set; }
        [DataMember(Name = "logLevels")]
        public IEnumerable<SelectOption> LogLevels { get; set; }
    }
    [Route("/updateSettings")]
    [DataContract]
    public class UpdateSettingsRequest
    {
        [DataMember(Name = "port")]
        public int Port { get; set; }
        [DataMember(Name = "userName")]
        public string UserName { get; set; }
        [DataMember(Name = "password")]
        public string Password { get; set; }
        [DataMember(Name = "logLevel")]
        public int LogLevel { get; set; }
    }


    #endregion

    #region Plugin Controller
    [Route("/plugin")]
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

    [Route("/updatePlugin")]
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

    [Route("/deletePlugin")]
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

        [DataMember(Name="lightColumns")]
        public List<LightColumn> LightColumns { get; set; }
    }
    [DataContract]
    public class LightColumn
    {
        [DataMember(Name = "id")]
        public string Id { get; set; }

        [DataMember(Name = "columnIndex")]
        public int ColumnIndex { get; set; }

        [DataMember(Name = "colourClass")]
        public string ColourClass { get; set; }

        [DataMember(Name = "enabled")]
        public bool Enabled { get; set; }
    }
    #endregion

    public class AfterglowService : Service
    {
        #region Index Controller
        public object Get(MenuSetupRequest request)
        {
            MenuSetupResponse response = new MenuSetupResponse();
            response.Profiles = Get(new ProfilesRequest()).Profiles;

            if (Program.Runtime.CurrentProfile != null)
            {
                response.CurrentProfile = (from p in response.Profiles
                                           where p.Id == Program.Runtime.CurrentProfile.Id
                                           select p).FirstOrDefault();
            }
            return response;
        }
        public object Get(IsRunningRequest request)
        {
            return new AfterglowActiveResponse() { Active = Program.Runtime.Active };
        }
        public object Get(StartRequest request)
        {
            Program.Runtime.Start();
            return new AfterglowActiveResponse() { Active = Program.Runtime.Active };
        }
        public object Get(StopRequest request)
        {
            Program.Runtime.Stop();
            return new AfterglowActiveResponse() { Active = Program.Runtime.Active };
        }
        public object Get(ToggleStartStopRequest request)
        {
            if (!Program.Runtime.Active)
            {
                Program.Runtime.Start();
            }
            else
            {
                Program.Runtime.Stop();
            }
            return new AfterglowActiveResponse() { Active = Program.Runtime.Active };
        }
        public object Post(SetProfileRequest request)
        {

            Profile profile = (from p in Program.Runtime.Setup.Profiles
                                       where p.Id == request.ProfileId
                                       select p).FirstOrDefault();

            Program.Runtime.CurrentProfile = profile;
            Program.Runtime.Save();
            return new ProfilesProfileResponse() { Id = profile.Id, Name = profile.Name, Description = profile.Description };
        }
        #endregion

        #region Plugins Controller

        public object Get(AvailablePluginsRequest request)
        {
            AvailablePluginsResult result = new AvailablePluginsResult();

            result.AvailableLightSetupPlugins = AfterglowSetup.GetAvailblePlugins(Program.Runtime.Setup.AvailableLightSetupPlugins);
            result.AvailableCapturePlugins = AfterglowSetup.GetAvailblePlugins(Program.Runtime.Setup.AvailableCapturePlugins);
            result.AvailableColourExtractionPlugins = AfterglowSetup.GetAvailblePlugins(Program.Runtime.Setup.AvailableColourExtractionPlugins);
            result.AvailablePostProcessPlugins = AfterglowSetup.GetAvailblePlugins(Program.Runtime.Setup.AvailablePostProcessPlugins);
            result.AvailablePreOutputPlugins = AfterglowSetup.GetAvailblePlugins(Program.Runtime.Setup.AvailablePreOutputPlugins);
            result.AvailableOutputPlugins = AfterglowSetup.GetAvailblePlugins(Program.Runtime.Setup.AvailableOutputPlugins);

            return result;
        }
        #endregion

        #region Profiles Controller
        public ProfilesResponse Get(ProfilesRequest request)
        {
            return new ProfilesResponse
            {
                Profiles = (from p in Program.Runtime.Setup.Profiles
                            select new ProfilesProfileResponse
                            {
                                Id = p.Id,
                                Name = p.Name,
                                Description = p.Description
                            })
            };
        }

        public object Get(AddProfileRequest request)
        {
            int id = Program.Runtime.Setup.AddNewProfile().Id;
            Program.Runtime.Save();

            return id;
        }
        #endregion

        #region Profile Controller

        public object Post(ProfileRequest request)
        {
            ProfileResponse response = null;
            Profile profile = (from p in Program.Runtime.Setup.Profiles
                               where p.Id == request.Id
                               select p).FirstOrDefault();
            if (profile != null)
            {
                response = new ProfileResponse();
                response.Id = profile.Id;
                response.Name = profile.Name;
                response.Description = profile.Description;
                response.CaptureFrequency = profile.CaptureFrequency;
                response.OutputFrequency = profile.OutputFrequency;

                response.LightSetupPlugins = (from p in profile.LightSetupPlugins
                                                select new SavedPlugin
                                                {
                                                    Id = p.Id,
                                                    Name = p.Name,
                                                    Description = p.ToString()
                                                });
                response.CapturePlugins = (from p in profile.CapturePlugins
                                           select new SavedPlugin
                                                {
                                                    Id = p.Id,
                                                    Name = p.Name,
                                                    Description = p.ToString()
                                                });
                response.ColourExtractionPlugins = (from p in profile.ColourExtractionPlugins
                                                    select new SavedPlugin
                                                {
                                                    Id = p.Id,
                                                    Name = p.Name,
                                                    Description = p.ToString()
                                                });
                response.PostProcessPlugins = (from p in profile.PostProcessPlugins
                                               select new SavedPlugin
                                                {
                                                    Id = p.Id,
                                                    Name = p.Name,
                                                    Description = p.ToString()
                                                });
                response.PreOutputPlugins = (from p in profile.PreOutputPlugins
                                             select new SavedPlugin
                                                {
                                                    Id = p.Id,
                                                    Name = p.Name,
                                                    Description = p.ToString()
                                                });
                response.OutputPlugins = (from p in profile.OutputPlugins
                                          select new SavedPlugin
                                                {
                                                    Id = p.Id,
                                                    Name = p.Name,
                                                    Description = p.ToString()
                                                });

            }
            return response;
        }

        public object Post(UpdateProfileRequest request)
        {
            object response = null;
            Profile existingProfile = (from p in Program.Runtime.Setup.Profiles
                               where p.Id == request.Id
                               select p).FirstOrDefault();
            if (existingProfile != null)
            {
                existingProfile.Name = request.Name;
                existingProfile.Description = request.Description;
                existingProfile.CaptureFrequency = request.CaptureFrequency;
                existingProfile.OutputFrequency = request.OutputFrequency;

                Program.Runtime.Save();

                // Re load saved changes to ensure save has worked
                response = Post(new ProfileRequest() { Id = request.Id });
            }
            return response;
        }

        public object Post(PluginTypesRequest request)
        {
            PluginTypesResponse result = new PluginTypesResponse();

            switch (request.PluginType)
            {
                case 1:
                    result.Plugins = AfterglowSetup.GetAvailblePlugins(Program.Runtime.Setup.AvailableLightSetupPlugins);
                    break;
                case 2:
                    result.Plugins = AfterglowSetup.GetAvailblePlugins(Program.Runtime.Setup.AvailableCapturePlugins);
                    break;
                case 3:
                    result.Plugins = AfterglowSetup.GetAvailblePlugins(Program.Runtime.Setup.AvailableColourExtractionPlugins);
                    break;
                case 4:
                    result.Plugins = AfterglowSetup.GetAvailblePlugins(Program.Runtime.Setup.AvailablePostProcessPlugins);
                    break;
                case 5:
                    result.Plugins = AfterglowSetup.GetAvailblePlugins(Program.Runtime.Setup.AvailablePreOutputPlugins);
                    break;
                case 6:
                    result.Plugins = AfterglowSetup.GetAvailblePlugins(Program.Runtime.Setup.AvailableOutputPlugins);
                    break;
                default:
                    result.Plugins = (new List<AvailablePluginDetails>()).AsEnumerable();
                    break;
            }

            return result;
        }
        #endregion

        #region Settings Controller
        public object Post(SettingsRequest request)
        {
            return new SettingsResponse
            {
                Port = Program.Runtime.Setup.Port,
                UserName = Program.Runtime.Setup.UserName,
                Password = Program.Runtime.Setup.Password,
                LogLevel = Program.Runtime.Setup.LogLevel,
                LogLevels = (from ll in Program.Runtime.Setup.LoggingLevels
                             select new SelectOption()
                             {
                                 Id = ll.Id,
                                 Name = ll.Name
                             }).ToArray()
            };
        }
        public object Post(UpdateSettingsRequest request)
        {
            AfterglowSetup setup = Program.Runtime.Setup;
            setup.Port = request.Port;
            setup.UserName = request.UserName;
            setup.Password = request.Password;
            setup.LogLevel = request.LogLevel;

            Program.Runtime.Save();

            // Load the saved version
            return Post(new SettingsRequest());
        }

        #endregion

        #region Plugin Settings

        public object Post(PluginRequest request)
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
            response.Title = string.Format("{0} - {1}", profile.Name, plugin.Name) ;
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
                                lightColumn.Id = (from l in lights
                                                  where l.Top == row
                                                      && l.Left == column
                                                  select l.Id.ToString()).FirstOrDefault();
                                if (column == 0 || column == numberOfColumns
                                    || row == 0 || row == numberOfRows)
                                {
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
                            options.Add(new SelectOption() { 
                                Id = pluginProperty.Value, 
                                Name = "Invalid! " + pluginProperty.Value.ToString() });
                            pluginProperty.Options = options;
                        }
                    }
                    pluginProperties.Add(pluginProperty);
                }
            }

            return pluginProperties.ToArray();
        }

        public object Post(UpdatePluginRequest request)
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
                    LightSetup lightSetup = pluginProperty.Value as LightSetup;
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
                                           Index = Convert.ToInt32(lightColumn.Id) - 1,
                                           Left = lightColumn.ColumnIndex,
                                           Top = lightRow.RowIndex
                                       }
                                       
                                       ).ToList();
                    }
                    objectProperty.SetValue(plugin, lights, null);
                }
            }

            return true;
        }

        public object Post(DeletePluginRequest request)
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

        #endregion

        public object Get(Runtime request)
        {
            return new RuntimeResponse
            {
                Active = Program.Runtime.Active,
                NumberOfLightsHigh = (Program.Runtime.CurrentProfile != null && Program.Runtime.CurrentProfile.LightSetupPlugin != null ? Program.Runtime.CurrentProfile.LightSetupPlugin.NumberOfLightsHigh : 0),
                NumberOfLightsWide = (Program.Runtime.CurrentProfile != null && Program.Runtime.CurrentProfile.LightSetupPlugin != null ? Program.Runtime.CurrentProfile.LightSetupPlugin.NumberOfLightsWide : 0)
            };
        }

        public object Post(Runtime request)
        {
            if (!request.Start.HasValue)
            {
                request.Start = !Program.Runtime.Active;
            }

            if (request.Start.Value)
            {
                Program.Runtime.Start();
            }
            else
            {
                Program.Runtime.Stop();
            }
        
            return new RuntimeResponse
            {
                Active = Program.Runtime.Active,
                NumberOfLightsHigh = Program.Runtime.CurrentProfile.LightSetupPlugin.NumberOfLightsHigh,
                NumberOfLightsWide = Program.Runtime.CurrentProfile.LightSetupPlugin.NumberOfLightsWide
            };
        }

        public object Get(Setup request)
        {
            return new SetupResponse
            {
                Setup = Program.Runtime.Setup
            };
        }

        public object Post(Setup request)
        {
            if (request.UpdatedSetup != null)
            {
                // TODO: replace Program.Runtime.Setup with request.Setup
            }
            return new SetupResponse
            {
                Setup = Program.Runtime.Setup
            };
        }

        public object Get(Preview request)
        {
            List<LightPreview> lights = new List<LightPreview>(Program.Runtime.CurrentProfile.LightSetupPlugin.Lights.Count);
            // Retrieve previous final light output data
            var lightData = Program.Runtime.GetPreviousLightData();
            if (lightData != null)
            {
                for (var i = 0; i < Program.Runtime.CurrentProfile.LightSetupPlugin.Lights.Count; i++)
                {
                    var light = Program.Runtime.CurrentProfile.LightSetupPlugin.Lights[i];
                    lights.Add(new LightPreview() { Top = light.Top, Left = light.Left, Colour = lightData[i] });
                }
            }
            return new PreviewResponse
            {
                Lights = lights,
                CaptureFPS = Program.Runtime.CaptureLoopFPS,
                CaptureFrameTime = Program.Runtime.CaptureLoopFrameTime,
                OutputFPS = Program.Runtime.OutputLoopFPS,
                OutputFrameTime = Program.Runtime.OutputLoopFrameTime
            };
        }
    }
}
