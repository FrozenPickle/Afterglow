using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Afterglow.Core.Load;
using Afterglow.Plugins.Capture;
using Afterglow.Plugins.ColourExtraction;
//using Afterglow.Plugins.PostProcess;
using Afterglow.Plugins.Output;
using Afterglow.Core.Plugins;


namespace Afterglow.Loader
{

    public class PluginLoader : ILoader
    {
        //TODO replace this function with dynamic loading of assemblies
        public PluginLoader()
        {
            Afterglow.Plugins.Output.ArduinoOutput a = new ArduinoOutput();
            Afterglow.Plugins.Output.DebugOutput b = new DebugOutput();
            Afterglow.Plugins.Capture.CopyScreenCapture c = new CopyScreenCapture();
            Afterglow.DirectX.Plugin.Direct3DCapture d3d = new DirectX.Plugin.Direct3DCapture();
            Afterglow.Plugins.ColourExtraction.AverageColourExtraction d = new AverageColourExtraction();
            Afterglow.Plugins.LightSetup.BasicLightSetupPlugin.BasicLightSetup e = new Plugins.LightSetup.BasicLightSetupPlugin.BasicLightSetup();
            Afterglow.Plugins.LightSetup.BasicLightSetupPlugin.BasicLightSetupUserControl f = new Plugins.LightSetup.BasicLightSetupPlugin.BasicLightSetupUserControl();
            Afterglow.Plugins.PostProcess.ColourCorrectionPostProcess g = new Plugins.PostProcess.ColourCorrectionPostProcess();
        }

        public Type GetObjectType(string typeName)
        {

            System.Type type = System.Type.GetType(typeName);

            if (type == null)
            {
                var types = from assembly in System.AppDomain.CurrentDomain.GetAssemblies()
                            from assemblyType in assembly.GetTypes()
                            where assemblyType.FullName == typeName
                            select assemblyType;

                type = types.FirstOrDefault();
            }

            return type;
        }

        public Type[] GetPlugins(Type pluginType)
        {
            Type[] plugins;

            plugins = (from assembly in System.AppDomain.CurrentDomain.GetAssemblies()
                        from assemblyType in assembly.GetTypes()
                        where assemblyType.IsInterface == false &&
                        (from i in assemblyType.GetInterfaces()
                         where i == pluginType
                         select i).Any()
                        select assemblyType).ToArray();

            return plugins;
        }
    }
}
