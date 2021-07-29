using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MettingSys.BLL
{
    public partial class statisticBLL
    {
        private readonly DAL.statisticDAL dal;
        public statisticBLL()
        {
            dal = new DAL.statisticDAL();
        }

        /// <summary>
        /// 员工业绩统计
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataSet getAchievementStatisticData(Dictionary<string, string> dict, int pageSize, int pageIndex, string filedOrder, out int recordCount, out int tCount, out decimal tOrderShou, out decimal tUnIncome, out decimal tOrderFu, out decimal tUnCost, out decimal tOrderTicheng, out decimal tCust, out decimal tProfit1, out decimal tProfit2, bool isPage = true)
        {
            return dal.getAchievementStatisticData(dict, pageSize, pageIndex, filedOrder, out recordCount, out tCount,out tOrderShou,out tUnIncome, out tOrderFu,out tUnCost, out tOrderTicheng, out tCust, out tProfit1, out tProfit2, isPage);
        }
        /// <summary>
        /// 员工业绩明细
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataSet getAchievementStatisticDetail(Dictionary<string, string> dict, int pageSize, int pageIndex, string filedOrder, out int recordCount, out decimal tOrderShou, out decimal tUnIncome, out decimal tOrderFu, out decimal tUnCost, out decimal tOrderTicheng, out decimal tCust, out decimal tProfit1, out decimal tProfit2, bool isPage = true)
        {
            return dal.getAchievementStatisticDetail(dict, pageSize, pageIndex, filedOrder, out recordCount, out tOrderShou, out tUnIncome, out tOrderFu, out tUnCost, out tOrderTicheng, out tCust, out tProfit1, out tProfit2, isPage);
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
        public DataSet getAreaAchievementStatisticData(Dictionary<string, string> dict, int pageSize, int pageIndex, string filedOrder, out int recordCount, out int tCount, out decimal tShou, out decimal tUnIncome, out decimal tFu, out decimal tUnCost, out decimal tCust, out decimal tTicheng, out decimal tProfit1, out decimal tProfit2, bool isPage = true)
        {
            return dal.getAreaAchievementStatisticData(dict, pageSize, pageIndex, filedOrder, out recordCount, out tCount, out tShou,out tUnIncome, out tFu,out tUnCost, out tCust, out tTicheng,out tProfit1,out tProfit2,isPage);
        }
        /// <summary>
        /// 区域业绩明细
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataSet getAreaAchievementStatisticDeatil(Dictionary<string, string> dict, int pageSize, int pageIndex, string filedOrder, out int recordCount,out decimal tShou, out decimal tUnIncome, out decimal tFu, out decimal tUnCost, out decimal tCust, out decimal tTicheng, out decimal tProfit1, out decimal tProfit2, bool isPage = true)
        {
            return dal.getAreaAchievementStatisticDeatil(dict, pageSize, pageIndex, filedOrder, out recordCount, out tShou, out tUnIncome, out tFu, out tUnCost, out tCust, out tTicheng, out tProfit1, out tProfit2, isPage);
        }
        /// <summary>
        /// 客源收益分析明细列表
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataSet getRevenueAnalysisListData(Dictionary<string, string> dict, int pageSize, int pageIndex, string filedOrder, out int recordCount, out decimal tShou, out decimal tFu, out decimal tProfit, bool isPage=true)
        {
            return dal.getRevenueAnalysisListData(dict, pageSize, pageIndex, filedOrder, out recordCount,out tShou,out tFu,out tProfit, isPage);
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
        public DataSet getRevenueAnalysisListData1(Dictionary<string, string> dict, int pageSize, int pageIndex, string selectFiled,string groupFiled,string filedOrder, out int recordCount, out decimal tShou, out decimal tFu, out decimal tProfit, bool isPage = true)
        {
            return dal.getRevenueAnalysisListData1(dict, pageSize, pageIndex,selectFiled, groupFiled, filedOrder, out recordCount, out tShou, out tFu, out tProfit, isPage);
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
            return dal.getRevenueAnalysisListData2(dict, pageSize, pageIndex, selectFiled, filedOrder, out recordCount, out tShou, out tFu, out tProfit, isPage);
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
            return dal.getRevenueAnalysisListData3(dict, pageSize, pageIndex, selectFiled, filedOrder, out recordCount, out tShou, out tFu, out tProfit, isPage);
        }
        /// <summary>
        /// 供应商支出分析明细列表
        /// </summary>
        /// <param name="dict"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataSet getExpendAnalyzeData(Dictionary<string, string> dict, int pageSize, int pageIndex, string filedOrder, out int recordCount, out decimal tFu, bool isPage = true)
        {
            return dal.getExpendAnalyzeData(dict, pageSize, pageIndex, filedOrder, out recordCount, out tFu, isPage);
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
            return dal.getExpendAnalyzeData1(dict, pageSize, pageIndex, selectFiled,groupFiled, filedOrder, out recordCount, out tFu, isPage);
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
        public DataSet getReceiveOrderAnalyzeData(int pageSize, int pageIndex, string strWhere, string filedOrder, out int tCount3, out int tCount5, out int tCount, out int recordCount, bool isPage = true)
        {
            return dal.getReceiveOrderAnalyzeData(pageSize, pageIndex, strWhere, filedOrder,out tCount3,out tCount5,out tCount, out recordCount,isPage);
        }

        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount, out decimal money1, out decimal money2, out decimal money3, out decimal money4, out decimal money5, out decimal money6, bool isPage=true)
        {
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount, out money1, out money2, out money3, out money4, out money5, out money6, isPage);
        }
    }
}
