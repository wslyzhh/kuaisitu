using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Text;
using System.Data;

namespace MettingSys.Web.admin.finance
{
    public partial class invoice_edit : Web.UI.ManagePage
    {
        protected string action = DTEnums.ActionEnum.Add.ToString();//操作类型
        protected int id = 0;
        protected string oID = "", cid = "", cusName = "";
        protected string fromOrder = "";//true时表示从订单页面添加

        protected Model.business_log logmodel = null;
        protected Model.manager manager = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            action = DTRequest.GetQueryString("action");
            this.oID = DTRequest.GetQueryString("oID");
            fromOrder = DTRequest.GetQueryString("fromOrder");
            cid = DTRequest.GetString("cid");
            cusName = DTRequest.GetString("cusname");

            //ChkAdminLevel("sys_invoice", action); //检查权限
            if (action == DTEnums.ActionEnum.Add.ToString())
            {
                ddlserviceName.Visible = true;
                txtserviceName.Visible = false;
                txtCusName.Text = cusName;
                hCusId.Value = cid;
            }
            if (!string.IsNullOrEmpty(action) && action == DTEnums.ActionEnum.Edit.ToString())
            {
                this.action = DTEnums.ActionEnum.Edit.ToString();//修改类型
                this.id = DTRequest.GetQueryInt("id");
                if (this.id == 0)
                {
                    JscriptMsg("传输参数不正确！", "back");
                    return;
                }
                if (!new BLL.invoices().Exists(this.id))
                {
                    JscriptMsg("记录不存在或已被删除！", "back");
                    return;
                }
            }
            if (action == DTEnums.ActionEnum.View.ToString())
            {
                this.id = DTRequest.GetQueryInt("id");
                btnSubmit.Visible = false;
            }
            manager = GetAdminInfo();
            if (!Page.IsPostBack)
            {
                InitData();
                if (action == DTEnums.ActionEnum.Edit.ToString() || action == DTEnums.ActionEnum.View.ToString()) //修改或查看
                {
                    ShowInfo(this.id);
                }
            }
        }
        #region 初始化数据=================================
        private void InitData()
        {
            ddlserviceType.DataSource = Common.BusinessDict.taxableType();
            ddlserviceType.DataTextField = "value";
            ddlserviceType.DataValueField = "key";
            ddlserviceType.DataBind();
            ddlserviceType.Items.Insert(0, new ListItem("请选择", ""));

            ddlsentWay.DataSource = Common.BusinessDict.sentMethod();
            ddlsentWay.DataTextField = "value";
            ddlsentWay.DataValueField = "key";
            ddlsentWay.DataBind();
            ddlsentWay.Items.Insert(0, new ListItem("请选择", ""));

            ddldarea.DataSource = new BLL.department().GetList(0, "de_type=1 and de_parentid<>0", "de_sort,de_id");
            ddldarea.DataTextField = "de_subname";
            ddldarea.DataValueField = "de_area";
            ddldarea.DataBind();
            ddldarea.Items.Insert(0, new ListItem("请选择", ""));
            ddldarea.SelectedValue = manager.area;//添加时默认开票区域为本区域

            ddlinvType.DataSource = Common.BusinessDict.invType();
            ddlinvType.DataTextField = "value";
            ddlinvType.DataValueField = "key";
            ddlinvType.DataBind();
            ddlinvType.Items.Insert(0, new ListItem("请选择", ""));
        }
        #endregion
        #region 赋值操作=================================
        private void ShowInfo(int _id)
        {
            btnAudit.Visible = false;
            BLL.invoices bll = new BLL.invoices();
            DataSet ds = bll.GetList(0, "inv_id=" + _id + "", "");
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                DataRow dr = ds.Tables[0].Rows[0];
                oID = dr["inv_oid"].ToString();
                txtCusName.Text = dr["c_name"].ToString();
                hCusId.Value = dr["inv_cid"].ToString();
                labCusName.Text = dr["c_name"].ToString();
                txtpurchaserName.Text = dr["inv_purchaserName"].ToString();
                txtpurchaserNum.Text = dr["inv_purchaserNum"].ToString();
                txtpurchaserAddress.Text = dr["inv_purchaserAddress"].ToString();
                txtpurchaserPhone.Text = dr["inv_purchaserPhone"].ToString();
                txtpurchaserBank.Text = dr["inv_purchaserBank"].ToString();
                txtpurchaserBankNum.Text = dr["inv_purchaserBankNum"].ToString();
                txtmoney.Text = dr["inv_money"].ToString();
                labOverMoney.Text = dr["inv_overmoney"].ToString();
                txtreceiveName.Text = dr["inv_receiveName"].ToString();
                txtreceivePhone.Text = dr["inv_receivePhone"].ToString();
                txtreceiveAddress.Text = dr["inv_receiveAddress"].ToString();
                txtremark.Text = dr["inv_remark"].ToString();
                Dictionary<byte?, string> stype = Common.BusinessDict.taxableType();
                byte? _type = 0;
                foreach (var item in stype.Keys)
                {
                    if (stype[item].ToString()==dr["inv_serviceType"].ToString())
                    {
                        ddlserviceType.SelectedValue = item.ToString();
                        _type = Utils.ObjToByte(item);
                        break;
                    }
                }
                if (_type == 4)
                {
                    ddlserviceName.Visible = false;
                    txtserviceName.Visible = true;
                    txtserviceName.Text = dr["inv_serviceName"].ToString();
                }
                else
                {
                    ddlserviceName.Visible = true;
                    txtserviceName.Visible = false;
                    Dictionary<byte?, string> sname = Common.BusinessDict.serviceName(_type);
                    ddlserviceName.DataSource = sname;
                    ddlserviceName.DataTextField = "value";
                    ddlserviceName.DataValueField = "key";
                    ddlserviceName.DataBind();
                    ddlserviceName.Items.Insert(0, new ListItem("请选择", ""));
                    foreach (var item in sname.Keys)
                    {
                        if (sname[item].ToString() == dr["inv_serviceName"].ToString())
                        {
                            ddlserviceName.SelectedValue = item.ToString();
                            break;
                        }
                    }
                }                
                Dictionary<byte?, string> smethod = Common.BusinessDict.sentMethod();
                foreach (var item in smethod.Keys)
                {
                    if (smethod[item].ToString() == dr["inv_sentWay"].ToString())
                    {
                        ddlsentWay.SelectedValue = item.ToString();
                        break;
                    }
                }
                ddldarea.SelectedValue = dr["inv_darea"].ToString();
                BindUnit(ddldarea.SelectedValue);
                ddlunit.SelectedValue = dr["inv_unit"].ToString();
                ddlinvType.SelectedValue = dr["inv_type"].ToString();

                if (((manager.area==dr["inv_farea"].ToString() || manager.area==dr["inv_darea"].ToString()) && new MettingSys.BLL.permission().checkHasPermission(manager, "0603")) || new MettingSys.BLL.permission().checkHasPermission(manager, "0402"))
                {
                    btnAudit.Visible = true;
                    ddlflag.DataSource = Common.BusinessDict.checkStatus();
                    ddlflag.DataTextField = "value";
                    ddlflag.DataValueField = "key";
                    ddlflag.DataBind();
                    ddlflag.Items.Insert(0, new ListItem("请选择", ""));

                    if (new MettingSys.BLL.permission().checkHasPermission(manager, "0603"))
                    {
                        if (dr["inv_farea"].ToString() == dr["inv_darea"].ToString())
                        {
                            if (dr["inv_flag1"].ToString()!="2")
                            {
                                ddlchecktype.SelectedValue = "1";
                                ddlflag.SelectedValue = dr["inv_flag1"].ToString();
                                txtCheckRemark.Text = dr["inv_checkRemark1"].ToString();
                            }
                            if (dr["inv_flag2"].ToString()=="2")
                            {
                                ddlchecktype.SelectedValue = "2";
                                ddlflag.SelectedValue = dr["inv_flag2"].ToString();
                                txtCheckRemark.Text = dr["inv_checkRemark2"].ToString();
                            }
                        }
                        else
                        {
                            if (manager.area == dr["inv_farea"].ToString())
                            {
                                ddlchecktype.SelectedValue = "1";
                                ddlflag.SelectedValue = dr["inv_flag1"].ToString();
                                txtCheckRemark.Text = dr["inv_checkRemark1"].ToString();
                            }
                            if (manager.area == dr["inv_darea"].ToString())
                            {
                                ddlchecktype.SelectedValue = "2";
                                ddlflag.SelectedValue = dr["inv_flag2"].ToString();
                                txtCheckRemark.Text = dr["inv_checkRemark2"].ToString();
                            }
                        }
                    }
                    if (new MettingSys.BLL.permission().checkHasPermission(manager, "0402"))
                    {
                        ddlchecktype.SelectedValue = "3";
                        ddlflag.SelectedValue = dr["inv_flag3"].ToString();
                        txtCheckRemark.Text = dr["inv_checkRemark3"].ToString();
                    }
                }

            }
            txtCusName.Visible = false;
            
        }
        #endregion
        #region 增加操作=================================
        private string DoAdd()
        {
            Model.invoices model = new Model.invoices();
            BLL.invoices bll = new BLL.invoices();
            
            manager = GetAdminInfo();
            model.inv_oid = oID;
            model.inv_cid = Utils.StrToInt(hCusId.Value,0);
            if (string.IsNullOrEmpty(ddlinvType.SelectedValue))
            {
                return "请选择专普票类型";
            }
            model.inv_type = Utils.StrToBool(ddlinvType.SelectedValue, false);
            model.inv_purchaserName = txtpurchaserName.Text.Trim();
            model.inv_purchaserNum = txtpurchaserNum.Text.Trim();
            model.inv_purchaserAddress = txtpurchaserAddress.Text.Trim();
            model.inv_purchaserPhone = txtpurchaserPhone.Text.Trim();
            model.inv_purchaserBank = txtpurchaserBank.Text.Trim();
            model.inv_purchaserBankNum = txtpurchaserBankNum.Text.Trim();
            model.inv_serviceType = ddlserviceType.SelectedItem.Text;
            if (ddlserviceType.SelectedValue == "4")
            {
                model.inv_serviceName = txtserviceName.Text;
            }
            else
            {
                model.inv_serviceName = ddlserviceName.SelectedItem.Text;
            }
            model.inv_money = Utils.StrToDecimal(txtmoney.Text.Trim(), 0);
            model.inv_sentWay = ddlsentWay.SelectedItem.Text;
            model.inv_farea = manager.area;
            model.inv_darea = ddldarea.SelectedValue;
            model.inv_unit = Utils.StrToInt(ddlunit.SelectedValue, 0);
            model.inv_receiveName = txtreceiveName.Text.Trim();
            model.inv_receivePhone = txtreceivePhone.Text.Trim();
            model.inv_receiveAddress = txtreceiveAddress.Text.Trim();
            model.inv_remark = txtremark.Text.Trim();
            model.inv_personName = manager.real_name;
            model.inv_personNum = manager.user_name;
            model.inv_addDate = DateTime.Now;
            model.inv_flag1 = 0;
            model.inv_flag2 = 0;
            model.inv_flag3 = 0;
            model.inv_isConfirm = false;
            return bll.Add(model, manager);
        }
        #endregion
        #region 修改操作=================================
        private string DoEdit(int _id)
        {
            BLL.invoices bll = new BLL.invoices();
            Model.invoices model = bll.GetModel(_id);     
            manager = GetAdminInfo();
            string _content = "";
            if (model.inv_purchaserName != txtpurchaserName.Text.Trim())
            {
                _content += "购买方名称：" + model.inv_purchaserName + "→<font color='red'>" + txtpurchaserName.Text.Trim() + "</font><br/>";
            }
            model.inv_purchaserName = txtpurchaserName.Text.Trim();
            if (model.inv_purchaserNum != txtpurchaserNum.Text.Trim())
            {
                _content += "购买方纳税人识别号：" + model.inv_purchaserNum + "→<font color='red'>" + txtpurchaserNum.Text.Trim() + "</font><br/>";
            }
            model.inv_purchaserNum = txtpurchaserNum.Text.Trim();
            if (model.inv_purchaserAddress != txtpurchaserAddress.Text.Trim())
            {
                _content += "购买方纳税人识别号：" + model.inv_purchaserAddress + "→<font color='red'>" + txtpurchaserAddress.Text.Trim() + "</font><br/>";
            }
            model.inv_purchaserAddress = txtpurchaserAddress.Text.Trim();
            if (model.inv_purchaserPhone != txtpurchaserPhone.Text.Trim())
            {
                _content += "购买方地址：" + model.inv_purchaserPhone + "→<font color='red'>" + txtpurchaserPhone.Text.Trim() + "</font><br/>";
            }
            model.inv_purchaserPhone = txtpurchaserPhone.Text.Trim();
            if (model.inv_purchaserBank != txtpurchaserBank.Text.Trim())
            {
                _content += "购买方电话：" + model.inv_purchaserBank + "→<font color='red'>" + txtpurchaserBank.Text.Trim() + "</font><br/>";
            }
            model.inv_purchaserBank = txtpurchaserBank.Text.Trim();
            if (model.inv_purchaserBankNum != txtpurchaserBankNum.Text.Trim())
            {
                _content += "购买方开户行：" + model.inv_purchaserBankNum + "→<font color='red'>" + txtpurchaserBankNum.Text.Trim() + "</font><br/>";
            }
            model.inv_purchaserBankNum = txtpurchaserBankNum.Text.Trim();
            if (model.inv_serviceType != ddlserviceType.SelectedItem.Text)
            {
                _content += "应税劳务：" + model.inv_serviceType + "→<font color='red'>" + ddlserviceType.SelectedItem.Text + "</font><br/>";
            }
            model.inv_serviceType = ddlserviceType.SelectedItem.Text;
            if (ddlserviceType.SelectedValue == "4")
            {
                if (model.inv_serviceName != txtserviceName.Text)
                {
                    _content += "服务名称：" + model.inv_serviceName + "→<font color='red'>" + txtserviceName.Text + "</font><br/>";
                }
                model.inv_serviceName = txtserviceName.Text;
            }
            else
            {
                if (model.inv_serviceName != ddlserviceName.SelectedItem.Text)
                {
                    _content += "服务名称：" + model.inv_serviceName + "→<font color='red'>" + ddlserviceName.SelectedItem.Text + "</font><br/>";
                }
                model.inv_serviceName = ddlserviceName.SelectedItem.Text;
            }            
            
            if (string.IsNullOrEmpty(ddlinvType.SelectedValue))
            {
                return "请选择专普票类型";
            }
            if (model.inv_type != Utils.StrToBool(ddlinvType.SelectedValue, false))
            {
                _content += "专普票：" + BusinessDict.invType()[model.inv_type] + "→<font color='red'>" + BusinessDict.invType()[Utils.StrToBool(ddlinvType.SelectedValue, false)] + "</font><br/>";
            }
            model.inv_type = Utils.StrToBool(ddlinvType.SelectedValue, false);
            decimal _money = 0;
            if (!decimal.TryParse(txtmoney.Text.Trim(), out _money))
            {
                return "请正确填写开票金额";
            }
            if (model.inv_money != _money)
            {
                _content += "开票金额：" + model.inv_money + "→<font color='red'>" + _money + "</font><br/>";
            }
            model.inv_money = _money;
            if (model.inv_sentWay != ddlsentWay.SelectedItem.Text)
            {
                _content += "送票方式：" + model.inv_sentWay + "→<font color='red'>" + ddlsentWay.SelectedItem.Text + "</font><br/>";
            }
            model.inv_sentWay = ddlsentWay.SelectedItem.Text;
            if (model.inv_darea != ddldarea.SelectedValue)
            {
                _content += "开票区域：" + model.inv_darea + "→<font color='red'>" + ddldarea.SelectedValue + "</font><br/>";
            }
            model.inv_darea = ddldarea.SelectedValue;
            if (model.inv_unit != Utils.StrToInt(ddlunit.SelectedValue,0))
            {
                _content += "开票单位：" + model.inv_unit + "→<font color='red'>" + ddlunit.SelectedValue + "</font><br/>";
            }
            model.inv_unit = Utils.StrToInt(ddlunit.SelectedValue, 0);
            if (model.inv_receiveName != txtreceiveName.Text.Trim())
            {
                _content += "收票人名称：" + model.inv_receiveName + "→<font color='red'>" + txtreceiveName.Text.Trim() + "</font><br/>";
            }
            model.inv_receiveName = txtreceiveName.Text.Trim();
            if (model.inv_receivePhone != txtreceivePhone.Text.Trim())
            {
                _content += "收票人电话：" + model.inv_receivePhone + "→<font color='red'>" + txtreceivePhone.Text.Trim() + "</font><br/>";
            }
            model.inv_receivePhone = txtreceivePhone.Text.Trim();
            if (model.inv_receiveAddress != txtreceiveAddress.Text.Trim())
            {
                _content += "收票人地址：" + model.inv_receiveAddress + "→<font color='red'>" + txtreceiveAddress.Text.Trim() + "</font><br/>";
            }
            model.inv_receiveAddress = txtreceiveAddress.Text.Trim();
            if (model.inv_remark != txtremark.Text.Trim())
            {
                _content += "备注：" + model.inv_remark + "→<font color='red'>" + txtremark.Text.Trim() + "</font><br/>";
            }
            model.inv_remark = txtremark.Text.Trim();
            return bll.Update(model, _content, manager);
        }

        
        #endregion

        //保存
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string result = "";
            if (action == DTEnums.ActionEnum.Edit.ToString()) //修改
            {
                //ChkAdminLevel("sys_invoice", DTEnums.ActionEnum.Edit.ToString()); //检查权限
                result = DoEdit(this.id);
                if (result != "")
                {
                    PrintJscriptMsg(result, "");
                    return;
                }
                if (fromOrder == "true")
                {
                    PrintMsg("修改发票成功！");
                }
                else
                {
                    JscriptMsg("修改发票成功！", "invoice_list.aspx");
                }
            }
            else //添加
            {
                //ChkAdminLevel("sys_invoice", DTEnums.ActionEnum.Add.ToString()); //检查权限
                result = DoAdd();
                if (result != "")
                {
                    PrintJscriptMsg(result, "");
                    return;
                }
                if (fromOrder == "true")
                {
                    PrintMsg("添加发票成功！");
                }
                else
                {
                    JscriptMsg("添加发票成功！", "invoice_list.aspx");
                }
            }
        }


        protected void ddlserviceType_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            if (!string.IsNullOrEmpty(ddl.SelectedValue))
            {
                if (ddl.SelectedValue == "4")
                {
                    ddlserviceName.Visible = false;
                    txtserviceName.Visible = true;
                }
                else 
                {
                    ddlserviceName.Visible = true;
                    txtserviceName.Visible = false;
                }
                ddlserviceName.DataSource = Common.BusinessDict.serviceName(Utils.ObjToByte(ddl.SelectedValue));
                ddlserviceName.DataTextField = "value";
                ddlserviceName.DataValueField = "key";
                ddlserviceName.DataBind();
                ddlserviceName.Items.Insert(0, new ListItem("请选择", ""));
            }
        }

        protected void ddldarea_SelectedIndexChanged(object sender, EventArgs e)
        {
            DropDownList ddl = (DropDownList)sender;
            if (!string.IsNullOrEmpty(ddl.SelectedValue))
            {
                BindUnit(ddl.SelectedValue);
            }
        }

        private void BindUnit(string area)
        {
            DataTable dt = new BLL.invoiceUnit().getUnitbyArea(area);
            ddlunit.DataSource = dt;
            ddlunit.DataTextField = "invU_name";
            ddlunit.DataValueField = "invU_id";
            ddlunit.DataBind();
            ddlunit.Items.Insert(0, new ListItem("请选择", ""));
        }
    }
}