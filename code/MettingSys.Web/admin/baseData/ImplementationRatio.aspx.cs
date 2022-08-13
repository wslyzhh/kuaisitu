using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin.baseData
{
    public partial class ImplementationRatio : Web.UI.ManagePage
    {
        protected Model.manager manager = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                ShowInfo();
            }
        }
        #region 赋值操作=================================
        private void ShowInfo()
        {
            BLL.publicSetting bll = new BLL.publicSetting();
            Model.publicSetting model = bll.GetModel(1);
            if (model != null)
            {
                if (model.ps_isuse)
                {
                    cbIsUse.Checked = true;
                }
                else
                {
                    cbIsUse.Checked = false;
                }
                txtsDate.Text = model.ps_sdate.Value.ToString("yyyy-MM-dd");
                txteDate.Text = model.ps_edate==null?"": model.ps_edate.Value.ToString("yyyy-MM-dd");
                txtRatio.Text = model.ps_ratio.ToString();
            }
        }
        #endregion
        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            BLL.publicSetting bll = new BLL.publicSetting();
            Model.publicSetting model = bll.GetModel(1);
            manager = GetAdminInfo();
            string result = string.Empty;
            if (model == null)
            {
                model = new Model.publicSetting();
                model.ps_type = 1;
                model.ps_isuse = cbIsUse.Checked;
                model.ps_sdate = ConvertHelper.toDate(txtsDate.Text);
                model.ps_edate = ConvertHelper.toDate(txteDate.Text);
                model.ps_ratio = Utils.ObjToInt(txtRatio.Text, 0);
                result = bll.Add(model, manager);
            }
            else
            {
                string content = "";
                if (model.ps_isuse != cbIsUse.Checked)
                {
                    content += "是否启用:" + model.ps_isuse + "→<font color='red'>" + cbIsUse.Checked + "<font><br/>";
                }
                if (model.ps_sdate != ConvertHelper.toDate(txtsDate.Text))
                {
                    content += "开始执行日期起始日期:" + model.ps_sdate.Value.ToString("yyyy-MM-dd") + "→<font color='red'>" + txtsDate.Text + "<font><br/>";
                }
                if (model.ps_edate != ConvertHelper.toDate(txteDate.Text))
                {
                    content += "开始执行日期截止日期:" + (model.ps_edate == null ? "" : model.ps_edate.Value.ToString("yyyy-MM-dd")) + "→<font color='red'>" + txteDate.Text + "<font><br/>";
                }
                if (model.ps_ratio.ToString() != txtRatio.Text)
                {
                    content += "业绩总比例:" + model.ps_ratio + "→<font color='red'>" + txtRatio.Text + "<font><br/>";
                }
                model.ps_isuse = cbIsUse.Checked;
                model.ps_sdate = ConvertHelper.toDate(txtsDate.Text);
                model.ps_edate = ConvertHelper.toDate(txteDate.Text);
                model.ps_ratio = Utils.ObjToInt(txtRatio.Text, 0);
                result = bll.Update(model, manager, content);
            }
            if (!string.IsNullOrEmpty(result))            
            {
                JscriptMsg(result, "");
                return;
            }
            JscriptMsg("保存成功！", "ImplementationRatio.aspx");
        }
    }
}