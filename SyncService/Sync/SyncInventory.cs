using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUtility;
using System.Data;
using System.Data.SqlClient;
namespace SyncService.Sync
{
    public class SyncInventory
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
                string tsql = "select * from AA_Inventory where id=@id";
                IList<SqlParameter> tlist = new List<SqlParameter>();
                tlist.Add(new SqlParameter("@id", srow["BusinessID"].Equals(null) ? DBNull.Value : srow["BusinessID"]));
                row = DBAccess.QueryDataTable(MainCon, tsql, tlist.ToArray()).Rows[0];//赋值需要同步的记录
            }
            catch (Exception ex)
            {
                SyncHelper.InsertLog(srow["BusinessID"].ToString(), srow["BusinessCode"].ToString(), srow["BusinessType"].ToString(), srow["SyncType"].ToString(),
                                srow["CodeType"].ToString(), srow["IsFinish"].ToString(), "在正式帐套下未找到ID为" + id + "的AA_Inventory条目");
                return;
            }
            IList<SqlParameter> paramList = new List<SqlParameter>();
            try
            {
                SetSqlParameter(ref paramList, row);
            }
            catch (Exception ex)
            {
                SyncHelper.InsertLog(row["id"].ToString(), row["code"].ToString(), "AA_Inventory", "Insert", "数据同步", "0",
                    "When SyncSystem SyncInventory Do Insert,Some SqlParameter Throw Some Message,The Message Like[" + ex.InnerException + "]");
                return;
            }
            try
            {
                DBAccess.ExecReturnSP(PDACon, "AA_Inventory_Insert", paramList.ToArray());
                SyncHelper.UpdateSyncList(srow["BusinessID"].ToString(),srow["BusinessType"].ToString(),srow["SyncType"].ToString(),1.ToString(),DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch (Exception ex)
            {
                SyncHelper.InsertLog(row["id"].ToString(), row["code"].ToString(), "AA_Inventory", "Insert", "数据同步", "0",
                    "When SyncSystem SyncInventory Do Insert,This Sql Throw Some Message,The Message Like[" + ex.InnerException + "]");
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
                string tsql = "select * from AA_Inventory where id=@id";
                IList<SqlParameter> tlist = new List<SqlParameter>();
                tlist.Add(new SqlParameter("@id", srow["BusinessID"].Equals(null) ? DBNull.Value : srow["BusinessID"]));
                row = DBAccess.QueryDataTable(MainCon, tsql, tlist.ToArray()).Rows[0];//赋值需要同步的记录
            }
            catch (Exception ex)
            {
                SyncHelper.InsertLog(srow["BusinessID"].ToString(), srow["BusinessCode"].ToString(), srow["BusinessType"].ToString(), srow["SyncType"].ToString(),
                                srow["CodeType"].ToString(), srow["IsFinish"].ToString(), "在正式帐套下未找到ID为" + id + "的AA_Inventory条目");
                return;
            }
            IList<SqlParameter> paramList = new List<SqlParameter>();
            try
            {
                SetSqlParameter(ref paramList, row);
            }
            catch (Exception ex)
            {
                SyncHelper.InsertLog(row["id"].ToString(), row["code"].ToString(), "AA_Inventory", "Update", "数据同步", "0",
                    "When SyncSystem SyncInventory Do Update,Some SqlParameter Throw Some Message,The Message Like[" + ex.InnerException + "]");
                return;
            }
            try
            {
                DBAccess.ExecReturnSP(PDACon, "AA_Inventory_Update", paramList.ToArray());
                SyncHelper.UpdateSyncList(srow["BusinessID"].ToString(), srow["BusinessType"].ToString(), srow["SyncType"].ToString(), 1.ToString(), DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch (Exception ex)
            {
                SyncHelper.InsertLog(row["id"].ToString(), row["code"].ToString(), "AA_Inventory", "Insert", "数据同步", "0",
                    "When SyncSystem SyncInventory Do Update,This Sql Throw Some Message,The Message Like[" + ex.InnerException + "]");
                return;
            }
        }
        /// <summary>
        /// Set存储过程参数（除外键类型之外的所有参数都使用此方法）
        /// </summary>
        /// <param name="list">ref 参数列表</param>
        /// <param name="row">具体需要同步的那行数据</param>
        private void SetSqlParameter(ref IList<SqlParameter> list, DataRow row)
        {
            list.Add(new SqlParameter("@code", row["code"]));
            list.Add(new SqlParameter("@name", row["name"]));
            list.Add(new SqlParameter("@shorthand", row["shorthand"]));
            list.Add(new SqlParameter("@specification", row["specification"]));
            list.Add(new SqlParameter("@procureBatch", string.IsNullOrEmpty(row["procureBatch"].ToString()) ? DBNull.Value : row["procureBatch"]));
            list.Add(new SqlParameter("@invSCost", string.IsNullOrEmpty(row["invSCost"].ToString()) ? DBNull.Value : row["invSCost"]));
            list.Add(new SqlParameter("@latestCost", string.IsNullOrEmpty(row["latestCost"].ToString()) ? DBNull.Value : row["latestCost"]));
            list.Add(new SqlParameter("@avagCost", string.IsNullOrEmpty(row["avagCost"].ToString()) ? DBNull.Value : row["avagCost"]));
            list.Add(new SqlParameter("@isLimitedWithdraw", string.IsNullOrEmpty(row["isLimitedWithdraw"].ToString()) ? DBNull.Value : row["isLimitedWithdraw"]));
            list.Add(new SqlParameter("@isBatch", string.IsNullOrEmpty(row["isBatch"].ToString()) ? DBNull.Value : row["isBatch"]));
            list.Add(new SqlParameter("@isQualityPeriod", string.IsNullOrEmpty(row["isQualityPeriod"].ToString()) ? DBNull.Value : row["isQualityPeriod"]));
            list.Add(new SqlParameter("@isSale", string.IsNullOrEmpty(row["isSale"].ToString()) ? DBNull.Value : row["isSale"]));
            list.Add(new SqlParameter("@isMadeSelf", string.IsNullOrEmpty(row["isMadeSelf"].ToString()) ? DBNull.Value : row["isMadeSelf"]));
            list.Add(new SqlParameter("@isPurchase", string.IsNullOrEmpty(row["isPurchase"].ToString()) ? DBNull.Value : row["isPurchase"]));
            list.Add(new SqlParameter("@isMaterial", string.IsNullOrEmpty(row["isMaterial"].ToString()) ? DBNull.Value : row["isMaterial"]));
            list.Add(new SqlParameter("@lowQuantity", string.IsNullOrEmpty(row["lowQuantity"].ToString()) ? DBNull.Value : row["lowQuantity"]));
            list.Add(new SqlParameter("@topQuantity", string.IsNullOrEmpty(row["topQuantity"].ToString()) ? DBNull.Value : row["topQuantity"]));
            list.Add(new SqlParameter("@safeQuantity", string.IsNullOrEmpty(row["safeQuantity"].ToString()) ? DBNull.Value : row["safeQuantity"]));
            list.Add(new SqlParameter("@picture", row["picture"]));
            list.Add(new SqlParameter("@disabled", string.IsNullOrEmpty(row["disabled"].ToString()) ? DBNull.Value : row["disabled"]));
            list.Add(new SqlParameter("@isQualityCheck", string.IsNullOrEmpty(row["isQualityCheck"].ToString()) ? DBNull.Value : row["isQualityCheck"]));
            list.Add(new SqlParameter("@isMadeRequest", string.IsNullOrEmpty(row["isMadeRequest"].ToString()) ? DBNull.Value : row["isMadeRequest"]));
            list.Add(new SqlParameter("@isSingleUnit", string.IsNullOrEmpty(row["isSingleUnit"].ToString()) ? DBNull.Value : row["isSingleUnit"]));
            list.Add(new SqlParameter("@updatedBy", row["updatedBy"]));
            list.Add(new SqlParameter("@Userfreeitem7", string.IsNullOrEmpty(row["Userfreeitem7"].ToString()) ? DBNull.Value : row["Userfreeitem7"]));
            list.Add(new SqlParameter("@Userfreeitem6", string.IsNullOrEmpty(row["Userfreeitem6"].ToString()) ? DBNull.Value : row["Userfreeitem6"]));
            list.Add(new SqlParameter("@Userfreeitem2", string.IsNullOrEmpty(row["Userfreeitem2"].ToString()) ? DBNull.Value : row["Userfreeitem2"]));
            list.Add(new SqlParameter("@Userfreeitem1", string.IsNullOrEmpty(row["Userfreeitem1"].ToString()) ? DBNull.Value : row["Userfreeitem1"]));
            list.Add(new SqlParameter("@Userfreeitem9", string.IsNullOrEmpty(row["Userfreeitem9"].ToString()) ? DBNull.Value : row["Userfreeitem9"]));
            list.Add(new SqlParameter("@Userfreeitem0", string.IsNullOrEmpty(row["Userfreeitem0"].ToString()) ? DBNull.Value : row["Userfreeitem0"]));
            list.Add(new SqlParameter("@Userfreeitem8", string.IsNullOrEmpty(row["Userfreeitem8"].ToString()) ? DBNull.Value : row["Userfreeitem8"]));
            list.Add(new SqlParameter("@Userfreeitem5", string.IsNullOrEmpty(row["Userfreeitem5"].ToString()) ? DBNull.Value : row["Userfreeitem5"]));
            list.Add(new SqlParameter("@Userfreeitem4", string.IsNullOrEmpty(row["Userfreeitem4"].ToString()) ? DBNull.Value : row["Userfreeitem4"]));
            list.Add(new SqlParameter("@Userfreeitem3", string.IsNullOrEmpty(row["Userfreeitem3"].ToString()) ? DBNull.Value : row["Userfreeitem3"]));
            list.Add(new SqlParameter("@MustInputFreeitem7", string.IsNullOrEmpty(row["MustInputFreeitem7"].ToString()) ? DBNull.Value : row["MustInputFreeitem7"]));
            list.Add(new SqlParameter("@MustInputFreeitem2", string.IsNullOrEmpty(row["MustInputFreeitem2"].ToString()) ? DBNull.Value : row["MustInputFreeitem2"]));
            list.Add(new SqlParameter("@MustInputFreeitem6", string.IsNullOrEmpty(row["MustInputFreeitem6"].ToString()) ? DBNull.Value : row["MustInputFreeitem6"]));
            list.Add(new SqlParameter("@MustInputFreeitem3", string.IsNullOrEmpty(row["MustInputFreeitem3"].ToString()) ? DBNull.Value : row["MustInputFreeitem3"]));
            list.Add(new SqlParameter("@MustInputFreeitem5", string.IsNullOrEmpty(row["MustInputFreeitem5"].ToString()) ? DBNull.Value : row["MustInputFreeitem5"]));
            list.Add(new SqlParameter("@MustInputFreeitem4", string.IsNullOrEmpty(row["MustInputFreeitem4"].ToString()) ? DBNull.Value : row["MustInputFreeitem4"]));
            list.Add(new SqlParameter("@MustInputFreeitem9", string.IsNullOrEmpty(row["MustInputFreeitem9"].ToString()) ? DBNull.Value : row["MustInputFreeitem9"]));
            list.Add(new SqlParameter("@MustInputFreeitem1", string.IsNullOrEmpty(row["MustInputFreeitem1"].ToString()) ? DBNull.Value : row["MustInputFreeitem1"]));
            list.Add(new SqlParameter("@MustInputFreeitem8", string.IsNullOrEmpty(row["MustInputFreeitem8"].ToString()) ? DBNull.Value : row["MustInputFreeitem8"]));
            list.Add(new SqlParameter("@MustInputFreeitem0", string.IsNullOrEmpty(row["MustInputFreeitem0"].ToString()) ? DBNull.Value : row["MustInputFreeitem0"]));
            list.Add(new SqlParameter("@produceBatch", string.IsNullOrEmpty(row["produceBatch"].ToString()) ? DBNull.Value : row["produceBatch"]));
            list.Add(new SqlParameter("@imageFile", row["imageFile"]));
            list.Add(new SqlParameter("@priuserdefnvc1", row["priuserdefnvc1"]));
            list.Add(new SqlParameter("@priuserdefdecm1", string.IsNullOrEmpty(row["priuserdefdecm1"].ToString()) ? DBNull.Value : row["priuserdefdecm1"]));
            list.Add(new SqlParameter("@priuserdefnvc2", row["priuserdefnvc2"]));
            list.Add(new SqlParameter("@priuserdefdecm2", string.IsNullOrEmpty(row["priuserdefdecm2"].ToString()) ? DBNull.Value : row["priuserdefdecm2"]));
            list.Add(new SqlParameter("@priuserdefnvc3", row["priuserdefnvc3"]));
            list.Add(new SqlParameter("@priuserdefdecm3", string.IsNullOrEmpty(row["priuserdefdecm3"].ToString()) ? DBNull.Value : row["priuserdefdecm3"]));
            list.Add(new SqlParameter("@priuserdefnvc4", row["priuserdefnvc4"]));
            list.Add(new SqlParameter("@priuserdefdecm4", string.IsNullOrEmpty(row["priuserdefdecm4"].ToString()) ? DBNull.Value : row["priuserdefdecm4"]));
            list.Add(new SqlParameter("@priuserdefnvc5", row["priuserdefnvc5"]));
            list.Add(new SqlParameter("@priuserdefdecm5", string.IsNullOrEmpty(row["priuserdefdecm5"].ToString()) ? DBNull.Value : row["priuserdefdecm5"]));
            list.Add(new SqlParameter("@standardturnoverdays", string.IsNullOrEmpty(row["standardturnoverdays"].ToString()) ? DBNull.Value : row["standardturnoverdays"]));
            list.Add(new SqlParameter("@HasEverChanged", row["HasEverChanged"]));
            list.Add(new SqlParameter("@pickbatch", string.IsNullOrEmpty(row["pickbatch"].ToString()) ? DBNull.Value : row["pickbatch"]));
            list.Add(new SqlParameter("@isphantom", string.IsNullOrEmpty(row["isphantom"].ToString()) ? DBNull.Value : row["isphantom"]));
            list.Add(new SqlParameter("@ControlRangeFreeitem0", string.IsNullOrEmpty(row["ControlRangeFreeitem0"].ToString()) ? DBNull.Value : row["ControlRangeFreeitem0"]));
            list.Add(new SqlParameter("@ControlRangeFreeitem1", string.IsNullOrEmpty(row["ControlRangeFreeitem1"].ToString()) ? DBNull.Value : row["ControlRangeFreeitem1"]));
            list.Add(new SqlParameter("@ControlRangeFreeitem2", string.IsNullOrEmpty(row["ControlRangeFreeitem2"].ToString()) ? DBNull.Value : row["ControlRangeFreeitem2"]));
            list.Add(new SqlParameter("@ControlRangeFreeitem3", string.IsNullOrEmpty(row["ControlRangeFreeitem3"].ToString()) ? DBNull.Value : row["ControlRangeFreeitem3"]));
            list.Add(new SqlParameter("@ControlRangeFreeitem4", string.IsNullOrEmpty(row["ControlRangeFreeitem4"].ToString()) ? DBNull.Value : row["ControlRangeFreeitem4"]));
            list.Add(new SqlParameter("@ControlRangeFreeitem5", string.IsNullOrEmpty(row["ControlRangeFreeitem5"].ToString()) ? DBNull.Value : row["ControlRangeFreeitem5"]));
            list.Add(new SqlParameter("@ControlRangeFreeitem6", string.IsNullOrEmpty(row["ControlRangeFreeitem6"].ToString()) ? DBNull.Value : row["ControlRangeFreeitem6"]));
            list.Add(new SqlParameter("@ControlRangeFreeitem7", string.IsNullOrEmpty(row["ControlRangeFreeitem7"].ToString()) ? DBNull.Value : row["ControlRangeFreeitem7"]));
            list.Add(new SqlParameter("@ControlRangeFreeitem8", string.IsNullOrEmpty(row["ControlRangeFreeitem8"].ToString()) ? DBNull.Value : row["ControlRangeFreeitem8"]));
            list.Add(new SqlParameter("@ControlRangeFreeitem9", string.IsNullOrEmpty(row["ControlRangeFreeitem9"].ToString()) ? DBNull.Value : row["ControlRangeFreeitem9"]));
            list.Add(new SqlParameter("@IsLaborCost", string.IsNullOrEmpty(row["IsLaborCost"].ToString()) ? DBNull.Value : row["IsLaborCost"]));
            list.Add(new SqlParameter("@BatchRunNumber", string.IsNullOrEmpty(row["BatchRunNumber"].ToString()) ? DBNull.Value : row["BatchRunNumber"]));
            list.Add(new SqlParameter("@IsNew", string.IsNullOrEmpty(row["IsNew"].ToString()) ? DBNull.Value : row["IsNew"]));
            list.Add(new SqlParameter("@MadeRecordDate", row["MadeRecordDate"]));
            list.Add(new SqlParameter("@InventoryDescript", row["InventoryDescript"]));
            list.Add(new SqlParameter("@ReNewGoodSellDays", string.IsNullOrEmpty(row["ReNewGoodSellDays"].ToString()) ? DBNull.Value : row["ReNewGoodSellDays"]));
            list.Add(new SqlParameter("@ReNewGoodAheadDays", string.IsNullOrEmpty(row["ReNewGoodAheadDays"].ToString()) ? DBNull.Value : row["ReNewGoodAheadDays"]));
            list.Add(new SqlParameter("@IsSuite", string.IsNullOrEmpty(row["IsSuite"].ToString()) ? DBNull.Value : row["IsSuite"]));
            list.Add(new SqlParameter("@IsWeigh", string.IsNullOrEmpty(row["IsWeigh"].ToString()) ? DBNull.Value : row["IsWeigh"]));
            list.Add(new SqlParameter("@DefaultBarCode", row["DefaultBarCode"]));
            list.Add(new SqlParameter("@NewProductPeriod", string.IsNullOrEmpty(row["NewProductPeriod"].ToString()) ? DBNull.Value : row["NewProductPeriod"]));
            list.Add(new SqlParameter("@Expired", string.IsNullOrEmpty(row["Expired"].ToString()) ? DBNull.Value : row["Expired"]));
            //list.Add(new SqlParameter("@ExternalCode", row["ExternalCode"]));
            list.Add(new SqlParameter("@idbarcodesolution", string.IsNullOrEmpty(row["idbarcodesolution"].ToString()) ? DBNull.Value : row["idbarcodesolution"]));
            list.Add(new SqlParameter("@idinvlocation", string.IsNullOrEmpty(row["idinvlocation"].ToString()) ? DBNull.Value : row["idinvlocation"]));
            list.Add(new SqlParameter("@idpartner", string.IsNullOrEmpty(row["idpartner"].ToString()) ? DBNull.Value : row["idpartner"]));
            list.Add(new SqlParameter("@idunitgroup", string.IsNullOrEmpty(row["idunitgroup"].ToString()) ? DBNull.Value : row["idunitgroup"]));
            list.Add(new SqlParameter("@idSubUnitByReport", string.IsNullOrEmpty(row["idSubUnitByReport"].ToString()) ? DBNull.Value : row["idSubUnitByReport"]));
            list.Add(new SqlParameter("@ExpiredUnit", string.IsNullOrEmpty(row["ExpiredUnit"].ToString()) ? DBNull.Value : row["ExpiredUnit"]));
            list.Add(new SqlParameter("@idwarehouse", string.IsNullOrEmpty(row["idwarehouse"].ToString()) ? DBNull.Value : row["idwarehouse"]));
            list.Add(new SqlParameter("@customerReplenishmentRule", string.IsNullOrEmpty(row["customerReplenishmentRule"].ToString()) ? DBNull.Value : row["customerReplenishmentRule"]));
            list.Add(new SqlParameter("@pickbatchmethod", string.IsNullOrEmpty(row["pickbatchmethod"].ToString()) ? DBNull.Value : row["pickbatchmethod"]));
            list.Add(new SqlParameter("@planattribute", string.IsNullOrEmpty(row["planattribute"].ToString()) ? DBNull.Value : row["planattribute"]));
            list.Add(new SqlParameter("@productInfo", string.IsNullOrEmpty(row["productInfo"].ToString()) ? DBNull.Value : row["productInfo"]));
            list.Add(new SqlParameter("@storeReplenishmentRule", string.IsNullOrEmpty(row["storeReplenishmentRule"].ToString()) ? DBNull.Value : row["storeReplenishmentRule"]));
            list.Add(new SqlParameter("@taxRate", string.IsNullOrEmpty(row["taxRate"].ToString()) ? DBNull.Value : row["taxRate"]));
            list.Add(new SqlParameter("@unittype", string.IsNullOrEmpty(row["unittype"].ToString()) ? DBNull.Value : row["unittype"]));
            list.Add(new SqlParameter("@valueType", string.IsNullOrEmpty(row["valueType"].ToString()) ? DBNull.Value : row["valueType"]));
            list.Add(new SqlParameter("@madeDate", row["madeDate"]));
            list.Add(new SqlParameter("@updated", row["updated"]));
            list.Add(new SqlParameter("@createdTime", row["createdTime"]));
            list.Add(new SqlParameter("@Creater", row["Creater"]));
            list.Add(new SqlParameter("@Changer", row["Changer"]));
            list.Add(new SqlParameter("@Changedate", row["Changedate"]));
        }
    }
}
