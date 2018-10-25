using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ANT.MapInformation.WebAPI.Models
{
    public class MarkersModel
    {
        public string Id { get; set; }
        public string AreaName { get; set; }
        public string UserName { get; set; }
        public string CoverImage { get; set; }
        public string Remark { get; set; }
        public long AcceptNum { get; set; }
        public string CreateTime { get; set; }
        public int Status { get; set; }

    }
}