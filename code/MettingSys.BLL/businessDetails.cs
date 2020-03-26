using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MettingSys.BLL
{
    /// <summary>
    ///业务明细
    /// </summary>
    public partial class businessDetails
    {
        private readonly DAL.businessDetails dal;

        public businessDetails()
        {
            dal = new DAL.businessDetails();
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
        public int Add(Model.businessDetails model)
        {
            return dal.Add(model);
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Model.businessDetails model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public string Delete(int id,Model.manager manager)
        {
            Model.businessDetails de = GetModel(id);
            if (de == null) return "业务明细不存在";
            if (isUse(id))
            {
                return "已被使用，不能删除";
            }
            if (dal.Delete(id))
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = id;
                logmodel.ol_title = "删除业务明细";
                logmodel.ol_content = "业务明细：" + de.de_name;
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Delete.ToString(), logmodel, manager.user_name, manager.real_name);
                return "";
            }
            return "删除失败";
        }
        
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.businessDetails GetModel(int id)
        {
            return dal.GetModel(id);
        }

        /// <summary>
        /// 获取业务明细的具体列
        /// </summary>
        /// <param name="Top"></param>
        /// <param name="strWhere"></param>
        /// <param name="filedOrder"></param>
        /// <returns></returns>
        public DataSet GetNameList(string fields, string strWhere, string filedOrder)
        {
            return dal.GetNameList(fields, strWhere, filedOrder);
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
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string title,int naid,int id=0)
        {
            return dal.Exists(title,naid,id);
        }
        /// <summary>
        /// 保存排序
        /// </summary>
        public bool UpdateSort(int id, int sort_id)
        {
            dal.UpdateField(id, "de_sort=" + sort_id);
            return true;
        }
        /// <summary>
        /// 返回已被使用的业务明细的名称
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public bool isUse(int id)
        {
            return dal.isUse(id);
        }
        #endregion
    }
}
