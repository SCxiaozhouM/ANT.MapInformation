using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANT.MapInformation.Entity
{
    /// <summary>
    /// 意见反馈
    /// </summary>
    public class Feedback:IBaseEntity
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 发送者openid
        /// </summary>
        public string SendOpenId { get; set; }
        /// <summary>
        /// 接收者oepnid
        /// </summary>
        public string ReceiveOpenId { get; set; }
        /// <summary>
        /// 坐标id
        /// </summary>
        public string MarkersId { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        /// 状态 0：待接受 1：已接收
        /// </summary>
        public int Status { get; set; }
        
    }
}
