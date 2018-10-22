using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Video.Streaming.Models
{
    public class AssignVideoModel
    {
        [Required(ErrorMessage = "Select the user to assign.")]
        public string UserId { get; set; }
        public string VideoIds { get; set; }
        public SelectList UserList { get; set; }
        public SelectList VideoList { get; set; }
        [Required(ErrorMessage = "Select the videos to assign.")]
        public SelectList AssignedVideoList { get; set; }
    }


    public class Videos
    {
        public string UserId { get; set; }
        public Nullable<long> VideoId { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
        public string Key { get; set; }
        public string Name { get; set; }
        public string Title { get; set; }
    }
}