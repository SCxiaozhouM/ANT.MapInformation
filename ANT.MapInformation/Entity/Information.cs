using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANT.MapInformation.Entity
{
    /// <summary>
    /// 情报表
    /// </summary>
    public class Information:IBaseEntity
    {
        /// <summary>
        /// openId
        /// </summary>
        public string OpenId { get; set; }

        /// <summary>
        ///纬度
        /// </summary>
        public string Latitude { get; set; }

        /// <summary>
        /// 经度
        /// </summary>
        public string Longitude { get; set; }

        /// <summary>
        /// 地区名
        /// </summary>
        public string AreaName { get; set; }

        /// <summary>
        /// 是否可以进入
        /// </summary>
        public bool IsGetIn { get; set; }

        /// <summary>
        /// 是否收费
        /// </summary>
        public bool IsCharges { get; set; }

        /// <summary>
        /// 是否地下停车场
        /// </summary>
        public bool IsUndergroundGarage { get; set; }
        /// <summary>
        /// 限高
        /// </summary>
        public string LimitHeight { get; set; }
        /// <summary>
        /// 收费标准
        /// </summary>
        public string ChargingStandard { get; set; }
        /// <summary>
        /// 距离
        /// </summary>
        public string Distance { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Remark { get; set; }

     
       

        /// <summary>
        /// 图片
        /// </summary>
        public string Images { get; set; }

        /// <summary>
        /// 采纳数
        /// </summary>
        public long AcceptNum { get; set; }

        /// <summary>
        /// 是否匿名
        /// </summary>
        public bool IsAnonymity { get; set; }

     

       
    }
}
