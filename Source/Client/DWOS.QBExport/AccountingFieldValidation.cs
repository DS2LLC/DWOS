using DWOS.Data;
using NLog;
using nsoftware.InQB;
using System;

namespace DWOS.QBExport
{
    /// <summary>
    /// Contains utility methods for validating items against QuickBooks.
    /// </summary>
    public class AccountingFieldValidation
    {
        /// <summary>
        /// Validates the list item for a fee.
        /// </summary>
        /// <param name="listItemName">Name of the list item.</param>
        /// <param name="feeAmount">The fee amount.</param>
        /// <param name="feeType">Type of the fee.</param>
        /// <returns>True if able to validate list item; otherwise, false</returns>
        public static bool ValidateListItemFee(string listItemName, decimal feeAmount, string feeType)
        {
            var item = new Item() { QBConnectionString = ApplicationSettings.Current.QBConnectionString, QBXMLVersion = Properties.Settings.Default.QBXMLVersion };

            try
            {
                // Item does not exist, add it
                using(var frm = new AddListItem())
                {
                    LogManager.GetCurrentClassLogger().Info("Fee missing in QuickBooks, preparing to add.");

                    frm.ItemName = listItemName;
                    frm.LoadData();

                    var formResult = frm.ShowDialog();

                    if (formResult == System.Windows.Forms.DialogResult.OK)
                    {
                        item.Reset();
                        item.ItemType = frm.ItemType;
                        item.ItemName = listItemName;

                        if (feeType == "Fixed")
                            item.Price = feeAmount.ToString("F" + ApplicationSettings.Current.PriceDecimalPlaces);
                        else
                            item.PricePercent = feeAmount.ToString("F" + ApplicationSettings.Current.PriceDecimalPlaces);

                        item.Description = frm.ItemDescription;
                        item.AccountName = frm.ItemAccountType;
                            
                        item.Add();

                        LogManager.GetCurrentClassLogger().Info("Fee, {0}, added to QuickBooks successfully.".FormatWith(listItemName));
                    }

                    return formResult == System.Windows.Forms.DialogResult.OK;
                }
            }
            catch(Exception exc)
            {
                LogManager.GetCurrentClassLogger().Info("Error validating QuickBooks list item: " + listItemName + ". " + exc);
                return false;
            }
            finally
            {
                if (item != null)
                {
                    item.CloseQBConnection();
                    item.Dispose();
                }
            }
        }

        /// <summary>
        /// Validates a department's QuickBooks list item.
        /// </summary>
        /// <param name="departmentName">The department's name; required</param>
        /// <param name="accountingCode">The department's current accounting code; not required</param>
        /// <param name="newAccountingCode">Contains the new accounting code for the department, if any.</param>
        /// <exception cref="ArgumentNullException">Thrown if departmentName is null or empty.</exception>
        /// <returns>True if able to validate department; otherwise, false.</returns>
        public static bool ValidateListItemDepartment(string departmentName, string accountingCode, out string newAccountingCode)
        {
            if (string.IsNullOrEmpty(departmentName))
            {
                throw new ArgumentNullException(nameof(departmentName));
            }

            newAccountingCode = null;
            var item = new Item() { QBConnectionString = ApplicationSettings.Current.QBConnectionString, QBXMLVersion = Properties.Settings.Default.QBXMLVersion };

            try
            {
                // Item does not exist, add it
                using (var frm = new AddDepartmentItem())
                {
                    LogManager.GetCurrentClassLogger().Info("Department missing in QuickBooks, preparing to add.");

                    frm.Initialize(departmentName, accountingCode);

                    var formResult = frm.ShowDialog();

                    bool addItem = false;

                    if (formResult == System.Windows.Forms.DialogResult.OK)
                    {
                        addItem = !DoesItemExistInQB(frm.AccountingCode);
                    }

                    if (addItem)
                    {
                        item.Reset();

                        item.ItemType = frm.ItemType;
                        item.ItemName = frm.AccountingCode;
                        item.Description = frm.Description;
                        item.AccountName = frm.AccountType;

                        item.Add();

                        newAccountingCode = frm.AccountingCode;

                        LogManager.GetCurrentClassLogger().Info("Department, {0}, added to QuickBooks successfully.".FormatWith(departmentName));
                    }
                    else if (formResult == System.Windows.Forms.DialogResult.OK)
                    {
                        // Item already exists.
                        LogManager.GetCurrentClassLogger().Warn(
                            "Attempted to add existing QB department list item named {0}.",
                            frm.AccountingCode);
                    }

                    return addItem;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Info("Error validating QuickBooks department list item: " + departmentName + ". " + exc);
                return false;
            }
            finally
            {
                if (item != null)
                {
                    item.CloseQBConnection();
                    item.Dispose();
                }
            }
        }

        public static bool ValidateListItemProductClass(string productClass, string accountingCode, out string newAccountingCode)
        {
            if (string.IsNullOrEmpty(productClass))
            {
                throw new ArgumentNullException(nameof(productClass));
            }

            newAccountingCode = null;
            var item = new Item() { QBConnectionString = ApplicationSettings.Current.QBConnectionString, QBXMLVersion = Properties.Settings.Default.QBXMLVersion };

            try
            {
                // Item does not exist, add it
                using (var frm = new AddProductClassItem())
                {
                    LogManager.GetCurrentClassLogger().Info("Product Class missing in QuickBooks, preparing to add.");

                    frm.Initialize(productClass, accountingCode);

                    var formResult = frm.ShowDialog();

                    bool addItem = false;

                    if (formResult == System.Windows.Forms.DialogResult.OK)
                    {
                        addItem = !DoesItemExistInQB(frm.AccountingCode);
                    }

                    if (addItem)
                    {
                        item.Reset();

                        item.ItemType = frm.ItemType;
                        item.ItemName = frm.AccountingCode;
                        item.Description = frm.Description;
                        item.AccountName = frm.AccountType;

                        item.Add();

                        newAccountingCode = frm.AccountingCode;

                        LogManager.GetCurrentClassLogger().Info($"Product Class, {productClass}, added to QuickBooks successfully.");
                    }
                    else if (formResult == System.Windows.Forms.DialogResult.OK)
                    {
                        // Item already exists.
                        LogManager.GetCurrentClassLogger().Warn(
                            "Attempted to add existing QB product class list item named {0}.",
                            frm.AccountingCode);
                    }

                    return addItem;
                }
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Info("Error validating QuickBooks department list item: " + productClass + ". " + exc);
                return false;
            }
            finally
            {
                if (item != null)
                {
                    item.CloseQBConnection();
                    item.Dispose();
                }
            }
        }

        /// <summary>
        /// Validates a list item.
        /// </summary>
        /// <param name="listItem">The name of the list item.</param>
        /// <returns>True if able to validate list item; otherwise, false</returns>
        public static bool ValidateListItem(string listItem)
        {
            if (string.IsNullOrEmpty(listItem))
            {
                throw new ArgumentNullException(nameof(listItem));
            }

            var item = new Item() { QBConnectionString = ApplicationSettings.Current.QBConnectionString, QBXMLVersion = Properties.Settings.Default.QBXMLVersion };

            try
            {
                // Item does not exist, add it
                using(var frm = new AddListItem())
                {
                    LogManager.GetCurrentClassLogger().Info("List item missing in QuickBooks, preparing to add.");

                    frm.ItemName = listItem;
                    frm.LoadData();

                    var formResult = frm.ShowDialog();

                    if (formResult == System.Windows.Forms.DialogResult.OK)
                    {
                        item.Reset();
                        item.ItemType = frm.ItemType;
                        item.ItemName = listItem;
                        item.Description = frm.ItemDescription;
                        item.AccountName = frm.ItemAccountType;

                        item.Add();

                        LogManager.GetCurrentClassLogger().Info("List item, {0}, added to QuickBooks successfully.".FormatWith(listItem));
                    }

                    return formResult == System.Windows.Forms.DialogResult.OK;
                }
            }
            catch (Exception exc)
            {
                var errorMsg = "Error validating QuickBooks list item: " + listItem + ". ";
                LogManager.GetCurrentClassLogger().Info(exc, errorMsg);
                return false;
            }
            finally
            {
                if (item != null)
                {
                    item.CloseQBConnection();
                    item.Dispose();
                }
            }
        }

        /// <summary>
        /// Validates terms.
        /// </summary>
        /// <param name="termsName"></param>
        /// <returns></returns>
        public static bool ValidateTerms(string termsName)
        {
            if (string.IsNullOrEmpty(termsName))
            {
                throw new ArgumentNullException(nameof(termsName));
            }

            var term = new Qblists
            {
                QBConnectionString = ApplicationSettings.Current.QBConnectionString,
                QBXMLVersion = Properties.Settings.Default.QBXMLVersion,
                ListType = QblistsListTypes.ltStandardTerms
            };

            try
            {
                using (var frm = new AddTerms())
                {
                    LogManager.GetCurrentClassLogger().Info("Term missing in QuickBooks, preparing to add.");
                    frm.Initialize(termsName);

                    var formResult = frm.ShowDialog();

                    if (formResult == System.Windows.Forms.DialogResult.OK)
                    {
                        term.Reset();
                        term.QBName = termsName;
                        term.ListType = QblistsListTypes.ltStandardTerms;
                        // Import fails if TermsDueDate == 0; see #16397
                        term.TermsDueDay = frm.DueDate == 0 ? -1 : frm.DueDate;
                        term.Add();

                        LogManager.GetCurrentClassLogger().Info("Term, {0}, added to QuickBooks successfully.", termsName);
                    }

                    return formResult == System.Windows.Forms.DialogResult.OK;
                }
            }
            catch (Exception exc)
            {
                var errorMsg = $"Error validating QuickBooks list item: {termsName}.";
                LogManager.GetCurrentClassLogger().Info(exc, errorMsg);
                return false;
            }
            finally
            {
                term.CloseQBConnection();
                term.Dispose();
            }
        }

        private static bool DoesItemExistInQB(string itemName)
        {
            Item qbItem = null;

            try
            {
                qbItem = new Item();
                qbItem.OpenQBConnection();

                qbItem.GetByName(itemName);

                return !string.IsNullOrEmpty(qbItem.ItemName);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Warn(exc, "Item does not exist in QuickBooks " + itemName);
                return false;
            }
            finally
            {
                if (qbItem != null)
                {
                    qbItem.CloseQBConnection();
                    qbItem.Dispose();
                }
            }
        }
    }
}
