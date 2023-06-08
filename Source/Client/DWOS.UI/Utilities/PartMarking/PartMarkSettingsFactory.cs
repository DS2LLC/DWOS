using System;
using System.IO;
using DWOS.Data;
using DWOS.Shared.Utilities;
using NLog;

namespace DWOS.UI.Utilities.PartMarking
{
    public static class PartMarkSettingsFactory
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();
        private const int CurrentFileVersion = 2;

        #endregion

        #region Methods
        
        public static PartMarkSettings Load()
        {
            var deviceType = UserSettings.Default.ParkMarkingType;
            var filePath = GetFilePath(deviceType);

            if (File.Exists(filePath))
            {
                var settings = new PartMarkSettings() as object;
                try
                {
                    FileSystem.DeserializeObjectXml(filePath, ref settings);
                    return (PartMarkSettings)settings;
                }
                catch (Exception ex)
                {
                    var errorMsg = $"Unable to deseralize {filePath} as {nameof(PartMarkSettings)}";
                    _log.Warn(ex, errorMsg);
                }
            }

            if (deviceType == PartMarkingDeviceType.VideoJetExcel)
            {
                return CreateDefaultVideoJetExcel();
            }

            if (deviceType == PartMarkingDeviceType.VideoJet1000Line)
            {
                return CreateDefaultVideo1000Line();
            }

            return null;
        }

        public static void Save(PartMarkSettings settings)
        {
            // Always save as current file version
            settings.FileVersion = CurrentFileVersion;

            var filePath = GetFilePath(UserSettings.Default.ParkMarkingType);

            if(File.Exists(filePath))
                File.Delete(filePath);

            FileSystem.SerializeObjectXml(filePath, settings);
        }

        private static PartMarkSettings CreateDefaultVideoJetExcel()
        {
            var settings = new PartMarkSettings() { MaxLines = 4, EndOfLineCode = "0D", FileVersion = CurrentFileVersion, DeviceType = PartMarkingDeviceType.VideoJetExcel.ToString() };

            var small = new PartMarkSettings.FontSizeMatrices(PartMarkFontSize.Small);
            small.Matrices.Add(new PartMarkSettings.PartMarkMatrix(1, "1B,04,01", "5 X 7 - 1 Line")); // :) 5x7 SL
            small.Matrices.Add(new PartMarkSettings.PartMarkMatrix(2, "1B,04,04", "5 X 7 - 2 Line")); // :) 5x7 TL
            small.Matrices.Add(new PartMarkSettings.PartMarkMatrix(3, "1B,04,08", "5 X 7 - 3 Line")); // :) 5x7/5x7/5x7
            small.Matrices.Add(new PartMarkSettings.PartMarkMatrix(4, "1B,04,16", "5 X 5 - 4 Line")); // :) 5x5/5x5/5x5/5x5
            settings.FontSizes.Add(small);

            var medium = new PartMarkSettings.FontSizeMatrices(PartMarkFontSize.Medium);
            medium.Matrices.Add(new PartMarkSettings.PartMarkMatrix(1, "1B,04,02", "7 X 9 - 1 Line")); // :) 7x9 SL
            //medium.Matrices.Add(new PartMarkSettings.PartMarkMatrix(2, "1B,04,1C", "7 X 9 - 2 Line")); // :)
            //medium.Matrices.Add(new PartMarkSettings.PartMarkMatrix(3, "1B,04,17", "7 X 9 - 3 Line")); // :)
            //medium.Matrices.Add(new PartMarkSettings.PartMarkMatrix(4, "1B,04,05", "5 X 7 - 4 Line"));
            settings.FontSizes.Add(medium);

            var large = new PartMarkSettings.FontSizeMatrices(PartMarkFontSize.Large);
            large.Matrices.Add(new PartMarkSettings.PartMarkMatrix(1, "1B,04,03", "10 X 16 - 1 Line")); // :) 10x16
            //large.Matrices.Add(new PartMarkSettings.PartMarkMatrix(2, "1B,04,24", "9 X 9 - 2 Line"));
            //large.Matrices.Add(new PartMarkSettings.PartMarkMatrix(3, "1B,04,07", "16 X 24 - 3 Line"));
            //large.Matrices.Add(new PartMarkSettings.PartMarkMatrix(4, "1B,04,05", "5 X 7 - 4 Line"));
            settings.FontSizes.Add(large);

            settings.MultiStrokeCodes.Add(new PartMarkSettings.MultiStrokeCode() { MultiStrokeType = MultiStroke.MultiStroke1, HexCode = "1B,03,06" });
            settings.MultiStrokeCodes.Add(new PartMarkSettings.MultiStrokeCode() { MultiStrokeType = MultiStroke.MultiStroke2, HexCode = "1B,03,07" });
            settings.MultiStrokeCodes.Add(new PartMarkSettings.MultiStrokeCode() { MultiStrokeType = MultiStroke.MultiStroke3, HexCode = "1B,03,08" });
            settings.MultiStrokeCodes.Add(new PartMarkSettings.MultiStrokeCode() { MultiStrokeType = MultiStroke.MultiStroke4, HexCode = "1B,03,09" });

            return settings;
        }

        private static PartMarkSettings CreateDefaultVideo1000Line()
        {
            var settings = new PartMarkSettings() { MaxLines = 4, EndOfLineCode = "0D", FileVersion = CurrentFileVersion, DeviceType = PartMarkingDeviceType.VideoJet1000Line.ToString() };

            var small = new PartMarkSettings.FontSizeMatrices(PartMarkFontSize.Small);
            small.Matrices.Add(new PartMarkSettings.PartMarkMatrix(1, "1B,04,01", "5 X 7 - 1 Line")); // 5x7 SL
            small.Matrices.Add(new PartMarkSettings.PartMarkMatrix(2, "1B,04,04", "5 X 7 - 2 Line")); // 5x7 TL
            small.Matrices.Add(new PartMarkSettings.PartMarkMatrix(3, "1B,04,08", "5 X 7 - 3 Line")); // 5x7/5x7/5x7
            small.Matrices.Add(new PartMarkSettings.PartMarkMatrix(4, "1B,04,16", "5 X 5 - 4 Line")); // 5x5/5x5/5x5/5x5
            settings.FontSizes.Add(small);

            var medium = new PartMarkSettings.FontSizeMatrices(PartMarkFontSize.Medium);
            medium.Matrices.Add(new PartMarkSettings.PartMarkMatrix(1, "1B,04,02", "7 X 9 - 1 Line")); // 7x9 SL
            settings.FontSizes.Add(medium);

            var large = new PartMarkSettings.FontSizeMatrices(PartMarkFontSize.Large);
            large.Matrices.Add(new PartMarkSettings.PartMarkMatrix(1, "1B,04,03", "10 X 16 - 1 Line")); // :) 10x16
            settings.FontSizes.Add(large);

            settings.MultiStrokeCodes.Add(new PartMarkSettings.MultiStrokeCode() { MultiStrokeType = MultiStroke.MultiStroke1, HexCode = "1B,03,06" });
            settings.MultiStrokeCodes.Add(new PartMarkSettings.MultiStrokeCode() { MultiStrokeType = MultiStroke.MultiStroke2, HexCode = "1B,03,07" });
            settings.MultiStrokeCodes.Add(new PartMarkSettings.MultiStrokeCode() { MultiStrokeType = MultiStroke.MultiStroke3, HexCode = "1B,03,08" });
            settings.MultiStrokeCodes.Add(new PartMarkSettings.MultiStrokeCode() { MultiStrokeType = MultiStroke.MultiStroke4, HexCode = "1B,03,09" });

            return settings;
        }

        private static string GetFilePath(PartMarkingDeviceType deviceType)
        {
            return Path.Combine(FileSystem.UserAppDataPath(), "PartMarkSettings_" + deviceType + ".xml");
        }

        #endregion
    }
}
