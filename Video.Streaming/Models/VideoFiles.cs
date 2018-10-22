using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Video.Streaming.Models
{
    public class VideoFiles
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Key { get; set; }
        public string Description { get; set; }
        public Nullable<int> FileSize { get; set; }
        public string FilePath { get; set; }
    }
}