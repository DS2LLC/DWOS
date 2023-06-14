using DWOS.Shared.Utilities;
using Infragistics.Documents.Excel;
using NLog;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Runtime.Serialization;

namespace DWOS.DataImporter
{
    [DataContract]
    public class FieldMapCollection
    {
        #region Fields

        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        #endregion

        #region Properties

        [DataMember]
        public List<FieldMap> FieldMaps { get; set; }

        [DataMember]
        public List<FieldMap> UnmappedFields { get; set; }

        #endregion

        #region Methods

        public static FieldMapCollection LoadFieldMappings(string configName)
        {
            try
            {
                var filePath = System.IO.Path.Combine(FileSystem.ApplicationPath(), configName + ".config");

                if (File.Exists(filePath))
                {
                    using (Stream stream = new MemoryStream())
                    {
                        var data = System.Text.Encoding.UTF8.GetBytes(File.ReadAllText(filePath));
                        stream.Write(data, 0, data.Length);
                        stream.Position = 0;

                        var deserializer = new DataContractSerializer(typeof(FieldMapCollection));
                        var config = deserializer.ReadObject(stream) as FieldMapCollection;

                        if (config != null)
                            return config;
                    }
                }

                return null;
            }
            catch (System.Exception exc)
            {
                NLog.LogManager.GetCurrentClassLogger().Error(exc, "Error loading configuration from file.");
                return null;
            }
        }

        public static FieldMapCollection CreateFieldMap(Worksheet workSheet, DataTable table)
        {
            var mappings = new List<FieldMap>();
            var unmapped = new List<FieldMap>();

            //use first row as column headers
            foreach (var cell in workSheet.Rows[0].Cells)
            {
                if (cell.Value == null)
                {
                    continue;
                }

                var columnName = cell.Value.ToString();

                if(table.Columns.Contains(columnName))
                {
                    if (!table.Columns[columnName].ReadOnly) //skip read only columns (auto increment)
                        mappings.Add(new FieldMap() { ColumnName = columnName, ColumnIndex = cell.ColumnIndex });
                }
                else
                {
                    _log.Info("Unmapped column: " + columnName);
                    unmapped.Add(new FieldMap
                    {
                        ColumnName = columnName,
                        ColumnIndex = cell.ColumnIndex
                    });
                }
            }

            return new FieldMapCollection
            {
                FieldMaps = mappings,
                UnmappedFields = unmapped
            };
        }

        #endregion
    }
}