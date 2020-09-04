using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web
{
    public partial class DecryptPassword : System.Web.UI.Page
    {
        protected string _password = "", _salt = "";
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            labpassword.Text = DESEncrypt.Decrypt(txtPassword.Text.Trim(), txtSalt.Text.Trim());
        }
    }
}