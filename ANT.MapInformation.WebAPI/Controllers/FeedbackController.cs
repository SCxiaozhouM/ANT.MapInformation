using ANT.MapInformation.Dapper;
using ANT.MapInformation.Entity;
using ANT.MapInformation.WebAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("api/feedbackList")]
        public HttpResponseMessage FeedbackList([FromBody] PageModel pageModel)
        {
            if (ModelState.IsValid)
            {
                pageModel.PageSize = 5;

                //根据type判断是接收还是发送
                var t = pageModel.Type == "0" ? "SendOpenId" : "ReceiveOpenId";
                pageModel.Search = "%" + pageModel.Search + "%";
                var modelList = FeedbackDapper.Query("select * from (select row_number()over(order by id) as rownumber,* from feedback where "+ t + "=@openId  and IsDel=0 and status in ("+pageModel.State+")) a " +
                                        "  where rownumber  between @minnum and @maxNum", pageModel).OrderByDescending(o => o.CreateTime); ;
                var count = FeedbackDapper.GetCount(" SendOpenId=@openId", new { openId = pageModel.OpenId });
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                string str = JsonConvert.SerializeObject(modelList, settings);
                var obj = JsonConvert.DeserializeObject(str);
                HttpResponseMessage result =
                   Request.CreateResponse(HttpStatusCode.OK, new { status = "OK", data = new { modelList = obj, isMax = count < pageModel.MaxNum } }, Configuration.Formatters.JsonFormatter);
                return result;
            }
            return Request.CreateResponse(HttpStatusCode.OK, new { status = "error", errorMsg = "参数错误" }, Configuration.Formatters.JsonFormatter);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        [Route("api/FeedbackAction")]
        public HttpResponseMessage FeedbackAction([FromBody] JObject obj)
        {
            string status = "error";
            var data = -1;
            string Id = obj["Id"].ToString();
            string type = obj["type"].ToString();
            if(Id!=null)
            {
                status = "OK";
                //拒绝
                if (type == "reject")
                {
                    FeedbackDapper.Update("update feedback set Status=2 where Id=@Id", new Feedback { Id = Id });
                }
                //接受
                else if (type == "receive")
                {
                    var dic = new Dictionary<string, string>();
                    dic.Add("@feedbackId", Id);
                    data =Convert.ToInt32(FeedbackDapper.Transaction("receiveFeedback", dic));
                }
                //查看
                else if (type == "view")
                {
                    FeedbackDapper.Update("update feedback set Status=1 where Id=@Id", new Feedback { Id = Id });
                }
               
            }

            HttpResponseMessage result =
                     Request.CreateResponse(HttpStatusCode.OK, new {status,data }, Configuration.Formatters.JsonFormatter);
            return result;
        }

    }
}
