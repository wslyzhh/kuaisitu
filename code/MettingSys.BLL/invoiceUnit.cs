using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MettingSys.BLL
{
    /// <summary>
    /// 收付款方式
    /// </summary>
    public partial class invoiceUnit
    {
        private readonly DAL.invoiceUnit dal;

        public invoiceUnit()
        {
            dal = new DAL.invoiceUnit();
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
        public string Add(Model.invoiceUnit model, Model.manager manager)
        {
            if (string.IsNullOrEmpty(model.invU_area))
            {
                return "请选择所属区域";
            }
            if (string.IsNullOrEmpty(model.invU_name))
            {
                return "请填写开票单位";
            }
            if (Exists(model.invU_area,model.invU_name))
            {
                return "该区域下已存在相同的开票单位，请检查";
            }
            int ret = dal.Add(model);
            if (ret > 0)
            {
                StringBuilder content = new StringBuilder();
                content.Append("所属区域：" + model.invU_area + "<br/>");
                content.Append("开票单位：" + model.invU_name + "<br/>");
                content.Append("联系人：" + model.invU_contact + "<br/>");
                content.Append("联系电话：" + model.invU_contactPhone + "<br/>");
                content.Append("启用状态：" + (model.invU_flag.Value ? "启用" : "禁用") + "");

                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = ret;
                logmodel.ol_title = "添加开票单位";
                logmodel.ol_content = content.ToString();
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Add.ToString(), logmodel, manager.user_name, manager.real_name); //记录日志
                return "";
            }
            return "添加失败";
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public string Update(Model.invoiceUnit model,string content,Model.manager manager)
        {
            if (model == null)
            {
                return "数据不存在";
            }
            if (string.IsNullOrEmpty(model.invU_area))
            {
                return "请选择所属区域";
            }
            if (string.IsNullOrEmpty(model.invU_name))
            {
                return "请填写开票单位";
            }
            if (Exists(model.invU_area, model.invU_name,model.invU_id.Value))
            {
                return "该区域下已存在相同的开票单位，请检查";
            }
            if (dal.Update(model))
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = model.invU_id.Value;
                logmodel.ol_title = "编辑开票单位";
                logmodel.ol_content = content;
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Add.ToString(), logmodel, manager.user_name, manager.real_name); //记录日志
                return "";
            }
            return "更新失败";
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public string Delete(int id,Model.manager manager)
        {
            Model.invoiceUnit na = GetModel(id);
            if (na == null) return "开票单位不存在";
            if (isUse(id))
            {
                return "已被使用，不能删除";
            }
            if (dal.Delete(id))
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = id;
                logmodel.ol_title = "删除开票单位";
                logmodel.ol_content = "开票单位：" + na.invU_name;
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Delete.ToString(), logmodel, manager.user_name, manager.real_name);
                return "";
            }
            return "删除失败";
        }

        public bool isUse(int id)
        {
            return dal.isUse(id);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.invoiceUnit GetModel(int id)
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
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string area,string unit, int id = 0)
        {
            return dal.Exists(area, unit, id);
        }
        #endregion
    }
}
