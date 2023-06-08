using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DWOS.UI.Utilities
{
    /// <summary>
    /// Interaction logic for HeaderToolTip.xaml
    /// </summary>
    public partial class HeaderToolTip : UserControl, INotifyPropertyChanged
    {
        private string _header;
        private string _text;

        public string ToolTipHeader
        {
            get { return _header; }
            set
            {
                _header = value;

                if(PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ToolTipHeader"));
            }
        }
        public string ToolTipText
        {
            get { return _text; }
            set
            {
                _text = value;

                if (PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs("ToolTipText"));
            }
        }

        public HeaderToolTip()
        {
            InitializeComponent();
            this.DataContext = this;

            
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void Border_Loaded(object sender, RoutedEventArgs e)
        {
            var toolTip = this.Parent as ToolTip;

            if(toolTip != null)
            {
                toolTip.Margin = new Thickness(0);
                toolTip.Padding = new Thickness(0);
            }
        }
    }
}
