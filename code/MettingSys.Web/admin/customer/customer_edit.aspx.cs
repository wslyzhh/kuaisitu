using MettingSys.Common;
using NPOI.HSSF.UserModel;
using NPOI.HSSF.Util;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BorderStyle = NPOI.SS.UserModel.BorderStyle;

namespace MettingSys.Web.admin.customer
{
    public partial class customer_edit : Web.UI.ManagePage
    {
        protected string action = DTEnums.ActionEnum.Add.ToString();//操作类型
        protected int id = 0;

        protected Model.business_log log = null;
        protected Model.manager manager = null;
        protected Model.Customer model = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            action = DTRequest.GetQueryString("action");
            ChkAdminLevel("sys_customer_list", action); //检查权限
            if (!string.IsNullOrEmpty(action) && (action == DTEnums.ActionEnum.Edit.ToString() || action == DTEnums.ActionEnum.View.ToString()))
            {
                this.action = DTEnums.ActionEnum.Edit.ToString();//修改类型
                this.id = DTRequest.GetQueryInt("id");
                if (this.id == 0)
                {
                    JscriptMsg("传输参数不正确！", "back");
                    return;
                }
                if (!new BLL.Customer().Exists(this.id))
                {
                    JscriptMsg("记录不存在或已被删除！", "back");
                    return;
                }
            }
            manager = GetAdminInfo();
            contactDiv.Visible = true;
            bankDiv.Visible = true;
            if (id == 0)
            {
                contactDiv.Visible = false;
                bankDiv.Visible = false;
            }
            if (!Page.IsPostBack)
            {
                initData();                
                if (action == DTEnums.ActionEnum.Edit.ToString() || action == DTEnums.ActionEnum.View.ToString()) //修改
                {
                    ShowInfo(this.id);
                }
            }
        }

        private void initData()
        {
            byte type = 1;
            manager = GetAdminInfo();
            //存在客户管理权限可以添加管理用客户
            if (new BLL.permission().checkHasPermission(manager, "0301"))
            {
                type = 2;
            }
            ddltype.DataSource = Common.BusinessDict.customerType(type);
            ddltype.DataTextField = "value";
            ddltype.DataValueField = "key";
            ddltype.DataBind();
            ddltype.Items.Insert(0, new ListItem("请选择", ""));
        }

        #region 赋值操作=================================
        private void ShowInfo(int _id)
        {
            BLL.Customer bll = new BLL.Customer();
            model = bll.GetModel(_id);

            txtName.Text = model.c_name;
            ddltype.SelectedValue = model.c_type.ToString();
            if (model.c_type == 3)
            {
                ddltype.Visible = false;
                labtype.Text = Common.BusinessDict.customerType()[model.c_type.Value];
                btnDelete.Visible = false;
                btnDelBank.Visible = false;
                liAdd.Visible = false;
            }
            txtNum.Text = model.c_num;
            txtBusinessScope.Text = model.c_business;
            txtRemark.Text = model.c_remarks;
            if (model.c_isUse.Value)
            {
                cbIsUse.Checked = true;
            }
            else
            {
                cbIsUse.Checked = false;
            }
            btnSubmit.Visible = false;
            BtnContact.Visible = false;
            BtnBank.Visible = false;
            //已审核通过的不能再修改
            if (model.c_flag == 2)
            {
                btnSubmit.Visible = false;
                BtnContact.Visible = false;
                BtnBank.Visible = false;
            }
            else
            {
                if (manager.user_name == model.c_owner)
                {
                    btnSubmit.Visible = true;
                    BtnContact.Visible = true;
                    BtnBank.Visible = true;
                }
                else
                {
                    if (new MettingSys.BLL.permission().checkHasPermission(manager, "0301"))
                    {
                        btnSubmit.Visible = true;
                        BtnContact.Visible = true;
                        BtnBank.Visible = true;
                    }
                }
            }
            
            Mdl1.Visible = false;
            Mdl2.Visible = false;            

            //绑定联系人
            this.rptList.DataSource = new BLL.Contacts().GetList(0,"co_cid="+_id+"","co_flag desc,co_id asc");
            this.rptList.DataBind();

            //绑定银行账号
            this.bankrptList.DataSource = new BLL.customerBank().GetList(0, "cb_cid=" + _id + "", "cb_id asc");
            this.bankrptList.DataBind();
        }
        #endregion

        #region 增加操作=================================
        private string DoAdd(out int cid)
        {
            cid = 0;
            Model.Customer model = new Model.Customer();
            BLL.Customer bll = new BLL.Customer();
            
            manager = GetAdminInfo();
            model.c_name = txtName.Text.Trim();
            model.c_type = (byte)Utils.ObjToInt(ddltype.SelectedValue);
            model.c_num = txtNum.Text.ToString();
            model.c_isUse = cbIsUse.Checked;
            model.c_remarks = txtRemark.Text.Trim();
            model.c_flag = 0;
            model.c_owner = manager.user_name;
            model.c_ownerName = manager.real_name;
            model.c_addDate = DateTime.Now;
            model.c_business = txtBusinessScope.Text.Trim();

            Model.Contacts contact = new Model.Contacts();
            contact.co_flag = true;
            contact.co_name = txtMContact.Text.Trim();
            contact.co_number = txtMPhone.Text.Trim();
            
            return bll.Add(model, contact, manager, out cid);
        }
        #endregion

        #region 修改操作=================================
        private string DoEdit(int _id)
        {
            BLL.Customer bll = new BLL.Customer();
            Model.Customer model = bll.GetModel(_id);
            manager = GetAdminInfo();
           
            string _content = string.Empty;
            if (model.c_name != txtName.Text.Trim())
            {
                _content += "客户名称:" + model.c_name + "→<font color='red'>" + txtName.Text.Trim() + "<font><br/>";
            }
            byte oldtype = model.c_type.Value;
            model.c_name = txtName.Text.Trim();
            if (model.c_type != 3)
            {
                if (model.c_type != Utils.ObjToByte(ddltype.SelectedValue))
                {
                    _content += "客户类别:" + Common.BusinessDict.customerType()[model.c_type] + "→<font color='red'>" + Common.BusinessDict.customerType()[Utils.ObjToByte(ddltype.SelectedValue)] + "<font><br/>";
                }
                model.c_type = Utils.ObjToByte(ddltype.SelectedValue);
            }
            if (model.c_num != txtNum.Text.Trim())
            {
                _content += "信用代码(税号):" + model.c_num + "→<font color='red'>" + txtNum.Text.Trim() + "<font><br/>";
            }
            model.c_num = txtNum.Text.Trim();
            if (model.c_isUse != cbIsUse.Checked)
            {
                _content += "启用状态:" + Common.BusinessDict.isUseStatus()[model.c_isUse] + "→<font color='red'>" + Common.BusinessDict.isUseStatus()[cbIsUse.Checked] + "<font><br/>";
            }
            model.c_isUse = cbIsUse.Checked;
            if (model.c_remarks != txtRemark.Text.Trim())
            {
                _content += "备注:" + model.c_remarks + "→<font color='red'>" + txtRemark.Text.Trim() + "<font><br/>";
            }
            model.c_remarks = txtRemark.Text.Trim();
            if (model.c_business != txtBusinessScope.Text.Trim())
            {
                _content += "业务范围:" + model.c_business + "→<font color='red'>" + txtBusinessScope.Text.Trim() + "<font><br/>";
            }
            model.c_business = txtBusinessScope.Text.Trim();


            return bll.Update(oldtype,model, manager, _content);
        }
        #endregion

        //保存
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string result = "";
            if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
            {
                ChkAdminLevel("sys_customer_list", DTEnums.ActionEnum.Edit.ToString()); //检查权限
                result = DoEdit(this.id);
                if (result != "")
                {
                    JscriptMsg(result, "");
                    return;
                }
                JscriptMsg("修改客户成功！", "customer_edit.aspx?action=Edit&id=" + this.id, "");
            }
            else //添加
            {
                ChkAdminLevel("sys_customer_list", DTEnums.ActionEnum.Add.ToString()); //检查权限
                int cid = 0;
                result = DoAdd(out cid);
                if (result != "")
                {
                    JscriptMsg(result, "");
                    return;
                }
                string msbox = "jsprint(\"添加客户成功！\", \"customer_edit.aspx?action=Edit&id=" + cid + "\", \"\")";
                ClientScript.RegisterClientScriptBlock(Page.GetType(), "JsPrint", msbox, true);
                Response.Redirect("customer_edit.aspx?action=Edit&id=" + cid);
                //JscriptMsg("添加客户成功！", "customer_edit.aspx?action=" + action + "&id=" + cid, "");
            }
        }
        //批量删除
        protected void btnDelete_Click(object sender, EventArgs e)
        {
            //ChkAdminLevel("sys_customer_list", DTEnums.ActionEnum.Delete.ToString()); //检查权限
            BLL.Contacts bll = new BLL.Contacts();
            string result = "";
            log = new Model.business_log();
            string idstr = string.Empty;
            int success = 0, error = 0;
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < rptList.Items.Count; i++)
            {
                int id = Convert.ToInt32(((HiddenField)rptList.Items[i].FindControl("hidId")).Value);
                CheckBox cb = (CheckBox)rptList.Items[i].FindControl("chkId");
                if (cb.Checked)
                {
                    result = bll.Delete(Convert.ToInt32(id), manager);
                    if (result == "")
                    {
                        success++;
                    }
                    else
                    {
                        error++;
                        sb.Append(result + "<br/>");
                    }
                }
            }
            JscriptMsg("共选择" + (success + error) + "条记录，成功" + success + "条，失败" + error + "条<br/>" + sb.ToString(), "customer_edit.aspx?action=" + action + "&id=" + this.id);
        }

        protected void btnDelBank_Click(object sender, EventArgs e)
        {
            //ChkAdminLevel("sys_customerBank", DTEnums.ActionEnum.Delete.ToString()); //检查权限
            BLL.customerBank bll = new BLL.customerBank();
            string result = "";
            int success = 0, error = 0;
            StringBuilder sb = new StringBuilder();
            manager = GetAdminInfo();
            for (int i = 0; i < bankrptList.Items.Count; i++)
            {
                int id = Convert.ToInt32(((HiddenField)bankrptList.Items[i].FindControl("hidId")).Value);
                CheckBox cb = (CheckBox)bankrptList.Items[i].FindControl("chkId");
                if (cb.Checked)
                {
                    result = bll.Delete(id, manager);
                    if (result == "")
                    {
                        success++;
                    }
                    else
                    {
                        error++;
                        sb.Append(result + "<br/>");
                    }
                }
            }
            JscriptMsg("共选择" + (success + error) + "条记录，成功" + success + "条，失败" + error + "条<br/>" + sb.ToString(), "customer_edit.aspx?action=" + action + "&id=" + this.id);
        }

        protected void btnExcel_Click(object sender, EventArgs e)
        {
            DataTable dt = new BLL.customerBank().GetList(0, "cb_cid=" + id + "", "cb_id asc").Tables[0];
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=客户银行账号列表.xlsx"); //HttpUtility.UrlEncode(fileName));
            HttpContext.Current.Response.ContentType = "application/vnd.ms-excel";

            HSSFWorkbook hssfworkbook = new HSSFWorkbook();
            ISheet sheet = hssfworkbook.CreateSheet("明细");
            IFont font = hssfworkbook.CreateFont();
            font.Boldweight = short.MaxValue;
            font.FontHeightInPoints = 11;

            #region 表格样式
            //设置单元格的样式：水平垂直对齐居中
            ICellStyle cellStyle = hssfworkbook.CreateCellStyle();
            cellStyle.Alignment = HorizontalAlignment.Center;
            cellStyle.VerticalAlignment = VerticalAlignment.Center;
            cellStyle.BorderBottom = BorderStyle.Thin;
            cellStyle.BorderLeft = BorderStyle.Thin;
            cellStyle.BorderRight = BorderStyle.Thin;
            cellStyle.BorderTop = BorderStyle.Thin;
            cellStyle.BottomBorderColor = HSSFColor.Black.Index;
            cellStyle.LeftBorderColor = HSSFColor.Black.Index;
            cellStyle.RightBorderColor = HSSFColor.Black.Index;
            cellStyle.TopBorderColor = HSSFColor.Black.Index;
            cellStyle.WrapText = true;//自动换行

            //设置表头的样式：水平垂直对齐居中，加粗
            ICellStyle titleCellStyle = hssfworkbook.CreateCellStyle();
            titleCellStyle.Alignment = HorizontalAlignment.Center;
            titleCellStyle.VerticalAlignment = VerticalAlignment.Center;
            titleCellStyle.FillForegroundColor = HSSFColor.Grey25Percent.Index; //图案颜色
            titleCellStyle.FillPattern = FillPattern.SparseDots; //图案样式
            titleCellStyle.FillBackgroundColor = HSSFColor.Grey25Percent.Index; //背景颜色
            //设置边框
            titleCellStyle.BorderBottom = BorderStyle.Thin;
            titleCellStyle.BorderLeft = BorderStyle.Thin;
            titleCellStyle.BorderRight = BorderStyle.Thin;
            titleCellStyle.BorderTop = BorderStyle.Thin;
            titleCellStyle.BottomBorderColor = HSSFColor.Black.Index;
            titleCellStyle.LeftBorderColor = HSSFColor.Black.Index;
            titleCellStyle.RightBorderColor = HSSFColor.Black.Index;
            titleCellStyle.TopBorderColor = HSSFColor.Black.Index;
            //设置字体
            titleCellStyle.SetFont(font);
            #endregion
            //表头
            IRow headRow = sheet.CreateRow(0);
            headRow.HeightInPoints = 25;

            headRow.CreateCell(0).SetCellValue("银行账户名称");
            headRow.CreateCell(1).SetCellValue("客户银行账号");
            headRow.CreateCell(2).SetCellValue("开户行");
            headRow.CreateCell(3).SetCellValue("开户地址");
            headRow.CreateCell(4).SetCellValue("状态");

            headRow.GetCell(0).CellStyle = titleCellStyle;
            headRow.GetCell(1).CellStyle = titleCellStyle;
            headRow.GetCell(2).CellStyle = titleCellStyle;
            headRow.GetCell(3).CellStyle = titleCellStyle;
            headRow.GetCell(4).CellStyle = titleCellStyle;

            sheet.SetColumnWidth(0, 15 * 256);
            sheet.SetColumnWidth(1, 20 * 256);
            sheet.SetColumnWidth(2, 20 * 256);
            sheet.SetColumnWidth(3, 20 * 256);
            sheet.SetColumnWidth(4, 20 * 256);

            if (dt != null)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    IRow row = sheet.CreateRow(i + 1);
                    row.HeightInPoints = 22;
                    row.CreateCell(0).SetCellValue(dt.Rows[i]["cb_bankName"].ToString());
                    row.CreateCell(1).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["cb_bankNum"]));
                    row.CreateCell(2).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["cb_bank"]));
                    row.CreateCell(3).SetCellValue(Utils.ObjectToStr(dt.Rows[i]["cb_bankAddress"]));
                    row.CreateCell(4).SetCellValue(Utils.StrToBool(Utils.ObjectToStr(dt.Rows[i]["cb_flag"]), false) ? "启用" : "禁用");

                    row.GetCell(0).CellStyle = cellStyle;
                    row.GetCell(1).CellStyle = cellStyle;
                    row.GetCell(2).CellStyle = cellStyle;
                    row.GetCell(3).CellStyle = cellStyle;
                    row.GetCell(4).CellStyle = cellStyle;
                }
            }

            MemoryStream file = new MemoryStream();
            hssfworkbook.Write(file);

            HttpContext.Current.Response.BinaryWrite(file.GetBuffer());
            HttpContext.Current.Response.End();
        }
    }
}