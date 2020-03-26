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
    public partial class department
    {
        public department() { }
        #region 基本方法================================
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(int id)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(1) from  MS_department");
            strSql.Append(" where ");
            strSql.Append(" de_id = @id  ");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.Int,4)};
            parameters[0].Value = id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 增加一条数据
        /// </summary>
        public int Add(Model.department model)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();//数据字段
            StringBuilder str2 = new StringBuilder();//数据参数
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<SqlParameter> paras = new List<SqlParameter>();
            strSql.Append("insert into MS_department(");
            foreach (PropertyInfo pi in pros)
            {
                //如果不是主键则追加sql字符串
                if (!pi.Name.Equals("de_id"))
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
        public bool Update(Model.department model)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            List<SqlParameter> paras = new List<SqlParameter>();
            strSql.Append("update MS_department set ");
            foreach (PropertyInfo pi in pros)
            {
                //如果不是主键则追加sql字符串
                if (!pi.Name.Equals("de_id"))
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
            strSql.Append(" where de_id=@id ");
            paras.Add(new SqlParameter("@id", model.de_id));
            return DbHelperSQL.ExecuteSql(strSql.ToString(), paras.ToArray()) > 0;
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public bool Delete(string idstr)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("delete from  MS_department ");
            strSql.Append(" where de_id in ("+idstr+")");
            return DbHelperSQL.ExecuteSql(strSql.ToString()) > 0;
        }
        /// <summary>
        /// 返回总部的名称简码
        /// </summary>
        /// <returns></returns>
        public string getGroupArea()
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select de_area FROM MS_department where de_isGroup=1");
            DataSet ds = DbHelperSQL.Query(strSql.ToString());
            if (ds != null && ds.Tables[0].Rows.Count >0)
            {
                return ds.Tables[0].Rows[0][0].ToString();
            }
            return "";
        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.department GetModel(int id)
        {
            StringBuilder strSql = new StringBuilder();
            StringBuilder str1 = new StringBuilder();
            Model.department model = new Model.department();
            //利用反射获得属性的所有公共属性
            PropertyInfo[] pros = model.GetType().GetProperties();
            foreach (PropertyInfo p in pros)
            {
                str1.Append(p.Name + ",");//拼接字段
            }
            strSql.Append("select top 1 " + str1.ToString().Trim(','));
            strSql.Append(" from MS_department");
            strSql.Append(" where de_id=@id");
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
        /// 判断某个机构下是否存在员工
        /// </summary>
        /// <param name="departID"></param>
        /// <returns></returns>
        public bool hasEmployee(int departID)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" select count(*) from dt_manager where ('-'+departtreeid+'-') like @id");
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.VarChar,20)};
            parameters[0].Value = "%-"+ departID + "-%";
            return DbHelperSQL.Exists(strSql.ToString(), parameters);
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
            strSql.Append(" FROM  MS_department ");
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
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM MS_department");
            if (strWhere.Trim() != "")
            {
                strSql.Append(" where " + strWhere);
            }
            recordCount = Convert.ToInt32(DbHelperSQL.GetSingle(PagingHelper.CreateCountingSql(strSql.ToString())));
            return DbHelperSQL.Query(PagingHelper.CreatePagingSql(recordCount, pageSize, pageIndex, strSql.ToString(), filedOrder));
        }

        public string getLastUserName(string area)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select top 1 user_name from dt_manager where area='" + area + "' and user_name like '" + area + "%' order by user_name desc");

            return Utils.ObjectToStr(DbHelperSQL.GetSingle(strSql.ToString()));
        }

        /// <summary>
        /// 根据父节点ID获取导航列表
        /// </summary>
        public DataTable GetList(int parent_id,string area, bool isUse = true)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select *  FROM MS_department where 1=1");
            if (!isUse)
            {
                strSql.Append(" and de_isUse=1");
            }
            if (!string.IsNullOrEmpty(area))
            {
                string[] arealist = area.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                strSql.Append(" and (de_parentid=0  ");
                foreach (string item in arealist)
                {
                    strSql.Append("or de_area='"+item+"'");
                }
                strSql.Append(") ");
            }
            strSql.Append(" order by de_sort asc,de_id asc");
            DataSet ds = DbHelperSQL.Query(strSql.ToString());
            //重组列表
            DataTable oldData = ds.Tables[0] as DataTable;
            if (oldData == null)
            {
                return null;
            }
            //创建一个新的DataTable增加一个深度字段
            DataTable newData = oldData.Clone();
            newData.Columns.Add("class_layer", typeof(int));
            //调用迭代组合成DAGATABLE
            GetChilds(oldData, newData, parent_id, 0);
            return newData;
        }
        #endregion

        #region 扩展方法================================
        /// <summary>
        /// 从内存中取得所有下级类别列表（自身迭代）
        /// </summary>
        private void GetChilds(DataTable oldData, DataTable newData, int parent_id, int class_layer)
        {
            class_layer++;
            DataRow[] dr = oldData.Select("de_parentid=" + parent_id);
            for (int i = 0; i < dr.Length; i++)
            {
                DataRow row = newData.NewRow();//创建新行
                //循环查找列数量赋值
                for (int j = 0; j < dr[i].Table.Columns.Count; j++)
                {
                    row[dr[i].Table.Columns[j].ColumnName] = dr[i][dr[i].Table.Columns[j].ColumnName];
                }
                row["class_layer"] = class_layer;//赋值深度字段
                newData.Rows.Add(row);//添加新行
                //调用自身迭代
                this.GetChilds(oldData, newData, int.Parse(dr[i]["de_id"].ToString()), class_layer);
            }
        }
        public DataTable getDepartText(int id,out string area)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select * FROM MS_department");
            strSql.Append(" order by de_sort asc,de_id desc");
            DataSet ds = DbHelperSQL.Query(strSql.ToString());

            DataTable dt = ds.Tables[0] as DataTable;
            DataTable newData = dt.Clone();
            newData.Columns.Add("class_layer", typeof(int));
            DataRow[] drs = dt.Select("de_id=" + id);
            DataRow row = newData.NewRow();//创建新行
            //循环查找列数量赋值
            for (int j = 0; j < drs[0].Table.Columns.Count; j++)
            {
                row[drs[0].Table.Columns[j].ColumnName] = drs[0][drs[0].Table.Columns[j].ColumnName];
            }
            newData.Rows.Add(row);
            newData.Rows[0]["class_layer"] = 0;
            newData = getDeparts(dt, newData, 0);
            area = Utils.ObjectToStr(newData.Rows[newData.Rows.Count - 1]["de_area"]); 
            return newData;
        }
        private DataTable getDeparts(DataTable oldData, DataTable newData, int class_layer)
        {
            class_layer++;
            newData.DefaultView.Sort = "class_layer desc";
            newData = newData.DefaultView.ToTable();
            
            DataRow[] dr = oldData.Select("de_id=" + newData.Rows[0]["de_parentid"]);
            for (int i = 0; i < dr.Length; i++)
            {
                DataRow row = newData.NewRow();//创建新行
                //循环查找列数量赋值
                for (int j = 0; j < dr[i].Table.Columns.Count; j++)
                {
                    row[dr[i].Table.Columns[j].ColumnName] = dr[i][dr[i].Table.Columns[j].ColumnName];
                }
                row["class_layer"] = class_layer;//赋值深度字段
                newData.Rows.Add(row);//添加新行
                newData = this.getDeparts(oldData, newData, class_layer);
            }
            return newData;
        }
        /// <summary>
        /// 是否存在该记录
        /// </summary>
        public bool Exists(string name,int parentid, int id = 0)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(0) from MS_department");
            strSql.Append(" where de_name=@name and de_parentid=@parentid");
            if (id > 0)
            {
                strSql.Append(" and de_id <>@id");
            }
            SqlParameter[] parameters = {
                    new SqlParameter("@name", SqlDbType.NVarChar,100),
                    new SqlParameter("@parentid", SqlDbType.Int,4),
                    new SqlParameter("@id", SqlDbType.Int,4)
            };
            parameters[0].Value = name;
            parameters[1].Value = parentid;
            parameters[2].Value = id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }
        /// <summary>
        /// 是否存在总部
        /// </summary>
        public bool ExistsGroup(int id = 0)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("select count(0) from MS_department");
            strSql.Append(" where de_isGroup=1");
            if (id > 0)
            {
                strSql.Append(" and de_id <>@id");
            }
            SqlParameter[] parameters = {
                    new SqlParameter("@id", SqlDbType.Int,4)
            };
            parameters[0].Value = id;

            return DbHelperSQL.Exists(strSql.ToString(), parameters);
        }

        /// <summary>
        /// 将对象转换实体
        /// </summary>
        public Model.department DataRowToModel(DataRow row)
        {
            Model.department model = new Model.department();
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
        /// 修改一列数据
        /// </summary>
        public bool UpdateField(int id, string strValue)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append("update MS_department set " + strValue);
            strSql.Append(" where de_id=" + id);
            return DbHelperSQL.ExecuteSql(strSql.ToString()) > 0;
        }
        #endregion
    }
}
