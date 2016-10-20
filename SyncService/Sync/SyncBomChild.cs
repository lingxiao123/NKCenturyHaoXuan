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
    public class SyncBomChild
    {
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
                SyncHelper.InsertLog("未指定", "未指定", "未指定", "未指定", "未指定", "0", "NeedSyncList为查询到ID=" + ID + "的数据!");
            }
        }

        private void DoInsert(DataRow row)
        {
            string sql = string.Format("select * from AA_BOMChild where id={0}", row["BusinessID"]);
            DataTable dt = MainDBAccess.QueryDataTable(sql);
            if (dt.Rows.Count>0)
            {
                foreach (DataRow rows in dt.Rows)
                {
                    try
                    {
                        IList<SqlParameter> listParam = new List<SqlParameter>();
                        listParam.Add(new SqlParameter("@code", rows["code"].Equals(null) ? DBNull.Value : rows["code"]));
                        listParam.Add(new SqlParameter("@name", rows["name"].Equals(null) ? DBNull.Value : rows["name"]));
                        listParam.Add(new SqlParameter("@produceQuantity", rows["produceQuantity"].Equals(null) ? DBNull.Value : rows["produceQuantity"]));
                        listParam.Add(new SqlParameter("@rationQuantity", rows["rationQuantity"].Equals(null) ? DBNull.Value : rows["rationQuantity"]));
                        listParam.Add(new SqlParameter("@wasteRate", rows["wasteRate"].Equals(null) ? DBNull.Value : rows["wasteRate"]));
                        listParam.Add(new SqlParameter("@ts", rows["ts"].Equals(null) ? DBNull.Value : rows["ts"]));
                        listParam.Add(new SqlParameter("@updatedBy", rows["updatedBy"].Equals(null) ? DBNull.Value : rows["updatedBy"]));
                        listParam.Add(new SqlParameter("@freeItem0", rows["freeItem0"].Equals(null) ? DBNull.Value : rows["freeItem0"]));
                        listParam.Add(new SqlParameter("@freeItem1", rows["freeItem1"].Equals(null) ? DBNull.Value : rows["freeItem1"]));
                        listParam.Add(new SqlParameter("@freeItem2", rows["freeItem2"].Equals(null) ? DBNull.Value : rows["freeItem2"]));
                        listParam.Add(new SqlParameter("@freeItem3", rows["freeItem3"].Equals(null) ? DBNull.Value : rows["freeItem3"]));
                        listParam.Add(new SqlParameter("@freeItem4", rows["freeItem4"].Equals(null) ? DBNull.Value : rows["freeItem4"]));
                        listParam.Add(new SqlParameter("@freeItem5", rows["freeItem5"].Equals(null) ? DBNull.Value : rows["freeItem5"]));
                        listParam.Add(new SqlParameter("@freeItem6", rows["freeItem6"].Equals(null) ? DBNull.Value : rows["freeItem6"]));
                        listParam.Add(new SqlParameter("@freeItem7", rows["freeItem7"].Equals(null) ? DBNull.Value : rows["freeItem7"]));
                        listParam.Add(new SqlParameter("@freeItem8", rows["freeItem8"].Equals(null) ? DBNull.Value : rows["freeItem8"]));
                        listParam.Add(new SqlParameter("@freeItem9", rows["freeItem9"].Equals(null) ? DBNull.Value : rows["freeItem9"]));
                        listParam.Add(new SqlParameter("@memo", rows["memo"].Equals(null) ? DBNull.Value : rows["memo"]));
                        listParam.Add(new SqlParameter("@requiredquantity", rows["requiredquantity"].Equals(null) ? DBNull.Value : rows["requiredquantity"]));
                        listParam.Add(new SqlParameter("@requiredquantity2", rows["requiredquantity2"].Equals(null) ? DBNull.Value : rows["requiredquantity2"]));
                        listParam.Add(new SqlParameter("@rateofexchange", rows["rateofexchange"].Equals(null) ? DBNull.Value : rows["rateofexchange"]));
                        listParam.Add(new SqlParameter("@cost", rows["cost"].Equals(null) ? DBNull.Value : rows["cost"]));
                        listParam.Add(new SqlParameter("@backflushmaterial", rows["backflushmaterial"].Equals(null) ? DBNull.Value : rows["backflushmaterial"]));
                        listParam.Add(new SqlParameter("@defaultchoice", rows["defaultchoice"].Equals(null) ? DBNull.Value : rows["defaultchoice"]));
                        listParam.Add(new SqlParameter("@priuserdefnvc1", rows["priuserdefnvc1"].Equals(null) ? DBNull.Value : rows["priuserdefnvc1"]));
                        listParam.Add(new SqlParameter("@priuserdefnvc2", rows["priuserdefnvc2"].Equals(null) ? DBNull.Value : rows["priuserdefnvc2"]));
                        listParam.Add(new SqlParameter("@priuserdefnvc3", rows["priuserdefnvc3"].Equals(null) ? DBNull.Value : rows["priuserdefnvc3"]));
                        listParam.Add(new SqlParameter("@priuserdefnvc4", rows["priuserdefnvc4"].Equals(null) ? DBNull.Value : rows["priuserdefnvc4"]));
                        listParam.Add(new SqlParameter("@priuserdefdecm1", rows["priuserdefdecm1"].Equals(null) ? DBNull.Value : rows["priuserdefdecm1"]));
                        listParam.Add(new SqlParameter("@priuserdefdecm2", rows["priuserdefdecm2"].Equals(null) ? DBNull.Value : rows["priuserdefdecm2"]));
                        listParam.Add(new SqlParameter("@priuserdefdecm3", rows["priuserdefdecm3"].Equals(null) ? DBNull.Value : rows["priuserdefdecm3"]));
                        listParam.Add(new SqlParameter("@priuserdefdecm4", rows["priuserdefdecm4"].Equals(null) ? DBNull.Value : rows["priuserdefdecm4"]));
                        listParam.Add(new SqlParameter("@pubuserdefnvc1", rows["pubuserdefnvc1"].Equals(null) ? DBNull.Value : rows["pubuserdefnvc1"]));
                        listParam.Add(new SqlParameter("@pubuserdefnvc2", rows["pubuserdefnvc2"].Equals(null) ? DBNull.Value : rows["pubuserdefnvc2"]));
                        listParam.Add(new SqlParameter("@pubuserdefnvc3", rows["pubuserdefnvc3"].Equals(null) ? DBNull.Value : rows["pubuserdefnvc3"]));
                        listParam.Add(new SqlParameter("@pubuserdefnvc4", rows["pubuserdefnvc4"].Equals(null) ? DBNull.Value : rows["pubuserdefnvc4"]));
                        listParam.Add(new SqlParameter("@pubuserdefdecm1", rows["pubuserdefdecm1"].Equals(null) ? DBNull.Value : rows["pubuserdefdecm1"]));
                        listParam.Add(new SqlParameter("@pubuserdefdecm2", rows["pubuserdefdecm2"].Equals(null) ? DBNull.Value : rows["pubuserdefdecm2"]));
                        listParam.Add(new SqlParameter("@pubuserdefdecm3", rows["pubuserdefdecm3"].Equals(null) ? DBNull.Value : rows["pubuserdefdecm3"]));
                        listParam.Add(new SqlParameter("@pubuserdefdecm4", rows["pubuserdefdecm4"].Equals(null) ? DBNull.Value : rows["pubuserdefdecm4"]));
                        listParam.Add(new SqlParameter("@BatchNumber", rows["BatchNumber"].Equals(null) ? DBNull.Value : rows["BatchNumber"]));
                        listParam.Add(new SqlParameter("@FailDate", rows["FailDate"].Equals(null) ? DBNull.Value : rows["FailDate"]));
                        listParam.Add(new SqlParameter("@idchildbom", rows["idchildbom"].Equals(null) ? DBNull.Value : rows["idchildbom"]));
                        listParam.Add(new SqlParameter("@idbom", rows["idbom"].Equals(null) ? DBNull.Value : rows["idbom"]));
                        listParam.Add(new SqlParameter("@idBomRelationDTO", rows["idBomRelationDTO"].Equals(null) ? DBNull.Value : rows["idBomRelationDTO"]));
                        listParam.Add(new SqlParameter("@idinventory", rows["idinventory"].Equals(null) ? DBNull.Value : rows["idinventory"]));
                        listParam.Add(new SqlParameter("@idproductprocess", rows["idproductprocess"].Equals(null) ? DBNull.Value : rows["idproductprocess"]));
                        listParam.Add(new SqlParameter("@idunit", rows["idunit"].Equals(null) ? DBNull.Value : rows["idunit"]));
                        listParam.Add(new SqlParameter("@idunit2", rows["idunit2"].Equals(null) ? DBNull.Value : rows["idunit2"]));
                        listParam.Add(new SqlParameter("@idwarehouse", rows["idwarehouse"].Equals(null) ? DBNull.Value : rows["idwarehouse"]));
                        listParam.Add(new SqlParameter("@bomchildattribute", rows["bomchildattribute"].Equals(null) ? DBNull.Value : rows["bomchildattribute"]));
                        listParam.Add(new SqlParameter("@madeDate", rows["madeDate"].Equals(null) ? DBNull.Value : rows["madeDate"]));
                        listParam.Add(new SqlParameter("@updated", rows["updated"].Equals(null) ? DBNull.Value : rows["updated"]));
                        listParam.Add(new SqlParameter("@createdtime", rows["createdtime"].Equals(null) ? DBNull.Value : rows["createdtime"]));

                        string sqls = string.Empty;
                        sqls += "insert into AA_BOMChild(code,name,produceQuantity,rationQuantity,wasteRate,";
                        sqls += "updatedBy,freeItem0,freeItem1,freeItem2,freeItem3,freeItem4,freeItem5,";
                        sqls += "freeItem6,freeItem7,freeItem8,freeItem9,memo,requiredquantity,requiredquantity2,";
                        sqls += "rateofexchange,cost,backflushmaterial,defaultchoice,priuserdefnvc1,priuserdefnvc2,";
                        sqls += "priuserdefnvc3,priuserdefnvc4,priuserdefdecm1,priuserdefdecm2,priuserdefdecm3,";
                        sqls += "priuserdefdecm4,pubuserdefnvc1,pubuserdefnvc2,pubuserdefnvc3,pubuserdefnvc4,";
                        sqls += "pubuserdefdecm1,pubuserdefdecm2,pubuserdefdecm3,pubuserdefdecm4,";
                        sqls += "BatchNumber,FailDate,idchildbom,idbom,idBomRelationDTO,idinventory,";
                        sqls += "idproductprocess,idunit,idunit2,idwarehouse,bomchildattribute,madeDate,";
                        sqls += "updated,createdtime)";
                        sqls += " values(@code,@name,@produceQuantity,@rationQuantity,@wasteRate,";
                        sqls += "@updatedBy,@freeItem0,@freeItem1,@freeItem2,@freeItem3,@freeItem4,@freeItem5,";
                        sqls += "@freeItem6,@freeItem7,@freeItem8,@freeItem9,@memo,@requiredquantity,@requiredquantity2,";
                        sqls += "@rateofexchange,@cost,@backflushmaterial,@defaultchoice,@priuserdefnvc1,@priuserdefnvc2,";
                        sqls += "@priuserdefnvc3,@priuserdefnvc4,@priuserdefdecm1,@priuserdefdecm2,@priuserdefdecm3,";
                        sqls += "@priuserdefdecm4,@pubuserdefnvc1,@pubuserdefnvc2,@pubuserdefnvc3,@pubuserdefnvc4,";
                        sqls += "@pubuserdefdecm1,@pubuserdefdecm2,@pubuserdefdecm3,@pubuserdefdecm4,";
                        sqls += "@BatchNumber,@FailDate,@idchildbom,@idbom,@idBomRelationDTO,@idinventory,";
                        sqls += "@idproductprocess,@idunit,@idunit2,@idwarehouse,@bomchildattribute,@madeDate,";
                        sqls += "@updated,@createdtime)";
                        int count= PDADBAccess.ExecTransSql(sqls,listParam.ToArray());
                        if (count > 0)
                        {
                            SyncHelper.UpdateSyncList(row["BusinessID"].ToString(), row["SyncType"].ToString(), row["SyncType"].ToString(), 1.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                    }
                    catch (Exception ex)
                    {
                        SyncHelper.InsertLog(row["id"].ToString(), row["code"].ToString(), "AA_BOMChild", "Insert", "数据同步", "0",
                        "#" + ex.Message + "#" + ex.Source + "#" + ex.StackTrace.Trim() + "#" + ex.TargetSite);
                        return;
                    }
                }
            }
        }

        private void DoUpdate(DataRow row)
        {
            string sql = string.Format("select * from AA_BOMChild where id={0}", row["BusinessID"]);
            DataTable dt = MainDBAccess.QueryDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow rows in dt.Rows)
                {
                    try
                    {
                        IList<SqlParameter> listParam = new List<SqlParameter>();
                        listParam.Add(new SqlParameter("@code", rows["code"].Equals(null) ? DBNull.Value : rows["code"]));
                        listParam.Add(new SqlParameter("@name", rows["name"].Equals(null) ? DBNull.Value : rows["name"]));
                        listParam.Add(new SqlParameter("@produceQuantity", rows["produceQuantity"].Equals(null) ? DBNull.Value : rows["produceQuantity"]));
                        listParam.Add(new SqlParameter("@rationQuantity", rows["rationQuantity"].Equals(null) ? DBNull.Value : rows["rationQuantity"]));
                        listParam.Add(new SqlParameter("@wasteRate", rows["wasteRate"].Equals(null) ? DBNull.Value : rows["wasteRate"]));
                        listParam.Add(new SqlParameter("@ts", rows["ts"].Equals(null) ? DBNull.Value : rows["ts"]));
                        listParam.Add(new SqlParameter("@updatedBy", rows["updatedBy"].Equals(null) ? DBNull.Value : rows["updatedBy"]));
                        listParam.Add(new SqlParameter("@freeItem0", rows["freeItem0"].Equals(null) ? DBNull.Value : rows["freeItem0"]));
                        listParam.Add(new SqlParameter("@freeItem1", rows["freeItem1"].Equals(null) ? DBNull.Value : rows["freeItem1"]));
                        listParam.Add(new SqlParameter("@freeItem2", rows["freeItem2"].Equals(null) ? DBNull.Value : rows["freeItem2"]));
                        listParam.Add(new SqlParameter("@freeItem3", rows["freeItem3"].Equals(null) ? DBNull.Value : rows["freeItem3"]));
                        listParam.Add(new SqlParameter("@freeItem4", rows["freeItem4"].Equals(null) ? DBNull.Value : rows["freeItem4"]));
                        listParam.Add(new SqlParameter("@freeItem5", rows["freeItem5"].Equals(null) ? DBNull.Value : rows["freeItem5"]));
                        listParam.Add(new SqlParameter("@freeItem6", rows["freeItem6"].Equals(null) ? DBNull.Value : rows["freeItem6"]));
                        listParam.Add(new SqlParameter("@freeItem7", rows["freeItem7"].Equals(null) ? DBNull.Value : rows["freeItem7"]));
                        listParam.Add(new SqlParameter("@freeItem8", rows["freeItem8"].Equals(null) ? DBNull.Value : rows["freeItem8"]));
                        listParam.Add(new SqlParameter("@freeItem9", rows["freeItem9"].Equals(null) ? DBNull.Value : rows["freeItem9"]));
                        listParam.Add(new SqlParameter("@memo", rows["memo"].Equals(null) ? DBNull.Value : rows["memo"]));
                        listParam.Add(new SqlParameter("@requiredquantity", rows["requiredquantity"].Equals(null) ? DBNull.Value : rows["requiredquantity"]));
                        listParam.Add(new SqlParameter("@requiredquantity2", rows["requiredquantity2"].Equals(null) ? DBNull.Value : rows["requiredquantity2"]));
                        listParam.Add(new SqlParameter("@rateofexchange", rows["rateofexchange"].Equals(null) ? DBNull.Value : rows["rateofexchange"]));
                        listParam.Add(new SqlParameter("@cost", rows["cost"].Equals(null) ? DBNull.Value : rows["cost"]));
                        listParam.Add(new SqlParameter("@backflushmaterial", rows["backflushmaterial"].Equals(null) ? DBNull.Value : rows["backflushmaterial"]));
                        listParam.Add(new SqlParameter("@defaultchoice", rows["defaultchoice"].Equals(null) ? DBNull.Value : rows["defaultchoice"]));
                        listParam.Add(new SqlParameter("@priuserdefnvc1", rows["priuserdefnvc1"].Equals(null) ? DBNull.Value : rows["priuserdefnvc1"]));
                        listParam.Add(new SqlParameter("@priuserdefnvc2", rows["priuserdefnvc2"].Equals(null) ? DBNull.Value : rows["priuserdefnvc2"]));
                        listParam.Add(new SqlParameter("@priuserdefnvc3", rows["priuserdefnvc3"].Equals(null) ? DBNull.Value : rows["priuserdefnvc3"]));
                        listParam.Add(new SqlParameter("@priuserdefnvc4", rows["priuserdefnvc4"].Equals(null) ? DBNull.Value : rows["priuserdefnvc4"]));
                        listParam.Add(new SqlParameter("@priuserdefdecm1", rows["priuserdefdecm1"].Equals(null) ? DBNull.Value : rows["priuserdefdecm1"]));
                        listParam.Add(new SqlParameter("@priuserdefdecm2", rows["priuserdefdecm2"].Equals(null) ? DBNull.Value : rows["priuserdefdecm2"]));
                        listParam.Add(new SqlParameter("@priuserdefdecm3", rows["priuserdefdecm3"].Equals(null) ? DBNull.Value : rows["priuserdefdecm3"]));
                        listParam.Add(new SqlParameter("@priuserdefdecm4", rows["priuserdefdecm4"].Equals(null) ? DBNull.Value : rows["priuserdefdecm4"]));
                        listParam.Add(new SqlParameter("@pubuserdefnvc1", rows["pubuserdefnvc1"].Equals(null) ? DBNull.Value : rows["pubuserdefnvc1"]));
                        listParam.Add(new SqlParameter("@pubuserdefnvc2", rows["pubuserdefnvc2"].Equals(null) ? DBNull.Value : rows["pubuserdefnvc2"]));
                        listParam.Add(new SqlParameter("@pubuserdefnvc3", rows["pubuserdefnvc3"].Equals(null) ? DBNull.Value : rows["pubuserdefnvc3"]));
                        listParam.Add(new SqlParameter("@pubuserdefnvc4", rows["pubuserdefnvc4"].Equals(null) ? DBNull.Value : rows["pubuserdefnvc4"]));
                        listParam.Add(new SqlParameter("@pubuserdefdecm1", rows["pubuserdefdecm1"].Equals(null) ? DBNull.Value : rows["pubuserdefdecm1"]));
                        listParam.Add(new SqlParameter("@pubuserdefdecm2", rows["pubuserdefdecm2"].Equals(null) ? DBNull.Value : rows["pubuserdefdecm2"]));
                        listParam.Add(new SqlParameter("@pubuserdefdecm3", rows["pubuserdefdecm3"].Equals(null) ? DBNull.Value : rows["pubuserdefdecm3"]));
                        listParam.Add(new SqlParameter("@pubuserdefdecm4", rows["pubuserdefdecm4"].Equals(null) ? DBNull.Value : rows["pubuserdefdecm4"]));
                        listParam.Add(new SqlParameter("@BatchNumber", rows["BatchNumber"].Equals(null) ? DBNull.Value : rows["BatchNumber"]));
                        listParam.Add(new SqlParameter("@FailDate", rows["FailDate"].Equals(null) ? DBNull.Value : rows["FailDate"]));
                        listParam.Add(new SqlParameter("@id", rows["id"].Equals(null) ? DBNull.Value : rows["id"]));
                        listParam.Add(new SqlParameter("@idchildbom", rows["idchildbom"].Equals(null) ? DBNull.Value : rows["idchildbom"]));
                        listParam.Add(new SqlParameter("@idbom", rows["idbom"].Equals(null) ? DBNull.Value : rows["idbom"]));
                        listParam.Add(new SqlParameter("@idBomRelationDTO", rows["idBomRelationDTO"].Equals(null) ? DBNull.Value : rows["idBomRelationDTO"]));
                        listParam.Add(new SqlParameter("@idinventory", rows["idinventory"].Equals(null) ? DBNull.Value : rows["idinventory"]));
                        listParam.Add(new SqlParameter("@idproductprocess", rows["idproductprocess"].Equals(null) ? DBNull.Value : rows["idproductprocess"]));
                        listParam.Add(new SqlParameter("@idunit", rows["idunit"].Equals(null) ? DBNull.Value : rows["idunit"]));
                        listParam.Add(new SqlParameter("@idunit2", rows["idunit2"].Equals(null) ? DBNull.Value : rows["idunit2"]));
                        listParam.Add(new SqlParameter("@idwarehouse", rows["idwarehouse"].Equals(null) ? DBNull.Value : rows["idwarehouse"]));
                        listParam.Add(new SqlParameter("@bomchildattribute", rows["bomchildattribute"].Equals(null) ? DBNull.Value : rows["bomchildattribute"]));
                        listParam.Add(new SqlParameter("@madeDate", rows["madeDate"].Equals(null) ? DBNull.Value : rows["madeDate"]));
                        listParam.Add(new SqlParameter("@updated", rows["updated"].Equals(null) ? DBNull.Value : rows["updated"]));
                        listParam.Add(new SqlParameter("@createdtime", rows["createdtime"].Equals(null) ? DBNull.Value : rows["createdtime"]));

                        string sqls = string.Empty;
                        sqls += "update AA_BOMChild set code=@code,name=@name,produceQuantity=@produceQuantity,rationQuantity=@rationQuantity,";
                        sqls += "wasteRate=@wasteRate,updatedBy=@updatedBy,freeItem0=@freeItem0,freeItem1=@freeItem1,freeItem2=@freeItem2,";
                        sqls += "freeItem3=@freeItem3,freeItem4=@freeItem4,freeItem5=@freeItem5,freeItem6=@freeItem6,freeItem7=@freeItem7,";
                        sqls += "freeItem8=@freeItem8,freeItem9=@freeItem9,memo=@memo,requiredquantity=@requiredquantity,";
                        sqls += "requiredquantity2=@requiredquantity2,rateofexchange=@rateofexchange,cost=@cost,backflushmaterial=@backflushmaterial,";
                        sqls += "defaultchoice=@defaultchoice,priuserdefnvc1=@priuserdefnvc1,priuserdefnvc2=@priuserdefnvc2,priuserdefnvc3=@priuserdefnvc3,";
                        sqls += "priuserdefnvc4=@priuserdefnvc4,priuserdefdecm1=@priuserdefdecm1,priuserdefdecm2=@priuserdefdecm2,priuserdefdecm3=@priuserdefdecm3,";
                        sqls += "priuserdefdecm4=@priuserdefdecm4,pubuserdefnvc1=@pubuserdefnvc1,pubuserdefnvc2=@pubuserdefnvc2,pubuserdefnvc3=@pubuserdefnvc3,";
                        sqls += "pubuserdefnvc4=@pubuserdefnvc4,pubuserdefdecm1=@pubuserdefdecm1,pubuserdefdecm2=@pubuserdefdecm2,pubuserdefdecm3=@pubuserdefdecm3,";
                        sqls += "pubuserdefdecm4=@pubuserdefdecm4,BatchNumber=@BatchNumber,FailDate=@FailDate,idchildbom=@idchildbom,";
                        sqls += "idbom=@idbom,idBomRelationDTO=@idBomRelationDTO,idinventory=@idinventory,idproductprocess=@idproductprocess,";
                        sqls += "idunit=@idunit,idunit2=@idunit2,idwarehouse=@idwarehouse,bomchildattribute=@bomchildattribute,madeDate=@madeDate,updated=@updated,createdtime=@createdtime";
                        sqls += " where id=@id";
                        int count=PDADBAccess.ExecTransSql(sqls,listParam.ToArray());
                        if (count>0)
                        {
                            SyncHelper.UpdateSyncList(row["BusinessID"].ToString(), row["SyncType"].ToString(), row["SyncType"].ToString(), 1.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                    }
                    catch (Exception ex)
                    {
                        SyncHelper.InsertLog(row["id"].ToString(), row["code"].ToString(), "AA_BOMChild", "Update", "数据同步", "0",
                                                "#" + ex.Message + "#" + ex.Source + "#" + ex.StackTrace.Trim() + "#" + ex.TargetSite);
                        return;
                    }
                }
            }
        }
    }
}
