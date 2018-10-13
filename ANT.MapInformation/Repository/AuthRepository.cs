using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using ANT.MapInformation.Dapper;
using ANT.MapInformation.Entity;

namespace ANT.MapInformation.Repository
{
    public class AuthRepository:IDisposable
    {
        /// <summary>
        /// 身份验证
        /// </summary>
        /// <returns></returns>
        public async Task<bool> Authentication(string parameter)
        {
            return await Task.Run<bool>(() => { return parameter == "ant"; });
        }

        public void Dispose()
        {
        }
    }
}
