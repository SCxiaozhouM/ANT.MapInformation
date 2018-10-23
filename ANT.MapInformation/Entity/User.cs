using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANT.MapInformation.Entity
{
    public class User:IBaseEntity
    {
        public string Id { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 盐
        /// </summary>
        public string Salt { get; set; }
        /// <summary>
        /// 状态 0：可登陆 1：不可
        /// </summary>
        public int Status { get; set; }


    }
}
