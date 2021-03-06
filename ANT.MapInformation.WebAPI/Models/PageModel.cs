﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ANT.MapInformation.WebAPI.Models
{
    /// <summary>
    /// 分页类
    /// </summary>
    public class PageModel
    {
        /// <summary>
        /// 每页多少条数据 默认25
        /// </summary>
        public int PageSize { get; set; } = 10;

        /// <summary>
        /// 当前页数 默认第一页
        /// </summary>
        public int PageCount { get; set; } = 1;

        /// <summary>
        /// 筛选条件
        /// </summary>
        public string Search { get; set; } = "";

        /// <summary>
        /// 筛选类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 最大
        /// </summary>
        public int MaxNum {
            get { return PageSize * PageCount; }
            set { this.MaxNum =  value; }
        }

        /// <summary>
        /// 最小
        /// </summary>
        public int MinNum {
            get{ return (PageCount - 1) * PageSize + 1; }
            set { this.MinNum = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        
        public string  OpenId { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string State { get; set; } = "0,1,2,3";
    }
}