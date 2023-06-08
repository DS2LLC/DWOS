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

namespace DWOS.UI
{
    /// <summary>
    /// Interaction logic for MainDock.xaml
    /// </summary>
    public partial class MainDock : UserControl
    {
        #region Properties

        public bool AllowTextInput { get; set; } = true;

        #endregion

        #region Methods

        public MainDock()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void MainDock_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!AllowTextInput)
            {
                e.Handled = true;
            }
        }

        #endregion
    }
}
