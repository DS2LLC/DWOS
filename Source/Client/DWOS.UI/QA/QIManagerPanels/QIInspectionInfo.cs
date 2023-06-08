using System.Data;
using System.Windows.Forms;
using DWOS.UI.Documents;
using DWOS.Utilities.Validation;
using DWOS.Data.Datasets;

namespace DWOS.UI.QA.QIManagerPanels
{
    public partial class QIInspectionInfo : DataPanel
    {
        #region Fields

        #endregion

        #region Properties

        public PartInspectionDataSet Dataset
        {
            get { return base._dataset as PartInspectionDataSet; }
            set { base._dataset = value; }
        }

        protected override string BindingSourcePrimaryKey
        {
            get { return Dataset.PartInspectionType.PartInspectionTypeIDColumn.ColumnName; }
        }

        #endregion

        #region Methods

        public QIInspectionInfo() { InitializeComponent(); }

        public void LoadData(PartInspectionDataSet dataset)
        {
            Dataset = dataset;
            bsData.DataSource = Dataset;
            bsData.DataMember = Dataset.PartInspectionType.TableName;

            //bind column to control
            base.BindValue(this.txtName, Dataset.PartInspectionType.NameColumn.ColumnName);
            base.BindValue(this.txtTestRef, Dataset.PartInspectionType.TestReferenceColumn.ColumnName);
            base.BindValue(this.txtTestReq, Dataset.PartInspectionType.TestRequirementsColumn.ColumnName);
            base.BindValue(this.chkActive, Dataset.PartInspectionType.ActiveColumn.ColumnName);
            base.BindValue(this.cboRevision, Dataset.PartInspectionType.RevisionColumn.ColumnName);

            base.BindList(this.cboRevision,
                Dataset.PartInspectionRevision,
                Dataset.PartInspectionRevision.RevisionColumn.ColumnName,
                Dataset.PartInspectionRevision.RevisionColumn.ColumnName);

            docLinkManagerProcess.InitializeData(LinkType.ControlInspection,
                this.Dataset.PartInspectionType,
                this.Dataset.PartInspectionTypeDocumentLink);

            base._panelLoaded = true;
        }

        public override void AddValidators(ValidatorManager manager, ErrorProvider errProvider)
        {
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.txtName, "Inspection name required."), errProvider));
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.txtTestRef, "Inspection test reference required."), errProvider));
            manager.Add(new ImageDisplayValidator(new TextControlValidator(this.txtTestReq, "Inspection test requirement required."), errProvider));
        }

        public PartInspectionDataSet.PartInspectionTypeRow AddRow()
        {
            var rowVw = bsData.AddNew() as DataRowView;
            var cr = rowVw.Row as PartInspectionDataSet.PartInspectionTypeRow;
            cr.Name = "New Inspection";
            cr.Active = true;
            cr.Revision = "<None>";

            return cr;
        }

        protected override void AfterMovedToNewRecord(object id)
        {
            base.AfterMovedToNewRecord(id);

            docLinkManagerProcess.ClearLinks();

            var partInspectionType = this.CurrentRecord as PartInspectionDataSet.PartInspectionTypeRow;
            if (partInspectionType != null)
            {
                docLinkManagerProcess.LoadLinks(partInspectionType,
                    partInspectionType.GetPartInspectionTypeDocumentLinkRows());
            }
        }
        #endregion
    }
}