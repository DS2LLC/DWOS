using DWOS.Data.Datasets;
using System;
using System.Collections.Generic;

namespace DWOS.Data.Coc
{
    public class CertificateBatchOrder
    {
        #region Properties

        public int OrderId { get; }

        public int CustomerId { get; }

        public int PartId { get; }

        public string PartName { get; }

        public int BatchQuantity { get; }

        public List<string> SerialNumbers { get; }

        public decimal? ImportedPrice { get; }

        public List<OrderProcess> Processes { get; }

        public List<CustomField> CustomFields { get; }

        public List<PartCustomField> PartCustomFields { get; }

        public OrderPartMark PartMark { get; }

        public List<OrderRework> Reworks { get; }

        #endregion

        #region Methods

        public CertificateBatchOrder(int orderId,
            int customerId,
            int partId,
            string partName,
            int batchQuantity,
            List<string> serialNumbers, decimal? importedPrice,
            List<OrderProcess> processes,
            List<CustomField> customFields,
            List<PartCustomField> partCustomFields,
            OrderPartMark partMark,
            List<OrderRework> reworks)
        {
            OrderId = orderId;
            PartId = partId;
            CustomerId = customerId;
            PartName = partName;
            BatchQuantity = batchQuantity;
            SerialNumbers = serialNumbers
                ?? throw new ArgumentNullException(nameof(serialNumbers));
            ImportedPrice = importedPrice;
            Processes = processes
                ?? throw new ArgumentNullException(nameof(processes));

            CustomFields = customFields
                ?? throw new ArgumentNullException(nameof(customFields));

            PartCustomFields = partCustomFields
                ?? throw new ArgumentNullException(nameof(partCustomFields));

            PartMark = partMark;

            Reworks = reworks
                ?? throw new ArgumentNullException(nameof(reworks));
        }

        #endregion

        #region OrderProcess

        public class OrderProcess
        {
            #region Properties

            public int OrderProcessesId { get; }

            public int ProcessId { get; set; }

            public string AliasName { get; }

            public string Description { get; }

            public bool ShowOnCoc { get; }

            public List<OrderProcessStep> Steps { get; }

            public List<PartInspection> Inspections { get; }

            #endregion

            #region Methods

            public OrderProcess(int orderProcessesId, int processId, string aliasName,
                string description, bool showOnCoc, List<OrderProcessStep> steps,
                List<PartInspection> inspections)
            {
                OrderProcessesId = orderProcessesId;
                ProcessId = processId;
                AliasName = aliasName;
                Description = description;
                ShowOnCoc = showOnCoc;
                Steps = steps
                    ?? throw new ArgumentNullException(nameof(steps));

                Inspections = inspections
                    ?? throw new ArgumentNullException(nameof(inspections));
            }

            #endregion
        }

        #endregion

        #region OrderProcessStep

        public class OrderProcessStep
        {
            #region Properties

            public string Name { get; }

            public bool ShowOnCoc { get; }

            public List<OrderProcessQuestion> Questions { get; }

            #endregion

            #region Methods

            public OrderProcessStep(string name, bool showOnCoc, List<OrderProcessQuestion> questions)
            {
                Name = name;
                ShowOnCoc = showOnCoc;
                Questions = questions
                    ?? throw new ArgumentNullException(nameof(questions));
            }

            #endregion
        }

        #endregion

        #region OrderProcessQuestion

        public class OrderProcessQuestion
        {
            #region Properties

            public string Name { get; }

            public bool IsRequired { get; }

            public InputType InputType { get; }

            public string NumericUnit { get; }

            public string Answer { get; }

            #endregion

            #region Methods

            public OrderProcessQuestion(string name,
                bool isRequired,
                InputType inputType,
                string numericUnit,
                string answer)
            {
                Name = name;
                IsRequired = isRequired;
                InputType = inputType;
                NumericUnit = numericUnit;
                Answer = answer;
            }

            #endregion
        }

        #endregion

        #region PartInspection

        public class PartInspection
        {
            #region Properties

            public string Name { get; }

            public int RejectedQuantity { get; }

            public bool ShowOnCoc { get; }

            public List<PartInpsectionQuestion> Questions { get; }

            #endregion

            #region Methods

            public PartInspection(string name, int rejectedQuantity,
                bool showOnCoc,
                List<PartInpsectionQuestion> questions)
            {
                Name = name;
                RejectedQuantity = rejectedQuantity;
                ShowOnCoc = showOnCoc;
                Questions = questions
                    ?? throw new ArgumentNullException(nameof(questions));
            }

            #endregion
        }

        #endregion

        #region PartInspectionQuestion

        public class PartInpsectionQuestion
        {
            #region Properties

            public string QuestionName { get; }

            public string Answer { get; }

            #endregion

            #region Methods

            public PartInpsectionQuestion(string questionName, string answer)
            {
                QuestionName = questionName;
                Answer = answer;
            }

            #endregion
        }

        #endregion

        #region CustomField

        public class CustomField
        {
            #region Properties

            public int CustomFieldId { get; }

            public string Name { get; }

            public string Value { get; }

            public bool ShowOnCoc { get; }

            #endregion

            #region Methods

            public CustomField(int customFieldId, string name, string value, bool showOnCoc)
            {
                CustomFieldId = customFieldId;
                Name = name;
                Value = value;
                ShowOnCoc = showOnCoc;
            }

            #endregion
        }

        #endregion

        #region PartCustomField

        public class PartCustomField
        {
            #region Properties

            public int PartLevelCustomFieldId { get; }

            public string Name { get; }

            public string Value { get; }

            public bool ShowOnCoc { get; }

            #endregion

            #region Methods

            public PartCustomField(
                int partLevelCustomFieldId, string name,string value,
                bool showOnCoc)
            {
                PartLevelCustomFieldId = partLevelCustomFieldId;
                Name = name;
                Value = value;
                ShowOnCoc = showOnCoc;
            }

            #endregion
        }

        #endregion

        #region OrderPartMark

        public class OrderPartMark
        {
            #region Properties

            public string ProcessSpec { get; }

            public DateTime? DateMarked { get; }

            #endregion

            #region Methods

            public OrderPartMark(string processSpec, DateTime? dateMarked)
            {
                ProcessSpec = processSpec;
                DateMarked = dateMarked;
            }

            #endregion
        }

        #endregion

        #region OrderRework

        public class OrderRework
        {
            #region Properties

            public string Name { get; }

            public bool ShowOnCoc { get; }

            #endregion

            #region Methods

            public OrderRework(string name, bool showOnCoc)
            {
                Name = name;
                ShowOnCoc = showOnCoc;
            }

            #endregion
        }

        #endregion
    }
}
