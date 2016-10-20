using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using DBUtility;
using SyncService.Sync;
namespace SyncService
{
    public partial class Service1 : ServiceBase
    {
        SyncBom bom = new SyncBom();
        SyncBomChild bomchild = new SyncBomChild();
        SyncInventory syncinventory = new SyncInventory();
        SyncInventoryClass inventoryclass = new SyncInventoryClass();
        public Service1()
        {
            InitializeComponent();
        }
        public void OnStart()
        {
            string sql = "select * from NeedSyncList where IsSync=0";
            DataTable dt = new DataTable();
            string str = "";
            try
            {
                dt = DBAccess.QueryDataTable("ConnectionString",sql,null);
            }
            catch (Exception ex)
            {
                return;
            }
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    switch (row["BusinessType"].ToString())
                    {
                        case "AA_BOM":
                            bom.DoSync(row["id"].ToString());
                            break;
                        case "AA_BOMChild":
                            bomchild.DoSync(row["id"].ToString());
                            break;
                        case "AA_Inventory":
                            syncinventory.DoSync(row["id"].ToString());
                            break;
                        case "AA_InventoryClass":
                            inventoryclass.DoSync(row["id"].ToString());
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        //protected override void OnStart(string[] args)
        //{
        //    //AllWaysRun();
        //}
        /// <summary>
        /// 设置定时器方法
        /// </summary>
        private void AllWaysRun()
        {
            System.Timers.Timer mt = new System.Timers.Timer(60000);
            mt.Elapsed += new System.Timers.ElapsedEventHandler(MTimedEvent);
            mt.Enabled = true;
        }
        /// <summary>
        /// 定时器方法体（同步功能的主入口）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MTimedEvent(object sender, ElapsedEventArgs e)
        {
            string sql = "select * from NeedSyncList where IsSync=0";
            DataTable dt = new DataTable();
            try
            {
                dt = DBAccess.QueryDataTable(sql);
            }
            catch (Exception ex)
            {
                return;
            }
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    switch (row["BusinessType"].ToString())
                    {
                        case "AA_BOM":
                            bom.DoSync(row["id"].ToString());
                            break;
                        case "AA_BOMChild":
                            bomchild.DoSync(row["id"].ToString());
                            break;
                        case "AA_Inventory":
                            syncinventory.DoSync(row["id"].ToString());
                            break;
                        case "AA_InventoryClass":
                            inventoryclass.DoSync(row.ToString());
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        protected override void OnStop()
        {
            this.Dispose();

        }
    }
}
