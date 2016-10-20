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
    public class SyncBom
    {
        public void DoSync(string ID)
        {
            string sql = string.Format("select * from NeedSyncList where ID={0}", ID);
            DataTable dt = new DataTable();
            try
            {
                dt = DBAccess.QueryDataTable("ConnectionString", sql,null);
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
            string sql = string.Format("select * from AA_BOM where id={0} and code='{1}'",row["BusinessID"],row["BusinessCode"]);
            DataTable dt = MainDBAccess.QueryDataTable(sql);
            int count=0;
            if (dt.Rows.Count>0)
            {
                foreach (DataRow rows in dt.Rows)
                {
                    try
                    {
                        IList<SqlParameter> listParam = new List<SqlParameter>();
                        listParam.Add(new SqlParameter("@code", rows["code"].Equals(null) ? DBNull.Value : rows["code"]));
                        listParam.Add(new SqlParameter("@name", rows["name"].Equals(null) ? DBNull.Value : rows["name"]));
                        listParam.Add(new SqlParameter("@version", rows["version"].Equals(null) ? DBNull.Value : rows["version"]));
                        listParam.Add(new SqlParameter("@produceQuantity", rows["produceQuantity"].Equals(null) ? DBNull.Value : rows["produceQuantity"]));
                        listParam.Add(new SqlParameter("@rationWage", rows["rationWage"].Equals(null) ? DBNull.Value : rows["rationWage"]));
                        listParam.Add(new SqlParameter("@rationCharge", rows["rationCharge"].Equals(null) ? DBNull.Value : rows["rationCharge"]));
                        listParam.Add(new SqlParameter("@rationManHour", rows["rationManHour"].Equals(null) ? DBNull.Value : rows["rationManHour"]));
                        listParam.Add(new SqlParameter("@isDefaultBom", rows["isDefaultBom"].Equals(null) ? DBNull.Value : rows["isDefaultBom"]));
                        listParam.Add(new SqlParameter("@bomDepth", rows["bomDepth"].Equals(null) ? DBNull.Value : rows["bomDepth"]));
                        listParam.Add(new SqlParameter("@disabled", rows["disabled"].Equals(null) ? DBNull.Value : rows["disabled"]));
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
                        listParam.Add(new SqlParameter("@priuserdefnvc1", rows["priuserdefnvc1"].Equals(null) ? DBNull.Value : rows["priuserdefnvc1"]));
                        listParam.Add(new SqlParameter("@priuserdefdecm1", rows["priuserdefdecm1"].Equals(null) ? DBNull.Value : rows["priuserdefdecm1"]));
                        listParam.Add(new SqlParameter("@priuserdefnvc2", rows["priuserdefnvc2"].Equals(null) ? DBNull.Value : rows["priuserdefnvc2"]));
                        listParam.Add(new SqlParameter("@priuserdefdecm2", rows["priuserdefdecm2"].Equals(null) ? DBNull.Value : rows["priuserdefdecm2"]));
                        listParam.Add(new SqlParameter("@priuserdefnvc3", rows["priuserdefnvc3"].Equals(null) ? DBNull.Value : rows["priuserdefnvc3"]));
                        listParam.Add(new SqlParameter("@priuserdefdecm3", rows["priuserdefdecm3"].Equals(null) ? DBNull.Value : rows["priuserdefdecm3"]));
                        listParam.Add(new SqlParameter("@priuserdefnvc4", rows["priuserdefnvc4"].Equals(null) ? DBNull.Value : rows["priuserdefnvc4"]));
                        listParam.Add(new SqlParameter("@priuserdefdecm4", rows["priuserdefdecm4"].Equals(null) ? DBNull.Value : rows["priuserdefdecm4"]));
                        listParam.Add(new SqlParameter("@priuserdefnvc5", rows["priuserdefnvc5"].Equals(null) ? DBNull.Value : rows["priuserdefnvc5"]));
                        listParam.Add(new SqlParameter("@priuserdefdecm5", rows["priuserdefdecm5"].Equals(null) ? DBNull.Value : rows["priuserdefdecm5"]));
                        listParam.Add(new SqlParameter("@producequantity2", rows["producequantity2"].Equals(null) ? DBNull.Value : rows["producequantity2"]));
                        listParam.Add(new SqlParameter("@rationcost", rows["rationcost"].Equals(null) ? DBNull.Value : rows["rationcost"]));
                        listParam.Add(new SqlParameter("@rationmaterial", rows["rationmaterial"].Equals(null) ? DBNull.Value : rows["rationmaterial"]));
                        listParam.Add(new SqlParameter("@cost", rows["cost"].Equals(null) ? DBNull.Value : rows["cost"]));
                        listParam.Add(new SqlParameter("@rateofexchange", rows["rateofexchange"].Equals(null) ? DBNull.Value : rows["rateofexchange"]));
                        listParam.Add(new SqlParameter("@yieldrate", rows["yieldrate"].Equals(null) ? DBNull.Value : rows["yieldrate"]));
                        listParam.Add(new SqlParameter("@iscostbom", rows["iscostbom"].Equals(null) ? DBNull.Value : rows["iscostbom"]));
                        listParam.Add(new SqlParameter("@idsaleorder", rows["idsaleorder"].Equals(null) ? DBNull.Value : rows["idsaleorder"]));
                        listParam.Add(new SqlParameter("@maker", rows["maker"].Equals(null) ? DBNull.Value : rows["maker"]));
                        listParam.Add(new SqlParameter("@auditor", rows["auditor"].Equals(null) ? DBNull.Value : rows["auditor"]));
                        listParam.Add(new SqlParameter("@reviser", rows["reviser"].Equals(null) ? DBNull.Value : rows["reviser"]));
                        listParam.Add(new SqlParameter("@isorderbom", rows["isorderbom"].Equals(null) ? DBNull.Value : rows["isorderbom"]));
                        listParam.Add(new SqlParameter("@customerName", rows["customerName"].Equals(null) ? DBNull.Value : rows["customerName"]));
                        listParam.Add(new SqlParameter("@customerCode", rows["customerCode"].Equals(null) ? DBNull.Value : rows["customerCode"]));
                        listParam.Add(new SqlParameter("@customerForShort", rows["customerForShort"].Equals(null) ? DBNull.Value : rows["customerForShort"]));
                        listParam.Add(new SqlParameter("@pubuserdefnvc1", rows["pubuserdefnvc1"].Equals(null) ? DBNull.Value : rows["pubuserdefnvc1"]));
                        listParam.Add(new SqlParameter("@pubuserdefnvc2", rows["pubuserdefnvc2"].Equals(null) ? DBNull.Value : rows["pubuserdefnvc2"]));
                        listParam.Add(new SqlParameter("@pubuserdefnvc3", rows["pubuserdefnvc3"].Equals(null) ? DBNull.Value : rows["pubuserdefnvc3"]));
                        listParam.Add(new SqlParameter("@pubuserdefnvc4", rows["pubuserdefnvc4"].Equals(null) ? DBNull.Value : rows["pubuserdefnvc4"]));
                        listParam.Add(new SqlParameter("@pubuserdefnvc5", rows["pubuserdefnvc5"].Equals(null) ? DBNull.Value : rows["pubuserdefnvc5"]));
                        listParam.Add(new SqlParameter("@pubuserdefnvc6", rows["pubuserdefnvc6"].Equals(null) ? DBNull.Value : rows["pubuserdefnvc6"]));
                        listParam.Add(new SqlParameter("@pubuserdefdecm1", rows["pubuserdefdecm1"].Equals(null) ? DBNull.Value : rows["pubuserdefdecm1"]));
                        listParam.Add(new SqlParameter("@pubuserdefdecm2", rows["pubuserdefdecm2"].Equals(null) ? DBNull.Value : rows["pubuserdefdecm2"]));
                        listParam.Add(new SqlParameter("@pubuserdefdecm3", rows["pubuserdefdecm3"].Equals(null) ? DBNull.Value : rows["pubuserdefdecm3"]));
                        listParam.Add(new SqlParameter("@pubuserdefdecm4", rows["pubuserdefdecm4"].Equals(null) ? DBNull.Value : rows["pubuserdefdecm4"]));
                        listParam.Add(new SqlParameter("@pubuserdefdecm5", rows["pubuserdefdecm5"].Equals(null) ? DBNull.Value : rows["pubuserdefdecm5"]));
                        listParam.Add(new SqlParameter("@pubuserdefdecm6", rows["pubuserdefdecm6"].Equals(null) ? DBNull.Value : rows["pubuserdefdecm6"]));
                        listParam.Add(new SqlParameter("@priuserdefdecm6", rows["priuserdefdecm6"].Equals(null) ? DBNull.Value : rows["priuserdefdecm6"]));
                        listParam.Add(new SqlParameter("@priuserdefnvc6", rows["priuserdefnvc6"].Equals(null) ? DBNull.Value : rows["priuserdefnvc6"]));
                        //listParam.Add(new SqlParameter("@id", rows["id"].Equals(null) ? DBNull.Value : rows["id"]));
                        listParam.Add(new SqlParameter("@idBomRelationDTO", rows["idBomRelationDTO"].Equals(null) ? DBNull.Value : rows["idBomRelationDTO"]));
                        listParam.Add(new SqlParameter("@idmanufactureplant", rows["idmanufactureplant"].Equals(null) ? DBNull.Value : rows["idmanufactureplant"]));
                        listParam.Add(new SqlParameter("@idinventory", rows["idinventory"].Equals(null) ? DBNull.Value : rows["idinventory"]));
                        listParam.Add(new SqlParameter("@idmechiner", rows["idmechiner"].Equals(null) ? DBNull.Value : rows["idmechiner"]));
                        listParam.Add(new SqlParameter("@idrouting", rows["idrouting"].Equals(null) ? DBNull.Value : rows["idrouting"]));
                        listParam.Add(new SqlParameter("@idunit", rows["idunit"].Equals(null) ? DBNull.Value : rows["idunit"]));
                        listParam.Add(new SqlParameter("@idunit2", rows["idunit2"].Equals(null) ? DBNull.Value : rows["idunit2"]));
                        listParam.Add(new SqlParameter("@idwarehouse", rows["idwarehouse"].Equals(null) ? DBNull.Value : rows["idwarehouse"]));
                        listParam.Add(new SqlParameter("@getbomchildunitmethod", rows["getbomchildunitmethod"].Equals(null) ? DBNull.Value : rows["getbomchildunitmethod"]));
                        listParam.Add(new SqlParameter("@produceType", rows["produceType"].Equals(null) ? DBNull.Value : rows["produceType"]));
                        listParam.Add(new SqlParameter("@voucherstate", rows["voucherstate"].Equals(null) ? DBNull.Value : rows["voucherstate"]));
                        listParam.Add(new SqlParameter("@auditorid", rows["auditorid"].Equals(null) ? DBNull.Value : rows["auditorid"]));
                        listParam.Add(new SqlParameter("@makerid", rows["makerid"].Equals(null) ? DBNull.Value : rows["makerid"]));
                        listParam.Add(new SqlParameter("@madeDate", rows["madeDate"].Equals(null) ? DBNull.Value : rows["madeDate"]));
                        listParam.Add(new SqlParameter("@updated", rows["updated"].Equals(null) ? DBNull.Value : rows["updated"]));
                        listParam.Add(new SqlParameter("@voucherdate", rows["voucherdate"].Equals(null) ? DBNull.Value : rows["voucherdate"]));
                        listParam.Add(new SqlParameter("@auditeddate", rows["auditeddate"].Equals(null) ? DBNull.Value : rows["auditeddate"]));
                        listParam.Add(new SqlParameter("@createdtime", rows["createdtime"].Equals(null) ? DBNull.Value : rows["createdtime"]));
                        listParam.Add(new SqlParameter("@reviserid", rows["reviserid"].Equals(null) ? DBNull.Value : rows["reviserid"]));
                        listParam.Add(new SqlParameter("@ReviserDate", rows["ReviserDate"].Equals(null) ? DBNull.Value : rows["ReviserDate"]));
                        string sqls = string.Empty;
                        sqls += "insert into AA_BOM(code,name,version,produceQuantity, ";
                        sqls += "rationWage,rationCharge,rationManHour,isDefaultBom,bomDepth,";
                        sqls += "disabled,updatedBy,freeItem0,freeItem1,freeItem2,";
                        sqls += "freeItem3,freeItem4,freeItem5,freeItem6,freeItem7,freeItem8,freeItem9,";
                        sqls += "priuserdefnvc1,priuserdefdecm1,priuserdefnvc2,priuserdefdecm2,priuserdefnvc3,";
                        sqls += "priuserdefdecm3,priuserdefnvc4,priuserdefdecm4,priuserdefnvc5,priuserdefdecm5,";
                        sqls += "producequantity2,rationcost,rateofexchange,yieldrate,iscostbom,";
                        sqls += "idsaleorder,maker,auditor,reviser,isorderbom,customerName,customerCode,";
                        sqls += "customerForShort,pubuserdefnvc1,pubuserdefnvc2,pubuserdefnvc3,pubuserdefnvc4,pubuserdefnvc5,";
                        sqls += "pubuserdefnvc6,priuserdefdecm6,priuserdefnvc6,idBomRelationDTO,idmanufactureplant,";
                        sqls += "idinventory,idmechiner,idrouting,idunit,idunit2,";
                        sqls += "idwarehouse,getbomchildunitmethod,produceType,voucherstate,auditorid,";
                        sqls += "makerid,madeDate,updated,voucherdate,auditeddate,";
                        sqls += "createdtime,reviserid,ReviserDate)";
                        sqls += " values(@code,@name,@version,@produceQuantity,";
                        sqls += "@rationWage,@rationCharge,@rationManHour,@isDefaultBom,@bomDepth,";
                        sqls += "@disabled,@updatedBy,@freeItem0,@freeItem1,@freeItem2,";
                        sqls += "@freeItem3,@freeItem4,@freeItem5,@freeItem6,@freeItem7,@freeItem8,@freeItem9,";
                        sqls += "@priuserdefnvc1,@priuserdefdecm1,@priuserdefnvc2,@priuserdefdecm2,@priuserdefnvc3,";
                        sqls += "@priuserdefdecm3,@priuserdefnvc4,@priuserdefdecm4,@priuserdefnvc5,@priuserdefdecm5,";
                        sqls += "@producequantity2,@rationcost,@rateofexchange,@yieldrate,@iscostbom,";
                        sqls += "@idsaleorder,@maker,@auditor,@reviser,@isorderbom,@customerName,@customerCode,";
                        sqls += "@customerForShort,@pubuserdefnvc1,@pubuserdefnvc2,@pubuserdefnvc3,@pubuserdefnvc4,@pubuserdefnvc5,";
                        sqls += "@pubuserdefnvc6,@priuserdefdecm6,@priuserdefnvc6,@idBomRelationDTO,@idmanufactureplant,";
                        sqls += "@idinventory,@idmechiner,@idrouting,@idunit,@idunit2,";
                        sqls += "@idwarehouse,@getbomchildunitmethod,@produceType,@voucherstate,@auditorid,";
                        sqls += "@makerid,@madeDate,@updated,@voucherdate,@auditeddate,";
                        sqls += "@createdtime,@reviserid,@ReviserDate)";
                        count+=PDADBAccess.ExecTransSql(sqls, listParam.ToArray());
                        if (count>0)
                        {
                            SyncHelper.UpdateSyncList(row["BusinessID"].ToString(),row["BusinessType"].ToString(),row["SyncType"].ToString(),1.ToString(),DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                        }
                    }
                    catch (Exception ex)
                    {
                        SyncHelper.InsertLog(row["id"].ToString(), row["code"].ToString(), "AA_Bom", "Insert", "数据同步", "0",
                        "#" + ex.Message + "#" + ex.Source + "#" + ex.StackTrace.Trim() + "#" + ex.TargetSite);
                        return;
                    }

                }
            }
        }
        private void DoUpdate(DataRow row)
        {
            try
            {
                string sql = string.Format("select * from AA_BOM where id={0} and code='{1}'", row["BusinessID"], row["BusinessCode"]);
                DataTable dt = MainDBAccess.QueryDataTable(sql);
                int count = 0;
                if (dt.Rows.Count>0)
                {
                    foreach (DataRow rows in dt.Rows)
                    {
                        try
                        {
                            IList<SqlParameter> listParam = new List<SqlParameter>();
                            listParam.Add(new SqlParameter("@code", rows["code"].Equals(null) ? DBNull.Value : rows["code"]));
                            listParam.Add(new SqlParameter("@name", rows["name"].Equals(null) ? DBNull.Value : rows["name"]));
                            listParam.Add(new SqlParameter("@version", rows["version"].Equals(null) ? DBNull.Value : rows["version"]));
                            listParam.Add(new SqlParameter("@produceQuantity", rows["produceQuantity"].Equals(null) ? DBNull.Value : rows["produceQuantity"]));
                            listParam.Add(new SqlParameter("@rationWage", rows["rationWage"].Equals(null) ? DBNull.Value : rows["rationWage"]));
                            listParam.Add(new SqlParameter("@rationCharge", rows["rationCharge"].Equals(null) ? DBNull.Value : rows["rationCharge"]));
                            listParam.Add(new SqlParameter("@rationManHour", rows["rationManHour"].Equals(null) ? DBNull.Value : rows["rationManHour"]));
                            listParam.Add(new SqlParameter("@isDefaultBom", rows["isDefaultBom"].Equals(null) ? DBNull.Value : rows["isDefaultBom"]));
                            listParam.Add(new SqlParameter("@bomDepth", rows["bomDepth"].Equals(null) ? DBNull.Value : rows["bomDepth"]));
                            listParam.Add(new SqlParameter("@disabled", rows["disabled"].Equals(null) ? DBNull.Value : rows["disabled"]));
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
                            listParam.Add(new SqlParameter("@priuserdefnvc1", rows["priuserdefnvc1"].Equals(null) ? DBNull.Value : rows["priuserdefnvc1"]));
                            listParam.Add(new SqlParameter("@priuserdefdecm1", rows["priuserdefdecm1"].Equals(null) ? DBNull.Value : rows["priuserdefdecm1"]));
                            listParam.Add(new SqlParameter("@priuserdefnvc2", rows["priuserdefnvc2"].Equals(null) ? DBNull.Value : rows["priuserdefnvc2"]));
                            listParam.Add(new SqlParameter("@priuserdefdecm2", rows["priuserdefdecm2"].Equals(null) ? DBNull.Value : rows["priuserdefdecm2"]));
                            listParam.Add(new SqlParameter("@priuserdefnvc3", rows["priuserdefnvc3"].Equals(null) ? DBNull.Value : rows["priuserdefnvc3"]));
                            listParam.Add(new SqlParameter("@priuserdefdecm3", rows["priuserdefdecm3"].Equals(null) ? DBNull.Value : rows["priuserdefdecm3"]));
                            listParam.Add(new SqlParameter("@priuserdefnvc4", rows["priuserdefnvc4"].Equals(null) ? DBNull.Value : rows["priuserdefnvc4"]));
                            listParam.Add(new SqlParameter("@priuserdefdecm4", rows["priuserdefdecm4"].Equals(null) ? DBNull.Value : rows["priuserdefdecm4"]));
                            listParam.Add(new SqlParameter("@priuserdefnvc5", rows["priuserdefnvc5"].Equals(null) ? DBNull.Value : rows["priuserdefnvc5"]));
                            listParam.Add(new SqlParameter("@priuserdefdecm5", rows["priuserdefdecm5"].Equals(null) ? DBNull.Value : rows["priuserdefdecm5"]));
                            listParam.Add(new SqlParameter("@producequantity2", rows["producequantity2"].Equals(null) ? DBNull.Value : rows["producequantity2"]));
                            listParam.Add(new SqlParameter("@rationcost", rows["rationcost"].Equals(null) ? DBNull.Value : rows["rationcost"]));
                            listParam.Add(new SqlParameter("@rationmaterial", rows["rationmaterial"].Equals(null) ? DBNull.Value : rows["rationmaterial"]));
                            listParam.Add(new SqlParameter("@cost", rows["cost"].Equals(null) ? DBNull.Value : rows["cost"]));
                            listParam.Add(new SqlParameter("@rateofexchange", rows["rateofexchange"].Equals(null) ? DBNull.Value : rows["rateofexchange"]));
                            listParam.Add(new SqlParameter("@yieldrate", rows["yieldrate"].Equals(null) ? DBNull.Value : rows["yieldrate"]));
                            listParam.Add(new SqlParameter("@iscostbom", rows["iscostbom"].Equals(null) ? DBNull.Value : rows["iscostbom"]));
                            listParam.Add(new SqlParameter("@idsaleorder", rows["idsaleorder"].Equals(null) ? DBNull.Value : rows["idsaleorder"]));
                            listParam.Add(new SqlParameter("@maker", rows["maker"].Equals(null) ? DBNull.Value : rows["maker"]));
                            listParam.Add(new SqlParameter("@auditor", rows["auditor"].Equals(null) ? DBNull.Value : rows["auditor"]));
                            listParam.Add(new SqlParameter("@reviser", rows["reviser"].Equals(null) ? DBNull.Value : rows["reviser"]));
                            listParam.Add(new SqlParameter("@isorderbom", rows["isorderbom"].Equals(null) ? DBNull.Value : rows["isorderbom"]));
                            listParam.Add(new SqlParameter("@customerName", rows["customerName"].Equals(null) ? DBNull.Value : rows["customerName"]));
                            listParam.Add(new SqlParameter("@customerCode", rows["customerCode"].Equals(null) ? DBNull.Value : rows["customerCode"]));
                            listParam.Add(new SqlParameter("@customerForShort", rows["customerForShort"].Equals(null) ? DBNull.Value : rows["customerForShort"]));
                            listParam.Add(new SqlParameter("@pubuserdefnvc1", rows["pubuserdefnvc1"].Equals(null) ? DBNull.Value : rows["pubuserdefnvc1"]));
                            listParam.Add(new SqlParameter("@pubuserdefnvc2", rows["pubuserdefnvc2"].Equals(null) ? DBNull.Value : rows["pubuserdefnvc2"]));
                            listParam.Add(new SqlParameter("@pubuserdefnvc3", rows["pubuserdefnvc3"].Equals(null) ? DBNull.Value : rows["pubuserdefnvc3"]));
                            listParam.Add(new SqlParameter("@pubuserdefnvc4", rows["pubuserdefnvc4"].Equals(null) ? DBNull.Value : rows["pubuserdefnvc4"]));
                            listParam.Add(new SqlParameter("@pubuserdefnvc5", rows["pubuserdefnvc5"].Equals(null) ? DBNull.Value : rows["pubuserdefnvc5"]));
                            listParam.Add(new SqlParameter("@pubuserdefnvc6", rows["pubuserdefnvc6"].Equals(null) ? DBNull.Value : rows["pubuserdefnvc6"]));
                            listParam.Add(new SqlParameter("@pubuserdefdecm1", rows["pubuserdefdecm1"].Equals(null) ? DBNull.Value : rows["pubuserdefdecm1"]));
                            listParam.Add(new SqlParameter("@pubuserdefdecm2", rows["pubuserdefdecm2"].Equals(null) ? DBNull.Value : rows["pubuserdefdecm2"]));
                            listParam.Add(new SqlParameter("@pubuserdefdecm3", rows["pubuserdefdecm3"].Equals(null) ? DBNull.Value : rows["pubuserdefdecm3"]));
                            listParam.Add(new SqlParameter("@pubuserdefdecm4", rows["pubuserdefdecm4"].Equals(null) ? DBNull.Value : rows["pubuserdefdecm4"]));
                            listParam.Add(new SqlParameter("@pubuserdefdecm5", rows["pubuserdefdecm5"].Equals(null) ? DBNull.Value : rows["pubuserdefdecm5"]));
                            listParam.Add(new SqlParameter("@pubuserdefdecm6", rows["pubuserdefdecm6"].Equals(null) ? DBNull.Value : rows["pubuserdefdecm6"]));
                            listParam.Add(new SqlParameter("@priuserdefdecm6", rows["priuserdefdecm6"].Equals(null) ? DBNull.Value : rows["priuserdefdecm6"]));
                            listParam.Add(new SqlParameter("@priuserdefnvc6", rows["priuserdefnvc6"].Equals(null) ? DBNull.Value : rows["priuserdefnvc6"]));
                            listParam.Add(new SqlParameter("@id", rows["id"].Equals(null) ? DBNull.Value : rows["id"]));
                            listParam.Add(new SqlParameter("@idBomRelationDTO", rows["idBomRelationDTO"].Equals(null) ? DBNull.Value : rows["idBomRelationDTO"]));
                            listParam.Add(new SqlParameter("@idmanufactureplant", rows["idmanufactureplant"].Equals(null) ? DBNull.Value : rows["idmanufactureplant"]));
                            listParam.Add(new SqlParameter("@idinventory", rows["idinventory"].Equals(null) ? DBNull.Value : rows["idinventory"]));
                            listParam.Add(new SqlParameter("@idmechiner", rows["idmechiner"].Equals(null) ? DBNull.Value : rows["idmechiner"]));
                            listParam.Add(new SqlParameter("@idrouting", rows["idrouting"].Equals(null) ? DBNull.Value : rows["idrouting"]));
                            listParam.Add(new SqlParameter("@idunit", rows["idunit"].Equals(null) ? DBNull.Value : rows["idunit"]));
                            listParam.Add(new SqlParameter("@idunit2", rows["idunit2"].Equals(null) ? DBNull.Value : rows["idunit2"]));
                            listParam.Add(new SqlParameter("@idwarehouse", rows["idwarehouse"].Equals(null) ? DBNull.Value : rows["idwarehouse"]));
                            listParam.Add(new SqlParameter("@getbomchildunitmethod", rows["getbomchildunitmethod"].Equals(null) ? DBNull.Value : rows["getbomchildunitmethod"]));
                            listParam.Add(new SqlParameter("@produceType", rows["produceType"].Equals(null) ? DBNull.Value : rows["produceType"]));
                            listParam.Add(new SqlParameter("@voucherstate", rows["voucherstate"].Equals(null) ? DBNull.Value : rows["voucherstate"]));
                            listParam.Add(new SqlParameter("@auditorid", rows["auditorid"].Equals(null) ? DBNull.Value : rows["auditorid"]));
                            listParam.Add(new SqlParameter("@makerid", rows["makerid"].Equals(null) ? DBNull.Value : rows["makerid"]));
                            listParam.Add(new SqlParameter("@madeDate", rows["madeDate"].Equals(null) ? DBNull.Value : rows["madeDate"]));
                            listParam.Add(new SqlParameter("@updated", rows["updated"].Equals(null) ? DBNull.Value : rows["updated"]));
                            listParam.Add(new SqlParameter("@voucherdate", rows["voucherdate"].Equals(null) ? DBNull.Value : rows["voucherdate"]));
                            listParam.Add(new SqlParameter("@auditeddate", rows["auditeddate"].Equals(null) ? DBNull.Value : rows["auditeddate"]));
                            listParam.Add(new SqlParameter("@createdtime", rows["createdtime"].Equals(null) ? DBNull.Value : rows["createdtime"]));
                            listParam.Add(new SqlParameter("@reviserid", rows["reviserid"].Equals(null) ? DBNull.Value : rows["reviserid"]));
                            listParam.Add(new SqlParameter("@ReviserDate", rows["ReviserDate"].Equals(null) ? DBNull.Value : rows["ReviserDate"]));
                            string sqls = "update AA_BOM set code=@code,name=@name,version=@version,produceQuantity=@produceQuantity,";
                            sqls += "rationWage=@rationWage,rationCharge=@rationCharge,rationManHour=@rationManHour,isDefaultBom=@isDefaultBom,";
                            sqls += "bomDepth=@bomDepth,disabled=@disabled,updatedBy=@updatedBy,freeItem0=@freeItem0,";
                            sqls += "freeItem1=@freeItem1,freeItem2=@freeItem2,freeItem3=@freeItem3,freeItem4=@freeItem4,";
                            sqls += "freeItem5=@freeItem5,freeItem6=@freeItem6,freeItem7=@freeItem7,freeItem8=@freeItem8,";
                            sqls += "freeItem9=@freeItem9,priuserdefnvc1=@priuserdefnvc1,priuserdefdecm1=@priuserdefdecm1,";
                            sqls += "priuserdefnvc2=@priuserdefnvc2,priuserdefdecm2=@priuserdefdecm2,priuserdefnvc3=@priuserdefnvc3,";
                            sqls += "priuserdefnvc4=@priuserdefnvc4,priuserdefdecm4=@priuserdefdecm4,priuserdefnvc5=@priuserdefnvc5,";
                            sqls += "producequantity2=@producequantity2,rationcost=@rationcost,rationmaterial=@rationmaterial,";
                            sqls += "cost=@cost,rateofexchange=@rateofexchange,yieldrate=@yieldrate,iscostbom=@iscostbom,";
                            sqls += "idsaleorder=@idsaleorder,maker=@maker,auditor=@auditor,reviser=@reviser,";
                            sqls += "isorderbom=@isorderbom,customerName=@customerName,customerCode=@customerCode,";
                            sqls += "customerForShort=@customerForShort,pubuserdefnvc1=@pubuserdefnvc1,pubuserdefnvc2=@pubuserdefnvc2,";
                            sqls += "pubuserdefnvc3=@pubuserdefnvc3,pubuserdefnvc4=@pubuserdefnvc4,pubuserdefnvc5=@pubuserdefnvc5,";
                            sqls += "pubuserdefnvc6=@pubuserdefnvc6,pubuserdefdecm1=@pubuserdefdecm1,pubuserdefdecm2=@pubuserdefdecm2,";
                            sqls += "pubuserdefdecm3=@pubuserdefdecm3,pubuserdefdecm4=@pubuserdefdecm4,pubuserdefdecm5=@pubuserdefdecm5,";
                            sqls += "pubuserdefdecm6=@pubuserdefdecm6,priuserdefdecm6=@priuserdefdecm6,priuserdefnvc6=@priuserdefnvc6,";
                            sqls += "idBomRelationDTO=@idBomRelationDTO,idmanufactureplant=@idmanufactureplant,idinventory=@idinventory,";
                            sqls += "idmechiner=@idmechiner,idrouting=@idrouting,idunit=@idunit,idunit2=@idunit2,idwarehouse=@idwarehouse,";
                            sqls += "getbomchildunitmethod=@getbomchildunitmethod,produceType=@produceType,voucherstate=@voucherstate,";
                            sqls += "auditorid=@auditorid,makerid=@makerid,madeDate=@madeDate,updated=@updated,voucherdate=@voucherdate,";
                            sqls += "auditeddate=@auditeddate,createdtime=@createdtime,reviserid=@reviserid,ReviserDate=@ReviserDate";
                            sqls += " where id=@id";
                            count += PDADBAccess.ExecTransSql(sqls, listParam.ToArray());
                            if (count > 0)
                            {
                                SyncHelper.UpdateSyncList(row["BusinessID"].ToString(), row["BusinessType"].ToString(), row["SyncType"].ToString(), 1.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                            }
                        }
                        catch (Exception ex)
                        {
                            SyncHelper.InsertLog(row["id"].ToString(), row["code"].ToString(), "AA_Bom", "Update", "数据同步", "0",
                            "#" + ex.Message + "#" + ex.Source + "#" + ex.StackTrace.Trim() + "#" + ex.TargetSite);
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                SyncHelper.InsertLog(row["id"].ToString(), row["code"].ToString(), "AA_Bom", "Update", "数据同步", "0",
                       "#" + ex.Message + "#" + ex.Source + "#" + ex.StackTrace.Trim() + "#" + ex.TargetSite);
                return;
            }
        }
    }
}
