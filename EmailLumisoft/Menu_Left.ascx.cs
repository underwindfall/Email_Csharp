using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Email_Test
{
    public partial class Menu_Left : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //System.Web.HttpFileCollection _file = System.Web.HttpContext.Current.Request.Files;
            ////ScriptManager.RegisterStartupScript(Page, typeof(string), "Sort", "updateJs()", true);
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
              System.Web.HttpFileCollection _file1 = System.Web.HttpContext.Current.Request.Files;

               
            int i = 90;
            i++;
        }
    }
}