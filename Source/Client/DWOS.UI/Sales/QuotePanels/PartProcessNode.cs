using DWOS.Data.Datasets;
using DWOS.UI.Utilities;

namespace DWOS.UI.Sales
{
    internal class PartProcessNode : DataNode<QuoteDataSet.QuotePart_ProcessRow>
    {
        #region Fields

        private const string KEY_PREFIX = "PP";

        #endregion

        #region Methods

        public PartProcessNode(QuoteDataSet.QuotePart_ProcessRow cr)
            : base(cr, cr.QuotePartProcessID.ToString(), KEY_PREFIX, "Process")
        {
            LeftImages.Add(Properties.Resources.Process_16);

            this.UpdateNodeUI();

            if (!DataRow.ProcessRow.Active)
            {
                LeftImages.Add(Properties.Resources.RoundDashRed_32);
            }
        }

        public override void UpdateNodeUI()
        {
            string processName = null;
            bool processShowOnQuote = false;

            if (base.DataRow.ProcessRow != null && base.DataRow.ProcessAliasRow != null)
            {
                if (base.DataRow.ProcessAliasRow.Name.Contains(base.DataRow.ProcessRow.ProcessName))
                {
                    processName = base.DataRow.ProcessAliasRow.Name;
                }
                else
                {
                    processName = base.DataRow.ProcessAliasRow.Name + " [" + base.DataRow.ProcessRow.ProcessName + "]";
                }
            }
            else
            {
                processName = "Unknown Process";
            }

            if (base.DataRow.ProcessRow != null && base.DataRow.IsShowOnQuoteNull())
            {
                processShowOnQuote = false;
            }
            else
            {
                processShowOnQuote = base.DataRow.ShowOnQuote;
            }

            Text = base.DataRow.StepOrder + " - " + processName;
            Text = Text + " | " + processShowOnQuote;
        }

        #endregion
    }
}
