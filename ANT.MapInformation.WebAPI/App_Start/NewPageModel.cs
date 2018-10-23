using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ANT.MapInformation.WebAPI.App_Start
{
    public class NewPageModel
    {
        public int Start { get; set; }
        public int Length { get; set; }
        public string Search { get; set; }
        public int PageCount { get; set; }
        /// <summary>
        /// 最大
        /// </summary>
        public int MaxNum
        {
            get { return Start + Length; }
            set { this.MaxNum = value; }
        }

        /// <summary>
        /// 最小
        /// </summary>
        public int MinNum
        {
            get { return Start + 1; }
            set { this.MinNum = value; }
        }

    }
}