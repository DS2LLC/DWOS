using System.Collections.Generic;
using System.Threading;
using DWOS.Data.Datasets;
using DWOS.Data.Datasets.SecurityDataSetTableAdapters;
using DWOS.Data.Reports;
using Infragistics.Documents.Excel;
using Infragistics.Documents.Reports.Report;

namespace DWOS.Reports
{
    public class UserSecurityAuditReport : ExcelBaseReport
    {
        #region Properties

        public override string Title
        {
            get { return "User Security Audit"; }
        }

        /// <summary>
        ///     Gets the report page orientation.
        /// </summary>
        /// <value> The report page orientation. </value>
        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Landscape; }
        }

        #endregion

        #region Methods

        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            var dtUsers = new SecurityDataSet.UsersDataTable();

            var taUsers = new UsersTableAdapter();
            taUsers.FillByActive(dtUsers, true);

            CreateReportExcel(dtUsers);
        }

        /// <summary>
        ///     Creates the Security Audit report in Excel format
        /// </summary>
        /// <param name="dtUsers"> The users. </param>
        private void CreateReportExcel(SecurityDataSet.UsersDataTable dtUsers)
        {
            var dtUser = new SecurityDataSet.User_SecurityRolesDataTable();

            //Table (list) of user security permissions
            var dtSecurityRoles = new SecurityDataSet.SecurityRoleDataTable();
            using(var taSecurityRoles = new SecurityRoleTableAdapter())
                taSecurityRoles.Fill(dtSecurityRoles);

            Worksheet wks = CreateWorksheet("User Security Permissions", 1, 1);
            FormatHeader(wks.Rows[0]);

            //Format first column
            wks.Columns[0].Width = 25 * 256;
            wks.Rows[0].Cells[0].Value = "User";

            //List to hold permissions so that their index can be used for proper cell placement
            var securityRoleColumnIndex = new Dictionary <string, int>();
            int columnIndex = 1;

            //Security role id and description
            foreach(SecurityDataSet.SecurityRoleRow role in dtSecurityRoles)
            {
                //Security Role
                FormatBorder(wks.Rows[0].Cells[columnIndex], CellBorderLineStyle.Thin);
                wks.Columns[columnIndex].Width = 5 * 256;
                wks.Rows[0].Cells[columnIndex].CellFormat.Rotation = 45;
                wks.Rows[0].Cells[columnIndex].Value = role.SecurityRoleID;

                //Add the Security Description as a comment to the cell
                wks.Rows[0].Cells[columnIndex].Comment = new WorksheetCellComment { Text = new FormattedString(role.IsDescriptionNull() ? role.SecurityRoleID : role.Description), Visible = false };

                securityRoleColumnIndex.Add(role.SecurityRoleID, columnIndex);
                columnIndex++;
            }

            using(var taUser = new User_SecurityRolesTableAdapter())
            {
                int rowIndex = 1;

                //User Name
                foreach(SecurityDataSet.UsersRow users in dtUsers)
                {
                    taUser.FillAllByUser(dtUser, users.UserID);

                    FormatBorder(wks.Rows[rowIndex].Cells[0], CellBorderLineStyle.Thin);

                    if(rowIndex % 2 == 0)
                        FormatAlternateRow(wks.Rows[rowIndex]);

                    wks.Rows[rowIndex].Cells[0].Value = users.Name;

                    FormatBorder(wks.Rows[rowIndex], CellBorderLineStyle.Thin);

                    //Each security permission role for this user
                    foreach(SecurityDataSet.User_SecurityRolesRow roles in dtUser)
                    {
                        int colIndex = securityRoleColumnIndex[roles.SecurityRoleID];
                        wks.Rows[rowIndex].Cells[colIndex].Value = "X";
                    }

                    rowIndex++;
                }
            }
        }

        #endregion
    }

    public class SecurityGroupPermissionsReport : ExcelBaseReport
    {
        #region Properties

        public override string Title
        {
            get { return "Security Group Permissions"; }
        }

        /// <summary>
        ///     Gets the report page orientation.
        /// </summary>
        /// <value> The report page orientation. </value>
        protected override PageOrientation ReportPageOrientation
        {
            get { return PageOrientation.Landscape; }
        }

        #endregion

        #region Methods
        
        protected override void CreateReport(CancellationToken cancellationToken) =>
            CreateReport();

        private void CreateReport()
        {
            var dtSecurityGroups = new SecurityReport.SecurityGroupDataTable();
            var taSecurityGroups = new Data.Reports.SecurityReportTableAdapters.SecurityGroupTableAdapter();
            taSecurityGroups.Fill(dtSecurityGroups);

            CreateReportExcel(dtSecurityGroups);
        }

        /// <summary>
        ///     Creates the Security Group Permissions report in Excel format
        /// </summary>
        /// <param name="dtUsers"> The users. </param>
        private void CreateReportExcel(SecurityReport.SecurityGroupDataTable securityGroups)
        {
            //Tabe (list) of group security permissions
            var dtSecurityRoles = new SecurityReport.SecurityRoleDataTable();

            using (var taSecurityRoles = new Data.Reports.SecurityReportTableAdapters.SecurityRoleTableAdapter())
                taSecurityRoles.Fill(dtSecurityRoles);

            var wks = CreateWorksheet("Security Group Permissions", 1, 1);

            //Header row
            base.FormatHeader(wks.Rows[0]);

            wks.Columns[0].Width = 25 * 256;
            wks.Rows[0].Cells[0].Value = "Security Group";

            //List to hold permissions so that their index can be used for proper cell placement
            var securityRoleColumnIndex = new Dictionary <string, int>();
            int columnIndex = 1;

            //Security role id and description
            foreach(SecurityReport.SecurityRoleRow role in dtSecurityRoles)
            {
                FormatBorder(wks.Rows[0].Cells[columnIndex], CellBorderLineStyle.Thin);
                wks.Columns[columnIndex].Width = 5 * 256;
                wks.Rows[0].Cells[columnIndex].CellFormat.Rotation = 45;
                wks.Rows[0].Cells[columnIndex].Value = role.SecurityRoleID;

                //Add the Security Description as a comment to the cell
                var description = role.IsDescriptionNull()
                    ? string.Empty
                    : role.Description;

                wks.Rows[0].Cells[columnIndex].Comment = new WorksheetCellComment { Text = new FormattedString(description), Visible = false };

                securityRoleColumnIndex.Add(role.SecurityRoleID, columnIndex);
                columnIndex++;
            }

            using(var taSecurityGroupRoles = new Data.Reports.SecurityReportTableAdapters.SecurityGroup_RoleTableAdapter())
            {
                int rowIndex = 1;

                foreach(SecurityReport.SecurityGroupRow group in securityGroups)
                {
                    //Security Group Name
                    FormatBorder(wks.Rows[rowIndex].Cells[0], CellBorderLineStyle.Thin);

                    if(rowIndex % 2 == 0)
                        FormatAlternateRow(wks.Rows[rowIndex]);

                    wks.Rows[rowIndex].Cells[0].Value = group.Name;

                    //for each row format every cell to the max number of columns created
                    //for(int colIndex2 = 1; colIndex2 < columnIndex; colIndex2++)
                        //FormatBorder(wks.Rows[rowIndex].Cells[colIndex2], CellBorderLineStyle.Thin, HorizontalCellAlignment.Center);
                    FormatBorder(wks.Rows[rowIndex], CellBorderLineStyle.Thin);

                    //Each security group permission
                    SecurityReport.SecurityGroup_RoleDataTable dtRoles = taSecurityGroupRoles.GetSecurityGroupPermissions(group.SecurityGroupID);

                    foreach(SecurityReport.SecurityGroup_RoleRow role in dtRoles)
                    {
                        int colIndex = securityRoleColumnIndex[role.SecurityRoleID];
                        wks.Rows[rowIndex].Cells[colIndex].Value = "X";
                    }

                    rowIndex++;
                }
            }
        }

        #endregion
    }
}