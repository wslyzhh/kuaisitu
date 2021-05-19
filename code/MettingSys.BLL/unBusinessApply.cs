using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace MettingSys.BLL
{
    public partial class unBusinessApply
    {
        private readonly DAL.unBusinessApply dal;

        public unBusinessApply()
        {
            dal = new DAL.unBusinessApply();
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
        public string Add(Model.unBusinessApply model, Model.manager manager,out int id)
        {
            id = 0;
            //如果添加的是员工往来，要判断添加人是否是订单的相关人员
            if (model.uba_type == 0 && model.uba_function == "业务活动执行备用金借款")
            {
                //验证权限：财务，订单的业务员、报账人员、执行人员，才能添加应收应付
                Model.Order order = new BLL.Order().GetModel(model.uba_oid);
                if (order == null)
                {
                    return "订单不存在";
                }
                if (order.o_lockStatus == 1)
                {
                    return "订单已锁单，不能再添加非业务支付申请";
                }
                if (!new BLL.permission().checkHasPermission(manager, "0401"))//如果不是财务
                {
                    if (order.personlist.Where(p => p.op_number == manager.user_name && p.op_type != 3).ToArray().Length == 0)
                    {
                        return "无权限添加";
                    }
                }
            }
            if (string.IsNullOrWhiteSpace(model.uba_function))
            {
                return "请选择或填写支付用途";
            }
            if (string.IsNullOrWhiteSpace(model.uba_receiveBank))
            {
                return "请填写收款银行";
            }
            if (string.IsNullOrWhiteSpace(model.uba_receiveBankName))
            {
                return "请填写账户名称";
            }
            if (string.IsNullOrWhiteSpace(model.uba_receiveBankNum))
            {
                return "请填写收款账号";
            }
            if (model.uba_foreDate == null)
            {
                return "请选择预付日期";
            }
            model.uba_addDate = DateTime.Now;
            model.uba_flag1 = 0;
            model.uba_flag2 = 0;
            model.uba_flag3 = 0;
            model.uba_isConfirm = false;
            id = dal.Add(model);
            if (id > 0)
            {
                Model.business_log log = new Model.business_log();
                log.ol_title = "添加非业务支付申请";
                if (model.uba_type == 0 && model.uba_function == "业务活动执行备用金借款")
                {
                    log.ol_oid = model.uba_oid;
                }
                log.ol_relateID = id;
                log.ol_content = "支付类别：" + BusinessDict.unBusinessNature()[model.uba_type.Value] + "</br>"
                                + "支付用途：" + model.uba_function + "</br>"
                                + "用途说明：" + model.uba_instruction + "</br>"
                                + "收款账户名称：" + model.uba_receiveBankName + "</br>"
                                + "收款账号：" + model.uba_receiveBankNum + "</br>"
                                + "收款银行：" + model.uba_receiveBank + "</br>"
                                + "金额：" + model.uba_money + "</br>"
                                + "预付日期：" + model.uba_foreDate.Value.ToString("yyyy-MM-dd") + "</br>"
                                + "备注：" + model.uba_remark + "</br>";
                new business_log().Add(DTEnums.ActionEnum.Add.ToString(), log, manager.user_name, manager.real_name);

                //钉钉通知
                DataTable userDt = new BLL.manager().getUserByPermission("0603", model.uba_area).Tables[0];
                if (userDt != null)
                {
                    string replaceContent = model.uba_money + "," + model.uba_function;
                    string replaceUser = model.uba_PersonNum + "," + model.uba_personName;
                    foreach (DataRow dr in userDt.Rows)
                    {
                        //钉钉推送通知
                        if (!string.IsNullOrEmpty(Utils.ObjectToStr(dr["oauth_userid"])))
                        {
                            new BLL.selfMessage().sentDingMessage("添加付款通知", dr["oauth_userid"].ToString(), replaceContent, replaceUser);
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
        public string Update(Model.unBusinessApply model, string content, Model.manager manager)
        {
            if (model.uba_type == 0 && model.uba_function == "业务活动执行备用金借款")
            {
                Model.Order order = new BLL.Order().GetModel(model.uba_oid);
                if (order == null)
                {
                    return "订单不存在";
                }
                if (!new BLL.permission().checkHasPermission(manager, "0401"))//如果不是财务
                {
                    //验证权限：在同一个订单里，业务员与业务报账员可以对未审核地接进行编辑与删除！执行人员只能对自己地址进行编辑与删除操作！                    
                    if (model.uba_PersonNum != manager.user_name && order.personlist.Where(p => p.op_number == manager.user_name && (p.op_type == 3 || p.op_type == 4)).ToArray().Length > 0)
                    {
                        return "无权限编辑";
                    }
                }
                else
                {
                    if (model.uba_PersonNum != manager.user_name && !new BLL.permission().checkHasPermission(manager, "0403"))
                    {
                        return "无权限编辑";
                    }
                }
            }
            else
            {
                if (manager.user_name != model.uba_PersonNum.ToUpper() && !new BLL.permission().checkHasPermission(manager, "0403"))
                {
                    return "无权限修改！";
                }
            }
            if (model.uba_flag1 == 2 && model.uba_flag2 != 1 && model.uba_flag3 != 1)
            {
                return "已审批通过的不能编辑";
            }
            if (string.IsNullOrWhiteSpace(model.uba_function))
            {
                return "请选择或填写支付用途";
            }
            if (string.IsNullOrWhiteSpace(model.uba_receiveBank))
            {
                return "请填写收款银行";
            }
            if (string.IsNullOrWhiteSpace(model.uba_receiveBankName))
            {
                return "请填写账户名称";
            }
            if (string.IsNullOrWhiteSpace(model.uba_receiveBankNum))
            {
                return "请填写收款账号";
            }
            if (model.uba_foreDate == null)
            {
                return "请选择预付日期";
            }
            //编辑后清空审批内容
            if (model.uba_flag1 == 1 || model.uba_flag2 == 1 || model.uba_flag2 == 1)
            {
                model.uba_flag1 = 0;
                model.uba_checkNum1 = "";
                model.uba_checkName1 = "";
                model.uba_checkRemark1 = "";
                model.uba_checkTime1 = null;
                model.uba_flag2 = 0;
                model.uba_checkNum2 = "";
                model.uba_checkName2 = "";
                model.uba_checkRemark2 = "";
                model.uba_checkTime2 = null;
                model.uba_flag3 = 0;
                model.uba_checkNum3 = "";
                model.uba_checkName3 = "";
                model.uba_checkRemark3 = "";
                model.uba_checkTime3 = null;
            }
            if (dal.Update(model))
            {
                Model.business_log log = new Model.business_log();
                log.ol_title = "修改非业务支付申请";
                if (model.uba_type == 0 && model.uba_function == "业务活动执行备用金借款")
                {
                    log.ol_oid = model.uba_oid;
                }
                log.ol_relateID = model.uba_id.Value;
                log.ol_content = content;
                new business_log().Add(DTEnums.ActionEnum.Edit.ToString(), log, manager.user_name, manager.real_name);

                if (!string.IsNullOrEmpty(content))
                {
                    //钉钉通知
                    DataTable userDt = new BLL.manager().getUserByPermission("0603", model.uba_area).Tables[0];
                    if (userDt != null)
                    {
                        string replaceContent = model.uba_money + "," + model.uba_function;
                        string replaceUser = model.uba_PersonNum + "," + model.uba_personName;
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
            return "更新失败";
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public string Delete(int id, Model.manager manager)
        {
            Model.unBusinessApply model = GetModel(id);
            if (model == null)
                return "数据不存在";
            //非本人不能删除
            if (manager.user_name != model.uba_PersonNum.ToUpper() && !new permission().checkHasPermission(manager,"0403"))
            {
                return "非申请人或没有删除他人数据权限不能删除";
            }
            if (model.uba_flag3 == 2)
            {
                return "最终审批通过不能再编辑";
            }
            if (dal.Delete(id))
            {
                //删除图片附件
                new BLL.payPic().deleteFileByid(id, 2);

                Model.business_log log = new Model.business_log();
                log.ol_title = "删除非业务支付申请";
                if (model.uba_type == 0)
                {
                    log.ol_oid = model.uba_oid;
                }
                log.ol_relateID = model.uba_id.Value;
                log.ol_content = "支付类别：" + BusinessDict.unBusinessNature()[model.uba_type.Value] + "</br>"
                                + "支付用途：" + model.uba_function + "</br>"
                                + "用途说明：" + model.uba_instruction + "</br>"
                                + "收款账户名称：" + model.uba_receiveBankName + "</br>"
                                + "收款账号：" + model.uba_receiveBankNum + "</br>"
                                + "收款银行：" + model.uba_receiveBank + "</br>"
                                + "金额：" + model.uba_money + "</br>"
                                + "预付日期：" + model.uba_foreDate.Value.ToString("yyyy-MM-dd") + "</br>"
                                + "备注：" + model.uba_remark + "</br>";
                new business_log().Add(DTEnums.ActionEnum.Delete.ToString(), log, manager.user_name, manager.real_name);
                return "";
            }
            return "删除失败";
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.unBusinessApply GetModel(int id)
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
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder,Model.manager manager, out int recordCount, out decimal tmoney, bool isPage = true)
        {            
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount,out tmoney,isPage);
        }
        #endregion

        #region 扩展方法================================


        /// <summary>
        /// 审批非业务支付申请状态
        /// </summary>
        /// <param name="id">记录ID</param>
        /// <param name="type">1部门审批，2财务审批，3总经理审批</param>
        /// <param name="status">0待审批，1审批未通过，2审批通过</param>
        /// <param name="remark">备注</param>
        /// <param name="username">操作人工号</param>
        /// <param name="realname">操作人姓名</param>
        /// <returns></returns>
        public string checkStatus(int id, byte? type, byte? status,string checkMoney, string remark, Model.manager adminModel)
        {
            Model.unBusinessApply model = GetModel(id);
            if (model == null) return "记录不存在";
            //如果是员工往来，要验证对应的订单是否已经锁单，锁单的不能再审批
            if (model.uba_type == 0 && !string.IsNullOrEmpty(model.uba_oid))
            {
                Model.Order order = new BLL.Order().GetModel(model.uba_oid);
                if (order != null && order.o_lockStatus == 1)
                {
                    return "订单已锁单，不能再审批";
                }
            }
            //批复金额
            decimal _checkmoney = 0, _oldmoney = model.uba_money.Value;
            string addremark = string.Empty, ubmlRemark = string.Empty;
            Model.unBusinessMoneyLog mlog = null;
            int ubmlID = 0;
            if (model.uba_function == "业务活动执行备用金借款" && !string.IsNullOrEmpty(checkMoney) && status == 2)
            {
                if (!decimal.TryParse(checkMoney, out _checkmoney))
                {
                    return "请正确填写批复金额";
                }
                else 
                {
                    if (_checkmoney <= 0 || _checkmoney >= model.uba_money)
                    {
                        return "批复金额需大于0且小于申请金额(" + model.uba_money + ")";
                    }
                    else
                    {
                        mlog = new Model.unBusinessMoneyLog();
                        mlog.ubml_ubaid = model.uba_id;
                        mlog.ubml_newMoney = _checkmoney;
                        mlog.ubml_oldMoney = model.uba_money;
                        mlog.ubml_date = DateTime.Now;
                        mlog.ubml_username = adminModel.user_name;
                        mlog.ubml_realname = adminModel.real_name;
                        addremark = "批复借款金额由原" + model.uba_money + "元改为" + _checkmoney + "元；";
                        model.uba_money = _checkmoney;
                    }
                }
            }
            string content = "";
            //3.如果是财务审批，要先验证部门审批是否已经审批通过，不通过不能审批；如果是总经理审批，要先验证财务审批是否已经审批通过，不通过不能审批
            //4.反审批时：a.总经理要先验证是否已经确认支付。b.财务要先验证总经理是否已经审批通过。c.部门要先验证财务是否已经审批通过
            switch (type)
            {
                case 1://部门审批
                    if (model.uba_flag1 == status) return "状态未改变";
                    //判断有没有部门审批权限
                    if (model.uba_area != adminModel.area || !new permission().checkHasPermission(adminModel, "0603"))
                    {
                        return "无权限审批";
                    }

                    content = "记录id：" + id + "，部门审批状态：" + Common.BusinessDict.checkStatus()[model.uba_flag1] + "→<font color='red'>" + Common.BusinessDict.checkStatus()[status] + "</font>";
                    model.uba_flag1 = status;
                    model.uba_checkNum1 = adminModel.user_name;
                    model.uba_checkName1 = adminModel.real_name;
                    model.uba_checkRemark1 = remark;
                    model.uba_checkTime1 = DateTime.Now;
                    if (status == 2)
                    {
                        //由待审批、审批未通过→审批通过

                        //存在批复金额
                        if (model.uba_function == "业务活动执行备用金借款" && mlog != null)
                        {
                            mlog.ubml_type = 1;
                            mlog.ubml_remark = "部门审批" + addremark;
                            model.uba_checkRemark1 += "部门审批" + addremark;
                        }
                    }
                    else
                    {                        
                        //由审批通过→待审批、审批未通过：验证财务审批是否通过，审批通过的不能再做部门反审批
                        if (model.uba_flag2 == 2)
                        {
                            return "财务已经审批通过，不能做部门审批";
                        }
                        if (model.uba_function == "业务活动执行备用金借款")
                        {
                            //检查部门审批是否存在批复记录，存在则退回原来的金额
                            ubmlID = getOldMoney(model, 1, out ubmlRemark);
                            if (!string.IsNullOrEmpty(ubmlRemark))
                            {
                                model.uba_checkRemark1 = model.uba_checkRemark1.Replace(ubmlRemark, "");
                            }
                        }
                    }
                    
                    break;
                case 2://财务审批
                    if (model.uba_flag2 == status) return "状态未改变";
                    if (new BLL.department().getGroupArea() != adminModel.area || !new permission().checkHasPermission(adminModel, "0402"))
                    {
                        return "无权限审批";
                    }
                    content = "记录id：" + id + "，财务审批状态：" + Common.BusinessDict.checkStatus()[model.uba_flag2] + "→<font color='red'>" + Common.BusinessDict.checkStatus()[status] + "</font>";
                    model.uba_flag2 = status;
                    model.uba_checkNum2 = adminModel.user_name;
                    model.uba_checkName2 = adminModel.real_name;
                    model.uba_checkRemark2 = remark;
                    model.uba_checkTime2 = DateTime.Now;
                    if (status == 2)
                    {
                        //由待审批、审批未通过→审批通过：验证部门审批是否存在待审批或审批未通过的记录，存在则不能做财务审批
                        if (model.uba_flag1 != 2)
                        {
                            return "部门审批是待审批或审批未通过的，不能做财务审批";
                        }
                        //存在批复金额
                        if (model.uba_function == "业务活动执行备用金借款" && mlog != null)
                        {
                            mlog.ubml_type = 2;
                            mlog.ubml_remark = "财务审批" + addremark;
                            model.uba_checkRemark2 += "财务审批" + addremark;
                        }
                    }
                    else
                    {
                        //由审批通过→待审批、审批未通过：验证总经理审批是否通过，审批通过的不能再做财务反审批
                        if (model.uba_flag3 == 2)
                        {
                            return "总经理已经审批通过，不能做财务审批";
                        }
                        if (model.uba_function == "业务活动执行备用金借款")
                        {
                            //检查部门审批是否存在批复记录，存在则退回原来的金额
                            ubmlID = getOldMoney(model, 2, out ubmlRemark);
                            if (!string.IsNullOrEmpty(ubmlRemark))
                            {
                                model.uba_checkRemark2 = model.uba_checkRemark2.Replace(ubmlRemark, "");
                            }
                        }
                    }
                    break;
                case 3://总经理审批
                    if (model.uba_flag3 == status) return "状态未改变";
                    if (new BLL.department().getGroupArea() != adminModel.area || !new permission().checkHasPermission(adminModel, "0601"))
                    {
                        return "无权限审批";
                    }
                    content = "记录id：" + id + "，总经理审批状态：" + Common.BusinessDict.checkStatus()[model.uba_flag3] + "→<font color='red'>" + Common.BusinessDict.checkStatus()[status] + "</font>";
                    model.uba_flag3 = status;
                    model.uba_checkNum3 = adminModel.user_name;
                    model.uba_checkName3 = adminModel.real_name;
                    model.uba_checkRemark3 = remark;
                    model.uba_checkTime3 = DateTime.Now;
                    if (status == 2)
                    {
                        //由待审批、审批未通过→审批通过：验证财务审批是否存在待审批或审批未通过的记录，存在则不能做总经理审批
                        if (model.uba_flag2 != 2)
                        {
                            return "财务审批是待审批或审批未通过的，不能做总经理审批";
                        }
                        //存在批复金额
                        if (model.uba_function == "业务活动执行备用金借款" && mlog != null)
                        {
                            mlog.ubml_type = 3;
                            mlog.ubml_remark = "总经理审批" + addremark;
                            model.uba_checkRemark3 += "总经理审批" + addremark;
                        }
                    }
                    else
                    {
                        //由审批通过→待审批、审批未通过：验证是否已经确认支付，已经确认支付的不能做总经理审批
                        if (model.uba_isConfirm.Value)
                        {
                            return "已经确认支付，不能做总经理审批";
                        }
                        if (model.uba_function == "业务活动执行备用金借款")
                        {
                            //检查部门审批是否存在批复记录，存在则退回原来的金额
                            ubmlID = getOldMoney(model, 3, out ubmlRemark);
                            if (!string.IsNullOrEmpty(ubmlRemark))
                            {
                                model.uba_checkRemark3 = model.uba_checkRemark3.Replace(ubmlRemark, "");
                            }
                        }
                    }
                    break;
            }           
            if (dal.Update(model))
            {
                //写日志
                Model.business_log log = new Model.business_log();
                log.ol_title = "审批非业务支付申请状态";
                log.ol_content = content;
                new business_log().Add(DTEnums.ActionEnum.Audit.ToString(), log, adminModel.user_name, adminModel.real_name);

                //插入批复金额记录
                if (mlog != null)
                {
                    new unBusinessMoneyLog().Add(mlog);
                }

                //删除已经批复的记录
                if (ubmlID > 0)
                {
                    new unBusinessMoneyLog().Delete(ubmlID);
                }

                #region 审批未通过、审批通过时通知
                if (status == 1 || status == 2)
                {
                    string replaceUser1 = "";
                    string nodeName = string.Empty;
                    DataTable dt = null;
                    string replaceContent = model.uba_money + "," + model.uba_function;
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
                    Model.manager_oauth oauthModel = new BLL.manager_oauth().GetModel(model.uba_PersonNum);
                    if (oauthModel != null && oauthModel.is_lock == 1 && !string.IsNullOrEmpty(oauthModel.oauth_userid))
                    {
                        new BLL.selfMessage().sentDingMessage(nodeName, oauthModel.oauth_userid, replaceContent, replaceUser);
                    }


                    //通知下付款通知人
                    new BLL.selfMessage().AddMessage(nodeName, model.uba_PersonNum, model.uba_personName, replaceContent, replaceUser, replaceUser1);

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
                #endregion
                return "";
            }

            return "操作失败";
        }

        /// <summary>
        /// 获取旧的申请金额
        /// </summary>
        /// <returns></returns>
        public int getOldMoney(Model.unBusinessApply model,byte ? type,out string remark)
        {
            int ubmlID = 0;
            remark = string.Empty;
            DataSet ds = new unBusinessMoneyLog().GetList(1, " ubml_ubaid = " + model.uba_id + " and ubml_type=" + type + "", " ubml_date desc");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                model.uba_money = Utils.ObjToDecimal(dr["ubml_oldMoney"], 0);
                ubmlID = Utils.ObjToInt(dr["ubml_id"], 0);
                remark = Utils.ObjectToStr(dr["ubml_remark"]);
            }
            return ubmlID;
        }

        /// <summary>
        /// 审批非业务支付申请支付状态
        /// </summary>
        /// <param name="id">记录ID</param>
        /// <param name="status">true已支付，false待支付</param>
        /// <param name="date">实付日期</param>
        /// <param name="paymethod">付款方式</param>
        /// <param name="username">操作人工号</param>
        /// <param name="realname">操作人姓名</param>
        /// <returns></returns>
        public string confirmStatus(int id, string status, string date, int paymethod, string paymethodName, Model.manager adminModel)
        {
            if (string.IsNullOrEmpty(status) && paymethod == 0)
            {
                return "请选择支付状态或支付方式";
            }
            Model.unBusinessApply model = GetModel(id);
            if (model == null) return "记录不存在";

            //如果是员工往来，要验证对应的订单是否已经锁单，锁单的不能再审批
            if (model.uba_type == 1 && !string.IsNullOrEmpty(model.uba_oid))
            {
                Model.Order order = new BLL.Order().GetModel(model.uba_oid);
                if (order != null && order.o_lockStatus == 1)
                {
                    return "订单已锁单，不能再审批";
                }
            }
            //2.判断当前登录工号是否有权限操作
            if (new BLL.department().getGroupArea() != adminModel.area || !new permission().checkHasPermission(adminModel, "0404"))
            {
                return "无权限操作";
            }
            bool isConfirm = false;
            string _content = string.Empty;
            if (!string.IsNullOrEmpty(status))
            {
                isConfirm = Utils.StrToBool(status, false);
                if (model.uba_isConfirm == isConfirm)
                {
                    return "支付状态未变更";
                }
                //3.要确认支付，先要验证总经理审批是否已经通过，未通过的不能确认支付
                _content += "记录id：" + id + "，支付状态：" + Common.BusinessDict.financeConfirmStatus()[model.uba_isConfirm] + "→<font color='red'>" + Common.BusinessDict.financeConfirmStatus()[isConfirm] + "</font><br/>";
                if (isConfirm)
                {
                    if (model.uba_flag3 != 2)
                    {
                        return "总经理存在待审批或审批未通过的记录，不能确认支付";
                    }
                    if (model.uba_payMethod == 0 && paymethod == 0)
                    {
                        return "请先标记支付方式";
                    }
                    if (string.IsNullOrEmpty(date))
                    {
                        return "请选择实付日期";
                    }
                    model.uba_date = Utils.StrToDateTime(date);
                    _content += "实付日期：" + model.uba_date.Value.ToString("yyyy-MM-dd") + "<br/>";
                    if (paymethod > 0)
                    {
                        model.uba_payMethod = paymethod;
                        _content += "付款方式：" + paymethodName + "<br/>";
                    }
                    model.uba_confirmerNum = adminModel.user_name;
                    model.uba_confirmerName = adminModel.real_name;

                }
                else
                {
                    model.uba_date = null;
                    model.uba_confirmerNum = "";
                    model.uba_confirmerName = "";
                }
                model.uba_isConfirm = isConfirm;
            }
            if (paymethod > 0)
            {
                model.uba_payMethod = paymethod;
                _content += "记录id：" + id + "，付款方式：" + paymethodName + "<br/>";
            }

            
            
            if (dal.Update(model))
            {
                //写日志
                Model.business_log log = new Model.business_log();
                log.ol_title = "审批非业务支付申请支付状态";
                log.ol_content = _content;
                new business_log().Add(DTEnums.ActionEnum.Confirm.ToString(), log, adminModel.user_name, adminModel.real_name);
                //确认支付要发送消息
                if (isConfirm)
                {
                    string replaceContent = string.Empty, replaceUser = string.Empty, replaceUser1 = string.Empty;
                    replaceContent = model.uba_money.ToString() + "," + model.uba_function;
                    replaceUser = adminModel.user_name + "," + adminModel.real_name;
                    replaceUser1 = "";
                    new BLL.selfMessage().AddMessage("非业务付款通知确认支付", model.uba_PersonNum, model.uba_personName, replaceContent, replaceUser, replaceUser1);
                }
                return "";
            }
            return "操作失败";
        }
        #endregion
    }
}
