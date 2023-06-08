using System;
using System.IO;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using DWOS.Shared.Utilities;
using NLog;

namespace DWOS.UI.Utilities
{
    public static class DwosTabDataUtilities
    {
        private const string XML_NAME = "Name";
        private const string XML_KEY = "Key";
        private const string XML_DATA_TYPE = "DataType";
        private const string XML_VERSION = "Version";
        private const string XML_LAYOUT = "Layout";

        public static XDocument CreateDocument(DwosTabData data)
        {
            if (data == null)
            {
                return null;
            }

            var layoutData = String.IsNullOrEmpty(data.Layout)
                ? null
                : new XCData(data.Layout);

            var doc = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("Data",
                    new XElement(XML_NAME, data.Name),
                    new XElement(XML_KEY, data.Key),
                    new XElement(XML_DATA_TYPE, data.DataType),
                    new XElement(XML_VERSION, data.Version),
                    new XElement(XML_LAYOUT, layoutData)));

            return doc;
        }

        public static DwosTabData Import(XDocument doc)
        {
            if (doc?.Root == null)
            {
                return null;
            }

            var nameNode = doc.Root.Element(XML_NAME);
            var keyNode = doc.Root.Element(XML_KEY);
            var dataTypeNode = doc.Root.Element(XML_DATA_TYPE);
            var versionNode = doc.Root.Element(XML_VERSION);
            var layoutNode = doc.Root.Element(XML_LAYOUT);

            int? version;
            NullableParser.TryParse(versionNode?.Value, out version);

            return new DwosTabData
            {
                Name = nameNode?.Value,
                Key = keyNode?.Value,
                DataType = dataTypeNode?.Value,
                Layout = layoutNode?.Value,
                Version = version
            };
        }

        public static DwosTabData Import(string fileName)
        {
            try
            {
                return String.IsNullOrEmpty(fileName) ? null : Import(XDocument.Load(fileName));
            }
            catch (XmlException exc)
            {
                LogManager.GetCurrentClassLogger().Warn(exc, "Unable to import tab file.");
                return null;
            }
        }

        public static void DoExport(DwosTabData tab)
        {
            var fileName = String.Empty;

            // Show dialog
            using(var fileDialog = new SaveFileDialog())
            {
                var initialDirectory = Path.Combine(FileSystem.UserDocumentPath(), "Tabs");
                if(Directory.Exists(initialDirectory))
                {
                    Directory.CreateDirectory(initialDirectory);
                }

                fileDialog.AddExtension = true;
                fileDialog.Title = @"Export DWOS Tab";
                fileDialog.Filter = @"DWOS Tab (*.dwostab)|*.dwostab";
                fileDialog.OverwritePrompt = true;
                fileDialog.InitialDirectory = initialDirectory;
                fileDialog.FileName = tab.Name + ".dwostab";

                if(fileDialog.ShowDialog() == DialogResult.OK)
                {
                    fileName = fileDialog.FileName;
                }
            }

            // Save tab data
            if(!String.IsNullOrEmpty(fileName))
            {
                var doc = DwosTabDataUtilities.CreateDocument(tab);
                using(var fileStream = File.Open(fileName, FileMode.Create))
                {
                    doc.Save(fileStream);
                }

                MessageBoxUtilities.ShowMessageBoxOK("Successfully saved tab.", "Export Tab");
            }
        }
    }
}
