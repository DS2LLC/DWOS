using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Reports;
using DWOS.Shared.Utilities;
using DWOS.UI.Properties;
using Newtonsoft.Json;
using Neodynamic.SDK.Printing;

namespace DWOS.UI.Utilities
{
    internal static class LabelFactory
    {
        public enum LabelType
        {
            [LabelCategory(LabelCategory.WO)]
            [LabelTypeRole(LabelTypeRole.Default)]
            WO = 1,

            [LabelCategory(LabelCategory.Shipping)]
            ShippingPackage = 2,

            [LabelCategory(LabelCategory.Shipping)]
            ShippingOrder = 3,

            [LabelCategory(LabelCategory.Container)]
            [LabelTypeRole(LabelTypeRole.Default)]
            Container = 4,

            [LabelCategory(LabelCategory.WO)]
            [LabelTypeRole(LabelTypeRole.Rework)]
            Rework = 5,
            
            [LabelCategory(LabelCategory.Container)]
            [LabelTypeRole(LabelTypeRole.COC)]
            COCContainer = 6,

            [LabelCategory(LabelCategory.Container)]
            [LabelTypeRole(LabelTypeRole.Rework)]
            ReworkContainer = 7,

            [LabelCategory(LabelCategory.WO)]
            [LabelTypeRole(LabelTypeRole.COC)]
            COC = 8,

            [LabelCategory(LabelCategory.WO)]
            [LabelTypeRole(LabelTypeRole.Hold)]
            Hold = 9,

            [LabelCategory(LabelCategory.Container)]
            [LabelTypeRole(LabelTypeRole.Hold)]
            HoldContainer = 10,

            [LabelCategory(LabelCategory.WO)]
            [LabelTypeRole(LabelTypeRole.OutsideProcessing)]
            OutsideProcessing = 11,

            [LabelCategory(LabelCategory.Container)]
            [LabelTypeRole(LabelTypeRole.OutsideProcessing)]
            OutsideProcessingContainer = 12,

            [LabelCategory(LabelCategory.WO)]
            [LabelTypeRole(LabelTypeRole.OutsideProcessingRework)]
            OutsideProcessingRework = 13,

            [LabelCategory(LabelCategory.Container)]
            [LabelTypeRole(LabelTypeRole.OutsideProcessingRework)]
            OutsideProcessingReworkContainer = 14,

            [LabelCategory(LabelCategory.WO)]
            [LabelTypeRole(LabelTypeRole.ExternalRework)]
            ExternalRework = 15,

            [LabelCategory(LabelCategory.Container)]
            [LabelTypeRole(LabelTypeRole.ExternalRework)]
            ExternalReworkContainer = 16,

            [LabelCategory(LabelCategory.Container)]
            [LabelTypeRole(LabelTypeRole.Receiving)]
            ReceivingContainer = 17,

            [LabelCategory(LabelCategory.WO)]
            [LabelTypeRole(LabelTypeRole.Racking)]
            Racking = 18
        }

        public enum LabelCategory
        {
            WO = 1,
            Container = 2,
            Shipping = 3
        }

        public enum LabelTypeRole
        {
            Default = 1,
            Rework = 2,
            COC = 3,
            Hold = 4,
            OutsideProcessing = 5,
            OutsideProcessingRework = 6,
            ExternalRework = 7,
            Receiving = 8,
            Racking = 9

        }

        public enum LabelTokens
        {
            CUSTOMERNAME, WORKORDER, PARTNAME, PARTQUANTITY, PARTREVISION, PARTDESCRIPTION, ORDERPRIORITY, CUSTOMERWO, USERNAME, SHIPMENTID,
            BOXNUMBER, SHIPPINGCARRIER, DATE, COMPANYNAME, PURCHASEORDER, REQUIREDDATE, CONTAINER, CONTAINERCOUNT, CONTAINERNUMBER, CONTAINERQTY, CUSTOMERADDRESS1, CUSTOMERADDRESS2, CUSTOMERCITYSTATEZIP,
            CURRENTPROCESS, CURRENTPROCESSALIAS, PROCESSNAME1, PROCESSDEPT1, PROCESSNAME2, PROCESSDEPT2, PROCESSNAME3, PROCESSDEPT3, PROCESSNAME4, PROCESSDEPT4, PROCESSNAME5, PROCESSDEPT5, PROCESSNAME6, PROCESSDEPT6, PROCESSNAME7, PROCESSDEPT7, PROCESSNAME8, PROCESSDEPT8, CHECKINCOMMAND,
            CUSTOM, GROSSWEIGHT, STEPORDER1, STEPORDER2, STEPORDER3, STEPORDER4, STEPORDER5, STEPORDER6, STEPORDER7, STEPORDER8, REWORKPENDING, ADDORDERCOMMAND, PROCESSORDERCOMMAND,
            TOTALSURFACEAREA, SHIPPINGNAME, SHIPPINGADDRESS1, SHIPPINGADDRESS2, SHIPPINGCITYSTATEZIP, SERIALNUMBER,ESTSHIPDATE,
            PROCESSFIXTURES1, PROCESSFIXTURES2, PROCESSFIXTURES3, PROCESSFIXTURES4, PROCESSFIXTURES5, PROCESSFIXTURES6, PROCESSFIXTURES7, PROCESSFIXTURES8,
            PROCESSFIXTUREWEIGHT1, PROCESSFIXTUREWEIGHT2, PROCESSFIXTUREWEIGHT3, PROCESSFIXTUREWEIGHT4, PROCESSFIXTUREWEIGHT5, PROCESSFIXTUREWEIGHT6, PROCESSFIXTUREWEIGHT7, PROCESSFIXTUREWEIGHT8, RECEVEINGID, RECEVIEVING2, COMPANYIMAGE
        }

        public static LabelCategory GetCategory(LabelType labelType)
        {
            var valueField = typeof(LabelType).GetMember(labelType.ToString()).FirstOrDefault();
            var categoryAttribute = valueField
                .GetCustomAttributes(typeof(LabelCategoryAttribute), false)
                .FirstOrDefault() as LabelCategoryAttribute;

            return categoryAttribute?.Category ?? LabelCategory.WO;
        }

        public static LabelType? GetLabelType(LabelCategory category, LabelTypeRole role)
        {
            LabelType? returnValue = null;
            var fields = typeof(LabelType).GetFields();
            foreach (var field in fields)
            {
                var roleAttribute = field.GetCustomAttributes(typeof(LabelTypeRoleAttribute), false)
                    .FirstOrDefault() as LabelTypeRoleAttribute;

                var categoryAttribute = field.GetCustomAttributes(typeof(LabelCategoryAttribute), false)
                    .FirstOrDefault() as LabelCategoryAttribute;

                if (roleAttribute?.Role == role && categoryAttribute.Category == category)
                {
                    returnValue = field.GetValue(null) as LabelType?;
                    break;
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Gets the tokens that can be used based on the label type. If the customer has a specific version then use that also...
        /// </summary>
        /// <param name="labelType">Type of the label.</param>
        /// <param name="customerID">The customer identifier.</param>
        /// <returns>List&lt;LabelEditor.Token&gt;.</returns>
        public static List <LabelEditor.Token> GetTokens(LabelType labelType, int customerID)
        {
            var tokens = new List <LabelEditor.Token>();

            tokens.Add(new LabelEditor.Token() { ID = LabelTokens.USERNAME.ToString(), DisplayName = "User Name", TokenType = LabelEditor.TokenType.Text, SampleValue = "Jon Doe" });
            tokens.Add(new LabelEditor.Token() { ID = LabelTokens.DATE.ToString(), DisplayName = "Date", TokenType = LabelEditor.TokenType.Text, SampleValue = DateTime.Now.ToShortDateString() });
            tokens.Add(new LabelEditor.Token() { ID = LabelTokens.COMPANYNAME.ToString(), DisplayName = "Company Name", TokenType = LabelEditor.TokenType.Text, SampleValue = ApplicationSettings.Current.CompanyName });
            tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERNAME.ToString(), DisplayName = "Customer Name", TokenType = LabelEditor.TokenType.Text, SampleValue = "My Customer" });
            //byte[] bytes = Encoding.ASCII.GetBytes(ApplicationSettings.Current.CompanyLogoImageEncoded64);
            tokens.Add(new LabelEditor.Token() { ID = LabelTokens.COMPANYIMAGE.ToString(), DisplayName = "Company LOGO", TokenType = LabelEditor.TokenType.Image, SampleValue = ApplicationSettings.Current.CompanyLogoImageEncoded64 });

            switch (labelType)
            {
                case LabelType.WO:
                case LabelType.Racking:
                case LabelType.OutsideProcessing:
                case LabelType.ExternalRework:
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.ORDERPRIORITY.ToString(), DisplayName = "Order Priority", TokenType = LabelEditor.TokenType.Text, SampleValue = "Normal" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.WORKORDER.ToString(), DisplayName = "Work Order", TokenType = LabelEditor.TokenType.Text, SampleValue = "123456" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.WORKORDER.ToString(), DisplayName = "Work Order", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "123456" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.SERIALNUMBER.ToString(), DisplayName = "Serial Number", TokenType = LabelEditor.TokenType.Text, SampleValue = "000000000000000" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERWO.ToString(), DisplayName = "Customer WO", TokenType = LabelEditor.TokenType.Text, SampleValue = "01717785" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTNAME.ToString(), DisplayName = "Part Name", TokenType = LabelEditor.TokenType.Text, SampleValue = "17P-12312311" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTNAME.ToString(), DisplayName = "Part Name", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "17P-12312311" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTDESCRIPTION.ToString(), DisplayName = "Part Description", TokenType = LabelEditor.TokenType.Text, SampleValue = "Aileron/Flap 'D' Section" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTQUANTITY.ToString(), DisplayName = "Part Quantity", TokenType = LabelEditor.TokenType.Text, SampleValue = "10" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTQUANTITY.ToString(), DisplayName = "Part Quantity", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "10" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PURCHASEORDER.ToString(), DisplayName = "Purchase Order", TokenType = LabelEditor.TokenType.Text, SampleValue = "P31665" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PURCHASEORDER.ToString(), DisplayName = "Purchase Order", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "P31665" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.REQUIREDDATE.ToString(), DisplayName = "Required Date", TokenType = LabelEditor.TokenType.Text, SampleValue = DateTime.Now.ToShortDateString() });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.ESTSHIPDATE.ToString(), DisplayName = "Estimated Shipping Date", TokenType = LabelEditor.TokenType.Text, SampleValue = DateTime.Now.ToShortDateString() });

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CHECKINCOMMAND.ToString(), DisplayName = "Order Check-in", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "~123456~" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.ADDORDERCOMMAND.ToString(), DisplayName = "Add Order to Package/Batch", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "`123456`" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSORDERCOMMAND.ToString(), DisplayName = "Process Order", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "!123456!" });

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.GROSSWEIGHT.ToString(), DisplayName = "Order Weight", TokenType = LabelEditor.TokenType.Text, SampleValue = "1445", ToolTip = "The total weight of the order." });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.TOTALSURFACEAREA.ToString(), DisplayName = "Order Surface Area", TokenType = LabelEditor.TokenType.Text, SampleValue = "1 in²", ToolTip = "The total surface area of parts in the order." });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERADDRESS1.ToString(), DisplayName = "Customer Address 1", TokenType = LabelEditor.TokenType.Text, SampleValue = "123 Main Street" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERADDRESS2.ToString(), DisplayName = "Customer Address 2", TokenType = LabelEditor.TokenType.Text, SampleValue = "ATTN: Billing Dept." });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERCITYSTATEZIP.ToString(), DisplayName = "Customer City, ST ZIP", TokenType = LabelEditor.TokenType.Text, SampleValue = "Niceville, FL 32578" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.SHIPPINGNAME.ToString(), DisplayName = "Customer Shipping Name", TokenType = LabelEditor.TokenType.Text, SampleValue = "My Customer - Ship To" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.SHIPPINGADDRESS1.ToString(), DisplayName = "Customer Shipping Address 1", TokenType = LabelEditor.TokenType.Text, SampleValue = "123 Main Street" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.SHIPPINGADDRESS2.ToString(), DisplayName = "Customer Shipping Address 2", TokenType = LabelEditor.TokenType.Text, SampleValue = "ATTN: Shipping Dept." });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.SHIPPINGCITYSTATEZIP.ToString(), DisplayName = "Customer Shipping City, ST ZIP", TokenType = LabelEditor.TokenType.Text, SampleValue = "Niceville, FL 32578" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CONTAINER.ToString(), DisplayName = "Container Id", TokenType = LabelEditor.TokenType.Text, SampleValue = "1" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CONTAINERQTY.ToString(), DisplayName = "Container Qty", TokenType = LabelEditor.TokenType.Text, SampleValue = "1122" });

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CURRENTPROCESS.ToString(), DisplayName = "Current Process", TokenType = LabelEditor.TokenType.Text, SampleValue = "Sulfuric Acid Anodize" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CURRENTPROCESSALIAS.ToString(), DisplayName = "Current Process Alias", TokenType = LabelEditor.TokenType.Text, SampleValue = "MIL-A-8625 (Rev. F1) Sulfuric Acid Anodize Type IIB, Class I" });


                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSNAME1.ToString(), DisplayName = "Process 1", TokenType = LabelEditor.TokenType.Text, SampleValue = "Sulfuric Acid Anodize" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSDEPT1.ToString(), DisplayName = "Process Dept. 1", TokenType = LabelEditor.TokenType.Text, SampleValue = "Chemical" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTURES1.ToString(), DisplayName = "Process Fixtures 1", TokenType = LabelEditor.TokenType.Text, SampleValue = "1122"} );
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTUREWEIGHT1.ToString(), DisplayName = "Process Fixture Weight 1", TokenType = LabelEditor.TokenType.Text, SampleValue = "1445"});

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSNAME2.ToString(), DisplayName = "Process 2", TokenType = LabelEditor.TokenType.Text, SampleValue = "FPI" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSDEPT2.ToString(), DisplayName = "Process Dept. 2", TokenType = LabelEditor.TokenType.Text, SampleValue = "NDT" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTURES2.ToString(), DisplayName = "Process Fixtures 2", TokenType = LabelEditor.TokenType.Text, SampleValue = "1122"} );
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTUREWEIGHT2.ToString(), DisplayName = "Process Fixture Weight 2", TokenType = LabelEditor.TokenType.Text, SampleValue = "1445"});

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSNAME3.ToString(), DisplayName = "Process 3", TokenType = LabelEditor.TokenType.Text, SampleValue = "Chemical Conversion" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSDEPT3.ToString(), DisplayName = "Process Dept. 3", TokenType = LabelEditor.TokenType.Text, SampleValue = "Chemical" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTURES3.ToString(), DisplayName = "Process Fixtures 3", TokenType = LabelEditor.TokenType.Text, SampleValue = "1122"} );
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTUREWEIGHT3.ToString(), DisplayName = "Process Fixture Weight 3", TokenType = LabelEditor.TokenType.Text, SampleValue = "1445"});

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSNAME4.ToString(), DisplayName = "Process 4", TokenType = LabelEditor.TokenType.Text, SampleValue = "Mask Part" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSDEPT4.ToString(), DisplayName = "Process Dept. 4", TokenType = LabelEditor.TokenType.Text, SampleValue = "Masking" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTURES4.ToString(), DisplayName = "Process Fixtures 4", TokenType = LabelEditor.TokenType.Text, SampleValue = "1122"} );
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTUREWEIGHT4.ToString(), DisplayName = "Process Fixture Weight 4", TokenType = LabelEditor.TokenType.Text, SampleValue = "1445"});

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSNAME5.ToString(), DisplayName = "Process 5", TokenType = LabelEditor.TokenType.Text, SampleValue = "PA Red" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSDEPT5.ToString(), DisplayName = "Process Dept. 5", TokenType = LabelEditor.TokenType.Text, SampleValue = "Paint" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTURES5.ToString(), DisplayName = "Process Fixtures 5", TokenType = LabelEditor.TokenType.Text, SampleValue = "1122"} );
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTUREWEIGHT5.ToString(), DisplayName = "Process Fixture Weight 5", TokenType = LabelEditor.TokenType.Text, SampleValue = "1445"});

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSNAME6.ToString(), DisplayName = "Process 6", TokenType = LabelEditor.TokenType.Text, SampleValue =  "PA Blue"});
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSDEPT6.ToString(), DisplayName = "Process Dept. 6", TokenType = LabelEditor.TokenType.Text, SampleValue = "Paint" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTURES6.ToString(), DisplayName = "Process Fixtures 6", TokenType = LabelEditor.TokenType.Text, SampleValue = "1122"} );
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTUREWEIGHT6.ToString(), DisplayName = "Process Fixture Weight 6", TokenType = LabelEditor.TokenType.Text, SampleValue = "1445"});

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSNAME7.ToString(), DisplayName = "Process 7", TokenType = LabelEditor.TokenType.Text, SampleValue = "PA Green" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSDEPT7.ToString(), DisplayName = "Process Dept. 7", TokenType = LabelEditor.TokenType.Text, SampleValue = "Paint" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTURES7.ToString(), DisplayName = "Process Fixtures 7", TokenType = LabelEditor.TokenType.Text, SampleValue = "1122"} );
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTUREWEIGHT7.ToString(), DisplayName = "Process Fixture Weight 7", TokenType = LabelEditor.TokenType.Text, SampleValue = "1445"});

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSNAME8.ToString(), DisplayName = "Process 8", TokenType = LabelEditor.TokenType.Text, SampleValue = "PA Yellow" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSDEPT8.ToString(), DisplayName = "Process Dept. 8", TokenType = LabelEditor.TokenType.Text, SampleValue = "Paint" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTURES8.ToString(), DisplayName = "Process Fixtures 8", TokenType = LabelEditor.TokenType.Text, SampleValue = "1122"} );
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTUREWEIGHT8.ToString(), DisplayName = "Process Fixture Weight 8", TokenType = LabelEditor.TokenType.Text, SampleValue = "1445"});

                    break;
                case LabelType.COC:
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.ORDERPRIORITY.ToString(), DisplayName = "Order Priority", TokenType = LabelEditor.TokenType.Text, SampleValue = "Normal" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.WORKORDER.ToString(), DisplayName = "Work Order", TokenType = LabelEditor.TokenType.Text, SampleValue = "123456" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.WORKORDER.ToString(), DisplayName = "Work Order", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "123456" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.SERIALNUMBER.ToString(), DisplayName = "Serial Number", TokenType = LabelEditor.TokenType.Text, SampleValue = "000000000000000" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERWO.ToString(), DisplayName = "Customer WO", TokenType = LabelEditor.TokenType.Text, SampleValue = "01717785" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTNAME.ToString(), DisplayName = "Part Name", TokenType = LabelEditor.TokenType.Text, SampleValue = "17P-12312311" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTNAME.ToString(), DisplayName = "Part Name", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "17P-12312311" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTDESCRIPTION.ToString(), DisplayName = "Part Description", TokenType = LabelEditor.TokenType.Text, SampleValue = "Aileron/Flap 'D' Section" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTQUANTITY.ToString(), DisplayName = "Part Quantity", TokenType = LabelEditor.TokenType.Text, SampleValue = "10" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTQUANTITY.ToString(), DisplayName = "Part Quantity", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "10" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PURCHASEORDER.ToString(), DisplayName = "Purchase Order", TokenType = LabelEditor.TokenType.Text, SampleValue = "P31665" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PURCHASEORDER.ToString(), DisplayName = "Purchase Order", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "P31665" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.REQUIREDDATE.ToString(), DisplayName = "Required Date", TokenType = LabelEditor.TokenType.Text, SampleValue = DateTime.Now.ToShortDateString() });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.ESTSHIPDATE.ToString(), DisplayName = "Estimated Shipping Date", TokenType = LabelEditor.TokenType.Text, SampleValue = DateTime.Now.ToShortDateString() });

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CHECKINCOMMAND.ToString(), DisplayName = "Order Check-in", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "~123456~" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.ADDORDERCOMMAND.ToString(), DisplayName = "Add Order to Package/Batch", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "`123456`" });

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.GROSSWEIGHT.ToString(), DisplayName = "Order Weight", TokenType = LabelEditor.TokenType.Text, SampleValue = "1445", ToolTip = "The total weight of the order." });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.TOTALSURFACEAREA.ToString(), DisplayName = "Order Surface Area", TokenType = LabelEditor.TokenType.Text, SampleValue = "1 in²", ToolTip = "The total surface area of parts in the order." });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERADDRESS1.ToString(), DisplayName = "Customer Address 1", TokenType = LabelEditor.TokenType.Text, SampleValue = "123 Main Street" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERADDRESS2.ToString(), DisplayName = "Customer Address 2", TokenType = LabelEditor.TokenType.Text, SampleValue = "ATTN: Billing Dept." });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERCITYSTATEZIP.ToString(), DisplayName = "Customer City, ST ZIP", TokenType = LabelEditor.TokenType.Text, SampleValue = "Niceville, FL 32578" });

                    break;
                case LabelType.Rework:
                case LabelType.Hold:
                case LabelType.OutsideProcessingRework:
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.ORDERPRIORITY.ToString(), DisplayName = "Order Priority", TokenType = LabelEditor.TokenType.Text, SampleValue = "Normal" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.WORKORDER.ToString(), DisplayName = "Work Order", TokenType = LabelEditor.TokenType.Text, SampleValue = "123456" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.WORKORDER.ToString(), DisplayName = "Work Order", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "123456" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.SERIALNUMBER.ToString(), DisplayName = "Serial Number", TokenType = LabelEditor.TokenType.Text, SampleValue = "000000000000000" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERWO.ToString(), DisplayName = "Customer WO", TokenType = LabelEditor.TokenType.Text, SampleValue = "01717785" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTNAME.ToString(), DisplayName = "Part Name", TokenType = LabelEditor.TokenType.Text, SampleValue = "17P-12312311" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTNAME.ToString(), DisplayName = "Part Name", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "17P-12312311" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTDESCRIPTION.ToString(), DisplayName = "Part Description", TokenType = LabelEditor.TokenType.Text, SampleValue = "Aileron/Flap 'D' Section" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTQUANTITY.ToString(), DisplayName = "Part Quantity", TokenType = LabelEditor.TokenType.Text, SampleValue = "10" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTQUANTITY.ToString(), DisplayName = "Part Quantity", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "10" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PURCHASEORDER.ToString(), DisplayName = "Purchase Order", TokenType = LabelEditor.TokenType.Text, SampleValue = "P31665" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PURCHASEORDER.ToString(), DisplayName = "Purchase Order", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "P31665" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.REQUIREDDATE.ToString(), DisplayName = "Required Date", TokenType = LabelEditor.TokenType.Text, SampleValue = DateTime.Now.ToShortDateString() });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.ESTSHIPDATE.ToString(), DisplayName = "Estimated Shipping Date", TokenType = LabelEditor.TokenType.Text, SampleValue = DateTime.Now.ToShortDateString() });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CHECKINCOMMAND.ToString(), DisplayName = "Order Check-in", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "~123456~" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.REWORKPENDING.ToString(), DisplayName = "Rework Pending", TokenType = LabelEditor.TokenType.Text, SampleValue = "** Pending Rework Planning **" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.GROSSWEIGHT.ToString(), DisplayName = "Order Weight", TokenType = LabelEditor.TokenType.Text, SampleValue = "1445", ToolTip = "The total weight of the order." });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.TOTALSURFACEAREA.ToString(), DisplayName = "Order Surface Area", TokenType = LabelEditor.TokenType.Text, SampleValue = "1 in²", ToolTip = "The total surface area of parts in the order." });

                    tokens.AddRange(ReworkTokens());

                    break;

                case LabelType.ReceivingContainer:
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.ORDERPRIORITY.ToString(), DisplayName = "Order Priority", TokenType = LabelEditor.TokenType.Text, SampleValue = "Normal" });

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.RECEVEINGID.ToString(), DisplayName = "Receiving Order", TokenType = LabelEditor.TokenType.Text, SampleValue = "125578" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.RECEVEINGID.ToString(), DisplayName = "Receiving Order", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "125578" });

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERWO.ToString(), DisplayName = "Customer WO", TokenType = LabelEditor.TokenType.Text, SampleValue = "01717785" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERWO.ToString(), DisplayName = "Customer WO", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "01717785" });

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PURCHASEORDER.ToString(), DisplayName = "Purchase Order", TokenType = LabelEditor.TokenType.Text, SampleValue = "P31665" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PURCHASEORDER.ToString(), DisplayName = "Purchase Order", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "P31665" });

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTNAME.ToString(), DisplayName = "Part Name", TokenType = LabelEditor.TokenType.Text, SampleValue = "17P-12312311" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTNAME.ToString(), DisplayName = "Part Name", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "17P-12312311" });

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTDESCRIPTION.ToString(), DisplayName = "Part Description", TokenType = LabelEditor.TokenType.Text, SampleValue = "Aileron/Flap 'D' Section" });

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTQUANTITY.ToString(), DisplayName = "Part Quantity", TokenType = LabelEditor.TokenType.Text, SampleValue = "10" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTQUANTITY.ToString(), DisplayName = "Part Quantity", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "10" });

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.REQUIREDDATE.ToString(), DisplayName = "Required Date", TokenType = LabelEditor.TokenType.Text, SampleValue = DateTime.Now.ToShortDateString() });
                   
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.GROSSWEIGHT.ToString(), DisplayName = "Container Weight", TokenType = LabelEditor.TokenType.Text, SampleValue = "1445", ToolTip = "The weight of the container." });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CONTAINERCOUNT.ToString(), DisplayName = "Container Count", TokenType = LabelEditor.TokenType.Text, SampleValue = "4" });

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CONTAINERNUMBER.ToString(), DisplayName = "Container Number", TokenType = LabelEditor.TokenType.Text, SampleValue = "1" });

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CONTAINERQTY.ToString(), DisplayName = "Container Quantity", TokenType = LabelEditor.TokenType.Text, SampleValue = "1122" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CONTAINERQTY.ToString(), DisplayName = "Container Quantity", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "1122" });

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERADDRESS1.ToString(), DisplayName = "Customer Address 1", TokenType = LabelEditor.TokenType.Text, SampleValue = "123 Main Street" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERADDRESS2.ToString(), DisplayName = "Customer Address 2", TokenType = LabelEditor.TokenType.Text, SampleValue = "ATTN: Billing Dept." });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERCITYSTATEZIP.ToString(), DisplayName = "Customer City, ST ZIP", TokenType = LabelEditor.TokenType.Text, SampleValue = "Niceville, FL 32578" });

                    break;

                //end

                case LabelType.Container:
                case LabelType.OutsideProcessingContainer:
                case LabelType.ExternalReworkContainer:
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.ORDERPRIORITY.ToString(), DisplayName = "Order Priority", TokenType = LabelEditor.TokenType.Text, SampleValue = "Normal" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.WORKORDER.ToString(), DisplayName = "Work Order", TokenType = LabelEditor.TokenType.Text, SampleValue = "123456" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.WORKORDER.ToString(), DisplayName = "Work Order", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "123456" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.SERIALNUMBER.ToString(), DisplayName = "Serial Number", TokenType = LabelEditor.TokenType.Text, SampleValue = "000000000000000" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERWO.ToString(), DisplayName = "Customer WO", TokenType = LabelEditor.TokenType.Text, SampleValue = "01717785" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTNAME.ToString(), DisplayName = "Part Name", TokenType = LabelEditor.TokenType.Text, SampleValue = "17P-12312311" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTDESCRIPTION.ToString(), DisplayName = "Part Description", TokenType = LabelEditor.TokenType.Text, SampleValue = "Aileron/Flap 'D' Section" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTQUANTITY.ToString(), DisplayName = "Part Quantity", TokenType = LabelEditor.TokenType.Text, SampleValue = "10" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PURCHASEORDER.ToString(), DisplayName = "Purchase Order", TokenType = LabelEditor.TokenType.Text, SampleValue = "P31665" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PURCHASEORDER.ToString(), DisplayName = "Purchase Order", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "P31665" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.REQUIREDDATE.ToString(), DisplayName = "Required Date", TokenType = LabelEditor.TokenType.Text, SampleValue = DateTime.Now.ToShortDateString() });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.ESTSHIPDATE.ToString(), DisplayName = "Estimated Shipping Date", TokenType = LabelEditor.TokenType.Text, SampleValue = DateTime.Now.ToShortDateString() });

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CHECKINCOMMAND.ToString(), DisplayName = "Order Check-in", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "~123456~" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.ADDORDERCOMMAND.ToString(), DisplayName = "Add Order to Package/Batch", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "`123456`" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PROCESSORDERCOMMAND.ToString(), DisplayName = "Process Order", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "!123456!" });

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTQUANTITY.ToString(), DisplayName = "Part Quantity", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "10" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTNAME.ToString(), DisplayName = "Part Name", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "17P-12312311" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.GROSSWEIGHT.ToString(), DisplayName = "Container Weight", TokenType = LabelEditor.TokenType.Text, SampleValue = "1445", ToolTip = "The weight of the container." });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CONTAINERCOUNT.ToString(), DisplayName = "Container Count", TokenType = LabelEditor.TokenType.Text, SampleValue = "4" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CONTAINER.ToString(), DisplayName = "Container Id", TokenType = LabelEditor.TokenType.Text, SampleValue = "1487869" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CONTAINERNUMBER.ToString(), DisplayName = "Container Number", TokenType = LabelEditor.TokenType.Text, SampleValue = "1" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CONTAINER.ToString(), DisplayName = "Container Id", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "1487869" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CONTAINERQTY.ToString(), DisplayName = "Container Quantity", TokenType = LabelEditor.TokenType.Text, SampleValue = "1122" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CONTAINERQTY.ToString(), DisplayName = "Container Quantity", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "1122" });

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERADDRESS1.ToString(), DisplayName = "Customer Address 1", TokenType = LabelEditor.TokenType.Text, SampleValue = "123 Main Street" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERADDRESS2.ToString(), DisplayName = "Customer Address 2", TokenType = LabelEditor.TokenType.Text, SampleValue = "ATTN: Billing Dept." });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERCITYSTATEZIP.ToString(), DisplayName = "Customer City, ST ZIP", TokenType = LabelEditor.TokenType.Text, SampleValue = "Niceville, FL 32578" });
                    
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERWO.ToString(), DisplayName = "Customer WO", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "01717785" });

                    break;
                case LabelType.COCContainer:
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.ORDERPRIORITY.ToString(), DisplayName = "Order Priority", TokenType = LabelEditor.TokenType.Text, SampleValue = "Normal" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.WORKORDER.ToString(), DisplayName = "Work Order", TokenType = LabelEditor.TokenType.Text, SampleValue = "123456" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.WORKORDER.ToString(), DisplayName = "Work Order", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "123456" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.SERIALNUMBER.ToString(), DisplayName = "Serial Number", TokenType = LabelEditor.TokenType.Text, SampleValue = "000000000000000" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERWO.ToString(), DisplayName = "Customer WO", TokenType = LabelEditor.TokenType.Text, SampleValue = "01717785" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTNAME.ToString(), DisplayName = "Part Name", TokenType = LabelEditor.TokenType.Text, SampleValue = "17P-12312311" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTDESCRIPTION.ToString(), DisplayName = "Part Description", TokenType = LabelEditor.TokenType.Text, SampleValue = "Aileron/Flap 'D' Section" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTQUANTITY.ToString(), DisplayName = "Part Quantity", TokenType = LabelEditor.TokenType.Text, SampleValue = "10" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PURCHASEORDER.ToString(), DisplayName = "Purchase Order", TokenType = LabelEditor.TokenType.Text, SampleValue = "P31665" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PURCHASEORDER.ToString(), DisplayName = "Purchase Order", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "P31665" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.REQUIREDDATE.ToString(), DisplayName = "Required Date", TokenType = LabelEditor.TokenType.Text, SampleValue = DateTime.Now.ToShortDateString() });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.ESTSHIPDATE.ToString(), DisplayName = "Estimated Shipping Date", TokenType = LabelEditor.TokenType.Text, SampleValue = DateTime.Now.ToShortDateString() });

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTQUANTITY.ToString(), DisplayName = "Part Quantity", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "10" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTNAME.ToString(), DisplayName = "Part Name", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "17P-12312311" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.GROSSWEIGHT.ToString(), DisplayName = "Container Weight", TokenType = LabelEditor.TokenType.Text, SampleValue = "1445", ToolTip = "The weight of the container." });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CONTAINERCOUNT.ToString(), DisplayName = "Container Count", TokenType = LabelEditor.TokenType.Text, SampleValue = "4" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CONTAINERNUMBER.ToString(), DisplayName = "Container Number", TokenType = LabelEditor.TokenType.Text, SampleValue = "1" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CONTAINER.ToString(), DisplayName = "Container Id", TokenType = LabelEditor.TokenType.Text, SampleValue = "1487869" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CONTAINER.ToString(), DisplayName = "Container Id", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "1487869" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CONTAINERQTY.ToString(), DisplayName = "Container Quantity", TokenType = LabelEditor.TokenType.Text, SampleValue = "1122" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CONTAINERQTY.ToString(), DisplayName = "Container Quantity", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "1122" });

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERADDRESS1.ToString(), DisplayName = "Customer Address 1", TokenType = LabelEditor.TokenType.Text, SampleValue = "123 Main Street" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERADDRESS2.ToString(), DisplayName = "Customer Address 2", TokenType = LabelEditor.TokenType.Text, SampleValue = "ATTN: Billing Dept." });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERCITYSTATEZIP.ToString(), DisplayName = "Customer City, ST ZIP", TokenType = LabelEditor.TokenType.Text, SampleValue = "Niceville, FL 32578" });

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERWO.ToString(), DisplayName = "Customer WO", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "01717785" });

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.ADDORDERCOMMAND.ToString(), DisplayName = "Add Order to Package/Batch", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "`123456`" });

                    break;
                case LabelType.ReworkContainer:
                case LabelType.OutsideProcessingReworkContainer:
                case LabelType.HoldContainer:
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.ORDERPRIORITY.ToString(), DisplayName = "Order Priority", TokenType = LabelEditor.TokenType.Text, SampleValue = "Normal" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.WORKORDER.ToString(), DisplayName = "Work Order", TokenType = LabelEditor.TokenType.Text, SampleValue = "123456" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.WORKORDER.ToString(), DisplayName = "Work Order", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "123456" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.SERIALNUMBER.ToString(), DisplayName = "Serial Number", TokenType = LabelEditor.TokenType.Text, SampleValue = "000000000000000" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERWO.ToString(), DisplayName = "Customer WO", TokenType = LabelEditor.TokenType.Text, SampleValue = "01717785" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTNAME.ToString(), DisplayName = "Part Name", TokenType = LabelEditor.TokenType.Text, SampleValue = "17P-12312311" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTDESCRIPTION.ToString(), DisplayName = "Part Description", TokenType = LabelEditor.TokenType.Text, SampleValue = "Aileron/Flap 'D' Section" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTQUANTITY.ToString(), DisplayName = "Part Quantity", TokenType = LabelEditor.TokenType.Text, SampleValue = "10" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PURCHASEORDER.ToString(), DisplayName = "Purchase Order", TokenType = LabelEditor.TokenType.Text, SampleValue = "P31665" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PURCHASEORDER.ToString(), DisplayName = "Purchase Order", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "P31665" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.REQUIREDDATE.ToString(), DisplayName = "Required Date", TokenType = LabelEditor.TokenType.Text, SampleValue = DateTime.Now.ToShortDateString() });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.ESTSHIPDATE.ToString(), DisplayName = "Estimated Shipping Date", TokenType = LabelEditor.TokenType.Text, SampleValue = DateTime.Now.ToShortDateString() });

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTQUANTITY.ToString(), DisplayName = "Part Quantity", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "10" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTNAME.ToString(), DisplayName = "Part Name", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "17P-12312311" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.GROSSWEIGHT.ToString(), DisplayName = "Container Weight", TokenType = LabelEditor.TokenType.Text, SampleValue = "1445", ToolTip = "The weight of the container." });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CONTAINERCOUNT.ToString(), DisplayName = "Container Count", TokenType = LabelEditor.TokenType.Text, SampleValue = "4" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CONTAINERNUMBER.ToString(), DisplayName = "Container Number", TokenType = LabelEditor.TokenType.Text, SampleValue = "1" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CONTAINER.ToString(), DisplayName = "Container Id", TokenType = LabelEditor.TokenType.Text, SampleValue = "1487869" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CONTAINER.ToString(), DisplayName = "Container Id", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "1487869" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CONTAINERQTY.ToString(), DisplayName = "Container Quantity", TokenType = LabelEditor.TokenType.Text, SampleValue = "1122" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CONTAINERQTY.ToString(), DisplayName = "Container Quantity", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "1122" });

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERADDRESS1.ToString(), DisplayName = "Customer Address 1", TokenType = LabelEditor.TokenType.Text, SampleValue = "123 Main Street" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERADDRESS2.ToString(), DisplayName = "Customer Address 2", TokenType = LabelEditor.TokenType.Text, SampleValue = "ATTN: Billing Dept." });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERCITYSTATEZIP.ToString(), DisplayName = "Customer City, ST ZIP", TokenType = LabelEditor.TokenType.Text, SampleValue = "Niceville, FL 32578" });

                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERWO.ToString(), DisplayName = "Customer WO", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "01717785" });

                    tokens.AddRange(ReworkTokens());

                    break;
                case LabelType.ShippingPackage:
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.SHIPPINGCARRIER.ToString(), DisplayName = "Shipping Carrier", TokenType = LabelEditor.TokenType.Text, SampleValue = "UPS" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.BOXNUMBER.ToString(), DisplayName = "Box Number", TokenType = LabelEditor.TokenType.Text, SampleValue = "1" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.SHIPMENTID.ToString(), DisplayName = "Package ID", TokenType = LabelEditor.TokenType.Text, SampleValue = "1000" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.SHIPMENTID.ToString(), DisplayName = "Package ID", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "1000" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERADDRESS1.ToString(), DisplayName = "Customer Address 1", TokenType = LabelEditor.TokenType.Text, SampleValue = "123 Main Street" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERADDRESS2.ToString(), DisplayName = "Customer Address 2", TokenType = LabelEditor.TokenType.Text, SampleValue = "ATTN: Billing Dept." });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERCITYSTATEZIP.ToString(), DisplayName = "Customer City, ST ZIP", TokenType = LabelEditor.TokenType.Text, SampleValue = "Niceville, FL 32578" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.SHIPPINGNAME.ToString(), DisplayName = "Customer Shipping Name", TokenType = LabelEditor.TokenType.Text, SampleValue = "My Customer - Ship To" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.SHIPPINGADDRESS1.ToString(), DisplayName = "Customer Shipping Address 1", TokenType = LabelEditor.TokenType.Text, SampleValue = "123 Main Street" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.SHIPPINGADDRESS2.ToString(), DisplayName = "Customer Shipping Address 2", TokenType = LabelEditor.TokenType.Text, SampleValue = "ATTN: Shipping Dept." });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.SHIPPINGCITYSTATEZIP.ToString(), DisplayName = "Customer Shipping City, ST ZIP", TokenType = LabelEditor.TokenType.Text, SampleValue = "Niceville, FL 32578" });
                    break;
                case LabelType.ShippingOrder:
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.ORDERPRIORITY.ToString(), DisplayName = "Order Priority", TokenType = LabelEditor.TokenType.Text, SampleValue = "Normal" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.WORKORDER.ToString(), DisplayName = "Work Order", TokenType = LabelEditor.TokenType.Text, SampleValue = "123456" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.WORKORDER.ToString(), DisplayName = "Work Order", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "123456" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.SERIALNUMBER.ToString(), DisplayName = "Serial Number", TokenType = LabelEditor.TokenType.Text, SampleValue = "000000000000000" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERWO.ToString(), DisplayName = "Customer WO", TokenType = LabelEditor.TokenType.Text, SampleValue = "01717785" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PURCHASEORDER.ToString(), DisplayName = "Purchase Order", TokenType = LabelEditor.TokenType.Text, SampleValue = "P31665" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PURCHASEORDER.ToString(), DisplayName = "Purchase Order", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "P31665" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTNAME.ToString(), DisplayName = "Part Name", TokenType = LabelEditor.TokenType.Text, SampleValue = "17P-12312311" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTNAME.ToString(), DisplayName = "Part Name", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "17P-12312311" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTDESCRIPTION.ToString(), DisplayName = "Part Description", TokenType = LabelEditor.TokenType.Text, SampleValue = "Aileron/Flap 'D' Section" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTQUANTITY.ToString(), DisplayName = "Part Quantity", TokenType = LabelEditor.TokenType.Text, SampleValue = "10" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTQUANTITY.ToString(), DisplayName = "Part Quantity", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "10" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERADDRESS1.ToString(), DisplayName = "Customer Address 1", TokenType = LabelEditor.TokenType.Text, SampleValue = "123 Main Street" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERADDRESS2.ToString(), DisplayName = "Customer Address 2", TokenType = LabelEditor.TokenType.Text, SampleValue = "ATTN: Billing Dept." });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.CUSTOMERCITYSTATEZIP.ToString(), DisplayName = "Customer City, ST ZIP", TokenType = LabelEditor.TokenType.Text, SampleValue = "Niceville, FL 32578" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.SHIPMENTID.ToString(), DisplayName = "Package ID", TokenType = LabelEditor.TokenType.Text, SampleValue = "1000" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.SHIPMENTID.ToString(), DisplayName = "Package ID", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "1000" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTREVISION.ToString(), DisplayName = "Part Revision", TokenType = LabelEditor.TokenType.Text, SampleValue = "A" });
                    tokens.Add(new LabelEditor.Token() { ID = LabelTokens.PARTREVISION.ToString(), DisplayName = "Part Revision", TokenType = LabelEditor.TokenType.Barcode, SampleValue = "A" });
                    break;
                default:
                    break;
            }
            
            //if tokens contain a work order then we can use custom order fields
            if (tokens.Exists(t => t.ID == LabelTokens.WORKORDER.ToString()))
            {
                if (customerID > 0)
                {
                    // Add fields for customer
                    var customFields = new Data.Datasets.CustomersDataset.CustomFieldDataTable();
                    using (var ta = new Data.Datasets.CustomersDatasetTableAdapters.CustomFieldTableAdapter())
                        ta.FillByCustomer(customFields, customerID);

                    foreach (var customField in customFields)
                    {
                        tokens.Add(new LabelEditor.Token() { ID = "CUSTOM_" + customField.CustomFieldID, DisplayName = customField.Name, TokenType = LabelEditor.TokenType.Text, SampleValue = "" });
                        tokens.Add(new LabelEditor.Token() { ID = "CUSTOM_" + customField.CustomFieldID, DisplayName = customField.Name, TokenType = LabelEditor.TokenType.Barcode, SampleValue = "N/A" });
                    }
                }
                else
                {
                    // Add fields for all customers
                    using (var ta = new Data.Datasets.OrderStatusDataSetTableAdapters.CustomFieldNameTableAdapter())
                    {
                        using (var dtCustomField = ta.GetData())
                        {
                            foreach (var customField in dtCustomField)
                            {
                                var id = $"CUSTOMNAME_{customField.Name}";
                                tokens.Add(new LabelEditor.Token { ID = id, DisplayName = customField.Name, TokenType = LabelEditor.TokenType.Text, SampleValue = string.Empty });
                                tokens.Add(new LabelEditor.Token { ID = id, DisplayName = customField.Name, TokenType = LabelEditor.TokenType.Barcode, SampleValue = "N/A" });
                            }
                        }
                    }
                }
            }

            tokens.Sort((t1, t2) => t1.ToString().CompareTo(t2.ToString()));

            return tokens;
        }

        private static List<LabelEditor.Token> ReworkTokens()
        {
            return new List<LabelEditor.Token>()
            {
                (new LabelEditor.Token() { ID = LabelTokens.STEPORDER1.ToString(), DisplayName = "Step Order 1", TokenType = LabelEditor.TokenType.Text, SampleValue = "1-" }),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSNAME1.ToString(), DisplayName = "Process 1", TokenType = LabelEditor.TokenType.Text, SampleValue = "Sulfuric Acid Anodize" }),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSDEPT1.ToString(), DisplayName = "Process Dept. 1", TokenType = LabelEditor.TokenType.Text, SampleValue = "Chemical" }),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTURES1.ToString(), DisplayName = "Process Fixtures 1", TokenType = LabelEditor.TokenType.Text, SampleValue = "1122"}),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTUREWEIGHT1.ToString(), DisplayName = "Process Fixture Weight 1", TokenType = LabelEditor.TokenType.Text, SampleValue = "1445"}),

                (new LabelEditor.Token() { ID = LabelTokens.STEPORDER2.ToString(), DisplayName = "Step Order 2", TokenType = LabelEditor.TokenType.Text, SampleValue = "2-" }),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSNAME2.ToString(), DisplayName = "Process 2", TokenType = LabelEditor.TokenType.Text, SampleValue = "FPI" }),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSDEPT2.ToString(), DisplayName = "Process Dept. 2", TokenType = LabelEditor.TokenType.Text, SampleValue = "NDT" }),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTURES2.ToString(), DisplayName = "Process Fixtures 2", TokenType = LabelEditor.TokenType.Text, SampleValue = "1122"}),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTUREWEIGHT2.ToString(), DisplayName = "Process Fixture Weight 2", TokenType = LabelEditor.TokenType.Text, SampleValue = "1445"}),

                (new LabelEditor.Token() { ID = LabelTokens.STEPORDER3.ToString(), DisplayName = "Step Order 3", TokenType = LabelEditor.TokenType.Text, SampleValue = "3-" }),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSNAME3.ToString(), DisplayName = "Process 3", TokenType = LabelEditor.TokenType.Text, SampleValue = "Chemical Conversion" }),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSDEPT3.ToString(), DisplayName = "Process Dept. 3", TokenType = LabelEditor.TokenType.Text, SampleValue = "Chemical" }),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTURES3.ToString(), DisplayName = "Process Fixtures 3", TokenType = LabelEditor.TokenType.Text, SampleValue = "1122"}),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTUREWEIGHT3.ToString(), DisplayName = "Process Fixture Weight 3", TokenType = LabelEditor.TokenType.Text, SampleValue = "1445"}),

                (new LabelEditor.Token() { ID = LabelTokens.STEPORDER4.ToString(), DisplayName = "Step Order 4", TokenType = LabelEditor.TokenType.Text, SampleValue = "4-" }),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSNAME4.ToString(), DisplayName = "Process 4", TokenType = LabelEditor.TokenType.Text, SampleValue = "Mask Part" }),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSDEPT4.ToString(), DisplayName = "Process Dept. 4", TokenType = LabelEditor.TokenType.Text, SampleValue = "Masking" }),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTURES4.ToString(), DisplayName = "Process Fixtures 4", TokenType = LabelEditor.TokenType.Text, SampleValue = "1122"}),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTUREWEIGHT4.ToString(), DisplayName = "Process Fixture Weight 4", TokenType = LabelEditor.TokenType.Text, SampleValue = "1445"}),

                (new LabelEditor.Token() { ID = LabelTokens.STEPORDER5.ToString(), DisplayName = "Step Order 5", TokenType = LabelEditor.TokenType.Text, SampleValue = "5-" }),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSNAME5.ToString(), DisplayName = "Process 5", TokenType = LabelEditor.TokenType.Text, SampleValue = "PA Red" }),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSDEPT5.ToString(), DisplayName = "Process Dept. 5", TokenType = LabelEditor.TokenType.Text, SampleValue = "Paint" }),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTURES5.ToString(), DisplayName = "Process Fixtures 5", TokenType = LabelEditor.TokenType.Text, SampleValue = "1122"}),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTUREWEIGHT5.ToString(), DisplayName = "Process Fixture Weight 5", TokenType = LabelEditor.TokenType.Text, SampleValue = "1445"}),

                (new LabelEditor.Token() { ID = LabelTokens.STEPORDER6.ToString(), DisplayName = "Step Order 6", TokenType = LabelEditor.TokenType.Text, SampleValue = "6-" }),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSNAME6.ToString(), DisplayName = "Process 6", TokenType = LabelEditor.TokenType.Text, SampleValue = "PA Blue" }),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSDEPT6.ToString(), DisplayName = "Process Dept. 6", TokenType = LabelEditor.TokenType.Text, SampleValue = "Paint" }),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTURES6.ToString(), DisplayName = "Process Fixtures 6", TokenType = LabelEditor.TokenType.Text, SampleValue = "1122"}),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTUREWEIGHT6.ToString(), DisplayName = "Process Fixture Weight 6", TokenType = LabelEditor.TokenType.Text, SampleValue = "1445"}),

                (new LabelEditor.Token() { ID = LabelTokens.STEPORDER7.ToString(), DisplayName = "Step Order 7", TokenType = LabelEditor.TokenType.Text, SampleValue = "7-" }),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSNAME7.ToString(), DisplayName = "Process 7", TokenType = LabelEditor.TokenType.Text, SampleValue = "PA Green" }),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSDEPT7.ToString(), DisplayName = "Process Dept. 7", TokenType = LabelEditor.TokenType.Text, SampleValue = "Paint" }),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTURES7.ToString(), DisplayName = "Process Fixtures 7", TokenType = LabelEditor.TokenType.Text, SampleValue = "1122"}),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTUREWEIGHT7.ToString(), DisplayName = "Process Fixture Weight 7", TokenType = LabelEditor.TokenType.Text, SampleValue = "1445"}),

                (new LabelEditor.Token() { ID = LabelTokens.STEPORDER8.ToString(), DisplayName = "Step Order 8", TokenType = LabelEditor.TokenType.Text, SampleValue = "8-" }),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSNAME8.ToString(), DisplayName = "Process 8", TokenType = LabelEditor.TokenType.Text, SampleValue = "PA Yellow" }),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSDEPT8.ToString(), DisplayName = "Process Dept. 8", TokenType = LabelEditor.TokenType.Text, SampleValue = "Paint" }),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTURES8.ToString(), DisplayName = "Process Fixtures 8", TokenType = LabelEditor.TokenType.Text, SampleValue = "1122"}),
                (new LabelEditor.Token() { ID = LabelTokens.PROCESSFIXTUREWEIGHT8.ToString(), DisplayName = "Process Fixture Weight 8", TokenType = LabelEditor.TokenType.Text, SampleValue = "1445"}),
            };
        }

        /// <summary>
        /// Creates and prints a label.
        /// </summary>
        /// <param name="labelData">Label to print</param>
        /// <param name="copies">Number of copies to print.</param>
        /// <exception cref="LabelPrinterException">The label printer fails to print.</exception>
        /// <exception cref="ArgumentNullException">labelData is null</exception>
        public static void PrintLabel(LabelData labelData, string printerName, int copies = 1)
        {
            if (labelData == null)
            {
                throw new ArgumentNullException(nameof(LabelData));
            }

            try
            {
                using (var pj = CreatePrintJob(printerName, labelData.Label, labelData.Orientation, copies))
                {
                    pj?.Print();
                }
            }
            catch (Exception exc)
            {
                throw new LabelPrinterException("Error printing label.", exc);
            }
        }

        /// <summary>
        /// Creates a labels and shows a preview of that label.
        /// </summary>
        /// <param name="labelData">The label to show.</param>
        /// <exception cref="LabelPrinterException">The label printer fails to print.</exception>
        /// <exception cref="ArgumentNullException">labelData is null.</exception>
        public static void PreviewLabel(LabelData labelData)
        {
            if (labelData == null)
            {
                throw new ArgumentNullException(nameof(labelData));
            }

            try
            {
                using (var pj = CreatePreviewJob(labelData.Label, labelData.Orientation, 1))
                {
                    if (pj != null)
                    {
                        var fileName = Path.Combine(FileSystem.GetTempDirectory(), "LABEL_" + new Random().Next(111111, 999999) + ".png");
                        pj.ExportToImage(fileName, new ImageSettings(ImageFormat.Png) { PngIncludeDpiMetadata = true }, 300);

                        FileLauncher.New()
                            .HandleErrorsWith((exception, errorMsg) => MessageBoxUtilities.ShowMessageBoxWarn(errorMsg, "Label"))
                            .Launch(fileName);
                    }
                }
            }
            catch (Exception exc)
            {
                throw new LabelPrinterException("Error printing label.", exc);
            }
        }

        /// <summary>
        /// Creates a print job for a label.
        /// </summary>
        /// <param name="label">Label to create a print job for.</param>
        /// <param name="orientation">Print orientation.</param>
        /// <param name="copies">Number of copies to print.</param>
        /// <returns>
        /// <see cref="PrintJob"/> instance if there is a label printer;
        /// otherwise, returns null.
        /// </returns>
        private static WindowsPrintJob CreatePrintJob(string shippingPrinterName, ThermalLabel label, PrintOrientation orientation, int copies)
        {
            ProgrammingLanguage printerLanguage;
            if (!Enum.TryParse<ProgrammingLanguage>(UserSettings.Default.ShippingLabelPrinterLanguage, out printerLanguage))
            {
                printerLanguage = ProgrammingLanguage.ZPL;
            }

            var pj = new WindowsPrintJob();
            pj.PrintOrientation = orientation;
            pj.Copies = copies;
            pj.PrinterSettings = new PrinterSettings
            {
                PrinterName = shippingPrinterName,
                ProgrammingLanguage = printerLanguage,
                Dpi = UserSettings.Default.LabelPrinterDpi
            };

            pj.ThermalLabel = label;

            return pj;
        }

        private static PrintJob CreatePreviewJob(ThermalLabel label, PrintOrientation orientation, int copies)
        {
            ProgrammingLanguage printerLanguage;
            if (!Enum.TryParse<ProgrammingLanguage>(UserSettings.Default.ShippingLabelPrinterLanguage, out printerLanguage))
            {
                printerLanguage = ProgrammingLanguage.ZPL;
            }

            var pj = new PrintJob();
            pj.PrintOrientation = orientation;
            pj.Copies = copies;
            pj.ThermalLabel = label;

            return pj;
        }

        public static ThermalLabel GenerateDefaultShippingOrderLabel(int orderID, string orderPriority, string customerName, string customerWO, string partName, int partQty, string shipper)
        {
            //Define a ThermalLabel object and set unit to inch and label size
            var label = new ThermalLabel(UnitType.Inch, 4, 2);
            double BORDER = .1;
            const string FONT = "Tahoma";

            /* FIRST LINE */
            var txtCompanyName = new TextItem(0, 0, 2.5, 0.5, ApplicationSettings.Current.CompanyName);
            txtCompanyName.Font.Bold = true;
            txtCompanyName.Font.Italic = true;
            txtCompanyName.Font.Underline = true;
            txtCompanyName.Sizing = TextSizing.Stretch;
            txtCompanyName.TextAlignment = TextAlignment.Center;
            txtCompanyName.TextPadding = new FrameThickness(.05);
            txtCompanyName.Font.Name = FONT;

            //Order Priority
            var txtPriority = new TextItem(2.5, 0.1, 1.5 - BORDER, 0.5, orderPriority.ToUpper());
            txtPriority.Font.Bold = true;
            txtPriority.Sizing = TextSizing.Stretch;
            txtPriority.TextAlignment = TextAlignment.Center;
            txtPriority.TextPadding = new FrameThickness(.1, .01, .1, .15);
            txtPriority.Font.Name = FONT;

            /* SECOND LINE */
            //Customer WO
            var txtCustomerWO = new TextItem(0, 0.5, 4 - BORDER, 0.25, "Customer WO: {0}".FormatWith(customerWO));
            txtCustomerWO.Font.Unit = FontUnit.Point;
            txtCustomerWO.Font.Size = 12;
            txtCustomerWO.BackColor = Neodynamic.SDK.Printing.Color.Black;
            txtCustomerWO.ForeColor = Neodynamic.SDK.Printing.Color.White;
            txtCustomerWO.TextPadding = new FrameThickness(.03);
            txtCustomerWO.BorderThickness = new FrameThickness(0, 0, 0, .01);
            txtCustomerWO.BorderColor = Neodynamic.SDK.Printing.Color.White;
            txtCustomerWO.Font.Name = FONT;

            /* THIRD LINE */
            //Part Number
            var txtPartNumber = new TextItem(0, 0.75, 3, 0.25, "Part: {0}".FormatWith(partName));
            txtPartNumber.Font.Unit = FontUnit.Point;
            txtPartNumber.Font.Size = 12;
            txtPartNumber.BackColor = Neodynamic.SDK.Printing.Color.Black;
            txtPartNumber.ForeColor = Neodynamic.SDK.Printing.Color.White;
            txtPartNumber.TextPadding = new FrameThickness(.03);
            txtPartNumber.Font.Name = FONT;

            //Part Quantity
            var txtPartQty = new TextItem(3, 0.75, 1 - BORDER, 0.25, "Total: {0}".FormatWith(partQty));
            txtPartQty.Font.Unit = FontUnit.Point;
            txtPartQty.Font.Size = 12;
            txtPartQty.TextAlignment = TextAlignment.Center;
            txtPartQty.BackColor = Neodynamic.SDK.Printing.Color.Black;
            txtPartQty.ForeColor = Neodynamic.SDK.Printing.Color.White;
            txtPartQty.TextPadding = new FrameThickness(.03);
            txtPartQty.BorderThickness = new FrameThickness(.01, 0, 0, 0);
            txtPartQty.BorderColor = Neodynamic.SDK.Printing.Color.White;
            txtPartQty.Font.Name = FONT;

            /* FOURTH LINE */
            //Customer Name
            var txtCustomerName = new TextItem(0, 1.0, 3, 0.5, customerName);
            txtCustomerName.Sizing = TextSizing.Stretch;
            txtCustomerName.TextAlignment = TextAlignment.Center;
            txtCustomerName.TextPadding = new FrameThickness(.03);
            txtCustomerName.BorderThickness = new FrameThickness(.01, 0, 0, .01);
            txtCustomerName.Font.Name = FONT;

            var txtPackageBy = new TextItem(3, 1, 1 - BORDER, 0.25, "Packaged By:");
            txtPackageBy.Sizing = TextSizing.Stretch;
            txtPackageBy.TextAlignment = TextAlignment.Center;
            txtPackageBy.TextPadding = new FrameThickness(.03);
            txtPackageBy.BorderThickness = new FrameThickness(.01, 0, .01, .01);
            txtPackageBy.Font.Name = FONT;

            var txtPackageUser = new TextItem(3, 1.25, 1 - BORDER, 0.25, shipper.TrimToMaxLength(25));
            txtPackageUser.Sizing = TextSizing.Stretch;
            txtPackageUser.TextAlignment = TextAlignment.Center;
            txtPackageUser.TextPadding = new FrameThickness(.03);
            txtPackageUser.BorderThickness = new FrameThickness(.01, .01, .01, .01);
            txtPackageUser.Font.Name = FONT;

            //LineShapeItem customerLine = new LineShapeItem(0, 1.5, 4, 0.03);
            //customerLine.Orientation = LineOrientation.Horizontal;

            /* FIFTH LINE */
            //WO Barcode
            var woBarcode = new BarcodeItem(0.1, 1.5, 2, 0.5, BarcodeSymbology.Code128, orderID.ToString());
            woBarcode.DisplayStartStopChar = false;
            woBarcode.BarHeight = 0.3;
            woBarcode.BarWidth = 0.02;
            woBarcode.AddChecksum = false;
            woBarcode.DisplayCode = true;
            woBarcode.BarcodeAlignment = BarcodeAlignment.MiddleCenter;

            ImageItem logoNADCAP = null;
            
            if (!string.IsNullOrEmpty(ApplicationSettings.Current.AccreditationLogoImagePath))
            {
                logoNADCAP = new ImageItem(2.6, 1.52);
                logoNADCAP.SourceBinary = MediaUtilities.GetImageAsBytes(Bitmap.FromFile(ApplicationSettings.Current.AccreditationLogoImagePath), 95);
                
                logoNADCAP.Width = 1.5 - BORDER;
                logoNADCAP.Height = .46;
                logoNADCAP.LockAspectRatio = LockAspectRatio.Fit;
                logoNADCAP.MonochromeSettings.DitherMethod = DitherMethod.Threshold;
                logoNADCAP.MonochromeSettings.Threshold = 80;
            }

            //Add items to ThermalLabel object...
            label.Items.Add(txtCompanyName);
            label.Items.Add(txtPriority);
            label.Items.Add(txtCustomerWO);
            label.Items.Add(txtPartNumber);
            label.Items.Add(txtPartQty);
            label.Items.Add(txtCustomerName);
            label.Items.Add(txtPackageBy);
            label.Items.Add(txtPackageUser);
            //label.Items.Add(customerLine);
            label.Items.Add(woBarcode);

            if (logoNADCAP != null)
                label.Items.Add(logoNADCAP);

            return label;
        }

        public static ThermalLabel GenerateDefaultShippingPackageLabel(int shipmentID, int boxNumber, string shippingCarrier, DateTime timestamp, string customerName, string shipper)
        {
            //Define a ThermalLabel object and set unit to inch and label size
            var label = new ThermalLabel(UnitType.Inch, 4, 2);
            double BORDER = .1;
            const string FONT = "Tahoma";

            /* FIRST LINE */
            var txtCompanyName = new TextItem(0, 0, 2.5, 0.5, ApplicationSettings.Current.CompanyName);
            txtCompanyName.Font.Bold = true;
            txtCompanyName.Font.Italic = true;
            txtCompanyName.Font.Underline = true;
            txtCompanyName.Sizing = TextSizing.Stretch;
            txtCompanyName.TextAlignment = TextAlignment.Center;
            txtCompanyName.TextPadding = new FrameThickness(.05);
            txtCompanyName.Font.Name = FONT;

            //Box Number
            var txtPriority = new TextItem(2.5, 0.1, 1.5 - BORDER, 0.5, "Box: " + boxNumber);
            txtPriority.Font.Bold = true;
            txtPriority.Sizing = TextSizing.Stretch;
            txtPriority.TextAlignment = TextAlignment.Center;
            txtPriority.TextPadding = new FrameThickness(.1, .01, .1, .15);
            txtPriority.Font.Name = FONT;

            /* SECOND LINE */
            //Timestamp
            var txtCustomerWO = new TextItem(0, 0.5, 4 - BORDER, 0.25, "Packaged: " + timestamp);
            txtCustomerWO.Font.Unit = FontUnit.Point;
            txtCustomerWO.Font.Size = 12;
            txtCustomerWO.BackColor = Neodynamic.SDK.Printing.Color.Black;
            txtCustomerWO.ForeColor = Neodynamic.SDK.Printing.Color.White;
            txtCustomerWO.TextPadding = new FrameThickness(.03);
            txtCustomerWO.BorderThickness = new FrameThickness(0, 0, 0, .01);
            txtCustomerWO.BorderColor = Neodynamic.SDK.Printing.Color.White;
            txtCustomerWO.Font.Name = FONT;

            /* THIRD LINE */
            //Carrier
            var txtPartNumber = new TextItem(0, 0.75, 4 - BORDER, 0.25, "Carrier: " + shippingCarrier);
            txtPartNumber.Font.Unit = FontUnit.Point;
            txtPartNumber.Font.Size = 12;
            txtPartNumber.BackColor = Neodynamic.SDK.Printing.Color.Black;
            txtPartNumber.ForeColor = Neodynamic.SDK.Printing.Color.White;
            txtPartNumber.TextPadding = new FrameThickness(.03);
            txtPartNumber.Font.Name = FONT;

            /* FOURTH LINE */
            //Customer Name
            var txtCustomerName = new TextItem(0, 1.0, 3, 0.5, customerName);
            txtCustomerName.Sizing = TextSizing.Stretch;
            txtCustomerName.TextAlignment = TextAlignment.Center;
            txtCustomerName.TextPadding = new FrameThickness(.03);
            txtCustomerName.BorderThickness = new FrameThickness(.01, 0, 0, .01);
            txtCustomerName.Font.Name = FONT;

            var txtPackageBy = new TextItem(3, 1, 1 - BORDER, 0.25, "Packaged By:");
            txtPackageBy.Sizing = TextSizing.Stretch;
            txtPackageBy.TextAlignment = TextAlignment.Center;
            txtPackageBy.TextPadding = new FrameThickness(.03);
            txtPackageBy.BorderThickness = new FrameThickness(.01, 0, .01, .01);
            txtPackageBy.Font.Name = FONT;

            var txtPackageUser = new TextItem(3, 1.25, 1 - BORDER, 0.25, shipper.TrimToMaxLength(25));
            txtPackageUser.Sizing = TextSizing.Stretch;
            txtPackageUser.TextAlignment = TextAlignment.Center;
            txtPackageUser.TextPadding = new FrameThickness(.03);
            txtPackageUser.BorderThickness = new FrameThickness(.01, .01, .01, .01);
            txtPackageUser.Font.Name = FONT;

            /* FIFTH LINE */
            //WO Barcode
            var woBarcode = new BarcodeItem(0.1, 1.5, 2, 0.5, BarcodeSymbology.Code128, Report.BARCODE_SHIPPING_PACKAGE_PREFIX + shipmentID.ToString() + Report.BARCODE_SHIPPING_PACKAGE_PREFIX);
            woBarcode.BarHeight = 0.3;
            woBarcode.BarWidth = 0.02;
            woBarcode.AddChecksum = false;
            woBarcode.DisplayCode = true;
            woBarcode.DisplayStartStopChar = false;
            woBarcode.BarcodeAlignment = BarcodeAlignment.MiddleCenter;

            ImageItem logoNadcap = null;
            if (!string.IsNullOrEmpty(ApplicationSettings.Current.AccreditationLogoImagePath))
            {
                logoNadcap = new ImageItem(2.6, 1.52);
                logoNadcap.SourceBinary = MediaUtilities.GetImageAsBytes(Bitmap.FromFile(ApplicationSettings.Current.AccreditationLogoImagePath), 95);
                logoNadcap.Width = 1.5 - BORDER;
                logoNadcap.Height = .46;
                logoNadcap.LockAspectRatio = LockAspectRatio.Fit;
                logoNadcap.MonochromeSettings.DitherMethod = DitherMethod.Threshold;
                logoNadcap.MonochromeSettings.Threshold = 80;
            }

            //Add items to ThermalLabel object...
            label.Items.Add(txtCompanyName);
            label.Items.Add(txtPriority);
            label.Items.Add(txtCustomerWO);
            label.Items.Add(txtPartNumber);
            label.Items.Add(txtCustomerName);
            label.Items.Add(txtPackageBy);
            label.Items.Add(txtPackageUser);
            label.Items.Add(woBarcode);
            if(logoNadcap != null)
                label.Items.Add(logoNadcap);

            return label;
        }

        public static List<TokenValue> GetCustomTokensValuesByOrder(int orderId, int customerId)
        {
            var tokens = new List<TokenValue>();
            var customFields = new Data.Datasets.OrdersDataSet.CustomFieldDataTable();
            var orderCustomFields = new Data.Datasets.OrdersDataSet.OrderCustomFieldsDataTable();

            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.CustomFieldTableAdapter())
            {
                ta.FillByCustomer(customFields, customerId);
            }

            using (var ta = new Data.Datasets.OrdersDataSetTableAdapters.OrderCustomFieldsTableAdapter())
                ta.FillByOrder(orderCustomFields, orderId);

            foreach (var orderCustomField in orderCustomFields)
            {
                if (!orderCustomField.IsValueNull())
                {
                    tokens.Add(new TokenValue() { TokenName = "CUSTOM_" + orderCustomField.CustomFieldID, Value = orderCustomField.Value });

                    var customField = customFields.FirstOrDefault(f => f.CustomFieldID == orderCustomField.CustomFieldID);

                    if (customField != null)
                    {
                        tokens.Add(new TokenValue { TokenName = "CUSTOMNAME_" + customField.Name, Value = orderCustomField.Value });
                    }
                }
            }

            return tokens;
        }

        public class TokenValue
        {
            #region Properties

            public string TokenName { get; set; }
            public string Value { get; set; }

            #endregion

            #region Methods

            public static TokenValue From(LabelTokens tokenName, string tokenValue)
            {
                return new TokenValue() { TokenName = tokenName.ToString(), Value = tokenValue };
            }

            #endregion
        }

        #region LabelData

        public sealed class LabelData
        {
            private static readonly Regex UnicodeRegex = new Regex(@"[^\u0000-\u007F]");

            public ThermalLabel Label
            {
                get;
                private set;
            }

            public PrintOrientation Orientation
            {
                get;
                private set;
            }

            /// <summary>
            /// Creates label data for use as a printout.
            /// </summary>
            /// <param name="labelType">Type of label to show.</param>
            /// <param name="orderId">Unique ID of the order.</param>
            /// <param name="tokens">Tokens to include in the label.</param>
            /// <returns>
            /// New <see cref="LabelData"/> instance if found; otherwise, returns null.
            /// </returns>
            public static LabelData GetLabelForOrder(LabelType labelType, int orderId, List<TokenValue> tokens)
            {
                string labelXML = null;

                int customerId;
                using (var taCustomer = new Data.Datasets.LabelDataSetTableAdapters.LabelCustomerSummaryTableAdapter())
                {
                    using (var dtCustomer = taCustomer.GetDataByOrder(orderId))
                    {
                        customerId = dtCustomer.FirstOrDefault()?.CustomerID ?? -1;
                    }
                }

                // Get Customer label
                using (var taLabel = new Data.Datasets.LabelDataSetTableAdapters.LabelsTableAdapter())
                    labelXML = taLabel.GetLabelDataByCustomerType(customerId, (int)labelType);

                // Get Product Class label
                if (string.IsNullOrWhiteSpace(labelXML) && FieldUtilities.IsFieldEnabled("Order", "Product Class"))
                {
                    var productClass = string.Empty;

                    using (var dtProductClass = new OrdersDataSet.OrderProductClassDataTable())
                    {
                        using (var taProductClass = new Data.Datasets.OrdersDataSetTableAdapters.OrderProductClassTableAdapter())
                        {
                            taProductClass.FillByOrder(dtProductClass, orderId);
                        }

                        var productClassRow = dtProductClass.FirstOrDefault();

                        if (productClassRow != null && !productClassRow.IsProductClassNull())
                        {
                            productClass = productClassRow.ProductClass;
                        }
                    }

                    using (var taProductClassLabel = new Data.Datasets.LabelDataSetTableAdapters.ProductClassLabelsTableAdapter())
                    {
                        labelXML = taProductClassLabel.GetLabelDataByProductClass((int)labelType, productClass);
                    }
                }

                // Get default label
                if (string.IsNullOrWhiteSpace(labelXML))
                {
                    using (var taLabel = new Data.Datasets.LabelDataSetTableAdapters.LabelTypeTableAdapter())
                        labelXML = taLabel.GetLabelData((int)labelType);
                }

                if (labelXML == null)
                {
                    return null;
                }

                return NewFromXML(tokens, labelXML);
            }

            /// <summary>
            /// Creates label data for use as a printout.
            /// </summary>
            /// <param name="labelType">Type of label to show.</param>
            /// <param name="customerID">Unique ID of the customer.</param>
            /// <param name="tokens">Tokens to include in the label.</param>
            /// <returns>
            /// New <see cref="LabelData"/> instance if found; otherwise, returns null.
            /// </returns>
            public static LabelData GetLabelForCustomer(LabelType labelType, int customerID, List<TokenValue> tokens)
            {
                string labelXML = null;

                //get the label specific to the customer
                using (var taLabel = new Data.Datasets.LabelDataSetTableAdapters.LabelsTableAdapter())
                    labelXML = taLabel.GetLabelDataByCustomerType(customerID, (int)labelType);

                //no customer label then get system default
                if (string.IsNullOrWhiteSpace(labelXML))
                {
                    using (var taLabel = new Data.Datasets.LabelDataSetTableAdapters.LabelTypeTableAdapter())
                        labelXML = taLabel.GetLabelData((int)labelType);
                }

                if (labelXML == null)
                {
                    return null;
                }

                return NewFromXML(tokens, labelXML);
            }

            /// <summary>
            /// Creates a default label for use as a printout.
            /// </summary>
            /// <returns>A new <see cref="LabelData"/> instance.</returns>
            public static LabelData CreateDefault()
            {
                return new LabelData()
                {
                    Label = new ThermalLabel(UnitType.Inch, 4, 2),
                    Orientation = PrintOrientation.Landscape90
                };
            }

            private static LabelData NewFromXML(List<TokenValue> tokens, string labelXML)
            {
                var label = ThermalLabel.CreateFromXmlTemplate(labelXML.TrimStart('?'));
                var orientation = LabelEditor.MainWindow.GetLabelOrientation(labelXML);

                //Add default tokens
                if (!tokens.Exists(t => t.TokenName == LabelTokens.USERNAME.ToString()))
                    tokens.Add(new TokenValue() { TokenName = LabelTokens.USERNAME.ToString(), Value = SecurityManager.Current.UserName });
                if (!tokens.Exists(t => t.TokenName == LabelTokens.DATE.ToString()))
                    tokens.Add(new TokenValue() { TokenName = LabelTokens.DATE.ToString(), Value = DateTime.Now.ToShortDateString() });
                if (!tokens.Exists(t => t.TokenName == LabelTokens.COMPANYNAME.ToString()))
                    tokens.Add(new TokenValue() { TokenName = LabelTokens.COMPANYNAME.ToString(), Value = ApplicationSettings.Current.CompanyName });

                //update the template with the token values passed in
                foreach (var item in label.Items)
                {
                    if (String.IsNullOrWhiteSpace(item.Name))
                    {
                        if (item is BarcodeItem)
                        {
                            var bci = ((BarcodeItem)item);
                            if (!string.IsNullOrWhiteSpace(bci.Tag)) //Multifield barcode
                            {
                                // Tags for multi-field barcodes are JSON-encoded.
                                var content = ReplaceTokens(JsonConvert.DeserializeObject<string>(bci.Tag), tokens);
                                bci.Code = FormatForSymbology(content, bci.Symbology);
                            }
                        }
                    }
                    else
                    {
                        var token = tokens.FirstOrDefault(t => t.TokenName.ToString() == item.Name);

                        if (token != null)
                        {
                            if (item is BarcodeItem)
                            {
                                var barcodeItem = ((BarcodeItem)item);
                                barcodeItem.Code = FormatForSymbology(token.Value, barcodeItem.Symbology);
                            }
                            else if (item is TextItem)
                            {
                                ((TextItem)item).Text = token.Value;
                            }
                        }
                        else
                        {
                            //remove template values
                            if (item is BarcodeItem barcodeItem)
                            {
                                barcodeItem.Code = string.Empty;
                            }
                            else if (item is TextItem textItem)
                            {
                                (textItem).Text = string.Empty;
                            }
                        }
                    }

                    // Fix barcode properties
                    // If these are not fixed, the barcode will be blank.
                    const double minimumModuleSize = 0.0417;
                    if (item is BarcodeItem bcItem)
                    {
                        if (bcItem.AztecCodeModuleSize < minimumModuleSize)
                        {
                            bcItem.AztecCodeModuleSize = minimumModuleSize;
                        }

                        if (bcItem.DataMatrixModuleSize < minimumModuleSize)
                        {
                            bcItem.DataMatrixModuleSize = minimumModuleSize;
                        }

                        if (bcItem.QRCodeModuleSize < minimumModuleSize)
                        {
                            bcItem.QRCodeModuleSize = minimumModuleSize;
                        }

                        if (bcItem.HanXinCodeModuleSize < minimumModuleSize)
                        {
                            bcItem.HanXinCodeModuleSize = minimumModuleSize;
                        }
                    }
                }

                return new LabelData()
                {
                    Label = label,
                    Orientation = orientation
                };
            }

            private static string FormatForSymbology(string value, BarcodeSymbology symbology)
            {
                if (symbology == BarcodeSymbology.Pdf417)
                {
                    return value;
                }

                // Many symbologies do not support Unicode characters - this
                // causes DWOS to crash.
                return UnicodeRegex.Replace(value, "?");
            }

            /// <summary>
            /// Replaces the contents containing tokens with actual values.
            /// </summary>
            /// <param name="content">The content.</param>
            /// <param name="tokens">The tokens.</param>
            /// <returns></returns>
            public static string ReplaceTokens(string content, List<TokenValue> tokens)
            {
                //Search and replace tokens in the form of: %TOKEN%
                var rex = new Regex(@"\%(.*?)\%");

                return (rex.Replace(content, delegate (Match m)
                {
                    string key = m.Groups[1].Value.Replace("%", String.Empty);
                    var token = tokens.FirstOrDefault(t => t.TokenName == key);
                    string rep = token == null ? m.Value : token.Value;
                    return (rep);
                }));
            }
        }

        #endregion

        #region LabelCategoryAttribute

        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
        public sealed class LabelCategoryAttribute
            : Attribute
        {
            #region Properties

            public LabelCategory Category { get; }

            #endregion

            #region Methods

            public LabelCategoryAttribute(LabelCategory category)
            {
                Category = category;
            }

            #endregion
        }

        #endregion

        #region LabelTypeRoleAttribute

        [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
        public sealed class LabelTypeRoleAttribute
            : Attribute
        {
            #region Properties

            public LabelTypeRole Role { get; }

            #endregion

            #region Methods

            public LabelTypeRoleAttribute(LabelTypeRole role)
            {
                Role = role;
            }

            #endregion
        }

        #endregion
    }
}