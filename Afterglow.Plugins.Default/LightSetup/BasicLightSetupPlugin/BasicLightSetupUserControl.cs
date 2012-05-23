using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Afterglow.Core.UI.Controls;
using Afterglow.Core.Plugins;
using Afterglow.Core;

namespace Afterglow.Plugins.LightSetup.BasicLightSetupPlugin
{
    public partial class BasicLightSetupUserControl : BaseControl
    {
        private BasicLightSetup _plugin;
        private Light _currentLight;
        private bool _allowEdit = true;

        private const int CELL_SIZE = 25;
        private Color[] CellColours = { Color.Blue, Color.Red, Color.Pink, Color.Green, Color.Yellow, Color.LimeGreen, Color.Violet, Color.Orange, Color.LightBlue };


        public BasicLightSetupUserControl()
        {
            InitializeComponent();
        }

        public BasicLightSetupUserControl(IAfterglowPlugin plugin)
        {
            InitializeComponent();
            this._plugin = plugin as BasicLightSetup;

            dgvRegions.AllowUserToOrderColumns = false;
            dgvRegions.AllowUserToResizeColumns = false;
            dgvRegions.AllowUserToResizeRows = false;
            dgvRegions.ShowEditingIcon = false;
            dgvRegions.AllowUserToAddRows = false;
            dgvRegions.AllowUserToDeleteRows = false;
            dgvRegions.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders | DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            dgvRegions.CellEnter += new DataGridViewCellEventHandler(dgvRegions_CellEnter);
            dgvRegions.KeyDown += new KeyEventHandler(dgvRegions_KeyDown);
            dgvRegions.CellValidating += new DataGridViewCellValidatingEventHandler(dgvRegions_CellValidating);
            dgvRegions.CellValidated += new DataGridViewCellEventHandler(dgvRegions_CellValidated);
            dgvRegions.PreviewKeyDown += new PreviewKeyDownEventHandler(dgvRegions_PreviewKeyDown);
            //dgvRegions.
            dgvRegions.Resize += new EventHandler(dgvRegions_Resize);
        }

        void dgvRegions_Resize(object sender, EventArgs e)
        {
            //SetUpGrid();
        }


        #region Keybourd Navigation
        void dgvRegions_CellValidated(object sender, DataGridViewCellEventArgs e)
        {
            _allowEdit = true;
        }

        void dgvRegions_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            e.Cancel = !_allowEdit;
        }

        void dgvRegions_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                case Keys.Back:
                case Keys.Delete:
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    break;
                default:
                    _allowEdit = false;
                    break;
            }
        }

        void dgvRegions_KeyDown(object sender, KeyEventArgs e)
        {
            
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    CreateRegion();
                    e.Handled = true;
                    break;
                case Keys.Back:
                case Keys.Delete:
                    DeleteRegion();
                    break;
                case Keys.Up:
                case Keys.Down:
                case Keys.Left:
                case Keys.Right:
                    break;
                default:
                    dgvRegions.InvalidateCell(lastCellLeft, lastCellTop);
                    break;
            }
        }
        #endregion

        int lastCellTop = 0;
        int lastCellLeft = 0;
        void dgvRegions_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == -1 || e.RowIndex == -1 ||
                (lastCellTop == e.RowIndex && lastCellLeft == e.ColumnIndex))
            {
                lastCellTop = e.RowIndex;
                lastCellLeft = e.ColumnIndex;
                //return;
            }
            else
            {
                lastCellTop = e.RowIndex;
                lastCellLeft = e.ColumnIndex;
            }

            //Get Light if it exists
            int index = 0;
            if (dgvRegions.Rows[e.RowIndex].Cells[e.ColumnIndex].Value != null && 
                int.TryParse(dgvRegions.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(), out index))
            {
                _currentLight = _plugin.Lights.Where(l => l.Index == index).FirstOrDefault();
            }
            else
            {
                _currentLight = null;
            }
            SetCurrentRegionValues();
        }

        private void SetCurrentRegionValues()
        {
            if (_currentLight != null)
            {
                //Set Numeric Up Down values
                nudIndex.Value = Convert.ToDecimal(_currentLight.Index);
                nudTop.Value = Convert.ToDecimal(_currentLight.Top);
                nudLeft.Value = Convert.ToDecimal(_currentLight.Left);
                nudHeight.Value = Convert.ToDecimal(_currentLight.Height);
                nudWidth.Value = Convert.ToDecimal(_currentLight.Width); 
                
                EnableCurrentRegion(true);
            }
            else
            {
                nudIndex.Value = 0;
                nudTop.Value = 0;
                nudLeft.Value = 0;
                nudHeight.Value = 1;
                nudWidth.Value = 1;

                EnableCurrentRegion(false);
            }
            if (_currentLight != null)
            {
                dgvRegions.Rows[_currentLight.Top.Value].Cells[_currentLight.Left.Value].Selected = true;
            }
            dgvRegions.Select();
            SetUpGrid();
        }

        private void EnableCurrentRegion(bool enable)
        {
            nudIndex.Enabled = enable;
            nudTop.Enabled = false;
            nudLeft.Enabled = false;
            nudHeight.Enabled = enable;
            nudWidth.Enabled = enable;
            btnCreate.Visible = !enable;
            btnDelete.Visible = enable;
        }

        private void BasicLightSetupUserControl_Load(object sender, EventArgs e)
        {
            //TODO get min and max from attributes

            nudHigh.DecimalPlaces = 0;
            nudHigh.Value = Convert.ToDecimal(_plugin.NumberOfLightsHigh);
            nudHigh.Minimum = Convert.ToDecimal(1);
            nudHigh.Maximum = Convert.ToDecimal(999);
            nudHigh.ValueChanged += new EventHandler(nudHigh_ValueChanged);

            nudWide.DecimalPlaces = 0;
            nudWide.Value = Convert.ToDecimal(_plugin.NumberOfLightsWide);
            nudWide.Minimum = Convert.ToDecimal(1);
            nudWide.Maximum = Convert.ToDecimal(999);
            nudWide.ValueChanged += new EventHandler(nudWide_ValueChanged);

            nudIndex.DecimalPlaces = 0;
            nudIndex.Minimum = Convert.ToDecimal(0);
            nudIndex.Maximum = Convert.ToDecimal(999);
            nudIndex.ValueChanged += new EventHandler(nudIndex_ValueChanged);

            nudTop.DecimalPlaces = 0;
            nudTop.Minimum = Convert.ToDecimal(0);
            nudTop.Maximum = Convert.ToDecimal(999);
            nudTop.ValueChanged += new EventHandler(nudTop_ValueChanged);

            nudLeft.DecimalPlaces = 0;
            nudLeft.Minimum = Convert.ToDecimal(0);
            nudLeft.Maximum = Convert.ToDecimal(999);
            nudLeft.ValueChanged += new EventHandler(nudLeft_ValueChanged);

            nudHeight.DecimalPlaces = 0;
            nudHeight.Minimum = Convert.ToDecimal(1);
            nudHeight.Maximum = Convert.ToDecimal(999);
            nudHeight.ValueChanged += new EventHandler(nudHeight_ValueChanged);

            nudWidth.DecimalPlaces = 0;
            nudWidth.Minimum = Convert.ToDecimal(1);
            nudWidth.Maximum = Convert.ToDecimal(999);
            nudWidth.ValueChanged += new EventHandler(nudWidth_ValueChanged);

            SetUpGrid(true);
        }

        private bool _isSettingUp = false;
        private void SetUpGrid(bool? forceRedraw = null)
        {
            if (_isSettingUp)
            {
                return;
            }

            _isSettingUp = true;

            //if Grid has changed in size then resize it
            if (forceRedraw == true || (dgvRegions.RowCount != _plugin.NumberOfLightsHigh.Value ||
                dgvRegions.ColumnCount != _plugin.NumberOfLightsWide.Value))
            {
                dgvRegions.RowCount = _plugin.NumberOfLightsHigh.Value;
                dgvRegions.ColumnCount = _plugin.NumberOfLightsWide.Value;

                for (int i = 0; i < dgvRegions.Columns.Count; i++)
                {
                    dgvRegions.Columns[i].HeaderText = i.ToString();
                    dgvRegions.Columns[i].Width = CELL_SIZE;
                }
                for (int i = 0; i < dgvRegions.Rows.Count; i++)
                {
                    dgvRegions.Rows[i].HeaderCell.Value = i.ToString();
                    dgvRegions.Rows[i].Height = CELL_SIZE;
                }
            }
            
            //Add indexs and merge
            List<Light> lightsToRemove = new List<Light>();
            MergeCells(0, 0, dgvRegions.RowCount, dgvRegions.ColumnCount, true);

            foreach (Light light in _plugin.Lights)
            {
                try
                {
                    for (int row = light.Top.Value; row < light.Top.Value + light.Height; row++)
                    {
                        for (int column = light.Left.Value; column < light.Left.Value + light.Width.Value; column++)
                        {
                            _allowEdit = true;
                            dgvRegions.Rows[row].Cells[column].Value = light.Index;
                        }
                    }
                    MergeCells(light.Top.Value, light.Left.Value, light.Height.Value, light.Width.Value, false);
                }
                catch
                {
                    lightsToRemove.Add(light);
                }
            }
            lightsToRemove.ForEach(l => _plugin.RemoveLight(l));
            _isSettingUp = false;
        }


        private void ClearGridFocus()
        {
            _currentLight = null;
            SetCurrentRegionValues();
        }

        private void MergeCells(int top, int left, int height, int width, bool reset)
        {
            DataGridViewCellStyle CellStyle = new DataGridViewCellStyle();
            int result = 0;

            for (int row = top; row < top + height; row++)
			{
                for (int column = left; column < left + width; column++)
                {

                    if (!reset && 
                        dgvRegions.Rows[row].Cells[column].Value != null && 
                        int.TryParse(dgvRegions.Rows[row].Cells[column].Value.ToString(), out result))
                    {
                        //int index = random.Next(0, CellColours.Length); // list.Count for List<T>
                        Color value = CellColours[result % CellColours.Length];
                        CellStyle.BackColor = value;
                    }
                    dgvRegions.Rows[row].Cells[column].Style = CellStyle;
                    if (reset)
                    {
                        dgvRegions.Rows[row].Cells[column].Value = "";
                    }
                }
			}
        }

        internal void SetCellColours(IEnumerable<Light> lights)
        {
            foreach (var light in lights)
            {
                if (light.Top.HasValue && light.Left.HasValue)
                {
                    dgvRegions.Rows[light.Top.Value].Cells[light.Left.Value].Style = new DataGridViewCellStyle() { BackColor = light.LEDColour };
                }
            }
        }

        #region ValueChanged
        void nudWidth_ValueChanged(object sender, EventArgs e)
        {
            if (_currentLight != null)
            {
                _currentLight.Width = Convert.ToInt32(nudWidth.Value);
            }
            SetUpGrid(true);
        }

        void nudHeight_ValueChanged(object sender, EventArgs e)
        {
            if (_currentLight != null)
            {
                int value = Convert.ToInt32(nudHeight.Value);
                
                _currentLight.Height = value;
            }
            SetUpGrid(true);
        }

        void nudLeft_ValueChanged(object sender, EventArgs e)
        {
            if (_currentLight != null)
            {
                _currentLight.Left = Convert.ToInt32(nudLeft.Value);
            }
            SetUpGrid();
        }

        void nudTop_ValueChanged(object sender, EventArgs e)
        {
            if (_currentLight != null)
            {
                _currentLight.Top = Convert.ToInt32(nudTop.Value);
            }
            SetUpGrid();
        }

        void nudIndex_ValueChanged(object sender, EventArgs e)
        {
            if (_currentLight != null)
            {
                _currentLight.Index = Convert.ToInt32(nudIndex.Value);
            }
            SetUpGrid();
        }

        void nudWide_ValueChanged(object sender, EventArgs e)
        {
            if (_plugin != null)
            {
                _plugin.NumberOfLightsWide = Convert.ToInt32(nudWide.Value);
            }
            SetUpGrid();
        }

        void nudHigh_ValueChanged(object sender, EventArgs e)
        {
            if (_plugin != null)
            {
                _plugin.NumberOfLightsHigh = Convert.ToInt32(nudHigh.Value);
            }
            SetUpGrid();
        }
        #endregion

        #region Button Clicks
        private void btnCreate_Click(object sender, EventArgs e)
        {
            CreateRegion();
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            DeleteRegion();
        }

        private void btnDeleteAll_Click(object sender, EventArgs e)
        {
            DeleteAllRegions();
        }
        #endregion

        #region Create/Delete Regions
        private void CreateRegion()
        {
            if (btnCreate.Visible && _currentLight == null)
            {
                _currentLight = _plugin.AddLight();
                _currentLight.Index = _plugin.Lights.Max(l => l.Index).GetValueOrDefault(-1) + 1;
                _currentLight.Top = lastCellTop;
                _currentLight.Left = lastCellLeft;
                _currentLight.Height = 1;
                _currentLight.Width = 1;

                _allowEdit = true;
                SetUpGrid();

                SetCurrentRegionValues();
            }
        }

        private void DeleteRegion()
        {
            if (btnDelete.Visible && _currentLight != null)
            {
                _plugin.RemoveLight(_currentLight);

                ClearGridFocus();
                SetUpGrid(true);
            }
        }

        private void DeleteAllRegions()
        {
            ClearGridFocus();

            List<Light> lightsToRemove = new List<Light>();
            foreach (Light light in _plugin.Lights)
            {
                lightsToRemove.Add(light);
            }
            lightsToRemove.ForEach(l => _plugin.RemoveLight(l));

            SetUpGrid(true);
        }
        #endregion
    }
}
