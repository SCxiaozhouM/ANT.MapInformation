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
using System.Web.Http;

namespace ANT.MapInformation.WebAPI.Controllers
{
    public class UserController : ApiController
    {
        public DapperHelper<User> UserDapper { get; set; }

        public UserController()
        {
            UserDapper = new DapperHelper<User>();
        }
       
    }
}
