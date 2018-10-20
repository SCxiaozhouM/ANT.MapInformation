using ANT.MapInformation.Dapper;
using ANT.MapInformation.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace ANT.MapInformation.WebAPI.Controllers
{
    public class CarInfoController : ApiController
    {
        public CarInfoController()
        {

        }
        public DapperHelper<CarInfo> CarInfoDappler { get; set; }

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
            HttpResponseMessage result =
                  Request.CreateResponse(HttpStatusCode.OK, new { status = "OK", data }, Configuration.Formatters.JsonFormatter);
            return result;
        }
        /// <summary>
        /// 提交车辆信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public HttpResponseMessage Post([FromBody] CarInfo model)
        {
            model.Id = Guid.NewGuid().ToString();
            var count = CarInfoDappler.Add(model);
            HttpResponseMessage result =
                  Request.CreateResponse(HttpStatusCode.OK, new { status = "OK", data = count == 1 }, Configuration.Formatters.JsonFormatter);
            return result;
        }

    }
}
