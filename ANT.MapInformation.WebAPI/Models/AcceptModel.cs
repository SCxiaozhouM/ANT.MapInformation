using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ANT.MapInformation.WebAPI.Models
{
    /// <summary>
    /// 采纳
    /// </summary>
    public class AcceptModel
    {
        /// <summary>
        /// 采纳者openid
        /// </summary>
        [Required]
        public string OpenId { get; set; }
        /// <summary>
        /// 被采纳者openid
        /// </summary>
        [Required]
        public string InformationOpenId { get; set; }
        /// <summary>
        /// 地点id
        /// </summary>
        [Required]
        public long InformationId { get; set; }
        /// <summary>
        /// 类型 0：采纳
        /// </summary>
        [Required]
        public int Type { get; set; }

    }
}