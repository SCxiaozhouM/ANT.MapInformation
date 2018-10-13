using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANT.MapInformation.Entity;

namespace ANT.MapInformation.Dapper
{
    public class DapperHelper<T> where T: IEntity
    {
        //连接字符串
        public static readonly string connectionString = ConfigurationManager.AppSettings["connectionString"];
        #region 单列
        private static DapperHelper<T> _instance;
        public DapperHelper<T> GetInstance()
        {
            if (_instance == null)
            {
                _instance = new DapperHelper<T>();
            }
            return _instance;
        }
        #endregion

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="model">对象</param>
        /// <param name="transaction">事务对象</param>
        /// <returns></returns>
        public int Add(T model, IDbTransaction transaction = null)
        {
            int id = 0;
            StringBuilder sb = new StringBuilder();
            sb.Append(" insert into ");
            sb.Append(model.GetType().Name);
            sb.Append(" values ( ");
            foreach (var property in model.GetType().GetProperties())
            {
                if (property.Name == "Id")
                {
                    continue;
                }
                sb.Append("@" + property.Name + ",");
            }

           
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                id = connection.Execute(sb.ToString().TrimEnd(',')+")", model, transaction);
            }
            return id;
        }
        /// <summary>
        /// 批量添加
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="models">对象集合</param>
        /// /// <param name="transaction">事务对象</param>
        /// <returns></returns>
        public int AddBatch(string sql, IEnumerable<T> models, IDbTransaction transaction = null)
        {
            
            int id = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                id = connection.Execute(sql, models, transaction);
            }
            return id;
        }

        /// <summary>
        /// 删除（逻辑删除）
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="strWhere"></param>
        /// <param name="model"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public int Delete(string tableName, string strWhere, T model, IDbTransaction transaction = null)
        {
            int id = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "update " + tableName + " set IsDel=1 where " + strWhere;
                id = connection.Execute(sql, model, transaction);
            }
            return id;
        }
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="model">对象</param>
        /// <param name="transaction">事务对象</param>
        /// <returns></returns>
        public int Update(string sql, T model, IDbTransaction transaction = null)
        {
            int id = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                id = connection.Execute(sql, model, transaction);
            }
            return id;
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="param"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public IEnumerable<T> Query(string sql,object param=null, IDbTransaction transaction = null)
        {
            IEnumerable<T> models = new List<T>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                models = connection.Query<T>(sql, param, transaction);
            }

            return models;
        }

        /// <summary>
        /// 根据id 查询
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public T QueryById(long Id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "select * from " + typeof(T).Name + " where Id=@Id";
                var obj = new {Id};
                T model = connection.Query<T>(sql, obj).FirstOrDefault();
                return model;
            }
        }

        /// <summary>
        /// 获取最新id
        /// </summary>
        /// <returns></returns>
        public long GetNewId()
        {
            //
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string sql = "SELECT IDENT_CURRENT('" + typeof(T).Name +"') as newId " ;
                long newid = connection.Query(sql).FirstOrDefault();
                return newid;
            }
        }

        /// <summary>
        /// 存储过程
        /// </summary>
        /// <param name="name"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public object Transaction(string name, Dictionary<string,string> dic)
        {
            DynamicParameters param = new DynamicParameters();
            foreach (var item in dic)
            {
                param.Add(item.Key, item.Value);
            }
            param.Add("@count", "", DbType.Int32, ParameterDirection.Output);
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Execute(name, param, null, null, CommandType.StoredProcedure);
                return param.Get<object>("@count");
            }
        }
    }
}
