using System;
using System.Collections.Generic;
using System.Windows;
using DWOS.Data.Datasets;
using DWOS.Reports;
using DWOS.Shared;
using DWOS.UI.Utilities;
using DWOS.UI.Reports;
using NLog;
using DWOS.Data.Order;

namespace DWOS.UI.Sales.BatchOrderPanels
{
    /// <summary>
    /// Interaction logic for SplitBatchOrder.xaml
    /// </summary>
    public partial class SplitBatchOrder : Window
    {
        #region Properties

        public int MaxQuantity { get; set; }

        public int OrderQuantity
        {
            get { return Convert.ToInt32(numBatchQuantity.Value); }
        }

        public int RemainingQuantity
        {
            get { return MaxQuantity - OrderQuantity; }
        }

        private Data.Datasets.OrderProcessingDataSet.BatchOrderRow BatchOrderRow { get; set; }

        #endregion

        #region Methods

        public SplitBatchOrder()
        {
            InitializeComponent();
        }

        public void LoadData(Data.Datasets.OrderProcessingDataSet.BatchOrderRow batchOrderRow, int maxQty)
        {
            BatchOrderRow = batchOrderRow;
            MaxQuantity = maxQty;
            
            lblOrderId.Content = batchOrderRow.OrderID.ToString();
            numBatchQuantity.Value = batchOrderRow.PartQuantity;
            numBatchQuantity.ValueConstraint = new Infragistics.Controls.Editors.ValueConstraint() { ValidateAsType = Infragistics.Controls.Editors.ValidateAsType.Integer32, MaxInclusive = maxQty, MinInclusive = 1 };

            btnOK.IsEnabled = false;
        }
       
        private bool Save()
        {
            try
            {
                using(new UsingWaitCursorWpf(this))
                {
                    NLog.LogManager.GetCurrentClassLogger().Info("Splitting order {0} within batch.".FormatWith(BatchOrderRow.OrderID));

                    var factory = new OrderFactory();
                    factory.Load();
                    factory.LoadOrderData(BatchOrderRow.OrderID);

                    var order = factory.Orders.Order.FindByOrderID(BatchOrderRow.OrderID);

                    var splits = new List <SplitOrderInfo>();
                    splits.Add(new SplitOrderInfo() {Order = BatchOrderRow.OrderID.ToString(), IsOriginalOrder = true, PartQty = OrderQuantity});
                    splits.Add(new SplitOrderInfo() {Order = "New", IsOriginalOrder = false, PartQty = RemainingQuantity});

                    var newOrders = factory.SplitOrder(order, splits, (int) OrderChangeType.Split);

                    factory.Save();

                    try
                    {
                        //Print the travelers [TODO: Allow to print label traveler]
                        foreach(var newOrder in newOrders)
                        {
                            var rpt = new WorkOrderTravelerReport(newOrder);
                            if (chkPrint.IsChecked.GetValueOrDefault())
                                rpt.PrintReport();
                            else
                                rpt.DisplayReport();
                        }

                        var origRpt = new WorkOrderTravelerReport(order);
                        if (chkPrint.IsChecked.GetValueOrDefault())
                            origRpt.PrintReport();
                        else
                            origRpt.DisplayReport();
                    }
                    catch (Exception exc)
                    {
                        LogManager.GetCurrentClassLogger().Error(exc, "Error printing WO Traveler.");
                    }

                    factory.Orders.Dispose();
                    factory.TableManager.Dispose();

                    return true;
                }
            }
            catch (Exception exc)
            {
                ErrorMessageBox.ShowDialog("Error during order split.", exc);
                return false;
            }

        }

        #endregion

        #region Events
        
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                btnOK.IsEnabled = false;

                if (this.Save())
                {
                    this.DialogResult = true;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error splitting batch order.");
            }
            finally
            {
                btnOK.IsEnabled = true;
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                
               
            }
            catch (Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error during closing.");
            }
        }

        private void numBatchQuantity_ValueChanged(object sender, EventArgs e)
        {
            numRemQuantity.Value = RemainingQuantity;

            //enable button only if there is something to split
            btnOK.IsEnabled = RemainingQuantity > 0;

        }

        #endregion
    }
}
