using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Email_Test
{
    public partial class WebUserControl1 : System.Web.UI.UserControl
    {

        public class Emaildatastatic
        {
            private static Emaildatastatic dts;
            private DataTable dt;           
            public System.Web.HttpFileCollection _file;
            public string mailSend;
            private Emaildatastatic()
            {
                dt = new DataTable();
                mailSend = "UNSEND";
          
             
                if (dt.Columns.Count == 0)
                {
                    dt.Columns.Add(new DataColumn("iconpath", typeof(string)));
                    dt.Columns.Add(new DataColumn("iconName", typeof(string)));
                    dt.Columns.Add(new DataColumn("filepath", typeof(string)));
                }
              
            }
            public static Emaildatastatic getInstance()
            {
                if (dts == null)
                {
                    dts = new Emaildatastatic();
                }
                return dts;
            }
            public void supprdatalist()//supprimer la list de table
            {
                dt.Rows.Clear();
            }
            public DataTable getDataTable()//retourer dan ce table
            {
                return this.dt;

            }
            public bool getTest()
            {
                return dts == null;
            }
          
        }

        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                Emaildatastatic.getInstance();
                Emaildatastatic.getInstance()._file = System.Web.HttpContext.Current.Request.Files;
            }
            else
            {


            }
            this.FileUpload1.Attributes.Add("onchange", "EquFileUpload_OnChange()");
        }
        private bool IsAllowedExtension(string minefile)//permis quel type de ficher peevent teledeposer
        {
            bool result = true;
            string filename = minefile;
            if (filename == ".xls" || filename == ".xlsx")
            {
                result = true;
            }
            return result;
        }
        protected void Btn_upFile_Click(object sender, EventArgs e)//l'action de Button Upload
        {

            int nbrFile = FileUpload1.PostedFiles.Count;
            string UploadURL = Server.MapPath("~/upload/");
            if (FileUpload1.HasFile)
            {
                for (int i = 0; i < nbrFile; i++)
                {
                    if (IsAllowedExtension(FileUpload1.PostedFiles[i].ContentType.ToLower()))
                    {
                        string filename = FileUpload1.PostedFiles[i].FileName.ToString();
                        try
                        {
                            if (!System.IO.Directory.Exists(UploadURL))
                            {
                                System.IO.Directory.CreateDirectory(UploadURL);
                            }
                            FileInfo file = new FileInfo(UploadURL + filename);
                            if (file.Exists)
                            {
                                file.Attributes = FileAttributes.Normal;
                                file.Delete();
                            }
                            FileUpload1.PostedFiles[i].SaveAs(UploadURL + filename);
                            DataRow dr = Emaildatastatic.getInstance().getDataTable().NewRow();
                            dr["iconpath"] = "icon.aspx?namefile=" + filename;
                            dr["iconName"] = filename;
                            dr["filepath"] = UploadURL + filename;
                            Emaildatastatic.getInstance().getDataTable().Rows.Add(dr);

                        }
                        catch
                        {
                            Response.Write("error");
                        }
                    }
                    else
                    {

                    }
                }
                dtlImage.DataSource = Emaildatastatic.getInstance().getDataTable();
                dtlImage.DataBind();
                dtlImage.DataSource = null;
            }
            else if (Emaildatastatic.getInstance()._file.Count > 0)
            {

               


                for (int k = 0; k < Emaildatastatic.getInstance()._file.Count; k++)
                {
                    if (IsAllowedExtension(Emaildatastatic.getInstance()._file[k].ContentType.ToLower()) && !string.IsNullOrEmpty(Emaildatastatic.getInstance()._file[k].FileName))
                    {
                        string filename = Emaildatastatic.getInstance()._file[k].FileName.ToString();
                        try
                        {
                            if (!System.IO.Directory.Exists(UploadURL))
                            {
                                System.IO.Directory.CreateDirectory(UploadURL);
                            }
                            FileInfo file = new FileInfo(UploadURL + filename);
                            if (file.Exists)
                            {
                                file.Attributes = FileAttributes.Normal;
                                file.Delete();
                            }

                            Emaildatastatic.getInstance()._file[k].SaveAs(UploadURL + filename);
                            if (Emaildatastatic.getInstance().getDataTable().Select("filepath='@filepath'".Replace("@filepath", UploadURL + filename)).Length < 1)
                            {
                                DataRow dr = Emaildatastatic.getInstance().getDataTable().NewRow();
                                dr["iconpath"] = "icon.aspx?namefile=" + filename;
                                dr["iconName"] = filename;
                                dr["filepath"] = UploadURL + filename;
                                Emaildatastatic.getInstance().getDataTable().Rows.Add(dr);
                            }
                        }
                        catch(Exception ex)
                        {
                            Response.Write("error "+ex.StackTrace);
                        }
                    }
                } 
                dtlImage.DataSource = Emaildatastatic.getInstance().getDataTable();
                dtlImage.DataBind();
                dtlImage.DataSource = null;
            }

        }

        public void DeleteFolder(string dir)//fonction de supprimer un ficher
        {
            if (Directory.Exists(dir)) //   Si cet dossier existe on va supprimer
            {

                foreach (string d in Directory.GetFileSystemEntries(dir))
                {
                    if (File.Exists(d))

                        File.Delete(d); //  on supprimer cet ficher directement                         
                    else
                        DeleteFolder(d); //  //utiliser un bool pour supprimer sous-dossier
                }
            }
        }

        protected void Btn_anulle_Click(object sender, EventArgs e)//action de Button Annuler
        {
            DeleteFolder(Server.MapPath("~/upload"));
            Emaildatastatic.getInstance().supprdatalist();
        }

        protected void imgBtnSuppr_Click(object sender, ImageClickEventArgs e)
        {

        }

        protected void dtlImage_DeleteCommand(object source, DataListCommandEventArgs e)//supprimer les images 
        {
            string name = ((Label)e.Item.FindControl("lblFile")).Text;
            string UploadURL = Server.MapPath("~/upload/");
            FileInfo file = new FileInfo(UploadURL + name);
            if (file.Exists)
            {
                file.Attributes = FileAttributes.Normal;
                file.Delete();
            }
            if (Emaildatastatic.getInstance().getDataTable().Rows.Count > 0)
            {
                Emaildatastatic.getInstance().getDataTable().Rows.RemoveAt(e.Item.ItemIndex);
            }
            dtlImage.DataSource = Emaildatastatic.getInstance().getDataTable();
            dtlImage.DataBind();
            dtlImage.DataSource = null;
        }
        protected void dtlImage_databind()
        {

        }

        protected void dtlImage_ItemDataBound(object sender, DataListItemEventArgs e)//fonction pour mettre les informations d'image dans ce tool
        {
            for(int i=0;i< dtlImage.Items.Count; i++)
            { 
            Label lblFile = dtlImage.Items[i].FindControl("lblFile") as Label;
             if (lblFile != null)
            {
                    string lbl = lblFile.Text;
                if (lbl.Length > 30) {
                    lblFile.Text = lbl.Substring(0, 20)+"..."+ lbl.Substring(lbl.Length-5,5);
                    }
            }
           }
        }
    }
}