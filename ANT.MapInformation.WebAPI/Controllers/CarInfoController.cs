using ANT.MapInformation.Dapper;
using ANT.MapInformation.Entity;
using ANT.MapInformation.WebAPI.App_Start;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Web.Http;

namespace ANT.MapInformation.WebAPI.Controllers
{
    public class CarInfoController : ApiController
    {
        public CarInfoController()
        {
            CarInfoDappler = new DapperHelper<CarInfo>();
            WechatDappler = new DapperHelper<WeChatUser>();
        }
        public DapperHelper<CarInfo> CarInfoDappler { get; set; }
        public DapperHelper<WeChatUser> WechatDappler { get; set; }

        /// <summary>
        /// 获取车辆信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public HttpResponseMessage Get(string Id)
        {
            var data = CarInfoDappler.QueryById(Id);
            //序列化对象
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            string str = JsonConvert.SerializeObject(data, settings);
            var obj = JsonConvert.DeserializeObject(str);
            HttpResponseMessage result =
                  Request.CreateResponse(HttpStatusCode.OK, new { status = "OK", data=obj }, Configuration.Formatters.JsonFormatter);
            return result;
        }
        /// <summary>
        /// 提交车辆信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public HttpResponseMessage Post([FromBody] CarInfo model)
        {
           
            var wemodel = new WeChatUser() { CarId = model.Id, OpenId = model.OpenId };

            var count = 0;
            if(string.IsNullOrEmpty(model.Id))
            {
                model.Id = Guid.NewGuid().ToString();
                wemodel.CarId = model.Id;
                count = CarInfoDappler.Add(model);
                //添加完后 用户绑定信息id
                if(count==1)
                {
                    WechatDappler.Update("update wechatUser set carId = @carId where openId=@openId",wemodel);
                }
            }
            else
            {
                count = CarInfoDappler.Update(model);
            }
            //判断是否所以信息都已填写
            Type t = model.GetType();
            PropertyInfo[] PropertyList = t.GetProperties();
            var b = true ;
            foreach (PropertyInfo item in PropertyList)
            {
                string name = item.Name;
                object value = item.GetValue(model);
                if (value!=null)
                {
                    if(value.ToString()!="")
                    {
                        continue;
                    }
                }
                b = false;
                break;
            }
            if(b)
            {
                WechatDappler.Update("update wechatUser set status = 1 where openId=@openId", wemodel);
            }
            HttpResponseMessage result =
                  Request.CreateResponse(HttpStatusCode.OK, new { status = "OK", data = count == 1 }, Configuration.Formatters.JsonFormatter);
            return result; 
        }

        [HttpPost]
        [Route("post/carList")]
        public IHttpActionResult Post([FromBody]NewPageModel pagemodel)
        {
            pagemodel.Search = "%" + pagemodel.Search + "%";
            var modelList = CarInfoDappler.Query("select * from (select row_number()over(order by id) as rownumber,* from WechatUser where  IsDel=0 ) a " +
                                        "  where rownumber  between @minnum and @maxNum", pagemodel).OrderByDescending(o => o.CreateTime);
            var count = CarInfoDappler.GetCount();
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            string str = JsonConvert.SerializeObject(modelList, settings);
            var obj = JsonConvert.DeserializeObject(str);
            //返回参数集合
            Dictionary<string, object> map = new Dictionary<string, object>();
            map.Add("iTotalRecords", pagemodel.Start);
            map.Add("iTotalDisplayRecords", count);//总数据个数
            map.Add("aData", obj);



            return Json(map);
        }

    }
}
