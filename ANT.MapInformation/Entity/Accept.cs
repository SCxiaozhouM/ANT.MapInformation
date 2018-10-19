using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANT.MapInformation.Entity
{
    public class Accept:IBaseEntity
    {
        public long Id { get; set; }

        /// <summary>
        /// openid
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// InformationId
        /// </summary>
        public string MarkersId { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public int Type { get; set; }
    }
}
