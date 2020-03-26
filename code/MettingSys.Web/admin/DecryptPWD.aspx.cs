using MettingSys.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin
{
    public partial class DecryptPWD : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            string p1 = pwd1.Text.Trim();
            string s1 = salt.Text.Trim();
            if (!string.IsNullOrEmpty(p1) && !string.IsNullOrEmpty(s1))
            {
                pwd2.Text = DESEncrypt.Decrypt(p1, s1);
            }
        }
    }
}