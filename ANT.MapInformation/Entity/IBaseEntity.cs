using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANT.MapInformation.Entity
{
    public class IBaseEntity:IEntity
    {
      

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDel { get; set; } = false;

        /// <summary>
        /// 添加时间
        /// </summary>
        public DateTime CreateTime { get; set; }=DateTime.Now;
    }
}
