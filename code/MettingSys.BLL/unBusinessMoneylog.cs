using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MettingSys.BLL
{
    public partial class unBusinessMoneyLog
    {
        private readonly DAL.unBusinessMoneyLog dal;

        public unBusinessMoneyLog()
        {
            dal = new DAL.unBusinessMoneyLog();
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
        public string Add(Model.unBusinessMoneyLog model)
        {
            if (dal.Add(model) > 0)
            {
                return "";
            }
            return "添加失败";
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public string Update(Model.unBusinessMoneyLog model, string content, Model.manager manager)
        {
            if (dal.Update(model))
            {
                return "";
            }
            return "更新失败";
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public string Delete(int id)
        {
            Model.unBusinessMoneyLog model = GetModel(id);
            if (model == null)
                return "数据不存在";
            
            if (dal.Delete(id))
            {
                return "";
            }
            return "删除失败";
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.unBusinessMoneyLog GetModel(int id)
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
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder,Model.manager manager, out int recordCount)
        {
            //列表权限控制
            if (manager.area != new BLL.department().getGroupArea())//如果不是总部的工号
            {
                if (new BLL.permission().checkHasPermission(manager, "0602"))
                {
                    //含有区域权限可以查看本区域添加的
                    strWhere += " and uba_area='" + manager.area + "'";
                }
                else
                {
                    //只能
                    strWhere += " and uba_PersonNum='" + manager.user_name + "'";
                }
            }
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }
        #endregion

        #region 扩展方法================================

        #endregion
    }
}
