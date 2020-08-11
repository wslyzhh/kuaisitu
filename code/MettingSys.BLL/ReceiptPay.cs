using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MettingSys.BLL
{
    public partial class ReceiptPay
    {
        private readonly DAL.ReceiptPay dal;
        public ReceiptPay()
        {
            dal = new DAL.ReceiptPay();
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
        /// 计算已审未支付退款的数量
        /// </summary>
        /// <returns></returns>
        public int getUnPaycount()
        {
            return dal.getUnPaycount();
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public string Add(Model.ReceiptPay model,Model.manager manager,string num,string date,out int rpid,bool flag=true)
        {
            rpid = 0;
            string typeText = "收款";
            if (!model.rp_type.Value) typeText = "付款";
            if (flag)
            {
                model.rp_isExpect = true;
            }
            else
            {
                model.rp_isExpect = false;
            }
            if (model.rp_isExpect.Value && !new BLL.permission().checkHasPermission(manager, "0406"))
            {
                return "无权限添加";
            }
            if (model.rp_cid == 0)
            {
                return "请选择"+ typeText + "对象";
            }
            if (model.rp_money == 0)
            {
                return "请填写" + typeText + "金额";
            }
            if (model.rp_foredate == null)
            {
                return "请选择" + (model.rp_type.Value?"预收":"预付") + "日期";
            }
            //预收款的收款方式是必填的，预付款的付款方式是非必填的
            if ( model.rp_type.Value && model.rp_method == 0)
            {
                return "请选择" + typeText + "方式";
            }
            else
            {
                if (model.rp_method != 0)
                {
                    Model.payMethod method = new BLL.payMethod().GetModel(model.rp_method.Value);
                    if (method.pm_type.Value)
                    {
                        model.rp_cbid = 0;
                        if (string.IsNullOrEmpty(num))
                        {
                            return "请填写凭证号";
                        }
                        if (string.IsNullOrEmpty(date))
                        {
                            return "请填写凭证日期";
                        }
                        Model.certificates ce = new BLL.certificates().GetModel(num, Convert.ToDateTime(date));
                        int ceid = 0;
                        if (ce == null)
                        {
                            Model.certificates cemodel = new Model.certificates();
                            cemodel.ce_num = num;
                            cemodel.ce_date = ConvertHelper.toDate(date);
                            cemodel.ce_personNum = manager.user_name;
                            cemodel.ce_personName = manager.real_name;
                            new BLL.certificates().Add(cemodel, out ceid);
                            if (ceid > 0)
                            {
                                cemodel.ce_id = ceid;
                            }
                        }
                        else
                        {
                            ceid = ce.ce_id.Value;
                        }
                        model.rp_ceid = ceid;
                    }
                    else
                    {
                        if (model.rp_cbid == 0)
                        {
                            return "请选择客户银行账号";
                        }
                    }
                }
                else
                {
                    if (model.rp_cbid == 0)
                    {
                        return "请选择客户银行账号";
                    }
                }
            }
            model.rp_isConfirm = false;

            bool isChongzhang = false;
            if (model.rp_type.Value)
            {
                model.rp_flag = 0;
                model.rp_flag1 = 0;
                if (model.rp_money >= 0)
                {
                    model.rp_flag = 2;
                    model.rp_checkTime = DateTime.Now;
                    model.rp_flag1 = 2;
                    model.rp_checkTime1 = DateTime.Now;
                    isChongzhang = model.rp_method.Value > 0 && new BLL.payMethod().GetModel(model.rp_method.Value).pm_type.Value;
                    if (isChongzhang)
                    {
                        model.rp_isConfirm = true;
                        model.rp_date = model.rp_foredate;
                        model.rp_confirmerName = manager.real_name;
                        model.rp_confirmerNum = manager.user_name;
                    }
                }
                else
                {
                    isChongzhang = model.rp_method.Value > 0 && new BLL.payMethod().GetModel(model.rp_method.Value).pm_type.Value;
                    if (isChongzhang)
                    {
                        model.rp_flag = 2;
                        model.rp_checkTime = DateTime.Now;
                        model.rp_flag1 = 2;
                        model.rp_checkTime1 = DateTime.Now;
                        model.rp_isConfirm = true;
                        model.rp_date = model.rp_foredate;
                        model.rp_confirmerName = manager.real_name;
                        model.rp_confirmerNum = manager.user_name;
                    }
                    else 
                    {
                        if (model.rp_cbid == 0)
                        {
                            return "请选择客户银行账号";
                        }
                    }
                }
            }
            else
            {
                model.rp_isExpect = true;
                model.rp_flag = 0;
                model.rp_flag1 = 0;
                isChongzhang = model.rp_method.Value > 0 && new BLL.payMethod().GetModel(model.rp_method.Value).pm_type.Value;
                if (isChongzhang || model.rp_money <0)
                {
                    model.rp_flag = 2;
                    model.rp_checkTime = DateTime.Now;
                    model.rp_flag1 = 2;
                    model.rp_checkTime1 = DateTime.Now;                    
                    model.rp_isConfirm = true;
                    model.rp_date = model.rp_foredate;
                    model.rp_confirmerName = manager.real_name;
                    model.rp_confirmerNum = manager.user_name;
                    
                }
            }
            
            model.rp_personNum = manager.user_name;
            model.rp_personName = manager.real_name;
            model.rp_adddate = DateTime.Now;
            model.rp_area = manager.area;
            rpid = dal.Add(model);
            if (rpid > 0)
            {
                StringBuilder content = new StringBuilder();
                content.Append("" + typeText + "对象ID：" + model.rp_cid + "<br/>");
                content.Append("预收款：是<br/>");
                content.Append("" + typeText + "金额：" + model.rp_money + "<br/>");
                content.Append("" + (model.rp_type.Value ? "预收" : "预付") + "日期：" + model.rp_foredate.Value.ToString("yyyy-MM-dd") + "<br/>");
                content.Append("收款方式ID：" + model.rp_method + "<br/>");
                content.Append("收款内容：" + model.rp_content + "<br/>");
                content.Append("客户银行账号：" + model.rp_cbid + "<br/>");

                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = rpid;
                logmodel.ol_cid = model.rp_cid.Value;
                logmodel.ol_title = "添加"+ typeText + "";
                logmodel.ol_content = content.ToString();
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Add.ToString(), logmodel, manager.user_name, manager.real_name);

                //钉钉通知
                if (!model.rp_type.Value && model.rp_money >=0 && (model.rp_method == 0 || !isChongzhang))
                {
                    DataTable userDt = new BLL.manager().getUserByPermission("0402").Tables[0];
                    if (userDt != null)
                    {
                        string replaceContent = model.rp_money + "," + model.rp_content;
                        string replaceUser = model.rp_personNum + "," + model.rp_personName;
                        foreach (DataRow dr in userDt.Rows)
                        {
                            //钉钉推送通知
                            if (!string.IsNullOrEmpty(Utils.ObjectToStr(dr["oauth_userid"])))
                            {
                                new BLL.selfMessage().sentDingMessage("添加预付款", dr["oauth_userid"].ToString(), replaceContent, replaceUser);
                            }
                        }
                    }
                }

                return "";
            }
            return "添加失败";
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public string Update(Model.ReceiptPay model,string content,Model.manager manager, string num, string date,bool updateMoney=false)
        {
            if (!new BLL.permission().checkHasPermission(manager, "0401"))//如果不是财务
            {
                return "无权限编辑";
            }
            else
            {
                if (model.rp_personNum != manager.user_name && !new BLL.permission().checkHasPermission(manager, "0403"))
                {
                    return "无权限编辑";
                }
            }
            string typeText = "收款";
            if (model.rp_type.Value)
            {
                if (model.rp_isConfirm.Value)
                {
                    return "已经确认收款，不能再编辑";
                }
            }
            else
            {
                typeText = "付款";
                if (updateMoney)
                {
                    if (model.rp_money < 0)
                    {
                        model.rp_flag = 2;
                        model.rp_checkTime = DateTime.Now;
                        model.rp_flag1 = 2;
                        model.rp_checkTime1 = DateTime.Now;
                        model.rp_isConfirm = true;
                        model.rp_date = model.rp_foredate;
                        model.rp_confirmerName = manager.real_name;
                        model.rp_confirmerNum = manager.user_name;
                    }
                    else
                    {
                        model.rp_flag = 0;
                        model.rp_checkNum = "";
                        model.rp_checkName = "";
                        model.rp_checkRemark = "";
                        model.rp_checkTime = null;
                        model.rp_flag1 = 0;
                        model.rp_checkNum1 = "";
                        model.rp_checkName1 = "";
                        model.rp_checkRemark1 = "";
                        model.rp_checkTime1 = null;
                        model.rp_isConfirm = false;
                    }
                }
                else
                {
                    if (model.rp_money >=0 && model.rp_flag == 2 && model.rp_flag1 != 1 && !new BLL.payMethod().GetModel(model.rp_method.Value).pm_type.Value)
                    {
                        return "财务已经审批通过，不能再编辑";
                    }
                    if (model.rp_flag == 1 || model.rp_flag1 == 1)
                    {
                        model.rp_flag = 0;
                        model.rp_checkNum = "";
                        model.rp_checkName = "";
                        model.rp_checkRemark = "";
                        model.rp_checkTime = null;
                        model.rp_flag1 = 0;
                        model.rp_checkNum1 = "";
                        model.rp_checkName1 = "";
                        model.rp_checkRemark1 = "";
                        model.rp_checkTime1 = null;
                    }
                }
            }
            if (model.rp_cid == 0)
            {
                return "请选择" + typeText + "对象";
            }
            if (model.rp_money == 0)
            {
                return "请填写" + typeText + "金额";
            }
            if (model.rp_foredate == null)
            {
                return "请选择" + (model.rp_type.Value ? "预收" : "预付") + "日期";
            }
            if ( model.rp_type.Value && model.rp_method == 0)
            {
                return "请选择" + typeText + "方式";
            }
            else
            {
                if (model.rp_method != 0)
                {
                    Model.payMethod method = new BLL.payMethod().GetModel(model.rp_method.Value);
                    if (method.pm_type.Value)
                    {
                        model.rp_cbid = 0;
                        if (string.IsNullOrEmpty(num))
                        {
                            return "请填写凭证号";
                        }
                        if (string.IsNullOrEmpty(date))
                        {
                            return "请填写凭证日期";
                        }
                        Model.certificates ce = new BLL.certificates().GetModel(num, Convert.ToDateTime(date));
                        int ceid = 0;
                        if (ce == null)
                        {
                            Model.certificates cemodel = new Model.certificates();
                            cemodel.ce_num = num;
                            cemodel.ce_date = ConvertHelper.toDate(date);
                            cemodel.ce_personNum = manager.user_name;
                            cemodel.ce_personName = manager.real_name;
                            new BLL.certificates().Add(cemodel, out ceid);
                            if (ceid > 0)
                            {
                                cemodel.ce_id = ceid;
                            }
                        }
                        else
                        {
                            ceid = ce.ce_id.Value;
                        }
                        model.rp_ceid = ceid;
                    }
                    else
                    {
                        model.rp_ceid = 0;
                        if (model.rp_cbid == 0)
                        {
                            return "请选择客户银行账号";
                        }
                    }
                }
                else
                {
                    if (model.rp_cbid == 0)
                    {
                        return "请选择客户银行账号";
                    }
                }
            }
            if (dal.Update(model))
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = model.rp_id.Value;
                logmodel.ol_cid = model.rp_cid.Value;
                logmodel.ol_title = "编辑" + typeText + "";
                logmodel.ol_content = content.ToString();
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Edit.ToString(), logmodel, manager.user_name, manager.real_name);

                //钉钉通知
                if (!model.rp_type.Value)
                {
                    DataTable userDt = new BLL.manager().getUserByPermission("0402").Tables[0];
                    if (userDt != null)
                    {
                        string replaceContent = model.rp_money + "," + model.rp_content;
                        string replaceUser = model.rp_personNum + "," + model.rp_personName;
                        foreach (DataRow dr in userDt.Rows)
                        {
                            //钉钉推送通知
                            if (!string.IsNullOrEmpty(Utils.ObjectToStr(dr["oauth_userid"])))
                            {
                                new BLL.selfMessage().sentDingMessage("添加预付款", dr["oauth_userid"].ToString(), replaceContent, replaceUser);
                            }
                        }
                    }
                }

                return "";
            }
            return "编辑失败";
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public string Delete(int id, Model.manager manager)
        {
            Model.ReceiptPay model = new BLL.ReceiptPay().GetModel(id);
            if (model == null)
            {
                return "数据不存在";
            }
            if (!new BLL.permission().checkHasPermission(manager, "0401"))//如果不是财务
            {
                return "无权限删除";
            }
            else
            {
                if (model.rp_personNum != manager.user_name && !new BLL.permission().checkHasPermission(manager, "0403"))
                {
                    return "无权限删除";
                }
            }
            string typeText = "收款";
            bool flag = false;
            if (model.rp_type.Value)
            {
                if (model.rp_isConfirm.Value)
                {
                    return "已经确认收款，不能删除";
                }
                flag = dal.deleteReceipt(id);
            }
            else
            {
                typeText = "付款";
                if ( model.rp_isExpect.Value && model.rp_flag1 == 2 && model.rp_money >=0 && (model.rp_method==0 || !new BLL.payMethod().GetModel(model.rp_method.Value).pm_type.Value))
                {
                    return "最终审批通过不能删除";
                }
                flag = dal.deletePay(model);
            }
            if (flag)
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = id;
                logmodel.ol_title = "删除收款";
                logmodel.ol_content = typeText+ "ID：" + id+"，金额："+model.rp_money;
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Delete.ToString(), logmodel, manager.user_name, manager.real_name);
                return "";
            }
            return "删除失败";
        }
        /// <summary>
        /// 审批付款
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="remark"></param>
        /// <param name="username"></param>
        /// <param name="realname"></param>
        /// <returns></returns>
        public string checkPay(int id, byte? type, byte? status, string remark,Model.manager manager)
        {           
            Model.ReceiptPay model = GetModel(id);
            if (model == null)
                return "数据不存在";
            if (!model.rp_isExpect.Value)
            {
                return "非预付款不需要做审批";
            }
            string content = "";
            //3.如果是总经理审批，要先验证财务审批是否已经审批通过，不通过不能审批
            //4.反审批时：a.总经理要先验证是否已经确认付款。b.财务要先验证总经理是否已经审批通过。
            switch (type)
            {
                case 1://财务审批
                    if (model.rp_flag == status) return "状态未改变";
                    if (new BLL.department().getGroupArea() != manager.area || !new permission().checkHasPermission(manager, "0402"))
                    {
                        return "无权限审批";
                    }
                    if (status == 2)
                    {
                        
                    }
                    else
                    {
                        //由审批通过→待审批、审批未通过：验证总经理审批是否通过，审批通过的不能再做财务反审批
                        if (model.rp_flag1 == 2)
                        {
                            return "总经理已经审批通过，不能做财务审批";
                        }
                    }
                    content = "记录id：" + id + "，财务审批状态：" + Common.BusinessDict.checkStatus()[model.rp_flag] + "→<font color='red'>" + Common.BusinessDict.checkStatus()[status] + "</font>";
                    model.rp_flag = status;
                    model.rp_checkNum = manager.user_name;
                    model.rp_checkName = manager.real_name;
                    model.rp_checkRemark = remark;
                    model.rp_checkTime = DateTime.Now;
                    break;
                case 2://总经理审批
                    if (model.rp_flag1 == status) return "状态未改变";
                    if (new BLL.department().getGroupArea() != manager.area || !new permission().checkHasPermission(manager, "0601"))
                    {
                        return "无权限审批";
                    }
                    if (status == 2)
                    {
                        //由待审批、审批未通过→审批通过：验证财务审批是否存在待审批或审批未通过的记录，存在则不能做总经理审批
                        if (model.rp_flag != 2)
                        {
                            return "财务审批是待审批或审批未通过的，不能做总经理审批";
                        }
                    }
                    else
                    {
                        //由审批通过→待审批、审批未通过：验证是否已经汇总，已经汇总的不能做总经理审批
                        if (model.rp_isConfirm.Value)
                        {
                            return "已经确认付款，不能做总经理审批";
                        }
                    }
                    content = "记录id：" + id + "，总经理审批状态：" + Common.BusinessDict.checkStatus()[model.rp_flag1] + "→<font color='red'>" + Common.BusinessDict.checkStatus()[status] + "</font>";
                    model.rp_flag1 = status;
                    model.rp_checkNum1 = manager.user_name;
                    model.rp_checkName1 = manager.real_name;
                    model.rp_checkRemark1 = remark;
                    model.rp_checkTime1 = DateTime.Now;
                    break;
            }
            if (dal.Update(model))
            {
                //写日志
                Model.business_log log = new Model.business_log();
                log.ol_title = "审批付款";
                log.ol_cid = model.rp_cid.Value;
                log.ol_relateID = id;
                string _content = content;
                log.ol_content = _content;
                new business_log().Add(DTEnums.ActionEnum.Audit.ToString(), log, manager.user_name, manager.real_name);

                if (status == 1 || status == 2)
                {
                    string nodeName = status == 2 ? "预付款审批通过" : "预付款审批未通过";
                    string replaceContent = new BLL.Customer().GetModel(model.rp_cid.Value).c_name + "," + model.rp_money + "," + model.rp_content;
                    string replaceUser = manager.user_name + "," + manager.real_name;
                    //钉钉通知申请人
                    Model.manager_oauth oauthModel = new BLL.manager_oauth().GetModel(model.rp_personNum);
                    if (oauthModel != null && oauthModel.is_lock == 1 && !string.IsNullOrEmpty(oauthModel.oauth_userid))
                    {
                        new BLL.selfMessage().sentDingMessage(nodeName, oauthModel.oauth_userid, replaceContent, replaceUser);
                    }
                }
                if (type == 1  && status == 2)
                {
                    //信息通知
                    DataTable dt = dt = new manager().getUserByPermission("0601").Tables[0];
                    string replaceContent = new BLL.Customer().GetModel(model.rp_cid.Value).c_name + "," + model.rp_money + "," + model.rp_content;
                    string replaceUser = manager.user_name + "," + manager.real_name;
                    if (dt != null)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            //钉钉推送通知
                            if (!string.IsNullOrEmpty(Utils.ObjectToStr(dr["oauth_userid"])))
                            {
                                new BLL.selfMessage().sentDingMessage("预付款财务审批通过", dr["oauth_userid"].ToString(), replaceContent, replaceUser);
                            }
                        }
                    }         
                }
                return "";
            }
            return "操作失败";
        }

        /// <summary>
        /// 确认收付款
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isConfirm"></param>
        /// <param name="date"></param>
        /// <param name="method"></param>
        /// <param name="username"></param>
        /// <param name="realname"></param>
        /// <returns></returns>
        public string confirmReceiptPay(int id, string isConfirm,string date, string method, Model.manager manager)
        {
            string text = "收款", text1 = "实收";
            bool _isConfirm = false, _isChangeMethod = false;
            Model.ReceiptPay model = GetModel(id);
            if (model == null)
                return "数据不存在";
            if (new BLL.department().getGroupArea() != manager.area || !new permission().checkHasPermission(manager, "0404"))
            {
                return "无权限操作";
            }
            string _content = "";
            if (!model.rp_type.Value)
            {
                text = "付款";
                text1 = "实付";
                if (string.IsNullOrEmpty(isConfirm) && string.IsNullOrEmpty(method))
                {
                    return "请选择" + text + "状态或" + text + "方式";
                }
                if (!string.IsNullOrEmpty(isConfirm))
                {
                    _isConfirm = Utils.StrToBool(isConfirm, false);
                    if (model.rp_isConfirm == _isConfirm)
                    {
                        return text + "状态未变更";
                    }
                    if (model.rp_isExpect.Value && model.rp_flag1 != 2)
                    {
                        return "预付款总经理审批状态未通过，不能确认" + text;
                    }
                    _content = text + "ID：" + id + "<br/>状态：" + Common.BusinessDict.financeConfirmStatus(0)[model.rp_isConfirm] + "→<font color='red'>" + Common.BusinessDict.financeConfirmStatus(0)[_isConfirm] + "</font><br/>";
                    model.rp_isConfirm = _isConfirm;
                    if (_isConfirm)
                    {
                        if (model.rp_method == 0 && (string.IsNullOrEmpty(method) || method =="0"))
                        {
                            return "请标记付款方式";
                        }
                        if (string.IsNullOrEmpty(date))
                        {
                            return "请选择" + text1 + "日期";
                        }
                        model.rp_date = Utils.StrToDateTime(date);
                        _content += "实付日期：<font color='red'>" + model.rp_date.Value.ToString("yyyy-MM-dd") + "</font><br/>";
                        if (!string.IsNullOrEmpty(method))
                        {
                            _isChangeMethod = true;
                            model.rp_method = Utils.StrToInt(method, 0);
                            _content += "付款方式：<font color='red'>" + new BLL.payMethod().GetModel(model.rp_method.Value).pm_name + "</font>";
                        }
                        model.rp_confirmerNum = manager.user_name;
                        model.rp_confirmerName = manager.real_name;
                        
                    }
                    else
                    {
                        model.rp_date = null;
                        model.rp_confirmerNum = "";
                        model.rp_confirmerName = "";
                    }
                }
                if (!string.IsNullOrEmpty(method))
                {
                    _isChangeMethod = true;
                    model.rp_method = Utils.StrToInt(method, 0);
                    _content += "ID：" + id + "<br/>付款方式：<font color='red'>" + new BLL.payMethod().GetModel(model.rp_method.Value).pm_name + "</font>";
                }
            }
            else
            {
                if (string.IsNullOrEmpty(isConfirm))
                {
                    return "请选择" + text + "状态";
                }
                _isConfirm = Utils.StrToBool(isConfirm, false);
                if (model.rp_isConfirm == _isConfirm)
                {
                    return text + "状态未变更";
                }
                _content = text + "ID：" + id + "<br/>状态：" + Common.BusinessDict.financeConfirmStatus(1)[model.rp_isConfirm] + "→<font color='red'>" + Common.BusinessDict.financeConfirmStatus(1)[_isConfirm] + "</font><br/>";
                model.rp_isConfirm = _isConfirm;
                if (_isConfirm)
                {
                    if (string.IsNullOrEmpty(date))
                    {
                        return "请选择" + text1 + "日期";
                    }
                    model.rp_date = Utils.StrToDateTime(date);
                    model.rp_confirmerNum = manager.user_name;
                    model.rp_confirmerName = manager.real_name;
                    _content += "实收日期：<font color='red'>" + model.rp_date.Value.ToString("yyyy-MM-dd") + "</font>";
                }
                else
                {
                    model.rp_date = null;
                    model.rp_confirmerNum = "";
                    model.rp_confirmerName = "";
                }
            }
            if (dal.Update(model,_isChangeMethod))
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = id;
                logmodel.ol_cid = model.rp_cid.Value;
                logmodel.ol_title = model.rp_type.Value?"确认收款": "支付操作";
                logmodel.ol_content = _content;
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Audit.ToString(), logmodel, manager.user_name, manager.real_name);

                //消息通知
                if (model.rp_type.Value)
                {
                    if (_isConfirm)//确认到账
                    {
                        //通知下收款通知人
                        string replaceContent = new BLL.Customer().GetModel(model.rp_cid.Value).c_name + "," + model.rp_money + "," + model.rp_foredate.Value.ToString("yyyy-MM-dd") + "," + new BLL.payMethod().GetModel(model.rp_method.Value).pm_name;
                        string replaceUser = manager.user_name + "," + manager.real_name;
                        new BLL.selfMessage().AddMessage("收款通知确认到账", model.rp_personNum, model.rp_personName, replaceContent, replaceUser);

                        //通知业务员
                        DataTable dt = dal.getOrderUser(model.rp_id.Value);
                        if (dt != null)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                new BLL.selfMessage().AddMessage("收款通知确认到账", dr["op_number"].ToString(), dr["op_name"].ToString(), replaceContent, replaceUser);
                            }
                        }
                    }
                    else//确认未到账
                    {
                        //通知下收款通知人
                        string replaceContent = new BLL.Customer().GetModel(model.rp_cid.Value).c_name + "," + model.rp_money + "," + model.rp_foredate.Value.ToString("yyyy-MM-dd") + "," + new BLL.payMethod().GetModel(model.rp_method.Value).pm_name;
                        string replaceUser = manager.user_name + "," + manager.real_name;
                        new BLL.selfMessage().AddMessage("收款通知确认未到账", model.rp_personNum, model.rp_personName, replaceContent, replaceUser);

                        //通知业务员
                        DataTable dt = dal.getOrderUser(model.rp_id.Value);
                        if (dt != null)
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                new BLL.selfMessage().AddMessage("收款通知确认未到账", dr["op_number"].ToString(), dr["op_name"].ToString(), replaceContent, replaceUser);
                            }
                        }
                    }
                }
                return "";
            }
            return "审批失败";
        }
        /// <summary>
        /// 标记凭证
        /// </summary>
        /// <param name="id"></param>
        /// <param name="num"></param>
        /// <param name="date"></param>
        /// <param name="username"></param>
        /// <param name="realname"></param>
        /// <returns></returns>
        public string signCertificate(int id, string num, string date, Model.manager manager)
        {
            if (new BLL.department().getGroupArea() != manager.area || (!new permission().checkHasPermission(manager, "0404") && !new permission().checkHasPermission(manager, "0406")))
            {
                return "无权限标记";
            }
            Model.certificates model = new BLL.certificates().GetModel(num, Convert.ToDateTime(date));
            int ceid = 0;
            if (model == null)
            {
                Model.certificates cemodel = new Model.certificates();
                cemodel.ce_num = num;
                cemodel.ce_date = ConvertHelper.toDate(date);
                cemodel.ce_personNum = manager.user_name;
                cemodel.ce_personName = manager.real_name;
                new BLL.certificates().Add(cemodel, out ceid);
                if (ceid>0)
                {
                    cemodel.ce_id = ceid;
                    model = cemodel;
                }
            }
            else {
                ceid = model.ce_id.Value;
            }
            if (model.ce_flag == 2)
            {
                return "凭证号和凭证日期已经审批通过，不能使用";
            }
            Model.ReceiptPay rp = GetModel(id);
            if (rp == null)
            {
                return "记录不存在";
            }
            string text = "收款";
            if (!rp.rp_type.Value)
            {
                text = "付款";
            }
            if (!rp.rp_isConfirm.Value)
            {
                return "必须确认" + text + "后才能标记凭证";
            }
            rp.rp_ceid = ceid;
            if (dal.Update(rp))
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = id;
                logmodel.ol_cid = rp.rp_cid.Value;
                logmodel.ol_title = "标记凭证";
                logmodel.ol_content = "凭证号：" + num + "<br/>凭证日期：" + date;
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Sign.ToString(), logmodel, manager.user_name, manager.real_name);
                return "";
            }
            return "标记失败";
        }
        /// <summary>
        /// 取消标记凭证
        /// </summary>
        /// <param name="id"></param>
        /// <param name="username"></param>
        /// <param name="realname"></param>
        /// <returns></returns>
        public string cancelSignCertificate(int id, Model.manager manager)
        {
            if (new BLL.department().getGroupArea() != manager.area || (!new permission().checkHasPermission(manager, "0404") && !new permission().checkHasPermission(manager, "0406")))
            {
                return "无权限取消标记";
            }
            Model.ReceiptPay rp = GetModel(id);
            if (rp == null)
            {
                return "记录不存在";
            }
            int ceid = 0;
            int.TryParse(rp.rp_ceid.ToString(), out ceid);
            if (ceid == 0)
            {
                return "还未标记凭证的不能取消";
            }
            Model.certificates model = new BLL.certificates().GetModel(ceid);
            if (model == null)
                return "凭证不存在";
            if (model.ce_flag == 2)
            {
                return "凭证已经审批通过，不能取消";
            }
            rp.rp_ceid = 0;
            if (dal.Update(rp))
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = id;
                logmodel.ol_cid = rp.rp_cid.Value;
                logmodel.ol_title = "取消标记凭证";
                logmodel.ol_content = "凭证号：" + model.ce_num + "<br/>凭证日期：" + model.ce_date.Value.ToString("yyyy-MM-dd");
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Sign.ToString(), logmodel, manager.user_name, manager.real_name);
                return "";
            }
            return "标记失败";
        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.ReceiptPay GetModel(int id)
        {
            return dal.GetModel(id);
        }
        /// <summary>
        /// 获取已分配总额
        /// </summary>
        /// <param name="rpid"></param>
        /// <returns></returns>
        public decimal getDistributeMoney(int rpid)
        {
            return dal.getDistributeMoney(rpid);
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
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount, out decimal pmoney, out decimal punmoney, bool isPage = true)
        {
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount, out pmoney, out punmoney, isPage);
        }
        /// <summary>
        /// 分配列表
        /// </summary>
        public DataSet GetDistributionList(Dictionary<string, string> dict, int pageSize, int pageIndex, out int recordCount)
        {
            return dal.GetDistributionList(dict, pageSize, pageIndex, out recordCount);
        }
        #endregion

        #region 扩展方法================================

        #endregion
    }
}
