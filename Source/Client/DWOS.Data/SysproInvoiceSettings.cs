using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Collections.Generic;
using System.Linq;

namespace DWOS.Data
{
    /// <summary>
    /// Represents invoice settings for SYSPRO integration.
    /// </summary>
    public class SysproInvoiceSettings
    {
        #region Properties

        public string PriceUnitEach
        {
            get; set;
        }

        public string PriceUnitLot
        {
            get; set;
        }

        [JsonConverter(typeof(StringEnumConverter))]
        public LineItemType LineItem
        {
            get; set;
        }

        public FieldMapping OrderHeaderMap { get; set; } = new FieldMapping();

        public FieldMapping StockLineMap { get; set; } = new FieldMapping();

        [JsonProperty("MiscChargeMap")]
        public FieldMapping StockLineFeeMap { get; set; } = new FieldMapping();

        public CommentMapping CommentMap { get; set; } = new CommentMapping();

        public bool IncludeEmptyFreightLine { get; set; } = true;

        public bool GenerateSingleFile { get; set; } = true;

        public bool IncludeDiscountsInFees { get; set; } = true;

        #endregion

        #region Methods

        public SysproInvoiceSettings Copy()
        {
            return new SysproInvoiceSettings
            {
                PriceUnitEach = PriceUnitEach,
                PriceUnitLot = PriceUnitLot,
                LineItem = LineItem,
                OrderHeaderMap = OrderHeaderMap?.Copy(),
                StockLineMap = StockLineMap?.Copy(),
                StockLineFeeMap = StockLineFeeMap?.Copy(),
                CommentMap = CommentMap?.Copy(),
                IncludeEmptyFreightLine = IncludeEmptyFreightLine,
                GenerateSingleFile = GenerateSingleFile,
                IncludeDiscountsInFees = IncludeDiscountsInFees
            };
        }

        #endregion

        #region FieldMapping class

        public class FieldMapping
        {
            #region Properties

            public List<Literal> Literals { get; set; } = new List<Literal>();

            public List<CustomField> CustomFields { get; set; } = new List<CustomField>();

            public List<Field> Fields { get; set; } = new List<Field>();

            #endregion

            #region Methods

            public List<IField> AllFields()
            {
                var fields = new List<IField>();
                fields.AddRange(Literals ?? Enumerable.Empty<IField>());
                fields.AddRange(CustomFields ?? Enumerable.Empty<IField>());
                fields.AddRange(Fields ?? Enumerable.Empty<IField>());

                return fields;
            }

            public void Add(IField field)
            {
                var literal = field as Literal;
                var customField = field as CustomField;
                var dwosField = field as Field;

                if (literal != null)
                {
                    Literals.Add(literal);
                }
                else if (customField != null)
                {
                    CustomFields.Add(customField);
                }
                else if (dwosField != null)
                {
                    Fields.Add(dwosField);
                }
            }

            public void Remove(IField field)
            {
                var literal = field as Literal;
                var customField = field as CustomField;
                var dwosField = field as Field;

                if (literal != null)
                {
                    Literals.Remove(literal);
                }
                else if (customField != null)
                {
                    CustomFields.Remove(customField);
                }
                else if (dwosField != null)
                {
                    Fields.Remove(dwosField);
                }
            }

            internal FieldMapping Copy()
            {
                var literals = Literals.Select(m => m.Copy()).ToList();
                var customFields = CustomFields.Select(m => m.Copy()).ToList();
                var fields = Fields.Select(m => m.Copy()).ToList();

                return new FieldMapping
                {
                    Literals = literals,
                    CustomFields = customFields,
                    Fields = fields
                };
            }

            #endregion
        }

        #endregion

        #region CommentMapping class

        public class CommentMapping
        {
            #region Properties

            public List<CommentLiteral> Literals { get; set; } = new List<CommentLiteral>();

            public List<CommentCustomField> CustomFields { get; set; } = new List<CommentCustomField>();

            public List<CommentField> Fields { get; set; } = new List<CommentField>();

            #endregion

            #region Methods

            public List<ICommentField> AllFields()
            {
                var fields = new List<ICommentField>();
                fields.AddRange(Literals ?? Enumerable.Empty<ICommentField>());
                fields.AddRange(CustomFields ?? Enumerable.Empty<ICommentField>());
                fields.AddRange(Fields ?? Enumerable.Empty<ICommentField>());

                return fields;
            }

            public void Add(ICommentField field)
            {
                var literal = field as CommentLiteral;
                var customField = field as CommentCustomField;
                var dwosField = field as CommentField;

                if (literal != null)
                {
                    Literals.Add(literal);
                }
                else if (customField != null)
                {
                    CustomFields.Add(customField);
                }
                else if (dwosField != null)
                {
                    Fields.Add(dwosField);
                }
            }

            public void Remove(ICommentField field)
            {
                var literal = field as CommentLiteral;
                var customField = field as CommentCustomField;
                var dwosField = field as CommentField;

                if (literal != null)
                {
                    Literals.Remove(literal);
                }
                else if (customField != null)
                {
                    CustomFields.Remove(customField);
                }
                else if (dwosField != null)
                {
                    Fields.Remove(dwosField);
                }
            }

            internal CommentMapping Copy()
            {
                var literals = Literals.Select(m => m.Copy()).ToList();
                var customFields = CustomFields.Select(m => m.Copy()).ToList();
                var fields = Fields.Select(m => m.Copy()).ToList();

                return new CommentMapping
                {
                    Literals = literals,
                    CustomFields = customFields,
                    Fields = fields
                };
            }

            #endregion
        }

        #endregion

        #region Field-related interfaces and classes

        public interface IField
        {
            string Syspro { get; }

            string DisplayString { get; }
        }

        public interface ICommentField
        {
            string Format { get; }

            CommentType Type { get; }

            CommentPosition Position { get; set; }

            int Order { get; set; }

            [JsonIgnore]
            string DisplayString { get; }
        }

        public class CustomField : IField
        {
            public string Syspro { get; set; }

            public string TokenName { get; set; }

            [JsonIgnore]
            public string DisplayString => $"CustomField: {TokenName}";

            internal CustomField Copy()
            {
                return MemberwiseClone() as CustomField;
            }
        }

        public class CommentCustomField : ICommentField
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public CommentType Type { get; set; }

            public string TokenName { get; set; }

            public string Format { get; set; }

            [JsonConverter(typeof(StringEnumConverter))]
            public CommentPosition Position { get; set; } = CommentPosition.AfterEverythingElse;

            public int Order { get; set; } = -1;

            [JsonIgnore]
            public string DisplayString => $"CustomField: {TokenName}";

            internal CommentCustomField Copy()
            {
                return MemberwiseClone() as CommentCustomField;
            }
        }

        public class Literal : IField
        {
            public string Syspro { get; set; }

            public string Value { get; set; }

            [JsonIgnore]
            public string DisplayString => $"Literal: {Value}";

            internal Literal Copy()
            {
                return MemberwiseClone() as Literal;
            }
        }

        public class CommentLiteral : ICommentField
        {

            [JsonIgnore]
            public string Format => "N/A";

            [JsonConverter(typeof(StringEnumConverter))]
            public CommentType Type { get; set; }

            public string Value { get; set; }

            [JsonIgnore]
            public string DisplayString => $"Literal: {Value}";

            [JsonConverter(typeof(StringEnumConverter))]
            public CommentPosition Position { get; set; } = CommentPosition.AfterEverythingElse;

            public int Order { get; set; } = -1;

            internal CommentLiteral Copy()
            {
                return MemberwiseClone() as CommentLiteral;
            }
        }

        public class Field : IField
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public FieldType Dwos { get; set; }

            public string Syspro { get; set; }

            [JsonIgnore]
            public string DisplayString => $"Field: {Dwos}";

            internal Field Copy()
            {
                return MemberwiseClone() as Field;
            }
        }

        public class CommentField : ICommentField
        {
            [JsonConverter(typeof(StringEnumConverter))]
            public FieldType Dwos { get; set; }

            [JsonConverter(typeof(StringEnumConverter))]
            public CommentType Type { get; set; }

            public string Format { get; set; }

            [JsonConverter(typeof(StringEnumConverter))]
            public CommentPosition Position { get; set; } = CommentPosition.AfterEverythingElse;

            public int Order { get; set; } = -1;

            [JsonIgnore]
            public string DisplayString => $"Field: {Dwos}";

            internal CommentField Copy()
            {
                return MemberwiseClone() as CommentField;
            }
        }

        #endregion

        #region Enumerated types

        public enum LineItemType
        {
            Part,
            Process
        }

        public enum CommentType
        {
            Order,
            Invoice
        }

        public enum CommentPosition
        {
            AfterEverythingElse,
            AfterStockLine,
        }

        public enum FieldType
        {
            OrderId,
            OrderDate,
            OrderRequiredDate,
            OrderPurchaseOrder,
            OrderCustomerWo,
            OrderSerialNumber,
            OrderProductClass,
            OrderQuantity,
            OrderUnitPrice,

            /// <summary>
            /// Line Item Subtotal; does not include fees.
            /// </summary>
            OrderSubtotal,

            /// <summary>
            /// Order total; includes fees
            /// </summary>
            OrderTotalPrice,

            OrderUnit,
            OrderImportValue,
            OrderInvoice,
            OrderShippingCarrier,
            OrderDiscountTotal,
            OrderDiscountTotalPercent,
            CustomerAccountingId,
            CustomerAddressLine1,
            CustomerAddressLine2,
            CustomerAddressCityStateZip,
            CustomerAddressZip,

            CustomerShipName,
            CustomerShipAddressLine1,
            CustomerShipAddressLine2,
            CustomerShipAddressCityStateZip,
            CustomerShipAddressZip,

            PartName,
            PartDescription,
            ProcessProductCode,
            PackageNumber,
            ShipDate,
            FeeName,
            FeeInvoiceItemName,
            FeeTotal
        }

        #endregion
    }
}
