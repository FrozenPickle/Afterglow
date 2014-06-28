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
        [Route("previewSetup")]
        public PreviewSetupResponse GetPreviewSetup()
        {
            PreviewSetupResponse response = new PreviewSetupResponse();
            if (Program.Runtime.CurrentProfile != null)
            {
                response.LightSetup = GetLightSetup(Program.Runtime.CurrentProfile.LightSetupPlugin);
            }
            return response;
        }

        [Route("previewLights")]
        public PreviewLightResponse GetPreviewLights()
        {
            if (Program.Runtime.CurrentProfile == null)
            {
                return new PreviewLightResponse
                {
                    Lights = new List<LightPreview>(),
                    CaptureFPS = 0.00,
                    CaptureFrameTime = 0.00,
                    OutputFPS = 0.00,
                    OutputFrameTime = 0.00
                };
            }

            List<LightPreview> lights = new List<LightPreview>(Program.Runtime.CurrentProfile.LightSetupPlugin.Lights.Count);
            // Retrieve previous final light output data
            var lightData = Program.Runtime.GetPreviousLightData();
            if (lightData != null)
            {
                for (var i = 0; i < Program.Runtime.CurrentProfile.LightSetupPlugin.Lights.Count; i++)
                {
                    var light = Program.Runtime.CurrentProfile.LightSetupPlugin.Lights[i];
                    lights.Add(new LightPreview() { Top = light.Top, Left = light.Left, Colour = System.Drawing.ColorTranslator.ToHtml(lightData[i]) });
                }
            }
            return new PreviewLightResponse
            {
                Lights = lights,
                CaptureFPS = GetJsonDouble(Program.Runtime.CaptureLoopFPS),
                CaptureFrameTime = GetJsonDouble(Program.Runtime.CaptureLoopFrameTime),
                OutputFPS = GetJsonDouble(Program.Runtime.OutputLoopFPS),
                OutputFrameTime = GetJsonDouble(Program.Runtime.OutputLoopFrameTime)
            };
        }

        private double GetJsonDouble(double value)
        {
            if (double.IsInfinity(value))
            {
                return 0.00d;
            }
            else
            {
                return Math.Round(value, 3);
            }
        }

        private LightSetup GetLightSetup(ILightSetupPlugin plugin)
        {
            LightSetup lightSetup = new LightSetup();

            if (plugin.Lights != null && plugin.Lights.Any())
            {
                //convert back to 2d array for setup
                lightSetup.LightRows = new List<LightRow>();

                lightSetup.NumberOfRows = plugin.Lights.Max(l => l.Top);
                lightSetup.NumberOfColumns = plugin.Lights.Max(l => l.Left);

                for (int row = 0; row <= lightSetup.NumberOfRows; row++)
                {
                    LightRow lightRow = new LightRow();
                    lightRow.RowIndex = row;
                    lightRow.LightColumns = new List<LightColumn>();
                    for (int column = 0; column <= lightSetup.NumberOfColumns; column++)
                    {
                        LightColumn lightColumn = new LightColumn();
                        lightColumn.ColumnIndex = column;
                        if (column == 0 || column == lightSetup.NumberOfColumns
                            || row == 0 || row == lightSetup.NumberOfRows)
                        {

                            Light light = (from l in plugin.Lights
                                           where l.Top == row
                                               && l.Left == column
                                           select l).FirstOrDefault();
                            if (light != null)
                            {
                                lightColumn.Id = light.Id.ToString();
                                lightColumn.Index = light.Index.ToString();

                                lightColumn.Enabled = !string.IsNullOrEmpty(lightColumn.Id);
                            }
                        }
                        else
                        {
                            lightColumn.Enabled = false;
                        }

                        lightRow.LightColumns.Add(lightColumn);
                    }
                    lightSetup.LightRows.Add(lightRow);
                }
            }
            return lightSetup;
        }
    }
}
