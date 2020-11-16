using System;
using System.Collections.Generic;
using System.Text;

namespace PlexMediaArchiver
{
    public class MovieViewModel
    {
        public Plex.Api.Models.Metadata MetaData { get; set; }
        public DateTime? LastViewed { get; set; }
    }
}
