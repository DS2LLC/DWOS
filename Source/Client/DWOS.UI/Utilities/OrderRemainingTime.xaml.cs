using System;
using System.Collections.Generic;
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
    /// Interaction logic for OrderRemainingTime.xaml
    /// </summary>
    public partial class OrderRemainingTime : UserControl
    {
        public static DependencyProperty DataItemProperty = DependencyProperty.Register(
            "DataItem",
            typeof(object),
            typeof(OrderRemainingTime));

        public object DataItem
        {
            get
            {
                return GetValue(DataItemProperty);
            }
            set
            {
                SetValue(DataItemProperty, value);
            }
        }

        public OrderRemainingTime()
        {
            InitializeComponent();
        }
    }
}
