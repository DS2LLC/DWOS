using System.ComponentModel;
using System.Windows.Forms;
using Infragistics.UltraGauge.Resources;

namespace DWOS.UI.Utilities
{
    public partial class DigitalGuage : UserControl
    {
        public DigitalGuage() { InitializeComponent(); }

        [Browsable(true)]
        public string GuageLabel
        {
            get { return this.ultraGroupBox1.Text; }
            set { this.ultraGroupBox1.Text = value; }
        }

        [Browsable(true)]
        public string GuageCount
        {
            set
            {
                var g = this.guageOrderCount.Gauges[0] as DigitalGauge;
                g.Text = value;
            }
        }
    }
}