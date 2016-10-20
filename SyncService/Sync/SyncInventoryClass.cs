using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using DBUtility;
using System.Data.SqlClient;
namespace SyncService.Sync
{
    /// <summary>
    /// 
    /// </summary>
    public class SyncInventoryClass
    {
        private string PDACon = "PDAConnection";
        private string MainCon = "MainConnection";
        /// <summary>
        /// 同步主入口
        /// </summary>
        /// <param name="ID">同步记录表主键ID</param>
        public void DoSync(string ID)
        {
            string sql = string.Format("select * from NeedSyncList where ID={0}", ID);
            DataTable dt = new DataTable();
            try
            {
                dt = DBAccess.QueryDataTable(sql);
            }
            catch (Exception ex)
            {
                SyncHelper.InsertLog("未指定", "未指定", "未指定", "未指定", "未指定", "0", "查询需要NeedSyncList记录失败,主键ID值为:" + ID
                    + "#" + ex.Message + "#" + ex.Source + "#" + ex.StackTrace.Trim() + "#" + ex.TargetSite);
                return;
            }
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    switch (row["SyncType"].ToString())
                    {
                        case "Insert": DoInsert(row); break;
                        case "Update": DoUpdate(row); break;
                        default:
                            SyncHelper.InsertLog(row["BusinessID"].ToString(), row["BusinessCode"].ToString(), row["BusinessType"].ToString(), row["SyncType"].ToString(),
                                row["CodeType"].ToString(), row["IsFinish"].ToString(), "未找到指定的SyncType");
                            break;
                    }
                }
            }
            else
            {
                SyncHelper.InsertLog("未指定", "未指定", "未指定", "未指定", "未指定", "0", "NeedSyncList未查询到ID=" + ID + "的数据!");
                return;
            }
        }
        /// <summary>
        /// 执行Insert
        /// </summary>
        /// <param name="srow">NeedSyncList的条目</param>
        private void DoInsert(DataRow srow)
        {
            //将需要同步的ID赋值
            string id = srow["BusinessID"].ToString();
            DataRow row;//声明需要同步的记录
            try
            {
                string tsql = "select * from AA_InventoryClass where id=@id";
                IList<SqlParameter> tlist = new List<SqlParameter>();
                tlist.Add(new SqlParameter("@code", srow["BusinessID"].Equals(null) ? DBNull.Value : srow["BusinessID"]));
                row = DBAccess.QueryDataTable(MainCon, tsql, tlist.ToArray()).Rows[0];//赋值需要同步的记录
            }
            catch (Exception ex)
            {
                SyncHelper.InsertLog(srow["BusinessID"].ToString(), srow["BusinessCode"].ToString(), srow["BusinessType"].ToString(), srow["SyncType"].ToString(),
                                srow["CodeType"].ToString(), srow["IsFinish"].ToString(), "在正式帐套下找到ID为" + id + "的AA_InventoryClass条目");
                return;
            }
            IList<SqlParameter> paramList = new List<SqlParameter>();
            try
            {
                //IList<SqlParameter> paramList = new List<SqlParameter>();
                paramList.Add(new SqlParameter("@code", row["code"].Equals(null) ? DBNull.Value : row["code"]));
                paramList.Add(new SqlParameter("@name", row["name"].Equals(null) ? DBNull.Value : row["name"]));
                paramList.Add(new SqlParameter("@isEndNode", row["isEndNode"].Equals(null) ? DBNull.Value : row["isEndNode"]));
                paramList.Add(new SqlParameter("@depth", row["depth"].Equals(null) ? DBNull.Value : row["depth"]));
                paramList.Add(new SqlParameter("@disabled", row["disabled"].Equals(null) ? DBNull.Value : row["disabled"]));
                paramList.Add(new SqlParameter("@updatedBy", row["updatedBy"].Equals(null) ? DBNull.Value : row["updatedBy"]));
                paramList.Add(new SqlParameter("@inId", row["inId"].Equals(null) ? DBNull.Value : row["inId"]));
                paramList.Add(new SqlParameter("@idMarketingOrgan", row["idMarketingOrgan"].Equals(null) ? DBNull.Value : row["idMarketingOrgan"]));

                //获取ID
                string idparent = string.Empty;
                idparent = SyncHelper.GetidByCodeFormOtherBook(SyncHelper.GetCodeByIDFromThisBook(row["idparent"].ToString(), "AA_InventoryClass", MainCon), PDACon, "AA_InventoryClass");
                //paramList.Add(new SqlParameter("@idparent", string.IsNullOrEmpty(idparent) ? row["idparent"] : idparent));
                paramList.Add(new SqlParameter("@idparent", row["idparent"].Equals(null) ? DBNull.Value : row["idparent"]));

                paramList.Add(new SqlParameter("@madeDate", row["madeDate"].Equals(null) ? DBNull.Value : row["madeDate"]));
                paramList.Add(new SqlParameter("@updated", row["updated"].Equals(null) ? DBNull.Value : row["updated"]));
                paramList.Add(new SqlParameter("@createdTime", row["createdTime"].Equals(null) ? DBNull.Value : row["createdTime"]));
            }
            catch (Exception ex)
            {
                SyncHelper.InsertLog(row["id"].ToString(), row["code"].ToString(), "AA_InventoryClass", "Insert", "数据同步", "0",
                    "When SyncSystem SyncInventoryClass Do Insert,Some SqlParameter Throw Some Message,The Message Like[" + ex.InnerException + "]");
                return;
            }
            string sql = string.Empty;
            sql += "Insert into AA_InventoryClass(code,name,isEndNode,depth,disabled,updatedBy,inId,idMarketingOrgan,idparent,madeDate,updated,createdTime) ";
            sql += "values(@code,@name,@isEndNode,@depth,@disabled,@updatedBy,@inId,@idMarketingOrgan,@idparent,@madeDate,@updated,@createdTime)";
            try
            {
                DBAccess.ExecSql(PDACon, sql, paramList.ToArray());
                SyncHelper.UpdateSyncList(srow["BusinessID"].ToString(),srow["BusinessType"].ToString(),srow["SyncType"].ToString(),1.ToString(),DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch (Exception ex)
            {
                SyncHelper.InsertLog(row["id"].ToString(), row["code"].ToString(), "AA_InventoryClass", "Insert", "数据同步", "0",
                    "When SyncSystem SyncInventoryClass Do Insert,This Sql Throw Some Message,The Message Like[" + ex.InnerException + "]");
                return;
            }
        }
        private void DoUpdate(DataRow srow)
        {
            //将需要同步的ID赋值
            string id = srow["BusinessID"].ToString();
            DataRow row;//声明需要同步的记录
            try
            {
                string tsql = "select * from AA_InventoryClass where id=@id";
                IList<SqlParameter> tlist = new List<SqlParameter>();
                tlist.Add(new SqlParameter("@id", srow["BusinessID"].Equals(null) ? DBNull.Value : srow["BusinessID"]));
                row = DBAccess.QueryDataTable(MainCon, tsql, tlist.ToArray()).Rows[0];//赋值需要同步的记录
            }
            catch (Exception ex)
            {
                SyncHelper.InsertLog(srow["BusinessID"].ToString(), srow["BusinessCode"].ToString(), srow["BusinessType"].ToString(), srow["SyncType"].ToString(),
                                srow["CodeType"].ToString(), srow["IsFinish"].ToString(), "在正式帐套下找到ID为" + id + "的AA_InventoryClass条目");
                return;
            }
            IList<SqlParameter> paramList = new List<SqlParameter>();
            try
            {
                paramList.Add(new SqlParameter("@id", row["id"].Equals(null) ? DBNull.Value : row["id"]));
                paramList.Add(new SqlParameter("@code", row["code"].Equals(null) ? DBNull.Value : row["code"]));
                paramList.Add(new SqlParameter("@name", row["name"].Equals(null) ? DBNull.Value : row["name"]));
                paramList.Add(new SqlParameter("@isEndNode", row["isEndNode"].Equals(null) ? DBNull.Value : row["isEndNode"]));
                paramList.Add(new SqlParameter("@depth", row["depth"].Equals(null) ? DBNull.Value : row["depth"]));
                paramList.Add(new SqlParameter("@disabled", row["disabled"].Equals(null) ? DBNull.Value : row["disabled"]));
                paramList.Add(new SqlParameter("@updatedBy", row["updatedBy"].Equals(null) ? DBNull.Value : row["updatedBy"]));
                paramList.Add(new SqlParameter("@inId", row["inId"].Equals(null) ? DBNull.Value : row["inId"]));
                paramList.Add(new SqlParameter("@idMarketingOrgan", row["idMarketingOrgan"].Equals(null) ? DBNull.Value : row["idMarketingOrgan"]));
                paramList.Add(new SqlParameter("@idparent", row["idparent"].Equals(null) ? DBNull.Value : row["idparent"]));
                paramList.Add(new SqlParameter("@madeDate", row["madeDate"].Equals(null) ? DBNull.Value : row["madeDate"]));
                paramList.Add(new SqlParameter("@updated", row["updated"].Equals(null) ? DBNull.Value : row["updated"]));
                paramList.Add(new SqlParameter("@createdTime", row["createdTime"].Equals(null) ? DBNull.Value : row["createdTime"]));
            }
            catch (Exception ex)
            {
                SyncHelper.InsertLog(row["id"].ToString(), row["code"].ToString(), "AA_InventoryClass", "Update", "数据同步", "0",
                    "When SyncSystem SyncInventoryClass Do Update,Some SqlParameter Throw Some Message,The Message Like[" + ex.InnerException + "]");
                return;
            }
            string sql = string.Empty;
            sql += "update AA_InventoryClass set code=@code,name=@name,isEndNode=@isEndNode,depth=@depth,disabled=@disabled,";
            sql += "updatedBy = @updatedBy,inId = @inId,idMarketing = @idMarketing,idparent = @idparent,madeDate = @madeDate,";
            sql += "updated = @updated,createdTime = @createdTime where id = @id";
            try
            {
                DBAccess.ExecSql(PDACon, sql, paramList.ToArray());
                SyncHelper.UpdateSyncList(srow["BusinessID"].ToString(), srow["BusinessType"].ToString(), srow["SyncType"].ToString(), 1.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch (Exception ex)
            {
                SyncHelper.InsertLog(row["id"].ToString(), row["code"].ToString(), "AA_InventoryClass", "Insert", "数据同步", "0",
                    "When SyncSystem SyncInventoryClass Do Update,This Sql Throw Some Message,The Message Like[" + ex.InnerException + "]");
                return;
            }

        }
    }
}
