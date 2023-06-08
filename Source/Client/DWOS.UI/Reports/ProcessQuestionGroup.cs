using System.Collections.Generic;
using System.Linq;

namespace DWOS.UI.Reports
{
    public class ProcessQuestionGroup
    {
        #region Properties

        public int ProcessId { get; set; }

        public string ProcessName { get; set; }

        public int? ProcessQuestionId { get; set; }

        public decimal? ProcessQuestionOrder { get; set; }

        public string ProcessQuestionName { get; set; }

        public string ProcessQuestionString => ProcessQuestionId.HasValue
            ? $"{ProcessQuestionOrder} - {ProcessQuestionName}"
            : "N/A";

        public int ProcessStepId { get; set; }

        public decimal ProcessStepOrder { get; set; }

        public string ProcessStepName { get; set; }

        public string ProcessStepString => $"{ProcessStepOrder} - {ProcessStepName}";

        public List<GroupQuestion> Questions { get; set; }

        public string IncludesString => Questions == null || Questions.Count(q => q.Include) == 0
            ? "<None>"
            : string.Join(",", Questions.Where(q => q.Include).Select(q => q.Name));

        #endregion

        #region GroupQuestion

        public class GroupQuestion
        {
            public int QuestionId { get; set; }

            public bool Include { get; set; }

            public string Name { get; set; }
        }

        #endregion
    }
}