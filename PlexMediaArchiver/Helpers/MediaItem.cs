using System;
using System.Collections.Generic;
using System.Text;

namespace PlexMediaArchiver.Helpers
{
    public class MediaItem
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public DateTime? LastPlayed { get; set; }
    }
}
