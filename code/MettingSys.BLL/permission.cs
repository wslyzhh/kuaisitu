using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MettingSys.BLL
{
    public partial class permission
    {
        private readonly DAL.permission dal;
        public permission()
        {
            dal = new DAL.permission();
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
        public string Add(Model.permission model, string username, string realname)
        {
            if (string.IsNullOrEmpty(model.pe_code))
            {
                return "请填写权限代码";
            }
            if (Exists(model.pe_code))
            {
                return "权限代码已存在";
            }
            int ret = dal.Add(model);
            if (ret > 0)
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = ret;
                logmodel.ol_title = "添加权限";
                logmodel.ol_content = "权限代码：" + model.pe_code + "，权限名称：" + model.pe_name + "，备注：" + model.pe_remark;
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Add.ToString(), logmodel, username, realname);
                return "";
            }
            return "添加失败";
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public string Update(Model.permission model, string content, string username, string realname)
        {
            if (string.IsNullOrEmpty(model.pe_code))
            {
                return "请填写权限代码";
            }
            if (Exists(model.pe_code, model.pe_id.Value))
            {
                return "权限代码已存在";
            }
            if (dal.Update(model))
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = model.pe_id.Value;
                logmodel.ol_title = "修改权限";
                logmodel.ol_content = content;
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Edit.ToString(), logmodel, username, realname);
                return "";
            }
            return "修改失败";
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public string Delete(int id, string username, string realname)
        {
            if (dal.isUse(id))
            {
                return "已被使用，不能删除";
            }
            if (dal.Delete(id))
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = id;
                logmodel.ol_title = "删除权限";
                logmodel.ol_content = "权限ID：" + id;
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Delete.ToString(), logmodel, username, realname);
                return "";
            }
            return "删除失败";
        }
        /// <summary>
        /// 检查是否具有什么权限
        /// </summary>
        /// <param name="manager">员工信息</param>
        /// <param name="code">权限编码，多个用“,”隔开，如："0401","0402","0301"</param>
        /// <returns></returns>
        public bool checkHasPermission(Model.manager manager, string code)
        {
            if (manager == null) return false;
            if (string.IsNullOrEmpty(code)) return false;
            string[] list = code.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in list)
            {
                //判断用户的角色权限
                if (manager.RolePemissionList != null && manager.RolePemissionList.Where(p => p.urp_code == item).ToArray().Length > 0)
                {
                    return true;
                }
                //判断用户的权限
                if (manager.UserPemissionList !=null && manager.UserPemissionList.Where(p => p.urp_code == item).ToArray().Length > 0)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 返回含有某个权限的工号
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public DataTable getUserNameByPermission(string code)
        {
            if (string.IsNullOrEmpty(code))
                return null;
            return dal.getUserNameByPermission(code);            
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.permission GetModel(int id)
        {
            return dal.GetModel(id);
        }
        /// <summary>
        /// 取得所有列表
        /// </summary>
        public DataTable GetList(int parent_id)
        {
            return dal.GetList(parent_id);
        }
        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetUserRolePermissionList(int Top, string strWhere, string filedOrder)
        {
            return dal.GetUserRolePermissionList(Top, strWhere, filedOrder);
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
        public bool Exists(string title, int id = 0)
        {
            return dal.Exists(title, id);
        }
        #endregion
    }
}
