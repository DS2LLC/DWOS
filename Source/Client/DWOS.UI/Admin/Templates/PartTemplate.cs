using DWOS.Data;
using DWOS.Data.Datasets;
using DWOS.Shared.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using DWOS.UI.Utilities;
using NLog;

namespace DWOS.UI.Admin.Templates
{
    [Serializable]
    public class PartTemplate
    {
        #region Fields

        private const double PART_TEMPLATE_VERSION = 1.0;

        #endregion Fields

        #region Properties

        public bool IsActive { get; set; }
        public double TemplateVersion { get; set; }
        public double SurfaceArea { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public decimal EachPrice { get; set; }
        public string Revision { get; set; }
        public string Name { get; set; }
        public string Assembly { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
        public string Material { get; set; }
        public string Shape { get; set; }
        public decimal LotPrice { get; set; }
        public string Notes { get; set; }
        public bool PartMarking { get; set; }
        public List<MediaTemplate> Media { get; set; }
        public List<int> ProcessAliasIDs { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Creates the template.
        /// </summary>
        /// <param name="partRow">The part row.</param>
        public static PartTemplate CreateTemplate(PartsDataset.PartRow partRow)
        {
            var template = new PartTemplate();

            template.TemplateVersion = PART_TEMPLATE_VERSION;
            
            template.IsActive = partRow.Active;
            template.SurfaceArea = partRow.SurfaceArea;
            template.Length = partRow.Length;
            template.Width = partRow.Width;
            template.Height = partRow.Height;
            template.EachPrice = partRow.IsEachPriceNull() ? 0 : partRow.EachPrice;
            template.Revision = partRow.IsRevisionNull() ? "" : partRow.Revision;
            template.Name = partRow.Name;
            template.Assembly = partRow.IsAssemblyNumberNull() ? "" : partRow.AssemblyNumber;
            template.Manufacturer = partRow.IsManufacturerIDNull() ? "" : partRow.ManufacturerID;
            template.Model = partRow.IsAirframeNull() ? "" : partRow.Airframe;
            template.Material = partRow.IsMaterialNull() ? "" : partRow.Material;
            template.Shape = partRow.IsShapeTypeNull() ? "Box" : partRow.ShapeType;
            template.LotPrice = partRow.IsLotPriceNull() ? 0 : partRow.LotPrice;
            template.Notes = partRow.IsNotesNull() ? "" : partRow.Notes;
            template.PartMarking = partRow.PartMarking;

            template.Media = new List<MediaTemplate>();
            foreach (var media in partRow.GetPart_MediaRows().ToList())
            {
                var mediaTemplate = new MediaTemplate
                                        {
                                            IsDefault = !media.IsDefaultMediaNull() && media.DefaultMedia,
                                            MediaId = media.MediaID
                                        };
                template.Media.Add(mediaTemplate);
            }

            template.ProcessAliasIDs = new List<int>();
            foreach (var process in partRow.GetPartProcessRows().ToList())
            {
                template.ProcessAliasIDs.Add(process.ProcessAliasID);
            }

            return template;
        }

        public static PartTemplate LoadTemplate(byte[] bytes)
        {
            var uncompressedBytes = bytes.DecompressBytes();
            return FileSystem.DeserializeBinary(uncompressedBytes) as PartTemplate;
        }

        public static byte[] SaveTemplate(PartTemplate template)
        {
            var ms = FileSystem.SerializeBinary(template);
            return ms.ToArray().CompressBytes();
        }

        #endregion Methods

        #region MediaTemplate Class

        [Serializable]
        public class MediaTemplate
        {
            public bool IsDefault { get; set; }
            public int MediaId { get; set; }
        }

        #endregion MediaTemplate Class
    }
}
