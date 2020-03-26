using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MettingSys.BLL
{
    /// <summary>
    ///业务日志表
    /// </summary>
    public partial class business_log
    {
        private readonly Model.sysconfig sysConfig = new BLL.sysconfig().loadConfig(); //获得系统配置信息
        private readonly DAL.business_log dal;

        public business_log()
        {
            dal = new DAL.business_log();
        }

        #region 基本方法================================
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(string action_type, Model.business_log model,string username,string realname)
        {
            model.ol_operaterNum = username;
            model.ol_operaterName = realname;
            model.ol_operateDate = DateTime.Now;
            model.ol_type = action_type;
            return dal.Add(model);
        }
        
        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetList(int Top, string strWhere, string filedOrder)
        {
            return dal.GetList(Top, strWhere, filedOrder);
        }

        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }
        #endregion

        #region 扩展方法================================
        /// <summary>
        /// 返回数据数
        /// </summary>
        public int GetCount(string strWhere)
        {
            return dal.GetCount(strWhere);
        }
        #endregion
    }
}
