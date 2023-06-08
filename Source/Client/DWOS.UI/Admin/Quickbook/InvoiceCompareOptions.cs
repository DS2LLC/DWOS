using System;
using System.Windows.Forms;
using DWOS.Data;
using DWOS.Reports;

namespace DWOS.UI.Admin.Quickbook
{
    public partial class InvoiceCompareOptions: Form
    {
        public InvoiceCompareOptions()
        {
            this.InitializeComponent();

            this.dteFromDate.Value = DateUtilities.GetFirstDayOfMonth(DateTime.Now);
            this.dteToDate.Value = DateUtilities.GetLastDayOfMonth(DateTime.Now);
        }
    }
}