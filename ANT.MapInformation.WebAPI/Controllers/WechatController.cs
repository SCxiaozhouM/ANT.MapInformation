using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using ANT.MapInformation.Dapper;
using ANT.MapInformation.Entity;
using ANT.MapInformation.WebAPI.App_Start;
using ANT.MapInformation.WebAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ANT.MapInformation.WebAPI.Controllers
{
    public class WechatController : ApiController
    {
        /// <summary>
        /// appid
        /// </summary>
        public static string AppId = "wxb55e0389a2a5498f";

        /// <summary>
        /// secret
        /// </summary>
        public static string Secret = "3bacad0dd17162a0ba5146919a5839cb";
        


        /// <summary>
        /// wechatDapper对象
        /// </summary>
        public DapperHelper<WeChatUser> WechatDapper { get; set; }
        /// <summary>
        /// 积分Dapper对象
        /// </summary>
        public DapperHelper<Integral> IntegralDapper { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WechatController()
        {
            this.WechatDapper = new DapperHelper<WeChatUser>();
            IntegralDapper = new DapperHelper<Integral>();
        }

        // GET api/<controller>
        /// <summary>
        /// 获取openid
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public HttpResponseMessage GetOpenId(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { status = "error", error = "code参数错误" }, Configuration.Formatters.JsonFormatter);
            }
            var responseStr = GetHttpRequest(
                "https://api.weixin.qq.com/sns/jscode2session?appid=" + AppId + "&secret=" + Secret + "&js_code=" + code + "&grant_type=authorization_code",
                "GET");
            var obj = JsonConvert.DeserializeObject<OpenIdModel>(responseStr);
            //查询是否存在
                obj.IsExist = WechatDapper.GetCount(" openId=@openId ", obj) == 1;
            HttpResponseMessage result =
                Request.CreateResponse(HttpStatusCode.OK, new { status = "OK", data = obj }, Configuration.Formatters.JsonFormatter);
            return result;
        }
        /// <summary>
        /// post用户
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public HttpResponseMessage Post([FromBody]WeChatUser model)
        {
            var m = WechatDapper.Query("select * from WechatUser where openid=@openId", new { openId = model.OpenId }).FirstOrDefault();
            if(m==null)
            {
                WechatDapper.Add(model);
            }
            HttpResponseMessage result =
                Request.CreateResponse(HttpStatusCode.OK, new { status = "OK" }, Configuration.Formatters.JsonFormatter);
            return result;
        }

        [HttpPost]
        [Route("post/wechatList")]
        public IHttpActionResult Post([FromBody]NewPageModel pagemodel)
        {
            pagemodel.Search = "%" + pagemodel.Search + "%";
            var modelList = WechatDapper.Query("select * from (select row_number()over(order by id) as rownumber,* from WechatUser where  IsDel=0 ) a " +
                                        "  where rownumber  between @minnum and @maxNum", pagemodel).OrderByDescending(o => o.CreateTime);
            var count = WechatDapper.GetCount();
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

        /// <summary>
        /// 发送get请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string GetHttpRequest(string url,string data="")
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream myResponseStream = response.GetResponseStream();
            StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8"));
            string retString = myStreamReader.ReadToEnd();
            myStreamReader.Close();
            
            return retString;

        }

        /// <summary>
        /// 通过openId获取积分
        /// </summary>
        /// <param name="openId"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public HttpResponseMessage GetIntegral(string openId)
        {
            var model = WechatDapper.Query("select * from WeChatUser where openId=@openId", new { openId }).FirstOrDefault();
            var integral = -1;
            if(model!=null)
            {
                integral = model.Integral;
            }
            HttpResponseMessage result =
               Request.CreateResponse(HttpStatusCode.OK, new { status = "OK",data=new { integral ,carId= model.CarId} }, Configuration.Formatters.JsonFormatter);
            return result;
        }
        /// <summary>
        /// 通过openid获取列表信息
        /// </summary>
        /// <param name="openId"></param>
        /// <param name="pages"></param>
        /// <param name="search"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("api/IntegralList")]
        public HttpResponseMessage GetIntegralList([FromBody]PageModel pageModel )
        {

            if(ModelState.IsValid)
            {
                pageModel.Search = "%"+pageModel.Search + "%";
                var modelList = IntegralDapper.Query("select * from (select row_number()over(order by id) as rownumber,* from Integral where openId=@openId and type in ("+ pageModel.Type+ ") and markersName like @search) a " +
                                        "  where rownumber  between @minnum and @maxNum", pageModel).OrderByDescending(o => o.CreateTime); ;
                var count = IntegralDapper.GetCount(" openId=@openId",new { openId=pageModel.OpenId});
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                string str = JsonConvert.SerializeObject(modelList, settings);
                var obj = JsonConvert.DeserializeObject(str);
                HttpResponseMessage result =
                   Request.CreateResponse(HttpStatusCode.OK, new { status = "OK", data = new { modelList= obj, isMax= count< pageModel.MaxNum} }, Configuration.Formatters.JsonFormatter);
                return result;
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { status = "error", errorMsg="参数错误" }, Configuration.Formatters.JsonFormatter);
        }

        
    }
}