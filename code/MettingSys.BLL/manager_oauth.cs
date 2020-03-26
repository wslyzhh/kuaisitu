using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using MettingSys.Common;

namespace MettingSys.BLL
{
    /// <summary>
    ///OAuth授权用户信息
    /// </summary>
    public partial class manager_oauth
    {
        private readonly Model.sysconfig sysConfig = new BLL.sysconfig().loadConfig();//获得系统配置信息
        private readonly DAL.manager_oauth dal;

        public manager_oauth()
        {
            dal = new DAL.manager_oauth(sysConfig.sysdatabaseprefix);
        }

        #region 基本方法================================
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int id)
        {
            return dal.Exists(id);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(Model.manager_oauth model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Model.manager_oauth model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            return dal.Delete(id);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.manager_oauth GetModel(int id)
        {
            return dal.GetModel(id);
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
        /// 修改一列数据
        /// </summary>
        public bool UpdateField(string oauth_name, string oauth_userid, string strValue)
        {
            return dal.UpdateField(oauth_name, oauth_userid, strValue);
        }

        /// <summary>
        /// 根据开放平台和oauth_userid返回一个实体
        /// </summary>
        public Model.manager_oauth GetModel(string oauth_name, string oauth_userid)
        {
            return dal.GetModel(oauth_name, oauth_userid);
        }
        public Model.manager_oauth GetModel(string username)
        {
            return dal.GetModel(username);
        }
        #endregion
    }
}