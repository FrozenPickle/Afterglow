using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ServiceStack.ServiceInterface;
using ServiceStack.ServiceHost;
using ServiceStack.Text;
using System.Runtime.Serialization;
using Afterglow.Core;
using Afterglow.Core.Plugins;

namespace Afterglow.Web
{
    [Route("/runtime")]
    public class Runtime
    {
    }

    public class RuntimeResponse
    {
        public bool Active { get; set; }
        public int NumberOfLights { get; set; }
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
        public int ProfileId { get; set; }
        public int PluginId { get; set; }
        public string PluginType { get; set; }
        public string ActionType { get; set; }

        public const string ActionType_Add = "add";
        public const string ActionType_Remove = "remove";

        public const string PluginType_LightSetup = "lightSetup";
        public const string PluginType_Capture = "capture";
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
        public int Index { get; set; }
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
                Active = Program.Active,
                NumberOfLights = Program.Runtime.CurrentProfile.LightSetupPlugin.Lights.Count
            };
        }

        public object Post(Runtime request)
        {
            Program.ToggleActive();
            return new RuntimeResponse
            {
                Active = Program.Active,
                NumberOfLights = Program.Runtime.CurrentProfile.LightSetupPlugin.Lights.Count
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
            if (updateProfile != null && updateProfile.ProfileId != 0)
            {

                Profile profile = (from p in Program.Runtime.Setup.Profiles
                                   where p.Id == updateProfile.ProfileId
                                   select p).FirstOrDefault();
                
                if (profile == null) return "Fail";

                if (updateProfile.PluginType == UpdateProfile.PluginType_LightSetup)
                {
                    ILightSetupPlugin plugin = (from pl in Program.Runtime.Setup.ConfiguredLightSetupPlugins
                                                where pl.Id == updateProfile.PluginId
                                                select pl).FirstOrDefault();

                    //Ensure that the profile does not already use the selected plugin
                    if (plugin != null && profile.LightSetupPlugin.Id != updateProfile.PluginId)
                    {
                        profile.LightSetupPlugins.Clear();
                        profile.LightSetupPlugins.Add(plugin);

                        Program.Runtime.Save();
                    }
                }
                else if (updateProfile.PluginType == UpdateProfile.PluginType_Capture)
                {
                    ICapturePlugin plugin = (from pl in Program.Runtime.Setup.ConfiguredCapturePlugins
                                                where pl.Id == updateProfile.PluginId
                                                select pl).FirstOrDefault();

                    //Ensure that the profile does not already use the selected plugin
                    if (plugin != null && profile.CapturePlugin.Id != updateProfile.PluginId)
                    {
                        profile.CapturePlugins.Clear();
                        profile.CapturePlugins.Add(plugin);

                        Program.Runtime.Save();
                    }
                }
                else if (updateProfile.PluginType == UpdateProfile.PluginType_PostProcess)
                {
                    IPostProcessPlugin plugin = (from pl in Program.Runtime.Setup.ConfiguredPostProcessPlugins
                                                where pl.Id == updateProfile.PluginId
                                                select pl).FirstOrDefault();

                    //Ensure that the profile does not already use the selected plugin
                    if (plugin != null && !(from pp in profile.PostProcessPlugins
                                            where pp.Id == updateProfile.PluginId
                                            select pp).Any())
                    {
                        profile.PostProcessPlugins.Add(plugin);
                        Program.Runtime.Save();
                    }
                }
                else if (updateProfile.PluginType == UpdateProfile.PluginType_Output)
                {
                    IOutputPlugin plugin = (from pl in Program.Runtime.Setup.ConfiguredOutputPlugins
                                                where pl.Id == updateProfile.PluginId
                                                select pl).FirstOrDefault();

                    //Ensure that the profile does not already use the selected plugin
                    if (plugin != null && !(from op in profile.OutputPlugins
                                             where op.Id == updateProfile.PluginId
                                             select op).Any())
                    {
                        profile.OutputPlugins.Add(plugin);
                        Program.Runtime.Save();
                    }
                }

                return profile;
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
                         select new LightPreview() { Index = light.Index, Colour = light.LightColour }).ToList();
            return new PreviewResponse
            {
                Lights = lights
            };
        }
    }
}
