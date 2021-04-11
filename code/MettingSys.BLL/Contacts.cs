using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MettingSys.BLL
{
    public partial class Contacts
    {
        private readonly DAL.Contacts dal;

        public Contacts()
        {
            dal = new DAL.Contacts();
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
        public string Add(Model.Contacts model,Model.manager manager)
        {
            if (string.IsNullOrEmpty(model.co_name))
            {
                return "请填写联系人";
            }
            if (string.IsNullOrEmpty(model.co_number))
            {
                return "请填写联系号码";
            }
            string existMsg = Exists(model.co_number);
            if (!string.IsNullOrEmpty(existMsg))
            {
                return existMsg;
            }
            Model.Customer cu = new BLL.Customer().GetModel(model.co_cid.Value);
            if (cu == null)
                return "客户不存在";
            if (cu.c_owner!=manager.user_name && cu.c_ownerName!=manager.real_name)
            {
                return "您不是该客户的所属人，不能添加联系人";
            }
            
            model.co_flag = false;
            if (dal.Add(model) > 0)
            {
                Model.business_log log = new Model.business_log();
                log.ol_cid = model.co_cid.Value;
                log.ol_title = "添加客户联系人";
                log.ol_content = "次要联系人<br/>联系人：" + model.co_name + "<br/>联系号码：" + model.co_number;
                log.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Add.ToString(), log, manager.user_name, manager.real_name);
                return "";
            }
            return "添加失败";
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public string Update(Model.Contacts model, Model.manager manager,string content)
        {
            if (string.IsNullOrEmpty(model.co_name))
            {
                return "请填写联系人";
            }
            if (string.IsNullOrEmpty(model.co_number))
            {
                return "请填写联系号码";
            }
            Model.Customer cu = new BLL.Customer().GetModel(model.co_cid.Value);
            if (cu == null)
                return "客户不存在";
            if (cu.c_flag ==2)
            {
                return "客户已经审批通过，不能编辑联系人";
            }
            if (cu.c_type == 3)
            {
                return "内部客户不能编辑";
            }
            if (cu.c_owner != manager.user_name && cu.c_ownerName != manager.real_name)
            {
                if (!new BLL.permission().checkHasPermission(manager, "0301"))
                {
                    return "不是客户所属人或无权限修改客户信息";
                }
            }
            if (cu.c_type != 2)
            {
                string existMsg = Exists(model.co_number, model.co_id.Value);
                if (!string.IsNullOrEmpty(existMsg))
                {
                    return existMsg;
                }
            }
            if (dal.Update(model))
            {
                Model.business_log log = new Model.business_log();
                log.ol_cid = model.co_cid.Value;
                log.ol_title = "修改客户联系人";
                log.ol_content = content;
                log.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Edit.ToString(), log, manager.user_name, manager.real_name);
                return "";
            }
            return "修改失败";
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public string Delete(int id, Model.manager manager)
        {
            if (checkIsUse(id))
            {
                return "该联系人已被订单使用，不能删除";
            }
            Model.Contacts model = GetModel(id);
            if (model == null)
                return "联系人不存在";
            Model.Customer cu = new BLL.Customer().GetModel(model.co_cid.Value);
            if (cu == null)
                return "客户不存在";
            if (cu.c_flag == 2)
            {
                return "客户已经审批通过，不能删除联系人";
            }
            if (model.co_flag.Value)
            {
                return "不能删除主要联系人";
            }
            if (cu.c_owner != manager.user_name && cu.c_ownerName != manager.real_name)
            {
                if (!new BLL.permission().checkHasPermission(manager, "0301"))
                {
                    return "不是客户所属人或无权限修改客户信息，不能删除联系人";
                }
            }
            if (dal.Delete(id))
            {
                Model.business_log log = new Model.business_log();
                log.ol_cid = model.co_cid.Value;
                log.ol_title = "删除客户联系人";
                log.ol_content = "次要联系人<br/>联系人：" + model.co_name + "<br/>联系号码：" + model.co_number;
                log.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Edit.ToString(), log, manager.user_name, manager.real_name);
                return "";
            }
            return "删除失败";
        }
        
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.Contacts GetModel(int id)
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
        public string Exists(string phone, int id = 0)
        {
            DataSet ds = dal.Exists(phone, id);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                return "客户【" + ds.Tables[0].Rows[0]["c_name"] + "】，联系人【" + ds.Tables[0].Rows[0]["co_name"] + "】的联系号码也是【" + ds.Tables[0].Rows[0]["co_number"] + "】，请查实：是否为同一客户！并联系客户管理人员修改！";
            }
            return "";
        }
        /// <summary>
        /// 是否存在主要联系人
        /// </summary>
        public bool ExistsMainContact(int cid, int id = 0)
        {
            return dal.ExistsMainContact(cid, id);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strWhere"></param>
        /// <param name="filedOrder"></param>
        /// <returns></returns>
        public List<Model.Contacts> getList(string strWhere, string filedOrder)
        {
            return dal.getList(strWhere, filedOrder);
        }

        /// <summary>
        /// 检查联系人是否被使用
        /// </summary>
        /// <param name="coid"></param>
        /// <returns></returns>
        public bool checkIsUse(int coid)
        {
            return dal.checkIsUse(coid);
        }
        #endregion
    }
}
