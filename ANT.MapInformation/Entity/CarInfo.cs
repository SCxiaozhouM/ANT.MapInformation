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
        [Serializable]
        public partial class CarInfo:IBaseEntity
        {
            public CarInfo()
            { }
            #region Model
            private string _id;
            private string _flappernumber;
            private string _brand;
            private string _cartype;
            private string _fuel;
            private string _nature;
            private string _width;
            private string _busload;
            private string _loadingcapacity;
            /// <summary>
            /// 
            /// </summary>
            public string Id
            {
                set { _id = value; }
                get { return _id; }
            }
            /// <summary>
            /// 车牌号码
            /// </summary>
            public string FlapperNumber
            {
                set { _flappernumber = value; }
                get { return _flappernumber; }
            }
            /// <summary>
            /// 品牌
            /// </summary>
            public string Brand
            {
                set { _brand = value; }
                get { return _brand; }
            }
            /// <summary>
            /// 车辆类型
            /// </summary>
            public string CarType
            {
                set { _cartype = value; }
                get { return _cartype; }
            }
            /// <summary>
            /// 使用燃料
            /// </summary>
            public string Fuel
            {
                set { _fuel = value; }
                get { return _fuel; }
            }
            /// <summary>
            /// 使用性质
            /// </summary>
            public string Nature
            {
                set { _nature = value; }
                get { return _nature; }
            }
            /// <summary>
            /// 宽
            /// </summary>
            public string WidthHeight
        {
                set { _width = value; }
                get { return _width; }
            }
            /// <summary>
            /// 载客量
            /// </summary>
            public string Busload
            {
                set { _busload = value; }
                get { return _busload; }
            }
            /// <summary>
            /// 载重量
            /// </summary>
            public string LoadingCapacity
            {
                set { _loadingcapacity = value; }
                get { return _loadingcapacity; }
            }
            #endregion Model

        }
}
