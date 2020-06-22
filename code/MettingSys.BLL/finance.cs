using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MettingSys.BLL
{
    public partial class finance
    {
        private readonly DAL.finance dal;
        public finance()
        {
            dal = new DAL.finance();
        }

        #region 基本方法================================
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int id)
        {
            return dal.Exists(id);
        }
        public bool Update(Model.finance model)
        {
            return dal.Update(model);
        }
        /// <summary>
        /// 增加应收应付
        /// </summary>
        public string Add(Model.finance model, Model.manager manager)
        {
            try
            {
                //验证权限：财务，订单的业务员、报账人员、执行人员，才能添加应收应付
                Model.Order order = new BLL.Order().GetModel(model.fin_oid);
                if (order == null)
                {
                    return "订单不存在";
                }
                //地接的区域和订单区域保持一致
                if (string.IsNullOrEmpty(model.fin_area))
                {
                    model.fin_area = order.personlist.Where(p => p.op_type == 1).ToArray()[0].op_area;
                }
                string typeText = "应收";
                if (!model.fin_type.Value)
                {
                    typeText = "应付";
                }
                if (order.o_lockStatus==1)
                {
                    return "订单已锁单，不能再添加" + typeText + "记录";
                }
                if (!new BLL.permission().checkHasPermission(manager, "0401"))//如果不是财务
                {
                    if (order.personlist.Where(p => p.op_number == manager.user_name && p.op_type != 3).ToArray().Length == 0)
                    {
                        return "无权限添加";
                    }
                }
                if (model.fin_cid == 0)
                {
                    return "请选择" + typeText + "对象";
                }
                if (model.fin_nature == 0)
                {
                    return "请选择业务性质";
                }
                if (string.IsNullOrEmpty(model.fin_detail))
                {
                    return "请选择业务明细";
                }
                if (string.IsNullOrEmpty(model.fin_expression))
                {
                    return "请填写金额表达式";
                }
                //if (model.fin_sdate == null)
                //{
                //    return "请选择业务开始日期";
                //}
                //if (model.fin_edate == null)
                //{
                //    return "请选择业务结束日期";
                //}
                //if (DateTime.Compare(model.fin_sdate.Value, model.fin_edate.Value) > 0)
                //{
                //    return "业务开始日期不能大于业务结束日期";
                //}
                //员工费用的结束日期是<订单的结束就可以，开始日期是没有限制的;其他的需全部包含在订单日期内
                //if (model.fin_nature == 17)
                //{
                //    if (DateTime.Compare(model.fin_edate.Value, order.o_edate.Value) > 0)
                //    {
                //        return "业务结束日期不能大于订单活动结束日期";
                //    }
                //}
                //else
                //{
                //    if (DateTime.Compare(order.o_sdate.Value, model.fin_sdate.Value) > 0 || DateTime.Compare(model.fin_sdate.Value, order.o_edate.Value) > 0)
                //    {
                //        return "业务开始日期不在订单活动日期范围内";
                //    }
                //    if (DateTime.Compare(order.o_sdate.Value, model.fin_edate.Value) > 0 || DateTime.Compare(model.fin_edate.Value, order.o_edate.Value) > 0)
                //    {
                //        return "业务结束日期不在订单活动日期范围内";
                //    }
                //}


                int ret = dal.Add(model);
                if (ret > 0)
                {
                    StringBuilder content = new StringBuilder();
                    content.Append("订单号：" + model.fin_oid + "<br/>");
                    content.Append("收付性质：" + typeText + "<br/>");
                    content.Append("" + typeText + "对象ID：" + model.fin_cid + "<br/>");
                    content.Append("业务性质ID：" + model.fin_nature + "<br/>");
                    content.Append("业务明细：" + model.fin_detail + "<br/>");
                    //content.Append("业务日期：" + model.fin_sdate.Value.ToString("yyyy-MM-dd") + "/" + model.fin_edate.Value.ToString("yyyy-MM-dd") + "<br/>");
                    content.Append("业务说明：" + model.fin_illustration + "<br/>");
                    content.Append("金额表达式：" + model.fin_expression + "=" + model.fin_money + "<br/>");

                    Model.business_log logmodel = new Model.business_log();
                    logmodel.ol_relateID = ret;
                    logmodel.ol_oid = model.fin_oid;
                    logmodel.ol_cid = model.fin_cid.Value;
                    logmodel.ol_title = "添加" + typeText + "";
                    logmodel.ol_content = content.ToString();
                    logmodel.ol_operateDate = DateTime.Now;
                    new business_log().Add(DTEnums.ActionEnum.Add.ToString(), logmodel, manager.user_name, manager.real_name); //记录日志

                    return "";
                }
                return "添加失败";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }    
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public string Update(Model.finance model, string content, Model.manager manager)
        {
            if (model == null) return "数据不存在";
            Model.Order order = new BLL.Order().GetModel(model.fin_oid);
            if (order == null)
            {
                return "订单不存在";
            }
            if (!new BLL.permission().checkHasPermission(manager, "0401"))//如果不是财务
            {
                //验证权限：在同一个订单里，业务员与业务报账员可以对未审核地接进行编辑与删除！执行人员只能对自己地址进行编辑与删除操作！
                Model.businessNature na = new BLL.businessNature().GetModel(model.fin_nature.Value);
                if (na.na_flag.Value) return "无权限编辑";
                if (model.fin_personNum != manager.user_name && order.personlist.Where(p => p.op_number == manager.user_name && p.op_type != 2).ToArray().Length == 0 && order.personlist.Where(p => p.op_number == manager.user_name && (p.op_type == 3 || p.op_type == 4)).ToArray().Length > 0)
                {
                    return "无权限编辑";
                }
            }
            else
            {
                if (model.fin_personNum != manager.user_name && !new BLL.permission().checkHasPermission(manager, "0403"))
                {
                    return "无权限编辑";
                }
            }
            string typeText = "应收";
            if (!model.fin_type.Value)
            {
                typeText = "应付";
            }
            if (model.fin_flag == 2)
            {
                return "已审批通过不能再修改";
            }
            if (model.fin_cid == 0)
            {
                return "请选择" + typeText + "对象";
            }
            //if (model.fin_nature == 0)
            //{
            //    return "请选择业务性质";
            //}
            if (string.IsNullOrEmpty(model.fin_detail))
            {
                return "请选择业务明细";
            }
            if (string.IsNullOrEmpty(model.fin_expression))
            {
                return "请填写金额表达式";
            }
            //if (model.fin_sdate == null)
            //{
            //    return "请选择业务开始日期";
            //}
            //if (model.fin_edate == null)
            //{
            //    return "请选择业务结束日期";
            //}
            //if (DateTime.Compare(model.fin_sdate.Value, model.fin_edate.Value) > 0)
            //{
            //    return "业务开始日期不能大于业务结束日期";
            //}
            ////员工费用的结束日期是<订单的结束就可以，开始日期是没有限制的;其他的需全部包含在订单日期内
            //if (model.fin_nature == 17)
            //{
            //    if (DateTime.Compare(model.fin_edate.Value, order.o_edate.Value) > 0)
            //    {
            //        return "业务结束日期不能大于订单活动结束日期";
            //    }
            //}
            //else
            //{
            //    if (DateTime.Compare(order.o_sdate.Value, model.fin_sdate.Value) > 0 || DateTime.Compare(model.fin_sdate.Value, order.o_edate.Value) > 0)
            //    {
            //        return "业务开始日期不在订单活动日期范围内";
            //    }
            //    if (DateTime.Compare(order.o_sdate.Value, model.fin_edate.Value) > 0 || DateTime.Compare(model.fin_edate.Value, order.o_edate.Value) > 0)
            //    {
            //        return "业务结束日期不在订单活动日期范围内";
            //    }
            //}            
            if (model.fin_flag == 1)
            {
                model.fin_flag = 0;
                model.fin_checkName = "";
                model.fin_checkNum = "";
                model.fin_checkRemark = "";
            }
            if (dal.Update(model))
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = model.fin_id.Value;
                logmodel.ol_oid = model.fin_oid;
                logmodel.ol_cid = model.fin_cid.Value;
                logmodel.ol_title = "编辑" + typeText + "";
                logmodel.ol_content = content.ToString();
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Edit.ToString(), logmodel, manager.user_name, manager.real_name); //记录日志
                return "";
            }
            return "修改失败";
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public string Delete(int id, Model.manager manager)
        {
            Model.finance model = GetModel(id);
            if (model == null) return "数据不存在";
            if (model.fin_flag == 2) return "已审批通过，不能删除";
            Model.Order order = new BLL.Order().GetModel(model.fin_oid);
            if (order == null)
            {
                return "订单不存在";
            }
            if (!new BLL.permission().checkHasPermission(manager, "0401"))//如果不是财务
            {
                //验证权限：在同一个订单里，业务员与业务报账员可以对未审核地接进行编辑与删除！执行人员只能对自己地址进行编辑与删除操作！
                Model.businessNature na = new BLL.businessNature().GetModel(model.fin_nature.Value);
                if (na.na_flag.Value) return "无权限删除";
                if (model.fin_personNum != manager.user_name && order.personlist.Where(p => p.op_number == manager.user_name && p.op_type != 2).ToArray().Length == 0 && order.personlist.Where(p => p.op_number == manager.user_name && (p.op_type == 3 || p.op_type == 4)).ToArray().Length > 0)
                {
                    return "无权限删除";
                }
            }
            else
            {
                if (model.fin_personNum != manager.user_name && !new BLL.permission().checkHasPermission(manager, "0403"))
                {
                    return "无权限删除";
                }
            }
            string typeText = "应收";
            if (!model.fin_type.Value)
            {
                typeText = "应付";
            }
            if (dal.Delete(id))
            {
                //删除该应收付下的对账信息
                new BLL.finance_chk().delChkByFinID(id, model.fin_oid, manager);
                //日志
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = model.fin_id.Value;
                logmodel.ol_oid = model.fin_oid;
                logmodel.ol_cid = model.fin_cid.Value;
                logmodel.ol_title = "删除" + typeText + "";
                logmodel.ol_content = "金额：" + model.fin_month;
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Delete.ToString(), logmodel, manager.user_name, manager.real_name); //记录日志
                return "";
            }
            return "删除失败";
        }

        /// <summary>
        /// 更新财务备注
        /// </summary>
        /// <param name="id"></param>
        /// <param name="remark"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public string AddFinanceRemark(int id, string remark, Model.manager manager)
        {
            Model.finance model = GetModel(id);
            if (model == null) return "数据不存在";
            if (model.fin_flag != 2) return "审批通过的才能";
            return "";
        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.finance GetModel(int id)
        {
            return dal.GetModel(id);
        }
        /// <summary>
        /// 返回订单中所有的业务性质
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public DataTable getNature(string oid, string username = "")
        {
            return dal.getNature(oid, username);
        }
        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetList(int Top, string strWhere, string filedOrder)
        {
            return dal.GetList(Top, strWhere, filedOrder);
        }
        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetList(int Top, string strWhere,string chk, string filedOrder)
        {
            return dal.GetList(Top, strWhere, chk, filedOrder);
        }

        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder,Model.manager manager, out int recordCount, bool isPage = true)
        {
            recordCount = 0;
            if (manager == null)
            {
                return null;
            }
            //非集团工号只能看到订单归属地中含有本区域的记录
            if (manager.area != new BLL.department().getGroupArea())
            {
                strWhere += " and o_place like '%"+ manager.area + "%' ";
            }
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount,isPage);
        }

        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public DataSet GetList1(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount, bool isPage = true)
        {
            return dal.GetList1(pageSize, pageIndex, strWhere, filedOrder, out recordCount, isPage);
        }

        /// <summary>
        /// 审单列表
        /// </summary>
        public DataSet GetApprovalList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount, bool isPage = true)
        {
            return dal.GetApprovalList(pageSize, pageIndex, strWhere, filedOrder, out recordCount,isPage);
        }

        /// <summary>
        /// 往来客户总体统计列表
        /// </summary>
        /// <returns></returns>
        public DataSet getSettleCustomerList(string _sdate, string _edate, string _sdate1, string _edate1, string _sdate2, string _edate2, string _status, string _lockstatus, string _area, string _person1)
        {
            return dal.getSettleCustomerList(_sdate, _edate, _sdate1, _edate1, _sdate2, _edate2, _status, _lockstatus, _area, _person1);
        }
        /// <summary>
        /// 往来客户明细列表(按应收付对象分组)
        /// </summary>
        /// <returns></returns>
        public DataSet getSettleCustomerDetailList(int pageSize, int pageIndex, string _type, string _cid, string _cname, string _sdate, string _edate, string _sdate1, string _edate1, string _sdate2, string _edate2, string _status, string _sign, string _money1,string username, string _lockstatus, string _area, string _person1, string filedOrder, out int recordCount, out decimal money1, out decimal money2, out decimal money3, out decimal money4, out decimal money5, out decimal money6, bool isPage = true)
        {
            return dal.getSettleCustomerDetailList(pageSize, pageIndex, _type, _cid, _cname, _sdate, _edate, _sdate1, _edate1, _sdate2, _edate2, _status,_sign, _money1, username, _lockstatus, _area, _person1, filedOrder, out recordCount, out money1, out money2, out money3, out money4, out money5, out money6, isPage);
        }
        /// <summary>
        /// 往来客户明细列表(按业务员分组)
        /// </summary>
        /// <returns></returns>
        public DataSet getSettleCustomerDetailListByUser(int pageSize, int pageIndex, string _type, string _cid, string _cname, string _sdate, string _edate, string _sdate1, string _edate1, string _sdate2, string _edate2, string _status, string _sign, string _money1, string username, string _lockstatus, string _area, string _person1, string filedOrder, out int recordCount, out decimal money1, out decimal money2, out decimal money3, bool isPage = true)
        {
            return dal.getSettleCustomerDetailListByUser(pageSize, pageIndex, _type, _cid, _cname, _sdate, _edate, _sdate1, _edate1, _sdate2, _edate2, _status, _sign, _money1, username, _lockstatus, _area, _person1, filedOrder, out recordCount, out money1, out money2, out money3, isPage);
        }
        /// <summary>
        /// 客户对账明细
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataSet getReconciliationDetail(Dictionary<string, string> dict, int pageSize, int pageIndex, string filedOrder, out int recordCount, out decimal totalFin, out decimal totalRpd, out decimal totalUnMoney, out decimal totalChk, out decimal totalunChk,bool isPage=true)
        {
            return dal.getReconciliationDetail(dict,pageSize, pageIndex, filedOrder, out recordCount, out totalFin, out totalRpd, out totalUnMoney, out totalChk, out totalunChk,isPage);
        }
        #endregion

        #region 扩展方法================================
        /// <summary>
        /// 检查是否存在已审批的记录
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool existCheckCount(string ids, byte? status)
        {
            return dal.existCheckCount(ids, status);
        }
        /// <summary>
        /// 审批应收应付
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="remark"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public string checkStatus(int id, byte? status, string remark, Model.manager manager)
        {
            Model.finance model = GetModel(id);
            if (model == null) return "数据不存在";
            if (model.fin_flag == status) return "状态未变更";
            if (manager.area != new BLL.department().getGroupArea() || !new BLL.permission().checkHasPermission(manager, "0405"))
            {
                return "无权限审批";
            }
            Model.Order order = new BLL.Order().GetModel(model.fin_oid);
            if (order != null && order.o_lockStatus == 1)
            {
                return "订单已锁单，不能再审批";
            }
            string content = "记录id：" + id + "，审批状态：" + Common.BusinessDict.checkStatus()[model.fin_flag] + "→<font color='red'>" + Common.BusinessDict.checkStatus()[status] + "</font><br/>审批人：" + model.fin_checkNum + "（" + model.fin_checkName + "）→<font color='red'>" + manager.user_name + "（" + manager.real_name + "）</font><br/>审批备注：" + model.fin_checkRemark + "→<font color='red'>" + remark + "</font><br/>";
            model.fin_flag = status;
            model.fin_checkNum = manager.user_name;
            model.fin_checkName = manager.real_name;
            model.fin_checkRemark = remark;
            if (model.fin_flag == 0)
            {
                model.fin_checkNum = "";
                model.fin_checkName = "";
                model.fin_checkRemark = "";
            }
            if (dal.Update(model))
            {
                //写日志
                Model.business_log log = new Model.business_log();
                log.ol_title = "审批应收付记录";
                log.ol_oid = model.fin_oid;
                log.ol_cid = model.fin_cid.Value;
                log.ol_content = content;
                new business_log().Add(DTEnums.ActionEnum.Audit.ToString(), log, manager.user_name, manager.real_name);
                return "";
            }
            return "审批失败";
        }
        #endregion

        #region 财务结账
        /// <summary>
        /// 返回当前已结账的最后一个结账月份
        /// </summary>
        /// <returns></returns>
        public string getLastFinancialMonth()
        {
            return dal.getLastFinancialMonth();
        }
        /// <summary>
        /// 结账
        /// </summary>
        /// <returns></returns>
        public bool dealFinancial(string month)
        {
            return dal.dealFinancial(month);
        }

        /// <summary>
        /// 反结账
        /// </summary>
        /// <returns></returns>
        public bool cancelFinancial(string month)
        {
            return dal.cancelFinancial(month);
        }
        /// <summary>
        /// 已结账应收付总账
        /// </summary>
        /// <param name="sMonth"></param>
        /// <param name="eMonth"></param>
        /// <returns></returns>
        public DataSet getFinancialSumary(string sMonth, string eMonth, string area)
        {
            return dal.getFinancialSumary(sMonth,eMonth,area);
        }
        /// <summary>
        /// 已结账应收付明细账
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="strWhere"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <param name="isPage"></param>
        /// <returns></returns>
        public DataSet getFinancialCustomer(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount, out decimal tmoney1, out decimal tmoney2, bool isPage = true)
        {
            return dal.getFinancialCustomer(pageSize, pageIndex, strWhere, filedOrder, out recordCount, out tmoney1, out tmoney2, isPage);
        }
        /// <summary>
        /// 应收付对象已结账订单地接明细
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="strWhere"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <param name="isPage"></param>
        /// <returns></returns>
        public DataSet getFinancialOrder(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount, out decimal tmoney1, out decimal tmoney2, bool isPage = true)
        {
            return dal.getFinancialOrder(pageSize, pageIndex, strWhere, filedOrder, out recordCount, out tmoney1, out tmoney2, isPage);
        }
        #endregion
    }
}
