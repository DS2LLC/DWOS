namespace DWOS.AutomatedWorkOrderTool.Model
{
    // Assumptions:
    // - "pgm" is short for "program"
    // - "ProductCode" is short for "ProgramCode"
    // - "Ident" is short for "Identity"

    public class MasterListPart
    {
        #region Properties

        public string Name { get; set; }

        public string Description { get; set; }

        public string Program { get; set; }

        public string ProductCode { get; set; }

        public string Identity { get; set; }

        public string OspCode { get; set; }

        public string Preferred { get; set; }

        public string Alt { get; set; }

        public string MaterialDescription { get; set; }

        public string Mask { get; set; }

        public string PartMark { get; set; }

        public string IdentityCode { get; set; }

        public PartStatus Status { get; set; }

        public string ImportNotes { get; set; }

        #endregion

        #region PartStatus

        /// <summary>
        /// Represents the status of a <see cref="MasterListPart"/> instance.
        /// </summary>
        public enum PartStatus
        {
            /// <summary>
            /// New part
            /// </summary>
            New,

            /// <summary>
            /// Existing part to update; should only update price & add part
            /// marking if applicable.
            /// </summary>
            Existing,

            /// <summary>
            /// Existing part to update; may specify processes different than
            /// those currently defined.
            /// </summary>
            ExistingWithWarning,

            /// <summary>
            /// Invalid part.
            /// </summary>
            Invalid
        }

        #endregion
    }
}
