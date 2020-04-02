using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MettingSys.BLL
{
    public partial class customerBank
    {
        private readonly DAL.customerBank dal;

        public customerBank()
        {
            dal = new DAL.customerBank();
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
        public string Add(Model.customerBank model,Model.manager manager)
        {
            if (model.cb_cid == 0)
            {
                return "请选择客户";
            }
            if (string.IsNullOrEmpty(model.cb_bankName))
            {
                return "请填写银行账户名称";
            }
            if (string.IsNullOrEmpty(model.cb_bankNum))
            {
                return "请填写客户银行账号";
            }
            if (Exists(model.cb_bankNum, model.cb_cid.Value))
            {
                return "该银行账号已存在";
            }
            if (string.IsNullOrEmpty(model.cb_bank))
            {
                return "请填写开户行";
            }
            if (string.IsNullOrEmpty(model.cb_bankAddress))
            {
                return "请填写开户地址";
            }
            int ret = dal.Add(model);
            if ( ret > 0)
            {
                Model.business_log log = new Model.business_log();
                log.ol_cid = model.cb_cid.Value;
                log.ol_relateID = ret;
                log.ol_title = "添加客户银行账号";
                log.ol_content = "银行账户名称：" + model.cb_bankName + "<br/>客户银行账号：" + model.cb_bankNum + "<br/>开户行：" + model.cb_bank + "<br/>开户地址：" + model.cb_bankAddress + "<br/>状态：" + (model.cb_flag.Value ? "启用" : "禁用");
                log.ol_operateDate = DateTime.Now;
                log.ol_operaterNum = manager.user_name;
                log.ol_operaterName = manager.real_name;
                new business_log().Add(DTEnums.ActionEnum.Add.ToString(), log, manager.user_name, manager.real_name);
                return "";
            }
            return "添加失败";
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public string Update(Model.customerBank model,string content, Model.manager manager)
        {
            if (string.IsNullOrEmpty(model.cb_bankName))
            {
                return "请填写银行账户名称";
            }
            if (string.IsNullOrEmpty(model.cb_bankNum))
            {
                return "请填写客户银行账号";
            }
            if (Exists(model.cb_bankNum, model.cb_cid.Value, model.cb_id.Value))
            {
                return "该银行账号已存在";
            }
            if (string.IsNullOrEmpty(model.cb_bank))
            {
                return "请填写开户行";
            }
            if (string.IsNullOrEmpty(model.cb_bankAddress))
            {
                return "请填写开户地址";
            }
            if (dal.Update(model))
            {
                Model.business_log log = new Model.business_log();
                log.ol_cid = model.cb_cid.Value;
                log.ol_relateID = model.cb_id.Value;
                log.ol_title = "编辑客户银行账号";
                log.ol_content = content;
                log.ol_operateDate = DateTime.Now;
                log.ol_operaterNum = manager.user_name;
                log.ol_operaterName = manager.real_name;
                new business_log().Add(DTEnums.ActionEnum.Edit.ToString(), log, manager.user_name, manager.real_name);
                return "";
            }
            return "更新失败";
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public string Delete(int id, Model.manager manager)
        {
            Model.customerBank na = GetModel(id);
            if (na == null) return "数据不存在";
            //if (isUse(id))
            //{
            //    return "已被使用，不能删除";
            //}
            if (dal.Delete(id))
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = id;
                logmodel.ol_title = "删除客户银行账号";
                logmodel.ol_content = "客户ID：" + na.cb_cid + "<br/>银行账户名称：" + na.cb_bankName + "<br/>客户银行账号：" + na.cb_bankNum + "";
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Delete.ToString(), logmodel, manager.user_name, manager.real_name);
                return "";
            }
            return "删除失败";
        }
        
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.customerBank GetModel(int id)
        {
            return dal.GetModel(id);
        }

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetList(int Top, string strWhere, string filedOrder, bool selectField = false)
        {
            return dal.GetList(Top, strWhere, filedOrder,selectField);
        }
        /// <summary>
        /// 根据客户id获得前几行数据
        /// </summary>
        public DataSet GetList(int cusid)
        {
            if (cusid==0)
            {
                return null;
            }
            return dal.GetList(cusid);
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
        public bool Exists(string banknum, int cid, int id = 0)
        {
            return dal.Exists(banknum,cid, id);
        }
        #endregion
    }
}
