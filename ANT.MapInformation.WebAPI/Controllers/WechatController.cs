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
using Newtonsoft.Json;

namespace ANT.MapInformation.WebAPI.Controllers
{
    public class WechatController : ApiController
    {
        /// <summary>
        /// appid
        /// </summary>
        public static string AppId = "wx78315f71a2589d92";

        /// <summary>
        /// secret
        /// </summary>
        public static string Secret = "90f1ccbc2435d835544508bde9cde78c";



        public DapperHelper<WeChatUser> WechatDapper { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public WechatController()
        {
            this.WechatDapper = new DapperHelper<WeChatUser>();

        }

        // GET api/<controller>
        /// <summary>
        /// 获取openid
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/Wechat/getOpenId")]
        public HttpResponseMessage GetOpenId(string code)
        {
            if (string.IsNullOrEmpty(code))
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { status = "error", error="code参数错误" }, Configuration.Formatters.JsonFormatter);
                 
            }
            var responseStr = GetHttpRequest(
                "https://api.weixin.qq.com/sns/jscode2session?appid="+ AppId + "&secret="+ Secret + "&js_code="+ code + "&grant_type=authorization_code",
                "GET");
            var obj = JsonConvert.DeserializeObject(responseStr);
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
            if(model==null)
            {
                WechatDapper.Add(model);
            }
            HttpResponseMessage result =
                Request.CreateResponse(HttpStatusCode.OK, new { status = "OK" }, Configuration.Formatters.JsonFormatter);
            return result;
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
    }
}