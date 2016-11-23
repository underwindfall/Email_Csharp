using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Email_Test.Email.bll;
using Email_Test.Email.dll;


namespace Email_Test
{
    public partial class Login : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void btnLogin_Click(object sender, EventArgs e)
        {
            string login = tblogin.Text;
            string pass = tbmdp.Text;
            Label lblerror = (Label)Master.FindControl("lblerror");
            if (EmailClass.IsUser(login, pass))
            {
              
                Response.Redirect("Default.aspx");
                
            }
            else 
            {
                Response.Write("ERROR");
            }
        }
    }
}