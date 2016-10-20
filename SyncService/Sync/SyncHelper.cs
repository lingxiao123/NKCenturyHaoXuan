using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using DBUtility;
using System.IO;
using Chanjet.TP.OpenAPI;
using System.Data;
/// <summary>
/// 同步程序的基础类,所有同步类都继承此类.
/// Add By WJL 2016-10-11 16:26:52
/// 主要功能:1.针对日志处理;
///          2.同步完成后修改同步记录;
///          
/// </summary>
namespace SyncService.Sync
{
    public static class SyncHelper
    {
        /// <summary>
        /// 日志表(Insert)
        /// </summary>
        /// <param name="BusinessID">业务ID</param>
        /// <param name="BusinessCode">业务Code</param>
        /// <param name="BusinessType">业务类型</param>
        /// <param name="CodeType">代码类型</param>
        /// <param name="IsFinish">当前操作是否成功</param>
        /// <param name="Remark">备注,若出现错误,写明错误信息</param>
        public static void InsertLog(string BusinessID, string BusinessCode, string BusinessType, string SyncType, string CodeType, string IsFinish, string Remark)
        {
            IList<SqlParameter> listParam = new List<SqlParameter>();
            listParam.Add(new SqlParameter("@BusinessID", BusinessID));
            listParam.Add(new SqlParameter("@BusinessCode", BusinessCode));
            listParam.Add(new SqlParameter("@BusinessType", BusinessType));
            listParam.Add(new SqlParameter("@CodeType", CodeType));
            listParam.Add(new SqlParameter("@SyncType", SyncType));
            listParam.Add(new SqlParameter("@IsFinish", IsFinish));
            listParam.Add(new SqlParameter("@AddTime", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")));
            listParam.Add(new SqlParameter("@Remark", Remark));
            string sql = string.Format("insert into SyncLog values(@BusinessID,@BusinessCode,@BusinessType,@SyncType,@CodeType,@IsFinish,@AddTime,@Remark);");
            try
            {
                DBAccess.ExecSql(sql, listParam.ToArray());
            }
            catch (Exception ex)
            {
                WriteError(ex);
            }
        }
        /// <summary>
        /// 更新待同步列表
        /// </summary>
        /// <param name="BusinessID">业务ID</param>
        /// <param name="BusinessType">业务类型</param>
        /// <param name="IsSync">是否已同步</param>
        /// <param name="SyncTime">同步的时间[默认Now]</param>
        public static void UpdateSyncList(string BusinessID, string BusinessType,string SyncType, string IsSync, string SyncTime)
        {
            IList<SqlParameter> listParam = new List<SqlParameter>();
            listParam.Add(new SqlParameter("@BusinessID", BusinessID));
            listParam.Add(new SqlParameter("@BusinessType", BusinessType));
            listParam.Add(new SqlParameter("@SyncType", SyncType));
            listParam.Add(new SqlParameter("@IsSync", IsSync));
            listParam.Add(new SqlParameter("@SyncTime", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss")));
            string sql = string.Format("update NeedSyncList set IsSync=@IsSync,SyncTime=@SyncTime where BusinessID=@BusinessID and BusinessType=@BusinessType and SyncType=@SyncType;");
            try
            {
                DBAccess.ExecSql(sql, listParam.ToArray());
            }
            catch (Exception ex)
            {
                WriteError(ex);
            }
        }
        ///写文件
        private static StreamWriter streamWriter;
        /// <summary>
        /// 记录文本日志文件(当日志表不能插入日志数据时使用)
        /// </summary>
        /// <param name="ex">错误信息</param>
        public static void WriteError(Exception ex)
        {
            try
            {
                //DateTime dt = new DateTime();
                string directPath = AppDomain.CurrentDomain.BaseDirectory + "logs";    //获得文件夹路径
                if (!Directory.Exists(directPath))   //判断文件夹是否存在，如果不存在则创建
                {
                    Directory.CreateDirectory(directPath);
                }
                directPath += string.Format(@"\{0}.txt", DateTime.Now.ToString("yyyy-MM-dd"));
                if (streamWriter == null)
                {
                    streamWriter = !File.Exists(directPath) ? File.CreateText(directPath) : File.AppendText(directPath);    //判断文件是否存在如果不存在则创建，如果存在则添加。
                }
                streamWriter.WriteLine("***********************************************************************");
                streamWriter.WriteLine(DateTime.Now.ToString("HH:mm:ss"));
                streamWriter.WriteLine("输出信息：错误信息");
                if (ex != null)
                {
                    streamWriter.WriteLine("当前时间：" + DateTime.Now.ToString());
                    streamWriter.WriteLine("异常信息：" + ex.Message);
                    streamWriter.WriteLine("异常对象：" + ex.Source);
                    streamWriter.WriteLine("调用堆栈：\n" + ex.StackTrace.Trim());
                    streamWriter.WriteLine("触发方法：" + ex.TargetSite);
                    streamWriter.WriteLine();
                }
            }
            finally
            {
                if (streamWriter != null)
                {
                    streamWriter.Flush();
                    streamWriter.Close();
                    streamWriter = null;
                }
            }
        }
        /// <summary>
        /// ChanJet.API 可调用部分接口.
        /// </summary>
        /// <param name="MoudleID">模块ID</param>
        /// <param name="param">param = new { Code = "0KH0001" };</param>
        /// <returns></returns>
        public static dynamic[] UseAPI(string MoudleID, object param)
        {
            string host = "192.168.1.133:8080", uri = "TPlus/api/v1/" + MoudleID + "/Query";
            string ApiUrl = string.Format("http://{0}/{1}", host, uri);
            OpenAPI api = new OpenAPI(ApiUrl, new Credentials()
            {
                AppKey = "0ff8da4e-9a7e-4112-a993-f5d45d2979c8",
                AppSecret = "avgguf"
            });
            //param = new { Code = "0KH0001" };
            dynamic[] result = api.Call(ApiUrl, param);
            return result;
        }
        /// <summary>
        /// 根据ID获取当前帐套下的Code
        /// </summary>
        /// <param name="id">当前记录的ID</param>
        /// <param name="othertable">表名</param>
        /// <param name="conn">帐套的链接字符串</param>
        /// <returns>返回当前帐套下的Code</returns>
        public static string GetCodeByIDFromThisBook(string id, string othertable, string conn)
        {
            string ReturnCode = string.Empty;
            string sql = "select code from " + othertable + " where id='" + id + "'";
            try
            {
                DataTable dt = DBAccess.QueryDataTable(conn, sql, null);
                if (dt != null && dt.Rows.Count > 0)
                    ReturnCode = dt.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                InsertLog("未指定", "未指定", "未指定", "未指定", "未指定", "0", "调用GetCodeByIDFromThisBook方法失败!表名[" + othertable + "]");
            }
            return ReturnCode;
        }
        /// <summary>
        /// 根据Code查找在其他帐套下的ID
        /// </summary>
        /// <param name="code">当前记录的Code值</param>
        /// <param name="conn">其他帐套的链接字符串</param>
        /// <param name="othertable">其他帐套的表名</param>
        /// <returns>返回在其他帐套下的ID</returns>
        public static string GetidByCodeFormOtherBook(string code, string conn, string othertable)
        {
            string ReturnCode = string.Empty;
            string sql = "select id from " + othertable + " where code ='" + code + "'";
            try
            {
                DataTable dt = DBAccess.QueryDataTable(conn, sql, null);
                if (dt != null && dt.Rows.Count > 0)
                    ReturnCode = dt.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                InsertLog("未指定", "未指定", "未指定", "未指定", "未指定", "0", "调用GetidByCodeFormOtherBook方法失败!表名[" + othertable + "]");
            }
            return ReturnCode;
        }
    }
}
