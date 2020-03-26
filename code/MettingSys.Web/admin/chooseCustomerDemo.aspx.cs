using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MettingSys.Web.admin
{
    public partial class chooseCustomerDemo : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            autocomplete.Text = "Andorra";
        }
    }
}