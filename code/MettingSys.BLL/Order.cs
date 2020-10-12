using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using MettingSys.Common;
using MettingSys.Model;

namespace MettingSys.BLL
{
    /// <summary>
    ///订单表
    /// </summary>
    public partial class Order
    {
        private readonly DAL.Order dal;
        public Order()
        {
            dal = new DAL.Order();
        }

        #region 基本方法================================
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int cid, string content, DateTime? sdate, string id = "")
        {
            return dal.Exists(cid, content, sdate, id);
        }

        /// <summary>
        /// 添加订单
        /// </summary>
        /// <param name="model"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public string AddOrder(Model.Order model, Model.manager manager, out string o_id)
        {           
            model.o_id = getOrderID();
            o_id = model.o_id;
            model.o_lockStatus = 0;
            model.o_isPush = false;
            model.o_flag = 0;
            model.o_addDate = DateTime.Now;
            if (model.o_cid == 0)
            {
                return "请选择客户";
            }
            if (model.o_coid == 0)
            {
                return "请选择联系人";
            }
            if (string.IsNullOrEmpty(model.o_contractPrice))
            {
                return "请选择合同造价";
            }
            if (model.o_sdate == null)
            {
                return "请选择活动开始日期";
            }
            if (model.o_edate == null)
            {
                return "请选择活动结束日期";
            }
            if (model.o_sdate > model.o_edate)
            {
                return "活动开始日期不能大于活动结束日期";
            }
            if (string.IsNullOrEmpty(model.o_address))
            {
                return "请填写活动地点";
            }
            if (string.IsNullOrEmpty(model.o_content))
            {
                return "请填写活动名称";
            }
            if (string.IsNullOrEmpty(model.o_place))
            {
                return "请选择活动归属地";
            }
            if (model.o_place.IndexOf(manager.area) < 0)
            {
                return "活动归属地须包含下单人所在区域";
            }
            if (Exists(model.o_cid.Value,model.o_content,model.o_sdate))
            {
                return "存在相同的客户、活动名称、活动开始日期的订单，请确认是否重复下单";
            }

            //添加下单人
            model.personlist.Add(new OrderPerson() { op_oid = model.o_id, op_type = 1, op_number = manager.user_name, op_name = manager.real_name, op_area = manager.area, op_addTime = DateTime.Now });
            string person2 = string.Empty, person3 = string.Empty, person4 = string.Empty, person5 = string.Empty;
            IEnumerable<OrderPerson> list = model.personlist.Where(p => p.op_type == 2);
            if (list.ToArray().Length != 1)
            {
                return "必须添加业务报账人员，且只能添加一个";
            }
            foreach (var item in list)
            {
                person2 += "[" + item.op_name + "," + item.op_number + "," + item.op_area + "],";
            }
            person2 = person2.TrimEnd(',');
            list = model.personlist.Where(p => p.op_type == 3);
            if (list.ToArray().Length > 0)
            {
                foreach (var item in list)
                {
                    person3 += "[" + item.op_name + "," + item.op_number + "," + item.op_area + "," + item.op_dstatus + "],";
                }
                person3 = person3.TrimEnd(',');
            }
            list = model.personlist.Where(p => p.op_type == 4);
            if (list.ToArray().Length > 0)
            {
                foreach (var item in list)
                {
                    person4 += "[" + item.op_name + "," + item.op_number + "," + item.op_area + "],";
                }
                person4 = person4.TrimEnd(',');
            }
            list = model.personlist.Where(p => p.op_type == 5);
            if (list.ToArray().Length > 0)
            {
                foreach (var item in list)
                {
                    if (person3.IndexOf(item.op_number)>-1)
                    {
                        return "一个人不能同时是策划人员和设计人员";
                    }
                    person5 += "[" + item.op_name + "," + item.op_number + "," + item.op_area + "," + item.op_dstatus + "],";
                }
                person5 = person5.TrimEnd(',');
            }

            //当订单状态为确认时，添加订单确认时间
            if (model.o_status == 2)
            {
                model.o_statusTime = DateTime.Now;
            }
            else
            {
                model.o_statusTime = null;
            }

            if (dal.AddOrder(model))
            {
                StringBuilder content = new StringBuilder();
                content.Append("订单号：" + model.o_id + "<br/>");
                content.Append("客户ID：" + model.o_cid + ",联系人ID：" + model.o_coid + "<br/>");
                content.Append("合同造价：" + model.o_contractPrice + "<br/>");
                content.Append("活动日期：" + model.o_sdate.Value.ToString("yyyy-MM-dd") + "/" + model.o_edate.Value.ToString("yyyy-MM-dd") + "<br/>");
                content.Append("活动地点：" + model.o_address + "<br/>");
                content.Append("活动名称：" + model.o_content + "<br/>");
                content.Append("合同内容：" + model.o_contractContent + "<br/>");
                content.Append("备注：" + model.o_remarks + "<br/>");
                content.Append("活动归属地：" + model.o_place + "<br/>");
                content.Append("业务报账员：" + person2 + "<br/>");
                content.Append("业务设计策划人员：" + person3 + "<br/>");
                content.Append("业务执行人员：" + person4 + "<br/>");
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_oid = model.o_id;
                logmodel.ol_cid = model.o_cid.Value;
                logmodel.ol_title = "添加订单";
                logmodel.ol_content = content.ToString();
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Add.ToString(), logmodel, manager.user_name, manager.real_name);

                #region 消息通知
                string replaceContent = model.o_id + "," + model.o_content + "," + model.o_address + "," + model.o_sdate.Value.ToString("yyyy-MM-dd");
                string replaceUser = manager.user_name + "," + manager.real_name;
                list = model.personlist.Where(p => p.op_type == 2);
                foreach (OrderPerson op in list)
                {
                    new BLL.selfMessage().AddMessage("增加业务报账人员", op.op_number, op.op_name, replaceContent, replaceUser);
                }
                list = model.personlist.Where(p => p.op_type == 3);
                foreach (OrderPerson op in list)
                {
                    new BLL.selfMessage().AddMessage("增加业务设计策划人员", op.op_number, op.op_name, replaceContent, replaceUser);
                }
                list = model.personlist.Where(p => p.op_type == 4);
                foreach (OrderPerson op in list)
                {
                    new BLL.selfMessage().AddMessage("增加业务执行人员", op.op_number, op.op_name, replaceContent, replaceUser);
                }
                if (model.o_contractPrice=="100万以上")
                {
                    //通知总经理
                    string area = new BLL.department().getGroupArea();
                    DataTable userDt = new BLL.manager().getUserByPermission("0601", area).Tables[0];
                    if (userDt != null)
                    {
                        foreach (DataRow dr in userDt.Rows)
                        {
                            new BLL.selfMessage().AddMessage("新增特大业务活动", dr["user_name"].ToString(), dr["real_name"].ToString(), replaceContent, replaceUser);
                        }
                    }
                    //通知副总经理
                    userDt = new BLL.manager().getUserByPermission("0604", area).Tables[0];
                    if (userDt != null)
                    {
                        foreach (DataRow dr in userDt.Rows)
                        {
                            new BLL.selfMessage().AddMessage("新增特大业务活动", dr["user_name"].ToString(), dr["real_name"].ToString(), replaceContent, replaceUser);
                        }
                    }
                    //通知分公司负责人
                    userDt = new BLL.manager().getUserByPermission("0603", manager.area).Tables[0];
                    if (userDt != null)
                    {
                        foreach (DataRow dr in userDt.Rows)
                        {
                            new BLL.selfMessage().AddMessage("新增特大业务活动", dr["user_name"].ToString(), dr["real_name"].ToString(), replaceContent, replaceUser);
                        }
                    }
                }
                #endregion

                return "";
            }
            return "添加订单失败";
        }

        /// <summary>
        /// 编辑订单
        /// </summary>
        /// <param name="model"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public string UpdateOrder(Model.Order newModel, Model.manager manager)
        {
            bool isSent = false;
            Model.Order oldModel = GetModel(newModel.o_id);
            if (oldModel == null) return "订单不存在";
            if (oldModel.o_lockStatus==1) return "已锁单，不能再编辑订单信息";

            newModel.o_lastUpdateDate = DateTime.Now;
            IEnumerable<OrderPerson> list = oldModel.personlist.Where(p => (p.op_type == 1 && p.op_number== manager.user_name) || (p.op_type == 2 && p.op_number == manager.user_name));
            if (list.ToArray().Length == 0)
            {
                return "非下单人员和业务报账人员不能编辑订单信息";
            }

            string content = "";
            if (newModel.o_cid == 0)
            {
                return "请选择客户";
            }
            else
            {
                if (oldModel.o_cid != newModel.o_cid)
                {
                    content += "客户ID：" + oldModel.o_cid + "→<font color='red'>" + newModel.o_cid + "</font><br/>";
                }
            }
            if (newModel.o_coid == 0)
            {
                return "请选择联系人";
            }
            else
            {
                if (oldModel.o_coid != newModel.o_coid)
                {
                    content += "联系人ID：" + oldModel.o_coid + "→<font color='red'>" + newModel.o_coid + "</font><br/>";
                }
            }
            if (string.IsNullOrEmpty(newModel.o_content))
            {
                return "请填写活动名称";
            }
            else
            {
                if (oldModel.o_content != newModel.o_content)
                {
                    content += "活动名称：" + oldModel.o_content + "→<font color='red'>" + newModel.o_content + "</font><br/>";
                }
            }
            if (string.IsNullOrEmpty(newModel.o_address))
            {
                return "请填写活动地点";
            }
            else
            {
                if (oldModel.o_address != newModel.o_address)
                {
                    content += "活动地点：" + oldModel.o_address + "→<font color='red'>" + newModel.o_address + "</font><br/>";
                }
            }
            if (string.IsNullOrEmpty(newModel.o_contractPrice))
            {
                return "请选择合同造价";
            }
            else
            {
                if (oldModel.o_contractPrice != newModel.o_contractPrice)
                {
                    content += "合同造价：" + oldModel.o_contractPrice + "→<font color='red'>" + newModel.o_contractPrice + "</font><br/>";
                }
            }
            if (oldModel.o_contractContent != newModel.o_contractContent)
            {
                content += "合同内容：" + oldModel.o_contractContent + "→<font color='red'>" + newModel.o_contractContent + "</font><br/>";
            }
            if (newModel.o_sdate == null)
            {
                return "请选择活动开始日期";
            }
            else
            {
                if (oldModel.o_sdate != newModel.o_sdate)
                {
                    content += "活动开始日期：" + oldModel.o_sdate.Value.ToString("yyyy-MM-dd") + "→<font color='red'>" + newModel.o_sdate.Value.ToString("yyyy-MM-dd") + "</font><br/>";
                }
            }
            if (newModel.o_edate == null)
            {
                return "请选择活动结束日期";
            }
            else
            {
                if (oldModel.o_edate != newModel.o_edate)
                {
                    content += "活动结束日期：" + oldModel.o_edate.Value.ToString("yyyy-MM-dd") + "→<font color='red'>" + newModel.o_edate.Value.ToString("yyyy-MM-dd") + "</font><br/>";
                }
            }
            if (oldModel.o_place != newModel.o_place)
            {
                content += "活动归属地：" + oldModel.o_place + "→<font color='red'>" + newModel.o_place + "</font><br/>";
            }
            bool isChangeStatus = false;

            if (oldModel.o_status != newModel.o_status)
            {
                isChangeStatus = true;
                content += "订单状态：" + Common.BusinessDict.fStatus()[oldModel.o_status] + "→<font color='red'>" + Common.BusinessDict.fStatus()[newModel.o_status] + "</font><br/>";
                if (oldModel.o_status != 2 && newModel.o_status == 2)
                {
                    newModel.o_statusTime = DateTime.Now;
                }
                if (oldModel.o_status == 2 && newModel.o_status != 2)
                {
                    newModel.o_statusTime = null;
                }
            }
            else
            {
                newModel.o_statusTime = oldModel.o_statusTime;
            }
            if (oldModel.o_isPush != newModel.o_isPush)
            {
                if (newModel.o_isPush.Value)
                {
                    isSent = true;
                    if (newModel.o_status == 0)
                    {
                        return "订单状态为待定时，不能推送上级审批";
                    }
                }
                content += "推送上级审批：" + Common.BusinessDict.pushStatus()[oldModel.o_isPush] + "→<font color='red'>" + Common.BusinessDict.pushStatus()[newModel.o_isPush] + "</font><br/>";
            }
            if (oldModel.o_remarks != newModel.o_remarks)
            {
                content += "备注：" + oldModel.o_remarks + "→<font color='red'>" + newModel.o_remarks + "</font><br/>";
            }

            if (newModel.o_place.IndexOf(manager.area) < 0)
            {
                return "活动归属地须包含下单人所在区域";
            }
            if (Exists(newModel.o_cid.Value, newModel.o_content, newModel.o_sdate, newModel.o_id))
            {
                return "存在相同的客户、活动名称、活动开始日期的订单，请确认是否重复下单";
            }
            //添加下单人
            IEnumerable<OrderPerson> list0 = oldModel.personlist.Where(p => p.op_type == 1);
            newModel.personlist.Add(new OrderPerson() { op_oid = newModel.o_id, op_type = 1, op_number = list0.ToArray()[0].op_number, op_name = list0.ToArray()[0].op_name, op_area = list0.ToArray()[0].op_area,op_addTime = list0.ToArray()[0].op_addTime });

            string oStr2 = string.Empty, oStr3 = string.Empty, oStr4 = string.Empty, oStr5 = string.Empty;//旧人员
            string nStr2 = string.Empty, nStr3 = string.Empty, nStr4 = string.Empty, nStr5 = string.Empty;//新人员
            IEnumerable<OrderPerson> oli = null, nli = null;
            #region 业务报账人员
            oli = oldModel.personlist.Where(p => p.op_type == 2);
            nli = newModel.personlist.Where(p => p.op_type == 2);            
            List<OrderPerson> addlist2 = null, cutlist2 = null;
            dealPerson(nli, oli, out addlist2, out cutlist2);
            foreach (var item in oli)
            {
                oStr2 += "[" + item.op_name + "," + item.op_number + "," + item.op_area + "],";
            }
            oStr2 = oStr2.TrimEnd(',');
            if (nli.ToArray().Length ==1)
            {
                foreach (OrderPerson item in nli)
                {
                    nStr2 += "[" + item.op_name + "," + item.op_number + "," + item.op_area + "],";
                }
                nStr2 = nStr2.TrimEnd(',');
            }
            else
            {
                return "必须添加业务报账人员，且只能添加一个";
            }
            #endregion
            #region 策划人员
            oli = oldModel.personlist.Where(p => p.op_type == 3);
            nli = newModel.personlist.Where(p => p.op_type == 3);
            List<OrderPerson> addlist3 = null, cutlist3 = null;
            dealPerson(nli, oli, out addlist3, out cutlist3);
            foreach (var item in oli)
            {
                oStr3 += "[" + item.op_name + "," + item.op_number + "," + item.op_area + "," + item.op_dstatus + "],";
            }
            oStr3 = oStr3.TrimEnd(',');
            if (nli.ToArray().Length > 0)
            {
                foreach (OrderPerson item in nli)
                {
                    nStr3 += "[" + item.op_name + "," + item.op_number + "," + item.op_area + "," + item.op_dstatus + "],";
                }
                nStr3 = nStr3.TrimEnd(',');
            }
            #endregion
            #region 执行人员
            oli = oldModel.personlist.Where(p => p.op_type == 4);
            nli = newModel.personlist.Where(p => p.op_type == 4);
            List<OrderPerson> addlist4 = null, cutlist4 = null;
            dealPerson(nli, oli, out addlist4, out cutlist4);
            foreach (var item in oli)
            {
                oStr4 += "[" + item.op_name + "," + item.op_number + "," + item.op_area + "],";
            }
            oStr4 = oStr4.TrimEnd(',');
            if (nli.ToArray().Length > 0)
            {
                foreach (OrderPerson item in nli)
                {
                    nStr4 += "[" + item.op_name + "," + item.op_number + "," + item.op_area + "],";
                }
                nStr4 = nStr4.TrimEnd(',');
            }
            #endregion
            #region 设计人员
            oli = oldModel.personlist.Where(p => p.op_type == 5);
            nli = newModel.personlist.Where(p => p.op_type == 5);
            List<OrderPerson> addlist5 = null, cutlist5 = null;
            dealPerson(nli, oli, out addlist5, out cutlist5);
            foreach (var item in oli)
            {
                oStr5 += "[" + item.op_name + "," + item.op_number + "," + item.op_area + "," + item.op_dstatus + "],";
            }
            oStr5 = oStr5.TrimEnd(',');
            if (nli.ToArray().Length > 0)
            {
                foreach (OrderPerson item in nli)
                {
                    if (nStr3.IndexOf(item.op_number) > -1)
                    {
                        return "一个人不能同时是策划人员和设计人员";
                    }
                    nStr5 += "[" + item.op_name + "," + item.op_number + "," + item.op_area + "," + item.op_dstatus + "],";
                }
                nStr5 = nStr5.TrimEnd(',');
            }
            else
            #endregion
            if (oStr2 != nStr2)
            {
                content += "业务报账人员：" + oStr2 + "→<font color='red'>" + nStr2 + "</font><br/>";
            }
            if (oStr3 != nStr3)
            {
                content += "业务策划人员：" + oStr3 + "→<font color='red'>" + nStr3 + "</font><br/>";
            }
            if (oStr4 != nStr4)
            {
                content += "执行人员：" + oStr4 + "→<font color='red'>" + nStr4 + "</font><br/>";
            }
            if (oStr5 != nStr5)
            {
                content += "业务设计人员：" + oStr5 + "→<font color='red'>" + nStr5 + "</font><br/>";
            }
            if (dal.UpdateOrder(newModel))
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_oid = newModel.o_id;
                logmodel.ol_cid = newModel.o_cid.Value;
                logmodel.ol_title = "编辑订单";
                logmodel.ol_content = content.ToString();
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Edit.ToString(), logmodel, manager.user_name, manager.real_name);

                #region 发送通知
                string replaceContent = newModel.o_id + "," + newModel.o_content + "," + newModel.o_address + "," + newModel.o_sdate.Value.ToString("yyyy-MM-dd");
                string replaceUser = "";
                //发送审批通知
                if (isSent)
                {
                    DataTable userDt = new BLL.manager().getUserByPermission("0603", list.ToArray()[0].op_area).Tables[0];
                    if (userDt != null)
                    {
                        foreach (DataRow dr in userDt.Rows)
                        {                            
                            replaceUser = list.ToArray()[0].op_number + "," + list.ToArray()[0].op_name;
                            new BLL.selfMessage().AddMessage("订单推送上级审批", dr["user_name"].ToString(), dr["real_name"].ToString(), replaceContent, replaceUser);

                            //钉钉推送通知
                            if (!string.IsNullOrEmpty(Utils.ObjectToStr(dr["oauth_userid"])))
                            {
                                new BLL.selfMessage().sentDingMessage("订单推送上级审批", dr["oauth_userid"].ToString(), replaceContent, replaceUser);
                            }
                        }
                    }
                }
                replaceUser = manager.user_name + "," + manager.real_name;
                foreach (OrderPerson item in addlist2)
                {
                    new BLL.selfMessage().AddMessage("增加业务报账人员", item.op_number, item.op_name, replaceContent, replaceUser);
                }
                foreach (OrderPerson item in cutlist2)
                {
                    new BLL.selfMessage().AddMessage("取消业务报账人员", item.op_number, item.op_name, replaceContent, replaceUser);
                }
                foreach (OrderPerson item in addlist3)
                {
                    new BLL.selfMessage().AddMessage("增加业务设计策划人员", item.op_number, item.op_name, replaceContent, replaceUser);
                }
                foreach (OrderPerson item in cutlist3)
                {
                    new BLL.selfMessage().AddMessage("取消业务设计策划人员", item.op_number, item.op_name, replaceContent, replaceUser);
                }
                foreach (OrderPerson item in addlist4)
                {
                    new BLL.selfMessage().AddMessage("增加业务执行人员", item.op_number, item.op_name, replaceContent, replaceUser);
                }
                foreach (OrderPerson item in cutlist4)
                {
                    new BLL.selfMessage().AddMessage("取消业务执行人员", item.op_number, item.op_name, replaceContent, replaceUser);
                }

                if (isChangeStatus)
                {
                    DataTable userDt = new BLL.manager().getUserByPermission("0603", manager.area).Tables[0];
                    if (userDt != null)
                    {
                        foreach (DataRow dr in userDt.Rows)
                        {
                            new BLL.selfMessage().AddMessage("订单状态变更", dr["user_name"].ToString(), dr["real_name"].ToString(), replaceContent, Common.BusinessDict.fStatus()[oldModel.o_status], Common.BusinessDict.fStatus()[newModel.o_status]);
                        }
                    }

                    if (newModel.o_contractPrice == "100万以上")
                    {
                        //通知总经理
                        string area = new BLL.department().getGroupArea();
                        userDt = new BLL.manager().getUserByPermission("0601", area).Tables[0];
                        if (userDt != null)
                        {
                            foreach (DataRow dr in userDt.Rows)
                            {
                                new BLL.selfMessage().AddMessage("特大业务活动状态变更", dr["user_name"].ToString(), dr["real_name"].ToString(), replaceContent, Common.BusinessDict.fStatus()[oldModel.o_status], Common.BusinessDict.fStatus()[newModel.o_status]);
                            }
                        }
                        //通知副总经理
                        userDt = new BLL.manager().getUserByPermission("0604", area).Tables[0];
                        if (userDt != null)
                        {
                            foreach (DataRow dr in userDt.Rows)
                            {
                                new BLL.selfMessage().AddMessage("特大业务活动状态变更", dr["user_name"].ToString(), dr["real_name"].ToString(), replaceContent, Common.BusinessDict.fStatus()[oldModel.o_status], Common.BusinessDict.fStatus()[newModel.o_status]);
                            }
                        }
                        //通知分公司负责人
                        userDt = new BLL.manager().getUserByPermission("0603", manager.area).Tables[0];
                        if (userDt != null)
                        {
                            foreach (DataRow dr in userDt.Rows)
                            {
                                new BLL.selfMessage().AddMessage("特大业务活动状态变更", dr["user_name"].ToString(), dr["real_name"].ToString(), replaceContent, Common.BusinessDict.fStatus()[oldModel.o_status], Common.BusinessDict.fStatus()[newModel.o_status]);
                            }
                        }
                    }
                }



                #endregion
                return "";
            }
            return "修改失败";
        }

        /// <summary>
        /// 删除订单
        /// </summary>
        /// <param name="oid"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public string DeleteOrder(string oid, Model.manager manager)
        {
            Model.Order model = GetModel(oid);
            if (model == null) return "订单不存在";
            OrderPerson person = model.personlist.Where(p => p.op_type == 1).ToArray().ToList()[0];
            if (person.op_number != manager.user_name)
            {
                if (!new BLL.permission().checkHasPermission(manager, "0403"))
                {
                    return "无权限删除";
                }
            }

            if (model.o_status == 2)
            {
                return "订单状态已确认，不能删除";
            }
            if (model.personlist.Where(p => (p.op_type == 3 || p.op_type == 5) && p.op_dstatus == 2).ToArray().Length>0)
            {
                return "接单状态存在已确认的，不能删除";
            }
            if (dal.checkOrderFiles(oid))
            {
                return "订单中存在上传文件，不能删除";
            }
            if (dal.checkOrderFinance(oid))
            {
                return "订单中存在应收付地接、已收付，发票申请、执行备用金借款等财务信息，不能删除";
            }
            if (dal.deleteOrder(oid))
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_oid = oid;
                logmodel.ol_title = "删除订单";
                logmodel.ol_content = "删除订单：" + oid;
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Delete.ToString(), logmodel, manager.user_name, manager.real_name);
                return "";
            }
            return "删除失败";
        }

        private void dealPerson(IEnumerable<OrderPerson> nli, IEnumerable<OrderPerson> oli, out List<OrderPerson> addPerson, out List<OrderPerson> cutPerson)
        {
            addPerson = new List<OrderPerson>();
            cutPerson = new List<OrderPerson>();
            //得到添加了哪些人
            foreach (OrderPerson item in nli)
            {
                IEnumerable<OrderPerson> li = oli.Where(p => p.op_number == item.op_number && p.op_name == item.op_name);
                if (li.ToArray().Length == 0)
                {
                    addPerson.Add(item);
                }
                else
                {
                    //如果新的人员数组中某个人存在于旧的人员数组中，则把人员的添加时间更新回旧的时间
                    item.op_addTime = li.ToArray()[0].op_addTime;
                }
                
            }
            //得到删除了哪些人
            foreach (OrderPerson item in oli)
            {
                IEnumerable<OrderPerson> li = nli.Where(p => p.op_number == item.op_number && p.op_name == item.op_name);
                if (li.ToArray().Length == 0)
                {
                    cutPerson.Add(item);
                }
            }
            
        }

        /// <summary>
        /// 获取所有策划和设计人员中订单的接单状态为“待定与处理中”的订单数量
        /// </summary>
        /// <returns></returns>
        public DataTable getAllDStatusOrder()
        {
            return dal.getAllDStatusOrder();
        }

        /// <summary>
        /// 检查订单中是否存在对应的应收或应付客户
        /// </summary>
        /// <param name="orderID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool checkOrderCusID(string orderID, bool type,int cusID)
        {
            return dal.checkOrderCusID(orderID, type, cusID);
        }

        /// <summary>
        /// 生成订单号
        /// </summary>
        /// <returns></returns>
        public string getOrderID()
        {
            string date = DateTime.Now.ToString("yyyyMMdd");
            DataSet ds = GetList(0, "o_id like '" + date + "%'", "o_id desc");
            if (ds == null || ds.Tables[0].Rows.Count == 0)
            {
                return date + "001";
            }
            DataRow dr = ds.Tables[0].Rows[0];
            string num = dr["o_id"].ToString().Substring(8, 3);

            num = (int.Parse(num) + 1).ToString();
            if (num.Length == 1)
            {
                num = "00" + num;
            }
            else if (num.Length == 2)
            {
                num = "0" + num;
            }
            return date + num;
        }

        /// <summary>
        /// 更新接单状态
        /// </summary>
        /// <param name="oID"></param>
        /// <param name="status"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public string updateDstatus(string oID, byte? status, Model.manager manager)
        {
            DataTable dt = GetPersonList(0, " o_id='" + oID + "' ", "").Tables[0];
            if (dt == null || dt.Rows.Count == 0)
            {
                return "订单不存在";
            }
            DataRow[] drs = dt.Select(" (op_type=3 or op_type=5) and op_number='" + manager.user_name + "' ");
            if (dt==null || dt.Rows.Count == 0)
            {
                return "无权限变更接单状态";
            }
            DataRow dr = drs[0];
            if (dr["o_lockStatus"].ToString()=="1") return "订单已经锁单，不能变更接单状态";
            if (dr["op_dstatus"].ToString() == status.ToString()) return "状态未变更";
            string content = "接单状态：" + Common.BusinessDict.dStatus()[Utils.ObjToByte(dr["op_dstatus"])] + "→<font color='red'>" + Common.BusinessDict.dStatus()[status] + "</font>";
            string replaceUser = Common.BusinessDict.dStatus()[Utils.ObjToByte(dr["op_dstatus"])],replaceUser1= Common.BusinessDict.dStatus()[status];
            //model.o_dstatus = status;
            if (dal.updateOrderDstatus(oID,manager.user_name, status))
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_oid = oID;
                logmodel.ol_cid = Utils.ObjToInt(dr["o_cid"]);
                logmodel.ol_title = "更新接单状态";
                logmodel.ol_content = content.ToString();
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Edit.ToString(), logmodel, manager.user_name, manager.real_name);

                //给业务员发消息
                //string replaceContent = oID + "," + dr["o_content"] + "," + dr["o_address"] + "," + ConvertHelper.toDate(dr["o_sdate"]).Value.ToString("yyyy-MM-dd");
                //DataRow[] ddr= dt.Select(" op_type=1");
                //new BLL.selfMessage().AddMessage("订单接单状态变更", ddr[0]["op_number"].ToString(), ddr[0]["op_name"].ToString(), replaceContent, replaceUser, replaceUser1);

                return "";
            }
            return "更新失败";
        }

        /// <summary>
        /// 更新审批状态
        /// </summary>
        /// <param name="oID"></param>
        /// <param name="flag"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public string updateFlag(string oID, byte? flag, Model.manager manager)
        {
            try
            {
                Model.Order model = GetModel(oID);
                if (model == null) return "订单不存在";
                IEnumerable<OrderPerson> op = model.personlist.Where(p => p.op_type == 1).ToArray();
                if (op.ToList()[0].op_area != manager.area || !new BLL.permission().checkHasPermission(manager, "0603"))
                {
                    return "无权限变更审批状态";
                }
                if (model.o_flag == flag) return "状态未变更";
                if (model.o_lockStatus==1) return "订单已经锁单，不能变更审批状态";
                string content = "审批状态：" + Common.BusinessDict.checkStatus()[model.o_flag] + "→<font color='red'>" + Common.BusinessDict.checkStatus()[flag] + "</font>";
                string replaceUser = Common.BusinessDict.checkStatus()[model.o_flag], replaceUser1 = Common.BusinessDict.checkStatus()[flag];
                model.o_flag = flag;
                if (Update(model))
                {
                    Model.business_log logmodel = new Model.business_log();
                    logmodel.ol_oid = model.o_id;
                    logmodel.ol_cid = model.o_cid.Value;
                    logmodel.ol_title = "更新审批状态";
                    logmodel.ol_content = content.ToString();
                    logmodel.ol_operateDate = DateTime.Now;
                    new business_log().Add(DTEnums.ActionEnum.Audit.ToString(), logmodel, manager.user_name, manager.real_name);

                    //给业务员、业务报账人员发消息
                    string replaceContent = model.o_id + "," + model.o_content + "," + model.o_address + "," + model.o_sdate.Value.ToString("yyyy-MM-dd");
                    IEnumerable<OrderPerson> list = model.personlist.Where(p => p.op_type == 1 || p.op_type == 2);
                    foreach (OrderPerson item in list)
                    {
                        new BLL.selfMessage().AddMessage("订单审批状态变更", item.op_number, item.op_name, replaceContent, replaceUser, replaceUser1);
                    }
                    
                    return "";
                }
                return "审批失败";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        /// <summary>
        /// 更新锁单状态
        /// </summary>
        /// <param name="oID"></param>
        /// <param name="flag"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public string updateLockStatus(string oID, byte? lockStatus, Model.manager manager)
        {
            Model.Order model = GetModel(oID);
            if (model == null) return "订单不存在";
            if (new BLL.department().getGroupArea() != manager.area || !new BLL.permission().checkHasPermission(manager, "0405"))
            {
                return "无权限变更锁单状态";
            }
            if (model.o_lockStatus == lockStatus) return "状态未变更";
            if (lockStatus==1)
            {
                //对订单状态是已取消，无应收付地接记录，接单已完成或为空。上级状态待审或已审批通过，都可以锁单！！
                if (model.o_status == 0)
                {
                    return "订单状态须为已确认或取消时，才能锁单";
                }
                if (model.o_flag == 1)
                {
                    return "业务上级待审批或审批通过时，才能锁单";
                }
                DataTable dt = new BLL.Order().GetPersonList(0, "op_oid='" + oID + "' and (op_type=3 or op_type=5)", "").Tables[0];
                if (dt!=null && dt.Rows.Count > 0)
                {
                    DataRow[] drs = dt.Select("op_dstatus<>2");
                    if (drs.Length > 0)
                    {
                        return "所有策划人员和设计人员的接单状态为已完成才能锁单";
                    }
                }
                DataTable finDT = new BLL.finance().GetList(0, "fin_oid='"+oID+ "' and fin_flag<>2", "").Tables[0];
                if (finDT != null && finDT.Rows.Count > 0)
                {
                    return "存在待审批或审批未通过的应收付时，不能锁单";
                }
            }
            string content = "锁单状态：" + Common.BusinessDict.lockStatus()[Utils.ObjToByte(model.o_lockStatus)] + "→<font color='red'>" + Common.BusinessDict.lockStatus()[lockStatus] + "</font>";
            string replaceUser = Common.BusinessDict.lockStatus()[Utils.ObjToByte(model.o_lockStatus)], replaceUser1 = Common.BusinessDict.lockStatus()[lockStatus];
            model.o_lockStatus = lockStatus;
            if (Update(model))
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_oid = model.o_id;
                logmodel.ol_cid = model.o_cid.Value;
                logmodel.ol_title = "更新锁单状态";
                logmodel.ol_content = content.ToString();
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Audit.ToString(), logmodel, manager.user_name, manager.real_name);

                //给业务员、业务报账人员发消息
                string replaceContent = model.o_id + "," + model.o_content + "," + model.o_address + "," + model.o_sdate.Value.ToString("yyyy-MM-dd");
                IEnumerable<OrderPerson> list = model.personlist.Where(p => p.op_type == 1 || p.op_type == 2);
                foreach (OrderPerson item in list)
                {
                    new BLL.selfMessage().AddMessage("订单锁单状态变更", item.op_number, item.op_name, replaceContent, replaceUser, replaceUser1);
                }

                return "";
            }
            return "锁单失败";
        }
        /// <summary>
        /// 更新财务成本
        /// </summary>
        /// <param name="oID"></param>
        /// <param name="flag"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public string updateCost(string oID, decimal? cost, Model.manager manager)
        {
            Model.Order model = GetModel(oID);
            if (model == null) return "订单不存在";
            if (new BLL.department().getGroupArea() != manager.area || !new BLL.permission().checkHasPermission(manager, "0405"))
            {
                return "无权限变更税费成本";
            }
            if (model.o_lockStatus==1)
            {
                return "订单已经锁单，不能变更税费成本";
            }
            if (model.o_financeCust == cost) return "税费成本未变更";
            string content = "税费成本：" + model.o_financeCust + "→<font color='red'>" + cost + "</font>";
            model.o_financeCust = cost;
            if (Update(model))
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_oid = model.o_id;
                logmodel.ol_cid = model.o_cid.Value;
                logmodel.ol_title = "更新税费成本";
                logmodel.ol_content = content.ToString();
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Edit.ToString(), logmodel, manager.user_name, manager.real_name);
                return "";
            }
            return "更新失败";
        }

        /// <summary>
        /// 编辑财务备注
        /// </summary>
        /// <param name="oID"></param>
        /// <param name="finRemark"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public string updateFinRemark(string oID, string finRemark, Model.manager manager)
        {
            Model.Order model = GetModel(oID);
            if (model == null) return "订单不存在";
            if (!new BLL.permission().checkHasPermission(manager, "0401"))
            {
                return "无权限编辑财务备注";
            }
            if (model.o_finRemarks == finRemark) return "财务备注未变更";
            string content = "财务备注：" + model.o_finRemarks + "→<font color='red'>" + finRemark + "</font>";
            model.o_finRemarks = finRemark;
            if (Update(model))
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_oid = model.o_id;
                logmodel.ol_cid = model.o_cid.Value;
                logmodel.ol_title = "编辑财务备注";
                logmodel.ol_content = content.ToString();
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Edit.ToString(), logmodel, manager.user_name, manager.real_name);
                return "";
            }
            return "更新失败";
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Model.Order model)
        {
            return dal.Update(model);
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string id)
        {
            return dal.Delete(id);
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.Order GetModel(string id)
        {
            return dal.GetModel(id);
        }

        /// <summary>
        /// 获取某个区域已推送未审批的订单数量
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public int getUnAduitOrder(string area)
        {
            return dal.getUnAduitOrder(area);
        }
        /// <summary>
        /// 获取业务支付未审核数量
        /// </summary>
        /// <param name="type"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        public int getUnAduitPay(byte type, string area)
        {
            return dal.getUnAduitPay(type, area,new BLL.department().getGroupArea());
        }
        /// <summary>
        /// 获取非业务支付未审核数量
        /// </summary>
        /// <param name="type"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        public int getUnAduitUnBusinessPay(byte type, string area)
        {
            return dal.getUnAduitUnBusinessPay(type, area, new BLL.department().getGroupArea());
        }
        /// <summary>
        /// 获取发票未审核数量
        /// </summary>
        /// <param name="type"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        public int getUnAduitInvoice(byte type, string area)
        {
            return dal.getUnAduitInvoice(type, area, new BLL.department().getGroupArea());
        }
        /// <summary>
        /// 获取预付款未审核数量
        /// </summary>
        /// <param name="type"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        public int getUnAduitExpectPay(byte type)
        {
            return dal.getUnAduitExpectPay(type);
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
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount, bool type = false, string where = "", bool isPage = true, string orderType = "", string currentUser = "")
        {
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount, type, where, isPage, orderType, currentUser);
        }

        /// <summary>
        /// 计算订单结算汇总
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public DataTable getOrderCollect(string oid)
        {
            if (string.IsNullOrEmpty(oid)) return null;
            return dal.getOrderCollect(oid);
        }

        /// <summary>
        /// 计算存在未审应收付地接的订单数量
        /// </summary>
        /// <returns></returns>
        public int getUnCheckOrderCount(string _edate1, string _status, string _dstatus, string _flag)
        {
            return dal.getUnCheckOrderCount(_edate1, _status, _dstatus, _flag);
        }

        /// <summary>
        /// 计算应收付地接全部审批通过的未锁订单数量
        /// </summary>
        /// <returns></returns>
        public int getUnLockOrderCount()
        {
            return dal.getUnLockOrderCount();
        }
        /// <summary>
        /// 计算应收付地接全部审批通过的待处理订单数量
        /// </summary>
        /// <returns></returns>
        public int getUnDealOrderCount()
        {
            return dal.getUnDealOrderCount();
        }

        #endregion

        #region 扩展方法===

        #endregion

        #region 订单文件相关===
        /// <summary>
        /// 获取订单活动文件
        /// </summary>
        public DataSet GetFileList(int Top, string strWhere, string filedOrder)
        {
            return dal.GetFileList(Top, strWhere, filedOrder);
        }

        /// <summary>
        /// 添加订单活动文件
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public int insertOrderFile(Model.Files file, Model.manager manager)
        {
            int ret = dal.insertOrderFile(file);
            if (ret > 0)
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_oid = file.f_oid;
                logmodel.ol_relateID = ret;
                logmodel.ol_title = "添加订单活动文件";
                logmodel.ol_content = "文件名：" + file.f_fileName + "，文件路径：" + file.f_filePath;
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Add.ToString(), logmodel, manager.user_name, manager.real_name);
            }
            return ret;
        }
        /// <summary>
        /// 删除订单活动文件
        /// </summary>
        /// <param name="file"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public bool deleteOrderFile(Model.Files file, Model.manager manager)
        {
            if (dal.deleteOrderFile(file.f_id.Value))
            {
                string path = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "/" + file.f_filePath;
                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_oid = file.f_oid;
                logmodel.ol_relateID = file.f_id.Value;
                logmodel.ol_title = "删除订单活动文件";
                logmodel.ol_content = "文件名：" + file.f_fileName + "，文件路径：" + file.f_filePath;
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Add.ToString(), logmodel, manager.user_name, manager.real_name);
                return true;
            }
            return false;
        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.Files GetFileModel(int id)
        {
            return dal.GetFileModel(id);
        }
        #endregion

        #region 订单人员相关===
        /// <summary>
        /// 获取订单人员
        /// </summary>
        public DataSet GetPersonList(int Top, string strWhere, string filedOrder)
        {
            return dal.GetPersonList(Top, strWhere, filedOrder);
        }
        #endregion
    }
}