using System;
using System.Collections.Generic;
using System.Linq;

namespace DWOS.UI.Utilities.PartMarking
{
    [Serializable]
    public class PartMarkSettings
    {
        #region Properties

        public string EndOfLineCode { get; set; }

        public int MaxLines { get; set; }

        public List<FontSizeMatrices> FontSizes { get; set; }

        public List<MultiStrokeCode> MultiStrokeCodes { get; set; }

        public int FileVersion { get; set; }

        public string DeviceType { get; set; }

        public string PortName { get; set; } = "COM1";

        #endregion

        #region Methods

        public PartMarkSettings()
        {
            FontSizes        = new List<FontSizeMatrices>();
            MultiStrokeCodes = new List <MultiStrokeCode>();
            MaxLines         = 4;
        }

        public MultiStrokeCode GetMultiStrokeCode(MultiStroke strokeType)
        {
            return MultiStrokeCodes.FirstOrDefault(msc => msc.MultiStrokeType == strokeType);
        }

        public PartMarkMatrix GetFontMatrix(int numberOfLines, PartMarkFontSize fontSize)
        {
            var font = FontSizes.FirstOrDefault(f => f.FontSize == fontSize);

            if(font != null)
                return font.Matrices.FirstOrDefault(pm => pm.NumberOfLines == numberOfLines);

            return null;
        }

        #endregion

        #region FontSizeMatrices

        [Serializable]
        public class FontSizeMatrices
        {
            public PartMarkFontSize FontSize { get; set; }
            public List<PartMarkMatrix> Matrices { get; set; }

            public FontSizeMatrices() { }

            public FontSizeMatrices(PartMarkFontSize size)
            {
                FontSize = size;
                Matrices = new List<PartMarkMatrix>();
            }
        }

        #endregion

        #region PartMarkMatrix

        [Serializable]
        public class PartMarkMatrix
        {
            public int NumberOfLines { get; set; }
            public string HexCode { get; set; }
            public string Name { get; set; }

            public PartMarkMatrix() { }

            public PartMarkMatrix(int numberOfLines, string hexCodes, string name)
            {
                NumberOfLines = numberOfLines;
                HexCode = hexCodes;
                Name = name;
            }
        }

        #endregion

        #region MultiStrokeCode

        [Serializable]
        public class MultiStrokeCode
        {
            public MultiStroke MultiStrokeType { get; set; }
            public string HexCode { get; set; }
        }

        #endregion
    }
}
