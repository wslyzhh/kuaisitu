using MettingSys.Common;
using MettingSys.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MettingSys.DAL
{
    public partial class statisticDAL
    {
        public statisticDAL() { }

        /// <summary>
        /// 员工业绩统计
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataSet getAchievementStatisticData(Dictionary<string, string> dict, int pageSize, int pageIndex, string filedOrder, out int recordCount,out int tCount,out decimal tOrderShou,out decimal tUnIncome, out decimal tOrderFu,out decimal tUnCost, out decimal tOrderTicheng, out decimal tCust,out decimal tProfit1, out decimal tProfit2, bool isPage = true)
        {
            StringBuilder strWhere1 = new StringBuilder();
            StringBuilder strWhere2 = new StringBuilder();
            string custFiled = "";
            if (dict != null)
            {
                if (dict.ContainsKey("smonth"))
                {
                    if (dict["smonth"].Length == 7)
                    {
                        strWhere1.Append(" and datediff(MONTH,o_edate,'" + dict["smonth"] + "-01')<=0 ");
                    }
                    else
                    {
                        strWhere1.Append(" and datediff(DAY,o_edate,'" + dict["smonth"] + "')<=0 ");
                    }
                }
                if (dict.ContainsKey("emonth"))
                {
                    if (dict["emonth"].Length == 7)
                    {
                        strWhere1.Append(" and datediff(MONTH,o_edate,'" + dict["emonth"] + "-01')>=0 ");
                    }
                    else
                    {
                        strWhere1.Append(" and datediff(DAY,o_edate,'" + dict["emonth"] + "')>=0 ");
                    }
                }
                if (dict.ContainsKey("status"))
                {
                    if (dict["status"] == "3")
                    {
                        strWhere1.Append(" and (o_status=1 or o_status=2)");
                    }
                    else
                    {
                        strWhere1.Append(" and o_status=" + dict["status"] + "");
                    }
                }
                if (dict.ContainsKey("lockstatus"))
                {
                    if (dict["lockstatus"] == "3")
                    {
                        strWhere1.Append(" and (o_lockStatus=0 or o_lockStatus=2)");
                    }
                    else
                    {
                        strWhere1.Append(" and o_lockStatus=" + dict["lockstatus"] + "");
                    }
                }
                if (dict.ContainsKey("area"))
                {
                    strWhere1.Append(" and p1.op_area in ('" + dict["area"].Replace(",", "','") + "')");
                }
                if (dict.ContainsKey("person"))
                {
                    if (dict["type"] == "0")
                    {
                        strWhere1.Append(" and p1.op_number in ('" + dict["person"].Replace(",", "','") + "')");
                    }
                    else
                    {
                        strWhere1.Append(" and p3.op_number in ('" + dict["person"].Replace(",", "','") + "')");
                    }
                }
                if (dict.ContainsKey("isCust") && dict["isCust"] != "on")
                {
                    custFiled = "0 oCust";
                }
                else
                {
                    if (dict["type"] != "0")
                    {
                        custFiled = "sum(isnull(o_financeCust,0)) oCust";
                    }
                    else
                    {
                        custFiled = "Convert(decimal(10,2),sum(isnull(o_financeCust,0)*p1.op_ratio/100)) oCust";
                    }
                }
            }

            StringBuilder strSql = new StringBuilder();
            if (dict["type"] != "0")//接单
            {
                strSql.Append("select *,Convert(decimal(10,2),(shou-fu-oCust+ticheng)) profit1,Convert(decimal(10,2),case when shou-unIncome<>0 then (shou-fu-oCust+ticheng)*100/(shou-unIncome) else 0 end) profitRatio1");
                strSql.Append(" ,Convert(decimal(10,2),(shou-fu-oCust)) profit2,Convert(decimal(10,2),case when shou-unIncome<>0 then (shou-fu-oCust)*100/(shou-unIncome) else 0 end) profitRatio2");
                strSql.Append(" from (");
                strSql.Append(" select p3.op_name,p3.op_number,p3.op_area,count(*) oCount,sum(isnull(shou,0)) shou,sum(isnull(fu,0)) fu,sum(isnull(ticheng,0)) ticheng,"+ custFiled + ",sum(isnull(unIncome,0)) unIncome,sum(isnull(unCost,0)) unCost");
                strSql.Append(" from MS_Order left join MS_OrderPerson p1 on o_id=p1.op_oid and p1.op_type=1 ");
                strSql.Append(" left join(select fin_oid,sum(case when fin_type = 1 then isnull(fin_money, 0) else 0 end) shou,sum(case when fin_type = 0 then isnull(fin_money, 0) else 0 end) fu, sum(case when na_name like '%提成%' then fin_money else 0 end) ticheng,sum(case when fin_type = 1 and fin_detail='代收代付' then isnull(fin_money, 0) else 0 end) unIncome,sum(case when fin_type = 0 and fin_detail='代收代付' then isnull(fin_money, 0) else 0 end) unCost from MS_finance left join MS_Nature on fin_nature=na_id where fin_flag <> 1  group by fin_oid) t on o_id = t.fin_oid ");
                if (dict["type"] == "1")
                {
                    strSql.Append(" left join MS_OrderPerson p3 on o_id=p3.op_oid and p3.op_type=3");
                }
                else if (dict["type"] == "2")
                {
                    strSql.Append(" left join MS_OrderPerson p3 on o_id=p3.op_oid and p3.op_type=5");
                }
                strSql.Append(" where 1=1 " + strWhere1 + "");
                strSql.Append(" group by p3.op_name, p3.op_number, p3.op_area) v");
            }
            else //下单
            {
                strSql.Append("select *,Convert(decimal(10,2),(shou-fu-oCust+ticheng)) profit1,Convert(decimal(10,2),case when shou-unIncome<>0 then (shou-fu-oCust+ticheng)*100/(shou-unIncome) else 0 end) profitRatio1");
                strSql.Append(",Convert(decimal(10,2),(shou-fu-oCust)) profit2,Convert(decimal(10,2),case when shou-unIncome<>0 then (shou-fu-oCust)*100/(shou-unIncome) else 0 end) profitRatio2");
                strSql.Append(" from(");
                strSql.Append(" select p1.op_name, p1.op_number, p1.op_area,count(*) oCount,Convert(decimal(10,2),sum(isnull(shou,0)*p1.op_ratio/100)) shou,Convert(decimal(10,2),sum(isnull(fu,0)*p1.op_ratio/100)) fu,Convert(decimal(10,2),sum(isnull(ticheng,0)*p1.op_ratio/100)) ticheng,"+custFiled+",Convert(decimal(10,2),sum(isnull(unIncome,0)*p1.op_ratio/100)) unIncome,Convert(decimal(10,2),sum(isnull(unCost,0)*p1.op_ratio/100)) unCost");
                strSql.Append(" from MS_OrderPerson p1 left join MS_Order  on o_id=p1.op_oid and (p1.op_type=1 or p1.op_type=6)");
                strSql.Append(" left join (select fin_oid,sum(case when fin_type = 1 then isnull(fin_money, 0) else 0 end) shou,sum(case when fin_type = 0 then isnull(fin_money, 0) else 0 end) fu, sum(case when na_name like '%提成%' then fin_money else 0 end) ticheng,sum(case when fin_type = 1 and fin_detail='代收代付' then isnull(fin_money, 0) else 0 end) unIncome,sum(case when fin_type = 0 and fin_detail='代收代付' then isnull(fin_money, 0) else 0 end) unCost from MS_finance left join MS_Nature on fin_nature=na_id where fin_flag <> 1  group by fin_oid) t on o_id = t.fin_oid ");
                strSql.Append(" where 1=1  "+ strWhere1 + "");
                strSql.Append(" group by p1.op_name, p1.op_number, p1.op_area) v");
            }
            
            SqlParameter[] param = { };
            recordCount = 0;tCount = 0;tOrderShou = 0;tUnIncome = 0;tOrderFu = 0;tUnCost = 0;tOrderTicheng = 0;tCust = 0;tProfit1 = 0; tProfit2 = 0;
            if (!isPage)
            {
                return DbHelperSQL.Query("select * from ("+ strSql.ToString() + ") u order by "+ filedOrder + "");
            }
            //recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
            DataTable dt = DbHelperSQL.Query("select count(*) c,sum(oCount) tCount,sum(shou) tOrderShou,sum(unIncome) tUnIncome,sum(fu) tOrderFu,sum(unCost) tUncost,sum(Ticheng) tOrderTicheng,sum(oCust) tCust,sum(profit1) tProfit1,sum(profit2) tProfit2 from(" + strSql.ToString() + ") t").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                recordCount = Utils.ObjToInt(dt.Rows[0]["c"], 0);
                tCount = Utils.ObjToInt(dt.Rows[0]["tCount"], 0);
                tOrderShou = Utils.ObjToDecimal(dt.Rows[0]["tOrderShou"], 0);
                tUnIncome = Utils.ObjToDecimal(dt.Rows[0]["tUnIncome"], 0);
                tOrderFu = Utils.ObjToDecimal(dt.Rows[0]["tOrderFu"], 0);
                tUnCost = Utils.ObjToDecimal(dt.Rows[0]["tUncost"], 0);
                tOrderTicheng = Utils.ObjToDecimal(dt.Rows[0]["tOrderTicheng"], 0);
                tCust = Utils.ObjToDecimal(dt.Rows[0]["tCust"], 0);
                tProfit1 = Utils.ObjToDecimal(dt.Rows[0]["tProfit1"], 0);
                tProfit2 = Utils.ObjToDecimal(dt.Rows[0]["tProfit2"], 0);
            }
            return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }

        /// <summary>
        /// 区域业绩统计
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataSet getAreaAchievementStatisticData(Dictionary<string, string> dict, int pageSize, int pageIndex, string filedOrder, out int recordCount, out int tCount,out decimal tShou, out decimal tUnIncome, out decimal tFu, out decimal tUnCost, out decimal tCust, out decimal tTicheng, out decimal tProfit1, out decimal tProfit2, bool isPage = true)
        {
            StringBuilder strWhere1 = new StringBuilder();
            StringBuilder strWhere2 = new StringBuilder();
            string costFiled = "Convert(decimal(10,2),sum(isnull(o_financeCust,0)*p_ratio/100)) oCust";
            if (dict != null)
            {
                if (dict.ContainsKey("smonth"))
                {
                    strWhere1.Append(" and datediff(MONTH,o_edate,'" + dict["smonth"] + "-01')<=0 ");
                }
                if (dict.ContainsKey("emonth"))
                {
                    strWhere1.Append(" and datediff(MONTH,o_edate,'" + dict["emonth"] + "-01')>=0 ");
                }
                if (dict.ContainsKey("status"))
                {
                    if (dict["status"] == "3")
                    {
                        strWhere1.Append(" and (o_status=1 or o_status=2)");
                    }
                    else
                    {
                        strWhere1.Append(" and o_status=" + dict["status"] + "");
                    }
                }
                if (dict.ContainsKey("lockstatus"))
                {
                    if (dict["lockstatus"] == "3")
                    {
                        strWhere1.Append(" and (o_lockStatus=0 or o_lockStatus=2)");
                    }
                    else
                    {
                        strWhere1.Append(" and o_lockStatus=" + dict["lockstatus"] + "");
                    }
                }
                if (dict.ContainsKey("area"))
                {
                    strWhere1.Append(" and p_name in ('" + dict["area"].Replace(",", "','") + "')");
                }
                if (dict.ContainsKey("isCust") && dict["isCust"] != "on")
                {
                    costFiled = "0 oCust";
                }
            }
            StringBuilder strSql = new StringBuilder();
            //strSql.Append("select " + selectFiled + " ");
            //strSql.Append(" from(select a.*,isnull(b.c,0) oCount,isnull(b.o_financeCust,0) o_financeCust from (select de_area,de_subname from MS_department where de_type=1) a left join ");
            //strSql.Append(" (select op_area,count(*) c,sum(o_financeCust) o_financeCust from MS_Order left join MS_OrderPerson on o_id=op_oid and op_type=1");
            //strSql.Append(" where 1=1 "+ strWhere2 + " group by op_area) b on a.de_area=b.op_area) c ");
            //strSql.Append(" left join (select fin_area,sum(case when fin_type = 1 then fin_money else 0 end) shou, ");
            //strSql.Append(" sum(case when fin_type = 0 then fin_money else 0 end) fu,sum(case when fin_type = 1 and fin_detail='代收代付' then isnull(fin_money, 0) else 0 end) unIncome,sum(case when fin_type = 0 and fin_detail='代收代付' then isnull(fin_money, 0) else 0 end) unCost ");
            //strSql.Append(" from MS_finance left join MS_Order on fin_oid=o_id left join MS_Nature on fin_nature=na_id where 1=1  " + strWhere1 + " group by fin_area) d on c.de_area = d.fin_area");

            strSql.Append("select *,Convert(decimal(10,2),(shou-fu-oCust+ticheng)) profit1,Convert(decimal(10,2),case when shou-unIncome<>0 then (shou-fu-oCust+ticheng)*100/(shou-unIncome) else 0 end) profitRatio1");
            strSql.Append(",Convert(decimal(10,2),(shou-fu-oCust)) profit2,Convert(decimal(10,2),case when shou-unIncome<>0 then (shou-fu-oCust)*100/(shou-unIncome) else 0 end) profitRatio2");
            strSql.Append(" from (");
            strSql.Append("select p_name,p_chnName,count(*) oCount,Convert(decimal(10,2),sum(isnull(shou,0)*p_ratio/100)) shou,Convert(decimal(10,2),sum(isnull(fu,0)*p_ratio/100)) fu");
            strSql.Append(",Convert(decimal(10,2),sum(isnull(ticheng,0)*p_ratio/100)) ticheng,"+ costFiled + ",Convert(decimal(10,2),sum(isnull(unIncome,0)*p_ratio/100)) unIncome,Convert(decimal(10,2),sum(isnull(unCost,0)*p_ratio/100)) unCost");
            strSql.Append(" from MS_OrderPlace left join MS_Order on o_id=p_oid");
            strSql.Append(" left join (select fin_oid,sum(case when fin_type = 1 then isnull(fin_money, 0) else 0 end) shou,sum(case when fin_type = 0 then isnull(fin_money, 0) else 0 end) fu, sum(case when na_name like '%提成%' then fin_money else 0 end) ticheng,sum(case when fin_type = 1 and fin_detail='代收代付' then isnull(fin_money, 0) else 0 end) unIncome,sum(case when fin_type = 0 and fin_detail='代收代付' then isnull(fin_money, 0) else 0 end) unCost from MS_finance left join MS_Nature on fin_nature=na_id where fin_flag <> 1  group by fin_oid) t on o_id = t.fin_oid ");
            strSql.Append(" where 1=1 "+ strWhere1 + " group by p_name,p_chnName) v"); 

            SqlParameter[] param = { };
            recordCount = 0;tCount = 0;tUnIncome = 0;tUnCost = 0;tCust = 0;tShou = 0;tFu = 0;tTicheng = 0;tProfit1 = 0;tProfit2 = 0;
            if (!isPage)
            {
                return DbHelperSQL.Query("select * from (" + strSql.ToString() + ") u order by " + filedOrder + "");
            }
            //recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
            DataTable dt = DbHelperSQL.Query("select count(*) c,sum(oCount) tCount,sum(shou) tshou,sum(unIncome) tUnIncome,sum(fu) tfu,sum(unCost) tUncost,sum(oCust) tCust,sum(ticheng) tTicheng,sum(profit1) tProfit1,sum(profit2) tProfit2 from(" + strSql.ToString() + ") t").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                recordCount = Utils.ObjToInt(dt.Rows[0]["c"], 0);
                tCount = Utils.ObjToInt(dt.Rows[0]["tCount"], 0);
                tShou = Utils.ObjToDecimal(dt.Rows[0]["tshou"], 0);
                tUnIncome = Utils.ObjToDecimal(dt.Rows[0]["tUnIncome"], 0);
                tFu = Utils.ObjToDecimal(dt.Rows[0]["tfu"], 0);
                tUnCost = Utils.ObjToDecimal(dt.Rows[0]["tUncost"], 0);
                tCust = Utils.ObjToDecimal(dt.Rows[0]["tCust"], 0);
                tTicheng = Utils.ObjToDecimal(dt.Rows[0]["tTicheng"], 0);
                tProfit1 = Utils.ObjToDecimal(dt.Rows[0]["tProfit1"], 0);
                tProfit2 = Utils.ObjToDecimal(dt.Rows[0]["tProfit2"], 0);
            }
            return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }


        /// <summary>
        /// 客源收益分析-明细列表
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataSet getRevenueAnalysisListData(Dictionary<string, string> dict, int pageSize, int pageIndex, string filedOrder, out int recordCount,out decimal tShou,out decimal tFu,out decimal tProfit,bool isPage=true)
        {
            StringBuilder strWhere1 = new StringBuilder();
            StringBuilder strWhere2 = new StringBuilder();
            string selectFiled = "o_id,c_name,o_content,o_status,o_address,o_edate,isnull(o_financeCust,0) o_financeCust,na_name,op_area,op_name,op_number,person2,person3,person4,sum(case when fin_type=1 then fin_money else 0 end) shou,sum(case when fin_type=0 then fin_money else 0 end) fu,sum(case when fin_type=1 then fin_money else 0-fin_money end) profit";
            if (dict != null)
            {
                if (dict.ContainsKey("smonth"))
                {
                    strWhere1.Append(" and datediff(MONTH,o_edate,'" + dict["smonth"] + "-01')<=0 ");
                }
                if (dict.ContainsKey("emonth"))
                {
                    strWhere1.Append(" and datediff(MONTH,o_edate,'" + dict["emonth"] + "-01')>=0 ");
                }
                if (dict.ContainsKey("lockstatus"))
                {
                    strWhere1.Append(" and o_lockStatus='" + dict["lockstatus"] + "'");
                    if (dict["lockstatus"] == "3")
                    {
                        strWhere1.Append(" and (o_lockStatus=0 or o_lockStatus=2)");
                    }
                    else
                    {
                        strWhere1.Append(" and o_lockStatus=" + dict["lockstatus"] + "");
                    }
                }
                if (dict.ContainsKey("area"))
                {
                    strWhere1.Append(" and op_area in ('" + dict["area"].Replace(",", "','") + "')");
                }
                if (dict.ContainsKey("cid") && dict["cid"] != "" && dict["cid"] != "0")
                {
                    strWhere1.Append(" and c_id =" + dict["cid"] + "");
                }
                else
                {
                    if (dict.ContainsKey("cusname"))
                    {
                        strWhere1.Append(" and c_name like '%" + dict["cusname"] + "%'");
                    }
                }
                if (dict.ContainsKey("nature"))
                {
                    strWhere1.Append(" and fin_nature in (" + dict["nature"] + ")");
                }
                if (dict.ContainsKey("person1"))
                {
                    strWhere1.Append(" and op_number in ('" + dict["person1"].Replace(",", "','") + "')");
                }
                if (dict.ContainsKey("person2"))
                {
                    string str = "";
                    string[] list = dict["person2"].Split(',');
                    foreach (string item in list)
                    {
                        if (string.IsNullOrEmpty(str))
                        {
                            str += " op.op_number='" + item + "' ";
                        }
                        else
                        {
                            str += " or  op.op_number='" + item + "' ";
                        }
                    }
                    strWhere1.Append(" and exists(select * from MS_OrderPerson op where op.op_oid=o_id and op.op_type=3 and (" + str + "))");
                }
                if (dict.ContainsKey("person3"))
                {
                    string str = "";
                    string[] list = dict["person3"].Split(',');
                    foreach (string item in list)
                    {
                        if (string.IsNullOrEmpty(str))
                        {
                            str += " op.op_number='" + item + "' ";
                        }
                        else
                        {
                            str += " or  op.op_number='" + item + "' ";
                        }
                    }                    
                    strWhere1.Append(" and exists(select * from MS_OrderPerson op where op.op_oid=o_id and op.op_type=5 and (" + str + "))");
                }
                if (dict.ContainsKey("person4"))
                {
                    string str = "";
                    string[] list = dict["person4"].Split(',');
                    foreach (string item in list)
                    {
                        if (string.IsNullOrEmpty(str))
                        {
                            str += " op.op_number='" + item + "' ";
                        }
                        else
                        {
                            str += " or  op.op_number='" + item + "' ";
                        }
                    }
                    strWhere1.Append(" and exists(select * from MS_OrderPerson op where op.op_oid=o_id and op.op_type=4 and (" + str + "))");
                }
                if (dict.ContainsKey("isCust") && dict["isCust"]!="on")
                {
                    selectFiled = "o_id,c_name,o_content,o_status,o_address,o_edate,0 as o_financeCust,na_name,op_area,op_name,op_number,person2,person3,person4,sum(case when fin_type=1 then fin_money else 0 end) shou,sum(case when fin_type=0 then fin_money else 0 end) fu,sum(case when fin_type=1 then fin_money else 0-fin_money end) profit";
                }
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select "+ selectFiled + " from (");
            strSql.Append(" select *,person2 = isnull(STUFF((SELECT ',' + op_name+'('+(case when op_dstatus=0 then '待定' else case when op_dstatus=1 then '处理中' else '已完成' end end)+')' FROM MS_OrderPerson WHERE  op_oid=o_id and op_type=3 FOR XML PATH('')), 1, 1, ''),'无'),person3 = isnull(STUFF((SELECT ',' + op_name+'('+(case when op_dstatus=0 then '待定' else case when op_dstatus=1 then '处理中' else '已完成' end end)+')' FROM MS_OrderPerson WHERE  op_oid=o_id and op_type=5 FOR XML PATH('')), 1, 1, ''),'无')");
            strSql.Append(" ,person4 = (STUFF((SELECT ',' + op_number + '(' + op_name + ')' FROM MS_OrderPerson WHERE  op_oid = fin_oid and op_type = 4 FOR XML PATH('')), 1, 1, ''))");
            strSql.Append(" from MS_finance left join MS_Order on fin_oid = o_id left join MS_Nature on fin_nature = na_id left join MS_OrderPerson on o_id = op_oid and op_type = 1 left join MS_Customer on o_cid = c_id where (o_status = 1 or o_status = 2) "+ strWhere1 + "");
            strSql.Append(" ) t group by o_id, c_name, o_content,o_status, o_address, o_edate,o_financeCust, na_name, op_area, op_name,op_number,person2, person3, person4");
            

            SqlParameter[] param = { };
            recordCount = 0;
            tShou = 0;
            tFu = 0;
            tProfit = 0;
            if (!isPage)
            {
                return DbHelperSQL.Query(strSql.ToString());                
            }
            //recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
            DataTable dt = DbHelperSQL.Query("select count(*) c,sum(shou) tshou,sum(fu) tfu,sum(profit) tprofit from(" + strSql.ToString() + ") t1").Tables[0];
            if (dt!=null && dt.Rows.Count>0)
            {
                recordCount = Utils.ObjToInt(dt.Rows[0]["c"],0);
                tShou = Utils.ObjToDecimal(dt.Rows[0]["tshou"],0);
                tFu = Utils.ObjToDecimal(dt.Rows[0]["tfu"], 0);
                tProfit = Utils.ObjToDecimal(dt.Rows[0]["tprofit"], 0);
            }
            return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }

        /// <summary>
        /// 客源收益分析-区域分组、客源分组、业务员分组、月份分组、业务性质
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataSet getRevenueAnalysisListData1(Dictionary<string, string> dict, int pageSize, int pageIndex,string selectFiled,string groupFiled, string filedOrder, out int recordCount, out decimal tShou, out decimal tFu, out decimal tProfit, bool isPage = true)
        {
            StringBuilder strWhere1 = new StringBuilder();
            StringBuilder strWhere2 = new StringBuilder();
            string sFiled = selectFiled + ",sum(case when fin_type=1 then fin_money else 0 end) shou,sum(case when fin_type=0 then fin_money else 0 end) fu,sum(case when fin_type=1 then fin_money else 0-fin_money end) profit";
            if (dict != null)
            {
                if (dict.ContainsKey("smonth"))
                {
                    strWhere1.Append(" and datediff(MONTH,o_edate,'" + dict["smonth"] + "-01')<=0 ");
                }
                if (dict.ContainsKey("emonth"))
                {
                    strWhere1.Append(" and datediff(MONTH,o_edate,'" + dict["emonth"] + "-01')>=0 ");
                }
                if (dict.ContainsKey("lockstatus"))
                {
                    strWhere1.Append(" and o_lockStatus='" + dict["lockstatus"] + "'");
                }
                if (dict.ContainsKey("area"))
                {
                    strWhere1.Append(" and op_area in ('" + dict["area"].Replace(",", "','") + "')");
                }
                if (dict.ContainsKey("cid") && dict["cid"] != "" && dict["cid"] != "0")
                {
                    strWhere1.Append(" and c_id =" + dict["cid"] + "");
                }
                else
                {
                    if (dict.ContainsKey("cusname"))
                    {
                        strWhere1.Append(" and c_name like '%" + dict["cusname"] + "%'");
                    }
                }
                if (dict.ContainsKey("nature"))
                {
                    strWhere1.Append(" and fin_nature in (" + dict["nature"] + ")");
                }
                if (dict.ContainsKey("person1"))
                {
                    strWhere1.Append(" and op_number in ('" + dict["person1"].Replace(",", "','") + "')");
                }
                if (dict.ContainsKey("person2"))
                {
                    string str = "";
                    string[] list = dict["person2"].Split(',');
                    foreach (string item in list)
                    {
                        if (string.IsNullOrEmpty(str))
                        {
                            str += " op.op_number='" + item + "' ";
                        }
                        else
                        {
                            str += " or  op.op_number='" + item + "' ";
                        }
                    }
                    strWhere1.Append(" and exists(select * from MS_OrderPerson op where op.op_oid=o_id and op.op_type=3 and (" + str + "))");
                }
                if (dict.ContainsKey("person3"))
                {
                    string str = "";
                    string[] list = dict["person3"].Split(',');
                    foreach (string item in list)
                    {
                        if (string.IsNullOrEmpty(str))
                        {
                            str += " op.op_number='" + item + "' ";
                        }
                        else
                        {
                            str += " or  op.op_number='" + item + "' ";
                        }
                    }
                    strWhere1.Append(" and exists(select * from MS_OrderPerson op where op.op_oid=o_id and op.op_type=5 and (" + str + "))");
                }
                if (dict.ContainsKey("person4"))
                {
                    string str = "";
                    string[] list = dict["person4"].Split(',');
                    foreach (string item in list)
                    {
                        if (string.IsNullOrEmpty(str))
                        {
                            str += " op.op_number='" + item + "' ";
                        }
                        else
                        {
                            str += " or  op.op_number='" + item + "' ";
                        }
                    }
                    strWhere1.Append(" and exists(select * from MS_OrderPerson op where op.op_oid=o_id and op.op_type=4 and (" + str + "))");
                }
                if (dict.ContainsKey("isCust") && dict["isCust"] != "on")
                {
                    sFiled = selectFiled + ",sum(case when fin_type=1 then fin_money else 0 end) shou,sum(case when fin_type=0 then fin_money else 0 end) fu,sum(case when fin_type=1 then fin_money else 0-fin_money end)-isnull(o_financeCust,0) profit";
                }
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select "+ selectFiled + ",sum(shou) shou,sum(fu) fu,sum(profit) profit from ( ");
            strSql.Append("select " + sFiled + " ");
            strSql.Append(" from MS_finance left join MS_Order on fin_oid=o_id left join MS_Nature on fin_nature=na_id left join MS_OrderPerson on o_id=op_oid and op_type=1 left join MS_Customer on o_cid=c_id where (o_status=1 or o_status =2) " + strWhere1 + "");
            strSql.Append(" group by "+ groupFiled + " ) tt group by "+ groupFiled.Replace(",o_financeCust","") + "");
            //月份分组
            if (dict.ContainsKey("group") && dict["group"] == "6")
            {
                strSql.Clear();
                strSql.Append("select oYear,oMonth,op_area,sum(shou) shou,sum(fu) fu,sum(profit) profit from ( ");
                strSql.Append("select " + sFiled + " ");
                strSql.Append(" from MS_finance left join MS_Order on fin_oid=o_id left join MS_Nature on fin_nature=na_id left join MS_OrderPerson on o_id=op_oid and op_type=1 left join MS_Customer on o_cid=c_id where (o_status=1 or o_status =2) " + strWhere1 + "");
                strSql.Append(" group by " + groupFiled + " ) tt group by oYear,oMonth,op_area");
                filedOrder = "oYear asc,oMonth asc";
            }

            SqlParameter[] param = { };
            recordCount = 0;
            tShou = 0;
            tFu = 0;
            tProfit = 0;
            if (!isPage)
            {
                return DbHelperSQL.Query(strSql.ToString());
            }
            //recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
            DataTable dt = DbHelperSQL.Query("select count(*) c,sum(shou) tshou,sum(fu) tfu,sum(profit) tprofit from(" + strSql.ToString() + ") t1").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                recordCount = Utils.ObjToInt(dt.Rows[0]["c"], 0);
                tShou = Utils.ObjToDecimal(dt.Rows[0]["tshou"], 0);
                tFu = Utils.ObjToDecimal(dt.Rows[0]["tfu"], 0);
                tProfit = Utils.ObjToDecimal(dt.Rows[0]["tprofit"], 0);
            }
            return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }
        /// <summary>
        /// 客源收益分析-设计策划人员分组
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataSet getRevenueAnalysisListData2(Dictionary<string, string> dict, int pageSize, int pageIndex, string selectFiled, string filedOrder, out int recordCount, out decimal tShou, out decimal tFu, out decimal tProfit, bool isPage = true)
        {
            StringBuilder strWhere1 = new StringBuilder();
            StringBuilder strWhere2 = new StringBuilder();
            string sFiled = selectFiled + ",sum(case when fin_type=1 then fin_money else 0 end) shou,sum(case when fin_type=0 then fin_money else 0 end) fu,sum(case when fin_type=1 then fin_money else 0-fin_money end) profit";
            if (dict != null)
            {
                strWhere1.Append(" and isnull(p3.op_id,0)>0");
                if (dict.ContainsKey("smonth"))
                {
                    strWhere1.Append(" and datediff(MONTH,o_edate,'" + dict["smonth"] + "-01')<=0 ");
                }
                if (dict.ContainsKey("emonth"))
                {
                    strWhere1.Append(" and datediff(MONTH,o_edate,'" + dict["emonth"] + "-01')>=0 ");
                }
                if (dict.ContainsKey("lockstatus"))
                {
                    strWhere1.Append(" and o_lockStatus='" + dict["lockstatus"] + "'");
                }
                if (dict.ContainsKey("area"))
                {
                    strWhere1.Append(" and p1.op_area in ('" + dict["area"].Replace(",", "','") + "')");
                }
                if (dict.ContainsKey("cid") && dict["cid"] != "" && dict["cid"] != "0")
                {
                    strWhere1.Append(" and c_id =" + dict["cid"] + "");
                }
                else
                {
                    if (dict.ContainsKey("cusname"))
                    {
                        strWhere1.Append(" and c_name like '%" + dict["cusname"] + "%'");
                    }
                }
                if (dict.ContainsKey("nature"))
                {
                    strWhere1.Append(" and fin_nature in (" + dict["nature"] + ")");
                }
                if (dict.ContainsKey("person1"))
                {
                    strWhere1.Append(" and p1.op_number in ('" + dict["person1"].Replace(",", "','") + "')");
                }
                if (dict.ContainsKey("person2"))
                {
                    string str = "";
                    string[] list = dict["person2"].Split(',');
                    foreach (string item in list)
                    {
                        if (string.IsNullOrEmpty(str))
                        {
                            str += " op.op_number='" + item + "' ";
                        }
                        else
                        {
                            str += " or  op.op_number='" + item + "' ";
                        }
                    }
                    strWhere1.Append(" and exists(select * from MS_OrderPerson op where op.op_oid=o_id and op.op_type=3 and (" + str + "))");
                }
                if (dict.ContainsKey("person3"))
                {
                    string str = "";
                    string[] list = dict["person3"].Split(',');
                    foreach (string item in list)
                    {
                        if (string.IsNullOrEmpty(str))
                        {
                            str += " op.op_number='" + item + "' ";
                        }
                        else
                        {
                            str += " or  op.op_number='" + item + "' ";
                        }
                    }
                    strWhere1.Append(" and exists(select * from MS_OrderPerson op where op.op_oid=o_id and op.op_type=5 and (" + str + "))");
                }
                if (dict.ContainsKey("person4"))
                {
                    string str = "";
                    string[] list = dict["person4"].Split(',');
                    foreach (string item in list)
                    {
                        if (string.IsNullOrEmpty(str))
                        {
                            str += " op.op_number='" + item + "' ";
                        }
                        else
                        {
                            str += " or  op.op_number='" + item + "' ";
                        }
                    }
                    strWhere1.Append(" and exists(select * from MS_OrderPerson op where op.op_oid=o_id and op.op_type=4 and (" + str + "))");
                }
                if (dict.ContainsKey("isCust") && dict["isCust"] != "on")
                {
                    sFiled = selectFiled + ",sum(case when fin_type=1 then fin_money else 0 end) shou,sum(case when fin_type=0 then fin_money else 0 end) fu,sum(case when fin_type=1 then fin_money else 0-fin_money end)-isnull(o_financeCust,0) profit";
                }
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select op_name,op_area,op_number,sum(shou) shou,sum(fu) fu,sum(profit) profit from ( ");
            strSql.Append("select " + sFiled + " ");
            strSql.Append(" from MS_finance left join MS_Order on fin_oid=o_id left join MS_Nature on fin_nature=na_id left join MS_OrderPerson p1 on o_id=p1.op_oid and p1.op_type=1 left join MS_OrderPerson p3 on o_id=p3.op_oid and p3.op_type=" + (dict["group"] == "4" ? "3" : "5") + " left join MS_Customer on o_cid=c_id where (o_status=1 or o_status =2) " + strWhere1 + "");
            strSql.Append(" group by " + selectFiled + ",o_financeCust ) tt group by op_name,op_area,op_number");


            SqlParameter[] param = { };
            recordCount = 0;
            tShou = 0;
            tFu = 0;
            tProfit = 0;
            if (!isPage)
            {
                return DbHelperSQL.Query(strSql.ToString());
            }
            //recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
            DataTable dt = DbHelperSQL.Query("select count(*) c,sum(shou) tshou,sum(fu) tfu,sum(profit) tprofit from(" + strSql.ToString() + ") t1").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                recordCount = Utils.ObjToInt(dt.Rows[0]["c"], 0);
                tShou = Utils.ObjToDecimal(dt.Rows[0]["tshou"], 0);
                tFu = Utils.ObjToDecimal(dt.Rows[0]["tfu"], 0);
                tProfit = Utils.ObjToDecimal(dt.Rows[0]["tprofit"], 0);
            }
            return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }
        /// <summary>
        /// 客源收益分析-执行人员分组
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataSet getRevenueAnalysisListData3(Dictionary<string, string> dict, int pageSize, int pageIndex, string selectFiled, string filedOrder, out int recordCount, out decimal tShou, out decimal tFu, out decimal tProfit, bool isPage = true)
        {
            StringBuilder strWhere1 = new StringBuilder();
            StringBuilder strWhere2 = new StringBuilder();
            string sFiled = selectFiled + ",sum(case when fin_type=1 then fin_money else 0 end) shou,sum(case when fin_type=0 then fin_money else 0 end) fu,sum(case when fin_type=1 then fin_money else 0-fin_money end) profit";
            if (dict != null)
            {
                strWhere1.Append(" and isnull(p3.op_id,0)>0");
                if (dict.ContainsKey("smonth"))
                {
                    strWhere1.Append(" and datediff(MONTH,o_edate,'" + dict["smonth"] + "-01')<=0 ");
                }
                if (dict.ContainsKey("emonth"))
                {
                    strWhere1.Append(" and datediff(MONTH,o_edate,'" + dict["emonth"] + "-01')>=0 ");
                }
                if (dict.ContainsKey("lockstatus"))
                {
                    strWhere1.Append(" and o_lockStatus='" + dict["lockstatus"] + "'");
                }
                if (dict.ContainsKey("area"))
                {
                    strWhere1.Append(" and p1.op_area in ('" + dict["area"].Replace(",", "','") + "')");
                }
                if (dict.ContainsKey("cid") && dict["cid"] != "" && dict["cid"] != "0")
                {
                    strWhere1.Append(" and c_id =" + dict["cid"] + "");
                }
                else
                {
                    if (dict.ContainsKey("cusname"))
                    {
                        strWhere1.Append(" and c_name like '%" + dict["cusname"] + "%'");
                    }
                }
                if (dict.ContainsKey("nature"))
                {
                    strWhere1.Append(" and fin_nature in (" + dict["nature"] + ")");
                }
                if (dict.ContainsKey("person1"))
                {
                    strWhere1.Append(" and p1.op_number in ('" + dict["person1"].Replace(",", "','") + "')");
                }
                if (dict.ContainsKey("person2"))
                {
                    string str = "";
                    string[] list = dict["person2"].Split(',');
                    foreach (string item in list)
                    {
                        if (string.IsNullOrEmpty(str))
                        {
                            str += " op.op_number='" + item + "' ";
                        }
                        else
                        {
                            str += " or  op.op_number='" + item + "' ";
                        }
                    }
                    strWhere1.Append(" and exists(select * from MS_OrderPerson op where op.op_oid=o_id and op.op_type=3 and (" + str + "))");
                }
                if (dict.ContainsKey("person3"))
                {
                    string str = "";
                    string[] list = dict["person3"].Split(',');
                    foreach (string item in list)
                    {
                        if (string.IsNullOrEmpty(str))
                        {
                            str += " op.op_number='" + item + "' ";
                        }
                        else
                        {
                            str += " or  op.op_number='" + item + "' ";
                        }
                    }
                    strWhere1.Append(" and exists(select * from MS_OrderPerson op where op.op_oid=o_id and op.op_type=5 and (" + str + "))");
                }
                if (dict.ContainsKey("person4"))
                {
                    string str = "";
                    string[] list = dict["person4"].Split(',');
                    foreach (string item in list)
                    {
                        if (string.IsNullOrEmpty(str))
                        {
                            str += " op.op_number='" + item + "' ";
                        }
                        else
                        {
                            str += " or  op.op_number='" + item + "' ";
                        }
                    }
                    strWhere1.Append(" and exists(select * from MS_OrderPerson op where op.op_oid=o_id and op.op_type=4 and (" + str + "))");
                }
                if (dict.ContainsKey("isCust") && dict["isCust"] != "on")
                {
                    sFiled = selectFiled + ",sum(case when fin_type=1 then fin_money else 0 end) shou,sum(case when fin_type=0 then fin_money else 0 end) fu,sum(case when fin_type=1 then fin_money else 0-fin_money end)-isnull(o_financeCust,0) profit";
                }
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select op_name,op_area,op_number,sum(shou) shou,sum(fu) fu,sum(profit) profit from ( ");
            strSql.Append("select " + sFiled + " ");
            strSql.Append(" from MS_finance left join MS_Order on fin_oid=o_id left join MS_Nature on fin_nature=na_id left join MS_OrderPerson p1 on o_id=p1.op_oid and p1.op_type=1 left join MS_OrderPerson p3 on o_id=p3.op_oid and p3.op_type=4 left join MS_Customer on o_cid=c_id where (o_status=1 or o_status =2) " + strWhere1 + "");
            strSql.Append(" group by " + selectFiled + ",o_financeCust) tt group by op_name,op_area,op_number");


            SqlParameter[] param = { };
            recordCount = 0;
            tShou = 0;
            tFu = 0;
            tProfit = 0;
            if (!isPage)
            {
                return DbHelperSQL.Query(strSql.ToString());
            }
            //recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
            DataTable dt = DbHelperSQL.Query("select count(*) c,sum(shou) tshou,sum(fu) tfu,sum(profit) tprofit from(" + strSql.ToString() + ") t1").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                recordCount = Utils.ObjToInt(dt.Rows[0]["c"], 0);
                tShou = Utils.ObjToDecimal(dt.Rows[0]["tshou"], 0);
                tFu = Utils.ObjToDecimal(dt.Rows[0]["tfu"], 0);
                tProfit = Utils.ObjToDecimal(dt.Rows[0]["tprofit"], 0);
            }
            return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }

        /// <summary>
        /// 供应商支出分析-明细列表
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataSet getExpendAnalyzeData(Dictionary<string, string> dict, int pageSize, int pageIndex, string filedOrder, out int recordCount, out decimal tFu, bool isPage = true)
        {
            StringBuilder strWhere1 = new StringBuilder();
            if (dict != null)
            {
                if (dict.ContainsKey("smonth"))
                {
                    strWhere1.Append(" and datediff(MONTH,o_edate,'" + dict["smonth"] + "-01')<=0 ");
                }
                if (dict.ContainsKey("emonth"))
                {
                    strWhere1.Append(" and datediff(MONTH,o_edate,'" + dict["emonth"] + "-01')>=0 ");
                }
                if (dict.ContainsKey("lockstatus"))
                {
                    if (dict["lockstatus"] == "3")
                    {
                        strWhere1.Append(" and (o_lockStatus=0 or o_lockStatus=2)");
                    }
                    else
                    {
                        strWhere1.Append(" and o_lockStatus=" + dict["lockstatus"] + "");
                    }
                }
                if (dict.ContainsKey("area"))
                {
                    strWhere1.Append(" and op_area in ('" + dict["area"].Replace(",", "','") + "')");
                }
                if (dict.ContainsKey("cid") && dict["cid"] != "" && dict["cid"] != "0")
                {
                    strWhere1.Append(" and c_id =" + dict["cid"] + "");
                }
                else
                {
                    if (dict.ContainsKey("cusname"))
                    {
                        strWhere1.Append(" and c_name like '%" + dict["cusname"] + "%'");
                    }
                }
                if (dict.ContainsKey("nature"))
                {
                    strWhere1.Append(" and fin_nature in (" + dict["nature"] + ")");
                }
                if (dict.ContainsKey("person1"))
                {
                    strWhere1.Append(" and op_number in ('" + dict["person1"].Replace(",", "','") + "')");
                }
                if (dict.ContainsKey("person3"))
                {
                    string str = "";
                    string[] list = dict["person3"].Split(',');
                    foreach (string item in list)
                    {
                        if (string.IsNullOrEmpty(str))
                        {
                            str += " op.op_number='" + item + "' ";
                        }
                        else
                        {
                            str += " or  op.op_number='" + item + "' ";
                        }
                    }
                    strWhere1.Append(" and exists(select * from MS_OrderPerson op where op.op_oid=o_id and op.op_type=3 and (" + str + "))");
                }
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select o_id,c_name,o_content,o_status,o_address,o_edate,na_name,fin_detail,op_area,op_name,op_number,fin_personNum,fin_personName,sum(fin_money) fu ");
            strSql.Append(" from MS_finance left join MS_Order on fin_oid=o_id left join MS_Nature on fin_nature=na_id left join MS_OrderPerson on o_id=op_oid and op_type=1 left join MS_Customer on fin_cid=c_id ");
            strSql.Append(" where (o_status=1 or o_status =2) and fin_type=0 and fin_flag=2 and na_flag=0 "+ strWhere1 + "");
            strSql.Append(" group by o_id,c_name,o_content,o_status,o_address,o_edate,na_name,fin_detail,op_area,op_name,op_number,fin_personNum,fin_personName");


            SqlParameter[] param = { };
            recordCount = 0;
            tFu = 0;
            if (!isPage)
            {
                return DbHelperSQL.Query(strSql.ToString());
            }
            //recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
            DataTable dt = DbHelperSQL.Query("select count(*) c,sum(fu) tfu from(" + strSql.ToString() + ") t").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                recordCount = Utils.ObjToInt(dt.Rows[0]["c"], 0);
                tFu = Utils.ObjToDecimal(dt.Rows[0]["tfu"], 0);
            }
            return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }

        /// <summary>
        /// 供应商支出分析-分组
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataSet getExpendAnalyzeData1(Dictionary<string, string> dict, int pageSize, int pageIndex, string selectFiled, string groupFiled, string filedOrder, out int recordCount, out decimal tFu, bool isPage = true)
        {
            StringBuilder strWhere1 = new StringBuilder();
            if (dict != null)
            {
                if (dict.ContainsKey("smonth"))
                {
                    strWhere1.Append(" and datediff(MONTH,o_edate,'" + dict["smonth"] + "-01')<=0 ");
                }
                if (dict.ContainsKey("emonth"))
                {
                    strWhere1.Append(" and datediff(MONTH,o_edate,'" + dict["emonth"] + "-01')>=0 ");
                }
                if (dict.ContainsKey("lockstatus"))
                {
                    if (dict["lockstatus"] == "3")
                    {
                        strWhere1.Append(" and (o_lockStatus=0 or o_lockStatus=2)");
                    }
                    else
                    {
                        strWhere1.Append(" and o_lockStatus=" + dict["lockstatus"] + "");
                    }
                }
                if (dict.ContainsKey("area"))
                {
                    strWhere1.Append(" and op_area in ('" + dict["area"].Replace(",", "','") + "')");
                }
                if (dict.ContainsKey("cid") && dict["cid"] != "" && dict["cid"] != "0")
                {
                    strWhere1.Append(" and c_id =" + dict["cid"] + "");
                }
                else
                {
                    if (dict.ContainsKey("cusname"))
                    {
                        strWhere1.Append(" and c_name like '%" + dict["cusname"] + "%'");
                    }
                }
                if (dict.ContainsKey("nature"))
                {
                    strWhere1.Append(" and fin_nature in (" + dict["nature"] + ")");
                }
                if (dict.ContainsKey("person1"))
                {
                    strWhere1.Append(" and op_number in ('" + dict["person1"].Replace(",", "','") + "')");
                }
                if (dict.ContainsKey("person3"))
                {
                    string str = "";
                    string[] list = dict["person3"].Split(',');
                    foreach (string item in list)
                    {
                        if (string.IsNullOrEmpty(str))
                        {
                            str += " op.op_number='" + item + "' ";
                        }
                        else
                        {
                            str += " or  op.op_number='" + item + "' ";
                        }
                    }
                    strWhere1.Append(" and exists(select * from MS_OrderPerson op where op.op_oid=o_id and op.op_type=3 and (" + str + "))");
                }
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select "+ selectFiled + ",sum(fin_money) fu ");
            strSql.Append(" from MS_finance left join MS_Order on fin_oid=o_id left join MS_Nature on fin_nature=na_id left join MS_OrderPerson on o_id=op_oid and op_type=1 left join MS_Customer on fin_cid=c_id ");
            strSql.Append(" where (o_status=1 or o_status =2) and fin_type=0 and fin_flag=2 and na_flag=0 " + strWhere1 + "");
            strSql.Append(" group by "+ groupFiled + "");


            SqlParameter[] param = { };
            recordCount = 0;
            tFu = 0;
            if (!isPage)
            {
                return DbHelperSQL.Query(strSql.ToString());
            }
            //recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
            DataTable dt = DbHelperSQL.Query("select count(*) c,sum(fu) tfu from(" + strSql.ToString() + ") t").Tables[0];
            if (dt != null && dt.Rows.Count > 0)
            {
                recordCount = Utils.ObjToInt(dt.Rows[0]["c"], 0);
                tFu = Utils.ObjToDecimal(dt.Rows[0]["tfu"], 0);
            }
            return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }

        /// <summary>
        /// 策划与设计
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="strWhere"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataSet getReceiveOrderAnalyzeData(int pageSize, int pageIndex, string strWhere, string filedOrder,out int tCount3,out int tCount5,out int tCount, out int recordCount, bool isPage = true)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select op_number,op_name,detaildepart,sum(case when op_type = 3 then 1 else 0 end) type3,sum(case when op_type = 5 then 1 else 0 end) type5,sum(case when op_type = 3 or op_type = 5 then 1 else 0 end) sumType  from MS_OrderPerson left join MS_Order on op_oid = o_id");
            strSql.Append(" left join dt_manager on op_number = user_name");
            strSql.Append(" where (op_type = 3 or op_type = 5) ");
            if (strWhere.Trim() != "")
            {
                strSql.Append(strWhere);
            }
            strSql.Append(" group by op_number, op_name, detaildepart");
            //recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
            recordCount = 0;
            tCount3 = 0;
            tCount5 = 0;
            tCount = 0;
            if (isPage)
            {
                DataTable dt = DbHelperSQL.Query("select count(1) c,sum(type3) tCount3,sum(type5) tCount5,sum(sumType) tCount from(" + strSql.ToString() + ") t").Tables[0];
                if (dt != null && dt.Rows.Count > 0)
                {
                    recordCount = Utils.ObjToInt(dt.Rows[0]["c"], 0);
                    tCount3 = Utils.ObjToInt(dt.Rows[0]["tCount3"], 0);
                    tCount5 = Utils.ObjToInt(dt.Rows[0]["tCount5"], 0);
                    tCount = Utils.ObjToInt(dt.Rows[0]["tCount"], 0);
                }
                return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
            }
            else 
            {
                return DbHelperSQL.Query(strSql.ToString() + " order by  " + filedOrder);
            }
        }

        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="strWhere"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <param name="type">默认false,当为true时为订单上的未收款订单和多付款订单</param>
        /// <returns></returns>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount, out decimal money1, out decimal money2, out decimal money3, out decimal money4, out decimal money5, out decimal money6, bool isPage = true)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from (");
            strSql.Append("select *, person3 = isnull(STUFF((SELECT ',' + op_name + '(' + (case when op_dstatus = 0 then '待定' else case when op_dstatus = 1 then '处理中' else '已完成' end end) + ')' FROM MS_OrderPerson WHERE  op_oid = o_id and op_type = 3 FOR XML PATH('')), 1, 1, ''),'无'),place=stuff((select ','+p_name from MS_OrderPlace where p_oid = o_id for xml path('')),1,1,'')");
            strSql.Append(", person4 = isnull(STUFF((SELECT ',' + op_name + '(' + (case when op_dstatus = 0 then '待定' else case when op_dstatus = 1 then '处理中' else '已完成' end end) + ')' FROM MS_OrderPerson WHERE  op_oid = o_id and op_type = 5 FOR XML PATH('')), 1, 1, ''),'无')  ");
            strSql.Append(",person6 = isnull(STUFF((SELECT ',' +op_name FROM MS_OrderPerson WHERE  op_oid=o_id and op_type=6 FOR XML PATH('')), 1, 1, ''),'无')");
            strSql.Append(",(isnull(shou,0) - isnull(fu,0) - isnull(o_financeCust,0)) profit ");
            strSql.Append(" FROM MS_Order");
            strSql.Append(" left join");
            strSql.Append("(");
            strSql.Append(" select v1.fin_oid, isnull(v1.shou,0) shou,isnull(v1.shou, 0) - isnull(v2.yishou, 0) weishou,isnull(v1.fu, 0) fu,isnull(v1.fu, 0) - isnull(v2.yifu, 0) weifu from");
            strSql.Append("          (select fin_oid, sum(case when fin_type = 1 then isnull(fin_money,0) else 0 end) shou,sum(case when fin_type = 0 then isnull(fin_money, 0) else 0 end) fu from MS_finance group by fin_oid) v1 full join");
            strSql.Append("                (select rpd_oid, sum(case when rp_type = 1 and rp_isConfirm = 1 then isnull(rpd_money, 0) else 0 end) yishou, sum(case when rp_type = 0 and rp_isConfirm = 1 then isnull(rpd_money, 0) else 0 end) yifu from MS_ReceiptPayDetail left join MS_ReceiptPay on rp_id = rpd_rpid group by rpd_oid) v2 on v1.fin_oid = v2.rpd_oid");
            strSql.Append(" )t1 on o_id = t1.fin_oid");
            strSql.Append(" left join ms_customer on o_cid = c_id left join ms_contacts on o_coid = co_id left join ms_orderperson on o_id = op_oid and op_type = 1");
            strSql.Append(" ) t");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            recordCount = 0; ; money1 = 0; money2 = 0; money3 = 0; money4 = 0; money5 = 0; money6 = 0;
            if (isPage)
            {
                //recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
                DataTable dt = DbHelperSQL.Query("select count(*) c,sum(shou) shou,sum(weishou) weishou,sum(fu) fu,sum(weifu) weifu,sum(o_financeCust) o_financeCust,sum(profit) profit from (" + strSql.ToString() + ") v1").Tables[0];
                if (dt != null)
                {
                    recordCount = Utils.ObjToInt(dt.Rows[0]["c"], 0);
                    money1 = Utils.ObjToDecimal(dt.Rows[0]["shou"], 0);
                    money2 = Utils.ObjToDecimal(dt.Rows[0]["weishou"], 0);
                    money3 = Utils.ObjToDecimal(dt.Rows[0]["fu"], 0);
                    money4 = Utils.ObjToDecimal(dt.Rows[0]["weifu"], 0);
                    money5 = Utils.ObjToDecimal(dt.Rows[0]["o_financeCust"], 0);
                    money6 = Utils.ObjToDecimal(dt.Rows[0]["profit"], 0);
                }
                return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
            }
            else
            {
                return DbHelperSQL.Query(strSql.ToString() + " order by  " + filedOrder);
            }
        }
    }
}
