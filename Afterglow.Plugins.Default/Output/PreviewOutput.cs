using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Afterglow.Core.Plugins;
using System.Windows.Forms;
using Afterglow.Core;
using System.Threading.Tasks;
using Afterglow.Plugins.LightSetup.BasicLightSetupPlugin;
using System.Runtime.Serialization;

namespace Afterglow.Plugins.Output
{
    [DataContract]
    public class PreviewOutput: BasePlugin, IOutputPlugin
    {
        #region Read-only properties
        [DataMember]
        public override string Name
        {
            get { return "Light Preview Output"; }
        }

        [DataMember]
        public override string Author
        {
            get { return "Jono C. and Justin S."; }
        }

        [DataMember]
        public override string Description
        {
            get { return "Displays light colours on a form for previewing output"; }
        }

        [DataMember]
        public override string Website
        {
            get { return "https://github.com/FrozenPickle/Afterglow"; }
        }

        [DataMember]
        public override Version Version
        {
            get { return new Version(1, 0, 0); }
        }
        #endregion

        Form _previewForm;
        
        public override void Start()
        {
            Task _task = new Task(() =>
            {
                _previewForm = new Form();
                _previewForm.Width = 1024;
                _previewForm.Height = 768;
                //_lightControlDisplay = new BasicLightSetupUserControl(this.Runtime.CurrentProfile.OLDLightSetupPlugin) { Top = 0, Left = 0, Width = 1020, Height = 768, Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left };

                //_previewForm.Controls.Add(_lightControlDisplay);
                
                Application.Run(_previewForm);
            });

            _task.Start();
        }

        public override void Stop()
        {
            try
            {
                _previewForm.Invoke(new Action(() => { _previewForm.Close(); }));
            }
            catch
            {
                // Ignore errors while trying to close the form if already closed
            }
        }

        public void Output(List<Core.Light> lights)
        {
            try
            {
                if (!_previewForm.IsDisposed && _previewForm != null)
                {
                    _previewForm.Invoke(new Action(() =>
                    {
                        try
                        {
                            //_lightControlDisplay.SetCellColours(lights);
                        }
                        catch
                        {
                        }
                    }));
                }
            }
            catch
            {
            }
        }
    }
}
