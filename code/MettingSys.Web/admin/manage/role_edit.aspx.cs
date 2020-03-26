using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using MettingSys.Common;

namespace MettingSys.Web.admin.manage
{
    public partial class role_edit : Web.UI.ManagePage
    {
        protected string action = DTEnums.ActionEnum.Add.ToString(); //操作类型
        private int id = 0;
        protected string navtype = string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
            action = DTRequest.GetQueryString("action");
            this.id = DTRequest.GetQueryInt("id");
            this.navtype = DTRequest.GetQueryStringValue("navtype", "OA");
            ChkAdminLevel("sys_rolemanage", action); //检查权限
            if (!string.IsNullOrEmpty(action) && action == DTEnums.ActionEnum.Edit.ToString())
            {
                if (this.id == 0)
                {
                    JscriptMsg("传输参数不正确！", "back");
                    return;
                }
                if (!new BLL.manager_role().Exists(this.id))
                {
                    JscriptMsg("角色不存在或已被删除！", "back");
                    return;
                }
            }
            if (!Page.IsPostBack)
            {
                ChkAdminLevel("sys_rolemanage", DTEnums.ActionEnum.View.ToString()); //检查权限
                RoleTypeBind(); //绑定角色类型
                NavBind(this.navtype); //绑定导航
                if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
                {
                    ShowInfo(this.id);
                }
            }
        }

        #region 角色类型=================================
        private void RoleTypeBind()
        {
            Model.manager model = GetAdminInfo();
            ddlRoleType.Items.Clear();
            ddlRoleType.Items.Add(new ListItem("请选择类型...", ""));
            if (model.role_type < 2)
            {
                ddlRoleType.Items.Add(new ListItem("超级用户", "1"));
            }
            ddlRoleType.Items.Add(new ListItem("系统用户", "2"));
        }
        #endregion

        #region 导航菜单=================================
        private void NavBind(string nav_type)
        {
            BLL.navigation bll = new BLL.navigation();
            DataTable dt = bll.GetList(0, nav_type);
            this.rptList.DataSource = dt;
            this.rptList.DataBind();
        }
        #endregion

        #region 赋值操作=================================
        private void ShowInfo(int _id)
        {
            BLL.manager_role bll = new BLL.manager_role();
            Model.manager_role model = bll.GetModel(_id);
            txtRoleName.Text = model.role_name;
            ddlRoleType.SelectedValue = model.role_type.ToString();
            //管理菜单
            if (model.manager_role_values != null)
            {
                for (int i = 0; i < rptList.Items.Count; i++)
                {
                    string navName = ((HiddenField)rptList.Items[i].FindControl("hidName")).Value ;
                    CheckBoxList cblActionType = (CheckBoxList)rptList.Items[i].FindControl("cblActionType");
                    for (int n = 0; n < cblActionType.Items.Count; n++)
                    {
                        Model.manager_role_value modelt = model.manager_role_values.Find(p => p.nav_name == navName && p.action_type == cblActionType.Items[n].Value);
                        if (modelt != null)
                        {
                            cblActionType.Items[n].Selected = true;
                        }
                    }
                }
            }
            //管理权限
            if (model.RolePemissionList != null)
            {
                hCodeStr.Value = "," + string.Join(",", model.RolePemissionList.ConvertAll(u => u.urp_code).ToArray()) + ",";
                rptPermission.DataSource = model.RolePemissionList;
                rptPermission.DataBind();
            }
        }
        #endregion

        #region 增加操作=================================
        private string DoAdd()
        {
            Model.manager_role model = new Model.manager_role();
            BLL.manager_role bll = new BLL.manager_role();

            model.role_name = txtRoleName.Text.Trim();
            model.role_type = int.Parse(ddlRoleType.SelectedValue);

            //管理权限
            if (!string.IsNullOrEmpty(hCodeStr.Value))
            {
                string[] codelist = hCodeStr.Value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                List<Model.userRolePemission> ps = new List<Model.userRolePemission>();
                foreach (string item in codelist)
                {
                    ps.Add(new Model.userRolePemission() { urp_type = 2, urp_code = item });
                }
                model.RolePemissionList = ps;
                if (model.RolePemissionList.FindAll(p => p.urp_code == "0603" || p.urp_code == "0402" || p.urp_code == "0601").Count > 1)
                {
                    return "同一个角色不能同时开通区域审批权(0603)、财务部审批(0402)、总经理审批(0601)中的两个权限";
                }
            }

            //管理菜单
            List<Model.manager_role_value> ls = new List<Model.manager_role_value>();
            for (int i = 0; i < rptList.Items.Count; i++)
            {
                string navName = ((HiddenField)rptList.Items[i].FindControl("hidName")).Value;
                CheckBoxList cblActionType = (CheckBoxList)rptList.Items[i].FindControl("cblActionType");
                for (int n = 0; n < cblActionType.Items.Count; n++)
                {
                    if (cblActionType.Items[n].Selected == true)
                    {
                        ls.Add(new Model.manager_role_value { nav_name = navName, action_type = cblActionType.Items[n].Value });
                    }
                }
            }
            model.manager_role_values = ls;
            
            if (bll.Add(model) > 0)
            {
                AddAdminLog(DTEnums.ActionEnum.Add.ToString(), "添加管理角色:" + model.role_name); //记录日志
                return "";
            }
            return "添加失败";
        }
        #endregion

        #region 修改操作=================================
        private string DoEdit(int _id)
        {
            BLL.manager_role bll = new BLL.manager_role();
            Model.manager_role model = bll.GetModel(_id);

            model.role_name = txtRoleName.Text.Trim();
            model.role_type = int.Parse(ddlRoleType.SelectedValue);

            //管理权限
            string[] codelist = hCodeStr.Value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            List<Model.userRolePemission> ps = new List<Model.userRolePemission>();
            foreach (string item in codelist)
            {
                ps.Add(new Model.userRolePemission() { urp_type = 2, urp_code = item });
            }
            model.RolePemissionList = ps;
            if (model.RolePemissionList.FindAll(p => p.urp_code == "0603" || p.urp_code == "0402" || p.urp_code == "0601").Count > 1)
            {
                return "同一个角色不能同时开通区域审批权(0603)、财务部审批(0402)、总经理审批(0601)中的两个权限";
            }

            //管理菜单
            List<Model.manager_role_value> ls = new List<Model.manager_role_value>();
            for (int i = 0; i < rptList.Items.Count; i++)
            {
                string navName = ((HiddenField)rptList.Items[i].FindControl("hidName")).Value;
                CheckBoxList cblActionType = (CheckBoxList)rptList.Items[i].FindControl("cblActionType");
                for (int n = 0; n < cblActionType.Items.Count; n++)
                {
                    if (cblActionType.Items[n].Selected == true)
                    {
                        ls.Add(new Model.manager_role_value { role_id = _id, nav_name = navName, action_type = cblActionType.Items[n].Value });
                    }
                }
            }
            model.manager_role_values = ls;

            if (bll.Update(model))
            {
                AddAdminLog(DTEnums.ActionEnum.Edit.ToString(), "修改管理角色:" + model.role_name); //记录日志
                return "";
            }
            return "编辑失败";
        }
        #endregion

        //绑定导航权限资源
        protected void rptList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.Item)
            {
                string[] actionTypeArr = ((HiddenField)e.Item.FindControl("hidActionType")).Value.Split(',');
                CheckBoxList cblActionType = (CheckBoxList)e.Item.FindControl("cblActionType");
                cblActionType.Items.Clear();
                for (int i = 0; i < actionTypeArr.Length; i++)
                {
                    if (Utils.ActionType().ContainsKey(actionTypeArr[i]))
                    {
                        cblActionType.Items.Add(new ListItem(" " + Utils.ActionType()[actionTypeArr[i]] + " ", actionTypeArr[i]));
                    }
                }
            }
        }

        //保存
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string result = "";
            if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
            {
                ChkAdminLevel("sys_rolemanage", DTEnums.ActionEnum.Edit.ToString()); //检查权限
                result = DoEdit(this.id);
                if (!string.IsNullOrEmpty(result))
                {
                    JscriptMsg(result, "");
                    return;
                }
                JscriptMsg("修改管理角色成功！", "role_list.aspx");
            }
            else //添加
            {
                ChkAdminLevel("sys_rolemanage", DTEnums.ActionEnum.Add.ToString()); //检查权限
                result = DoAdd();
                if (!string.IsNullOrEmpty(result))
                {
                    JscriptMsg(result, "");
                    return;
                }
                JscriptMsg("添加管理角色成功！", "role_list.aspx");
            }
        }
        

    }
}