using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters;
using DWOS.Data.Process;
using NLog;

namespace DWOS.UI.Processing
{
    public partial class SampleSetEntry : Form
    {
        
        #region Properties
     
        public List<string> SampleValues { get; set; }

        public int SampleSize
        {
            get
            {
                return this._SampleSize;
            }
            set
            {
                this._SampleSize = value;
            }
        }

        public double DefaultValue
        {
            get
            {
                return this._DefaultValue;
            }
            set
            {
                this._DefaultValue = value;
            }
        }

        public Double AvgMax
        {
            get
            {
                return this._AvgMax;
            }
            set
            {
                this._AvgMax = value;
            }
        }

        public Double AvgMin
        {
            get
            {
                return this._AvgMin;
            }
            set
            {
                this._AvgMin = value;
            }
        }

        public int ParentID
        {
            get
            {
                return this._ParentID;
            }
            set
            {
                this._ParentID = value;
            }
        }

        #endregion


        #region Fields

        private int _ParentID = 0;
        

        private int _SampleSize = 1;

        private Double _DefaultValue = 0;

        private Double _AvgMin = 0;

        private Double _AvgMax = 0;
       

        bool newRowNeeded;

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        const int initialSize = 0;

        int numberOfRows = initialSize;

        const int initialValue = -1;

        #endregion

        #region Methods

        
        public SampleSetEntry()
        {
            InitializeComponent();
        }

        public void LoadSamples()
        {
            try
            {
                if (SampleValues == null)
                    SampleValues = new List<string>();
                for (var i = 1; i <= SampleValues.Count; i++)
                {
                    this.dgrSampleSet.Rows.Add(i.ToString(),SampleValues[i-1]);

                }

                this.tbxAverageValue.Text = _DefaultValue.ToString();
                this.txtSampleSize.Text = _SampleSize.ToString();
                this.txtMinMax.Text = _AvgMin.ToString() + " - " + _AvgMax.ToString();
                
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error Loading Samples dialog");
            }

        }

        private void averageValues()
        {
            try
            {

                double total = 0;
                var valueList = new List<Double> { };

                foreach (DataGridViewRow r in this.dgrSampleSet.Rows)
                {
                    if ((r.Cells[1].Value is DBNull) || (r.Cells[1].Value == null) || (r.Cells[1].Value.ToString() == ""))
                        continue;
                    total += Convert.ToDouble(r.Cells[1].Value);
                    if ((r.Cells[0].Value.ToString() != "") && (Convert.ToDouble(r.Cells[0].Value) > 0))
                        valueList.Add(Convert.ToDouble(r.Cells[1].Value));
                }

                this.lblMinVal.Text = valueList.Min(z => z).ToString();

                this.lblMaxVal.Text = valueList.Max(z => z).ToString();

                
                this.tbxAverageValue.Value = valueList.Average(z => z).ToString(); ;

                this.tbxAverageValue.Text = this.tbxAverageValue.Value.ToString();

                this.txtSampleSize.Appearance.ForeColor = (this.dgrSampleSet.Rows.Count <= _SampleSize) ? Color.Red : Color.Black;

            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error averaging sample values.");
            }
        }

        private void UpdateIndex()
        {
            
            for(var i = 1;i < this.dgrSampleSet.Rows.Count;i++)
            {
                DataGridViewRow r = this.dgrSampleSet.Rows[i-1];
                r.Cells[0].Value = r.Index + 1;
            }

        }

        #endregion

        #region Events

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult sampleWarningResults = DialogResult.OK;
                if (this.dgrSampleSet.Rows.Count < _SampleSize)
                {
                    sampleWarningResults = MessageBox.Show("Minumum samples have not been collected. \n Are you sure you wish to exit?", "Insufficient Samples", MessageBoxButtons.OKCancel);
                    //SampleValues = dgrSampleSet..AsEnumerable().Select(x => x["SampleValue"].ToString()).ToList(); _
                    foreach (DataGridViewRow row in dgrSampleSet.Rows)
                        SampleValues.Add(row.Cells[1].ToString());


                    if (sampleWarningResults == DialogResult.Cancel)
                        return;
                }
                if (sampleWarningResults == DialogResult.OK)
                {
                    this.DialogResult = DialogResult.OK;
                    this.Hide();
                    SampleValues.Clear();
                    foreach (DataGridViewRow row in dgrSampleSet.Rows)
                        if(row.Cells[1].Value != null)
                            SampleValues.Add(row.Cells[1].Value.ToString());
                }

            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error Closing Dialog");
            }

        }

        private void dgrSampleSet_NewRowNeeded(object sender, DataGridViewRowEventArgs e)
        {
            newRowNeeded = true;
        }

        private void dgrSampleSet_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {

            if (newRowNeeded)
            {
                newRowNeeded = false;
                numberOfRows = numberOfRows + 1;

            }
        }

        private void dgrSampleSet_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {

            if (store.ContainsKey(e.RowIndex))
            {
                // Use the store if the e value has been modified 
                // and stored.            
                e.Value = store[e.RowIndex];
            }
            else if (newRowNeeded && e.RowIndex == numberOfRows)
            {
                if (dgrSampleSet.IsCurrentCellInEditMode)
                {
                    e.Value = initialValue;
                }
                else
                {
                    // Show a blank value if the cursor is just resting
                    // on the last row.
                    e.Value = String.Empty;
                }
            }
            else
            {
                e.Value = e.RowIndex + 1;
            }
        }


        private Dictionary<int, int> store = new Dictionary<int, int>();

        private void dgrSampleSet_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {

            double abalbeginVal;
            bool parsed = double.TryParse(e.FormattedValue.ToString(), out abalbeginVal);
            if ((parsed && abalbeginVal >= 0.0) && (dgrSampleSet.Rows[e.RowIndex].Cells[0].Value != null))
            {
               
            }
            else if ((dgrSampleSet.Rows[e.RowIndex].Cells[1].Value == null) && (dgrSampleSet.Rows[e.RowIndex].Cells[0].Value == null))
            { 
            
            }
            else
            {
               // if (dgrSampleSet.Rows[e.RowIndex].Cells[0].Value != null)
                //{
                    e.Cancel = true;
                    dgrSampleSet.Rows[e.RowIndex].Cells[1].Selected = true;
                    dgrSampleSet.BeginEdit(true);
              // }
            }
        }

        private void dgrSampleSet_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow Row = ((DataGridView)sender).Rows[e.RowIndex];
            if (e.ColumnIndex == 1)
            {
                if ((Row.Cells[0].Value == null) && (Row.Cells[1].Value != null))
                    ((DataGridView)sender).Rows[e.RowIndex].Cells[0].Value = (e.RowIndex + 1).ToString();
                
                averageValues();
            
            }
            
        }

        private void SampleSetEntry_Load(object sender, EventArgs e)
        {
            averageValues();
        }



        #endregion

        private void dgrSampleSet_UserDeletedRow(object sender, DataGridViewRowEventArgs e)
        {
            UpdateIndex();
            averageValues();
        }
    }
}
