using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MettingSys.BLL
{
    public partial class invoices
    {
        private readonly DAL.invoices dal;
        public invoices()
        {
            dal = new DAL.invoices();
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
        public string Add(Model.invoices model, Model.manager manager)
        {
            #region 验证数据
            if (model.inv_cid == 0)
            {
                return "请选择客户";
            }
            if (string.IsNullOrEmpty(model.inv_purchaserName))
            {
                return "请填写购买方名称";
            }
            if (string.IsNullOrEmpty(model.inv_purchaserNum))
            {
                return "请填写购买方纳税人识别号";
            }
            if (string.IsNullOrEmpty(model.inv_purchaserAddress))
            {
                return "请填写购买方地址";
            }
            if (string.IsNullOrEmpty(model.inv_purchaserPhone))
            {
                return "请填写购买方电话";
            }
            if (string.IsNullOrEmpty(model.inv_purchaserBank))
            {
                return "请填写购买方开户行";
            }
            if (string.IsNullOrEmpty(model.inv_purchaserBankNum))
            {
                return "请填写购买方账号";
            }
            if (string.IsNullOrEmpty(model.inv_serviceType))
            {
                return "请选择应税劳务";
            }
            if (string.IsNullOrEmpty(model.inv_serviceName))
            {
                return "请选择服务名称";
            }
            //if (model.inv_money <= 0)
            //{
            //    return "开票金额必须大于0";
            //}
            if (string.IsNullOrEmpty(model.inv_sentWay))
            {
                return "请选择送票方式";
            }
            if (model.inv_sentWay == "邮寄")
            {
                if (string.IsNullOrEmpty(model.inv_receiveName))
                {
                    return "请填写收票人名称";
                }
                if (string.IsNullOrEmpty(model.inv_receivePhone))
                {
                    return "请填写收票人电话";
                }
                if (string.IsNullOrEmpty(model.inv_receiveAddress))
                {
                    return "请填写收票人地址";
                }
            }
            if (string.IsNullOrEmpty(model.inv_darea))
            {
                return "请选择开票区域";
            }
            if (model.inv_unit == 0)
            {
                return "请选择开票单位";
            }
            #endregion
            //验证权限：财务，订单的业务员、报账人员、执行人员，才能添加应收应付
            Model.Order order = new BLL.Order().GetModel(model.inv_oid);
            if (order == null)
            {
                return "订单不存在";
            }
            if (!new BLL.permission().checkHasPermission(manager, "0401"))//如果不是财务
            {
                if (order.personlist.Where(p => p.op_number == manager.user_name && p.op_type != 3).ToArray().Length == 0)
                {
                    return "无权限添加";
                }
            }
            //地接的区域和订单区域保持一致
            model.inv_farea = order.personlist.Where(p => p.op_type == 1).ToArray()[0].op_area;
            decimal? leftMoney = computeInvoiceLeftMoney(model.inv_oid, model.inv_cid.Value);
            model.inv_overMoney = leftMoney - model.inv_money >= 0 ? 0 : leftMoney - model.inv_money;
            int ret = dal.Add(model);
            if (ret > 0)
            {
                StringBuilder content = new StringBuilder();
                content.Append("购买方名称：" + model.inv_purchaserName + "<br/>");
                content.Append("购买方账号：" + model.inv_purchaserBankNum + "<br/>");
                content.Append("金额：" + model.inv_money + "<br/>");
                content.Append("应税劳务、服务名称：" + model.inv_serviceType + "，" + model.inv_serviceName + "<br/>");
                content.Append("送票方式：" + model.inv_sentWay + "<br/>");
                content.Append("开票区域：" + model.inv_darea + "<br/>");
                content.Append("开票单位：" + model.inv_unit + "<br/>");

                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = ret;
                logmodel.ol_oid = model.inv_oid;
                logmodel.ol_cid = model.inv_cid.Value;
                logmodel.ol_title = "添加发票";
                logmodel.ol_content = content.ToString();
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Add.ToString(), logmodel, manager.user_name, manager.real_name); //记录日志

                //钉钉通知
                DataTable userDt = new BLL.manager().getUserByPermission("0603", model.inv_farea).Tables[0];
                if (userDt != null)
                {
                    string replaceContent = new BLL.Customer().GetModel(model.inv_cid.Value).c_name + "," + model.inv_money;
                    string replaceUser = manager.user_name + "," + manager.real_name;
                    foreach (DataRow dr in userDt.Rows)
                    {
                        //钉钉推送通知
                        if (!string.IsNullOrEmpty(Utils.ObjectToStr(dr["oauth_userid"])))
                        {
                            new BLL.selfMessage().sentDingMessage("添加发票", dr["oauth_userid"].ToString(), replaceContent, replaceUser);
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
        public string Update(Model.invoices model, string content, Model.manager manager)
        {
            if (model == null) return "数据不存在";
            Model.Order order = new BLL.Order().GetModel(model.inv_oid);
            if (order == null)
            {
                return "订单不存在";
            }
            if (!new BLL.permission().checkHasPermission(manager, "0401"))//如果不是财务
            {
                //验证权限：在同一个订单里，业务员与业务报账员可以对未审核地接进行编辑与删除！执行人员只能对自己地址进行编辑与删除操作！
                if (model.inv_personNum != manager.user_name && order.personlist.Where(p => p.op_number == manager.user_name && (p.op_type == 3 || p.op_type == 4)).ToArray().Length > 0)
                {
                    return "无权限编辑";
                }
            }
            else
            {
                if (model.inv_personNum != manager.user_name && !new BLL.permission().checkHasPermission(manager, "0403"))
                {
                    return "无权限编辑";
                }
            }
            if (model.inv_flag1 == 2 && model.inv_flag2 != 1 && model.inv_flag3 != 1)
            {
                return "已审批通过的不能编辑";
            }
            #region 验证数据
            if (string.IsNullOrEmpty(model.inv_purchaserName))
            {
                return "请填写购买方名称";
            }
            if (string.IsNullOrEmpty(model.inv_purchaserNum))
            {
                return "请填写购买方纳税人识别号";
            }
            if (string.IsNullOrEmpty(model.inv_purchaserAddress))
            {
                return "请填写购买方地址";
            }
            if (string.IsNullOrEmpty(model.inv_purchaserPhone))
            {
                return "请填写购买方电话";
            }
            if (string.IsNullOrEmpty(model.inv_purchaserBank))
            {
                return "请填写购买方开户行";
            }
            if (string.IsNullOrEmpty(model.inv_purchaserBankNum))
            {
                return "请填写购买方账号";
            }
            if (string.IsNullOrEmpty(model.inv_serviceType))
            {
                return "请选择应税劳务";
            }
            if (string.IsNullOrEmpty(model.inv_serviceName))
            {
                return "请选择服务名称";
            }
            //if (model.inv_money <= 0)
            //{
            //    return "开票金额必须大于0";
            //}
            if (string.IsNullOrEmpty(model.inv_sentWay))
            {
                return "请选择送票方式";
            }
            if (model.inv_sentWay == "邮寄")
            {
                if (string.IsNullOrEmpty(model.inv_receiveName))
                {
                    return "请填写收票人名称";
                }
                if (string.IsNullOrEmpty(model.inv_receivePhone))
                {
                    return "请填写收票人电话";
                }
                if (string.IsNullOrEmpty(model.inv_receiveAddress))
                {
                    return "请填写收票人地址";
                }
            }
            if (string.IsNullOrEmpty(model.inv_darea))
            {
                return "请选择开票区域";
            }
            if (model.inv_unit == 0)
            {
                return "请选择开票单位";
            }
            #endregion
            if (model.inv_flag1 == 1 || model.inv_flag2 == 1 || model.inv_flag3 == 1)
            {
                model.inv_flag1 = 0;
                model.inv_checkName1 = "";
                model.inv_checkNum1 = "";
                model.inv_checkRemark1 = "";
                model.inv_checkTime1 = null;
                model.inv_flag2 = 0;
                model.inv_checkName2 = "";
                model.inv_checkNum2 = "";
                model.inv_checkRemark2 = "";
                model.inv_checkTime2 = null;
                model.inv_flag3 = 0;
                model.inv_checkName3 = "";
                model.inv_checkNum3 = "";
                model.inv_checkRemark3 = "";
                model.inv_checkTime3 = null;
            }
            if (dal.Update(model))
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = model.inv_id.Value;
                logmodel.ol_oid = model.inv_oid;
                logmodel.ol_cid = model.inv_cid.Value;
                logmodel.ol_title = "编辑发票";
                logmodel.ol_content = content.ToString();
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Edit.ToString(), logmodel, manager.user_name, manager.real_name); //记录日志

                //钉钉通知
                DataTable userDt = new BLL.manager().getUserByPermission("0603", model.inv_farea).Tables[0];
                if (userDt != null)
                {
                    string replaceContent = new BLL.Customer().GetModel(model.inv_cid.Value).c_name + "," + model.inv_money;
                    string replaceUser = manager.user_name + "," + manager.real_name;
                    foreach (DataRow dr in userDt.Rows)
                    {
                        //钉钉推送通知
                        if (!string.IsNullOrEmpty(Utils.ObjectToStr(dr["oauth_userid"])))
                        {
                            new BLL.selfMessage().sentDingMessage("添加发票", dr["oauth_userid"].ToString(), replaceContent, replaceUser);
                        }
                    }
                }

                return "";
            }
            return "修改失败";
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public string Delete(int id, Model.manager manager)
        {
            Model.invoices model = GetModel(id);
            if (model == null) return "数据不存在";
            if (model.inv_flag3 == 2)
            {
                return "最终审批通过不能再编辑";
            }
            Model.Order order = new BLL.Order().GetModel(model.inv_oid);
            if (order == null)
            {
                return "订单不存在";
            }
            if (!new BLL.permission().checkHasPermission(manager, "0401"))//如果不是财务
            {
                //验证权限：在同一个订单里，业务员与业务报账员可以对未审核地接进行编辑与删除！执行人员只能对自己地址进行编辑与删除操作！
                if (model.inv_personNum != manager.user_name && order.personlist.Where(p => p.op_number == manager.user_name && (p.op_type == 3 || p.op_type == 4)).ToArray().Length > 0)
                {
                    return "无权限删除";
                }
            }
            else
            {
                if (model.inv_personNum != manager.user_name && !new BLL.permission().checkHasPermission(manager, "0403"))
                {
                    return "非申请人或没有删除他人数据权限不能删除";
                }
            }
            if (dal.Delete(id))
            {
                StringBuilder content = new StringBuilder();
                content.Append("购买方名称：" + model.inv_purchaserName + "<br/>");
                content.Append("购买方账号：" + model.inv_purchaserBankNum + "<br/>");
                content.Append("金额：" + model.inv_money + "<br/>");
                content.Append("应税劳务、服务名称：" + model.inv_serviceType + "，" + model.inv_serviceName + "<br/>");
                content.Append("送票方式：" + model.inv_sentWay + "<br/>");
                content.Append("开票区域：" + model.inv_darea + "<br/>");
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = model.inv_id.Value;
                logmodel.ol_oid = model.inv_oid;
                logmodel.ol_cid = model.inv_cid.Value;
                logmodel.ol_title = "删除发票";
                logmodel.ol_content = content.ToString();
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Delete.ToString(), logmodel, manager.user_name, manager.real_name); //记录日志
                return "";
            }
            return "删除失败";
        }
        
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.invoices GetModel(int id)
        {
            return dal.GetModel(id);
        }
        /// <summary>
        /// 发票审批
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <param name="remark"></param>
        /// <param name="username"></param>
        /// <param name="realname"></param>
        /// <returns></returns>
        public string checkInvoiceStatus(int id, byte? type, byte? status, string remark, Model.manager adminModel)
        {
            Model.invoices model = GetModel(id);
            if (model == null)
                return "数据不存在";
            string content = "";
            //3.如果是开票区域审批，要先验证申请区域审批是否已经审批通过，不通过不能审批；如果是财务审批，要先验证开票区域审批是否已经审批通过，不通过不能审批
            //4.反审批时：a.财务要先验证是否已经开票。b.开票区域要先验证财务是否已经审批通过。c.申请区域要先验证开票区域是否已经审批通过
            switch (type)
            {
                case 1://申请区域审批
                    if (model.inv_flag1 == status) return "状态未变更";
                    //判断有没有部门审批权限
                    if (model.inv_farea != adminModel.area || !new permission().checkHasPermission(adminModel, "0603"))
                    {
                        return "无权限审批";
                    }
                    
                    if (status == 2)
                    {
                        //由待审批、审批未通过→审批通过
                        //当申请区域和开票区域相同时，申请区域审批通过时，同时把开票区域审批通过
                        if (model.inv_farea == model.inv_darea)
                        {
                            model.inv_flag2 = status;
                            model.inv_checkNum2 = adminModel.user_name;
                            model.inv_checkName2 = adminModel.real_name;
                            model.inv_checkRemark2 = remark;
                            model.inv_checkTime2 = DateTime.Now;
                        }
                    }
                    else
                    {
                        //由审批通过→待审批、审批未通过：验证开票区域审批是否通过，审批通过的不能再做申请区域反审批
                        if (model.inv_flag2 == 2)
                        {
                            return "开票区域已经审批通过，不能做申请区域审批";
                        }
                    }
                    content = "记录id：" + id + "，申请区域审批状态：" + Common.BusinessDict.checkStatus()[model.inv_flag1] + "→<font color='red'>" + Common.BusinessDict.checkStatus()[status] + "</font>";
                    model.inv_flag1 = status;
                    model.inv_checkNum1 = adminModel.user_name;
                    model.inv_checkName1 = adminModel.real_name;
                    model.inv_checkRemark1 = remark;
                    model.inv_checkTime1 = DateTime.Now;
                    break;
                case 2://开票区域审批
                    if (model.inv_flag2 == status) return "状态未变更";
                    //判断有没有部门审批权限
                    if (model.inv_darea != adminModel.area || !new permission().checkHasPermission(adminModel, "0603"))
                    {
                        return "无权限审批";
                    }
                    if (status == 2)
                    {
                        //由待审批、审批未通过→审批通过：验证申请区域审批是否存在待审批或审批未通过的记录，存在则不能做开票区域审批
                        if (model.inv_flag1 != 2)
                        {
                            return "申请区域审批是待审批或审批未通过的，不能做开票区域审批";
                        }
                    }
                    else
                    {
                        //由审批通过→待审批、审批未通过：验证财务审批是否通过，审批通过的不能再做开票区域反审批
                        if (model.inv_flag3 == 2)
                        {
                            return "财务已经审批通过的，不能做开票区域审批";
                        }
                        //当申请区域和开票区域相同时，开票区域反审批时，同时把申请区域反审批
                        if (model.inv_farea == model.inv_darea)
                        {
                            model.inv_flag1 = status;
                            model.inv_checkNum1 = adminModel.user_name;
                            model.inv_checkName1 = adminModel.real_name;
                            model.inv_checkRemark1 = remark;
                            model.inv_checkTime1 = DateTime.Now;
                        }
                    }
                    content = "记录id：" + id + "，开票区域审批状态：" + Common.BusinessDict.checkStatus()[model.inv_flag2] + "→<font color='red'>" + Common.BusinessDict.checkStatus()[status] + "</font>";
                    model.inv_flag2 = status;
                    model.inv_checkNum2 = adminModel.user_name;
                    model.inv_checkName2 = adminModel.real_name;
                    model.inv_checkRemark2 = remark;
                    model.inv_checkTime2 = DateTime.Now;
                    break;
                case 3://财务审批
                    if (model.inv_flag3 == status) return "状态未改变";
                    if (new BLL.department().getGroupArea() != adminModel.area || !new permission().checkHasPermission(adminModel, "0402"))
                    {
                        return "无权限审批";
                    }
                    if (status == 2)
                    {
                        //由待审批、审批未通过→审批通过：验证开票区域审批是否存在待审批或审批未通过的记录，存在则不能做财务审批
                        if (model.inv_flag2 != 2)
                        {
                            return "开票区域审批是待审批或审批未通过的，不能做财务审批";
                        }
                    }
                    else
                    {
                        //由审批通过→待审批、审批未通过：验证是否已经开票，已经开票的不能做财务审批
                        if (model.inv_isConfirm.Value)
                        {
                            return "已经开票，不能做财务审批";
                        }
                    }
                    content = "记录id：" + id + "，开票区域审批状态：" + Common.BusinessDict.checkStatus()[model.inv_flag3] + "→<font color='red'>" + Common.BusinessDict.checkStatus()[status] + "</font>";
                    model.inv_flag3 = status;
                    model.inv_checkNum3 = adminModel.user_name;
                    model.inv_checkName3 = adminModel.real_name;
                    model.inv_checkRemark3 = remark;
                    model.inv_checkTime3 = DateTime.Now;
                    break;
            }
            if (dal.Update(model))
            {
                //写日志
                Model.business_log log = new Model.business_log();
                log.ol_title = "审批发票申请";
                log.ol_oid = model.inv_oid;
                log.ol_cid = model.inv_cid.Value;
                log.ol_relateID = id;
                string _content = content;
                log.ol_content = _content;
                new business_log().Add(DTEnums.ActionEnum.Audit.ToString(), log, adminModel.user_name, adminModel.real_name);

                //信息通知下申请通知人、业务员
                if (status == 1)
                {
                    string replaceContent = new BLL.Customer().GetModel(model.inv_cid.Value).c_name + "," + model.inv_money;
                    string replaceUser = adminModel.user_name + "," + adminModel.real_name;
                    new BLL.selfMessage().AddMessage("开票申请审批未通过", model.inv_personNum, model.inv_personName, replaceContent, replaceUser);

                    //通知业务员
                    DataSet ds = new BLL.Order().GetPersonList(0, "op_oid='" + model.inv_oid + "' and op_type=1", "");
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        new BLL.selfMessage().AddMessage("开票申请审批未通过", ds.Tables[0].Rows[0]["op_number"].ToString(), ds.Tables[0].Rows[0]["op_name"].ToString(), replaceContent, replaceUser);
                    }

                    //钉钉通知申请人
                    Model.manager_oauth oauthModel = new BLL.manager_oauth().GetModel(model.inv_personNum);
                    if (oauthModel != null && oauthModel.is_lock == 1 && !string.IsNullOrEmpty(oauthModel.oauth_userid))
                    {
                        new BLL.selfMessage().sentDingMessage("发票申请审批未通过", oauthModel.oauth_userid, replaceContent, replaceUser);
                    }
                }
                //信息通知下申请通知人、业务员,如果是申请区域审批和开票区域审批 要通知下一级审批人
                if (status == 2)
                {
                    string replaceContent = new BLL.Customer().GetModel(model.inv_cid.Value).c_name + "," + model.inv_money;
                    string replaceUser = adminModel.user_name + "," + adminModel.real_name;
                    new BLL.selfMessage().AddMessage("开票申请审批通过1", model.inv_personNum, model.inv_personName, replaceContent, replaceUser);


                    //钉钉通知申请人
                    Model.manager_oauth oauthModel = new BLL.manager_oauth().GetModel(model.inv_personNum);
                    if (oauthModel != null && oauthModel.is_lock == 1 && !string.IsNullOrEmpty(oauthModel.oauth_userid))
                    {
                        new BLL.selfMessage().sentDingMessage("发票申请审批通过", oauthModel.oauth_userid, replaceContent, replaceUser);
                    }

                    //通知业务员
                    DataSet ds = new BLL.Order().GetPersonList(0, "op_oid='" + model.inv_oid + "' and op_type=1", "");
                    if (ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        new BLL.selfMessage().AddMessage("开票申请审批通过1", ds.Tables[0].Rows[0]["op_number"].ToString(), ds.Tables[0].Rows[0]["op_name"].ToString(), replaceContent, replaceUser);
                    }

                    if (type == 1)//申请区域审批，审批通过要通知开票区域审批人
                    {
                        if (model.inv_farea == model.inv_darea)
                        {
                            DataTable userDt = new BLL.manager().getUserByPermission("0402", new BLL.department().getGroupArea()).Tables[0];
                            if (userDt != null)
                            {
                                foreach (DataRow dr in userDt.Rows)
                                {
                                    new BLL.selfMessage().AddMessage("开票申请审批通过2", dr["user_name"].ToString(), dr["real_name"].ToString(), replaceContent, replaceUser);

                                    //钉钉推送通知
                                    if (!string.IsNullOrEmpty(Utils.ObjectToStr(dr["oauth_userid"])))
                                    {
                                        new BLL.selfMessage().sentDingMessage("开票申请审批通过2", dr["oauth_userid"].ToString(), replaceContent, replaceUser);
                                    }
                                }
                            }
                        }
                        else
                        {
                            DataTable userDt = new BLL.manager().getUserByPermission("0603", model.inv_darea).Tables[0];
                            if (userDt != null)
                            {
                                foreach (DataRow dr in userDt.Rows)
                                {
                                    new BLL.selfMessage().AddMessage("开票申请审批通过2", dr["user_name"].ToString(), dr["real_name"].ToString(), replaceContent, replaceUser);

                                    //钉钉推送通知
                                    if (!string.IsNullOrEmpty(Utils.ObjectToStr(dr["oauth_userid"])))
                                    {
                                        new BLL.selfMessage().sentDingMessage("开票申请审批通过2", dr["oauth_userid"].ToString(), replaceContent, replaceUser);
                                    }
                                }
                            }
                        }
                    }
                    if (type == 2)//开票区域审批，审批通过要通知财务审批人
                    {
                        DataTable userDt = new BLL.manager().getUserByPermission("0402", new BLL.department().getGroupArea()).Tables[0];
                        if (userDt != null)
                        {
                            foreach (DataRow dr in userDt.Rows)
                            {
                                new BLL.selfMessage().AddMessage("开票申请审批通过2", dr["user_name"].ToString(), dr["real_name"].ToString(), replaceContent, replaceUser);

                                //钉钉推送通知
                                if (!string.IsNullOrEmpty(Utils.ObjectToStr(dr["oauth_userid"])))
                                {
                                    new BLL.selfMessage().sentDingMessage("开票申请审批通过2", dr["oauth_userid"].ToString(), replaceContent, replaceUser);
                                }
                            }
                        }
                    }
                }
                return "";
            }
            return "操作失败";
        }

        /// <summary>
        /// 开票
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="username"></param>
        /// <param name="realname"></param>
        /// <returns></returns>
        public string confirmInvoice(int id, bool? status,string date, Model.manager adminModel)
        {
            if (status.Value && string.IsNullOrEmpty(date))
            {
                return "请填写开票日期";
            }
            Model.invoices model = GetModel(id);
            if (model == null) return "数据不存在";
            if (model.inv_isConfirm == status)
            {
                return "状态未变更";
            }
            if (model.inv_flag3 != 2)
            {
                return "财务审批未通过，不能开票";
            }
            if (!new permission().checkHasPermission(adminModel, "0408"))
            {
                return "无权限开票";
            }
            string _content = "发票id：" + id + "，开票状态：" + Common.BusinessDict.invoiceConfirmStatus()[model.inv_isConfirm] + "→<font color='red'>" + Common.BusinessDict.invoiceConfirmStatus()[status] + "</font>，开票日期：" + date;
            if (dal.confirmInvoice(id, status, ConvertHelper.toDate(date), adminModel.user_name, adminModel.real_name))
            {
                //写日志
                Model.business_log log = new Model.business_log();
                log.ol_title = "发票开票";
                log.ol_oid = model.inv_oid;
                log.ol_cid =model.inv_cid.Value;
                log.ol_relateID = id;
                log.ol_content = _content;
                new business_log().Add(DTEnums.ActionEnum.Audit.ToString(), log, adminModel.user_name, adminModel.real_name);

                //信息通知下申请通知人、业务员
                if (status.Value)
                {
                    string replaceContent = new BLL.Customer().GetModel(model.inv_cid.Value).c_name + "," + model.inv_money;
                    string replaceUser = adminModel.user_name + "," + adminModel.real_name;
                    new BLL.selfMessage().AddMessage("开票申请财务已开具", model.inv_personNum, model.inv_personName, replaceContent, replaceUser);

                    //通知业务员
                    DataSet ds = new BLL.Order().GetPersonList(0, "op_oid='" + model.inv_oid + "' and op_type=1", "");
                    if(ds != null && ds.Tables[0].Rows.Count > 0)
                    {
                        new BLL.selfMessage().AddMessage("开票申请财务已开具", ds.Tables[0].Rows[0]["op_number"].ToString(), ds.Tables[0].Rows[0]["op_name"].ToString(), replaceContent, replaceUser);
                    }
                }
                return "";
            }
            return "操作失败";
        }
        /// <summary>
        /// 计算某个客户到现在为止在某个订单中剩余的开票金额
        /// </summary>
        /// <param name="cid"></param>
        /// <param name="oid"></param>
        /// <returns></returns>
        public decimal? computeInvoiceLeftMoney(string oid,int cid=0)
        {
            if (string.IsNullOrEmpty(oid))
            {
                return 0;
            }
            return dal.computeInvoiceLeftMoney(oid, cid);
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
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder,Model.manager manager, out int recordCount, out decimal pmoney, bool isPage = true)
        {
            //列表权限控制
            if (manager.area != new BLL.department().getGroupArea())//如果不是总部的工号
            {
                if (new BLL.permission().checkHasPermission(manager, "0602"))
                {
                    //含有区域权限可以查看本区域添加的
                    strWhere += " and (inv_farea='" + manager.area + "' or inv_darea='" + manager.area + "')";
                }
                else
                {
                    //只能
                    strWhere += " and inv_PersonNum='" + manager.user_name + "'";
                }
            }
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount, out pmoney, isPage);
        }
        #endregion

        #region 扩展方法================================

        #endregion
    }
}
