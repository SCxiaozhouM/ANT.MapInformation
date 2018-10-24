using ANT.MapInformation.Dapper;
using ANT.MapInformation.Entity;
using ANT.MapInformation.WebAPI.App_Start;
using ANT.MapInformation.WebAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Http;
using System.Web.SessionState;

namespace ANT.MapInformation.WebAPI.Controllers
{
    public class UserController : ApiController
    {

        public DapperHelper<User> UserDapper { get; set; }

        public UserController()
        {
            UserDapper = new DapperHelper<User>();
        }
        [HttpPost]
        public IHttpActionResult Login([FromBody]  LoginModel model)
        {
            if(ModelState.IsValid)
            {
                //登陆验证
               var userModel = UserDapper.Query("select * from UserInfo where userName=@userName and password=@password", model).FirstOrDefault();
                if(userModel!=null)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("grant_type", "password");
                    dic.Add("username",model.UserName);
                    dic.Add("password",model.Password);
                    var responseStr = CreatePostHttpResponse("http://"+Url.Request.Headers.Host+"/token", dic);
                    var obj = JsonConvert.DeserializeObject<JObject>(responseStr);
                    var cooike = new HttpCookie("name", obj["access_token"].ToString());
                    HttpContext.Current.Response.AppendCookie(cooike);

                    return Json(new { status = "OK", data = obj });
                }
            }
            return Json(new { status="error"});
        }
        [JsonNetActionFilter]
        [HttpGet]
        public IHttpActionResult LogOut(string name)
        {
            var cooike = new HttpCookie("name", null);
            return Json(new { status = "OK"});
        }
        /// <summary>
        /// 发送http post请求
        /// </summary>
        /// <param name="url">地址</param>
        /// <param name="parameters">查询参数集合</param>
        /// <returns></returns>
        public string CreatePostHttpResponse(string url, IDictionary<string, string> parameters)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;//创建请求对象
            request.Method = "POST";//请求方式
            request.ContentType = "application/x-www-form-urlencoded";//链接类型
                                                                      //构造查询字符串
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                bool first = true;
                foreach (string key in parameters.Keys)
                {

                    if (!first)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                        first = false;
                    }
                }
                byte[] data = Encoding.UTF8.GetBytes(buffer.ToString());
                //写入请求流
                using (Stream stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }
            }
            using (Stream s = request.GetResponse().GetResponseStream())
            {
                StreamReader reader = new StreamReader(s, Encoding.UTF8);
                return reader.ReadToEnd();
            }
        }
    }
}
