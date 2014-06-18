using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Runtime.Serialization;
using Afterglow.Core;
using Afterglow.Core.Plugins;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using Afterglow.Core.Configuration;
using System.Web.Http;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using Afterglow.Web.Models;

namespace Afterglow.Web
{
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

    public class Setup
    {
        public Afterglow.Core.AfterglowSetup UpdatedSetup { get; set; }
    }

    public class SetupResponse
    {
        public Afterglow.Core.AfterglowSetup Setup { get; set; }
    }
    
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

    [DataContract]
    public class AfterglowActiveResponse
    {
        [DataMember(Name="active")]
        public bool Active { get; set; }
    }
    
    [DataContract]
    public class MenuSetupResponse
    {
        [DataMember(Name = "profiles")]
        public IEnumerable<ProfilesProfileResponse> Profiles { get; set; }

        [DataMember(Name = "currentProfile")]
        public ProfilesProfileResponse CurrentProfile { get; set; }
    }
    
    [DataContract]
    public class SetProfileRequest
    {
        [DataMember(Name="profileId")]
        public int ProfileId { get; set; }
    }
    #endregion

    #region Plugins Controller
    
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

    #endregion

    #region Profile Controller
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

    [RoutePrefix("")]
    public partial class AfterglowServiceController : ApiController
    {
        [Route("content/{path}")]
        public System.Net.Http.HttpResponseMessage GetContent(string path)
        {
            using (var f = File.Open(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path), FileMode.Open, FileAccess.Read))
            {
                var response = new System.Net.Http.HttpResponseMessage(HttpStatusCode.OK);
                response.Content = new System.Net.Http.StreamContent(f);
                response.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("text/html");
                return response;
            }
        }
        
        
        
        #region Index Controller
        [Route("menuSetup")]
        public MenuSetupResponse Get()
        {
            MenuSetupResponse response = new MenuSetupResponse();
            response.Profiles = GetProfiles().Profiles;

            if (Program.Runtime.CurrentProfile != null)
            {
                response.CurrentProfile = (from p in response.Profiles
                                           where p.Id == Program.Runtime.CurrentProfile.Id
                                           select p).FirstOrDefault();
            }
            return response;
        }

        [Route("isRunning")]
        public AfterglowActiveResponse GetIsRunning()
        {
            return new AfterglowActiveResponse() { Active = Program.Runtime.Active };
        }
        
        [Route("start")]
        public AfterglowActiveResponse GetStart()
        {
            Program.Runtime.Start();
            return new AfterglowActiveResponse() { Active = Program.Runtime.Active };
        }
        
        [Route("stop")]
        public AfterglowActiveResponse GetStop()
        {
            Program.Runtime.Stop();
            return new AfterglowActiveResponse() { Active = Program.Runtime.Active };
        }

        [Route("toggleStartStop")]
        public AfterglowActiveResponse GetToggleStartStop()
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

        [Route("setProfile")]
        public ProfilesProfileResponse Post(SetProfileRequest request)
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
        [Route("availablePlugins")]
        public AvailablePluginsResult GetAvailablePlugins()
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
        [Route("profiles")]
        public ProfilesResponse GetProfiles()
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

        [Route("addProfile")]
        public int GetAddProfile()
        {
            int id = Program.Runtime.Setup.AddNewProfile().Id;
            Program.Runtime.Save();

            return id;
        }
        #endregion

        #region Profile Controller

        [Route("profile")]
        public ProfileResponse Post(ProfileRequest request)
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

        [Route("updateProfile")]
        public ProfileResponse Post(UpdateProfileRequest request)
        {
            ProfileResponse response = null;
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

        [Route("pluginTypes")]
        public PluginTypesResponse Post(PluginTypesRequest request)
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
        [Route("settings")]
        public SettingsResponse GetSettings()
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
        [Route("updateSettings")]
        public SettingsResponse Post(UpdateSettingsRequest request)
        {
            AfterglowSetup setup = Program.Runtime.Setup;
            setup.Port = request.Port;
            setup.UserName = request.UserName;
            setup.Password = request.Password;
            setup.LogLevel = request.LogLevel;

            Program.Runtime.Save();

            // Load the saved version
            return GetSettings();
        }

        #endregion

        [Route("runtime")]
        public RuntimeResponse Get(Runtime request)
        {
            return new RuntimeResponse
            {
                Active = Program.Runtime.Active,
                NumberOfLightsHigh = (Program.Runtime.CurrentProfile != null && Program.Runtime.CurrentProfile.LightSetupPlugin != null ? Program.Runtime.CurrentProfile.LightSetupPlugin.NumberOfLightsHigh : 0),
                NumberOfLightsWide = (Program.Runtime.CurrentProfile != null && Program.Runtime.CurrentProfile.LightSetupPlugin != null ? Program.Runtime.CurrentProfile.LightSetupPlugin.NumberOfLightsWide : 0)
            };
        }

        [Route("runtime")]
        public RuntimeResponse Post(Runtime request)
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

        [Route("setup")]
        public SetupResponse GetSetup()
        {
            return new SetupResponse
            {
                Setup = Program.Runtime.Setup
            };
        }

        [Route("setup")]
        public SetupResponse Post(Setup request)
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

        [Route("preview")]
        public PreviewResponse GetPreview()
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
