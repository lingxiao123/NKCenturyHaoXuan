using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.Common;
using System.Collections;
using Microsoft.Practices.EnterpriseLibrary.Data;
namespace DBUtility
{
    public class MainDBAccess
    {
        private static string defaultDBString = "MainConnection";

        private static int commandTimeout = 600;

        static MainDBAccess()
        {
            //string strTimeOut = ConfigurationSettings.AppSettings["CommandTimeout"];
            string strTimeOut = "60";

            if (strTimeOut != null && strTimeOut != "")
                commandTimeout = int.Parse(strTimeOut);
        }

        #region 基础属性
        /// <summary>
        /// 默认数据库连接字符变更入口
        /// </summary>
        public static string DefaultDBString
        {
            get
            {
                return defaultDBString;
            }
            set
            {
                defaultDBString = value;
            }
        }


        #endregion

        #region 判断数据库是否可连接
        /// <summary>
        /// 判断数据库是否可连接，使用默认连接字符
        /// </summary>
        /// <returns>Success:True; Fail:False</returns>
        public static bool CanConnectDB()
        {
            return CanConnectDB(defaultDBString);
        }

        /// <summary>
        /// 判断数据库是否可连接
        /// </summary>
        /// <param name="strConnString">数据库连接字符串名</param>
        /// <returns>Success:True; Fail:False</returns>
        public static bool CanConnectDB(string strDBString)
        {
            //strDBString = string.IsNullOrEmpty(strDBString) ? defaultDBString : strDBString;
            bool blCanConn = false;

            Database db = DatabaseFactory.CreateDatabase(strDBString);
            DbConnection dbConn = db.CreateConnection();

            try
            {
                dbConn.Open();
                blCanConn = true;
            }
            catch { }
            finally
            {
                if (dbConn.State != ConnectionState.Closed)
                {
                    dbConn.Close();
                    dbConn.Dispose();
                }
            }

            return blCanConn;
        }
        #endregion

        #region Sql文，不带事务提交
        /// <summary>
        /// 单条语句执行
        /// </summary>
        /// <param name="strSql">单条Sql文</param>
        /// <returns>>=0: record affected;-1: fail</returns>
        public static int ExecSql(string strSql)
        {
            return ExecSql(defaultDBString, strSql, null);
        }

        /// <summary>
        /// 批量执行
        /// </summary>
        /// <param name="strSqls">Sql文数组</param>
        /// <returns>>=0: record affected;-1: fail</returns>
        public static int ExecSql(string[] strSqls)
        {
            string strSql = string.Join(";", strSqls);
            return ExecSql(defaultDBString, strSql, null);
        }

        /// <summary>
        /// 单条语句执行，并且带参数
        /// </summary>
        /// <param name="strSql">单条Sql文</param>
        /// <param name="sqlParams">参数数组</param>
        /// <returns>>=0: record affected;-1: fail</returns>
        public static int ExecSql(string strSql, params SqlParameter[] sqlParams)
        {
            return ExecSql(defaultDBString, strSql, sqlParams);
        }

        /// <summary>
        /// 单据语句执行，指定数据库链接
        /// </summary>
        /// <param name="dbName">数据库链接语句</param>
        /// <param name="strSql">单条Sql文</param>
        /// <returns>>=0: record affected;-1: fail</returns>
        public static int ExecSql(string dbName, string strSql)
        {
            return ExecSql(dbName, strSql, null);
        }

        /// <summary>
        /// 执行数据库交互
        /// </summary>
        /// <param name="strSql">Sql文，多条使用';'分号隔开</param>
        /// <param name="sqlParamValues">参数数组</param>
        /// <returns>>=0: record affected;-1: fail</returns>
        public static int ExecSql(string dbName, string strSql, params SqlParameter[] sqlParams)
        {
            Database db = DatabaseFactory.CreateDatabase(dbName);
            DbCommand dbCommand = db.GetSqlStringCommand(strSql);
            dbCommand.CommandTimeout = commandTimeout;

            if (sqlParams != null && sqlParams.Length > 0)
            {
                for (int i = 0; i < sqlParams.Length; i++)
                    dbCommand.Parameters.Add(sqlParams[i]);
            }

            int nRes = 0;
            try
            {
                nRes = db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception ex)
            {
                nRes = -1;
                throw ex;
            }

            return nRes;

        }
        #endregion

        #region Sql文，事务提交
        /// <summary>
        /// Sql文，事务提交
        /// </summary>
        /// <param name="strSql">Sql文，多条使用";"分号隔开</param>
        /// <returns>>=0: record affected;-1: fail</returns>
        public static int ExecTransSql(string strSql)
        {
            return ExecTransSql(strSql, null);
        }

        /// <summary>
        /// Sql文，事务提交
        /// </summary>
        /// <param name="strSql">Sql文，多条使用";"分号隔开</param>
        /// <param name="sqlParamValues">参数数组</param>
        /// <returns>>=0: record affected;-1: fail</returns>
        public static int ExecTransSql(string strSql, params SqlParameter[] sqlParams)
        {
            Database db = DatabaseFactory.CreateDatabase(defaultDBString);
            DbCommand dbCommand = db.GetSqlStringCommand(strSql);
            dbCommand.CommandTimeout = commandTimeout;

            if (sqlParams != null && sqlParams.Length > 0)
            {
                for (int i = 0; i < sqlParams.Length; i++)
                    dbCommand.Parameters.Add(sqlParams[i]);
            }

            DbConnection conn = db.CreateConnection();
            DbTransaction dbTrans = null;
            int nRes = 0;
            try
            {
                conn.Open();
                dbTrans = conn.BeginTransaction();
                nRes = db.ExecuteNonQuery(dbCommand, dbTrans);
                dbTrans.Commit();
            }
            catch (Exception ex)
            {
                dbTrans.Rollback();
                nRes = -1;
                throw ex;
            }
            finally
            {
                if (conn.State == ConnectionState.Open) conn.Close();
            }

            return nRes;
        }

        #endregion

        #region Sql文，返回DataSet
        /// <summary>
        /// Sql文，返回DataSet
        /// </summary>
        /// <param name="strSql">Sql文，多条使用";"分号隔开</param>
        /// <returns>The dataset object</returns>
        public static DataSet QueryDataSet(string strSql)
        {
            return QueryDataSet(defaultDBString, strSql, null);
        }

        /// <summary>
        /// Sql文，返回DataSet
        /// </summary>
        /// <param name="dbName">指定数据库链接字符串</param>
        /// <param name="strSql">Sql文，多条使用";"分号隔开</param>
        /// <returns>The dataset object</returns>
        public static DataSet QueryDataSet(string dbName, string strSql)
        {
            return QueryDataSet(dbName, strSql, null);
        }

        /// <summary>
        /// Sql文，返回DataSet
        /// </summary>
        /// <param name="strSql">Sql文</param>
        /// <param name="sqlParams">参数数组</param>
        /// <returns>The dataset object</returns>
        public static DataSet QueryDataSet(string strSql, params SqlParameter[] sqlParams)
        {
            return QueryDataSet(defaultDBString, strSql, sqlParams);
        }

        /// <summary>
        /// Sql文，返回DataSet
        /// </summary>
        /// <param name="strSql">Sql文，多条使用";"分号隔开</param>
        /// <param name="sqlParamValues">参数数组</param>
        /// <returns>The dataset object</returns>
        public static DataSet QueryDataSet(string dbName, string strSql, params SqlParameter[] sqlParams)
        {
            Database db = DatabaseFactory.CreateDatabase(dbName);
            DbCommand dbCommand = db.GetSqlStringCommand(strSql);
            dbCommand.CommandTimeout = commandTimeout;

            if (sqlParams != null && sqlParams.Length > 0)
            {
                for (int i = 0; i < sqlParams.Length; i++)
                    dbCommand.Parameters.Add(sqlParams[i]);
            }

            DataSet ds = new DataSet();

            try
            {
                ds = db.ExecuteDataSet(dbCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;

        }

        #endregion

        #region Sql文，返回DataTable
        /// <summary>
        /// Sql文，返回DataTable
        /// </summary>
        /// <param name="strSql">Sql文，多条使用";"分号隔开</param>
        /// <returns>The datatable object</returns>
        public static DataTable QueryDataTable(string strSql)
        {
            DataSet ds = QueryDataSet(strSql);
            if (ds != null && ds.Tables.Count > 0)
                return ds.Tables[0];

            return new DataTable();
        }

        /// <summary>
        /// Sql文，返回DataTable
        /// </summary>
        /// <param name="strSql">Sql文，多条使用";"分号隔开</param>
        /// <param name="sqlParamValues">参数数组</param>
        /// <returns>The datatable object</returns>
        public static DataTable QueryDataTable(string strSql, params SqlParameter[] sqlParams)
        {
            return QueryDataTable(defaultDBString, strSql, sqlParams);
        }

        /// <summary>
        /// Sql文，返回DataTable
        /// </summary>
        /// <param name="dbName">指定数据库链接字符串</param>
        /// <param name="strSql">Sql文，多条使用";"分号隔开</param>
        /// <param name="sqlParams">参数数组</param>
        /// <returns>The datatable object</returns>
        public static DataTable QueryDataTable(string dbName, string strSql, params SqlParameter[] sqlParams)
        {
            DataSet ds = QueryDataSet(dbName, strSql, sqlParams);
            if (ds != null && ds.Tables.Count > 0)
                return ds.Tables[0];

            return new DataTable();
        }

        #endregion

        #region 根据Sql文，判断是否有记录存在
        /// <summary>
        /// 根据Sql文，判断是否有记录存在
        /// </summary>
        /// <param name="strSql">Sql文</param>
        /// <returns>false：不存在；true：存在</returns>
        public static bool CheckIsExist(string strSql)
        {
            DataTable dt = QueryDataTable(strSql);
            if (dt == null || dt.Rows.Count <= 0)
                return false;
            else
                return true;
        }
        #endregion

        #region Sql文，返回单值
        /// <summary>
        /// Sql文，返回单值
        /// </summary>
        /// <param name="strSql">Sql文</param>
        /// <returns>The Single Object</returns>
        public static object QueryValue(string strSql)
        {
            return QueryValue(defaultDBString, strSql);
        }

        /// <summary>
        /// Sql文，返回单值
        /// </summary>
        /// <param name="dbName">指定数据库链接字符串</param>
        /// <param name="strSql">Sql文，多语句用";"分号隔开</param>
        /// <returns>The Single Object</returns>
        public static object QueryValue(string dbName, string strSql)
        {
            Database db = DatabaseFactory.CreateDatabase(dbName);
            DbCommand dbCommand = db.GetSqlStringCommand(strSql);
            dbCommand.CommandTimeout = commandTimeout;

            object value = null;
            try
            {
                value = db.ExecuteScalar(dbCommand);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return value;

        }
        #endregion

        #region 执行SP_Data Put
        /// <summary>
        /// 执行默认数据连接的SP
        /// </summary>
        /// <param name="spName">SP名</param>
        /// <param name="sqlParams">SP参数</param>
        /// <returns>SP执行结果</returns>
        public static Object[] ExecReturnSP(string spName, params SqlParameter[] sqlParams)
        {
            int returnValue = 0;
            return ExecReturnSP(defaultDBString, spName, ref returnValue, sqlParams);
        }

        /// <summary>
        /// 执行指定数据连接的SP
        /// </summary>
        /// <param name="dbName">指定数据连接名称</param>
        /// <param name="spName">SP名</param>
        /// <param name="sqlParams">>SP参数</param>
        /// <returns>SP执行结果</returns>
        public static Object[] ExecReturnSP(string dbName, string spName, params SqlParameter[] sqlParams)
        {
            int returnValue = -1;
            return ExecReturnSP(dbName, spName, ref returnValue, sqlParams);

        }

        /// <summary>
        /// 执行SP
        /// </summary>
        /// <param name="dbName">指定数据连接名称</param>
        /// <param name="spName">SP名</param>
        /// <param name="returnValue">返回执行结果</param>
        /// <param name="spParams">SP参数</param>
        /// <returns>SP执行结果</returns>
        public static Object[] ExecReturnSP(string dbName, string spName, ref int returnValue, params SqlParameter[] spParams)
        {
            Database db = DatabaseFactory.CreateDatabase(dbName);
            DbCommand dbCommand = db.GetStoredProcCommand(spName);
            dbCommand.CommandTimeout = commandTimeout;

            if (spParams != null && spParams.Length > 0)
            {
                for (int i = 0; i < spParams.Length; i++)
                {
                    if (spParams[i].Direction == ParameterDirection.Input)
                        db.AddInParameter(dbCommand, spParams[i].ParameterName, spParams[i].DbType, spParams[i].Value);
                    else if (spParams[i].Direction == ParameterDirection.InputOutput ||
                             spParams[i].Direction == ParameterDirection.Output)
                        db.AddOutParameter(dbCommand, spParams[i].ParameterName, spParams[i].DbType, spParams[i].Size);
                }
            }
            db.AddParameter(dbCommand, "@RETURN_VALUE", DbType.Int32, ParameterDirection.ReturnValue,
                            "ReturnValue", DataRowVersion.Current, 0);

            int res = 0;
            ArrayList outParams = new ArrayList();
            try
            {
                res = db.ExecuteNonQuery(dbCommand);
            }
            catch (Exception e)
            {
                res = -1;
            }

            for (int i = 0; i < spParams.Length; i++)
            {
                if (spParams[i].Direction == ParameterDirection.Output ||
                    spParams[i].Direction == ParameterDirection.InputOutput)
                    outParams.Add(db.GetParameterValue(dbCommand, spParams[i].ParameterName));
            }

            returnValue = Convert.ToInt32(db.GetParameterValue(dbCommand, "RETURN_VALUE"));

            return outParams.ToArray();
        }
        #endregion

        #region 执行SP_Data_Get
        /// <summary>
        /// 执行SP_Data_Get
        /// </summary>
        /// <param name="spName">SP名</param>
        /// <param name="returnParams">SP执行结果</param>
        /// <param name="spParams">SP参数</param>
        /// <returns>DataTable</returns>
        public static DataTable ExecReturnDataTableSP(string spName, out object[] returnParams, params SqlParameter[] spParams)
        {
            int returnValue = 0;
            return ExecReturnDataTableSP(defaultDBString, spName, ref returnValue, out returnParams, spParams);
        }

        /// <summary>
        /// 执行SP_Data_Get
        /// </summary>
        /// <param name="spName">SP名</param>
        /// <param name="returnValue">返回执行结果</param>
        /// <param name="returnParams">SP执行结果</param>
        /// <param name="spParams">SP参数</param>
        /// <returns>DataTable</returns>
        public static DataTable ExecReturnDataTableSP(string spName, ref int returnValue, out object[] returnParams, params SqlParameter[] spParams)
        {
            return ExecReturnDataTableSP(defaultDBString, ref returnValue, out returnParams, spParams);
        }

        /// <summary>
        /// 执行SP_Data_Get
        /// </summary>
        /// <param name="dbName">指定数据连接名称</param>
        /// <param name="spName">SP名</param>
        /// <param name="returnValue">返回执行结果</param>
        /// <param name="returnParams">SP执行结果</param>
        /// <param name="spParams">SP参数</param>
        /// <returns>DataTable</returns>
        public static DataTable ExecReturnDataTableSP(string dbName, string spName, ref int returnValue, out object[] returnParams, params SqlParameter[] spParams)
        {
            Database db = DatabaseFactory.CreateDatabase(dbName);
            DbCommand dbCommand = db.GetStoredProcCommand(spName);
            dbCommand.CommandTimeout = commandTimeout;

            if (spParams != null && spParams.Length > 0)
            {
                for (int i = 0; i < spParams.Length; i++)
                {
                    if (spParams[i].Direction == ParameterDirection.Input)
                        db.AddInParameter(dbCommand, spParams[i].ParameterName, spParams[i].DbType, spParams[i].Value);
                    else if (spParams[i].Direction == ParameterDirection.InputOutput ||
                             spParams[i].Direction == ParameterDirection.Output)
                        db.AddOutParameter(dbCommand, spParams[i].ParameterName, spParams[i].DbType, spParams[i].Size);
                }
            }
            db.AddParameter(dbCommand, "@RETURN_VALUE", DbType.Int32, ParameterDirection.ReturnValue,
                            "ReturnValue", DataRowVersion.Current, 0);

            int res = 0;
            ArrayList outParams = new ArrayList();
            DataSet ds = new DataSet();
            try
            {
                ds = db.ExecuteDataSet(dbCommand);
            }
            catch (Exception e)
            {
                res = -1;
            }

            for (int i = 0; i < spParams.Length; i++)
            {
                if (spParams[i].Direction == ParameterDirection.Output ||
                    spParams[i].Direction == ParameterDirection.InputOutput)
                    outParams.Add(db.GetParameterValue(dbCommand, spParams[i].ParameterName));
            }

            returnValue = Convert.ToInt32(db.GetParameterValue(dbCommand, "RETURN_VALUE"));

            returnParams = outParams.ToArray();
            if (ds.Tables.Count == 0) return new DataTable();
            return ds.Tables[0];
        }
        #endregion
    }
}
