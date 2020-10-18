using MettingSys.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MettingSys.BLL
{
    public partial class department
    {
        private readonly DAL.department dal;
        public department()
        {
            dal = new DAL.department();
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
        public string Add(Model.department model,string username,string realname)
        {
            if (model.de_type==0)
            {
                return "请选择机构类别";
            }
            if (string.IsNullOrEmpty(model.de_name))
            {
                return "请填写机构全称";
            }
            if (Exists(model.de_name,model.de_parentid.Value))
            {
                return "同一个上级机构下不能存在相同的机构";
            }
            if (model.de_isGroup.Value && ExistsGroup())
            {
                return "已经存在总部的机构，不能再添加总部机构";
            }
            if (model.de_type == 1)
            {
                if (string.IsNullOrEmpty(model.de_subname))
                {
                    return "请填写公司简称";
                }
                if (string.IsNullOrEmpty(model.de_area))
                {
                    return "请填写公司简码";
                }
            }
            else
            {
                model.de_subname = "";
                model.de_area = "";
            }
            Model.department depart = GetModel(model.de_parentid.Value);
            if (depart != null)
            {
                if (model.de_type == 1)//机构类别为公司时
                {
                    if (!depart.de_isGroup.Value)
                    {
                        return "上级机构为总公司时才能添加公司";
                    }
                }
                else
                {
                    if (model.de_parentid == 0)
                    {
                        return "机构类别为部门或岗位时，上级机构必填";
                    }
                    else
                    {
                        model.de_area = depart.de_area;
                    }
                }
            }
            int id = dal.Add(model);
            if (id>0)
            {
                if (model.de_type == 1)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    DataTable dt = dal.GetList(0, "de_type=1", "de_sort asc,de_id asc").Tables[0];
                    if (dt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dt.Rows)
                        {
                            dic.Add(dr["de_area"].ToString(), dr["de_subname"].ToString());
                        }
                        CacheHelper.Remove(DTKeys.COMPANY_AREA);
                        CacheHelper.Insert(DTKeys.COMPANY_AREA, dic, 10);//重新写入缓存
                    }
                }

                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = id;
                logmodel.ol_title = "添加部门岗位";
                logmodel.ol_content = "机构类别：" + Common.BusinessDict.departType()[model.de_type.Value] + "<br/>上级机构ID：" + model.de_parentid + "<br/>机构全称：" + model.de_name + "<br/>机构简称：" + model.de_subname + "<br/>名称简码：" + model.de_area + "<br/>是否总部：" + (model.de_isGroup.Value ? "是" : "否") + "";
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Add.ToString(), logmodel, username, realname);
                return "";
            }
            return "添加失败";
        }

        /// <summary>
        /// 更新一条数据
        /// </summary>
        public string Update(Model.department model,string content, string username, string realname)
        {
            if (string.IsNullOrEmpty(model.de_name))
            {
                return "请填写机构全称";
            }
            if (Exists(model.de_name, model.de_parentid.Value,model.de_id.Value))
            {
                return "同一个上级机构下不能存在相同的机构";
            }
            if (dal.Update(model))
            {
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = model.de_id.Value;
                logmodel.ol_title = "编辑部门岗位";
                logmodel.ol_content = content;
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Edit.ToString(), logmodel, username, realname);
                return "";
            }
            return "编辑失败";
        }

        /// <summary>
        /// 删除一条数据
        /// </summary>
        public string Delete(int id,Model.manager manager)
        {
            Model.department model = GetModel(id);
            if (model == null)
                return "数据不存在";
            if (hasEmployee(id))
            {
                return "该机构下存在员工，不能删除";
            }
            DataTable dt = GetList(id, "");            
            string idstr = id.ToString() + ",";
            foreach (DataRow dr in dt.Rows)
            {
                idstr += dr["de_id"] + ",";
            }
            idstr = idstr.TrimEnd(',');
            if (dal.Delete(idstr))
            {
                if (model.de_type == 1)
                {
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    DataTable dtt = dal.GetList(0, "de_type=1", "de_sort asc,de_id asc").Tables[0];
                    if (dtt.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtt.Rows)
                        {
                            dic.Add(dr["de_area"].ToString(), dr["de_subname"].ToString());
                        }
                        CacheHelper.Remove(DTKeys.COMPANY_AREA);
                        CacheHelper.Insert(DTKeys.COMPANY_AREA, dic, 10);//重新写入缓存
                    }
                }
                Model.business_log logmodel = new Model.business_log();
                logmodel.ol_relateID = id;
                logmodel.ol_title = "删除部门岗位";
                logmodel.ol_content = "机构ID：" + id + "<br/>机构名称："+model.de_name;
                logmodel.ol_operateDate = DateTime.Now;
                new business_log().Add(DTEnums.ActionEnum.Delete.ToString(), logmodel, manager.user_name, manager.real_name);
                return "";
            }
            return "删除失败";
        }
        /// <summary>
        /// 返回总部的名称简码
        /// </summary>
        /// <returns></returns>
        public string getGroupArea()
        {
            string jc = CacheHelper.Get<string>(DTKeys.COMPANY_JC);//从缓存取出
            if (string.IsNullOrEmpty(jc))
            {
                jc = dal.getGroupArea();
                CacheHelper.Insert(DTKeys.COMPANY_JC, jc, 10);//重新写入缓存
            }
            return jc;
        }
        /// <summary>
        /// 得到一个对象实体
        /// </summary>
        public Model.department GetModel(int id)
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
        /// 根据区域代码返回区域中文
        /// </summary>
        /// <param name="area">SY,HK</param>
        /// <returns></returns>
        public string getAreaText(string area)
        {
            if (string.IsNullOrEmpty(area))
                return "";
            Dictionary<string, string> dic = getAreaDict();
            string result = string.Empty;
            string[] list = area.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string item in list)
            {
                result += dic[item] + ",";
            }
            return result.TrimEnd(',');
        }
        /// <summary>
        /// 判断某个机构下是否存在员工
        /// </summary>
        /// <param name="departID"></param>
        /// <returns></returns>
        public bool hasEmployee(int departID)
        {
            return dal.hasEmployee(departID);
        }
        /// <summary>
        /// 公司区域字典
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> getAreaDict(string area="")
        {
            Dictionary<string, string> dic = CacheHelper.Get<Dictionary<string, string>>(DTKeys.COMPANY_AREA);//从缓存取出
            //如果缓存已过期则从数据库里面取出
            if (dic == null)
            {
                dic = new Dictionary<string, string>();
                DataTable dt = dal.GetList(0, "de_type=1", "de_sort asc,de_id asc").Tables[0];
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        dic.Add(dr["de_area"].ToString(), dr["de_subname"].ToString());
                    }
                    CacheHelper.Insert(DTKeys.COMPANY_AREA, dic, 10);//重新写入缓存
                }
            }
            Dictionary<string, string> newDict = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(area))
            {
                foreach (var item in dic.Keys)
                {
                    if (item == area)
                    {
                        newDict.Add(item, dic[item]);
                    }
                }
            }
            else
            {
                newDict = dic;
            }
            return newDict;
        }

        /// <summary>
        /// 获得查询分页数据
        /// </summary>
        public DataSet GetList(int pageSize, int pageIndex, string strWhere, string filedOrder, out int recordCount)
        {
            return dal.GetList(pageSize, pageIndex, strWhere, filedOrder, out recordCount);
        }
        /// <summary>
        /// 取得所有列表
        /// </summary>
        public DataTable GetList(int parent_id, string area, bool isUse = true)
        {
            return dal.GetList(parent_id,area, isUse);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="area"></param>
        /// <returns></returns>
        public string getNewUserName(string area)
        {
            if (string.IsNullOrEmpty(area)) return "";
            string username = string.Empty;// dal.getLastUserName(area);
            DataSet ds = new manager().GetList(0, "area='" + area + "' and user_name like '" + area + "%'", "user_name");
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count>0)
            {
                int n = 0, num = 0;
                string nextNum = string.Empty;
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    n = Utils.ObjToInt(Utils.ObjectToStr(ds.Tables[0].Rows[i]["user_name"]).Substring(2));
                    //下一个工号
                    nextNum = getUsernameByNum(n + 1, area);

                    if (ds.Tables[0].Select("user_name='" + nextNum + "'").Length > 0)
                    {
                        continue;
                    }
                    else
                    {
                        num = n + 1;
                        break;
                    }
                    //if (n != i + 1)
                    //{
                    //    num = i + 1;                        
                    //}
                    //else
                    //{
                    //    //最后一个也没匹配，则在最后一个上加1生成新的工号
                    //    if (i == ds.Tables[0].Rows.Count-1 && string.IsNullOrEmpty(username))
                    //    {
                    //        n = Utils.ObjToInt(Utils.ObjectToStr(ds.Tables[0].Rows[i]["user_name"]).Substring(2));
                    //        num = n + 1;
                    //    }
                    //}
                }
                username = getUsernameByNum(num, area);
            }
            if (string.IsNullOrEmpty(username)) return area + "001";

            return username;
        }

        public string getUsernameByNum(int num,string area)
        {
            if (string.IsNullOrEmpty(area)) return "";
            string numStr = "";
            if (num != 0)
            {
                if (num.ToString().Length == 1)
                {
                    numStr = "00" + num;
                }
                else if (num.ToString().Length == 2)
                {
                    numStr = "0" + num;
                }
                else
                {
                    numStr = num.ToString();
                }
                return area + numStr;
            }
            return area + "001";
        }

        /// <summary>
        /// 根据岗位ID返回完整的岗位描述
        /// </summary>
        /// <param name="id"></param>
        /// <param name="TextTree"></param>
        /// <param name="IDTree"></param>
        /// <returns></returns>
        public void getDepartText(int id,out string TextTree,out string IDTree,out string area)
        {
            TextTree = "";IDTree = "";
            DataTable dt= dal.getDepartText(id,out area);
            if (dt!=null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    TextTree += dr["de_name"].ToString() + "-";
                    IDTree += dr["de_id"].ToString() + "-";
                }
            }
            TextTree = TextTree.TrimEnd('-');
            IDTree = IDTree.TrimEnd('-');
        }
        #endregion

        #region 扩展方法================================
        public bool Exists(string name,int parentid, int id = 0)
        {
            return dal.Exists(name,parentid, id);
        }
        /// <summary>
        /// 是否存在总部
        /// </summary>
        public bool ExistsGroup(int id = 0)
        {
            return dal.ExistsGroup(id);
        }
        /// <summary>
        /// 保存排序
        /// </summary>
        public bool UpdateSort(int id, int sort_id)
        {
            dal.UpdateField(id, "de_sort=" + sort_id);
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="area"></param>
        /// <param name="username"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public DataTable getAllEmployee(string area,string username="",bool isShowNum = false,string hasOrder="")
        {
            DataTable dt = GetList(0, area, false);
            string sqlwhere = "";
            if (!string.IsNullOrEmpty(username))
            {
                sqlwhere = " and (user_name like '%"+ username + "%' or real_name like '%" + username + "%')";
            }
            DataSet ds = new BLL.manager().GetList(0, "is_lock=0 "+ sqlwhere + "", " user_name asc,id asc");
            if (!string.IsNullOrEmpty(username))
            {
                DataTable nDt = dt.Clone();
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        filerData(dt, nDt, Utils.ObjToInt(ds.Tables[0].Rows[i]["departID"], 0));
                    }
                    //保持原有顺序
                    DataTable newDT1 = nDt.Clone();
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        DataRow[] ndr = nDt.Select("de_id = " + dt.Rows[i]["de_id"] + "");
                        if (ndr != null && ndr.Length > 0)
                        {
                            DataRow row = newDT1.NewRow();//创建新行
                                                         //循环查找列数量赋值
                            for (int j = 0; j < dt.Columns.Count; j++)
                            {
                                row[dt.Columns[j].ColumnName] = dt.Rows[i][dt.Columns[j].ColumnName];
                            }
                            newDT1.Rows.Add(row);
                        }
                    }
                    dt = newDT1;
                }
                else
                {
                    dt = null;
                }
            }

            DataTable newDT = dt.Clone();
            newDT.Columns.Add("detailDepart");
            if (dt!=null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    DataRow ndr = newDT.NewRow();
                    //循环查找列数量赋值
                    for (int j = 0; j < dr.Table.Columns.Count; j++)
                    {
                        ndr[dr.Table.Columns[j].ColumnName] = dr[dr.Table.Columns[j].ColumnName];
                    }
                    newDT.Rows.Add(ndr);
                    DataRow[] drs = ds.Tables[0].Select("departID="+dr["de_id"]+"");
                    foreach (DataRow drr in drs)
                    {
                        ndr = newDT.NewRow();
                        ndr["de_parentid"] = dr["de_id"];
                        ndr["de_type"] = "4";
                        ndr["de_area"] = drr["area"];
                        ndr["de_subname"] = drr["user_name"];
                        ndr["de_name"] = drr["real_name"];
                        ndr["detailDepart"] = drr["detaildepart"];
                        ndr["class_layer"] = Convert.ToInt32(dr["class_layer"]) + 1;
                        newDT.Rows.Add(ndr);
                    }
                }
            }
            //显示人员的订单数量
            DataTable lastDT = newDT.Copy();
            lastDT.Columns.Add("orderCount");
            if (isShowNum)
            {
                DataTable orderNumDT = new BLL.Order().getAllDStatusOrder(hasOrder);                
                foreach (DataRow dr in lastDT.Rows)
                {
                    dr["orderCount"] = "0";
                    if (Utils.ObjectToStr(dr["de_type"]) == "4")
                    {
                        DataRow[] drs = orderNumDT.Select("op_number='"+dr["de_subname"] + "' and op_name='" + dr["de_name"] + "'");
                        if (drs != null && drs.Length >0)
                        {
                            dr["orderCount"] = Utils.ObjectToStr(drs[0]["ordernum"]);
                        }
                    }
                }
            }
            return lastDT;
        }

        public void filerData(DataTable oldData,DataTable newData,int departid)
        {
            DataRow[] dr = oldData.Select("de_id=" + departid);
            if (dr != null && dr.Length ==1)
            {
                DataRow row = newData.NewRow();//创建新行
                //循环查找列数量赋值
                for (int j = 0; j < dr[0].Table.Columns.Count; j++)
                {
                    row[dr[0].Table.Columns[j].ColumnName] = dr[0][dr[0].Table.Columns[j].ColumnName];
                }
                DataRow[] ndr = newData.Select("de_id=" + row["de_id"] + "");
                if (ndr == null || ndr.Length == 0)
                {
                    newData.Rows.Add(row);
                }
                departid = Utils.ObjToInt(dr[0]["de_parentid"], 0);
                this.filerData(oldData, newData, departid);
            }
            
        }
        
        #endregion
    }
}
