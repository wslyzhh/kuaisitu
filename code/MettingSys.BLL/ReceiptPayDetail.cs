using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MettingSys.BLL
{
    public partial class ReceiptPayDetail
    {
        private readonly DAL.ReceiptPayDetail dal;
        public ReceiptPayDetail()
        {
            dal = new DAL.ReceiptPayDetail();
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
        /// 某个订单号下的该应收付对象的该对账标识下存在已分配款
        /// </summary>
        public bool Exists(string oid, string chk, int cid)
        {
            return dal.Exists(oid, chk, cid);
        }
        /// <summary>
        /// 分配或取消分配
        /// </summary>
        /// <param name="model"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public string AddOrCancleDistribution(Model.ReceiptPayDetail model,Model.manager manager)
        {
            int ret = 0;
            Model.Order order = new BLL.Order().GetModel(model.rpd_oid);
            if (order == null)
            {
                return "订单不存在";
            }
            StringBuilder content = new StringBuilder();
            Model.business_log logmodel = new Model.business_log();
            if (model.rpd_money == 0)
            {
                ret = dal.delDistribution(model.rpd_rpid.Value, model.rpd_oid, model.rpd_cid.Value, model.rpd_num);
                logmodel.ol_title = "取消分配";
            }
            else
            {
                //收付款明细申请中的“收付款内容”＝预收付款申请的“收付款内容”
                model.rpd_content = new BLL.ReceiptPay().GetModel(model.rpd_rpid.Value).rp_content;
                //明细的区域和订单区域保持一致
                model.rpd_area = order.personlist.Where(p => p.op_type == 1).ToArray()[0].op_area;
                dal.delDistribution(model.rpd_rpid.Value, model.rpd_oid, model.rpd_cid.Value, model.rpd_num,"add");
                ret = dal.Add(model);
                logmodel.ol_title = "新增分配";
            }
            if (ret > 0)
            {
                logmodel.ol_oid = model.rpd_oid;
                logmodel.ol_cid = model.rpd_cid.Value;
                content.Append("金额：" + model.rpd_money + "");
                logmodel.ol_content = content.ToString();
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Distribute.ToString(), logmodel, manager.user_name, manager.real_name); //记录日志
                return "成功";
            }
            return "失败";
        }
        /// <summary>
        /// 增加收付款明细
        /// </summary>
        public string AddReceiptPay(Model.ReceiptPayDetail model, Model.manager manager,out int id)
        {
            id = 0;
            string typeText = "收款";
            if (!model.rpd_type.Value) typeText = "付款";
            Model.Order order = new BLL.Order().GetModel(model.rpd_oid);
            if (order == null) return "订单不存在";
            if (!new BLL.permission().checkHasPermission(manager, "0401"))//如果不是财务
            {
                if (order.personlist.Where(p => p.op_number == manager.user_name && p.op_type != 3).ToArray().Length == 0)
                {
                    return "无权限添加";
                }
            }
            //收付款明细的区域和订单区域保持一致
            model.rpd_area = order.personlist.Where(p => p.op_type == 1).ToArray()[0].op_area;
            if (model.rpd_cid == 0)
            {
                return "请选择" + typeText + "对象";
            }
            if (model.rpd_money == 0)
            {
                return "请填写" + typeText + "金额";
            }
            if (model.rpd_foredate == null)
            {
                return "请选择" + (model.rpd_type.Value ? "预收" : "预付") + "日期";
            }
            if (model.rpd_type.Value && model.rpd_method == 0)
            {
                return "请选择收款方式";
            }
            Model.ReceiptPay rp = null;
            if (model.rpd_type.Value)
            {
                rp = new Model.ReceiptPay();
                rp.rp_type = true;
                rp.rp_cid = model.rpd_cid;
                rp.rp_content = model.rpd_content;
                rp.rp_money = model.rpd_money;
                rp.rp_foredate = model.rpd_foredate;
                rp.rp_method = model.rpd_method;
                rp.rp_personNum = model.rpd_personNum;
                rp.rp_personName = model.rpd_personName;
                rp.rp_isConfirm = false;
                rp.rp_isExpect = false;
                rp.rp_flag = 2;
                if (rp.rp_money < 0)
                {
                    rp.rp_flag = 0;
                    rp.rp_flag1 = 0;
                }
                rp.rp_adddate = model.rpd_adddate;
                rp.rp_area = model.rpd_area;
                rp.rp_cbid = model.rpd_cbid;
            }
            else
            {
                if (model.rpd_cbid == 0)
                {
                    return "请选择客户银行账号";
                }
                //负数金额的业务付款明细申请自动审批通过，不需要人工审批！还有负数金额的业务付款明细申请不受“应付 - 已下付款申请〉＝0”限制
                if (model.rpd_money < 0)
                {
                    model.rpd_flag1 = 2;
                    model.rpd_checkTime1 = DateTime.Now;
                    model.rpd_flag2 = 2;
                    model.rpd_checkTime2 = DateTime.Now;
                    model.rpd_flag3 = 2;
                    model.rpd_checkTime3 = DateTime.Now;
                }
                else
                {
                    decimal? money = getUnPayMoney(model.rpd_cid.Value, model.rpd_oid);
                    if (model.rpd_money > money)
                    {
                        return "金额不能超过：" + money;
                    }
                }
            }
            
            id = dal.Add(model, rp);
            if (id > 0)
            {
                StringBuilder content = new StringBuilder();
                content.Append("订单号：" + model.rpd_oid + "<br/>");
                content.Append("" + typeText + "对象ID：" + model.rpd_cid + "<br/>");
                content.Append("" + typeText + "金额：" + model.rpd_money + "<br/>");
                content.Append("预收日期：" + model.rpd_foredate.Value.ToString("yyyy-MM-dd") + "<br/>");
                if (model.rpd_type.Value)
                {
                    content.Append("" + typeText + "方式ID：" + model.rpd_method + "<br/>");
                }
                content.Append("" + typeText + "内容：" + model.rpd_content + "<br/>");

                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_oid = model.rpd_oid;
                logmodel.ol_relateID = id;
                logmodel.ol_cid = model.rpd_cid.Value;
                logmodel.ol_title = "添加" + typeText + "明细";
                logmodel.ol_content = content.ToString();
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Add.ToString(), logmodel, manager.user_name, manager.real_name); //记录日志

                //钉钉通知
                if (!model.rpd_type.Value)
                {
                    DataTable userDt = new BLL.manager().getUserByPermission("0603", model.rpd_area).Tables[0];
                    if (userDt != null)
                    {
                        string replaceContent = model.rpd_oid + "," + model.rpd_money + "," + model.rpd_content;
                        string replaceUser = model.rpd_personNum + "," + model.rpd_personName;
                        foreach (DataRow dr in userDt.Rows)
                        {                           
                            //钉钉推送通知
                            if (!string.IsNullOrEmpty(Utils.ObjectToStr(dr["oauth_userid"])))
                            {
                                new BLL.selfMessage().sentDingMessage("添加付款通知", dr["oauth_userid"].ToString(), replaceContent, replaceUser);
                            }
                        }
                    }
                }

                return "";
            }
            return "添加失败";
        }

        /// <summary>
        /// 编辑收付款明细
        /// </summary>
        /// <param name="model"></param>
        /// <param name="content"></param>
        /// <param name="manager"></param>
        /// <param name="updateMoney">是否修改了收款明细的金额</param>
        /// <returns></returns>
        public string Update(Model.ReceiptPayDetail model, string content, Model.manager manager, bool updateMoney = false,bool updateMethod=false)
        {
            if (model == null) return "数据不存在";
            Model.Order order = new BLL.Order().GetModel(model.rpd_oid);
            if (order == null)
            {
                return "订单不存在";
            }
            if (!new BLL.permission().checkHasPermission(manager, "0401"))//如果不是财务
            {
                //验证权限：在同一个订单里，业务员与业务报账员可以对未审核地接进行编辑与删除！执行人员只能对自己地址进行编辑与删除操作！
                if (model.rpd_personNum != manager.user_name && order.personlist.Where(p => p.op_number == manager.user_name && (p.op_type == 3 || p.op_type == 4)).ToArray().Length > 0)
                {
                    return "无权限编辑";
                }
            }
            else
            {
                if (model.rpd_personNum != manager.user_name && !new BLL.permission().checkHasPermission(manager, "0403"))
                {
                    return "无权限编辑";
                }
            }
            string typeText = "收款";
            if (!model.rpd_type.Value) typeText = "付款";

            if (model.rpd_cid == 0)
            {
                return "请选择" + typeText + "对象";
            }
            if (model.rpd_money == 0)
            {
                return "请填写" + typeText + "金额";
            }
            if (model.rpd_foredate == null)
            {
                return "请选择" + (model.rpd_type.Value ? "预收" : "预付") + "日期";
            }
            bool flag = false;
            if (model.rpd_type.Value)
            {
                if (model.rpd_method == 0)
                {
                    return "请选择收款方式";
                }
                if (model.rpd_rpid > 0)
                {
                    Model.ReceiptPay rp = new BLL.ReceiptPay().GetModel(model.rpd_rpid.Value);
                    if (rp.rp_isConfirm.Value)
                    {
                        return "已确认收款，不能再编辑";
                    }
                }
                flag = UpdateReceiptDetail(model, updateMoney);
            }
            else
            {
                if (updateMethod)
                {
                    if (model.rpd_rpid > 0)
                    {
                        return "已经汇总的不能修改付款方式";
                    }
                }
                else if (updateMoney)
                {
                    if (model.rpd_money >= 0)
                    {
                        model.rpd_flag1 = 0;
                        model.rpd_checkNum1 = "";
                        model.rpd_checkName1 = "";
                        model.rpd_checkRemark1 = "";
                        model.rpd_checkTime1 = null;
                        model.rpd_flag2 = 0;
                        model.rpd_checkNum2 = "";
                        model.rpd_checkName2 = "";
                        model.rpd_checkRemark2 = "";
                        model.rpd_checkTime2 = null;
                        model.rpd_flag3 = 0;
                        model.rpd_checkNum3 = "";
                        model.rpd_checkName3 = "";
                        model.rpd_checkRemark3 = "";
                        model.rpd_checkTime3 = null;
                    }
                    else
                    {
                        model.rpd_flag1 = 2;
                        model.rpd_checkTime1 = DateTime.Now;
                        model.rpd_flag2 = 2;
                        model.rpd_checkTime2 = DateTime.Now;
                        model.rpd_flag3 = 2;
                        model.rpd_checkTime3 = DateTime.Now;
                    }
                }
                else if (model.rpd_money >=0 && model.rpd_flag1 == 2 && model.rpd_flag2 != 1 && model.rpd_flag3 != 1)
                {
                    return "部门审批已通过，不能再编辑";
                }
                //编辑后清空审批内容,如果只是修改付款方式则不需要情况 2020-05-30
                if ((model.rpd_flag1 == 1 || model.rpd_flag2 == 1 || model.rpd_flag3 == 1) && !updateMethod)
                {
                    model.rpd_flag1 = 0;
                    model.rpd_checkNum1 = "";
                    model.rpd_checkName1 = "";
                    model.rpd_checkRemark1 = "";
                    model.rpd_checkTime1 = null;
                    model.rpd_flag2 = 0;
                    model.rpd_checkNum2 = "";
                    model.rpd_checkName2 = "";
                    model.rpd_checkRemark2 = "";
                    model.rpd_checkTime2 = null;
                    model.rpd_flag3 = 0;
                    model.rpd_checkNum3 = "";
                    model.rpd_checkName3 = "";
                    model.rpd_checkRemark3 = "";
                    model.rpd_checkTime3 = null;
                }
                flag = dal.Update(model);
            }

            if (flag)
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_oid = model.rpd_oid;
                logmodel.ol_relateID = model.rpd_id.Value;
                logmodel.ol_cid = model.rpd_cid.Value;
                logmodel.ol_title = "编辑" + typeText + "明细";
                logmodel.ol_content = content.ToString();
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Edit.ToString(), logmodel, manager.user_name, manager.real_name); //记录日志

                //钉钉通知
                if (!model.rpd_type.Value && !string.IsNullOrEmpty(content))
                {
                    DataTable userDt = new BLL.manager().getUserByPermission("0603", model.rpd_area).Tables[0];
                    if (userDt != null)
                    {
                        string replaceContent = model.rpd_oid + "," + model.rpd_money + "," + model.rpd_content;
                        string replaceUser = model.rpd_personNum + "," + model.rpd_personName;
                        foreach (DataRow dr in userDt.Rows)
                        {
                            //钉钉推送通知
                            if (!string.IsNullOrEmpty(Utils.ObjectToStr(dr["oauth_userid"])))
                            {
                                new BLL.selfMessage().sentDingMessage("添加付款通知", dr["oauth_userid"].ToString(), replaceContent, replaceUser);
                            }
                        }
                    }
                }

                return "";
            }
            return "编辑失败";
        }
        /// <summary>
        /// 付款明细汇总页面批量填写付款方式
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="method"></param>
        /// <param name="sdate"></param>
        /// <param name="edate"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public string mutliUpdateMethod(int cid, int method,int oldmethod, string sdate, string edate, Model.manager manager)
        {
            if (!new BLL.permission().checkHasPermission(manager, "0401"))//如果不是财务
            {
                return "无权限编辑";
            }
            if (oldmethod==0 && method == 0)
            {
                return "请选择付款方式";
            }
            if (oldmethod == method)
            {
                return "付款方式未改变";
            }
            return dal.mutliUpdateMethod(cid, method, oldmethod, sdate, edate, manager) > 0 ? "" : "失败";
        }


        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool UpdateReceiptDetail(Model.ReceiptPayDetail model, bool updateMoney)
        {
            return dal.UpdateReceiptDetail(model, updateMoney);
        }

        /// <summary>
        /// 删除收付款明细
        /// </summary>
        public string Delete(int id, Model.manager manager)
        {
            Model.ReceiptPayDetail model = GetModel(id);
            if (model == null) return "数据不存在";
            Model.Order order = new BLL.Order().GetModel(model.rpd_oid);
            if (order == null)
            {
                return "订单不存在";
            }
            if (!new BLL.permission().checkHasPermission(manager, "0401"))//如果不是财务
            {
                //验证权限：在同一个订单里，业务员与业务报账员可以对未审核地接进行编辑与删除！执行人员只能对自己地址进行编辑与删除操作！
                if (model.rpd_personNum != manager.user_name && order.personlist.Where(p => p.op_number == manager.user_name && (p.op_type == 3 || p.op_type == 4)).ToArray().Length > 0)
                {
                    return "无权限删除";
                }
            }
            else
            {
                if (model.rpd_personNum != manager.user_name && !new BLL.permission().checkHasPermission(manager, "0403"))
                {
                    return "非申请人或没有删除他人数据权限不能删除";
                }
            }
            bool flag = false;
            if (model.rpd_type.Value)
            {
                if (model.rpd_rpid > 0)
                {
                    Model.ReceiptPay rp = new BLL.ReceiptPay().GetModel(model.rpd_rpid.Value);
                    if (rp.rp_isConfirm.Value)
                    {
                        return "已确认收款，不能再删除";
                    }
                }
                flag = dal.DeleteReceiptDetail(model);
            }
            else
            {
                if (model.rpd_flag3 == 2 && model.rpd_money >=0)
                {
                    return "最终审批通过不能再编辑";
                }
                flag = dal.Delete(id);
                if (flag)
                {
                    //删除图片附件
                    new BLL.payPic().deleteFileByid(id, 1);
                }
            }

            string typeText = "收款";
            if (!model.rpd_type.Value) typeText = "付款";
            if (flag)
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_oid = model.rpd_oid;
                logmodel.ol_relateID = model.rpd_id.Value;
                logmodel.ol_cid = model.rpd_cid.Value;
                logmodel.ol_title = "删除" + typeText + "明细";
                logmodel.ol_content = "记录ID：" + id + "，金额：" + model.rpd_money + "";
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Delete.ToString(), logmodel, manager.user_name, manager.real_name); //记录日志
                return "";
            }
            return "删除失败";
        }
        /// <summary>
        /// 计算某个客户在订单中的未付金额
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        public decimal? getUnPayMoney(int cid, string oid)
        {
            if (cid == 0) return 0;
            if (string.IsNullOrEmpty(oid)) return 0;
            return dal.getUnPayMoney(cid, oid);
        }
        /// <summary>
        /// 付款明细审批
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <param name="remark"></param>
        /// <param name="username"></param>
        /// <param name="realname"></param>
        /// <returns></returns>
        public string checkPayDetailStatus(int id, byte? type, byte? status, string remark, Model.manager adminModel)
        {
            Model.ReceiptPayDetail model = GetModel(id);
            if (model == null)
                return "数据不存在";
            string content = "";
            //3.如果是财务审批，要先验证部门审批是否已经审批通过，不通过不能审批；如果是总经理审批，要先验证财务审批是否已经审批通过，不通过不能审批
            //4.反审批时：a.总经理要先验证是否已经确认汇总。b.财务要先验证总经理是否已经审批通过。c.部门要先验证财务是否已经审批通过
            switch (type)
            {
                case 1://部门审批
                    if (model.rpd_flag1 == status) return "状态未变更";
                    //判断有没有部门审批权限
                    if (model.rpd_area != adminModel.area || !new permission().checkHasPermission(adminModel, "0603"))
                    {
                        return "无权限审批";
                    }
                    if (status == 2)
                    {
                        //由待审批、审批未通过→审批通过
                    }
                    else
                    {
                        //由审批通过→待审批、审批未通过：验证财务审批是否通过，审批通过的不能再做部门反审批
                        if (model.rpd_flag2 == 2)
                        {
                            return "财务已经审批通过，不能做部门审批";
                        }
                    }
                    content = "记录id：" + id + "，部门审批状态：" + Common.BusinessDict.checkStatus()[model.rpd_flag1] + "→<font color='red'>" + Common.BusinessDict.checkStatus()[status] + "</font>";
                    model.rpd_flag1 = status;
                    model.rpd_checkNum1 = adminModel.user_name;
                    model.rpd_checkName1 = adminModel.real_name;
                    model.rpd_checkRemark1 = remark;
                    model.rpd_checkTime1 = DateTime.Now;
                    break;
                case 2://财务审批
                    if (model.rpd_flag2 == status) return "状态未改变";
                    if (new BLL.department().getGroupArea() != adminModel.area || !new permission().checkHasPermission(adminModel, "0402"))
                    {
                        return "无权限审批";
                    }
                    if (status == 2)
                    {
                        //由待审批、审批未通过→审批通过：验证部门审批是否存在待审批或审批未通过的记录，存在则不能做财务审批
                        if (model.rpd_flag1 != 2)
                        {
                            return "部门审批是待审批或审批未通过的，不能做财务审批";
                        }
                    }
                    else
                    {
                        //由审批通过→待审批、审批未通过：验证总经理审批是否通过，审批通过的不能再做财务反审批
                        if (model.rpd_flag3 == 2)
                        {
                            return "总经理已经审批通过，不能做财务审批";
                        }
                    }
                    content = "记录id：" + id + "，财务审批状态：" + Common.BusinessDict.checkStatus()[model.rpd_flag2] + "→<font color='red'>" + Common.BusinessDict.checkStatus()[status] + "</font>";
                    model.rpd_flag2 = status;
                    model.rpd_checkNum2 = adminModel.user_name;
                    model.rpd_checkName2 = adminModel.real_name;
                    model.rpd_checkRemark2 = remark;
                    model.rpd_checkTime2 = DateTime.Now;
                    break;
                case 3://总经理审批
                    if (model.rpd_flag3 == status) return "状态未改变";
                    if (new BLL.department().getGroupArea() != adminModel.area || !new permission().checkHasPermission(adminModel, "0601"))
                    {
                        return "无权限审批";
                    }
                    if (status == 2)
                    {
                        //由待审批、审批未通过→审批通过：验证财务审批是否存在待审批或审批未通过的记录，存在则不能做总经理审批
                        if (model.rpd_flag2 != 2)
                        {
                            return "财务审批是待审批或审批未通过的，不能做总经理审批";
                        }
                    }
                    else
                    {
                        //由审批通过→待审批、审批未通过：验证是否已经汇总，已经汇总的不能做总经理审批
                        if (model.rpd_rpid > 0)
                        {
                            return "已经汇总，不能做总经理审批";
                        }
                    }
                    content = "记录id：" + id + "，总经理审批状态：" + Common.BusinessDict.checkStatus()[model.rpd_flag3] + "→<font color='red'>" + Common.BusinessDict.checkStatus()[status] + "</font>";
                    model.rpd_flag3 = status;
                    model.rpd_checkNum3 = adminModel.user_name;
                    model.rpd_checkName3 = adminModel.real_name;
                    model.rpd_checkRemark3 = remark;
                    model.rpd_checkTime3 = DateTime.Now;
                    break;
            }
            if (dal.Update(model))
            {
                //写日志
                Model.business_log log = new Model.business_log();
                log.ol_title = "审批付款明细";
                log.ol_oid = model.rpd_oid;
                log.ol_cid = model.rpd_cid.Value;
                log.ol_relateID = id;
                string _content = content;
                log.ol_content = _content;
                new business_log().Add(DTEnums.ActionEnum.Audit.ToString(), log, adminModel.user_name, adminModel.real_name);

                //信息通知
                //审批未通过、审批通过时通知
                if (status == 1 || status == 2)
                {
                    string replaceUser1 = "";
                    string nodeName = string.Empty;
                    DataTable dt = null;
                    string replaceContent = new BLL.Customer().GetModel(model.rpd_cid.Value).c_name + "," + model.rpd_money + "," + model.rpd_content + "," + model.rpd_oid;
                    string replaceUser = adminModel.user_name + "," + adminModel.real_name;
                    if (status == 1)
                    {
                        nodeName = "非业务付款通知部门审批、财务审批、总经理未通过";                        
                    }
                    else
                    {
                        if (type == 3)//总经理审批完成
                        {
                            nodeName = "非业务付款通知总经理审批通过";
                        }
                        else
                        {
                            nodeName = "非业务付款通知部门审批、财务审批通过";
                            if (type == 1)
                            {
                                dt = new manager().getUserByPermission("0402").Tables[0];
                            }
                            else
                            {
                                dt = new manager().getUserByPermission("0601").Tables[0];
                            }
                            if (dt != null)
                            {
                                foreach (DataRow dr in dt.Rows)
                                {
                                    replaceUser1 += dr["user_name"] + "(" + dr["real_name"] + "),";
                                }
                            }
                            replaceUser1 = replaceUser1.TrimEnd(',');
                        }
                    }
                    //钉钉通知申请人
                    Model.manager_oauth oauthModel = new BLL.manager_oauth().GetModel(model.rpd_personNum);
                    if (oauthModel != null && oauthModel.is_lock == 1 && !string.IsNullOrEmpty(oauthModel.oauth_userid))
                    {
                        new BLL.selfMessage().sentDingMessage(nodeName, oauthModel.oauth_userid, replaceContent, replaceUser);
                    }
                    //通知下付款通知人
                    new BLL.selfMessage().AddMessage(nodeName, model.rpd_personNum, model.rpd_personName, replaceContent, replaceUser, replaceUser1);

                    //通知下一级审批人
                    if (!string.IsNullOrEmpty(replaceUser1))
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            new BLL.selfMessage().AddMessage(nodeName, dr["user_name"].ToString(), dr["real_name"].ToString(), replaceContent, replaceUser, replaceUser1);

                            //钉钉推送通知
                            if (!string.IsNullOrEmpty(Utils.ObjectToStr(dr["oauth_userid"])))
                            {
                                new BLL.selfMessage().sentDingMessage(nodeName, dr["oauth_userid"].ToString(), replaceContent, replaceUser, replaceUser1);
                            }
                        }
                    }
                }
                return "";
            }
            return "操作失败";
        }

        /// <summary>
        /// 付款明细汇总
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="method"></param>
        /// <param name="foredate"></param>
        /// <param name="username"></param>
        /// <param name="realname"></param>
        /// <returns></returns>
        public string collectPayDetails(int cid, string area, int method,int newmethod, int cbid, bool isConfirm, DateTime date,string sdate,string edate, Model.manager manager)
        {
            if (new BLL.department().getGroupArea() != manager.area || !new permission().checkHasPermission(manager, "0404"))
            {
                return "无权限汇总";
            }
            string dateStr = "";
            if (!string.IsNullOrEmpty(sdate))
            {
                dateStr += " and datediff(d,rpd_foreDate,'" + sdate + "')<=0";
            }
            if (!string.IsNullOrEmpty(edate))
            {
                dateStr += " and datediff(d,rpd_foreDate,'" + edate + "')>=0";
            }
            DataSet ds = GetList(0, "rpd_type=0 and rpd_flag3=2 and rpd_flag2=2 and rpd_flag1=2 and isnull(rpd_rpid,0)=0 and rpd_cid=" + cid + " and isnull(rpd_method,0)=" + method + " and isnull(rpd_cbid,0)=" + cbid + " " + dateStr + "", "");
            if (ds == null || ds.Tables[0].Rows.Count == 0)
            {
                return "数据异常";
            }
            if (method>0 && newmethod==0)
            {
                newmethod = method;
            }

            decimal? money = 0;
            string ids = "";
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                money += Convert.ToDecimal(dr["rpd_money"]);
                ids += dr["rpd_id"].ToString() + ",";
            }
            ids = ids.TrimEnd(',');
            Model.ReceiptPay model = new Model.ReceiptPay();
            model.rp_type = false;
            model.rp_isExpect = false;
            model.rp_cid = cid;
            model.rp_money = money;
            model.rp_method = newmethod;
            model.rp_foredate = ConvertHelper.toDate(DateTime.Now.ToString("yyyy-MM-dd"));
            model.rp_personNum = manager.user_name;
            model.rp_personName = manager.real_name;
            model.rp_adddate = DateTime.Now;
            model.rp_flag = 2;
            model.rp_flag1 = 2;
            model.rp_area = area;
            model.rp_isConfirm = isConfirm;
            model.rp_cbid = cbid;
            if (isConfirm)
            {
                model.rp_confirmerNum = manager.user_name;
                model.rp_confirmerName = manager.real_name;
                model.rp_date = date;
            }
            int rpid = dal.collectPayDetails(model, method, sdate, edate);
            if (rpid > 0)
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = rpid;
                logmodel.ol_cid = model.rp_cid.Value;
                logmodel.ol_title = "汇总付款明细";
                logmodel.ol_content = "付款明细ID：" + ids + "，汇总总金额：" + money;
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Collect.ToString(), logmodel, manager.user_name, manager.real_name);
                return "";
            }
            return "汇总失败";
        }

        /// <summary>
        /// 取消汇总
        /// </summary>
        /// <param name="rpdid"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public string cancelCollect(int rpdid, Model.manager manager)
        {
            if (new BLL.department().getGroupArea() != manager.area || !new permission().checkHasPermission(manager, "0404"))
            {
                return "无权限取消汇总";
            }
            DataTable dt = GetList(0, "rpd_id=" + rpdid + "", "").Tables[0];
            if (dt == null || dt.Rows.Count == 0)
                return "数据不存在";
            if (Utils.ObjToInt(dt.Rows[0]["rpd_rpid"],0)==0)
            {
                return "还未汇总的记录不能取消汇总";
            }
            if (Utils.StrToBool(Utils.ObjectToStr(dt.Rows[0]["rp_isExpect"]), false))
            {
                return "预付款不能取消汇总";
            }
            if (Utils.StrToBool(Utils.ObjectToStr(dt.Rows[0]["rp_isConfirm"]), false))
            {
                return "已经确认付款，不能再取消汇总";
            }
            if (dal.cancelCollect(dt.Rows[0]))
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = Utils.ObjToInt(dt.Rows[0]["rpd_rpid"], 0);
                logmodel.ol_cid = Utils.ObjToInt(dt.Rows[0]["rpd_cid"], 0);
                logmodel.ol_title = "取消汇总付款明细";
                logmodel.ol_content = "付款明细ID：" + dt.Rows[0]["rpd_rpid"] + "，取消汇总金额：" + dt.Rows[0]["rpd_money"];
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Collect.ToString(), logmodel, manager.user_name, manager.real_name);
                return "";
            }
            return "取消汇总失败";
        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.ReceiptPayDetail GetModel(int id)
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
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder,Model.manager manager, out int recordCount, out decimal pmoney, bool isPay = false,bool isPage=true)
        {
            //列表权限控制
            if (manager.area != new BLL.department().getGroupArea())//如果不是总部的工号
            {
                if (new BLL.permission().checkHasPermission(manager, "0602"))
                {
                    //含有区域权限可以查看本区域添加的
                    strWhere += " and rpd_area='" + manager.area + "'";
                }
                else
                {
                    //只能
                    strWhere += " and rpd_PersonNum='" + manager.user_name + "'";
                }
            }            
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount, out pmoney, isPay, isPage);
        }
        public DataSet GetPayCertificationList(string strWhere)
        {
            return dal.GetPayCertificationList(strWhere);
        }
        /// <summary>
        /// 获取付款明细汇总数据 
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="strWhere"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataSet getCollectList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount, out decimal tmoney)
        {
            return dal.getCollectList(pageSize, pageIndex, strWhere, filedOrder, out recordCount,out tmoney);
        }
        #endregion

        #region 扩展方法================================

        #endregion
    }
}
