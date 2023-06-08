using System;
using System.Data;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Shared;
using Infragistics.Win.UltraWinGrid;
using NLog;
using System.Linq;

namespace DWOS.UI.Sales
{
    public partial class QuotePartFees: Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        private QuoteDataSet _dsQuotes;
        private int _quotePartID;

        #endregion

        #region Properties

        public QuoteDataSet QuotesDataset
        {
            get { return this._dsQuotes; }
            set { this._dsQuotes = value; }
        }

        public int QuotePartID
        {
            get { return this._quotePartID; }
            set
            {
                this._quotePartID = value;
                Text += " - " + this._quotePartID;
            }
        }

        #endregion

        #region Methods

        public QuotePartFees()
        {
            this.InitializeComponent();
        }

        private decimal CalculateDefaultFee(string feeTypeID)
        {
            if(String.IsNullOrEmpty(feeTypeID))
                return 0;
            else
            {
                QuoteDataSet.OrderFeeTypeRow oftRow = this.dsQuotes.OrderFeeType.FindByOrderFeeTypeID(feeTypeID);
                return oftRow != null ? oftRow.Price : 0;
            }
        }

        #endregion

        #region Events

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                //release variable before closing to prevent them from being disposed because they are used on parent form
                this.grdOrderFees.UpdateData();
                this.bsOrderFee.EndEdit();
                this.bsOrderFee = null;
                this.dsQuotes = null;
                this._dsQuotes = null;
                Close();
            }
            catch(Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error closing form.", exc);
                _log.Fatal(exc, "Error closing form.");
            }
        }

        private void OrderFees_Load(object sender, EventArgs e)
        {
            //rebind datasources
            this.dsQuotes = this._dsQuotes;
            this.bsOrderFee.DataSource = this._dsQuotes;
            this.bsOrderFee.Filter = "QuotePartID = " + this._quotePartID;
            this.grdOrderFees.DataBind();

            this.cboFeeType.DataSource = this.dsQuotes.OrderFeeType;
            this.cboFeeType.DisplayMember = this.dsQuotes.OrderFeeType.OrderFeeTypeIDColumn.ColumnName;
            this.cboFeeType.DataBind();

            //ensure an item is selected
            if(this.cboFeeType.Items.Count > 0)
                this.cboFeeType.SelectedIndex = 0;

            grdOrderFees.DisplayLayout.Bands[0].Columns["FeeType"].EditorComponent = cboFeeType;

            maskedEditorCurrency.InputMask = "{currency:5." + ApplicationSettings.Current.PriceDecimalPlaces + "}";

            maskedEditorPercent.InputMask = string.Format("nnn.{0} %",
                string.Concat(Enumerable.Repeat('n', ApplicationSettings.Current.PriceDecimalPlaces)));
        }

        private void grdOrderFees_InitializeRow(object sender, InitializeRowEventArgs e)
        {
            var ofRow = ((DataRowView)e.Row.ListObject).Row as QuoteDataSet.QuotePartFeesRow;

            if(ofRow != null)
            {
                ofRow.QuotePartID = this.QuotePartID;
                
                if(this.cboFeeType.SelectedItem == null)
                    this.cboFeeType.SelectedIndex = 0;

                if(ofRow.IsNull(this.dsQuotes.QuotePartFees.FeeTypeColumn.ColumnName) || String.IsNullOrEmpty(ofRow.FeeType))
                    ofRow.FeeType = this.cboFeeType.SelectedItem.DisplayText;
                if (ofRow.IsNull(this.dsQuotes.QuotePartFees.ChargeColumn.ColumnName) || ofRow.Charge <= 0)
                    ofRow.Charge = this.CalculateDefaultFee(ofRow.FeeType);

                if (ofRow.FeeCalculationType.EquivalentTo("Fixed"))
                {
                    e.Row.Cells["Charge"].EditorComponent = this.maskedEditorCurrency;
                }
                else if (ofRow.FeeCalculationType.EquivalentTo("Percentage"))
                {
                    e.Row.Cells["Charge"].EditorComponent = this.maskedEditorPercent;
                }
            }
        }

        private void lblDefaultFees_Click(object sender, EventArgs e)
        {
            try
            {
                if (String.IsNullOrWhiteSpace(ApplicationSettings.Current.DefaultFees))
                    return;

                var defaultFees = ApplicationSettings.Current.DefaultFees.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var feeType in defaultFees)
                {
                    var fee = this.dsQuotes.OrderFeeType.FindByOrderFeeTypeID(feeType.Trim());
                    
                    if (fee != null)
                    {
                        var feeRow = this.dsQuotes.QuotePartFees.NewQuotePartFeesRow();
                        feeRow.QuotePartID = this.QuotePartID;
                        feeRow.Charge = fee.Price;
                        feeRow.FeeType = fee.OrderFeeTypeID;
                        feeRow.FeeCalculationType = fee.FeeType;
                        this.dsQuotes.QuotePartFees.AddQuotePartFeesRow(feeRow);
                    }
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error adding default fees.");
            }
        }

        private void grdOrderFees_CellChange(object sender, CellEventArgs e)
        {
            if (e.Cell.Column.Key == "FeeType" && e.Cell.Text != null)
            {
                var feeType = this.dsQuotes.OrderFeeType.FindByOrderFeeTypeID(e.Cell.Text);

                if (feeType != null)
                {
                    e.Cell.Row.Cells["Charge"].SetValue(feeType.Price, true);
                    e.Cell.Row.Cells["FeeCalculationType"].SetValue(feeType.FeeType, true);
                    e.Cell.Row.Update(); // switches editor editor for charge (among other things)
                }
            }
        }

        #endregion
    }
}