using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Data;
using System.Windows.Forms;
using System.Text;
using System.Net;
using System.Data.SqlClient;
using System.Drawing;
using LumiSoft.Net.MIME;
using Email_Test.Email.bll;

namespace Email_Test
{
    public partial class Default : System.Web.UI.Page
    {

        public enum ViewEtat { ViewReceive, ViewSend, ViewImport, ViewSuppr, ViewBrouillon, ViewRechercher };//séparer les états par 6 partis ,dans le menu  il y a 5 boutons représenter les 5 états,la dernière état est fonction recherche
        public static ViewEtat flagViewLast = ViewEtat.ViewReceive;//je vais mémoriser tous les états anciens pour quelques fonctions suiviants(dans l'action Bouton Return du Région ViewDetail )
        EmailClass rm = new EmailClass();
        
        protected void Page_Load(object sender, EventArgs e)
        { 

        }
        protected void registrationJs()
        {
            ScriptManager.RegisterStartupScript(Page, typeof(string), "Sort", "updateJs()", true);
        }

        protected void UpdatePanel_MenuLeft_Load(object sender, EventArgs e)
        {
            ScriptManager.RegisterStartupScript(Page, typeof(string), "Sort", "load()", true);
        }//pour mis à jour le menu à gauche

        #region MenuLeftButtons_END
        //quand on clique les boutons il va lancer le page dans l'interface on a choisi 
        protected void btnEmailNouveau_Click(object sender, EventArgs e)
        {
              this.MultiView1.SetActiveView(ViewEcrire);
        }
        protected void btnEmailRecu_Click(object sender, EventArgs e)
        {
            flagViewLast = ViewEtat.ViewReceive;
            this.MultiView1.SetActiveView(ViewPrincipal);
        }
        protected void btnEmailEnvoye_Click(object sender, EventArgs e)
        {
            flagViewLast = ViewEtat.ViewSend;
            this.MultiView1.SetActiveView(ViewSend);
        }
        protected void btnEmailBrouillons_Click(object sender, EventArgs e)
        {
            this.MultiView1.SetActiveView(ViewBrouillons);
        }
        protected void btnEmailCorbeille_Click(object sender, EventArgs e)
        {
            this.MultiView1.SetActiveView(ViewCorbeille);
        }

        #endregion//quand on clique les boutons il va lancer le page dans l'interface on a choisi

        #region ViewEmailWrite_END
        //Je utilise plusieurs des méthodes dans le ToolFileUpload ce qui est utilisé pour planifier tous les formats de emaild'ecrit
      
        //si  on clique le bouton SEND ,il va envoyer un émail et lancer le méthode pour sauvegarder cet email dans la boîte envoyé
        //si  on clique le bouton SAVE, il va stocker email dans le méthode  ce qui est écrit dans le ToolFileUpload
        protected void ViewEcrire_Deactivate(object sender, EventArgs e)//END_
      
        {
           if (WebUserControl1.Emaildatastatic.getInstance().mailSend.Equals("SAVE"))
            {
                WebUserControl1.Emaildatastatic.getInstance().mailSend = "SAVE";//  WebUserControl1.Emaildatastatic.getInstance() c'est pour créer un nouveau objet 
            }
           else if (WebUserControl1.Emaildatastatic.getInstance().mailSend.Equals("SAVEBrouillons") )
            {
                WebUserControl1.Emaildatastatic.getInstance().mailSend = "UNSEND";
                EmailClass.cancelRemettreEmail();
            }
            else if(!WebUserControl1.Emaildatastatic.getInstance().mailSend.Equals("SEND"))
            {
            WebUserControl1.Emaildatastatic.getInstance().mailSend = "SAVE";
            EmailClass.saveMailTmp(tbxAdresseRecu.Text, tbxAdresseCC.Text, tbxSujet.Text, _editorHtml.Value);
            }
            else
            {
                WebUserControl1.Emaildatastatic.getInstance().mailSend = "UNSEND";
            }
            videDataEmailWrite();
        }  // quand on ferme la page qu'on a écrit, il va diviser 4 conditions
        protected void remettreEmailWrite()//END_ cet méthode est utilisé de rétablir les émail ce qui sont stockés dans le brouillon 
        {
            DataSet ds = EmailClass.getmailSaveBrouillons();
            DataRow dr = ds.Tables[0].Rows[0];
            //function 
            tbxAdresseRecu.Text = dr["mailto"].ToString();
            tbxAdresseCC.Text = dr["mailcc"].ToString(); ;
            tbxSujet.Text = dr["mailsubject"].ToString();
            tbxContent.Text = HttpUtility.UrlDecode(dr["mailbody"].ToString());
            _editorHtml.Value = tbxContent.Text;
            string listattam = dr["pathAttachmentFile"].ToString();
            DataTable dta = WebUserControl1.Emaildatastatic.getInstance().getDataTable();
            dta.Rows.Clear();
            string[] tableauattam = listattam.Split(';');
            int countfile = tableauattam.Length;
            string UploadURL = Server.MapPath("~/upload/");
            for (int i = 0; i < countfile - 1; i++)
            {
                DataRow dra = dta.NewRow();
                int index = tableauattam[i].LastIndexOf('\\');
                string filename = tableauattam[i].Substring(index + 1);
                dra["iconpath"] = "icon.aspx?namefile=" + filename;
                dra["iconName"] = filename;
                dra["filepath"] = UploadURL + filename;
                dta.Rows.Add(dra);
            }
            DataList dtlImage = ToolFileUpload.FindControl("dtlImage") as DataList;
            dtlImage.DataSource = dta;
            dtlImage.DataBind();
            dtlImage.DataSource = null;
            messagehid.Value = "fasle";
        }
        protected void btnHid_Click(object sender, EventArgs e)//END_cet méthode est utilisé de rétablir les émail  ce qui  sont stockés dans le Mailtmp.MailTmp est un table qui sauvegarder les donnés d'émail qui on a écrit dans le page de EcritMail, mais on ne clique aucun bouton.L'email va sauvegarder automatiquement dans cet page.
        {
           if( messagehid.Value.ToLower() == "true")
            {
                DataSet ds = EmailClass.getmailSaveTmp();
                DataRow dr = ds.Tables[0].Rows[0];
                //function 
                tbxAdresseRecu.Text = dr["mailto"].ToString();
                tbxAdresseCC.Text = dr["mailcc"].ToString(); ;
                tbxSujet.Text = dr["mailsubject"].ToString();
                tbxContent.Text = HttpUtility.UrlDecode(dr["mailbody"].ToString());
                _editorHtml.Value = tbxContent.Text;
                string listattam = dr["pathAttachmentFile"].ToString();
                DataTable dta = WebUserControl1.Emaildatastatic.getInstance().getDataTable();
                dta.Rows.Clear();
                string[] tableauattam = listattam.Split(';');
                int countfile = tableauattam.Length;
                string UploadURL = Server.MapPath("~/upload/");
                for (int i = 0; i < countfile - 1; i++)
                {
                    DataRow dra = dta.NewRow();
                    int index = tableauattam[i].LastIndexOf('\\');
                    string filename = tableauattam[i].Substring(index + 1);
                    dra["iconpath"] = "icon.aspx?namefile=" + filename;
                    dra["iconName"] = filename;
                    dra["filepath"] = UploadURL + filename;
                    dta.Rows.Add(dra);
                }
                DataList dtlImage = ToolFileUpload.FindControl("dtlImage") as DataList;
                dtlImage.DataSource = dta;
                dtlImage.DataBind();
                dtlImage.DataSource = null;
                messagehid.Value = "fasle";
            }
            
        }
        protected void ViewEcrire_Activate(object sender, EventArgs e)//END_quand on lance la page d'écrit,il y deux conditions,si on ne sauvegarder pas l'émail dans le MailTmp,il va lancer directement .Sinon, il va afficher une phrase  Vous voulez continuer à écrire le message dernier?
        {
            videDataEmailWrite();
            if (WebUserControl1.Emaildatastatic.getInstance().mailSend.Equals("SAVE"))
            {
                string js = string.Format("document.getElementById('{0}').value=confirm('Vous voulez continuer à écrire le message dernier?');document.getElementById('{1}').click();", messagehid.ClientID, btnHid.ClientID);
                ClientScript.RegisterStartupScript(GetType(), "confirm", js, true);
              
                //
            }
            else if (WebUserControl1.Emaildatastatic.getInstance().mailSend.Equals("SAVEBrouillons"))
                {
                remettreEmailWrite();
                 }
        }
        protected void btnEnvoyer_Click(object sender, EventArgs e)//END_ quand on clique le bouton Envoyer,il va envoyer l'email à serveur.En même temps, il va diviser en 2 partis, il va supprimer l'email ce qui soit dans le brouillon soit dans le MailTmp
        {
            
            rm.sentmails("test@sipcom.fr", "test123", 25, "auth.smtp.1and1.fr", true, tbxAdresseRecu.Text, tbxAdresseCC.Text, tbxSujet.Text, _editorHtml.Value,ddlPriorite.SelectedIndex);
            
            WebUserControl1.Emaildatastatic.getInstance().supprdatalist();
            if (WebUserControl1.Emaildatastatic.getInstance().mailSend.Equals("SAVEBrouillons"))
            {
                rm.deleteMailBrouillonsSended();
            }
            else if (WebUserControl1.Emaildatastatic.getInstance().mailSend.Equals("SAVE"))
            {
                rm.deleteMailSaveTmpSended();
            }
            WebUserControl1.Emaildatastatic.getInstance().mailSend = "SEND";
            this.MultiView1.SetActiveView(ViewPrincipal);
           
        }
        protected void btnAnnuler_Click(object sender, EventArgs e)//END_
        {
            WebUserControl1.Emaildatastatic.getInstance().mailSend = "SEND";
            this.MultiView1.SetActiveView(ViewPrincipal);
        }// l'action de bouton d'Anuuler, il va transférer dans le Page_Principal ,c'est-à-dire la boîte reçu 

        protected void btnEnregistre_Click(object sender, EventArgs e)//END_
        {
            WebUserControl1.Emaildatastatic.getInstance().mailSend = "SAVEBrouillons";
            EmailClass.saveMailBrouillons( tbxAdresseRecu.Text,tbxAdresseCC.Text,tbxSujet.Text,_editorHtml.Value);
            videDataEmailWrite();
        } //l'action de bouton d'Anuuler,il va stocker les émails dans le boîte brouillon

        #endregion

        #region ViewPrincipale 
        protected void ViewPrincipal_Activate(object sender, EventArgs e)//END_
        {
            rm.getMailImap("test@sipcom.fr", "test123", "imap.1and1.fr", true, 993);
            DataTable dt = rm.getMailList().Tables[0];
            string countmailNoseen = dt.Select(@"NotSeen=0").Length.ToString();
            lblnbrmailNoseen.Text = countmailNoseen;
            int countmail = dt.Rows.Count;
            lblnbrmail.Text = countmail.ToString();
            gvViewEmail.DataSource = dt;
            gvViewEmail.DataBind();
            gvViewEmail.SelectedIndex = -1;
            flagViewLast = ViewEtat.ViewReceive;
        }//l'action de l'activation de ViewPrincipal ,il affiche le nombre  total de Emailreçu et Email ce qui n'est pas  encore lu 
        protected void btnactualiser_Click(object sender, EventArgs e)//END_
        {
            ViewPrincipal_Activate(null, null);
        }//l'action de faire actualiser 
        protected void btnsuppr_Click(object sender, EventArgs e)//END_
        {
            foreach (GridViewRow gr in gvViewEmail.Rows)
            {
                System.Web.UI.WebControls.CheckBox cbx = gr.FindControl("checkSelect") as System.Web.UI.WebControls.CheckBox;
                if (cbx.Checked)
                {
                    string mailid = gvViewEmail.DataKeys[gr.RowIndex].Value.ToString();
                    try
                    {
                        rm.deleteMail(mailid);                      
                        //rm.supprMailIMAP("zhangziyu830@gmail.com", "zhang882451272", "imap.gmail.com", true, 993, mailid, mailid);                       
                    }
                    catch
                    {

                    }
                }
            }
            DataTable dt = rm.getMailList().Tables[0];
            int countmail = dt.Rows.Count;
            lblnbrmail.Text = countmail.ToString();
            string countmailNoseen = rm.getMailList().Tables[0].Select(@"NotSeen=0").Length.ToString();
            lblnbrmailNoseen.Text = countmailNoseen;
            gvViewEmail.DataSource = dt;
            gvViewEmail.DataBind();
        }//utilise cet action pour supprimer  le Checkbox qu'on a choisi 

        #region gvViewEmail
        protected void gvViewEmail_RowDeleting(object sender, GridViewDeleteEventArgs e)////END_l'action de supprimer la ligne de GridView_gvViewEmail, et le bouton Suppr utilise aussi cet méthode 
        {
            string mailid = gvViewEmail.DataKeys[e.RowIndex].Value.ToString();
            try
            {
                rm.deleteMail(mailid);            
               
                rm.supprMailIMAP("test@sipcom.fr", "test123", "imap.1and1.fr", true, 993, mailid, mailid);//supprimer l'email dans le serveur 
                
            }
            catch
            {
            }
            DataTable dt = rm.getMailList().Tables[0];
            int countmail = dt.Rows.Count;// mis à jour de nombre de Emailreçu et Email n'est pas encore lu 
            lblnbrmail.Text = countmail.ToString();
            string countmailNoseen = rm.getMailList().Tables[0].Select(@"NotSeen=0").Length.ToString();
            lblnbrmailNoseen.Text = countmailNoseen;
            gvViewEmail.DataSource = dt;
            gvViewEmail.DataBind();

        }
        protected void gvViewEmail_PageIndexChanging(object sender, GridViewPageEventArgs e)//END_
        {
            gvViewEmail.PageIndex = e.NewPageIndex;
            gvViewEmail.SelectedIndex = -1;
            gvViewEmail.DataSource = rm.getMailList().Tables[0];
            gvViewEmail.DataBind();
        }//l'action de changer le page 
        protected void gvViewEmail_SelectedIndexChanged(object sender, EventArgs e)//l'action de choisi  un ligne de émail ,et le bouton Detail utilise aussi cet méthode 
        {
            this.MultiView1.SetActiveView(ViewDetail);
            string id = gvViewEmail.DataKeys[gvViewEmail.SelectedIndex].Value.ToString();
            int testid = Convert.ToInt32(gvViewEmail.DataKeys[gvViewEmail.SelectedIndex].Value);
            EmailClass.markMailAsSeen(id);
            DataTable dt = rm.getMailList(id).Tables[0];
            foreach (DataRow testrow in dt.Rows)
            {
                string testidstr = testid.ToString();
                if (testidstr == testrow["mailId"].ToString())
                {
                    lblsubject.Text = testrow["mailsubject"].ToString();//quand on clique le bouton Detail,on va mémoriser tous les détail(@destinateur  ,@émetteur,Sujet，Le Contenu  ) de cet émail 
                    lblsender.Text = testrow["mailsender"].ToString();
                    lblfrom.Text = testrow["mailfrom"].ToString();
                    lbldate.Text = testrow["maildateTime"].ToString();
                    lblto.Text = testrow["mailto"].ToString();
                    lblcc.Text = testrow["mailcc"].ToString();
                    divcontenu.InnerHtml = testrow["mailbody"].ToString();
                    //ajouter des attachments (ci-joint)
                    string listattam = testrow["pathAttachmentFile"].ToString();

                    DataTable dta = new DataTable();
                    dta.Columns.Add(new DataColumn("pathfile", typeof(string)));
                    string[] tableauattam = listattam.Split(';');
                    int countfile = tableauattam.Length;
                    for (int i = 0; i < countfile - 1; i++)
                    {
                        DataRow dra = dta.NewRow();
                        int index = tableauattam[i].LastIndexOf('\\');
                        string filename = tableauattam[i].Substring(index + 1);
                        dra["pathfile"] = filename;
                        dta.Rows.Add(dra);
                    }
                    dtlAttatement.DataSource = dta;
                    dtlAttatement.DataBind();
                    dtlAttatement.DataSource = null;
                }
            }  

        }
        protected void gvViewEmail_RowDataBound(object sender, GridViewRowEventArgs e)////END_l'action de mettre les informations de Base de donnés à chaque ligne de le tableau 
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {//change le couleur
                if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate || e.Row.RowState == DataControlRowState.Selected)
                {
                    e.Row.Attributes.Add("onmouseover", "c=this.style.backgroundColor;this.style.backgroundColor='#99CCFF'");
                    e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=c");
                }
                string tmp = "";
                e.Row.Cells[1].Text = HttpUtility.HtmlDecode(e.Row.Cells[1].Text);
                e.Row.Cells[2].Text = HttpUtility.HtmlDecode(e.Row.Cells[2].Text);
                if (string.IsNullOrWhiteSpace(e.Row.Cells[1].Text))
                {
                    e.Row.Cells[1].Text = e.Row.Cells[2].Text;
                    e.Row.Cells[1].ToolTip = e.Row.Cells[2].Text;
                }
                else
                {
                    e.Row.Cells[1].ToolTip = e.Row.Cells[1].Text + " " + e.Row.Cells[2].Text;
                }
                if (e.Row.Cells[5].Text.Equals("0"))
                {
                    e.Row.Cells[1].Font.Bold = true;
                    tmp = @"<b style='font-size:20px'>" + e.Row.Cells[3].Text + "</b>" + "  " + "<i style='color:gray;font-size:15px'>" + e.Row.Cells[4].Text + "</i>";
                }
                else
                {
                   
                    tmp = @"<span style='font-size:20px'>" + e.Row.Cells[3].Text + "</span>" + "  " + "<i style='color:gray;font-size:15px'>" + e.Row.Cells[4].Text + "</i>";
                }
                System.Web.UI.WebControls.Label lbl = e.Row.FindControl("lblmailSujBody") as System.Web.UI.WebControls.Label;
                lbl.Text = tmp;
                e.Row.Cells[2].Visible = false;
                e.Row.Cells[3].Visible = false;
                e.Row.Cells[4].Visible = false;
                e.Row.Cells[5].Visible = false;
            }
        }
        #endregion
        #endregion

        #region ViewDetail

        protected void btnreturnINDetail_Click(object sender, EventArgs e)////END_l'action de Button Retour, il est changé le page selon l'attribut de flagViewLasst
        {
          //flagViewLast représente la dernière page on a été 
         
            switch (flagViewLast)
            {
                case ViewEtat.ViewReceive: this.MultiView1.SetActiveView(ViewPrincipal);
                    break;
                case ViewEtat.ViewSend: this.MultiView1.SetActiveView(ViewSend);
                    break;
                case ViewEtat.ViewSuppr: this.MultiView1.SetActiveView(ViewPrincipal);
                    break;
                case ViewEtat.ViewImport: this.MultiView1.SetActiveView(ViewPrincipal);
                    break;
                case ViewEtat.ViewBrouillon: this.MultiView1.SetActiveView(ViewPrincipal);
                    break;
                case ViewEtat.ViewRechercher: this.MultiView1.SetActiveView(ViewRechercher);
                    break;
            }
        }

        protected void ViewDetail_Activate(object sender, EventArgs e)//END_
        {

        }//Rien
        protected void btnSupprINDetail_Click(object sender, EventArgs e)//quand on est dans la page de detail ,il y a 3 conditions:viewPrincipal,viewRechercher,viewSend
            //on va supprimer l'email en fonction de chaque condition que on a choisi
        {
            string mailid="=================";
            if (flagViewLast == ViewEtat.ViewRechercher)
            {
                string couter = ddlselection.SelectedValue;
                int testnumer = getMailSelectedIndex();
                string mailid1 = gvmailRechercher.DataKeys[testnumer].Value.ToString();
                mailid = mailid1;
                string testmailid1 = tbxRechercher.Text;
                string datedebut = tbRechercherDateDebut.Text;
                string dateau = tbRechercherDateAu.Text;
                if (EmailClass.IsExistMailNomFrom(testmailid1) || EmailClass.IsExistMailNomTo(testmailid1) || EmailClass.IsExistMailNomBody(testmailid1) || EmailClass.IsExistMailNomBodySimple(testmailid1) || EmailClass.IsExistMailNomSubject(testmailid1))
                {
                    if (EmailClass.getMailNomFrom(mailid) == "test@sipcom.fr")
                    {
                        rm.deleteMail(mailid);
                        rm.deleteMailSend(mailid);
                        rm.supprMailIMAP("test@sipcom.fr", "test123", "imap.1and1.fr", true, 993, mailid, mailid);
                        gvmailRechercher.DataSource = gvMailDataSource(couter, testmailid1, datedebut, dateau);
                        gvmailRechercher.DataBind();
                     
                    }
                    else
                    {
                        rm.deleteMail(mailid);
                        rm.supprMailIMAP("test@sipcom.fr", "test123", "imap.1and1.fr", true, 993, mailid, mailid);
                        gvmailRechercher.DataSource = gvMailDataSource(couter, testmailid1, datedebut, dateau);
                        gvmailRechercher.DataBind();

                    }

                }
                else if (EmailClass.IsExistMailSendFrom(testmailid1) || EmailClass.IsExistMailSendTo(testmailid1) || EmailClass.IsExistMailSendBody(testmailid1) || EmailClass.IsExistMailSendBodySimple(testmailid1) || EmailClass.IsExistMailSendSubject(testmailid1))
                {
                    if (EmailClass.getMailSendNomTo(mailid) == "test@sipcom.fr")
                    {
                        rm.deleteMail(mailid);
                        rm.deleteMailSend(mailid);
                        rm.supprMailIMAP("test@sipcom.fr", "test123", "imap.1and1.fr", true, 993, mailid, mailid);
                        gvmailRechercher.DataSource = gvMailDataSource(couter, testmailid1, datedebut, dateau);
                        gvmailRechercher.DataBind();

                    }
                    else
                    {
                        rm.deleteMailSend(mailid);
                        rm.supprMailIMAP("test@sipcom.fr", "test123", "imap.1and1.fr", true, 993, mailid, mailid);
                        gvmailRechercher.DataSource = gvMailDataSource(couter,testmailid1, datedebut, dateau);
                        gvmailRechercher.DataBind();
                    }

                }
            }
            else if(flagViewLast==ViewEtat.ViewReceive)
            {
                string mailid2 = gvViewEmail.DataKeys[gvViewEmail.SelectedIndex].Value.ToString();
                mailid = mailid2;
                rm.deleteMail(mailid);
                rm.supprMailIMAP("test@sipcom.fr", "test123", "imap.1and1.fr", true, 993, mailid, mailid);
                       
            }
            else if (flagViewLast==ViewEtat.ViewSend)
            {
                string mailid3 = gvmailSend.DataKeys[gvmailSend.SelectedIndex].Value.ToString();
                mailid = mailid3;
                rm.deleteMailSend(mailid);
                rm.supprMailIMAP("test@sipcom.fr", "test123", "imap.1and1.fr", true, 993, mailid, mailid);
            }
                        
                       
                          
                        
                      if(flagViewLast==ViewEtat.ViewRechercher)
                      {  
                          this.MultiView1.SetActiveView(ViewRechercher);
                       }
                     else if(flagViewLast==ViewEtat.ViewReceive)
                      {
                          this.MultiView1.SetActiveView(ViewPrincipal);
                       }
                      else if (flagViewLast == ViewEtat.ViewSend)
                      {
                          this.MultiView1.SetActiveView(ViewSend);
                      }
          
        }


        // télécharger les attachements dans cet email 
        protected void linkbtn_Click(object sender, EventArgs e) //END_ télécharger les attachements dans cet email 
        {
            LinkButton lb = (LinkButton)sender;
            string pathfile = lb.Text;
            Response.ContentType = "application/x-zip-compressed";
            Response.AddHeader("Content-Disposition", "attachment;filename=" + pathfile);
            string filename = Server.MapPath("~/attchment/" + pathfile);
            Response.HeaderEncoding = System.Text.Encoding.GetEncoding("gb2312");
            Response.TransmitFile(filename);
         }
        #endregion

        #region ViewMailSend

        protected void ViewSend_Activate(object sender, EventArgs e)
        {
            
            rm.getMailSendImap("test@sipcom.fr", "test123", "imap.1and1.fr", true, 993);//END_obtenir les émails de le Serveur et après tous sont stocké dans le table de MailSend de la base de donnés 
         
            DataTable dt = rm.getmailSend().Tables[0];
            gvmailSend.DataSource = dt;
            gvmailSend.DataBind();
            gvmailSend.SelectedIndex = -1;
            
        }//l'action de l'activation de la boîte reçu 
        protected void gvmailSend_RowDataBound(object sender, GridViewRowEventArgs e)//END_l'action de mettre les informations dans chaque ligne de ViewSend
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {//souris arrive change le couleur
                if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate || e.Row.RowState == DataControlRowState.Selected)
                {
                    e.Row.Attributes.Add("onmouseover", "c=this.style.backgroundColor;this.style.backgroundColor='#99CCFF'");
                    e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=c");
                }
                string tmp = "";
                e.Row.Cells[1].Text = HttpUtility.HtmlDecode(e.Row.Cells[1].Text);
                e.Row.Cells[1].ToolTip = (e.Row.Cells[1].Text).Trim();
                if (string.IsNullOrWhiteSpace(e.Row.Cells[1].Text))
                {
                   
                }
                else if (e.Row.Cells[1].Text.Length > 25)
                {
                    e.Row.Cells[1].Text = e.Row.Cells[1].Text.Substring(0, 25) + "...";
                }
                tmp = @"<span style='font-size:20px'>" + e.Row.Cells[2].Text + "</span>" + "  " + "<i style='color:gray;font-size:15px'>" + e.Row.Cells[3].Text + "</i>";                     
                System.Web.UI.WebControls.Label lbl = e.Row.FindControl("lblmailSujBody") as System.Web.UI.WebControls.Label;
                lbl.Text = tmp;
                e.Row.Cells[2].Visible = false;
                e.Row.Cells[3].Visible = false;
               
            }
        }
        protected void gvmailSend_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            string mailid = gvmailSend.DataKeys[e.RowIndex].Value.ToString();
            try
            {
                rm.deleteMailSend(mailid);
                          
            }
            catch
            {
            }
            DataTable dt = rm.getmailSend().Tables[0];
            gvmailSend.DataSource = dt;
            gvmailSend.DataBind();
        }//END_l'action de supprimer un ligne de ViewSend et le bouton Suppr utilise cet méthode aussi 
        protected void gvmailSend_SelectedIndexChanged(object sender, EventArgs e)//END_
        {
            this.MultiView1.SetActiveView(ViewDetail);
            string id = gvmailSend.DataKeys[gvmailSend.SelectedIndex].Value.ToString();
            int testid = Convert.ToInt32(gvmailSend.DataKeys[gvmailSend.SelectedIndex].Value);
            DataTable dt = rm.getmailSend(id).Tables[0];
            foreach (DataRow testrow in dt.Rows)
            {
                string testidstr = testid.ToString();
                if (testidstr == testrow["mailId"].ToString())
                {
                    lblsubject.Text = testrow["mailsubject"].ToString();//quand on clique le bouton Detail,on va mémoriser tous les détail(@destinateur  ,@émetteur,Sujet，Le Contenu  ) de cet émail 
                    lblsender.Text = testrow["mailsender"].ToString();
                    lblfrom.Text = testrow["mailfrom"].ToString();
                    lbldate.Text = testrow["maildateTime"].ToString();
                    lblto.Text = testrow["mailto"].ToString();
                    lblcc.Text = testrow["mailcc"].ToString();
                    divcontenu.InnerHtml = testrow["mailbody"].ToString();
                    //ajouter des attachments
                    string listattam = testrow["pathAttachmentFile"].ToString();

                    DataTable dta = new DataTable();
                    dta.Columns.Add(new DataColumn("pathfile", typeof(string)));
                    string[] tableauattam = listattam.Split(';');
                    int countfile = tableauattam.Length;
                    for (int i = 0; i < countfile - 1; i++)
                    {
                        DataRow dra = dta.NewRow();
                        int index = tableauattam[i].LastIndexOf('\\');
                        string filename = tableauattam[i].Substring(index + 1);
                        dra["pathfile"] = filename;
                        dta.Rows.Add(dra);
                    }
                    dtlAttatement.DataSource = dta;
                    dtlAttatement.DataBind();
                    dtlAttatement.DataSource = null;
                }
            }


        }//l'action de choisi un ligne et le bouton Detail utilise aussi cet méthode pour voir les détails
        protected void gvmailSend_PageIndexChanging(object sender, GridViewPageEventArgs e)//END_
        {
            gvmailSend.PageIndex = e.NewPageIndex;
            gvmailSend.SelectedIndex = -1;           
            gvmailSend.DataSource = rm.getmailSend().Tables[0];
            gvmailSend.DataBind();
        }//l'action de changer le page 
        protected void btnactualiserINSend_Click(object sender, EventArgs e)//END_l'action d'actualiser les messages envoyées
        {
            rm.getMailSendImap("test@sipcom.fr", "test123", "imap.1and1.fr", true, 993);

            DataTable dt = rm.getmailSend().Tables[0];
            gvmailSend.DataSource = dt;
            gvmailSend.DataBind();
            gvmailSend.SelectedIndex = -1;
        }
        protected void btnsupprINSend_Click(object sender, EventArgs e)//END_
        {
            foreach (GridViewRow gr in gvmailSend.Rows)
            {
                System.Web.UI.WebControls.CheckBox cbx = gr.FindControl("checkSelect") as System.Web.UI.WebControls.CheckBox;
                if (cbx.Checked)
                {
                    string mailid = gvmailSend.DataKeys[gr.RowIndex].Value.ToString();
                    try
                    {
                        rm.deleteMail(mailid);
                    }
                    catch
                    {
                    }
                }
            }
            DataTable dt = rm.getmailSend().Tables[0];
            gvmailSend.DataSource = dt;
            gvmailSend.DataBind();
        }//l'actiom de supprimer les messages enoyees

        #endregion

        #region Emailcommun_END

        protected int getMultiviewEtat()//END_obtenir les etats des pages actuelles
        {
            return this.MultiView1.ActiveViewIndex;
        }

        protected void videDataEmailWrite()//END_vides les donnés de page qu'on a ecrit dans le ViewEmailEcirt
        {
            tbxAdresseRecu.Text = "";
            tbxAdresseCC.Text = "";
            tbxSujet.Text = "";
            _editorHtml.Value = "";
            _editorText.Value = "";
            WebUserControl1.Emaildatastatic.getInstance().supprdatalist();
        }
        protected int getMailSelectedIndex()//END_obtenir quel numéro on a choisi dans  le ViewRechercher
        {
            int numer = gvmailRechercher.SelectedIndex;
            return numer;
        }
        #endregion

        #region ViewBrouillons_END
        protected void ViewBrouillons_Activate(object sender, EventArgs e)//END_l'actualisation de viewbrouillons
        {
            gvmailBrouillons_databind();
        }

        protected void gvmailBrouillons_SelectedIndexChanged(object sender, EventArgs e)//END_l'action de choisi un ligne et le bouton Edit utilise aussi cet méthode pour voir les détails
        {
            string id = gvmailBrouillons.DataKeys[gvmailBrouillons.SelectedIndex].Value.ToString();
            EmailClass.remettreEmail(id);
            WebUserControl1.Emaildatastatic.getInstance().mailSend = "SAVEBrouillons";
            this.MultiView1.SetActiveView(ViewEcrire);
        }
        protected void gvmailBrouillons_databind()//END_l'action de mettre les informations dans le Viewbrouillons
        {
            gvmailBrouillons.DataSource = EmailClass.getmailSaveAll();
            gvmailBrouillons.DataBind();
            gvmailBrouillons.SelectedIndex = -1;
            gvmailBrouillons.DataSource = null;
        }

        protected void gvmailBrouillons_RowDeleting(object sender, GridViewDeleteEventArgs e)//END_l'action de supprimer un ligne de ViewSend et le bouton Delete utilise cet méthode aussi 
        {
            string mailid = gvmailBrouillons.DataKeys[e.RowIndex].Value.ToString();
            rm.deleteMailBrouillons(mailid);
            gvmailBrouillons_databind();
        }
        protected void gvmailBrouillons_RowDataBound(object sender, GridViewRowEventArgs e)//souris arrive change le couleur
        {//souris arrive change le couleur
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate || e.Row.RowState == DataControlRowState.Selected)
            {
                e.Row.Attributes.Add("onmouseover", "c=this.style.backgroundColor;this.style.backgroundColor='#99CCFF'");
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=c");
            }
        }
        #endregion

        #region ViewEmailCorbeill_END
        protected void ViewCorbeille_Activate(object sender, EventArgs e)//END_l'action de l'activation de corbeill
        {
            gvmailCorbeille_databind();
        }
        protected void gvmailCorbeille_SelectedIndexChanged(object sender, EventArgs e)//END_l'action de choisi un ligne et le bouton Remettre utilise aussi cet méthode pour voir les détails
        {
            string mailid = gvmailCorbeille.DataKeys[gvmailBrouillons.SelectedIndex].Value.ToString();
            rm.remettreEmailDelete(mailid);
            gvmailCorbeille_databind();
        }

        protected void gvmailCorbeille_RowDeleting(object sender, GridViewDeleteEventArgs e)//END_l'action de supprimer un ligne de ViewSend et le bouton Delete utilise cet méthode aussi 
        {
            string mailid = gvmailCorbeille.DataKeys[e.RowIndex].Value.ToString();
            rm.deleteMailGet(mailid);
            gvmailCorbeille_databind();
        }

        protected void gvmailCorbeille_PageIndexChanging(object sender, GridViewPageEventArgs e)//END_l'action de changer le page 
        {
            gvmailCorbeille.PageIndex = e.NewPageIndex;
            gvmailCorbeille_databind();           
        }
        protected void gvmailCorbeille_databind()//END_l'action de mettre les informations
        {
            gvmailCorbeille.DataSource = rm.getMailDelete();
            gvmailCorbeille.DataBind();
            gvmailCorbeille.SelectedIndex = -1;
            gvmailBrouillons.DataSource = null;

        }
        protected void gvmailCorbeille_RowDataBound(object sender, GridViewRowEventArgs e)//END_ Quand souris arrive il change le couleur
        {
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate || e.Row.RowState == DataControlRowState.Selected)
            {
                e.Row.Attributes.Add("onmouseover", "c=this.style.backgroundColor;this.style.backgroundColor='#99CCFF'");
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=c");
            }
        }
        #endregion
      
        #region ViewRechercher_END
        protected void ViewRechercher_Activate(object sender, EventArgs e)//END_
        {
            gvmailRechercher_databind();
        }// l'action de l'activation de Gridview Rechercher
        protected void gvmailRechercher_databind()//END_l'action de mettre les informations quand on  tape l'adresse d'email on va chercher 
        {
            string id = tbxRechercher.Text;
            string datedebut = tbRechercherDateDebut.Text;
            string dateau = tbRechercherDateAu.Text;
            if (!string.IsNullOrEmpty(dateau) && (!string.IsNullOrEmpty(datedebut)))
            {
                gvmailRechercher.DataSource = EmailClass.getmailRechercher(id, datedebut, dateau);
            }
            else
            {
                gvmailRechercher.DataSource = EmailClass.getmailRechercher(id);
            }
            gvmailRechercher.DataBind();
            gvmailRechercher.SelectedIndex = -1;
            gvmailRechercher.DataSource = null;
        }
        protected void tbxRechercher_TextChanged(object sender, EventArgs e)//END_l'action de taper les adresses des emails mais  on ne lance pas de recherche dès que on clique le bouton Rechercher
        {
            btnrecherche_Click(null,null);
        }
        protected void btnrecherche_Click(object sender, EventArgs e)//END_l'action de cliquer le bouton Rechercher
        {
            gvmailRechercher.PageIndex = 0;
            gvmailRechercher_databind();
            this.MultiView1.SetActiveView(ViewRechercher);
        }

        protected void gvmailrechercher_SelectedIndexChanged(object sender, EventArgs e)//END_l'action de choisi un ligne et le bouton Detail utilise aussi cet méthode pour voir les détails et il va transférer dans le page Gridview Detail
        {
            this.MultiView1.SetActiveView(ViewDetail);
            flagViewLast = ViewEtat.ViewRechercher;
            string mailid = gvmailRechercher.DataKeys[gvmailRechercher.SelectedIndex].Value.ToString();
     
            int testid = Convert.ToInt32(gvmailRechercher.DataKeys[gvmailRechercher.SelectedIndex].Value);//obtenir le mailid qu'on a choisi 
            
            string testmailid1= tbxRechercher.Text;
           
            DataTable dt = rm.getMailList(mailid).Tables[0];

            if (EmailClass.IsExistMailNomFrom(testmailid1) || EmailClass.IsExistMailNomTo(testmailid1)||EmailClass.IsExistMailNomBody(testmailid1) || EmailClass.IsExistMailNomBodySimple(testmailid1) || EmailClass.IsExistMailNomSubject(testmailid1))//il faut savoir l'adresse qu'on tapé est dans le boîte reçu ou bien le boîte  envoyé 
             {
                dt = rm.getMailList(mailid).Tables[0];
             }
            else if (EmailClass.IsExistMailSendFrom(testmailid1) || EmailClass.IsExistMailSendTo(testmailid1) || EmailClass.IsExistMailSendBody(testmailid1) || EmailClass.IsExistMailSendBodySimple(testmailid1) || EmailClass.IsExistMailSendSubject(testmailid1))
             {
                dt = rm.getmailSend(mailid).Tables[0];
              }
        
            foreach (DataRow testrow in dt.Rows)
            {
                    string testidstr = testid.ToString();
                    if (testidstr == testrow["mailId"].ToString()) 
                 {
                     lblsubject.Text = testrow["mailsubject"].ToString();
                     lblsender.Text = testrow["mailsender"].ToString();
                     lblfrom.Text = testrow["mailfrom"].ToString();
                     lbldate.Text = testrow["maildateTime"].ToString();
                     lblto.Text = testrow["mailto"].ToString();
                     lblcc.Text = testrow["mailcc"].ToString();
                     divcontenu.InnerHtml = testrow["mailbody"].ToString();
                    //ajouter des attachments
                     string listattam = testrow["pathAttachmentFile"].ToString();
               
                    DataTable dta = new DataTable();
                    dta.Columns.Add(new DataColumn("pathfile", typeof(string)));
                    string[] tableauattam = listattam.Split(';');
                    int countfile = tableauattam.Length;
                    for (int i = 0; i < countfile - 1; i++)
                    {
                        DataRow dra = dta.NewRow();
                        int index = tableauattam[i].LastIndexOf('\\');
                        string filename = tableauattam[i].Substring(index + 1);
                        dra["pathfile"] = filename;
                        dta.Rows.Add(dra);
                    }
                    dtlAttatement.DataSource = dta;
                    dtlAttatement.DataBind();
                    dtlAttatement.DataSource = null;
                }
            }  
        }

        protected void gvmailrechercher_RowDeleting(object sender, GridViewDeleteEventArgs e)//END_l'action de supprimer une ligne et  le bouton Suppr utilise aussi cet méthode et il va séparer 3 conditions
        //numéro 0 représente boite envoyé c'est-à-dire on va chercher les email dans  boite envoyé 
        //numéro 1 représente  boite recu c'est-à-dire on va chercher les email dans boite recu 
        //numéro 2  représente tout(boite envoyé et boite recu )
        //pour chaque numéro il existe un condition particulier: @destinataire ,@émetteur est la même adresse  à ce moment la , quand on supprimer l'email ,Il faut supprimer l'email dans boite envoyé et boite recu à la fois  
        {
            string mailid = gvmailRechercher.DataKeys[e.RowIndex].Value.ToString();
            string testmailid1= tbxRechercher.Text;
            string datedebut = tbRechercherDateDebut.Text;
            string dateau = tbRechercherDateAu.Text;
            string counter = ddlselection.SelectedValue;
            gvMailDeleteDataSource(counter,mailid,testmailid1,datedebut,dateau);
            this.MultiView1.SetActiveView(ViewRechercher);
            gvmailRechercher.SelectedIndex = -1;
            gvmailRechercher.DataSource = null;
            DataTable dt2 = rm.getMailList().Tables[0];//après on supprime l'email, on obtient les émail de basé des donné pour faire la mis  à jour 
            int countmail = dt2.Rows.Count;
            lblnbrmail.Text = countmail.ToString();
            string countmailNoseen = rm.getMailList().Tables[0].Select(@"NotSeen=0").Length.ToString();
            lblnbrmailNoseen.Text = countmailNoseen;
          
        }
        protected void gvmailRechercher_RowDataBound(object sender, GridViewRowEventArgs e)//souris arrive change le couleur
        {
            if (e.Row.RowState == DataControlRowState.Normal || e.Row.RowState == DataControlRowState.Alternate || e.Row.RowState == DataControlRowState.Selected)
            {
                e.Row.Attributes.Add("onmouseover", "c=this.style.backgroundColor;this.style.backgroundColor='#99CCFF'");
                e.Row.Attributes.Add("onmouseout", "this.style.backgroundColor=c");
            }
        }
        protected void gvmailRechercher_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvmailRechercher.PageIndex = e.NewPageIndex;
            gvmailRechercher.SelectedIndex = -1;
            string id = tbxRechercher.Text;
            string datedebut = tbRechercherDateDebut.Text;
            string dateau = tbRechercherDateAu.Text;
            string counter = ddlselection.SelectedValue;
            gvmailRechercher.DataSource = gvMailDataSource(counter, id, datedebut, dateau);
            gvmailRechercher.DataBind();
        }//END_l'action de changer le page et il a divisé  en 3 partis il faut regarder les options qu'on a choisi 


        protected void btnsupprcheck_Click(object sender, EventArgs e)//END_ presque la même chose pour ROWDeleting 
        {
            foreach (GridViewRow gr in gvmailRechercher.Rows)
            {
                System.Web.UI.WebControls.CheckBox cbx = gr.FindControl("checkSelect") as System.Web.UI.WebControls.CheckBox;
                if (cbx.Checked)
                {
                    int numero = gr.RowIndex;
                    string mailid = gvmailRechercher.DataKeys[numero].Value.ToString();
                    string testmailid1 = tbxRechercher.Text;
                    string datedebut = tbRechercherDateDebut.Text;
                    string dateau = tbRechercherDateAu.Text;
                    if (cbx.Checked)
                    {
                      gvMailDeleteDataSource(ddlselection.SelectedValue,mailid,testmailid1,datedebut,dateau);
                     }
                 }
            }
            this.MultiView1.SetActiveView(ViewRechercher);
            gvmailRechercher.SelectedIndex = -1;
            gvmailRechercher.DataSource = null;
            DataTable dt2 = rm.getMailList().Tables[0];//mis a jour le nombre de email ce qui ne sont pas encore vu et deja vu
            int countmail = dt2.Rows.Count;
            lblnbrmail.Text = countmail.ToString();
            string countmailNoseen = rm.getMailList().Tables[0].Select(@"NotSeen=0").Length.ToString();
            lblnbrmailNoseen.Text = countmailNoseen;
           
        }


        protected void ddlselection_SelectedIndexChanged(object sender, EventArgs e)//l'action de changer le option et il a divisé  en 3 partis il faut regarder les options qu'on a choisi 
        {
            string datedebut = tbRechercherDateDebut.Text;
            string dateau = tbRechercherDateAu.Text;
            string id = tbxRechercher.Text;
            string counter = ddlselection.SelectedValue;
            gvmailRechercher.DataSource = gvMailDataSource(counter,id,datedebut,dateau);
            gvmailRechercher.DataBind();
            this.MultiView1.SetActiveView(ViewRechercher);
            resetgvmailrecherche();
        }
        protected void resetgvmailrecherche()////rien
        {
            gvmailRechercher.SelectedIndex = -1;
            gvmailRechercher.DataSource = null;
        }

        protected void tbRechercherDateDebut_TextChanged(object sender, EventArgs e)//rien
        {
            btnrecherche_Click(null, null);
        }

        protected void tbRechercherDateAu_TextChanged(object sender, EventArgs e)//rien
        {
            btnrecherche_Click(null, null);
        }
        #endregion

        #region Commun_En_Rechercher

        public DataSet gvMailDataSource(string selecteoption,string testmailid, string timefrom, string timeto) // mettre les datasources  en  fonction de chaque option qu'on a choisi dans le dropdownlist
        {
            DataSet ds = new DataSet();
            if (selecteoption == "0") 
            {
                    if (!string.IsNullOrEmpty(timeto) && (!string.IsNullOrEmpty(timefrom)))
                    {
                        ds = EmailClass.getmailRechercherTo(testmailid, timefrom, timeto);
                    }
                    else
                    {
                        ds = EmailClass.getmailRechercherTo(testmailid);
                    }
                
            }
            else if (selecteoption == "1")
            {
                if (!string.IsNullOrEmpty(timeto) && (!string.IsNullOrEmpty(timefrom)))
                {
                    ds = EmailClass.getmailRechercherFrom(testmailid, timefrom, timeto);
                }
                else
                {
                    ds = EmailClass.getmailRechercherFrom(testmailid);
                }
            }
            else if (selecteoption == "2")
            {
                if (!string.IsNullOrEmpty(timeto) && (!string.IsNullOrEmpty(timefrom)))
                {
                   ds = EmailClass.getmailRechercher(testmailid, timefrom, timeto);
                }
                else
                {
                    ds = EmailClass.getmailRechercher(testmailid);
                }
            }
            return ds;
        }
        public void gvMailDeleteDataSource(string selecteoption,string mailadressespecial,string testmailid, string timefrom, string timeto)//mettre les datasources et supprimer les emails  en  fonction de chaque option qu'on a choisi dans le dropdownlist 
        {
            
            if (selecteoption == "0")
            {
                if (EmailClass.getMailSendNomTo(mailadressespecial) == "test@sipcom.fr")
                {
                    rm.deleteMail(mailadressespecial);
                    rm.deleteMailSend(mailadressespecial);
                    rm.supprMailIMAP("test@sipcom.fr", "test123", "imap.1and1.fr", true, 993, mailadressespecial, mailadressespecial);
                    if (!string.IsNullOrEmpty(timefrom) && (!string.IsNullOrEmpty(timeto)))
                    {
                        gvmailRechercher.DataSource = EmailClass.getmailRechercherTo(testmailid, timefrom, timeto);
                    }
                    else
                    {
                        gvmailRechercher.DataSource = EmailClass.getmailRechercherTo(testmailid);
                    }

                    gvmailRechercher.DataBind();
                }
                else
                {
                    rm.deleteMailSend(mailadressespecial);
                    rm.supprMailIMAP("test@sipcom.fr", "test123", "imap.1and1.fr", true, 993, mailadressespecial, mailadressespecial);
                    if (!string.IsNullOrEmpty(timefrom) && (!string.IsNullOrEmpty(timeto)))
                    {
                        gvmailRechercher.DataSource = EmailClass.getmailRechercherTo(testmailid, timefrom, timeto);
                    }
                    else
                    {
                        gvmailRechercher.DataSource = EmailClass.getmailRechercherTo(testmailid);
                    }
                    gvmailRechercher.DataBind();
                }
            }
            else if (selecteoption == "1") 
            {
                if (EmailClass.getMailNomFrom(mailadressespecial) == "test@sipcom.fr")
                {
                    rm.deleteMail(mailadressespecial);
                    rm.deleteMailSend(mailadressespecial);
                    rm.supprMailIMAP("test@sipcom.fr", "test123", "imap.1and1.fr", true, 993, mailadressespecial, mailadressespecial);
                    if (!string.IsNullOrEmpty(timefrom) && (!string.IsNullOrEmpty(timeto)))
                    {
                        gvmailRechercher.DataSource = EmailClass.getmailRechercherTo(testmailid, timefrom, timeto);
                    }
                    else
                    {
                        gvmailRechercher.DataSource = EmailClass.getmailRechercherTo(testmailid);
                    }
                    gvmailRechercher.DataBind();

                }
                else
                {
                    rm.deleteMail(mailadressespecial);
                    rm.supprMailIMAP("test@sipcom.fr", "test123", "imap.1and1.fr", true, 993, mailadressespecial, mailadressespecial);
                    if (!string.IsNullOrEmpty(timefrom) && (!string.IsNullOrEmpty(timeto)))
                    {
                        gvmailRechercher.DataSource = EmailClass.getmailRechercherFrom(testmailid, timefrom, timeto);
                    }
                    else
                    {
                        gvmailRechercher.DataSource = EmailClass.getmailRechercherFrom(testmailid);
                    }
                    gvmailRechercher.DataBind();
                }
            }
            else if (selecteoption == "2")
            {
                if (EmailClass.IsExistMailNomFrom(mailadressespecial) || EmailClass.IsExistMailNomTo(mailadressespecial) || EmailClass.IsExistMailNomBody(mailadressespecial) || EmailClass.IsExistMailNomBodySimple(mailadressespecial) || EmailClass.IsExistMailNomSubject(mailadressespecial))
                {
                    if (EmailClass.getMailNomFrom(mailadressespecial) == "test@sipcom.fr")
                    {
                        rm.deleteMail(mailadressespecial);
                        rm.deleteMailSend(mailadressespecial);
                        rm.supprMailIMAP("test@sipcom.fr", "test123", "imap.1and1.fr", true, 993, mailadressespecial, mailadressespecial);
                        if (!string.IsNullOrEmpty(timefrom) && (!string.IsNullOrEmpty(timeto)))
                        {
                            gvmailRechercher.DataSource = EmailClass.getmailRechercher(testmailid, timefrom, timeto);
                        }
                        else
                        {
                            gvmailRechercher.DataSource = EmailClass.getmailRechercher(testmailid);
                        }
                        gvmailRechercher.DataBind();

                    }
                    else
                    {
                        rm.deleteMail(mailadressespecial);
                        rm.supprMailIMAP("test@sipcom.fr", "test123", "imap.1and1.fr", true, 993, mailadressespecial, mailadressespecial);
                        if (!string.IsNullOrEmpty(timefrom) && (!string.IsNullOrEmpty(timeto)))
                        {
                            gvmailRechercher.DataSource = EmailClass.getmailRechercher(testmailid, timefrom, timeto);
                        }
                        else
                        {
                            gvmailRechercher.DataSource = EmailClass.getmailRechercher(testmailid);
                        }
                        gvmailRechercher.DataBind();
                    }

                }
                else if (EmailClass.IsExistMailSendFrom(testmailid) || EmailClass.IsExistMailSendTo(testmailid) || EmailClass.IsExistMailSendBody(testmailid) || EmailClass.IsExistMailSendBodySimple(testmailid) || EmailClass.IsExistMailSendSubject(testmailid))
                {
                    if (EmailClass.getMailSendNomTo(mailadressespecial) == "test@sipcom.fr")
                    {
                        rm.deleteMail(mailadressespecial);
                        rm.deleteMailSend(mailadressespecial);
                        rm.supprMailIMAP("test@sipcom.fr", "test123", "imap.1and1.fr", true, 993, mailadressespecial, mailadressespecial);
                        if (!string.IsNullOrEmpty(timefrom) && (!string.IsNullOrEmpty(timeto)))
                        {
                            gvmailRechercher.DataSource = EmailClass.getmailRechercher(testmailid, timefrom, timeto);
                        }
                        else
                        {
                            gvmailRechercher.DataSource = EmailClass.getmailRechercher(testmailid);
                        }
                        gvmailRechercher.DataBind();

                    }
                    else
                    {
                        rm.deleteMailSend(mailadressespecial);
                        rm.supprMailIMAP("test@sipcom.fr", "test123", "imap.1and1.fr", true, 993, mailadressespecial, mailadressespecial);
                        if (!string.IsNullOrEmpty(timefrom) && (!string.IsNullOrEmpty(timeto)))
                        {
                            gvmailRechercher.DataSource = EmailClass.getmailRechercher(testmailid, timefrom, timeto);
                        }
                        else
                        {
                            gvmailRechercher.DataSource = EmailClass.getmailRechercher(testmailid);
                        }
                        gvmailRechercher.DataBind();
                    }

                }
            }
           

        }
        #endregion
      
   }
}
