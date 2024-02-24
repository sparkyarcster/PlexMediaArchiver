﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PlexMediaArchiver.Classes.Model
{
    public class MediaItem
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public DateTime? LastPlayed { get; set; }
    }
}
