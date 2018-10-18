using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ANT.MapInformation.WebAPI.Models
{
    public class OpenIdModel
    {
        public string openid { get; set; }
        public string session_key { get; set; }
        public bool IsExist { get; set; }

    }
}