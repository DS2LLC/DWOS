using DWOS.Data.Datasets;
using DWOS.Data.Datasets.OrderProcessingDataSetTableAdapters;
using NLog;
using System;
using System.Data.SqlClient;
using System.Linq;

namespace DWOS.Data.Order
{
    /// <summary>
    /// Determines and updates cost for order processes.
    /// </summary>
    internal class ProcessCostHelper
    {
        #region Fields

        private const decimal MAX_MATERIAL_COST = 999999.99999M;
        private const string UNIT_SQUARE_INCH = "in²";
        private const string UNIT_POUND = "lb";
        private readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        public SqlConnection Connection { get; }

        public SqlTransaction Transaction { get; }

        #endregion

        #region Methods

        public ProcessCostHelper(SqlConnection conn, SqlTransaction transaction)
        {
            Connection = conn ?? throw new ArgumentNullException(nameof(conn));
            Transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }

        /// <summary>
        /// Updates an order process's cost based on its process and answers.
        /// </summary>
        /// <param name="orderProcessesID"></param>
        public void UpdateCost(int orderProcessesID)
        {
            OrderProcessingDataSet.OrderProcessesDataTable orderProcesses = null;
            OrderProcessingDataSet.ProcessDataTable processes = null;
            OrderProcessingDataSet.OrderProductClassDataTable orderProductClasses = null;
            OrderProcessingDataSet.ProcessProductClassDataTable processProductClasses = null;

            try
            {
                _log.Info("Updating Cost of Process.");
                orderProcesses = new OrderProcessingDataSet.OrderProcessesDataTable();
                processes = new OrderProcessingDataSet.ProcessDataTable();
                orderProductClasses = new OrderProcessingDataSet.OrderProductClassDataTable();
                processProductClasses = new OrderProcessingDataSet.ProcessProductClassDataTable();

                using (var taOrderProcesses = new OrderProcessesTableAdapter())
                {
                    taOrderProcesses.Connection = Connection;
                    taOrderProcesses.Transaction = Transaction;
                    taOrderProcesses.FillByID(orderProcesses, orderProcessesID);
                }

                if (orderProcesses.Count == 0)
                {
                    return;
                }

                var orderProcessRow = orderProcesses.First();

                using (var taP = new ProcessTableAdapter())
                {
                    taP.Connection = Connection;
                    taP.Transaction = Transaction;
                    taP.FillByProcess(processes, orderProcessRow.ProcessID);
                }

                if (processes.Count == 0)
                {
                    return;
                }

                var processRow = processes.First();
                var totalSurfaceArea = OrderUtilities.CalculateTotalSurfaceAreaInches(orderProcessRow.OrderID);
                var materialWeight = MaterialWeightPounds(orderProcessRow.OrderProcessesID,
                            orderProcessRow.ProcessID);

                // Base cost
                var materialUnit = processRow.IsMaterialUnitNull() ?
                    string.Empty :
                    processRow.MaterialUnit;

                var materialUnitCost = processRow.IsMaterialUnitCostNull() ?
                    0M :
                    processRow.MaterialUnitCost;

                var totalMaterialCost = CalculateMaterialCost(totalSurfaceArea, materialWeight, materialUnit, materialUnitCost);

                // Product class cost
                using (var taOrderProductClass = new OrderProductClassTableAdapter())
                {
                    taOrderProductClass.Connection = Connection;
                    taOrderProductClass.Transaction = Transaction;
                    taOrderProductClass.FillByOrder(orderProductClasses, orderProcessRow.OrderID);
                }

                using (var taProcessProductClass = new ProcessProductClassTableAdapter())
                {
                    taProcessProductClass.Connection = Connection;
                    taProcessProductClass.Transaction = Transaction;
                    taProcessProductClass.FillByProcess(processProductClasses, orderProcessRow.ProcessID);
                }

                foreach (var productClassName in orderProductClasses.Select(GetOrderProductClass).Distinct())
                {
                    // Calculate & add cost for each product class using the process ID
                    var matchingProcessProductClass = processProductClasses
                        .FirstOrDefault(c => GetProcessProductClass(c) == productClassName);

                    if (matchingProcessProductClass == null)
                    {
                        // Order has a product class that's not on the process
                        _log.Info($"Skipping product class {productClassName} - is not associated with process");
                    }
                    else
                    {
                        // Calculate product class
                        var productClassUnit = matchingProcessProductClass.IsMaterialUnitNull() ?
                            string.Empty :
                            matchingProcessProductClass.MaterialUnit;

                        var productClassUnitCost = matchingProcessProductClass.IsMaterialUnitCostNull() ?
                            0M :
                            matchingProcessProductClass.MaterialUnitCost;

                        var productClassCost = CalculateMaterialCost(totalSurfaceArea, materialWeight,
                            productClassUnit,
                            productClassUnitCost);

                        totalMaterialCost += productClassCost;
                    }
                }

                // Save rounded cost to OrderProcess
                using (var taOrderProcesses = new OrderProcessesTableAdapter())
                {
                    taOrderProcesses.Connection = Connection;
                    taOrderProcesses.Transaction = Transaction;

                    decimal? roundedCost = Math.Round(totalMaterialCost,
                        ApplicationSettings.Current.PriceDecimalPlaces);

                    if (roundedCost == 0M)
                    {
                        // Save null instead of 0 because the process has no cost
                        roundedCost = null;
                    }
                    else if (roundedCost > MAX_MATERIAL_COST)
                    {
                        _log.Warn("Calculated cost ({0}) for order process {1} was above maximum value.",
                            roundedCost,
                            orderProcessesID);

                        roundedCost = MAX_MATERIAL_COST;
                    }
                    else if (roundedCost < 0M)
                    {
                        _log.Warn("Calculated cost ({0}) for order process {1} was negative.",
                            roundedCost,
                            orderProcessesID);

                        roundedCost = null;
                    }

                    taOrderProcesses.UpdateMaterialCost(roundedCost, orderProcessesID);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error updating cost based on process and answers.");
            }
            finally
            {
                orderProcesses?.Dispose();
                processes?.Dispose();
                orderProductClasses?.Dispose();
                processProductClasses?.Dispose();
            }

            string GetOrderProductClass(OrderProcessingDataSet.OrderProductClassRow productClass) =>
                productClass.IsProductClassNull() ? string.Empty : productClass.ProductClass;

            string GetProcessProductClass(OrderProcessingDataSet.ProcessProductClassRow productClass) =>
                productClass.IsProductClassNull() ? string.Empty : productClass.ProductClass;
        }

        private static decimal CalculateMaterialCost(double totalSurfaceArea, decimal materialWeight, string materialUnit, decimal materialUnitCost)
        {
            var totalMaterialCost = 0M;

            if (materialUnitCost != 0M)
            {
                if (materialUnit == UNIT_SQUARE_INCH)
                {
                    totalMaterialCost = materialUnitCost * Convert.ToDecimal(totalSurfaceArea);
                }
                else if (materialUnit == UNIT_POUND)
                {
                    totalMaterialCost = materialUnitCost * materialWeight;
                }
            }

            return totalMaterialCost;
        }

        private decimal MaterialWeightPounds(int orderProcessID, int processID)
        {
            OrderProcessingDataSet dsOrderProcessing = null;
            _log.Info("Begin calculating of Material Weight.");
            var materialWeight = 0M;

            try
            {
                dsOrderProcessing = new OrderProcessingDataSet()
                {
                    EnforceConstraints = false
                };

                using (var taQuestions = new ProcessQuestionTableAdapter())
                {
                    taQuestions.Connection = Connection;
                    taQuestions.Transaction = Transaction;
                    taQuestions.FillBy(dsOrderProcessing.ProcessQuestion, processID);
                }

                using (var taAnswer = new OrderProcessAnswerTableAdapter())
                {
                    taAnswer.Connection = Connection;
                    taAnswer.Transaction = Transaction;
                    taAnswer.FillByOrderProcessesID(dsOrderProcessing.OrderProcessAnswer, orderProcessID);
                }

                using (var taProcessStep = new ProcessStepsTableAdapter())
                {
                    taProcessStep.Connection = Connection;
                    taProcessStep.Transaction = Transaction;
                    taProcessStep.FillBy(dsOrderProcessing.ProcessSteps, processID);
                }

                var preProcessAnswer = dsOrderProcessing.OrderProcessAnswer
                    .Where(r => r.ProcessQuestionRow.InputType == InputType.PreProcessWeight.ToString())
                    .OrderBy(r => r.ProcessQuestionRow.ProcessStepsRow.StepOrder)
                    .ThenBy(r => r.ProcessQuestionRow.StepOrder)
                    .FirstOrDefault();

                var postProcessAnswer = dsOrderProcessing.OrderProcessAnswer
                    .Where(r => r.ProcessQuestionRow.InputType == InputType.PostProcessWeight.ToString())
                    .OrderByDescending(r => r.ProcessQuestionRow.ProcessStepsRow.StepOrder)
                    .ThenByDescending(r => r.ProcessQuestionRow.StepOrder)
                    .FirstOrDefault();

                // Calculate
                decimal preProcessWeight = 0;
                decimal postProcessWeight = 0;

                var parsedAnswers = decimal.TryParse(preProcessAnswer?.Answer, out preProcessWeight) &&
                    decimal.TryParse(postProcessAnswer?.Answer, out postProcessWeight);

                if (!parsedAnswers)
                {
                    string errorMsg;

                    if (preProcessAnswer == null || postProcessAnswer == null)
                    {
                        errorMsg = string.Format(
                            "Cannot determine weight - process {0} lacks at least one weight question.",
                            processID);
                    }
                    else
                    {
                        errorMsg = "Cannot determine weight - answers were in the wrong format.";
                    }

                    _log.Warn(errorMsg);
                }
                else
                {
                    materialWeight = Math.Abs(postProcessWeight - preProcessWeight);
                }
            }
            catch (Exception exc)
            {
                _log.Error(exc, "Error calculating Material Weight.");
            }
            finally
            {
                dsOrderProcessing?.Dispose();
            }

            return materialWeight;
        }
    }
    #endregion
}
