using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin.manage
{
    public partial class department_edit : Web.UI.ManagePage
    {
        protected string action = DTEnums.ActionEnum.Add.ToString(); //操作类型
        private int id = 0;
        protected Model.manager manager = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            action = DTRequest.GetQueryString("action");
            this.id = DTRequest.GetQueryInt("id");
            ChkAdminLevel("sys_department", action); //检查权限
            if (!string.IsNullOrEmpty(action) && action == DTEnums.ActionEnum.Edit.ToString())
            {
                this.action = DTEnums.ActionEnum.Edit.ToString();//修改类型
                if (this.id == 0)
                {
                    JscriptMsg("传输参数不正确！", "back");
                    return;
                }
                if (!new BLL.department().Exists(this.id))
                {
                    JscriptMsg("导航不存在或已被删除！", "back");
                    return;
                }
            }
            if (!Page.IsPostBack)
            {
                initData(1);
                ChkAdminLevel("sys_department", DTEnums.ActionEnum.View.ToString()); //检查权限              
                if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
                {
                    ShowInfo(this.id);
                }
                else
                {
                    TreeBind();
                    if (this.id > 0)
                    {
                        this.ddlParentId.SelectedValue = this.id.ToString();                        
                        Model.department de = new BLL.department().GetModel(this.id);
                        if (de != null)
                        {
                            if (de.de_isGroup.Value) { initData(1); dlgroup.Visible = false; }
                            else initData(2);
                            string _treeText = string.Empty, _treeid = string.Empty;
                            new BLL.department().getDepartText(this.id, out _treeText, out _treeid, out string area);
                            labDepartText.Text = _treeText;
                        }

                    }
                }
            }
        }

        #region 绑定导航菜单=============================
        private void TreeBind()
        {
            BLL.department bll = new BLL.department();
            DataTable dt = bll.GetList(0, "", false);

            this.ddlParentId.Items.Clear();
            this.ddlParentId.Items.Add(new ListItem("无父级导航", "0"));
            foreach (DataRow dr in dt.Rows)
            {
                string Id = dr["de_id"].ToString();
                int ClassLayer = int.Parse(dr["class_layer"].ToString());
                string Title = dr["de_name"].ToString().Trim();

                if (ClassLayer == 1)
                {
                    this.ddlParentId.Items.Add(new ListItem(Title, Id));
                }
                else
                {
                    Title = "├ " + Title;
                    Title = Utils.StringOfChar(ClassLayer - 1, "　") + Title;
                    this.ddlParentId.Items.Add(new ListItem(Title, Id));
                }
            }
        }
        #endregion

        #region 初始化
        private void initData(byte? type=3)
        {
            ddltype.DataSource = Common.BusinessDict.departType(type);
            ddltype.DataTextField = "value";
            ddltype.DataValueField = "key";
            ddltype.DataBind();
            ddltype.Items.Insert(0, new ListItem("请选择", ""));
        }
        #endregion

        #region 赋值操作=================================
        private void ShowInfo(int _id)
        {
            BLL.department bll = new BLL.department();
            Model.department model = bll.GetModel(_id);

            TreeBind();
            ddlParentId.SelectedValue = model.de_parentid.ToString();
            string _treeText = string.Empty, _treeid = string.Empty;
            bll.getDepartText(_id, out _treeText, out _treeid, out string area);
            labDepartText.Text = _treeText;
            initData();
            ddltype.SelectedValue = model.de_type.ToString();
            txtSortId.Text = model.de_sort.ToString();
            txtTitle.Text = model.de_name;
            txtSubTitle.Text = model.de_subname;
            txtArea.Text = model.de_area;
            isgroupDiv.Visible = false;
            if (model.de_isGroup.Value)
            {
                labIsGroup.Text = "是";
                cbIsGroup.Checked = true;
            }
            else
            {
                labIsGroup.Text = "否";
                cbIsGroup.Checked = false;
            }
            if (model.de_isUse.Value)
            {
                cbIsUse.Checked = true;
            }
            else
            {
                cbIsUse.Checked = false;
            }
            if (model.de_type == 2 || model.de_type == 3)
            {
                dlsubtitle.Visible = false;
                dlarea.Visible = false;
                dlgroup.Visible = false;
            }
            txtSubTitle.Enabled = false;
            txtArea.Enabled = false;
            ddlParentId.Enabled = false;
            ddltype.Enabled = false;
        }
        #endregion

        #region 增加操作=================================
        private string DoAdd()
        {
            try
            {
                Model.department model = new Model.department();
                BLL.department bll = new BLL.department();
                manager = GetAdminInfo();
                model.de_type = string.IsNullOrEmpty(ddltype.SelectedValue) ? (byte)0 : Utils.ObjToByte(ddltype.SelectedValue);
                model.de_parentid = int.Parse(ddlParentId.SelectedValue);
                model.de_isGroup = cbIsGroup.Checked;
                model.de_name = txtTitle.Text.Trim();
                model.de_subname = txtSubTitle.Text.Trim();
                model.de_area = txtArea.Text.Trim().ToUpper();
                model.de_sort = Utils.ObjToInt(txtSortId.Text.Trim(),0);
                model.de_isUse = cbIsUse.Checked;
                return bll.Add(model, manager.user_name, manager.real_name);
            }
            catch (Exception e)
            {
                return e.Message;
            }

        }
        #endregion

        #region 修改操作=================================
        private string DoEdit(int _id)
        {
            try
            {
                BLL.department bll = new BLL.department();
                Model.department model = bll.GetModel(_id);
                manager = GetAdminInfo();
                string content = string.Empty;
                model.de_type = string.IsNullOrEmpty(ddltype.SelectedValue) ? (byte)0 : Utils.ObjToByte(ddltype.SelectedValue);
                model.de_parentid = int.Parse(ddlParentId.SelectedValue);
                if (model.de_name != txtTitle.Text.Trim())
                {
                    content += "机构名称：" + model.de_name + "→<font color='red'>" + txtTitle.Text.Trim() + "</font><br/>";
                }
                model.de_name = txtTitle.Text.Trim();
                if (model.de_isUse != cbIsUse.Checked)
                {
                    content += "状态：" + Common.BusinessDict.isUseStatus(1)[model.de_isUse] + "→<font color='red'>" + Common.BusinessDict.isUseStatus(1)[cbIsUse.Checked] + "</font>";
                }
                model.de_isUse = cbIsUse.Checked;
                model.de_sort = Convert.ToInt32(txtSortId.Text.Trim());
                return bll.Update(model, content, manager.user_name, manager.real_name);
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }
        #endregion

        //保存
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string result = string.Empty;
            if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
            {
                ChkAdminLevel("sys_department", DTEnums.ActionEnum.Edit.ToString()); //检查权限
                result = DoEdit(this.id);
                if (!string.IsNullOrEmpty(result))
                {
                    JscriptDialog("错误", result, "", "");
                    return;
                }
                JscriptMsg("修改部门岗位成功！", "department_list.aspx");
            }
            else //添加
            {
                ChkAdminLevel("sys_department", DTEnums.ActionEnum.Add.ToString()); //检查权限
                result = DoAdd();
                if (!string.IsNullOrEmpty(result))
                {
                    JscriptDialog("错误", result, "", "");
                    return;
                }
                JscriptMsg("添加部门岗位成功！", "department_list.aspx");
            }
        }

        protected void ddltype_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            if (ddl.SelectedValue == "2" || ddl.SelectedValue == "3")
            {
                dlsubtitle.Visible = false;
                dlarea.Visible = false;
                dlgroup.Visible = false;
            }
            else
            {
                dlsubtitle.Visible = true;
                dlarea.Visible = true;
            }
        }

        protected void ddlParentId_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            if (ddl.SelectedValue!="0")
            {
                Model.department de = new BLL.department().GetModel(int.Parse(ddl.SelectedValue));
                if (de.de_isGroup.Value)
                {
                    dlgroup.Visible = false;
                    dlsubtitle.Visible = true;
                    dlarea.Visible = true;
                    initData();
                }
                else
                {
                    string _treeText = string.Empty, _treeid = string.Empty;
                    new BLL.department().getDepartText(int.Parse(ddl.SelectedValue), out _treeText, out _treeid, out string area);
                    labDepartText.Text = _treeText;
                    dlsubtitle.Visible = false;
                    dlarea.Visible = false;
                    dlgroup.Visible = false;
                    initData(2);
                }

            }
            else
            {
                dlgroup.Visible = true;
                dlsubtitle.Visible = true;
                dlarea.Visible = true;
                initData(1);
            }
        }
    }
}