using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack;
using ServiceStack.Text;
using System.Runtime.Serialization;
using Afterglow.Core;
using Afterglow.Core.Plugins;

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

    [Route("/updateProfile")]
    public class UpdateProfile
    {
        public int profileId { get; set; }
        public int pluginId { get; set; }
        public string pluginType { get; set; }
        public string actionType { get; set; }
        
        public string name { get; set; }
        public string description { get; set; }
        public int captureFrequency { get; set; }
        public int outputFrequency { get; set; }

        public const string ActionType_AddProfile = "addProfile";
        public const string ActionType_RemoveProfile = "removeProfile";
        public const string ActionType_Update = "update";
        public const string ActionType_Add = "add";
        public const string ActionType_Remove = "remove";

        public const string PluginType_LightSetup = "lightSetup";
        public const string PluginType_Capture = "capture";
        public const string PluginType_ColourExtraction = "colourExtraction";
        public const string PluginType_PostProcess = "postProcess";
        public const string PluginType_Output = "output";
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
        public IEnumerable<AvailablePlugin> AvailableLightSetupPlugins { get; set; }

        [DataMember(Name = "availableCapturePlugins")]
        public IEnumerable<AvailablePlugin> AvailableCapturePlugins { get; set; }

        [DataMember(Name = "availableColourExtractionPlugins")]
        public IEnumerable<AvailablePlugin> AvailableColourExtractionPlugins { get; set; }

        [DataMember(Name = "availablePostProcessPlugins")]
        public IEnumerable<AvailablePlugin> AvailablePostProcessPlugins { get; set; }

        [DataMember(Name = "availablePreOutputPlugins")]
        public IEnumerable<AvailablePlugin> AvailablePreOutputPlugins { get; set; }

        [DataMember(Name = "availableOutputPlugins")]
        public IEnumerable<AvailablePlugin> AvailableOutputPlugins { get; set; }
    }
    [DataContract]
    public class AvailablePlugin
    {
        [DataMember(Name="name")]
        public string Name { get; set; }
        [DataMember(Name="description")]
        public string Description { get; set; }
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
        public IEnumerable<AvailablePlugin> LightSetupPlugins { get; set; }
        [DataMember(Name = "capturePlugins")]
        public IEnumerable<AvailablePlugin> CapturePlugins { get; set; }
        [DataMember(Name = "colourExtractionPlugins")]
        public IEnumerable<AvailablePlugin> ColourExtractionPlugins { get; set; }
        [DataMember(Name = "postProcessPlugins")]
        public IEnumerable<AvailablePlugin> PostProcessPlugins { get; set; }
        [DataMember(Name = "preOutputPlugins")]
        public IEnumerable<AvailablePlugin> PreOutputPlugins { get; set; }
        [DataMember(Name = "outputPlugins")]
        public IEnumerable<AvailablePlugin> OutputPlugins { get; set; }
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
    #endregion

    public class AfterglowService : Service
    {
        #region Plugins Controller
        public object Get(AvailablePluginsRequest request)
        {
            AvailablePluginsResult result = new AvailablePluginsResult();
            
            result.AvailableLightSetupPlugins = (from p in Program.Runtime.Setup.AvailableLightSetupPlugins
                                     select new AvailablePlugin
                                     {
                                         Name = p.Name,
                                         Description = p.ToString()
                                     });
            result.AvailableCapturePlugins = (from p in Program.Runtime.Setup.AvailableCapturePlugins
                                              select new AvailablePlugin
                                              {
                                                  Name = p.Name,
                                                  Description = p.ToString()
                                              });
            result.AvailableColourExtractionPlugins = (from p in Program.Runtime.Setup.AvailableColourExtractionPlugins
                                              select new AvailablePlugin
                                              {
                                                  Name = p.Name,
                                                  Description = p.ToString()
                                              });
            result.AvailablePostProcessPlugins = (from p in Program.Runtime.Setup.AvailablePostProcessPlugins
                                              select new AvailablePlugin
                                              {
                                                  Name = p.Name,
                                                  Description = p.ToString()
                                              });
            result.AvailablePreOutputPlugins = (from p in Program.Runtime.Setup.AvailablePreOutputPlugins
                                              select new AvailablePlugin
                                              {
                                                  Name = p.Name,
                                                  Description = p.ToString()
                                              });
            result.AvailableOutputPlugins = (from p in Program.Runtime.Setup.AvailableOutputPlugins
                                              select new AvailablePlugin
                                              {
                                                  Name = p.Name,
                                                  Description = p.ToString()
                                              });

            return result;
        }
        #endregion

        #region Profiles Controller
        public object Get(ProfilesRequest request)
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
                                                select new AvailablePlugin
                                                {
                                                    Name = p.Name,
                                                    Description = p.ToString()
                                                });
                response.CapturePlugins = (from p in profile.CapturePlugins
                                                select new AvailablePlugin
                                                {
                                                    Name = p.Name,
                                                    Description = p.ToString()
                                                });
                response.ColourExtractionPlugins = (from p in profile.ColourExtractionPlugins
                                                select new AvailablePlugin
                                                {
                                                    Name = p.Name,
                                                    Description = p.ToString()
                                                });
                response.PostProcessPlugins = (from p in profile.PostProcessPlugins
                                                select new AvailablePlugin
                                                {
                                                    Name = p.Name,
                                                    Description = p.ToString()
                                                });
                response.PreOutputPlugins = (from p in profile.PreOutputPlugins
                                                select new AvailablePlugin
                                                {
                                                    Name = p.Name,
                                                    Description = p.ToString()
                                                });
                response.OutputPlugins = (from p in profile.OutputPlugins
                                                select new AvailablePlugin
                                                {
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
        
        public static bool Active = false;

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

        public object Post(UpdateProfile updateProfile)
        {
            if (updateProfile != null && updateProfile.actionType == UpdateProfile.ActionType_AddProfile)
            {
                Profile profile = Program.Runtime.Setup.AddNewProfile();
                profile.Name = updateProfile.name;
                profile.Description = updateProfile.description;
                profile.CaptureFrequency = updateProfile.captureFrequency;
                profile.OutputFrequency = updateProfile.outputFrequency;

                Program.Runtime.Save();
            }

            if (updateProfile != null && updateProfile.profileId != 0)
            {

                Profile profile = (from p in Program.Runtime.Setup.Profiles
                                   where p.Id == updateProfile.profileId
                                   select p).FirstOrDefault();
                
                if (profile == null) return "Fail";

                if (updateProfile.actionType == UpdateProfile.ActionType_Update)
                {
                    profile.Name = updateProfile.name;
                    profile.Description = updateProfile.description;
                    profile.CaptureFrequency = updateProfile.captureFrequency;
                    profile.OutputFrequency = updateProfile.outputFrequency;

                    Program.Runtime.Save();
                }
                else if (updateProfile.actionType == UpdateProfile.ActionType_RemoveProfile)
                {
                    Program.Runtime.Setup.Profiles.Remove(profile);

                    Program.Runtime.Save();
                }
                else if (updateProfile.pluginType == UpdateProfile.PluginType_LightSetup)
                {
                    if (updateProfile.actionType == UpdateProfile.ActionType_Remove) return "Error: Not supported for Light Setup Plugins";

                    ILightSetupPlugin plugin = (from pl in Program.Runtime.Setup.ConfiguredLightSetupPlugins
                                                where pl.Id == updateProfile.pluginId
                                                select pl).FirstOrDefault();

                    //Ensure that the profile does not already use the selected plugin
                    if (plugin != null && profile.LightSetupPlugin != null && profile.LightSetupPlugin.Id != updateProfile.pluginId)
                    {
                        profile.LightSetupPlugins.Clear();
                        profile.LightSetupPlugins.Add(plugin);

                        Program.Runtime.Save();
                    }
                }
                else if (updateProfile.pluginType == UpdateProfile.PluginType_Capture)
                {
                    if (updateProfile.actionType == UpdateProfile.ActionType_Remove) return "Error: Not supported for Capture Plugins";

                    ICapturePlugin plugin = (from pl in Program.Runtime.Setup.ConfiguredCapturePlugins
                                             where pl.Id == updateProfile.pluginId
                                             select pl).FirstOrDefault();

                    //Ensure that the profile does not already use the selected plugin
                    if (plugin != null && profile.CapturePlugin != null && profile.CapturePlugin.Id != updateProfile.pluginId)
                    {
                        profile.CapturePlugins.Clear();
                        profile.CapturePlugins.Add(plugin);

                        Program.Runtime.Save();
                    }
                }
                else if (updateProfile.pluginType == UpdateProfile.PluginType_ColourExtraction)
                {
                    if (updateProfile.actionType == UpdateProfile.ActionType_Remove) return "Error: Not supported for Colour Extraction Plugins";

                    IColourExtractionPlugin plugin = (from pl in Program.Runtime.Setup.ConfiguredColourExtractionPlugins
                                                      where pl.Id == updateProfile.pluginId
                                                      select pl).FirstOrDefault();

                    //Ensure that the profile does not already use the selected plugin
                    if (plugin != null && profile.ColourExtractionPlugin != null && profile.ColourExtractionPlugin.Id != updateProfile.pluginId)
                    {
                        profile.ColourExtractionPlugins.Clear();
                        profile.ColourExtractionPlugins.Add(plugin);

                        Program.Runtime.Save();
                    }
                }
                else if (updateProfile.pluginType == UpdateProfile.PluginType_PostProcess)
                {
                    IPostProcessPlugin plugin = (from pl in Program.Runtime.Setup.ConfiguredPostProcessPlugins
                                                 where pl.Id == updateProfile.pluginId
                                                 select pl).FirstOrDefault();

                    if (updateProfile.actionType == UpdateProfile.ActionType_Add)
                    {
                        //Ensure that the profile does not already use the selected plugin
                        if (plugin != null && !(from pp in profile.PostProcessPlugins
                                                where pp.Id == updateProfile.pluginId
                                                select pp).Any())
                        {
                            profile.PostProcessPlugins.Add(plugin);
                            Program.Runtime.Save();
                        }
                    }
                    else if (updateProfile.actionType == UpdateProfile.ActionType_Remove)
                    {
                        //Ensure that the profile does already use the selected plugin
                        if (plugin != null && (from pp in profile.PostProcessPlugins
                                               where pp.Id == updateProfile.pluginId
                                               select pp).Any())
                        {
                            profile.PostProcessPlugins.Remove(plugin);
                            Program.Runtime.Save();
                        }
                    }
                }
                else if (updateProfile.pluginType == UpdateProfile.PluginType_Output)
                {
                    IOutputPlugin plugin = (from pl in Program.Runtime.Setup.ConfiguredOutputPlugins
                                            where pl.Id == updateProfile.pluginId
                                            select pl).FirstOrDefault();

                    if (updateProfile.actionType == UpdateProfile.ActionType_Add)
                    {
                        //Ensure that the profile does not already use the selected plugin
                        if (plugin != null && !(from op in profile.OutputPlugins
                                                where op.Id == updateProfile.pluginId
                                                select op).Any())
                        {
                            profile.OutputPlugins.Add(plugin);
                            Program.Runtime.Save();
                        }
                    }
                    else if (updateProfile.actionType == UpdateProfile.ActionType_Remove)
                    {
                        //Ensure that the profile does already use the selected plugin
                        if (plugin != null && (from op in profile.OutputPlugins
                                               where op.Id == updateProfile.pluginId
                                               select op).Any())
                        {
                            profile.OutputPlugins.Remove(plugin);
                            Program.Runtime.Save();
                        }
                    }
                }

                return Program.Runtime.Setup;
            }
            return "Fail";
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
