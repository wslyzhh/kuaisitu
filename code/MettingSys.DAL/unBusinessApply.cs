using MettingSys.Common;
using MettingSys.DBUtility;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MettingSys.DAL
{
    /// <summary>
    /// 非业务支付申请
    /// </summary>
    public partial class unBusinessApply
    {
        public unBusinessApply() {
        }

        #region 基本方法================================
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from  MS_unBusinessApply");
            strSql.Append(" where ");
            strSql.Append(" uba_id = @id  ");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.Int,4)};
            parameters[0].Value = id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(Model.unBusinessApply model)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();//数据字段
            StringBuilder str2 = new StringBuilder();//数据参数
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<SqlParameter> paras = new List<SqlParameter>();
            strSql.Append("insert into MS_unBusinessApply(");
            foreach (PropertyInfo pi in pros)
            {
                //如果不是主键则追加sql字符串
                if (!pi.Name.Equals("uba_id"))
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
        public bool Update(Model.unBusinessApply model)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<SqlParameter> paras = new List<SqlParameter>();
            strSql.Append("update MS_unBusinessApply set ");
            foreach (PropertyInfo pi in pros)
            {
                //如果不是主键则追加sql字符串
                if (!pi.Name.Equals("uba_id"))
                {
                    //判断属性值是否为空
                    //if (pi.GetValue(model, null) != null)
                    //{
                        
                    //}
                    str1.Append(pi.Name + "=@" + pi.Name + ",");//声明参数
                    paras.Add(new SqlParameter("@" + pi.Name, pi.GetValue(model, null)));//对参数赋值
                }
            }
            strSql.Append(str1.ToString().Trim(','));
            strSql.Append(" where uba_id=@id ");
            paras.Add(new SqlParameter("@id", model.uba_id));
            return DbHelperSQL.ExecuteSql(strSql.ToString(), paras.ToArray()) > 0;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from  MS_unBusinessApply ");
            strSql.Append(" where uba_id=@id");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.Int,4)};
            parameters[0].Value = id;

            return DbHelperSQL.ExecuteSql(strSql.ToString(), parameters) > 0;
        }

        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.unBusinessApply GetModel(int id)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.unBusinessApply model = new Model.unBusinessApply();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                str1.Append(p.Name + ",");//拼接字段
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from MS_unBusinessApply");
            strSql.Append(" where uba_id=@id");
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
            strSql.Append(" * ");
            strSql.Append(" FROM  MS_unBusinessApply b left join MS_payMethod m on b.uba_payMethod=m.pm_id and m.pm_isUse=1");
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
        /// 获得查询分页数据
        /// </summary>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount,out decimal tmoney,bool isPage=true)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select b.*,m.pm_name FROM MS_unBusinessApply b left join MS_payMethod m on b.uba_payMethod=m.pm_id and m.pm_isUse=1");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            recordCount = 0;
            tmoney = 0;
            //recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
            if (isPage)
            {
                DataTable dt = DbHelperSQL.Query("select count(*) c ,sum(uba_money) m from(" + strSql.ToString() + ")t").Tables[0];
                if (dt != null)
                {
                    recordCount = Utils.ObjToInt(dt.Rows[0]["c"], 0);
                    tmoney = Utils.ObjToDecimal(dt.Rows[0]["m"], 0);
                }

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
        public Model.unBusinessApply DataRowToModel(DataRow row)
        {
            Model.unBusinessApply model = new Model.unBusinessApply();
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
        /// 更新审批状态
        /// </summary>
        /// <param name="id">记录ID</param>
        /// <param name="type">1部门审批，2财务审批，3总经理审批</param>
        /// <param name="status">0待审批，1审批未通过，2审批通过</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        public int updateStatus(int id, byte? type, byte? status,string remark,string username,string realname)
        {
            string updateField = "uba_flag1", updateField1 = "uba_checkNum1", updateField2 = "uba_checkName1", updateField3 = "uba_checkRemark1";
            if (type==2)
            {
                updateField = "uba_flag2";
                updateField1 = "uba_checkNum2";
                updateField2 = "uba_checkName2";
                updateField3 = "uba_checkRemark2";
            }
            else if (type == 3)
            {
                updateField = "uba_flag3";
                updateField1 = "uba_checkNum3";
                updateField2 = "uba_checkName3";
                updateField3 = "uba_checkRemark3";
            }
            string sql = "update MS_unBusinessApply set " + updateField + "=@status," + updateField1 + "=@num," + updateField2 + "=@name," + updateField3 + "=@remark where uba_id = " + id + "";
            SqlParameter[] param = {
                new SqlParameter("@status",SqlDbType.TinyInt,4),
                new SqlParameter("@num",SqlDbType.Char,5),
                new SqlParameter("@name",SqlDbType.VarChar,20),
                new SqlParameter("@remark",SqlDbType.VarChar,200)
            };
            param[0].Value = status;
            param[1].Value = username;
            param[2].Value = realname;
            param[3].Value = remark;
            return DbHelperSQL.ExecuteSql(sql, param);
        }
        public int updateConfirmStatus(int id, bool status,DateTime? date,int paymethod,string username,string realname)
        {
            string sql = "update MS_unBusinessApply set uba_isConfirm=@confirm,uba_date=@date,uba_payMethod=@method,uba_confirmerNum=@num,uba_confirmerName=@name where uba_id = " + id + "";
            SqlParameter[] param = {
                new SqlParameter("@confirm",SqlDbType.Bit,4),
                new SqlParameter("@date",SqlDbType.DateTime,20),
                new SqlParameter("@method",SqlDbType.Int,4),
                new SqlParameter("@num",SqlDbType.VarChar,20),
                new SqlParameter("@name",SqlDbType.VarChar,20)
            };
            if (status)
            {
                param[0].Value = status;
                param[1].Value = date;
                param[2].Value = paymethod;
                param[3].Value = username;
                param[4].Value = realname;
            }
            else
            {
                param[0].Value = status;
                param[1].Value = null;
                param[2].Value = 0;
                param[3].Value = "";
                param[4].Value = "";
            }
            return DbHelperSQL.ExecuteSql(sql, param);
        }
        #endregion
    }
}
