using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.Text;
using System.Runtime.Serialization;
using Afterglow.Core;
using Afterglow.Core.Plugins;
using ServiceStack;

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
        public long frameRateLimiter { get; set; }

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

    [Route("/profiles")]
    [Route("/profiles/{Id}")]
    [Route("/profiles/name/{Name}")]
    public class Profiles
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    [Route("/preview")]
    public class Preview
    {

    }

    public class PreviewResponse
    {
        public List<LightPreview> Lights { get; set; }
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

    [Csv(CsvBehavior.FirstEnumerable)]
    public class ProfilesResponse
    {
        public int Total { get; set; }
        public List<Afterglow.Core.Profile> Results { get; set; }
    }



    public class AfterglowService : Service
    {
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

        public object Get(Profiles request)
        {
            return new ProfilesResponse
            {
                Total = Program.Runtime.Setup.Profiles.Count,
                Results = !String.IsNullOrEmpty(request.Name)
                    ? Program.Runtime.Setup.Profiles.Where(p => p.Name.ToLower() == request.Name.ToLower()).Select(p => p).ToList()
                    : Program.Runtime.Setup.Profiles
            };
        }

        public object Post(UpdateProfile updateProfile)
        {
            if (updateProfile != null && updateProfile.actionType == UpdateProfile.ActionType_AddProfile)
            {
                Profile profile = Program.Runtime.Setup.AddNewProfile();
                profile.Name = updateProfile.name;
                profile.Description = updateProfile.description;
                profile.FrameRateLimiter = updateProfile.frameRateLimiter;

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
                    profile.FrameRateLimiter = updateProfile.frameRateLimiter;

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
            var lights = (from light in Program.Runtime.CurrentProfile.LightSetupPlugin.Lights
                         select new LightPreview() { Top = light.Top, Left = light.Left, Colour = light.LightColour }).ToList();
            return new PreviewResponse
            {
                Lights = lights
            };
        }
    }
}
