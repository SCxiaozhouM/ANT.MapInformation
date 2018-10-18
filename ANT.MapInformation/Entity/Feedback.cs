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
        /// 标点名
        /// </summary>
        public string MarkersName { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        /// 状态 0：待查看 1：已查看 2：未接受 3：已接受
        /// </summary>
        public int Status { get; set; }


        #region 扩展类
        /// <summary>
        /// 时间
        /// </summary>
        public string DateTime {
            get {
                return this.CreateTime.ToString("yyyy年MM月dd日");
            }
        }

        #endregion
    }
}
