using ANT.MapInformation.Dapper;
using ANT.MapInformation.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ANT.MapInformation.WebAPI.Controllers
{
    /// <summary>
    /// 反馈
    /// </summary>
    public class FeedbackController : ApiController
    {
        public DapperHelper<Feedback> FeedbackDapper { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public FeedbackController()
        {
            this.FeedbackDapper = new DapperHelper<Feedback>();
        }

        /// <summary>
        /// 提交反馈
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public HttpResponseMessage Post([FromBody] Feedback model)
        {
            string status = "error";
            if(ModelState.IsValid)
            {
                status = "OK";
                if(model.Id==null)
                {
                    model.Id = Guid.NewGuid().ToString();
                    FeedbackDapper.Add(model);
                }
                else
                {
                    FeedbackDapper.Update(model);
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { status = status ,data= model.Id },Configuration.Formatters.JsonFormatter);
        }
        /// <summary>
        /// 反馈查询
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public HttpResponseMessage Get(string MarkersId, string sendOpenId)
        {
            var model = FeedbackDapper.Query("select * from Feedback where MarkersId=@MarkersId and sendOpenId=@sendOpenId",new {
                MarkersId,sendOpenId
            }).FirstOrDefault();
            //序列化对象
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            string str = JsonConvert.SerializeObject(model, settings);
            var obj = JsonConvert.DeserializeObject(str);
            return Request.CreateResponse(HttpStatusCode.OK, new { status = "OK",data=obj }, Configuration.Formatters.JsonFormatter);
        }
    }
}
