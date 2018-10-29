using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANT.MapInformation.Entity
{
    /// <summary>
    /// CarInfo:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    public partial class CarInfo : IBaseEntity
    {
        public CarInfo()
        { }
        public string Id
        {
            get; set;
        }
        /// <summary>
        /// openid
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 车牌号码
        /// </summary>
        public string FlapperNumber
        {
            get; set;
        }
        /// <summary>
        /// 品牌
        /// </summary>
        public string Brand
        {
            get; set;
        }
        /// <summary>
        /// 车辆类型
        /// </summary>
        public string CarType
        {
            get; set;
        }
        /// <summary>
        /// 使用燃料
        /// </summary>
        public string Fuel
        {
            get; set;
        }
        /// <summary>
        /// 使用性质
        /// </summary>
        public string Nature
        {
            get; set;
        }
        /// <summary>
        /// 长
        /// </summary>
        public string Height
        {
            get; set;
        }
        /// <summary>
        /// 宽
        /// </summary>
        public string Width
        {
            get; set;
        }
        /// <summary>
        /// 高
        /// </summary>
        public string Tall
        {
            get; set;
        }
        /// <summary>
        /// 载客量
        /// </summary>
        public string Busload
        {
            get; set;
        }
        /// <summary>
        /// 载重量
        /// </summary>
        public string LoadingCapacity
        {
            get; set;
        }

    }
}
