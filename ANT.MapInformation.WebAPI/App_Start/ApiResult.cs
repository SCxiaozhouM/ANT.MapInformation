using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace ANT.MapInformation.WebAPI.App_Start
{
    /// <summary>
    /// api返回结果
    /// </summary>
    public class ApiResult
    {
        /// <summary>
        /// 状态
        /// </summary>
        public ApiStatus Status { get; set; }
        /// <summary>
        /// 返回数据
        /// </summary>
        public object Data { get; set; }
        /// <summary>
        /// 返回信息
        /// </summary>
        public string Msg { get; set; }
        /// <summary>
        /// 异常信息
        /// </summary>
        public string ErrorMsg { get; set; }
    }

}