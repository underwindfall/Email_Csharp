using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Email_Test
{
    public partial class icon : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)// j'ai fait les règles pour afficher les images de peintures qui va afficher
        {
            string nmfile = Request.QueryString["namefile"];
            string UploadURL = Server.MapPath("~/upload/");
            if (!string.IsNullOrEmpty(nmfile))
            {
                Size size = new Size(16, 16);
                Icon icon = Icon.ExtractAssociatedIcon(UploadURL + nmfile);
                Bitmap bmap = new Bitmap(icon.ToBitmap());
                MemoryStream ms = new MemoryStream();
                bmap.Save(ms, ImageFormat.Png);
                Response.ClearContent();
                Response.ContentType = "image/png";
                Response.BinaryWrite(ms.ToArray());
                Response.Flush();
                ms.Dispose();
            }

        }
    }
}