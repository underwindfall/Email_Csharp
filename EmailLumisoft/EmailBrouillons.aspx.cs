using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Email_Test.Email.bll;

namespace Email_Test
{
    public partial class EmailBrouillons : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                DataSet dt = EmailClass.getmailSaveTmp();
                gvmailBrouillons.DataSource = dt;
                gvmailBrouillons.DataBind();
                gvmailBrouillons.SelectedIndex = -1;
            }
        }

        protected void gvmailBrouillons_SelectedIndexChanged(object sender, EventArgs e)
        {
            Server.Transfer("EmailCorbeille.aspx");
        }

        protected void gvmailBrouillons_RowDeleted(object sender, GridViewDeletedEventArgs e)
        {

        }
    }
}