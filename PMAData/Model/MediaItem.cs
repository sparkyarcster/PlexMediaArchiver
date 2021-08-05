using System;
using System.Collections.Generic;
using System.Text;

namespace PMAData.Model
{
    public class MediaItem
    {
        public int ID { get; set; }
        public string Title { get; set; }
        public string Year { get; set; }
        public DateTime? LastPlayed { get; set; }
        public DateTime? Added { get; set; }
        public List<GenericData> GenericData { get; set; }

        public MediaItem()
        {
            GenericData = new List<GenericData>();
        }
    }
}
