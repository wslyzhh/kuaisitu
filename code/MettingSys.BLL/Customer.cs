using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MettingSys.BLL
{
    public partial class Customer
    {
        private readonly DAL.Customer dal;

        public Customer()
        {
            dal = new DAL.Customer();
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
        /// 添加内部客户
        /// </summary>
        /// <param name="username"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public string AddInnerCustomer(string username, Model.manager manager)
        {
            Model.manager model =new BLL.manager().GetModel(username);
            if (model == null) return "用户不存在";
            Model.Customer cu = new Model.Customer();
            cu.c_name = "员工-" + model.real_name + "(" + model.user_name + ")";
            cu.c_type = 3;
            cu.c_flag = 2;
            cu.c_isUse = true;
            cu.c_owner = model.user_name;
            cu.c_ownerName = model.real_name;
            cu.c_addDate = DateTime.Now;

            Model.Contacts co = new Model.Contacts();
            co.co_flag = true;
            co.co_name = model.real_name;
            co.co_number = model.telephone;

            int cid = 0;
            return Add(cu, co, manager, out cid);
        }
        /// <summary>
        /// 增加一条数据
        /// </summary>
        public string Add(Model.Customer model, Model.Contacts contact, Model.manager manager, out int cid)
        {
            cid = 0;
            if (string.IsNullOrEmpty(model.c_name))
            {
                return "请填写客户名称";
            }
            if (model.c_type == 0)
            {
                return "请选择客户类别";
            }
            if (new BLL.Customer().Exists(model.c_name))
            {
                return "该客户名称已存在";
            }
            if (string.IsNullOrEmpty(contact.co_name))
            {
                return "主要联系人不能为空";
            }
            if (string.IsNullOrEmpty(contact.co_number))
            {
                return "主要联系号码不能为空";
            }
            if ((model.c_type == 2 || model.c_type == 3) && !new BLL.permission().checkHasPermission(manager, "0301"))
            {
                return "没有客户管理权限0301";                
            }
            string result = string.Empty;            
            result = dal.Add(model, contact, out cid);
            if (string.IsNullOrEmpty(result))
            {
                Model.business_log log = new Model.business_log();
                log.ol_title = "添加客户";
                log.ol_cid = cid;
                log.ol_content = "客户名称：" + model.c_name + "<br/>客户类别：" + Common.BusinessDict.customerType()[model.c_type] + "<br/>信用代码(税号)：" + model.c_num + "<br/>备注：" + model.c_remarks + "<br/>主要联系人：" + contact.co_name + "<br/>主要联系人号码：" + contact.co_number + "";
                log.ol_operateDate = DateTime.Now;
                log.ol_operaterNum = manager.user_name;
                log.ol_operaterName = manager.real_name;
                new business_log().Add(DTEnums.ActionEnum.Add.ToString(), log, manager.user_name, manager.real_name);
                return "";
            }
            return result;
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public string Update(byte? oldtype, Model.Customer model, Model.manager manager, string content)
        {
            if (model.c_flag == 2)
            {
                return "已审核通过的客户不能再修改";
            }
            if (model.c_owner != manager.user_name || model.c_ownerName != manager.real_name)
            {
                return "您不是客户所属人，不能修改客户信息";
            }
            if (string.IsNullOrEmpty(model.c_name))
            {
                return "请填写客户名称";
            }
            if (model.c_type == 0)
            {
                return "请选择客户类别";
            }
            if (Exists(model.c_name, model.c_id.Value))
            {
                return "该客户名称已存在";
            }
            if (oldtype != model.c_type)
            {
                if (oldtype == 3 || model.c_type == 3)
                {
                    return "不能更改内部客户的客户类别，或者从其他客户类别变更为内部客户";
                }
                if (oldtype == 1 && model.c_type == 2)
                {
                    if (!new BLL.permission().checkHasPermission(manager, "0301"))
                    {
                        return "没有客户管理权限0301";
                    }
                }
            }
            //else
            //{
            //    if (model.c_type == 3)
            //    {
            //        return "不能编辑内部客户";
            //    }
            //}
            if (model.c_flag == 1)
            {
                model.c_flag = 0;
            }
            if (dal.Update(model))
            {
                Model.business_log log = new Model.business_log();
                log.ol_title = "修改客户";
                log.ol_cid = model.c_id.Value;
                log.ol_content = content;
                log.ol_operateDate = DateTime.Now;
                log.ol_operaterNum = manager.user_name;
                log.ol_operaterName = manager.real_name;
                new business_log().Add(DTEnums.ActionEnum.Add.ToString(), log, manager.user_name, manager.real_name);
                return "";
            }
            return "修改失败";
        }


        /// <summary>
        /// 删除客户
        /// </summary>
        public string Delete(int id, Model.manager manager)
        {
            Model.Customer model = GetModel(id);
            if (model == null)
                return "客户不存在";
            if (model.c_flag == 2)
            {
                return "已审批通过的客户不能删除";
            }
            //判断是否含有删除客户的权限
            if (!new BLL.permission().checkHasPermission(manager, "0301"))
            {
                //如果没有删除客户的权限，判断是不是本人的客户
                if (model.c_owner != manager.user_name || model.c_ownerName != manager.real_name)
                {
                    return "没有客户管理权限0301，且不是客户所属人，不能删除客户信息";
                }
            }
            if (checkIsUse(id))
            {
                return "该客户已被使用，不能删除";
            }
            if (dal.Delete(id))
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_title = "删除客户";
                logmodel.ol_cid = id;
                logmodel.ol_content = "客户名称：" + model.c_name + "";
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Delete.ToString(), logmodel, manager.user_name, manager.real_name);
                return "";
            }
            return "删除失败";
        }
        /// <summary>
        /// 检查客户是否被使用
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool checkIsUse(int id)
        {
            return dal.checkIsUse(id);
        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.Customer GetModel(int id)
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
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder,Model.manager manager, out int recordCount, bool isPage = true,bool showAll=false)
        {
            if (!showAll)
            {
                //列表权限控制
                if (manager.area != new BLL.department().getGroupArea())//如果不是总部的工号
                {
                    if (new BLL.permission().checkHasPermission(manager, "0602"))
                    {
                        //含有区域权限可以查看本区域添加的客户
                        strWhere += " and area='" + manager.area + "'";
                    }
                    else
                    {
                        //只能
                        strWhere += " and c_owner='" + manager.user_name + "'";
                    }
                }
            }
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount,isPage);
        }
        #endregion

        #region 扩展方法================================
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string title, int id = 0)
        {
            return dal.Exists(title, id);
        }
        /// <summary>
        /// 客户审批
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="status"></param>
        /// <param name="username"></param>
        /// <param name="realname"></param>
        /// <returns></returns>
        public string checkStatus(int id, byte? status, Model.manager manager)
        {
            Model.Customer model = GetModel(id);
            if (model == null)
                return "客户不存在";
            if (model.c_flag == status)
            {
                return "状态未变更";
            }

            //2.验证有没有权限审批
            if (!new BLL.permission().checkHasPermission(manager, "0301"))
            {
                return "没有客户管理权限0301";
            }

            if (dal.updateStatus(id, status))
            {
                //写日志
                Model.business_log log = new Model.business_log();
                log.ol_title = "审批客户状态";
                log.ol_cid = id;
                log.ol_content = "审批状态：" + Common.BusinessDict.checkStatus()[model.c_flag] + "→<font color='red'>" + Common.BusinessDict.checkStatus()[status] + "</font><br/>"; ;
                new business_log().Add(DTEnums.ActionEnum.Audit.ToString(), log, manager.user_name, manager.real_name);
                return "";
            }
            return "操作失败";
        }
        /// <summary>
        /// 获取客户的具体列
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
        /// 更新所属人
        /// </summary>
        /// <param name="id"></param>
        /// <param name="newUsername"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public string updateOwner(int id, string newUsername, Model.manager manager)
        {
            //判断有没有权限更新所属人
            if (!new BLL.permission().checkHasPermission(manager, "0301"))
            {
                return "没有客户管理权限0301";
            }
            Model.Customer model = GetModel(id);
            if (model == null)
                return "客户不存在";
            if (model.c_owner == newUsername)
            {
                return "所属人未变更";
            }
            if (string.IsNullOrEmpty(newUsername))
            {
                return "请填写新的所属人工号";
            }
            Model.manager m = new BLL.manager().GetModel(newUsername);
            if (m == null)
                return "新的所属人工号不存在";
            if (dal.updateOwner(id, newUsername, m.real_name))
            {
                //写日志
                Model.business_log log = new Model.business_log();
                log.ol_title = "更新客户所属人";
                log.ol_cid = id;
                log.ol_content = "所属人：" + model.c_ownerName + "(" + model.c_owner + ")→<font color='red'>" + m.real_name + "(" + newUsername + ")</font><br/>";
                new business_log().Add(DTEnums.ActionEnum.Audit.ToString(), log, manager.user_name, manager.real_name);
                return "";
            }
            return "更新失败";
        }
        /// <summary>
        /// 合并客户
        /// </summary>
        /// <param name="cid1">源客户ID</param>
        /// <param name="cname1">源客户名称</param>
        /// <param name="cid2">目标客户ID</param>
        /// <param name="cname2">目标客户名称</param>
        /// <param name="username">操作人ID</param>
        /// <param name="realname">操作人姓名</param>
        /// <returns></returns>
        public string mergeCustomer(int cid1, string cname1, int cid2, string cname2, Model.manager manager)
        {
            if (cid1 == 0 || string.IsNullOrEmpty(cname1))
            {
                return "请填写源客户";
            }
            if (cid2 == 0 || string.IsNullOrEmpty(cname2))
            {
                return "请填写目标客户";
            }
            if (cid1 == cid2)
            {
                return "源客户和目标客户不能是同一个客户";
            }
            //判断有没有权限合并客户
            if (!new BLL.permission().checkHasPermission(manager, "0301"))
            {
                return "没有客户管理权限0301";
            }
            Model.Customer model1 = GetModel(cid1);
            Model.Customer model2 = GetModel(cid2);
            if (model1 == null || model2 == null)
            {
                return "数据异常";
            }
            if (model1.c_type != model2.c_type)
            {
                return "只能合并客户类别相同的客户";
            }
            bool _updateCustomer = false;
            StringBuilder logcontent = new StringBuilder();
            logcontent.Append("源客户：" + cid1 + "," + cname1 + "<br/>");
            logcontent.Append("目标客户：" + cid2 + "," + cname2 + "<br/>");
            if (model1.c_isUse.Value && !model2.c_isUse.Value)
            {
                model2.c_isUse = true;
                _updateCustomer = true;
                logcontent.Append("目标客户启用状态变更：禁用→<font color='red'>启用</font>");
            }
            if (dal.mergeCustomer(cid1, cname1, model2, manager.user_name, manager.real_name, _updateCustomer))
            {
                //写日志
                Model.business_log log = new Model.business_log();
                log.ol_title = "合并客户";
                log.ol_cid = cid2;
                string _content = string.Empty;
                log.ol_content = logcontent.ToString();
                new business_log().Add(DTEnums.ActionEnum.Merge.ToString(), log, manager.user_name, manager.real_name);
                return "";
            }
            return "客户合并失败";
        }
        #endregion
    }
}
