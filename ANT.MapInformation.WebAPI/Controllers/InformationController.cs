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
    public class InformationController : ApiController
    {


        public DapperHelper<Information> InformationDapper { get; set; }

        public InformationController()
        {
           this.InformationDapper = new DapperHelper<Information>();

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
                "select * from Information where latitude>@minlat and latitude<@maxlat and longitude>@minlng and longitude<@maxlng";
           var list = InformationDapper.Query(sql, new {minlat, maxlat, minlng, maxlng});
            var modelList = list.Select(o => new InformationListModel{Latitude =Convert.ToDouble(o.Latitude),Longitude = Convert.ToDouble(o.Longitude) ,Id = o.Id});
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
        public HttpResponseMessage Get(long Id)
        {
            var model  = InformationDapper.QueryById(Id);
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
        [Route("post/Information")]//68295134
        public HttpResponseMessage Post([FromBody]Information model)
        {
            model.Images = model.Images.TrimEnd(',');
            InformationDapper.Add(model);
            HttpResponseMessage result =
                Request.CreateResponse(HttpStatusCode.OK, new { status = "OK" }, Configuration.Formatters.JsonFormatter);
            return result;
        }

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("Upload/Information")]
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
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("@openId", model.OpenId);
            dic.Add("@InformationOpenId", model.InformationOpenId);
            dic.Add("@InformationId", model.InformationId.ToString());
            dic.Add("@type",model.Type.ToString());
            var count = InformationDapper.Transaction("accept", dic);
            HttpResponseMessage result =
                Request.CreateResponse(HttpStatusCode.OK, new { status = "OK",data= count }, Configuration.Formatters.JsonFormatter);
            return result;
        }

    }
}