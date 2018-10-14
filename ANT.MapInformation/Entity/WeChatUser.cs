using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANT.MapInformation.Entity
{
    /// <summary>
    /// 微信用户信息表
    /// </summary>
    public class WeChatUser: IBaseEntity
    {
        /// <summary>
        /// Id
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// openid
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        /// 微信昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 地区
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// UnionId
        /// </summary>
        public string UnionId { get; set; }

        /// <summary>
        /// 头像路径
        /// </summary>
        public string AvatarUrl { get; set; }
        /// <summary>
        /// 积分
        /// </summary>
        public int Integral { get; set; }
    }
}
