using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using DBUtility;
namespace SyncService.Sync
{
    public class SyncUnit
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
                string tsql = "select * from AA_Unit where id=@id";
                IList<SqlParameter> tlist = new List<SqlParameter>();
                tlist.Add(new SqlParameter("@id", srow["BusinessID"].Equals(null) ? DBNull.Value : srow["BusinessID"]));
                row = DBAccess.QueryDataTable(MainCon, tsql, tlist.ToArray()).Rows[0];//赋值需要同步的记录
            }
            catch (Exception ex)
            {
                SyncHelper.InsertLog(srow["BusinessID"].ToString(), srow["BusinessCode"].ToString(), srow["BusinessType"].ToString(), srow["SyncType"].ToString(),
                                srow["CodeType"].ToString(), srow["IsFinish"].ToString(), "在正式帐套下找到ID为" + id + "的AA_Unit条目");
                return;
            }
            IList<SqlParameter> paramList = new List<SqlParameter>();
            try
            {
                //IList<SqlParameter> paramList = new List<SqlParameter>();
                paramList.Add(new SqlParameter("@code", row["code"].Equals(null) ? DBNull.Value : row["code"]));
                paramList.Add(new SqlParameter("@name", row["name"].Equals(null) ? DBNull.Value : row["name"]));
                paramList.Add(new SqlParameter("@isMainUnit", row["isMainUnit"].Equals(null) ? DBNull.Value : row["isMainUnit"]));
                paramList.Add(new SqlParameter("@changeRate", row["changeRate"].Equals(null) ? DBNull.Value : row["changeRate"]));
                paramList.Add(new SqlParameter("@isSingleUnit", row["isSingleUnit"].Equals(null) ? DBNull.Value : row["isSingleUnit"]));
                paramList.Add(new SqlParameter("@disabled", row["disabled"].Equals(null) ? DBNull.Value : row["disabled"]));
                paramList.Add(new SqlParameter("@updatedBy", row["updatedBy"].Equals(null) ? DBNull.Value : row["updatedBy"]));
                paramList.Add(new SqlParameter("@rateDescription", row["rateDescription"].Equals(null) ? DBNull.Value : row["rateDescription"]));

                //获取ID
                string idparent = string.Empty;
                idparent = SyncHelper.GetidByCodeFormOtherBook(SyncHelper.GetCodeByIDFromThisBook(row["idparent"].ToString(), "AA_InventoryClass", MainCon), PDACon, "AA_InventoryClass");
                //paramList.Add(new SqlParameter("@idparent", string.IsNullOrEmpty(idparent) ? row["idparent"] : idparent));
                paramList.Add(new SqlParameter("@isGroup", row["isGroup"].Equals(null) ? DBNull.Value : row["isGroup"]));

                paramList.Add(new SqlParameter("@idunitgroup", row["idunitgroup"].Equals(null) ? DBNull.Value : row["idunitgroup"]));
                paramList.Add(new SqlParameter("@changeType", row["changeType"].Equals(null) ? DBNull.Value : row["changeType"]));
                paramList.Add(new SqlParameter("@changeType1", row["changeType1"].Equals(null) ? DBNull.Value : row["changeType1"]));
                paramList.Add(new SqlParameter("@madeDate", row["madeDate"].Equals(null) ? DBNull.Value : row["madeDate"]));
                paramList.Add(new SqlParameter("@updated", row["updated"].Equals(null) ? DBNull.Value : row["updated"]));
                paramList.Add(new SqlParameter("@createdTime", row["createdTime"].Equals(null) ? DBNull.Value : row["createdTime"]));
            }
            catch (Exception ex)
            {
                SyncHelper.InsertLog(row["id"].ToString(), row["code"].ToString(), "AA_Unit", "Insert", "数据同步", "0",
                    "When SyncSystem SyncUnit Do Insert,Some SqlParameter Throw Some Message,The Message Like[" + ex.InnerException + "]");
                return;
            }
            string sql = string.Empty;
            sql += "insert into AA_Unit(code,name,isMainUnit,changeRate,isSingleUnit,disabled,updatedBy,rateDescription,";
            sql += "isGroup,idunitgroup,changeType,changeType1,madeDate,updated,createdTime) values(@code, @name, @isMainUnit,";
            sql += "@changeRate, @isSingleUnit, @disabled, @updatedBy, @rateDescription, @isGroup, @idunitgroup, @changeType, @changeType1,";
            sql += "@madeDate, @updated, @createdTime)";
            try
            {
                DBAccess.ExecSql(PDACon, sql, paramList.ToArray());
                SyncHelper.UpdateSyncList(srow["BusinessID"].ToString(), srow["BusinessType"].ToString(), srow["SyncType"].ToString(), 1.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch (Exception ex)
            {
                SyncHelper.InsertLog(row["id"].ToString(), row["code"].ToString(), "AA_InventoryClass", "Insert", "数据同步", "0",
                    "When SyncSystem SyncUnit Do Insert,This Sql Throw Some Message,The Message Like[" + ex.InnerException + "]");
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
                                srow["CodeType"].ToString(), srow["IsFinish"].ToString(), "在正式帐套下找到ID为" + id + "的AA_Unit条目");
                return;
            }
            IList<SqlParameter> paramList = new List<SqlParameter>();
            try
            {
                paramList.Add(new SqlParameter("@id", row["id"].Equals(null) ? DBNull.Value : row["id"]));
                paramList.Add(new SqlParameter("@code", row["code"].Equals(null) ? DBNull.Value : row["code"]));
                paramList.Add(new SqlParameter("@name", row["name"].Equals(null) ? DBNull.Value : row["name"]));
                paramList.Add(new SqlParameter("@isMainUnit", row["isMainUnit"].Equals(null) ? DBNull.Value : row["isMainUnit"]));
                paramList.Add(new SqlParameter("@changeRate", row["changeRate"].Equals(null) ? DBNull.Value : row["changeRate"]));
                paramList.Add(new SqlParameter("@isSingleUnit", row["isSingleUnit"].Equals(null) ? DBNull.Value : row["isSingleUnit"]));
                paramList.Add(new SqlParameter("@disabled", row["disabled"].Equals(null) ? DBNull.Value : row["disabled"]));
                paramList.Add(new SqlParameter("@updatedBy", row["updatedBy"].Equals(null) ? DBNull.Value : row["updatedBy"]));
                paramList.Add(new SqlParameter("@rateDescription", row["rateDescription"].Equals(null) ? DBNull.Value : row["rateDescription"]));

                //获取ID
                string idparent = string.Empty;
                idparent = SyncHelper.GetidByCodeFormOtherBook(SyncHelper.GetCodeByIDFromThisBook(row["idparent"].ToString(), "AA_InventoryClass", MainCon), PDACon, "AA_InventoryClass");
                //paramList.Add(new SqlParameter("@idparent", string.IsNullOrEmpty(idparent) ? row["idparent"] : idparent));
                paramList.Add(new SqlParameter("@isGroup", row["isGroup"].Equals(null) ? DBNull.Value : row["isGroup"]));

                paramList.Add(new SqlParameter("@idunitgroup", row["idunitgroup"].Equals(null) ? DBNull.Value : row["idunitgroup"]));
                paramList.Add(new SqlParameter("@changeType", row["changeType"].Equals(null) ? DBNull.Value : row["changeType"]));
                paramList.Add(new SqlParameter("@changeType1", row["changeType1"].Equals(null) ? DBNull.Value : row["changeType1"]));
                paramList.Add(new SqlParameter("@madeDate", row["madeDate"].Equals(null) ? DBNull.Value : row["madeDate"]));
                paramList.Add(new SqlParameter("@updated", row["updated"].Equals(null) ? DBNull.Value : row["updated"]));
                paramList.Add(new SqlParameter("@createdTime", row["createdTime"].Equals(null) ? DBNull.Value : row["createdTime"]));
            }
            catch (Exception ex)
            {
                SyncHelper.InsertLog(row["id"].ToString(), row["code"].ToString(), "AA_Unit", "Update", "数据同步", "0",
                    "When SyncSystem SyncInventoryClass Do Update,Some SqlParameter Throw Some Message,The Message Like[" + ex.InnerException + "]");
                return;
            }
            string sql = string.Empty;
            sql += "update AA_Unit set code=@code,name=@name,isMainUnit=@isMainUnit,changeRate=@changeRate,isSingleUnit=@isSingleUnit,";
            sql += "disabled = @disabled,updatedBy = @updatedBy,rateDescription = @rateDescription,isGroup = @isGroup,idunitgroup = @idunitgroup,";
            sql += "changeType = @changeType,changeType1 = @changeType1,madeDate = @madeDate,updated = @updated,createdTime = createdTime where id = @id";
            try
            {
                DBAccess.ExecSql(PDACon, sql, paramList.ToArray());
                SyncHelper.UpdateSyncList(srow["BusinessID"].ToString(), srow["BusinessType"].ToString(), srow["SyncType"].ToString(), 1.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch (Exception ex)
            {
                SyncHelper.InsertLog(row["id"].ToString(), row["code"].ToString(), "AA_Unit", "Insert", "数据同步", "0",
                    "When SyncSystem SyncUnit Do Update,This Sql Throw Some Message,The Message Like[" + ex.InnerException + "]");
                return;
            }

        }
    }
}
