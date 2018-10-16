using System;
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

        public MarkersController()
        {
           this.MarkersDapper = new DapperHelper<MarkersInformation>();

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
        public HttpResponseMessage Get(double lat, double lng)
        {
            double r = 6371;//地球半径千米
            double dis = 0.1;//0.5千米距离
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
                "select * from MarkersInformation where latitude>@minlat and latitude<@maxlat and longitude>@minlng and longitude<@maxlng";
           var list = MarkersDapper.Query(sql, new {minlat, maxlat, minlng, maxlng});
            var modelList = list.Select(o => new MarkersListModel{OpenId=o.OpenId, Latitude =Convert.ToDouble(o.Latitude),Longitude = Convert.ToDouble(o.Longitude) ,Id = o.Id});
            //序列化对象
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            string str = JsonConvert.SerializeObject(modelList, settings);
            var obj = JsonConvert.DeserializeObject(str);
            HttpResponseMessage result =
                Request.CreateResponse(HttpStatusCode.OK, new {status="OK",data= obj }, Configuration.Formatters.JsonFormatter);
            return result;
        }
        /// <summary>
        /// 根据id 获取信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
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
                File.Delete(HttpContext.Current.Server.MapPath(file));
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
            var modelList = MarkersDapper.Query("select * from (select row_number()over(order by id) as rownumber,* from MarkersInformation where openId=@openId and IsDel=0 and areaName like @search) a " +
                                        "  where rownumber  between @minnum and @maxNum", pageModel);
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
        /// 获取所有标点
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
            var modelList = MarkersDapper.Query("select * from (select row_number()over(order by id) as rownumber,* from MarkersInformation where  Id in (select markersId from [dbo].[Accept] where OpenId=@openId) and IsDel=0 and areaName like @search) a " +
                                        "  where rownumber  between @minnum and @maxNum", pageModel);
            var count = MarkersDapper.GetCount(" openId=@openId", new { openId = pageModel.OpenId });
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            string str = JsonConvert.SerializeObject(modelList, settings);
            var obj = JsonConvert.DeserializeObject(str);
            HttpResponseMessage result =
               Request.CreateResponse(HttpStatusCode.OK, new { status = "OK", data = new { modelList = obj, isMax = count < pageModel.MaxNum } }, Configuration.Formatters.JsonFormatter);
            return result;
        }

    }
}