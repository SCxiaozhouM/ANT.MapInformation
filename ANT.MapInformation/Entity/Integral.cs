using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANT.MapInformation.Entity
{
    public class Integral:IBaseEntity
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// openId
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 积分数
        /// </summary>
        public int IntegralNum { get; set; }
        /// <summary>
        /// 标点名
        /// </summary>
        public string MarkersName { get; set; }
        /// <summary>
        /// 微信名
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 类型图片
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// 类型 0：采纳收益
        /// </summary>
        public int Type { get; set; }

    }
}
