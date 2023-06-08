using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DWOS.Data.Datasets;
using DWOS.UI.Utilities;
using Infragistics.Win.UltraWinTree;

namespace DWOS.UI.Admin
{
    public partial class DomainValueEditor : Form
    {
        #region Fields

        private SecurityFormWatcher _securityWatcher = null;

        #endregion

        #region Properties

        public Func<DomainValue> AddValue { get; set; }
        public Func<bool> DeleteValue { get; set; }
        public Action<DomainValue> NameChanged { get; set; }
        public int? MaxNameLength { get; set; }
        public bool AllowDuplicateNames { get; set; }
        public DomainValue SelectedValue
        {
            get
            {
                if (tvwValues.SelectedNode<DomainValueNode>() != null)
                    return tvwValues.SelectedNode<DomainValueNode>().Value;
                else
                    return null;
            }
        }

        #endregion

        #region Methods
        
        public DomainValueEditor()
        {
            InitializeComponent();

            _securityWatcher = new SecurityFormWatcher(this, btnCancel);
        }

        internal void AddDomainValue(DomainValue value)
        {
            var node = new DomainValueNode(value);
            tvwValues.Nodes.Add(node);
            node.Select(true);
        }
        
        private void RefreshButtons()
        {
            btnAddProcess.Enabled = true;
            var selectedValue = tvwValues.SelectedNode<DomainValueNode>();
            btnRemoveProcess.Enabled = selectedValue != null && selectedValue.Value.AllowDelete;
            btnEdit.Enabled = selectedValue != null && selectedValue.Value.AllowEdit;
        }

        private void EditName()
        {
            var selectedValue = tvwValues.SelectedNode<DomainValueNode>();

            if (selectedValue != null && selectedValue.Value.AllowEdit)
            {
                using (var form = new TextBoxForm())
                {
                    form.Text             = "Edit Name";
                    form.FormLabel.Text   = "Name:";
                    form.FormTextBox.Text = selectedValue.Value.Name;
                    
                    if(MaxNameLength.HasValue)
                        form.FormTextBox.MaxLength = MaxNameLength.Value;

                    if (form.ShowDialog(this) == DialogResult.OK && !String.IsNullOrWhiteSpace(form.FormTextBox.Text))
                    {
                        var name = form.FormTextBox.Text.Trim().Replace("'", "*");

                        if(!AllowDuplicateNames)
                        {
                            var exists = tvwValues.Nodes.FindNode <DomainValueNode>(dn => dn.Text == name);
                            if (exists != null && selectedValue != exists)
                            {
                                MessageBoxUtilities.ShowMessageBoxWarn("Duplicate name values are not allowed.", "Duplicate Names", "Ensure all names are unique.");
                                return;
                            }
                        }

                        selectedValue.Text = selectedValue.Value.Name = name;
                        if (NameChanged != null)
                            NameChanged(selectedValue.Value);
                    }
                    else
                    {
                        this.RemoveProcess();
                    }
                }
            }
        }

        #endregion

        #region Events
        
        private void btnAddProcess_Click(object sender, EventArgs e)
        {
            if(AddValue != null)
            {
                var value = AddValue();
                AddDomainValue(value);
                EditName();
            }
        }
        
        private void RemoveProcess()
        {
            var selectedValue = tvwValues.SelectedNode<DomainValueNode>();

            if (selectedValue != null && selectedValue.Value.AllowDelete)
            {
                var canDelete = true;

                //May be checked to see if this list item is being used
                if (DeleteValue != null)
                    canDelete = DeleteValue();

                if (canDelete)
                {
                    selectedValue.Value.Row.Delete();
                    selectedValue.Remove();
                    tvwValues.PerformAction(UltraTreeAction.ClearAllSelectedNodes, false, false);
                }
            }
        }

        private void btnRemoveProcess_Click(object sender, EventArgs e)
        {
            this.RemoveProcess();
        }
        
        private void btnEdit_Click(object sender, EventArgs e)
        {
            EditName();
        }
        
        private void tvwValues_AfterSelect(object sender, SelectEventArgs e) { RefreshButtons(); }
        
        private void tvwValues_DoubleClick(object sender, EventArgs e)
        {
            EditName();
        }

        #endregion

        internal class DomainValueNode : UltraTreeNode
        {
            #region Properties

            public DomainValue Value { get; set; }

            #endregion

            #region Methods

            public DomainValueNode(DomainValue value)
                : base()
            {
                this.Value = value;
                this.Text = value.Name;
            }

            #endregion Methods
        }

        public class DomainValue
        {
           public string Name { get; set; }
           public DataRow Row { get; set; }
           public bool AllowDelete { get; set; }
           public bool AllowEdit { get; set; }
        }
    }
}
