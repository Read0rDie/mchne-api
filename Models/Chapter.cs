using System;
using System.Collections.Generic;

namespace mchne_api.Models
{
    public class Chapter
    {
        public long ChapterID { get; set; }
        public long NovelID { get; set; }
        public long ChapterNumber{ get; set; }
        public string ContentUrl { get; set; }
        public Novel Novel { get; set; }
    }
    
}