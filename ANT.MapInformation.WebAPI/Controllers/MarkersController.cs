﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;
using ANT.MapInformation.Dapper;
using ANT.MapInformation.Entity;
using ANT.MapInformation.WebAPI.App_Start;
using ANT.MapInformation.WebAPI.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace ANT.MapInformation.WebAPI.Controllers
{
    public class MarkersController : ApiController
    {


        public DapperHelper<MarkersInformation> MarkersDapper { get; set; }
        public DapperHelper<Accept> AcceptDapper { get; set; }
        public DapperHelper<WeChatUser> WechatUserDpper { get; set; }
        public MarkersController()
        {
           this.MarkersDapper = new DapperHelper<MarkersInformation>();
            AcceptDapper = new DapperHelper<Accept>();
            WechatUserDpper = new DapperHelper<WeChatUser>();
        }

        // GET api/<controller>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lng"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public HttpResponseMessage Get(double lat, double lng,int scope)
        {
            double r = 6371;//地球半径千米
            double dis = scope / 1000.0;//0.5千米距离
            double dlng = 2 * Math.Asin(Math.Sin(dis / (2 * r)) / Math.Cos(lat * Math.PI / 180));
            dlng = dlng * 180 / Math.PI;
            double dlat = dis / r;
            dlat = dlat * 180 / Math.PI;
            double minlat = lat - dlat;
            double maxlat = lat + dlat;
            double minlng = lng - dlng;
            double maxlng = lng + dlng;
            //dapper对象
            string sql =
                "select * from MarkersInformation where latitude>@minlat and latitude<@maxlat and longitude>@minlng and longitude<@maxlng and isdel=0 and status=0";
           var list = MarkersDapper.Query(sql, new {minlat, maxlat, minlng, maxlng});
            var modelList = list.Select(o => new MarkersListModel{OpenId=o.OpenId, Latitude =Convert.ToDouble(o.Latitude),Longitude = Convert.ToDouble(o.Longitude) ,Id = o.Id});
            //序列化对象
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            string str = JsonConvert.SerializeObject(modelList, settings);
            var obj = JsonConvert.DeserializeObject(str);
            HttpResponseMessage result =
                Request.CreateResponse(HttpStatusCode.OK, new { status = "OK", data = obj }, Configuration.Formatters.JsonFormatter);
            return result;
        }



        /// <summary>
        /// 根据id 获取信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        //[Authorize]
        public HttpResponseMessage Get(string Id)
        {
            var model  = MarkersDapper.QueryById(Id);
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            string str = JsonConvert.SerializeObject(model, settings);
            var obj = JsonConvert.DeserializeObject(str);
            HttpResponseMessage result =
                Request.CreateResponse(HttpStatusCode.OK, new { status = "OK", data = obj }, Configuration.Formatters.JsonFormatter);
            return result;
        }


        /// <summary>
        /// 提交数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("post/Markers")]//68295134
        public HttpResponseMessage Post([FromBody]MarkersInformation model)
        {
            model.Images = model.Images.TrimEnd(',');
            var imgs = model.Images.Split(new char[]{ ',' }, StringSplitOptions.RemoveEmptyEntries);
            if(imgs.Length>0)
            {
                model.CoverImage = imgs[0];
            }
            model.Id = Guid.NewGuid().ToString();
            MarkersDapper.Add(model);
            HttpResponseMessage result =
                Request.CreateResponse(HttpStatusCode.OK, new { status = "OK" }, Configuration.Formatters.JsonFormatter);
            return result;
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Upload/Markers")]
        public HttpResponseMessage UploadImages()
        {
            HttpFileCollection files = HttpContext.Current.Request.Files;
            string path = "";
            //文件夹
            var directory = "/imgcoll/";
            Directory.CreateDirectory(directory);
            foreach (string key in files.AllKeys)
            {
                HttpPostedFile file = files[key];//file.ContentLength文件长度
                var fileA = file.FileName.Split('.');
                //fileName
                TimeSpan ts = DateTime.Now.ToUniversalTime() - new DateTime(1970, 1, 1);
                var fileName = ts.TotalMilliseconds + "." + fileA[fileA.Length - 1];
                path = directory + fileName;
                if (string.IsNullOrEmpty(file.FileName) == false)
                    file.SaveAs(HttpContext.Current.Server.MapPath(directory) + fileName);
            }
            HttpResponseMessage result =
                Request.CreateResponse(HttpStatusCode.OK, new { status = "OK",msg= path}, Configuration.Formatters.JsonFormatter);
            return result;
        }
        /// <summary>
        /// 删除图片
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Delete/Imgs")]
        public HttpResponseMessage DeleteImg(string url)
        {
            string[] files = url.Split(new char[]{ ',' },StringSplitOptions.RemoveEmptyEntries);
            foreach (var file in files)
            {
                var filea = file.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                var path =  "/" + filea[filea.Length - 2] + "/" + filea[filea.Length - 1];
                File.Delete(HttpContext.Current.Server.MapPath(path));
            }
            HttpResponseMessage result =
                Request.CreateResponse(HttpStatusCode.OK, new { status = "OK" }, Configuration.Formatters.JsonFormatter);
            return result;
        }
        /// <summary>
        /// 采纳
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("Post/Accept")]
        public HttpResponseMessage PostAccept([FromBody]AcceptModel model)
        {
            if(!ModelState.IsValid)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { status = "error", errorMsg="参数错误" }, Configuration.Formatters.JsonFormatter);                 
            }
            //先查询是否已采纳
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("@openId", model.OpenId);
            dic.Add("@MarkersOpenId", model.MarkersOpenId);
            dic.Add("@MarkersId", model.MarkersId.ToString());
            dic.Add("@type",model.Type.ToString());
            dic.Add("@image", "../../img/jifen.png");
            var count = MarkersDapper.Transaction("accept_C", dic);
            HttpResponseMessage result =
                Request.CreateResponse(HttpStatusCode.OK, new { status = "OK",data= count }, Configuration.Formatters.JsonFormatter);
            return result;
        }

        /// <summary>
        /// 获取所有标点
        /// </summary>
        /// <param name="OpenId"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("Markers/List")]
        public HttpResponseMessage MarkersList([FromBody]PageModel pageModel)
        {
            pageModel.PageSize = 6;

            pageModel.Search = "%" + pageModel.Search + "%";
            var modelList = MarkersDapper.Query("select * from (select row_number()over(order by id) as rownumber,* from MarkersInformation where openId=@openId and IsDel=0 and areaName like @search and status in (" + pageModel.State + ")) a " +
                                        "  where rownumber  between @minnum and @maxNum", pageModel).OrderByDescending(o => o.CreateTime); ;
            var count = MarkersDapper.GetCount(" openId=@openId",new { openId=pageModel.OpenId});
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            string str = JsonConvert.SerializeObject(modelList, settings);
            var obj = JsonConvert.DeserializeObject(str);
            HttpResponseMessage result =
               Request.CreateResponse(HttpStatusCode.OK, new { status = "OK", data = new { modelList = obj, isMax = count < pageModel.MaxNum } }, Configuration.Formatters.JsonFormatter);
            return result;
        }
        /// <summary>
        /// 获取采纳的所有标点
        /// </summary>
        /// <param name="OpenId"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [Route("Accept/List")]
        public HttpResponseMessage AcceptList([FromBody]PageModel pageModel)
        {
            pageModel.PageSize = 6;
            pageModel.Search = "%" + pageModel.Search + "%";
            var modelList = MarkersDapper.Query("select * from (select row_number()over(order by id) as rownumber,* from MarkersInformation where  Id in (select markersId from [dbo].[Accept] where OpenId=@openId and IsDel=0) and IsDel=0 and areaName like @search) a " +
                                        "  where rownumber  between @minnum and @maxNum", pageModel).OrderByDescending(o=>o.CreateTime);
            var count = MarkersDapper.GetCount(" openId=@openId", new { openId = pageModel.OpenId });
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            string str = JsonConvert.SerializeObject(modelList, settings);
            var obj = JsonConvert.DeserializeObject(str);
            HttpResponseMessage result =
               Request.CreateResponse(HttpStatusCode.OK, new { status = "OK", data = new { modelList = obj, isMax = count < pageModel.MaxNum } }, Configuration.Formatters.JsonFormatter);
            return result;
        }


        /// <summary>
        /// 更改标点信息
        /// </summary>
        /// <returns></returns>
        [Route("Update/Markers")]
        public HttpResponseMessage UpdateMarkers([FromBody] MarkersInformation model)
        {
            //查询坐标是否存在

            var markers = MarkersDapper.Query("select * from markersInformation where id=@id", new { id = model.Id }).FirstOrDefault();
            if(markers==null)
            {
                return Request.CreateResponse(HttpStatusCode.OK, new { status = "error" }, Configuration.Formatters.JsonFormatter);
            }
            model.Images = model.Images.Trim(',');
            var imgs = model.Images.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < imgs.Length; i++)
            {
                var imga = imgs[i].Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
                imgs[i] = "/"+imga[imga.Length-2]+ "/" + imga[imga.Length - 1];
            }
            model.Images = string.Join(",", imgs);
            if (imgs.Length > 0)
            {
                model.CoverImage = imgs[0];
            }
            //如果时异常状态，更改后偶则为审核状态
            if(markers.Status==1)
            {
                model.Status = 2;
            }
            MarkersDapper.Update(model);
            HttpResponseMessage result =
                Request.CreateResponse(HttpStatusCode.OK, new { status = "OK" }, Configuration.Formatters.JsonFormatter);
            return result;
        }
        /// <summary>
        /// 删除标点
        /// </summary>
        /// <param name="markerId"></param>
        /// <returns></returns>
        [Route("Delete/markers")]
        [HttpPost]
        public HttpResponseMessage DelMarkers([FromBody]MarkersInformation model)
        {
            var count = MarkersDapper.Update("update markersInformation set isdel=1 where id=@Id",model);
            HttpResponseMessage result =
                Request.CreateResponse(HttpStatusCode.OK, new { status = "OK" , data = count == 1 }, Configuration.Formatters.JsonFormatter);
            return result;
        }
        [HttpPost]
        [Authorize]
        [Route("Delete/Accept")]

        public HttpResponseMessage DelAccept([FromBody] Accept model)
        {

            var count = AcceptDapper.Update("update Accept set isdel=1 where markersId=@markersId and openId=@openId", model);
            HttpResponseMessage result =
                Request.CreateResponse(HttpStatusCode.OK, new { status = "OK",data=count==1 }, Configuration.Formatters.JsonFormatter);
            return result;
        }



        /// <summary>
        /// 获取所有标点信息
        /// </summary>
        /// <param name="OpenId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Markers/ListInfo")]
        [JsonNetActionFilter]
        public IHttpActionResult MarkersListInfo([FromBody]JObject objModel)
        {
            NewPageModel pagemodel   = new NewPageModel();
            pagemodel.Length = Convert.ToInt32(objModel["length"]);
            pagemodel.PageCount = Convert.ToInt32(objModel["pageCount"]);
            pagemodel.Search = "%" + objModel["search"].First.First.ToString() + "%";
            pagemodel.Start = Convert.ToInt32(objModel["start"]);
            var modelList = MarkersDapper.Query("select * from (select row_number()over(order by id) as rownumber,* from MarkersInformation where  IsDel=0 and areaName like @search) a " +
                                        "  where rownumber  between @minnum and @maxNum", pagemodel).OrderByDescending(o => o.CreateTime).Select(o=>new MarkersModel {
                                            AcceptNum = o.AcceptNum, CoverImage=o.CoverImage,Status=o.Status,
                                         Id=o.Id, AreaName=o.AreaName, CreateTime=o.CreateTime.ToString("yyyy-MM-dd"), Remark=o.Remark, UserName= WechatUserDpper.Query("select NickName from  WechatUser where openId='"+o.OpenId+"'").FirstOrDefault().NickName
                                        });
            var count = MarkersDapper.GetCount(" isdel=0");
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