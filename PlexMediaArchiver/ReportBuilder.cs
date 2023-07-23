using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;

namespace PlexMediaArchiver
{
    public class ReportBuilder
    {
        private string runPath;
        private string reportPath;
        private string filePath;

        public ReportBuilder()
        {
            runPath = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            reportPath = Path.Combine(runPath, "reports");
            
            if (!Directory.Exists(reportPath))
            {
                Directory.CreateDirectory(reportPath);
            }
        }

        public string BuildReport<T>(List<T> dataSet, string reportName = "")
        {
            if (dataSet == null || !dataSet.Any())
            {
                return null;
            }

            if (!string.IsNullOrEmpty(reportName))
            {
                filePath = Path.Combine(reportPath, $"report_{reportName}_{DateTime.Now.ToString("yyyyMMdd_hhmmss")}.csv");
            }
            else
            {
                filePath = Path.Combine(reportPath, $"report_{DateTime.Now.ToString("yyyyMMdd_hhmmss")}.csv");
            }

            var outputData = new System.Text.StringBuilder();
            var counter = 0;
            var properties = dataSet.First().GetType().GetProperties();

            foreach (var property in properties)
            {
                if (counter > 0)
                {
                    outputData.Append(",");
                }

                outputData.Append(property.Name);
                counter++;
            }

            outputData.Append(Environment.NewLine);

            foreach(var data in dataSet)
            {
                if (data != null)
                {
                    counter = 0;

                    foreach (var property in properties)
                    {
                        if (counter > 0)
                        {
                            outputData.Append(",");
                        }

                        var value = property.GetValue(data, null);

                        if (value != null)
                        {
                            if(property.Name == "MediaSize" || property.Name == "FileSize")
                            {
                                outputData.Append($"{(long.Parse(value.ToString()) / 1000000)} MB");
                            }
                            else if (value.ToString().Contains(","))
                            {
                                outputData.Append($"\"{value}\"");
                            }
                            else
                            {
                                outputData.Append(value.ToString());
                            }
                        }
                        else
                        {
                            outputData.Append("");
                        }

                        counter++;
                    }

                    outputData.Append(Environment.NewLine);
                }
            }

            System.IO.File.WriteAllText(filePath, outputData.ToString());

            Classes.AppLogger.log.Info($"Created {filePath}");

            return filePath;
        }
    }
}
