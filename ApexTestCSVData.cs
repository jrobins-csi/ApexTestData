using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using CsvHelper;
using NLog;

namespace Com.CelerisSoftware.ApexTestData
{
    public class CsvTestClass
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ReturnType { get; set; }
        public string Returns { get; set; }
        public string Var1 { get; set; }
        public string Var2 { get; set; }
        public string Var3 { get; set; }
    }

    public class ApexTestCSVData
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        public int RecordCount = 0;

        public int GetTestData(string csvPath)
        {
            using (var sr = new StreamReader(csvPath))
            {
                var reader = new CsvReader(sr);

                // Allow for commenting out CSV rows
                reader.Configuration.Comment = '#';
                reader.Configuration.Delimiter = ",";

                // Prepare for header validation
                reader.Configuration.HasHeaderRecord = true;
                // Turn off header validation
                // reader.Configuration.HeaderValidated = null;

                // Log exception handler for invalid header
                reader.Configuration.HeaderValidated = (isValid, headerNames, headerNameIndex, context) =>
                {
                    if (!isValid)
                    {
                        logger.Error($"Header matching ['{string.Join("', '", headerNames)}'] names at idex {headerNameIndex} was not found.");
                    }
                };

                // Prepare header for matching
                reader.Configuration.PrepareHeaderForMatch = header => header?.Trim();
                reader.Configuration.PrepareHeaderForMatch = header => header.Replace(" ", string.Empty);
                reader.Configuration.PrepareHeaderForMatch = header => header.Replace("_", string.Empty);
                reader.Configuration.PrepareHeaderForMatch = header => header.ToLower();

                // Ignore bad data
                // reader.Configuration.BadDataFound = null;

                // Log bad data
                reader.Configuration.BadDataFound = context =>
                {
                    logger.Error($"Bad data found on row '{context.RawRow}'");
                };

                try
                {
                    IEnumerable<CsvTestClass> records = reader.GetRecords<CsvTestClass>();
                    foreach (CsvTestClass record in records.Take(50))
                    {
                        ++RecordCount;
                        logger.Trace("Category: " + record.Category);
                        logger.Trace("Description: " + record.Description);
                        logger.Trace("Name: " + record.Name);
                        logger.Trace("ReturnType: " + record.ReturnType);
                        logger.Trace("Returns: " + record.Returns);
                        logger.Trace("Var1: " + record.Var1);
                        logger.Trace("Var2: " + record.Var2);
                        logger.Trace("Var3: " + record.Var3);
                    }
                }
                catch (Exception ex)
                {
                    logger.Trace("Error on reading" + ex);
                }
                return 0;
            }
        }
    }
}
