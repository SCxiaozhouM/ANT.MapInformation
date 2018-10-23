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

        public string Gender { get; set; }
        public string PhoneNum { get; set; } = "";

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
        /// <summary>
        ///汽车信息id
        /// </summary>
        public string CarId { get; set; }
        /// <summary>
        /// 当前状态  信息是否完善..
        /// </summary>
        public bool Status { get; set; }
    }
}
