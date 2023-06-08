using System;
using System.Data;
using System.Collections.Generic;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.Shared;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinGrid;
using NLog;

namespace DWOS.UI.Admin
{
    public partial class MaterialManager: Form
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private GridSettingsPersistence<UltraGridBandSettings> _gridSettingsPersistence =
            new GridSettingsPersistence<UltraGridBandSettings>("MaterialManager", new UltraGridBandSettings());

        #endregion

        #region Methods

        public MaterialManager()
        {
            this.InitializeComponent();
        }

        private void LoadData()
        {
            this.dsParts.EnforceConstraints = false;
            this.dsParts.d_Material.ChildRelations.Clear();
            this.dsParts.d_Material.BeginLoadData();
            this.taMaterial.FillWithPartCount(this.dsParts.d_Material);
            this.dsParts.d_Material.EndLoadData();
            this.dsParts.EnforceConstraints = true;
        }

        private bool SaveData()
        {
            try
            {
                if (ValidateMaterials())
                {
                    this.grdMaterial.UpdateData();
                    this.taManagerParts.UpdateAll(this.dsParts);

                    return true;
                }

                return false;
            }
            catch(Exception exc)
            {
                string errorMsg = "Error saving data.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);

                return false;
            }
        }

        public void Merge(string newMaterial)
        {
            try
            {
                using(new UsingWaitCursor(this))
                {
                    _log.Debug("Merging " + newMaterial);

                    //save any changes to the table
                    this.grdMaterial.UpdateData();

                    if(this.grdMaterial.Selected.Rows.Count > 0)
                    {
                        var oldMaterials = new List<string>();
                        foreach(UltraGridRow item in this.grdMaterial.Selected.Rows)
                        {
                            string oldMaterial = item.Cells[0].Value.ToString();

                            if(newMaterial.ToLower() != oldMaterial.ToLower()) //case insensitive compare to prevent deleting original material w/ different case
                                oldMaterials.Add(oldMaterial);
                        }

                        foreach(string oldMaterial in oldMaterials)
                        {
                            _log.Debug("Updating old material " + oldMaterial);

                            this.taParts.FillByMaterial(this.dsParts.Part, oldMaterial);

                            //get all parts in the table that have old material
                            var parts = this.dsParts.Part.Select(this.dsParts.Part.MaterialColumn.ColumnName + " = '" + oldMaterial + "'") as PartsDataset.PartRow[];

                            if(parts != null)
                            {
                                _log.Debug("Updating " + parts.Length + " parts.");

                                //update to new material
                                foreach(PartsDataset.PartRow p in parts)
                                    p.Material = newMaterial;
                            }

                            //delete old material
                            PartsDataset.d_MaterialRow mRow = this.dsParts.d_Material.FindByMaterialID(oldMaterial);

                            if(mRow != null)
                                mRow.Delete();
                            else
                                _log.Debug("WARN unable to find old material in materials table " + oldMaterial);
                        }

                        //find new material
                        PartsDataset.d_MaterialRow newMRow = this.dsParts.d_Material.FindByMaterialID(newMaterial);

                        //if not existent then add
                        if (newMRow == null)
                            this.dsParts.d_Material.Addd_MaterialRow(newMaterial, true);

                        //reload updated data
                        this.grdMaterial.DataBind();
                    }
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error merging selected rows.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private bool ValidateMaterials()
        {
            bool result = true;

            try
            {
                foreach (UltraGridRow row in grdMaterial.DisplayLayout.Bands[0].GetRowEnumerator(GridRowType.DataRow))
                {
                    UltraGridCell cell = row.Cells["MaterialID"];
                    if (cell != null && cell.Row != null)
                    {
                        DataRowView drv = (DataRowView)cell.Row.ListObject;
                        if (drv != null)
                        {
                            if (String.IsNullOrEmpty(cell.Text) || cell.Value == null)
                            {
                                drv.Row.SetColumnError("MaterialID", "Material can not be empty");
                                result = false;
                            }
                            else
                            {
                                drv.Row.ClearErrors();
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                string errorMsg = "Error validating data.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);

                return false;
            }

            return result;
        }

        #endregion

        #region Events

        private void MaterialManager_Load(object sender, EventArgs e)
        {
            try
            {
                this.LoadData();
            }
            catch(Exception exc)
            {
                string errorMsg = "Error loading Manufacturer Manager.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                if(this.SaveData())
                    Close();
                else
                    DialogResult = DialogResult.None;
            }
            catch(Exception exc)
            {
                string errorMsg = "Error saving materials.";
                ErrorMessageBox.ShowDialog(errorMsg, exc);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnMerge_Click(object sender, EventArgs e)
        {
            try
            {
                using(var form = new TextBoxForm())
                {
                    form.Text = "Material";
                    form.FormLabel.Text = "New Material";

                    if(form.ShowDialog(this) == DialogResult.OK)
                        this.Merge(form.FormTextBox.Text);
                }
            }
            catch(Exception exc)
            {
                string errorMsg = "Error adding new material.";
                _log.Error(exc, errorMsg);
            }
        }

        private void grdMaterial_BeforeRowsDeleted(object sender, BeforeRowsDeletedEventArgs e)
        {
            foreach (var row in e.Rows)
            {
                var materialName = row.Cells["MaterialID"].Value.ToString();
                var usageCount = row.Cells["PartCount"].Value;

                if (usageCount != null && usageCount != DBNull.Value && Convert.ToInt32(usageCount) > 0)
                {
                    var result = MessageBox.Show(materialName + " is in use by " + usageCount + " parts. The material will be marked as inactive.", "Warning", MessageBoxButtons.OKCancel);

                    if (result == DialogResult.OK)
                        row.Cells["IsActive"].Value = false;

                    // cancel the delete. Whether the user clicks OK or Cancel the part should remain
                    e.Cancel = true;
                }
            }
        }

        private void grdMaterial_BeforeExitEditMode(object sender, BeforeExitEditModeEventArgs e)
        {
            ValidateMaterials();
        }


        private void grdMaterial_InitializeLayout(object sender, InitializeLayoutEventArgs e)
        {
            try
            {
                grdMaterial.AfterColPosChanged -= grdMaterial_AfterColPosChanged;
                grdMaterial.AfterSortChange -= grdMaterial_AfterSortChange;

                // Load settings
                _gridSettingsPersistence.LoadSettings().ApplyTo(grdMaterial.DisplayLayout.Bands[0]);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error initializing grid layout.");
            }
            finally
            {
                grdMaterial.AfterColPosChanged += grdMaterial_AfterColPosChanged;
                grdMaterial.AfterSortChange += grdMaterial_AfterSortChange;
            }
        }

        private void grdMaterial_AfterColPosChanged(object sender, AfterColPosChangedEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdMaterial.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing column position.");
            }
        }


        private void grdMaterial_AfterSortChange(object sender, BandEventArgs e)
        {
            try
            {
                // Save settings
                var settings = new UltraGridBandSettings();
                settings.RetrieveSettingsFrom(grdMaterial.DisplayLayout.Bands[0]);
                _gridSettingsPersistence.SaveSettings(settings);
            }
            catch (Exception exc)
            {
                LogManager.GetCurrentClassLogger().Error(exc, "Error changing column sort.");
            }
        }

        #endregion
    }
}