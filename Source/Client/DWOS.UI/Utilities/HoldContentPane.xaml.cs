using Infragistics.Windows.DockManager;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace DWOS.UI.Utilities
{
    /// <summary>
    /// Interaction logic for HoldContentPane.xaml
    /// </summary>
    public partial class HoldContentPane : ContentPane
    {
        #region Properties

        public HoldList HoldList
        {
            get
            {
                return holdList;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Initializes a new instance of the <see cref="HoldContentPane"/> class.
        /// </summary>
        public HoldContentPane()
        {
            InitializeComponent();
        }

        #endregion

        #region Events

        private void ContentPane_Closed(object sender, Infragistics.Windows.DockManager.Events.PaneClosedEventArgs e)
        {
            holdList?.Dispose();
        }

        #endregion
    }
}
