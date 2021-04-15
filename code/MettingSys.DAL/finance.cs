using MettingSys.Common;
using MettingSys.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MettingSys.DAL
{
    public partial class finance
    {
        public finance() { }
        #region 基本方法================================
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from  MS_finance");
            strSql.Append(" where ");
            strSql.Append(" fin_id = @id  ");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.Int,4)};
            parameters[0].Value = id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(Model.finance model)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();//数据字段
            StringBuilder str2 = new StringBuilder();//数据参数
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<SqlParameter> paras = new List<SqlParameter>();
            strSql.Append("insert into MS_finance(");
            foreach (PropertyInfo pi in pros)
            {
                //如果不是主键则追加sql字符串
                if (!pi.Name.Equals("fin_id"))
                {
                    //判断属性值是否为空
                    if (pi.GetValue(model, null) != null)
                    {
                        str1.Append(pi.Name + ",");//拼接字段
                        str2.Append("@" + pi.Name + ",");//声明参数
                        paras.Add(new SqlParameter("@" + pi.Name, pi.GetValue(model, null)));//对参数赋值
                    }
                }
            }
            strSql.Append(str1.ToString().Trim(','));
            strSql.Append(") values (");
            strSql.Append(str2.ToString().Trim(','));
            strSql.Append(") ");
            strSql.Append(";select @@IDENTITY;");
            object obj = DbHelperSQL.GetSingle(strSql.ToString(), paras.ToArray());
            if (obj == null)
            {
                return 0;
            }
            else
            {
                return Convert.ToInt32(obj);
            }
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public bool Update(Model.finance model)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<SqlParameter> paras = new List<SqlParameter>();
            strSql.Append("update MS_finance set ");
            foreach (PropertyInfo pi in pros)
            {
                //如果不是主键则追加sql字符串
                if (!pi.Name.Equals("fin_id"))
                {
                    //判断属性值是否为空
                    if (pi.GetValue(model, null) != null)
                    {
                        str1.Append(pi.Name + "=@" + pi.Name + ",");//声明参数
                        paras.Add(new SqlParameter("@" + pi.Name, pi.GetValue(model, null)));//对参数赋值
                    }
                }
            }
            strSql.Append(str1.ToString().Trim(','));
            strSql.Append(" where fin_id=@id ");
            paras.Add(new SqlParameter("@id", model.fin_id));
            return DbHelperSQL.ExecuteSql(strSql.ToString(), paras.ToArray()) > 0;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from  MS_finance ");
            strSql.Append(" where fin_id=@id");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.Int,4)};
            parameters[0].Value = id;

            return DbHelperSQL.ExecuteSql(strSql.ToString(), parameters) > 0;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.finance GetModel(int id)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.finance model = new Model.finance();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                str1.Append(p.Name + ",");//拼接字段
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from MS_finance");
            strSql.Append(" where fin_id=@id");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.Int,4)};
            parameters[0].Value = id;
            DataTable dt = DbHelperSQL.Query(strSql.ToString(), parameters).Tables[0];

            if (dt.Rows.Count > 0)
            {
                return DataRowToModel(dt.Rows[0]);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetList(int Top, string strWhere, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (Top > 0)
            {
                strSql.Append(" top " + Top.ToString());
            }
            strSql.Append(" *,chk = isnull(STUFF((SELECT ',' + fc_num FROM MS_finance_chk WHERE  fc_finid=fin_id FOR XML PATH('')), 1, 1, ''),'无'),chkMoney=(select isnull(sum(isnull(fc_money,0)),0) fc_money from MS_finance_chk where fc_finid=fin_id)");
            strSql.Append(" FROM  MS_finance f left join MS_Order on fin_oid=o_id left join MS_customer c on fin_cid=c_id left join MS_Nature na on fin_nature=na_id");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            if (filedOrder.Trim() != "")
            {
                strSql.Append(" order by " + filedOrder);
            }
            return DbHelperSQL.Query(strSql.ToString());
        }

        /// <summary>
        /// 获得前几行数据
        /// </summary>
        public DataSet GetList(int Top, string strWhere,string chk, string filedOrder)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select ");
            if (Top > 0)
            {
                strSql.Append(" top " + Top.ToString());
            }
            string sqlwhere = "";
            if (!string.IsNullOrEmpty(chk))
            {
                if (chk == "0")
                {
                    sqlwhere += " and isnull(fc_num,'')=''";
                }
                else if (chk == "1")
                {
                    sqlwhere += " and isnull(fc_num,'')<>''";
                }
                else
                {
                    sqlwhere += " and fc_num='" + chk + "'";
                }
            }
            strSql.Append(" *,chk = isnull(STUFF((SELECT ',' + fc_num FROM MS_finance_chk WHERE  fc_finid=fin_id "+ sqlwhere + " FOR XML PATH('')), 1, 1, ''),'无'),chkMoney=(select isnull(sum(isnull(fc_money,0)),0) fc_money from MS_finance_chk where fc_finid=fin_id " + sqlwhere + ")");
            strSql.Append(" FROM  MS_finance f left join MS_Order on fin_oid=o_id left join MS_customer c on fin_cid=c_id left join MS_Nature na on fin_nature=na_id");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            if (filedOrder.Trim() != "")
            {
                strSql.Append(" order by " + filedOrder);
            }
            return DbHelperSQL.Query(strSql.ToString());
        }
        /// <summary>
        /// 返回订单中所有的业务性质
        /// </summary>
        /// <param name="oid"></param>
        /// <returns></returns>
        public DataTable getNature(string oid,string username="")
        {
            string sql = "select fin_oid,fin_nature,na_name,na_sort,sum(case when fin_type=1 then fin_money else 0-fin_money end) profit from MS_finance left join MS_nature on fin_nature=na_id where fin_oid='" + oid + "'";
            if (!string.IsNullOrEmpty(username))
            {
                sql += " and fin_personNum='" + username + "'";
            }
            sql += " group by fin_oid,fin_nature,na_name,na_sort order by na_sort";
            return DbHelperSQL.Query(sql).Tables[0];
        }
        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount,bool isPage=true)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select f.*,c.c_name,na.na_name,o.o_status,o.o_sdate,o.o_edate,chk = (STUFF((SELECT ',' + fc_num FROM MS_finance_chk WHERE  fc_finid=f.fin_id FOR XML PATH('')), 1, 1, '')) FROM MS_finance f left join MS_customer c on fin_cid=c_id left join MS_Nature na on fin_nature=na_id left join MS_Order o on fin_oid = o_id left join MS_OrderPerson on o_id=op_oid and op_type=1");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            recordCount = 0;
            if (isPage)
            {
                recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
                return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
            }
            else
            {
                return DbHelperSQL.Query(strSql.ToString());
            }
        }
        /// <summary>
        /// 审批未通过地接
        /// </summary>
        public DataSet GetList1(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount, bool isPage = true)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select f.*,c.c_name,na.na_name,o_status,chk = (STUFF((SELECT ',' + fc_num FROM MS_finance_chk WHERE  fc_finid=f.fin_id FOR XML PATH('')), 1, 1, '')) FROM MS_finance f left join MS_customer c on fin_cid=c_id left join MS_Nature na on fin_nature=na_id left join MS_Order on fin_oid=o_id left join MS_OrderPerson on fin_oid=op_oid and op_type=1");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where 1=1" + strWhere);
            }
            recordCount = 0;
            if (isPage)
            {
                recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
                return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
            }
            else
            {
                return DbHelperSQL.Query(strSql.ToString());
            }
        }
        /// <summary>
        /// 审单列表
        /// </summary>
        public DataSet GetApprovalList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount,bool isPage=true)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select *,(select count(*) from MS_finance where fin_oid=o_id and fin_flag=0) count0,(select count(*) from MS_finance where fin_oid=o_id and fin_flag=1) count1 FROM MS_Order left join ms_customer on o_cid=c_id left join ms_contacts on o_coid=co_id left join ms_orderperson on o_id=op_oid and op_type=1");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            recordCount = 0;
            if (isPage)
            {
                recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
                return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
            }
            else
            {
                return DbHelperSQL.Query(strSql.ToString());
            }
        }

        /// <summary>
        /// 往来客户总体统计列表
        /// </summary>
        /// <returns></returns>
        public DataSet getSettleCustomerList(string _sdate,string _edate,string _sdate1,string _edate1,string _sdate2,string _edate2,string _status,string _lockstatus,string _area,string _person1)
        {
            StringBuilder strTemp = new StringBuilder();
            StringBuilder strTemp1 = new StringBuilder();
            if (!string.IsNullOrEmpty(_sdate))
            {
                strTemp.Append(" and datediff(day,o_sdate,'" + _sdate + "')<=0");
            }
            if (!string.IsNullOrEmpty(_edate))
            {
                strTemp.Append(" and datediff(day,o_sdate,'" + _edate + "')>=0");
            }
            if (!string.IsNullOrEmpty(_sdate1))
            {
                strTemp.Append(" and datediff(day,o_edate,'" + _sdate1 + "')<=0");
            }
            if (!string.IsNullOrEmpty(_edate1))
            {
                strTemp.Append(" and datediff(day,o_edate,'" + _edate1 + "')>=0");
            }
            if (!string.IsNullOrEmpty(_status))
            {
                switch (_status)
                {
                    case "3":
                        strTemp.Append(" and (o_status=1 or o_status=2)");
                        break;
                    default:
                        strTemp.Append(" and o_status=" + _status + "");
                        break;
                }
            }
            if (!string.IsNullOrEmpty(_lockstatus))
            {
                if (_lockstatus == "3")
                {
                    strTemp.Append(" and (o_lockStatus=0 or o_lockStatus=2)");
                }
                else
                {
                    strTemp.Append(" and o_lockStatus=" + _lockstatus + "");
                }
            }
            if (!string.IsNullOrEmpty(_area))
            {
                strTemp.Append(" and op_area='" + _area + "'");
            }
            if (!string.IsNullOrEmpty(_person1))
            {
                strTemp.Append(" and (op_number like '%" + _person1 + "%' or op_name like '%" + _person1 + "%')");
            }
            if (!string.IsNullOrEmpty(_sdate2))
            {
                strTemp1.Append(" and datediff(day,rp_date,'" + _sdate2 + "')<=0");
            }
            if (!string.IsNullOrEmpty(_edate2))
            {
                strTemp1.Append(" and datediff(day,rp_date,'" + _edate2 + "')>=0");
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select isnull(fin_type,rp_type) fin_type,isnull(orderFinMoney,0) orderFinMoney,isnull(orderRpdMoney,0) orderRpdMoney,isnull(orderUnMoney,0) orderUnMoney,isnull(rpmoney,0) rpmoney,isnull(rpdmoney,0) rpdmoney,isnull(unmoney,0) unmoney from (");
            strSql.Append(" select fin_type, sum(finMoney) orderFinMoney, sum(rpdMoney) orderRpdMoney, (sum(finMoney) - sum(rpdMoney)) orderUnMoney  from(");
            strSql.Append(" select isnull(fin_oid, rpd_oid) fin_oid, isnull(fin_type, rpd_type) fin_type, isnull(finMoney, 0) finMoney, isnull(rpdMoney, 0) rpdMoney from");
            strSql.Append(" (select fin_oid, fin_type, sum(isnull(fin_money, 0)) finMoney from MS_finance where fin_flag <> 1 group by fin_oid, fin_type) t1");
            strSql.Append(" full join(select rpd_oid, rpd_type, sum(isnull(rpd_money, 0)) rpdMoney from MS_ReceiptPayDetail left join MS_ReceiptPay on rp_id = rpd_rpid where rp_isConfirm = 1 group by rpd_oid, rpd_type) t3 on t1.fin_oid = t3.rpd_oid and t1.fin_type = t3.rpd_type");
            strSql.Append(" left join MS_Order on isnull(fin_oid, rpd_oid) = o_id left join MS_OrderPerson on o_id=op_oid and op_type=1 where 1=1 " + strTemp + "");
            strSql.Append(" ) t group by fin_type) t2 full join(");
            strSql.Append(" select rp_type, sum(rp_money) rpmoney, sum(isnull(rpd_money, 0)) rpdmoney, (sum(rp_money) - sum(isnull(rpd_money, 0))) unmoney  from(");
            strSql.Append(" select * from MS_ReceiptPay left join (select rpd_rpid, sum(rpd_money) rpd_money  from MS_ReceiptPayDetail group by rpd_rpid) r1 on rp_id = r1.rpd_rpid where rp_isConfirm = 1 "+ strTemp1 + "");
            strSql.Append(" ) r2 group by rp_type) r3 on  t2.fin_type = r3.rp_type");
            strSql.Append(" where 1 = 1  order by isnull(fin_type,rp_type) desc");
            SqlParameter[] param = { };
            return DbHelperSQL.Query(strSql.ToString(), param);

        }

        /// <summary>
        /// 往来客户明细列表(按应收付对象分组)
        /// </summary>
        /// <returns></returns>
        public DataSet getSettleCustomerDetailList(int pageSize, int pageIndex, string _type, string _cid,string _cname, string _sdate, string _edate, string _sdate1, string _edate1, string _sdate2, string _edate2, string _status, string _sign, string _money1, string username, string _lockstatus, string _area, string _person1, string filedOrder, out int recordCount,out decimal money1, out decimal money2, out decimal money3, out decimal money4, out decimal money5, out decimal money6, bool isPage = true)
        {
            StringBuilder strTemp = new StringBuilder();
            StringBuilder strTemp1 = new StringBuilder();
            StringBuilder strTemp2 = new StringBuilder();
            string joinStr = " full join ";
            if (!string.IsNullOrEmpty(_type))
            {
                strTemp.Append(" and isnull(fin_type,rp_type)='" + _type + "'");
            }
            if (!string.IsNullOrEmpty(_cname))
            {
                strTemp.Append(" and c_name like '%" + _cname + "%'");
            }
            if (!string.IsNullOrEmpty(_cid))
            {
                strTemp.Append(" and isnull(fin_cid,rp_cid)=" + _cid + "");
            }
            if (!string.IsNullOrEmpty(_sdate))
            {
                joinStr = " left join ";
                strTemp1.Append(" and datediff(day,o_sdate,'" + _sdate + "')<=0");
            }
            if (!string.IsNullOrEmpty(_edate))
            {
                joinStr = " left join ";
                strTemp1.Append(" and datediff(day,o_sdate,'" + _edate + "')>=0");
            }
            if (!string.IsNullOrEmpty(_sdate1))
            {
                joinStr = " left join ";
                strTemp1.Append(" and datediff(day,o_edate,'" + _sdate1 + "')<=0");
            }
            if (!string.IsNullOrEmpty(_edate1))
            {
                joinStr = " left join ";
                strTemp1.Append(" and datediff(day,o_edate,'" + _edate1 + "')>=0");
            }
            if (!string.IsNullOrEmpty(_status))
            {
                joinStr = " left join ";
                switch (_status)
                {
                    case "3":
                        strTemp1.Append(" and (o_status=1 or o_status=2)");
                        break;
                    default:
                        strTemp1.Append(" and o_status=" + _status + "");
                        break;
                }
            }
            if (!string.IsNullOrEmpty(_lockstatus))
            {
                if (_lockstatus == "3")
                {
                    strTemp1.Append(" and (o_lockStatus=0 or o_lockStatus=2)");
                }
                else
                {
                    strTemp1.Append(" and o_lockStatus=" + _lockstatus + "");
                }
            }
            if (!string.IsNullOrEmpty(_area))
            {
                strTemp1.Append(" and op_area='" + _area + "'");
            }
            if (!string.IsNullOrEmpty(_person1))
            {
                strTemp1.Append(" and (op_number like '%" + _person1 + "%' or op_name like '%" + _person1 + "%')");
            }
            if (!string.IsNullOrEmpty(_sdate2))
            {
                strTemp2.Append(" and datediff(day,rp_date,'" + _sdate2 + "')<=0");
            }
            if (!string.IsNullOrEmpty(_edate2))
            {
                strTemp2.Append(" and datediff(day,rp_date,'" + _edate2 + "')>=0");
            }
            if (!string.IsNullOrEmpty(_money1))
            {
                joinStr = " left join ";
                strTemp1.Append(" and isnull(finMoney,0)-isnull(rpdMoney,0) " + _sign + " " + _money1 + "");
            }
            if (!string.IsNullOrEmpty(username))
            {
                strTemp1.Append(" and op_number='" + username + "'");
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  c_name,isnull(fin_cid,rp_cid) fin_cid,isnull(fin_type,rp_type) fin_type,isnull(orderFinMoney,0) orderFinMoney,isnull(orderRpdMoney,0) orderRpdMoney,isnull(orderUnMoney,0) orderUnMoney,isnull(rpmoney,0) rpmoney,isnull(rpdmoney,0) rpdmoney,isnull(unmoney,0) unmoney from (");
            strSql.Append(" select fin_cid, fin_type, sum(finMoney) orderFinMoney, sum(rpdMoney) orderRpdMoney, (sum(finMoney) - sum(rpdMoney)) orderUnMoney  from(");
            strSql.Append(" select isnull(fin_oid, rpd_oid) fin_oid, isnull(fin_cid, rpd_cid) fin_cid, isnull(fin_type, rpd_type) fin_type, isnull(finMoney, 0) finMoney, isnull(rpdMoney, 0) rpdMoney from");
            strSql.Append(" (select fin_oid, fin_cid, fin_type, sum(isnull(fin_money, 0)) finMoney from MS_finance where fin_flag <> 1 group by fin_oid, fin_cid, fin_type) t1");
            strSql.Append(" full join(select rpd_oid, rpd_cid, rpd_type, sum(isnull(rpd_money, 0)) rpdMoney from MS_ReceiptPayDetail left join MS_ReceiptPay on rp_id = rpd_rpid where rp_isConfirm = 1 group by rpd_oid, rpd_cid, rpd_type) t3 on t1.fin_oid = t3.rpd_oid and t1.fin_cid = t3.rpd_cid and t1.fin_type = t3.rpd_type");
            strSql.Append(" left join MS_Order on isnull(fin_oid, rpd_oid) = o_id left join MS_OrderPerson on o_id=op_oid and op_type=1 where 1=1 " + strTemp1 + "");
            strSql.Append(" ) t group by fin_cid, fin_type) t2 " + joinStr + " (");
            strSql.Append(" select rp_cid, rp_type, sum(rp_money) rpmoney, sum(isnull(rpd_money, 0)) rpdmoney, (sum(rp_money) - sum(isnull(rpd_money, 0))) unmoney  from(");
            strSql.Append(" select * from MS_ReceiptPay left join (select rpd_rpid, sum(rpd_money) rpd_money  from MS_ReceiptPayDetail group by rpd_rpid) r1 on rp_id = r1.rpd_rpid where rp_isConfirm = 1 " + strTemp2 + "");
            strSql.Append(" ) r2 group by rp_cid, rp_type) r3 on t2.fin_cid = r3.rp_cid and t2.fin_type = r3.rp_type left join MS_Customer on isnull(fin_cid, rp_cid) = c_id");
            strSql.Append("  where 1 = 1 " + strTemp + "");
            SqlParameter[] param = { };
            recordCount = 0;money1 = 0;money2 = 0;money3 = 0;money4 = 0;money5 = 0;money6 = 0;
            if (isPage)
            {
                //recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
                DataTable dt = DbHelperSQL.Query("select count(*) c,sum(orderFinMoney) orderFinMoney,sum(orderRpdMoney) orderRpdMoney,sum(orderUnMoney) orderUnMoney,sum(rpmoney) rpmoney,sum(rpdmoney) rpdmoney,sum(unmoney) unmoney from (" + strSql.ToString() + ") v1").Tables[0];
                if (dt != null)
                {
                    recordCount= Utils.ObjToInt(dt.Rows[0]["c"], 0);
                    money1 = Utils.ObjToDecimal(dt.Rows[0]["orderFinMoney"], 0);
                    money2 = Utils.ObjToDecimal(dt.Rows[0]["orderRpdMoney"], 0);
                    money3 = Utils.ObjToDecimal(dt.Rows[0]["orderUnMoney"], 0);
                    money4 = Utils.ObjToDecimal(dt.Rows[0]["rpmoney"], 0);
                    money5 = Utils.ObjToDecimal(dt.Rows[0]["rpdmoney"], 0);
                    money6 = Utils.ObjToDecimal(dt.Rows[0]["unmoney"], 0);
                }
                return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
            }
            else
            {
                return DbHelperSQL.Query(strSql.ToString() + " order by " + filedOrder);
            }

        }
        /// <summary>
        /// 往来客户明细列表(按业务员分组)
        /// </summary>
        /// <returns></returns>
        public DataSet getSettleCustomerDetailListByUser(int pageSize, int pageIndex, string _type, string _cid, string _cname, string _sdate, string _edate, string _sdate1, string _edate1, string _sdate2, string _edate2, string _status, string _sign, string _money1, string username, string _lockstatus, string _area, string _person1, string filedOrder, out int recordCount, out decimal money1, out decimal money2, out decimal money3, bool isPage = true)
        {
            StringBuilder strTemp = new StringBuilder();
            StringBuilder strTemp1 = new StringBuilder();
            StringBuilder strTemp2 = new StringBuilder();
            if (!string.IsNullOrEmpty(_type))
            {
                strTemp.Append(" and fin_type='" + _type + "'");
            }
            if (!string.IsNullOrEmpty(_cname))
            {
                strTemp1.Append(" and c_name like '%" + _cname + "%'");
            }
            if (!string.IsNullOrEmpty(_cid))
            {
                strTemp1.Append(" and isnull(fin_cid,rp_cid)=" + _cid + "");
            }
            if (!string.IsNullOrEmpty(_sdate))
            {
                strTemp1.Append(" and datediff(day,o_sdate,'" + _sdate + "')<=0");
            }
            if (!string.IsNullOrEmpty(_edate))
            {
                strTemp1.Append(" and datediff(day,o_sdate,'" + _edate + "')>=0");
            }
            if (!string.IsNullOrEmpty(_sdate1))
            {
                strTemp1.Append(" and datediff(day,o_edate,'" + _sdate1 + "')<=0");
            }
            if (!string.IsNullOrEmpty(_edate1))
            {
                strTemp1.Append(" and datediff(day,o_edate,'" + _edate1 + "')>=0");
            }
            if (!string.IsNullOrEmpty(_status))
            {
                switch (_status)
                {
                    case "3":
                        strTemp1.Append(" and (o_status=1 or o_status=2)");
                        break;
                    default:
                        strTemp1.Append(" and o_status=" + _status + "");
                        break;
                }
            }
            if (!string.IsNullOrEmpty(_lockstatus))
            {
                if (_lockstatus == "3")
                {
                    strTemp1.Append(" and (o_lockStatus=0 or o_lockStatus=2)");
                }
                else
                {
                    strTemp1.Append(" and o_lockStatus=" + _lockstatus + "");
                }
            }
            if (!string.IsNullOrEmpty(_area))
            {
                strTemp1.Append(" and op_area='" + _area + "'");
            }
            if (!string.IsNullOrEmpty(_person1))
            {
                strTemp.Append(" and (op_number like '%" + _person1 + "%' or op_name like '%" + _person1 + "%')");
            }
            //if (!string.IsNullOrEmpty(_sdate2))
            //{
            //    strTemp2.Append(" and datediff(day,rp_date,'" + _sdate2 + "')<=0");
            //}
            //if (!string.IsNullOrEmpty(_edate2))
            //{
            //    strTemp2.Append(" and datediff(day,rp_date,'" + _edate2 + "')>=0");
            //}
            if (!string.IsNullOrEmpty(_money1))
            {
                strTemp1.Append(" and isnull(finMoney,0)-isnull(rpdMoney,0) " + _sign + " " + _money1 + "");
            }
            if (!string.IsNullOrEmpty(username))
            {
                strTemp1.Append(" and op_number='" + username + "'");
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  op_number,op_name,fin_type,isnull(orderFinMoney,0) orderFinMoney,isnull(orderRpdMoney,0) orderRpdMoney,isnull(orderUnMoney,0) orderUnMoney from ");
            strSql.Append(" (select op_number,op_name, fin_type, sum(finMoney) orderFinMoney, sum(rpdMoney) orderRpdMoney, (sum(finMoney) - sum(rpdMoney)) orderUnMoney  from");
            strSql.Append(" (select isnull(fin_oid, rpd_oid) fin_oid,op_number,op_name, isnull(fin_cid, rpd_cid) fin_cid,c_name, isnull(fin_type, rpd_type) fin_type, isnull(finMoney, 0) finMoney, isnull(rpdMoney, 0) rpdMoney from ");
            strSql.Append(" (select fin_oid, fin_cid, fin_type, sum(isnull(fin_money, 0)) finMoney from MS_finance where fin_flag <> 1 group by fin_oid, fin_cid, fin_type) t1 ");
            strSql.Append(" full join(select rpd_oid, rpd_cid, rpd_type, sum(isnull(rpd_money, 0)) rpdMoney from MS_ReceiptPayDetail left join MS_ReceiptPay on rp_id = rpd_rpid where rp_isConfirm = 1 group by rpd_oid, rpd_cid, rpd_type) t3 ");
            strSql.Append(" on t1.fin_oid = t3.rpd_oid and t1.fin_cid = t3.rpd_cid and t1.fin_type = t3.rpd_type ");
            strSql.Append(" left join MS_Order on isnull(fin_oid, rpd_oid) = o_id left join MS_OrderPerson on o_id=op_oid and op_type=1 left join MS_Customer on fin_cid=c_id where 1=1  " + strTemp1 + "");
            strSql.Append(" ) t group by op_number,op_name, fin_type) t2  where 1 = 1  "+ strTemp + "");

            SqlParameter[] param = { };
            recordCount = 0; money1 = 0; money2 = 0; money3 = 0;
            if (isPage)
            {
                //recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
                DataTable dt = DbHelperSQL.Query("select count(*) c,sum(orderFinMoney) orderFinMoney,sum(orderRpdMoney) orderRpdMoney,sum(orderUnMoney) orderUnMoney from (" + strSql.ToString() + ") v1").Tables[0];
                if (dt != null)
                {
                    recordCount = Utils.ObjToInt(dt.Rows[0]["c"], 0);
                    money1 = Utils.ObjToDecimal(dt.Rows[0]["orderFinMoney"], 0);
                    money2 = Utils.ObjToDecimal(dt.Rows[0]["orderRpdMoney"], 0);
                    money3 = Utils.ObjToDecimal(dt.Rows[0]["orderUnMoney"], 0);
                }
                return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
            }
            else
            {
                return DbHelperSQL.Query(strSql.ToString() + " order by " + filedOrder);
            }

        }

        /// <summary>
        /// 员工未收款统计(按业务员分组)
        /// </summary>
        /// <returns></returns>
        public DataSet getUnReceiveDetailListByUser(int pageSize, int pageIndex, string _sdate, string _edate, string _sdate1, string _edate1,string _status, string _sign, string _money1, string _lockstatus, string _area, string _person1, string filedOrder, out int recordCount, out decimal money1, out decimal money2, out decimal money3, bool isPage = true)
        {
            StringBuilder strTemp = new StringBuilder();
            StringBuilder strTemp1 = new StringBuilder();
            StringBuilder strTemp2 = new StringBuilder();
            strTemp.Append(" and fin_type=1 ");            
            if (!string.IsNullOrEmpty(_sdate))
            {
                strTemp1.Append(" and datediff(day,o_sdate,'" + _sdate + "')<=0");
            }
            if (!string.IsNullOrEmpty(_edate))
            {
                strTemp1.Append(" and datediff(day,o_sdate,'" + _edate + "')>=0");
            }
            if (!string.IsNullOrEmpty(_sdate1))
            {
                strTemp1.Append(" and datediff(day,o_edate,'" + _sdate1 + "')<=0");
            }
            if (!string.IsNullOrEmpty(_edate1))
            {
                strTemp1.Append(" and datediff(day,o_edate,'" + _edate1 + "')>=0");
            }
            if (!string.IsNullOrEmpty(_status))
            {
                switch (_status)
                {
                    case "3":
                        strTemp1.Append(" and (o_status=1 or o_status=2)");
                        break;
                    default:
                        strTemp1.Append(" and o_status=" + _status + "");
                        break;
                }
            }
            if (!string.IsNullOrEmpty(_lockstatus))
            {
                if (_lockstatus == "3")
                {
                    strTemp1.Append(" and (o_lockStatus=0 or o_lockStatus=2)");
                }
                else
                {
                    strTemp1.Append(" and o_lockStatus=" + _lockstatus + "");
                }
            }
            if (!string.IsNullOrEmpty(_area))
            {
                strTemp1.Append(" and op_area='" + _area + "'");
            }
            if (!string.IsNullOrEmpty(_person1))
            {
                strTemp.Append(" and (op_number like '%" + _person1 + "%' or op_name like '%" + _person1 + "%')");
            }
            if (!string.IsNullOrEmpty(_money1))
            {
                strTemp1.Append(" and isnull(finMoney, 0)-isnull(rpdMoney, 0) " + _sign + " " + _money1 + "");
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select  op_number,op_name,fin_type,isnull(orderFinMoney,0) orderFinMoney,isnull(orderRpdMoney,0) orderRpdMoney,isnull(orderUnMoney,0) orderUnMoney from ");
            strSql.Append(" (select op_number,op_name, fin_type, sum(finMoney) orderFinMoney, sum(rpdMoney) orderRpdMoney, (sum(finMoney) - sum(rpdMoney)) orderUnMoney  from");
            strSql.Append(" (select isnull(fin_oid, rpd_oid) fin_oid,op_number,op_name, isnull(fin_type, rpd_type) fin_type, isnull(finMoney, 0) finMoney, isnull(rpdMoney, 0) rpdMoney from ");
            strSql.Append(" (select fin_oid, fin_type, sum(isnull(fin_money, 0)) finMoney from MS_finance where fin_flag <> 1 group by fin_oid, fin_type) t1 ");
            strSql.Append(" full join(select rpd_oid, rpd_type, sum(isnull(rpd_money, 0)) rpdMoney from MS_ReceiptPayDetail left join MS_ReceiptPay on rp_id = rpd_rpid where rp_isConfirm = 1 group by rpd_oid, rpd_type) t3 ");
            strSql.Append(" on t1.fin_oid = t3.rpd_oid and t1.fin_type = t3.rpd_type ");
            strSql.Append(" left join MS_Order on isnull(fin_oid, rpd_oid) = o_id left join MS_OrderPerson on o_id=op_oid and op_type=1 where 1=1  " + strTemp1 + "");
            strSql.Append(" ) t group by op_number,op_name, fin_type) t2  where 1 = 1  " + strTemp + "");

            SqlParameter[] param = { };
            recordCount = 0; money1 = 0; money2 = 0; money3 = 0;
            if (isPage)
            {
                //recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
                DataTable dt = DbHelperSQL.Query("select count(*) c,sum(orderFinMoney) orderFinMoney,sum(orderRpdMoney) orderRpdMoney,sum(orderUnMoney) orderUnMoney from (" + strSql.ToString() + ") v1").Tables[0];
                if (dt != null)
                {
                    recordCount = Utils.ObjToInt(dt.Rows[0]["c"], 0);
                    money1 = Utils.ObjToDecimal(dt.Rows[0]["orderFinMoney"], 0);
                    money2 = Utils.ObjToDecimal(dt.Rows[0]["orderRpdMoney"], 0);
                    money3 = Utils.ObjToDecimal(dt.Rows[0]["orderUnMoney"], 0);
                }
                return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
            }
            else
            {
                return DbHelperSQL.Query(strSql.ToString() + " order by " + filedOrder);
            }

        }

        /// <summary>
        /// 客户对账明细
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="filedOrder"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public DataSet getReconciliationDetail(Dictionary<string,string> dict,int pageSize, int pageIndex, string filedOrder, out int recordCount,out decimal totalFin,out decimal totalRpd,out decimal totalUnMoney,out decimal totalChk,out decimal totalunChk,bool isPage=true)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder strWhere1 = new StringBuilder();
            StringBuilder strWhere2 = new StringBuilder();
            StringBuilder strWhere3 = new StringBuilder();
            string selectFiled = "o_id,o_status,o_content,o_sdate,o_edate,o_address,c_name,co_name,isnull(t1.fin_money,0) fin_money,isnull(rpd_money,0) rpd_money,(isnull(t1.fin_money,0)-isnull(rpd_money,0)) unReceiptPay,isnull(tfcMoney,0) tfcMoney";
            string addTable = "", chkFiled1 = "", chkFiled2 = "", chkFiled3 = "", chkFiled4 = "", chkGroup = "";
            if (dict!=null)
            {
                if (dict.ContainsKey("type")) {
                    strWhere1.Append(" and fin_type='" + dict["type"] + "'");
                    strWhere2.Append(" and rpd_type='" + dict["type"] + "'");
                }
                if (dict.ContainsKey("cid") && dict["cid"]!="0")
                {
                    strWhere1.Append(" and fin_cid=" + dict["cid"] + "");
                    strWhere2.Append(" and rp_cid=" + dict["cid"] + "");
                    strWhere3.Append(" and (exists(select * from MS_finance f where f.fin_oid=o_id and fin_type='" + dict["type"] + "' and fin_cid=" + dict["cid"] + ") or exists(select * from MS_ReceiptPay left join MS_ReceiptPayDetail on rp_id=rpd_rpid where rpd_oid=o_id and rpd_type='" + dict["type"] + "' and rpd_cid=" + dict["cid"] + "))");
                }
                if (dict.ContainsKey("money1"))
                {
                    strWhere3.Append(" and isnull(t1.fin_money,0)-isnull(rpd_money,0) " + dict["sign"] + " " + dict["money1"] + "");
                    //if (!dict.ContainsKey("chk"))
                    //{
                    //    strWhere3.Append(" and isnull(t1.fin_money,0)-isnull(rpd_money,0) " + dict["sign"] + " " + dict["money1"] + "");
                    //}
                    //else
                    //{
                    //    if (dict["chk"] == "空")
                    //    {
                    //        strWhere3.Append(" and isnull(t1.fin_money,0)-isnull(rpd_money,0) " + dict["sign"] + " " + dict["money1"] + "");
                    //    }
                    //    else
                    //    {
                    //        strWhere3.Append(" and isnull(fcMoney,0)-isnull(chkMoney,0) " + dict["sign"] + " " + dict["money1"] + "");
                    //    }
                    //}
                }
                if (dict.ContainsKey("nature"))
                {
                    strWhere1.Append(" and fin_nature=" + dict["nature"] + "");
                    strWhere3.Append(" and exists(select 1 from MS_finance where t1.fin_oid=o_id and  fin_nature=" + dict["nature"] + ")");
                }
                if (dict.ContainsKey("detail"))
                {
                    strWhere1.Append(" and fin_detail like '%" + dict["detail"] + "%'");
                    strWhere3.Append(" and exists(select 1 from MS_finance where t1.fin_oid=o_id and fin_detail like '%" + dict["detail"] + "%')");
                }
                if (dict.ContainsKey("sdate"))
                {
                    strWhere3.Append(" and datediff(day,o_sdate,'" + dict["sdate"] + "')<=0");
                }
                if (dict.ContainsKey("edate"))
                {
                    strWhere3.Append(" and datediff(day,o_sdate,'" + dict["edate"] + "')>=0");
                }
                if (dict.ContainsKey("sdate1"))
                {
                    strWhere3.Append(" and datediff(day,o_edate,'" + dict["sdate1"] + "')<=0");
                }
                if (dict.ContainsKey("edate1"))
                {
                    strWhere3.Append(" and datediff(day,o_edate,'" + dict["edate1"] + "')>=0");
                }
                if (dict.ContainsKey("check"))
                {
                    strWhere1.Append(" and fin_flag=" + dict["check"] + "");
                    strWhere3.Append(" and exists(select 1 from MS_finance where t1.fin_oid=o_id and  fin_flag=" + dict["check"] + ")");
                }
                if (dict.ContainsKey("name"))
                {
                    strWhere3.Append(" and o_content like '%" + dict["name"] + "%'");
                }
                if (dict.ContainsKey("address"))
                {
                    strWhere3.Append(" and o_address like '%" + dict["address"] + "%'");
                }
                if (dict.ContainsKey("person1"))
                {
                    strWhere3.Append(" and exists(select * from MS_OrderPerson where op_oid=o_id and op_type =1 and op_number like '%" + dict["person1"] + "%')");
                }
                if (dict.ContainsKey("person2"))
                {
                    strWhere3.Append(" and exists(select * from MS_OrderPerson where op_oid=o_id and op_type =2 and op_number like '%" + dict["person2"] + "%')");
                }
                if (dict.ContainsKey("person3"))
                {
                    strWhere3.Append(" and exists(select * from MS_OrderPerson where op_oid=o_id and op_type =3 and op_number like '%" + dict["person3"] + "%')");
                }
                if (dict.ContainsKey("person4"))
                {
                    strWhere3.Append(" and exists(select * from MS_OrderPerson where op_oid=o_id and op_type =4 and op_number like '%" + dict["person4"] + "%')");
                }
                if (dict.ContainsKey("person5"))
                {
                    strWhere3.Append(" and exists(select * from MS_OrderPerson where op_oid=o_id and op_type =5 and op_number like '%" + dict["person5"] + "%')");
                }
                if (dict.ContainsKey("Oid"))
                {
                    strWhere3.Append(" and o_id='"+ dict["Oid"] + "'");
                }
                if (dict.ContainsKey("status"))
                {
                    if (dict["status"] == "3")
                    {
                        strWhere3.Append(" and (o_status=1 or o_status=2)");
                    }
                    else
                    {
                        strWhere3.Append(" and o_status=" + dict["status"] + "");
                    }
                }
                if (dict.ContainsKey("lockstatus"))
                {
                    if (Utils.ObjectToStr(dict["lockstatus"]) == "3")
                    {
                        strWhere3.Append(" and (o_lockStatus=0 or o_lockStatus=2)");
                    }
                    else
                    {
                        strWhere3.Append(" and o_lockStatus=" + dict["lockstatus"] + "");
                    }
                }
                if (dict.ContainsKey("area"))
                {
                    strWhere3.Append(" and exists(select * from MS_OrderPerson where op_oid=o_id and op_type =1 and op_area ='" + dict["area"] + "')");
                }
                if (dict.ContainsKey("sdate2") && !dict.ContainsKey("edate2"))
                {
                    strWhere3.Append(" and exists(select * from MS_ReceiptPay left join MS_ReceiptPayDetail on rp_id=rpd_rpid where rpd_oid=o_id and rp_type=1 and rp_isConfirm=1 and datediff(day,rp_date,'" + dict["sdate2"] + "')<=0)");
                }
                if (!dict.ContainsKey("sdate2") && dict.ContainsKey("edate2"))
                {
                    strWhere3.Append(" and exists(select * from MS_ReceiptPay left join MS_ReceiptPayDetail on rp_id=rpd_rpid where rpd_oid=o_id and rp_type=1 and rp_isConfirm=1 and datediff(day,rp_date,'" + dict["edate2"] + "')>=0)");
                }
                if (dict.ContainsKey("sdate2") && dict.ContainsKey("edate2"))
                {
                    strWhere3.Append(" and exists(select * from MS_ReceiptPay left join MS_ReceiptPayDetail on rp_id=rpd_rpid where rpd_oid=o_id and rp_type=1 and rp_isConfirm=1 and datediff(day,rp_date,'" + dict["sdate2"] + "')<=0 and datediff(day,rp_date,'" + dict["edate2"] + "')>=0)");
                }
                //if (dict.ContainsKey("sdate3"))
                //{
                //    strWhere1.Append(" and datediff(day,fin_edate,'" + dict["sdate3"] + "')<=0");
                //}
                //if (dict.ContainsKey("edate3"))
                //{
                //    strWhere1.Append(" and datediff(day,fin_edate,'" + dict["edate3"] + "')>=0");
                //}
                if (dict.ContainsKey("money2"))
                {
                    strWhere3.Append(" and exists(select * from(select fin_oid,sum(fin_money) fin_money from MS_finance where fin_type=1 group by fin_oid) v1 left join (select rpd_oid,sum(rpd_money) rpd_money from MS_ReceiptPayDetail left join MS_ReceiptPay on rp_id=rpd_rpid where rpd_type=1 and rp_isConfirm=1 group by rpd_oid) v2 on v1.fin_oid=v2.rpd_oid where fin_oid = o_id and fin_money-rpd_money "+dict["sign1"]+ " " + dict["money2"] + ")");
                }
                if (dict.ContainsKey("chk"))
                {
                    if (dict["chk"] == "0")
                    {
                        selectFiled = "o_id,o_status,o_content,o_sdate,o_edate,o_address,c_name,co_name,isnull(t1.fin_money,0) fin_money,isnull(rpd_money,0) rpd_money,isnull(chkMoney,0) chkMoney,(isnull(t1.fin_money,0)-isnull(rpd_money,0)) unReceiptPay,isnull(fcMoney,0) fcMoney,isnull(tfcMoney,0) tfcMoney,(isnull(fcMoney,0)-isnull(chkMoney,0)) unChkMoney";
                        addTable = " left join (select fin_oid,sum(isnull(fc_money,0)) fcMoney from MS_finance left join MS_finance_chk on fin_id = fc_finid where fin_flag<>1 and fin_cid=" + dict["cid"] + " and fin_type='" + dict["type"] + "' and isnull(fc_num,'')='' group by fin_oid) t3 on t1.fin_oid=t3.fin_oid ";
                        chkFiled1 = ",sum(chkMoney) chkMoney";
                        chkFiled2 = ",0 as chkMoney";
                        chkFiled3 = " and isnull(fc_num,'')=''";
                        chkFiled4 = ",0 as fcMoney";
                        chkGroup = ",rpd_num";
                        strWhere3.Append(" and exists(select * from MS_finance left join MS_finance_chk  on fc_finid=fin_id where fin_oid=o_id and fin_cid=" + dict["cid"] + " and isnull(fc_id,0)=0)");
                    }
                    else if (dict["chk"] == "1")
                    {
                        selectFiled = "o_id,o_status,o_content,o_sdate,o_edate,o_address,c_name,co_name,isnull(t1.fin_money,0) fin_money,isnull(rpd_money,0) rpd_money,isnull(chkMoney,0) chkMoney,(isnull(t1.fin_money,0)-isnull(rpd_money,0)) unReceiptPay,isnull(fcMoney,0) fcMoney,isnull(tfcMoney,0) tfcMoney,(isnull(fcMoney,0)-isnull(chkMoney,0)) unChkMoney";
                        addTable = " left join (select fin_oid,sum(isnull(fc_money,0)) fcMoney from MS_finance left join MS_finance_chk on fin_id = fc_finid where fin_flag<>1 and fin_cid=" + dict["cid"] + " and fin_type='" + dict["type"] + "' and isnull(fc_num,'')<>'' group by fin_oid) t3 on t1.fin_oid=t3.fin_oid ";
                        chkFiled1 = ",sum(chkMoney) chkMoney";
                        chkFiled2 = ",(case when isnull(rpd_num,'')<>'' and rp_isConfirm=1 then sum(isnull(rpd_money,0)) else 0 end) chkMoney";
                        chkFiled3 = " and isnull(fc_num,'')<>''";
                        chkFiled4 = ",sum(case when isnull(fc_num,'')<>'' then isnull(fc_money,0) else 0 end) fcMoney";
                        chkGroup = ",rpd_num";
                        strWhere3.Append(" and exists(select * from MS_finance_chk left join MS_finance on fc_finid=fin_id where fc_oid=o_id and fin_cid=" + dict["cid"] + " and isnull(fc_id,0)<>0)");
                    }
                    else
                    {
                        selectFiled = "o_id,o_status,o_content,o_sdate,o_edate,o_address,c_name,co_name,isnull(t1.fin_money,0) fin_money,isnull(rpd_money,0) rpd_money,isnull(chkMoney,0) chkMoney,(isnull(t1.fin_money,0)-isnull(rpd_money,0)) unReceiptPay,isnull(fcMoney,0) fcMoney,isnull(tfcMoney,0) tfcMoney,(isnull(fcMoney,0)-isnull(chkMoney,0)) unChkMoney";
                        addTable = " left join (select fin_oid,sum(isnull(fc_money,0)) fcMoney from MS_finance left join MS_finance_chk on fin_id = fc_finid where fin_flag<>1 and fin_cid=" + dict["cid"] + " and fin_type='" + dict["type"] + "' and fc_num='" + dict["chk"] + "' group by fin_oid) t3 on t1.fin_oid=t3.fin_oid ";
                        chkFiled1 = ",sum(chkMoney) chkMoney";
                        chkFiled2 = ",(case when rpd_num='" + dict["chk"] + "' and rp_isConfirm=1 then sum(isnull(rpd_money,0)) else 0 end) chkMoney";
                        chkFiled3 = " and fc_num='" + dict["chk"] + "'";
                        chkFiled4 = ",sum(case when fc_num='" + dict["chk"] + "' then isnull(fc_money,0) else 0 end) fcMoney";
                        chkGroup = ",rpd_num";
                        strWhere3.Append(" and exists(select * from MS_finance_chk left join MS_finance on fc_finid=fin_id where fc_oid=o_id and fin_cid=" + dict["cid"] + " and fc_num='" + dict["chk"] + "')");
                    }
                }
            }
            strSql.Append("select " + selectFiled + " from");
            strSql.Append("  MS_Order ");
            strSql.Append(" left join (select fin_oid,sum(isnull(fin_money,0)) fin_money from MS_finance where fin_flag <>1 " + strWhere1 + " group by fin_oid ) t1  on o_id = t1.fin_oid");
            strSql.Append(" left join (select fin_oid"+ chkFiled4 + ",sum(isnull(fc_money,0)) tfcMoney from MS_finance left join MS_finance_chk on fin_id = fc_finid where fin_flag<>1 and fin_cid=" + dict["cid"] + " and fin_type='" + dict["type"] + "' group by fin_oid) t3 on t1.fin_oid=t3.fin_oid ");
            strSql.Append(" left join (select rpd_oid,sum(rpd_money) rpd_money " + chkFiled1 + " from( select rpd_oid,(case when rp_isConfirm=1 then sum(isnull(rpd_money,0)) else 0 end) rpd_money " + chkFiled2 + " from MS_ReceiptPayDetail left join MS_ReceiptPay on rpd_rpid=rp_id where 1=1 " + strWhere2 + " group by rpd_oid,rp_isConfirm " + chkGroup + ") t group by rpd_oid) t2 on o_id=t2.rpd_oid");
            strSql.Append(" left join MS_Customer on o_cid=c_id");
            strSql.Append(" left join MS_Contacts on o_coid=co_id");
            strSql.Append(" where 1=1 " + strWhere3 + "");
            SqlParameter[] param = { };
            recordCount = 0;
            totalFin = 0;totalRpd = 0; totalUnMoney=0; totalChk=0 ;totalunChk = 0;
            //recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
            string sFiled = "count(*) c,sum(fin_money) fin_money,sum(rpd_money)rpd_money,sum(unReceiptPay) unReceiptPay,sum(tfcMoney) tfcMoney";
            if (dict.ContainsKey("chk"))
            {
                sFiled = "count(*) c,sum(fin_money) fin_money,sum(rpd_money)rpd_money,sum(unReceiptPay) unReceiptPay,sum(fcMoney) fcMoney,sum(tfcMoney) tfcMoney,sum(unChkMoney) unChkMoney";
            }
            DataTable dt = DbHelperSQL.Query("select " + sFiled + " from (" + strSql.ToString() + ") v").Tables[0];
            if (dt!=null)
            {
                recordCount = Utils.ObjToInt(dt.Rows[0]["c"], 0);
                totalFin = Utils.ObjToDecimal(dt.Rows[0]["fin_money"], 0);
                totalRpd = Utils.ObjToDecimal(dt.Rows[0]["rpd_money"], 0);
                totalUnMoney = Utils.ObjToDecimal(dt.Rows[0]["unReceiptPay"], 0);
                if (dict.ContainsKey("chk"))
                {
                    totalChk = Utils.ObjToDecimal(dt.Rows[0]["fcMoney"], 0);
                }
                else
                {
                    totalChk = Utils.ObjToDecimal(dt.Rows[0]["tfcMoney"], 0);
                }
                if (dict.ContainsKey("chk") && dict["chk"]!="空")
                {
                    totalunChk = Utils.ObjToDecimal(dt.Rows[0]["unChkMoney"], 0);
                }
            }
            if (isPage)
            {
                return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
            }
            else
            {
                return DbHelperSQL.Query(strSql.ToString() + " order by " + filedOrder);
            }
        }

        #endregion

        #region 扩展方法================================
        /// <summary>
        /// 将对象转换实体
        /// </summary>
        public Model.finance DataRowToModel(DataRow row)
        {
            Model.finance model = new Model.finance();
            if (row != null)
            {
                //利用反射获得属性的所有公共属性
                Type modelType = model.GetType();
                for (int i = 0; i < row.Table.Columns.Count; i++)
                {
                    //查找实体是否存在列表相同的公共属性
                    PropertyInfo proInfo = modelType.GetProperty(row.Table.Columns[i].ColumnName);
                    if (proInfo != null && row[i] != DBNull.Value)
                    {
                        proInfo.SetValue(model, row[i], null);//用索引值设置属性值
                    }
                }
            }
            return model;
        }
        /// <summary>
        /// 检查是否存在已审批的记录
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool existCheckCount(string ids, byte? status)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(0) from MS_finance");
            strSql.Append(" where fin_id in (" + ids + ") and fin_flag=@status");
            SqlParameter[] parameters = {
                   new SqlParameter("@status", SqlDbType.TinyInt,4)
            };
            parameters[0].Value = status;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }
        /// <summary>
         /// 更新审批状态
         /// </summary>
         /// <param name="ids">记录ID</param>
         /// <param name="status">0待审批，1审批未通过，2审批通过</param>
         /// <returns></returns>
        public bool updateStatus(int id, byte? status,string remark,string username,string realname)
        {
            string sql = "update MS_finance set fin_flag=@status,fin_checkNum=@num,fin_checkName=@name,fin_checkRemark=@remark where fin_id = "+id+"";
            SqlParameter[] param = {
                new SqlParameter("@status",SqlDbType.TinyInt,4),
                new SqlParameter("@num",SqlDbType.VarChar,5),
                new SqlParameter("@name",SqlDbType.VarChar,20),
                new SqlParameter("@remark",SqlDbType.VarChar,200)
            };
            param[0].Value = status;
            param[1].Value = username;
            param[2].Value = realname;
            param[3].Value = remark;
            return DbHelperSQL.ExecuteSql(sql, param) > 0;
        }
        #endregion

        #region 财务结账
        /// <summary>
        /// 返回当前已结账的最后一个结账月份
        /// </summary>
        /// <returns></returns>
        public string getLastFinancialMonth()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 fin_month from MS_finance where isnull(fin_month,'')<>'' order by fin_month desc");
            SqlParameter[] parameters = {};
            return Utils.ObjectToStr(DbHelperSQL.GetSingle(strSql.ToString(), parameters));
        }

        /// <summary>
        /// 结账
        /// </summary>
        /// <returns></returns>
        public bool dealFinancial(string month)
        {
            string sql = "update fin set fin.fin_month='" + month + "' from MS_finance fin left join MS_Order on fin_oid=o_id where o_lockStatus=1 and isnull(fin_month,'')='' and datediff(MONTH,o_edate,'" + month + "-01')>=0";
            return DbHelperSQL.ExecuteSql(sql) > 0;
        }

        /// <summary>
        /// 反结账
        /// </summary>
        /// <returns></returns>
        public bool cancelFinancial(string month)
        {
            string sql = "update fin set fin.fin_month='' from MS_finance fin left join MS_Order on fin_oid=o_id where o_lockStatus=1 and isnull(fin_month,'')='" + month + "'";
            return DbHelperSQL.ExecuteSql(sql) > 0;
        }

        /// <summary>
        /// 已结账应收付总账
        /// </summary>
        /// <param name="sMonth"></param>
        /// <param name="eMonth"></param>
        /// <returns></returns>
        public DataSet getFinancialSumary(string sMonth, string eMonth,string area)
        {
            StringBuilder strTemp = new StringBuilder();
            if (!string.IsNullOrEmpty(sMonth))
            {
                strTemp.Append(" and datediff(MONTH,fin_month+'-01','" + sMonth + "-01')<=0");
            }
            if (!string.IsNullOrEmpty(eMonth))
            {
                strTemp.Append(" and datediff(MONTH,fin_month+'-01','" + eMonth + "-01')>=0");
            }
            if (!string.IsNullOrEmpty(area))
            {
                strTemp.Append(" and fin_area='" + area + "'");
            }
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select fin_type,fin_area,fin_month,sum(fin_money) fin_money from  MS_finance where isnull(fin_month,'')<>'' " + strTemp + " group by fin_type,fin_area, fin_month  order by  fin_type desc");
            SqlParameter[] param = { };
            return DbHelperSQL.Query(strSql.ToString(), param);
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
        public DataSet getFinancialCustomer(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount,out decimal tmoney1,out decimal tmoney2, bool isPage = true)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select fin_type,fin_area,fin_cid,c_name,sum(fin_money) fin_money from  MS_finance left join MS_Customer on fin_cid=c_id where isnull(fin_month,'')<>'' " + strWhere + " group by fin_type,fin_area,fin_cid, c_name");
            recordCount = 0;tmoney1 = 0;tmoney2 = 0;
            if (isPage)
            {
                //recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
                DataTable dt = DbHelperSQL.Query("select count(*) c,sum(case when fin_type=1 then fin_money else 0 end) money1,sum(case when fin_type=0 then fin_money else 0 end) money2 from (" + strSql.ToString() + ") t").Tables[0];
                if (dt != null)
                {
                    recordCount = Utils.ObjToInt(dt.Rows[0]["c"], 0);
                    tmoney1 = Utils.ObjToDecimal(dt.Rows[0]["money1"], 0);
                    tmoney2 = Utils.ObjToDecimal(dt.Rows[0]["money2"], 0);
                }
                return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
            }
            else
            {
                return DbHelperSQL.Query(strSql.ToString());
            }
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
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * from  MS_finance left join MS_Order on fin_oid=o_id left join MS_Nature on fin_nature=na_id where isnull(fin_month,'')<>'' " + strWhere + "");
            recordCount = 0; tmoney1 = 0; tmoney2 = 0;
            if (isPage)
            {
                //recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
                DataTable dt = DbHelperSQL.Query("select count(*) c,sum(case when fin_type=1 then fin_money else 0 end) money1,sum(case when fin_type=0 then fin_money else 0 end) money2 from (" + strSql.ToString() + ") t").Tables[0];
                if (dt != null)
                {
                    recordCount = Utils.ObjToInt(dt.Rows[0]["c"], 0);
                    tmoney1 = Utils.ObjToDecimal(dt.Rows[0]["money1"], 0);
                    tmoney2 = Utils.ObjToDecimal(dt.Rows[0]["money2"], 0);
                }
                return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
            }
            else
            {
                return DbHelperSQL.Query(strSql.ToString());
            }
        }
        #endregion
    }
}
