using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Afterglow.Core;
using Afterglow.Core.Plugins;
using Afterglow.Core.UI;
using Afterglow.Core.UI.Controls;
using Afterglow.UserControls;
using System.Collections.ObjectModel;

namespace Afterglow.Forms
{
    public partial class SettingsForm : Form
    {
        private AfterglowRuntime _runtime;
        private Dictionary<TreeNode, BaseControl> _nodeUserControl;
        private MainForm _mainForm;
        
        public SettingsForm(AfterglowRuntime runtime, MainForm mainForm)
        {
            this._runtime = runtime;
            this._mainForm = mainForm;
            InitializeComponent();
            LoadTreeView();
        }

        private void LoadTreeView()
        {
            if (_runtime == null)
                return;

            _nodeUserControl = new Dictionary<TreeNode, BaseControl>();

            TreeNode rootNode = new TreeNode("Afterglow");
            rootNode.Tag = _runtime;
            AfterglowSettingsUserControl settingsUserControl = new UserControls.AfterglowSettingsUserControl(_runtime);
            _nodeUserControl.Add(rootNode, settingsUserControl);
            rootNode.Expand();
            tvSettings.Nodes.Add(rootNode);
            settingsUserControl.PluginsChanged += PluginSelectPluginsChanged;

            AddProfileNodes();
        }

        private void AddProfileNodes()
        {
            if (tvSettings.Nodes[0].Nodes != null)
            {
                tvSettings.Nodes[0].Nodes.Clear();
            }
            if (_runtime.Settings.Profiles != null)
            {
                foreach (Profile profile in _runtime.Settings.Profiles)
                {

                    TreeNode profileNode = new TreeNode(profile.Name);
                    _nodeUserControl.Add(profileNode, new ProfileSettingsUserControl(profile));
                    tvSettings.Nodes[0].Nodes.Add(profileNode);

                    #region Light Setup Node
                    TreeNode lightSetupNode = new TreeNode("Light Setup");
                    LightSetupPluginSelectUserControl lightSetupPluginSelect = new LightSetupPluginSelectUserControl(profile);
                    _nodeUserControl.Add(lightSetupNode, lightSetupPluginSelect);
                    profileNode.Nodes.Add(lightSetupNode);
                    lightSetupPluginSelect.PluginsChanged += PluginSelectPluginsChanged;

                    //Add Light Setup Plugins
                    if (profile.LightSetupPlugin != null)
                    {
                        SetChildNodes(lightSetupNode, profile.LightSetupPlugin);
                    }
                    #endregion

                    #region Capture Node
                    TreeNode captureNode = new TreeNode("Capture");
                    CapturePluginSelectUserControl capturePluginSelect = new CapturePluginSelectUserControl(profile);
                    _nodeUserControl.Add(captureNode, capturePluginSelect);
                    profileNode.Nodes.Add(captureNode);
                    capturePluginSelect.PluginsChanged += PluginSelectPluginsChanged;

                    //Add Capture Plugins
                    if (profile.CapturePlugin != null)
                    {
                        SetChildNodes(captureNode, profile.CapturePlugin);
                    }
                    #endregion

                    #region Colour Extraction Node
                    TreeNode colourExtractionNode = new TreeNode("Colour Extraction");
                    BaseControl colourExtractionPluginSelect = new ColourExtractionPluginSelectUserControl(profile);
                    _nodeUserControl.Add(colourExtractionNode, colourExtractionPluginSelect);
                    profileNode.Nodes.Add(colourExtractionNode);
                    colourExtractionPluginSelect.PluginsChanged += PluginSelectPluginsChanged;

                    //Colour Extraction Plugins
                    if (profile.ColourExtractionPlugin != null)
                    {
                        SetChildNodes(colourExtractionNode, profile.ColourExtractionPlugin);
                    }
                    #endregion

                    #region Post Process Node
                    TreeNode postProcessNode = new TreeNode("Post Process");
                    PostProcessPluginSelectUserControl postProcessPluginSelect = new PostProcessPluginSelectUserControl(profile);
                    _nodeUserControl.Add(postProcessNode, postProcessPluginSelect);
                    profileNode.Nodes.Add(postProcessNode);
                    postProcessPluginSelect.PluginsChanged += PluginSelectPluginsChanged;

                    //Post Process Plugins
                    if (profile.PostProcessPlugins != null)
                    {
                        SetChildNodes(postProcessNode, profile.PostProcessPlugins.ToList<IAfterglowPlugin>());
                    }
                    #endregion

                    #region Output Node
                    TreeNode outputNode = new TreeNode("Output");
                    OutputPluginSelectUserControl outputPluginSelect = new OutputPluginSelectUserControl(profile);
                    _nodeUserControl.Add(outputNode, outputPluginSelect);
                    profileNode.Nodes.Add(outputNode);
                    outputPluginSelect.PluginsChanged += PluginSelectPluginsChanged;

                    //Output Plugins
                    if (profile.OutputPlugins != null)
                    {
                        SetChildNodes(outputNode, profile.OutputPlugins.ToList<IAfterglowPlugin>());
                    }
                    #endregion
                }
            }
        }

        private void PluginSelectPluginsChanged(object sender, PluginsChangedEventArgs e)
        {
            if (e.Plugins != null)
            {
                SetChildNodes((from n in _nodeUserControl
                                    where n.Value == sender
                               select n.Key).FirstOrDefault(), e.Plugins.ToList<IAfterglowPlugin>());

            }
            else if (e.RefreshProfiles)
            {
                AddProfileNodes();
                _mainForm.RefreshProfiles();
            }
        }
            
        private void SetChildNodes(TreeNode parent, IAfterglowPlugin objectToAdd)
        {
            if (objectToAdd == null)
            {
                return;
            }
            List<IAfterglowPlugin> list = new List<IAfterglowPlugin>();
            list.Add(objectToAdd);
            SetChildNodes(parent, list);
        }

        private void SetChildNodes(TreeNode parent, List<IAfterglowPlugin> objectsToAdd)
        {
            if (objectsToAdd == null)
            {
                return;
            }

            //Clear Current Nodes
            foreach (TreeNode item in parent.Nodes)
            {
                _nodeUserControl.Remove(item);
            }
            parent.Nodes.Clear();

            //Plugins
            foreach (IAfterglowPlugin plugin in objectsToAdd)
            {
                TreeNode pluginNode = new TreeNode(plugin.DisplayName);
                _nodeUserControl.Add(pluginNode, new AutoGenPluginUserControl(plugin));
                parent.Nodes.Add(pluginNode);
            }
        }

        private void tvSettings_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ShowSettings(e.Node);
        }

        private void ShowSettings(TreeNode treeNode)
        {
            lblCurrentSetting.Text = treeNode.FullPath;

            panelPlaceHolder.Controls.Clear();
            _nodeUserControl[treeNode].Parent = panelPlaceHolder;
            _nodeUserControl[treeNode].Height = panelPlaceHolder.Height;
            _nodeUserControl[treeNode].Width = panelPlaceHolder.Width;
            _nodeUserControl[treeNode].Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
            panelPlaceHolder.Controls.Add(_nodeUserControl[treeNode]);
        }

    }
}
