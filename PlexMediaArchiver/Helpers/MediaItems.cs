using System;
using System.Collections.Generic;
using System.Text;

namespace PlexMediaArchiver.Helpers
{
    public class MediaItems
    {
        public List<MediaItem> Movies { get; set; }
        public List<MediaItem> TVShows { get; set; }

        public MediaItems()
        {
            Movies = new List<MediaItem>();
            TVShows = new List<MediaItem>();
        }
    }
}
