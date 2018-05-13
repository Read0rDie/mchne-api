using System;
using System.Collections.Generic;

namespace mchne_api.Models
{
    public class Novel
    {
        public long NovelID { get; set; }
        public string Title { get; set; }
        public string CoverImage { get; set; }
        //public ICollection<Genre> Genres { get; set; }

    }

    public enum Genre
    {
        Fantasy,
        SciFi,
        Xuanhuan,

    }
}