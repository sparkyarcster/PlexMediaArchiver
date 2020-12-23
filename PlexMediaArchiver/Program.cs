using System;

namespace PlexMediaArchiver
{
    class Program
    {
        static void Main(string[] args)
        {
            var reports = new MediaReport();
            var mediaData = reports.RunReports();
        }
    }
}
