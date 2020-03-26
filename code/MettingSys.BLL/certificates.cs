using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MettingSys.BLL
{
    public partial class certificates
    {
        private readonly DAL.certificates dal;

        public certificates()
        {
            dal = new DAL.certificates();
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
        public string Add(Model.certificates model,out int id)
        {
            id = 0;
            if (string.IsNullOrEmpty(model.ce_num))
            {
                return "请填写凭证号";
            }
            if (model.ce_date == null)
            {
                return "请填写凭证日期";
            }
            if (Exists(model.ce_num, model.ce_date))
            {
                return "凭证号、凭证日期重复";
            }
            model.ce_flag = 0;
            id = dal.Add(model);
            if (id > 0)
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = id;
                logmodel.ol_title = "添加凭证";
                logmodel.ol_content = "凭证号：" + model.ce_num + "<br/>凭证日期：" + model.ce_date.Value.ToString("yyyy-MM-dd") + "";
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Audit.ToString(), logmodel,model.ce_personNum,model.ce_personName);
                return "添加成功";
            }
            return "添加失败";
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Model.certificates model)
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
        /// 审批凭证
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="remark"></param>
        /// <param name="username"></param>
        /// <param name="realname"></param>
        /// <returns></returns>
        public string checkCertificate(int id, byte? status, string remark, string username, string realname)
        {
            Model.certificates model = GetModel(id);
            if (model == null)
            {
                return "凭证不存在";
            }
            if (model.ce_flag == status)
            {
                return "状态未变更";
            }
            if (dal.checkCertificate(id, status, remark, username, realname))
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = id;
                logmodel.ol_title = "审批凭证";
                logmodel.ol_content = "凭证号：" + model.ce_num + "<br/>凭证日期：" + model.ce_date.Value.ToString("yyyy-MM-dd") + "<br/>状态：" + Common.BusinessDict.checkStatus()[model.ce_flag.Value] + "→<font color='red'>" + Common.BusinessDict.checkStatus()[status] + "</font>";
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Audit.ToString(), logmodel, username, realname);
                return "";
            }
            return "审批失败";
        }
        /// <summary>
        /// 获取凭证的具体列
        /// </summary>
        /// <param name="Top"></param>
        /// <param name="strWhere"></param>
        /// <param name="filedOrder"></param>
        /// <returns></returns>
        public DataSet GetNumList(string fields, string strWhere, string filedOrder)
        {
            return dal.GetNumList(fields, strWhere, filedOrder);
        }
        /// <summary>
        /// 删除凭证
        /// </summary>
        /// <param name="id"></param>
        /// <param name="username"></param>
        /// <param name="realname"></param>
        /// <returns></returns>
        public string deleteCertificate(int id, string username, string realname)
        {
            Model.certificates model = GetModel(id);
            if (model == null)
            {
                return "凭证不存在";
            }
            if (model.ce_flag == 2)
            {
                return "审批通过的不能删除";
            }

            //判断是否被使用
            DataSet ds = new BLL.ReceiptPay().GetList(0, "rp_ceid=" + id, "");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                return "该凭证已被使用，不能删除";
            }
            if (dal.Delete(id))
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = id;
                logmodel.ol_title = "删除凭证";
                logmodel.ol_content = "凭证号：" + model.ce_num + "<br/>凭证日期：" + model.ce_date.Value.ToString("yyyy-MM-dd") + "";
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Delete.ToString(), logmodel, username, realname);
                return "";
            }
            return "删除失败";
        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.certificates GetModel(int id)
        {
            return dal.GetModel(id);
        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.certificates GetModel(string num, DateTime? date)
        {
            return dal.GetModel(num, date);
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
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, string group, out int recordCount, out decimal rMoney, out decimal pMoney, bool isPage = true)
        {
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder,group, out recordCount, out rMoney, out pMoney, isPage);
        }
        #endregion

        #region 扩展方法================================
        public bool Exists(string num, DateTime? date, int id = 0)
        {
            return dal.Exists(num, date, id);
        }
        #endregion
    }
}
