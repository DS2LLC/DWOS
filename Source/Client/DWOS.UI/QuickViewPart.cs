using System;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Data.Datasets.PartsDatasetTableAdapters;
using DWOS.Reports;
using DWOS.Shared;
using DWOS.UI.Utilities;
using NLog;

namespace DWOS.UI
{
    public partial class QuickViewPart: Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        public int PartID { get; set; }

        #endregion

        #region Methods

        public QuickViewPart()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            using (new UsingDataSetLoad(dsParts))
            {
                taPart.FillByPartID(dsParts.Part, PartID);
                taMedia.FillByPartIDWithoutMedia(dsParts.Media, PartID);
                taMaterial.Fill(dsParts.d_Material);
                taPart_Media.FillByPartID(dsParts.Part_Media, PartID);
                taPartDocumentLink.FillByPartID(dsParts.Part_DocumentLink, PartID);

                taAirframe.Fill(dsParts.d_Airframe);
                taManufacturer.Fill(dsParts.d_Manufacturer);
                taPartProcess.FillByPart(dsParts.PartProcess, PartID);
                taCustomer.FillByPartID(dsParts.Customer, PartID);
                taPriceUnit.Fill(dsParts.PriceUnit);
                taProcessAlias.FillBy(dsParts.ProcessAlias, PartID);

                //fill process names
                using (var ta = new ProcessTableAdapter())
                    ta.FillByPartID(dsParts.Process, PartID);

                taPartProcessVolumePrice.FillByPartID(dsParts.PartProcessVolumePrice, PartID);

                // Fill custom field information
                taLists.Fill(dsParts.Lists);
                taListValues.Fill(dsParts.ListValues);
                taPartLevelCustomField.FillByPartID(dsParts.PartLevelCustomField, PartID);
                taPartCustomFields.FillByPartID(dsParts.PartCustomFields, PartID);
            }

            pnlPartInfo.LoadData(dsParts);
        }

        #endregion

        #region Events

        private void QuickViewPart_Load(object sender, EventArgs e)
        {
            try
            {
                using(new UsingWaitCursor())
                {
                    pnlPartInfo.ViewOnly = true;

                    LoadData();
                    pnlPartInfo.MoveToRecord(PartID);
                    pnlPartInfo.Editable = false;
                }
            }
            catch(Exception exc)
            {
                _log.Warn("DataSet Errors: " + dsParts.GetDataErrors());
                ErrorMessageBox.ShowDialog("Error loading form.", exc);
            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            try
            {
                var psr = new PartSummaryReport(dsParts.Part.FindByPartID(PartID));
                psr.DisplayReport();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error displaying report.";
                _log.Error(exc, errorMsg);
            }
        }

        #endregion
    }
}