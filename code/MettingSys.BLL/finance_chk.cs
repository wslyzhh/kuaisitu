using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MettingSys.BLL
{
    public class finance_chk
    {
        private readonly DAL.finance_chk dal;
        public finance_chk()
        {
            dal = new DAL.finance_chk();
        }

        public Model.finance_chk GetModel(int id)
        {
            return dal.GetModel(id);
        }
        public bool Update(Model.finance_chk model)
        {
            return dal.Update(model);
        }
        /// <summary>
        /// 对账
        /// </summary>
        /// <param name="fin_id"></param>
        /// <param name="oid">订单号</param>
        /// <param name="num">对账标识</param>
        /// <param name="money">对账金额</param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public string addFinancechk(int? fin_id,string oid, string num, decimal? money, Model.manager manager,int fcid=0,bool flag = false)
        {
            if (!new BLL.permission().checkHasPermission(manager, "0406"))
            {
                return "无权限操作";
            }
            Model.finance_chk model = new Model.finance_chk();
            StringBuilder content = new StringBuilder();
            if (fcid > 0)
            {
                model = GetModel(fcid);
                if (model.fc_num != num)
                {
                    content.Append("对账标识：" + model.fc_num + "→<font color='red'>" + num + "</font><br/>");
                }
                model.fc_num = num;
                if (model.fc_money != money)
                {
                    content.Append("对账金额：" + model.fc_money + "→<font color='red'>" + money + "</font><br/>");
                }
                model.fc_money = money;
            }
            else
            {
                model.fc_finid = fin_id;
                model.fc_oid = oid;
                model.fc_num = num;
                model.fc_money = money;
                model.fc_number = manager.user_name;
                model.fc_name = manager.real_name;
                model.fc_addDate = DateTime.Now;
                content.Append("对账标识：" + num + "<br/>");
                content.Append("对账金额：" + money + "<br/>");
            }
            if (flag)
            {
                dal.DeleteByFinid(fin_id.Value);
            }
            bool result = fcid > 0 ? Update(model) : dal.Add(model) > 0;
            if (result)
            {

                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = fin_id.Value;
                logmodel.ol_oid = oid;
                logmodel.ol_title = "添加对账";
                logmodel.ol_content = content.ToString();
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Add.ToString(), logmodel, manager.user_name, manager.real_name); //记录日志
                return "";
            }
            return "对账失败";
        }

        /// <summary>
        /// 修改对账标识
        /// </summary>
        /// <param name="fc_id"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public string updateChk(int fcid, string num, Model.manager manager)
        {
            if (!new BLL.permission().checkHasPermission(manager, "0406"))
            {
                return "无权限操作";
            }
            if (string.IsNullOrEmpty(num))
            {
                return "对账标识不能为空";
            }
            DataSet ds = GetList(0, "fc_id = " + fcid + "", "fc_id");
            StringBuilder content = new StringBuilder();            
            if (ds==null || ds.Tables[0].Rows.Count == 0)
            {
                return "找不到数据";
            }
            DataRow dr = ds.Tables[0].Rows[0];
            if (Utils.ObjectToStr(dr["fc_num"]) == num)
            {
                return "对账标识未改变";
            }
            content.Append("对账标识：" + Utils.ObjectToStr(dr["fc_num"]) + "→<font color='red'>" + num + "</font><br/>");
            if (new BLL.ReceiptPayDetail().Exists(Utils.ObjectToStr(dr["fc_oid"]), Utils.ObjectToStr(dr["fc_num"]), Utils.ObjToInt(dr["fin_cid"],0))) {
                return "存在已分配款，不能修改对账标识";
            }
            Model.finance_chk model = new BLL.finance_chk().GetModel(fcid);
            model.fc_num = num;
            if (Update(model))
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = fcid;
                logmodel.ol_oid = model.fc_oid;
                logmodel.ol_title = "修改对账标识";
                logmodel.ol_content = content.ToString();
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Add.ToString(), logmodel, manager.user_name, manager.real_name); //记录日志
                return "";
            }
            return "修改失败";
        }
        /// <summary>
        /// 删除对账
        /// </summary>
        /// <param name="fcid"></param>
        /// <param name="manager"></param>
        /// <returns></returns>
        public string delFinancechk(int fcid, Model.manager manager)
        {
            if (!new BLL.permission().checkHasPermission(manager, "0406"))
            {
                return "无权限操作";
            }
            Model.finance_chk model = dal.GetModel(fcid);
            if (dal.Delete(fcid))
            {
                StringBuilder content = new StringBuilder();
                content.Append("对账标识：" + model.fc_num + "<br/>");
                content.Append("对账金额：" + model.fc_money + "<br/>");

                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = model.fc_finid.Value;
                logmodel.ol_oid = model.fc_oid;
                logmodel.ol_title = "删除对账";
                logmodel.ol_content = content.ToString();
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Delete.ToString(), logmodel, manager.user_name, manager.real_name); //记录日志
                return "";
            }
            return "删除失败";
        }

        /// <summary>
        /// 删除某条地接下的所有对账
        /// </summary>
        /// <param name="finid"></param>
        /// <returns></returns>
        public string delChkByFinID(int finid,string oid, Model.manager manager)
        {
            if (dal.DeleteByFinid(finid))
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = Utils.ObjToInt(finid,0);
                logmodel.ol_oid = oid;
                logmodel.ol_title = "删除地接下的对账信息";
                logmodel.ol_content = "地接ID" + finid;
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Delete.ToString(), logmodel, manager.user_name, manager.real_name); //记录日志
                return "";
            }
            return "删除失败";
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
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount, out decimal tMoney, bool isPage = true)
        {
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount,out tMoney,isPage);
        }
    }
}
