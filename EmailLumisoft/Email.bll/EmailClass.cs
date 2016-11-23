using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Net;
using OpenPop.Mime.Header;
using LumiSoft.Net.IMAP.Client;
using LumiSoft.Net.IMAP;
using LumiSoft.Net.Mail;
using LumiSoft.Net.MIME;
using LumiSoft.Net;
using System.IO;
using OpenPop.Pop3;
using OpenPop.Mime;
using System.Net.Mail;
using System.Text;
using Email_Test.Email.dll;

namespace Email_Test.Email.bll
{
    public class EmailClass
    {
       // public static string sqlConnection_SystemEmail = "Data source=YANGQIFAN;Initial Catalog=SystemEmail;Integrated Security=True";//entrée la base de donnes
        public static string sqlConnection_SystemEmail = "server=YANGQIFAN;uid=sa;pwd=123456;database=SystemEmail";
        #region lumisoft Imap
        public IMAP_Client loginMail(string userName, string psw, string service, bool ssl, int port)
        {
            IMAP_Client imapc = new IMAP_Client();
            try
            {
                if (imapc.IsConnected)
                {
                    imapc.Disconnect();
                }
                imapc.Connect(service, port, ssl);
                imapc.Login(userName, psw);
            }
            catch (Exception ex)
            {
                Console.WriteLine("ErrConnect:" + ex.Message);
            }
            return imapc;
        }//lumisoft de entre les mot de passe de utilisateur 
        public void supprMailIMAP(string userName, string psw, string service, bool ssl, int port, string mailsupprBegin, string mailsupprEnd)
        {
            IMAP_Client imapc = loginMail(userName, psw, service, ssl, port);
            var folderlist = imapc.GetFolders(null);
            imapc.SelectFolder("INBOX");
            var folder = imapc.SelectedFolder;
            var seqSet = IMAP_t_SeqSet.Parse("@begin:@end".Replace("@begin", mailsupprBegin).Replace("@end", mailsupprEnd));
            var items = new IMAP_t_Fetch_i[]
                {
                    new IMAP_t_Fetch_i_Envelope(),
                    new IMAP_t_Fetch_i_Uid(),
                    new IMAP_t_Fetch_i_Flags(),
                    new IMAP_t_Fetch_i_InternalDate(),
                    new IMAP_t_Fetch_i_Rfc822()
                };
            try
            {
                imapc.StoreMessageFlags(true, seqSet, IMAP_Flags_SetType.Add, new IMAP_t_MsgFlags(new string[] { IMAP_t_MsgFlags.Deleted }));
            }
            catch
            {
            }
            imapc.Disconnect();
        }//lumisoft supprimer les  email dans le serveur 
        public void getMailImap(string userName, string psw, string service, bool ssl, int port)
        {
            try
            {
                IMAP_Client imapc = loginMail(userName, psw, service, ssl, port);

                var folderlist = imapc.GetFolders(null);

                imapc.SelectFolder("INBOX");
                var folder = imapc.SelectedFolder;
                var seqSet = IMAP_t_SeqSet.Parse("1:*");
                var items = new IMAP_t_Fetch_i[]
                {
                    new IMAP_t_Fetch_i_Envelope(),
                    new IMAP_t_Fetch_i_Uid(),
                    new IMAP_t_Fetch_i_Flags(),
                    new IMAP_t_Fetch_i_InternalDate(),
                    new IMAP_t_Fetch_i_Rfc822()
                };

                imapc.Fetch(false, seqSet, items, this.m_pImap_Fetch_MessageItems_UntaggedResponse);
                System.Threading.Thread.Sleep(500);
                DataTable dtmail = datalistmail.getInstance().getdt();
                SqlDataAdapter adapteremailrecu = CreerDataAdapter();
                dtmail.DefaultView.Sort = "maildateTime DESC";
                int tr = adapteremailrecu.Update(dtmail.Select(null, null, DataViewRowState.ModifiedCurrent));
                int tr12 = adapteremailrecu.Update(dtmail.Select(null, null, DataViewRowState.Added));
                //System.Threading.Thread.Sleep(500);
                imapc.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ErrGetMail:" + ex.Message);
            }
        }//obtenir  les  emails reçus  dans le serveur 
        private void m_pImap_Fetch_MessageItems_UntaggedResponse(object sender, EventArgs<IMAP_r_u> e)
        {
            DataTable dtmail = null;

            switch (Default.flagViewLast)
            {
                case Default.ViewEtat.ViewReceive:
                    dtmail = datalistmail.getInstance().getdt();
                    break;
                case Default.ViewEtat.ViewSend:
                    dtmail = datalistmail.getInstance().getdtSend();
                    break;
                case Default.ViewEtat.ViewSuppr:
                    break;
                case Default.ViewEtat.ViewImport:
                    break;
                case Default.ViewEtat.ViewBrouillon:
                    break;
            }

            try
            {
                var email = e.Value as IMAP_r_u_Fetch;
                if (dtmail.Select("mailID='@id'".Replace("@id", email.UID.UID.ToString())).Length < 1)
                {
                    if (email.Rfc822 != null)
                    {
                        email.Rfc822.Stream.Position = 0;
                        var mine = Mail_Message.ParseFromStream(email.Rfc822.Stream);
                        email.Rfc822.Stream.Close();
                        DataRow dtr = dtmail.NewRow();
                        string msender = ((LumiSoft.Net.Mail.Mail_t_Mailbox)(email.Envelope.Sender[0])).DisplayName;
                        string from = mine.From[0].Address;
                        string subject = mine.Subject;
                        string keyw = mine.Keywords;
                        var mailCc = mine.Cc;
                        var mailTo = mine.To;
                        DateTime dateSent = mine.Date;
                        string seen = email.Flags.Flags.ToString();
                        string body = "";
                        string bodys = "";
                        if (string.IsNullOrEmpty(mine.BodyHtmlText))
                        {
                            if (!string.IsNullOrEmpty(mine.BodyText))
                            {
                                body = bodys = mine.BodyText;
                            }
                        }
                        else
                        {
                            body = mine.BodyHtmlText;
                        }
                        if (!string.IsNullOrEmpty(mine.BodyText))
                        {
                            bodys = mine.BodyText;
                        }
                        string pathAttachmentFile = "";
                        
                        string dir = System.AppDomain.CurrentDomain.BaseDirectory+"attchment\\";
                        if (!System.IO.Directory.Exists(dir))
                            System.IO.Directory.CreateDirectory(dir);
                        if (mine.Attachments.Count() > 0)
                        {
                            var list = mine.Attachments.ToList();
                            foreach (var att in list)
                            {
                                var part = att.Body as MIME_b_SinglepartBase;

                                string filename = dir + att.ContentType.Param_Name;
                                FileInfo file = new FileInfo(filename);
                                if (file.Exists)
                                {
                                    file.Attributes = FileAttributes.Normal;
                                    file.Delete();
                                }
                                File.WriteAllBytes(filename, part.Data);
                                pathAttachmentFile = filename + ";" + pathAttachmentFile;
                            }
                        }
                        string bodySimple = "";
                        if (bodys.Length >= 30)
                        {
                            bodySimple = bodys.Substring(0, 30);
                        }
                        else
                        {
                            bodySimple = bodys;
                        }
                        string listCc = "";
                        if (mailCc != null)
                        {
                            foreach (Mail_t_Mailbox address in mailCc.Mailboxes)
                            {
                                listCc = listCc + address.Address.ToString() + ";";
                            }
                        }
                        string listTo = "";
                        if (mailTo != null)
                        {
                            foreach (Mail_t_Mailbox address in mailTo.Mailboxes)
                            {
                                listTo = listTo + address.Address.ToString() + ";";
                            }
                        }
                        body = body.Replace(@"cid:", @"/attchment/");
                        dtr["mailID"] = email.UID.UID.ToString();
                        dtr["fk_userid"] = 1;
                        dtr["mailsender"] = msender;
                        dtr["mailfrom"] = from;
                        dtr["mailto"] = listTo;
                        dtr["mailcc"] = listCc;
                        dtr["maildateTime"] = dateSent.ToString("yyyy-MM-dd HH:mm");
                        dtr["mailsubject"] = subject;
                        dtr["mailbodySimple"] = bodySimple;
                        dtr["mailbody"] = body;
                        dtr["pathAttachmentFile"] = pathAttachmentFile;
                        dtr["NotSeen"] = seen.Equals("\\Seen") ? 1 : 0;
                        dtmail.Rows.Add(dtr);
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("Handle-Err:" + ex.Message);
            }

        }
        public void getMailSendImap(string userName, string psw, string service, bool ssl, int port)
        {
            try
            {

                IMAP_Client imapc = loginMail(userName, psw, service, ssl, port);
                var folderlist = imapc.GetFolders(null);
                imapc.SelectFolder("Éléments envoyés");
               
                var folder = imapc.SelectedFolder;
                var seqSet = IMAP_t_SeqSet.Parse("1:*");
                var items = new IMAP_t_Fetch_i[]
                {
                    new IMAP_t_Fetch_i_Envelope(),
                    new IMAP_t_Fetch_i_Uid(),
                    new IMAP_t_Fetch_i_Flags(),
                    new IMAP_t_Fetch_i_InternalDate(),
                    new IMAP_t_Fetch_i_Rfc822()
                };

                imapc.Fetch(false, seqSet, items, this.m_pImap_Fetch_MessageItems_UntaggedResponse);
                System.Threading.Thread.Sleep(500);
                DataTable dtmail = datalistmail.getInstance().getdtSend();
                SqlDataAdapter adapteremailrecu = CreerDataAdapterFormail("MailSend");
                dtmail.DefaultView.Sort = "maildateTime DESC";
                int tr = adapteremailrecu.Update(dtmail.Select(null, null, DataViewRowState.ModifiedCurrent));
                int tr12 = adapteremailrecu.Update(dtmail.Select(null, null, DataViewRowState.Added));
               
                imapc.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ErrGetMail:" + ex.Message);
            }
        }//obtenir  les  emails envoyés  dans le serveur 
        public void receiveMail(string userName, string psw, string service)
        {
            DataTable dtmail = new DataTable();
            SqlDataAdapter adapteremailrecu = CreerDataAdapter();
            adapteremailrecu.Fill(dtmail);
            
            Pop3Client receiveclient = new Pop3Client();
            if (receiveclient.Connected)
            {
                receiveclient.Disconnect();
            }
            receiveclient.Connect(service, 995, true);
            receiveclient.Authenticate(userName, psw);
            int messageCount = receiveclient.GetMessageCount();
            List<string> ids = receiveclient.GetMessageUids();
            for (int i = 0; i < messageCount; i++)
            {
                if (dtmail.Select("mailID='@id'".Replace("@id", ids[i])).Length < 1)
                {
                    DataRow dtr = dtmail.NewRow();
                    OpenPop.Mime.Message message = receiveclient.GetMessage(i + 1);
                    string sender = message.Headers.From.DisplayName;
                    string from = message.Headers.From.Address;
                    string subject = message.Headers.Subject;
                    List<string> keyw = message.Headers.Keywords;
                    List<RfcMailAddress> mailCc = message.Headers.Cc;
                    List<RfcMailAddress> mailTo = message.Headers.To;
                    DateTime dateSent = message.Headers.DateSent;
                    MessagePart msgPart = message.MessagePart;

                    string body = "";
                    string bodys = "";
                    if (msgPart.IsText)
                    {
                        body = msgPart.GetBodyAsText();
                        bodys = body;
                    }
                    else if (msgPart.IsMultiPart)
                    {
                        MessagePart plainTextPart = message.FindFirstPlainTextVersion();
                        MessagePart plainHtmlPart = message.FindFirstHtmlVersion();
                        if (plainTextPart != null)
                        {
                            
                            body = plainHtmlPart.GetBodyAsText();
                            bodys = plainTextPart.GetBodyAsText();
                            
                        }
                        else
                        {
                          
                            List<MessagePart> textVersions = message.FindAllTextVersions();
                            if (textVersions.Count >= 1)
                            {
                                body = textVersions[0].GetBodyAsText();
                                bodys = body;
                            }
                        }
                    }
                    List<MessagePart> attachments = message.FindAllAttachments();
                    string pathAttachmentFile = "";
                    if (attachments.Count > 0)
                    {
                        string dir = System.Web.HttpContext.Current.Server.MapPath("~/attchment/");
                        if (!System.IO.Directory.Exists(dir))
                            System.IO.Directory.CreateDirectory(dir);
                        foreach (MessagePart attachment in attachments)
                        {

                            string newFileName = attachment.FileName;
                            string path = dir + newFileName;
                            WebClient myWebClient = new WebClient();
                            myWebClient.Credentials = CredentialCache.DefaultCredentials;
                            try
                            {
                                Stream postStream = myWebClient.OpenWrite(path, "PUT");
                                if (postStream.CanWrite)
                                {
                                    postStream.Write(attachment.Body, 0, attachment.Body.Length);
                                }
                                else
                                {
                                   
                                }
                                postStream.Close();//关闭流    
                                pathAttachmentFile = path + ";" + pathAttachmentFile;
                            }
                            catch
                            {
                                ;
                            }
                        }
                        attachments.Clear();
                    }
                    string bodySimple = "";
                    if (bodys.Length > 30)
                    {
                        bodySimple = bodys.Substring(0, 30);
                    }
                    else
                    {
                        bodySimple = bodys.Substring(0, bodys.Length);
                    }
                    string listCc = "";
                    foreach (RfcMailAddress address in mailCc)
                    {
                        listCc = listCc + address.Address.ToString() + ";";
                    }
                    string listTo = "";
                    foreach (RfcMailAddress address in mailTo)
                    {
                        listTo = listTo + address.ToString() + ";";
                    }
                    body = body.Replace(@"cid:", @"/attchment/");
                    dtr["mailID"] = ids[i];
                    dtr["fk_userid"] = 1;
                    dtr["mailsender"] = sender;
                    dtr["mailfrom"] = from;
                    dtr["mailto"] = listTo;
                    dtr["mailcc"] = listCc;
                    dtr["maildateTime"] = dateSent.ToString("yyyy-MM-dd HH:mm");
                    dtr["mailsubject"] = subject;
                    dtr["mailbodySimple"] = bodySimple;
                    dtr["mailbody"] = body;
                    dtr["pathAttachmentFile"] = pathAttachmentFile;
                    dtmail.Rows.Add(dtr);



                }
            }
            dtmail.DefaultView.Sort = "maildateTime DESC";
            adapteremailrecu.Update(dtmail);
        }//commuent on recevoir les email si quelqu'un envoie un émail 
        #endregion

       
        #region deletemail
        public int deleteMail(string mailId)//supprimer les émail reçu et dans le base de donnes il existe 
        {
          return  daomail.deleteMail(mailId);          
        }
        public int deleteMailSend(string mailId)//supprimer les émail envoyé 
        {
            return daomail.deleteMailSend(mailId);
        }

        public int deleteMailBrouillons(string mailId)//supprimer les émail dans le boîte brouillon
        {
            return daomail.deleteMailBrouillons(mailId);
        }
        public int deleteMailRechercher(string mailId) //supprimer les émail rechercher
        {
            return daomail.deleteMailRechercher(mailId);
        }
        public int deleteMailBrouillonsSended()//supprimer les émail déjà stocké dans le SaveTmp mais cet email etait envoyé  
        {
            return daomail.deleteMailBrouillonsSended();
        }
        public int deleteMailSaveTmpSended()//supprimer les émail déjà stocké dans le SaveTmp mais cet email etait envoyé
        {
            return daomail.deleteMailSaveTmpSended();
        }
        public int deleteMailGet(string mailId)//supprimer les émail reçu et dans le base de donnes il existe plus 
        {
            return daomail.deleteMailGet(mailId);
        }
        #endregion

        #region actionRemettre 
        public int remettreEmailDelete(string mailId)//remettre  les émail reçu dans le corbeille 
        {
            return daomail.remettreEmailDelete(mailId);
        }
        public static void remettreEmail(string mailNo)////supprimer les émail ce qui est dans le MailSaveTmp
        {
            daomail.remettreEmail(mailNo);
        }
        public static void cancelRemettreEmail()//annuler  les émail reçu   ce qui est dans le MailSaveTmp
        {
            daomail.cancelRemettreEmail();
        }
        #endregion 

        #region Send mail
        //3 type de smtp pour envoyer l'email par serveur 
        //Jmail smtp
        public void sentEmail(string adressFrom, string psw,string stradresseRecu , string stradresseCC,string strSujet ,string strbodymailhtml, string strbodymailtext, int indexPriorite)
        {

            Dimac.JMail.Message jmsg = new Dimac.JMail.Message();
            string[] adresseRecu = getListAdress(stradresseRecu);
            string[] adressCC = getListAdress(stradresseCC);
            string mailSujet = strSujet;
            for (int i = 0; i < adresseRecu.Length; i++)
            {
                if (!string.IsNullOrEmpty(adresseRecu[i]))
                    jmsg.To.Add(adresseRecu[i]);
            }
            for (int i = 0; i < adressCC.Length; i++)
            {
                if (!string.IsNullOrEmpty(adressCC[i]))
                    jmsg.Cc.Add(adressCC[i]);
            }
            jmsg.Subject = mailSujet;
            jmsg.From = adressFrom;
            jmsg.Charset = System.Text.Encoding.UTF8;
            string mailbody = HttpUtility.UrlDecode(strbodymailhtml);
            jmsg.BodyHtml = mailbody;
            jmsg.BodyText = strbodymailtext;
            if (WebUserControl1.Emaildatastatic.getInstance().getDataTable().Rows.Count > 0)
            {
                foreach (DataRow dr in WebUserControl1.Emaildatastatic.getInstance().getDataTable().Rows)
                {
                    jmsg.Attachments.Add(dr["filepath"].ToString());
                }
            }

            switch (indexPriorite)
            {
                case 0: jmsg.Priority = Dimac.JMail.Priority.Medium; break;
                case 1: jmsg.Priority = Dimac.JMail.Priority.High; break;
                case 2: jmsg.Priority = Dimac.JMail.Priority.Low; break;
            }
            try
            {
               

                Dimac.JMail.Smtp.Send(jmsg, "smtp.qq.com", short.Parse("25"), "qq.com", Dimac.JMail.SmtpAuthentication.Login, adressFrom, psw);

               
            }
            catch (Exception ex)
            {
                string errmsg = "An exception occured: " + ex.Message;
            }

          
        }
        //Vs smtp
        public void sentmails(string adressFrom, string psw, int port, string hostname, bool ssl, string stradresseRecu, string stradresseCC, string strSujet, string strbodymailhtml, int indexPriorite)
        {


            MailMessage mailMsg = new MailMessage();
            string[] adresseRecu = getListAdress(stradresseRecu);
            string[] adressCC = getListAdress(stradresseCC);
            string mailSujet = strSujet;
            for (int i = 0; i < adresseRecu.Length; i++)
            {
                if (!string.IsNullOrEmpty(adresseRecu[i]))
                    mailMsg.To.Add(adresseRecu[i]);
            }
            for (int i = 0; i < adressCC.Length; i++)
            {
                if (!string.IsNullOrEmpty(adressCC[i]))
                    mailMsg.CC.Add(adressCC[i]);
            }
            mailMsg.Subject = mailSujet;
            mailMsg.From = new MailAddress(adressFrom);

            string mailbody = HttpUtility.UrlDecode(strbodymailhtml);

            switch (indexPriorite)
            {
                case 0: mailMsg.Priority = MailPriority.Normal; break;
                case 1: mailMsg.Priority = MailPriority.High; break;
                case 2: mailMsg.Priority = MailPriority.Low; break;
            }

            mailMsg.Body = mailbody;
            mailMsg.BodyEncoding = Encoding.UTF8;
            mailMsg.IsBodyHtml = true;
            SmtpClient smtpc = new SmtpClient();
            smtpc.Credentials = new NetworkCredential(adressFrom, psw);
            smtpc.Port = port;
            smtpc.Host = hostname;
            smtpc.EnableSsl = ssl;
            if (WebUserControl1.Emaildatastatic.getInstance().getDataTable().Rows.Count > 0)
            {
                foreach (DataRow dr in WebUserControl1.Emaildatastatic.getInstance().getDataTable().Rows)
                {
                    
                    mailMsg.Attachments.Add(new Attachment(dr["filepath"].ToString()));
                }
            }


           
            try
            {
                smtpc.Send(mailMsg);
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            mailMsg.Dispose();
        







            Console.WriteLine("Goodbye.");
        }
        //lumisoft smtp
        public void sendMailSmtp(string adressFrom, string psw, int port, string hostname, bool ssl, string stradresseRecu, string stradresseCC, string strSujet, string strbodymailhtml, int indexPriorite)
        {
            var Host = new LumiSoft.Net.SMTP.Client.SMTP_Client();
            Host.Connect(hostname, port, ssl);
            Host.EhloHelo(hostname);
            Host.Auth(Host.AuthGetStrongestMethod(adressFrom, psw));
            LumiSoft.Net.Mail.Mail_Message mailMsg = new LumiSoft.Net.Mail.Mail_Message();
            mailMsg.MessageID = LumiSoft.Net.MIME.MIME_Utils.CreateMessageID();
            string[] adresseRecu = getListAdress(stradresseRecu);
            string[] adressCC = getListAdress(stradresseCC);
            string mailSujet = strSujet;

            for (int i = 0; i < adresseRecu.Length; i++)
            {
                if (!string.IsNullOrEmpty(adresseRecu[i]))
                    mailMsg.To = new LumiSoft.Net.Mail.Mail_t_AddressList();
                mailMsg.To.Add(new LumiSoft.Net.Mail.Mail_t_Mailbox(adresseRecu[i], adresseRecu[i]));
            }
            for (int i = 0; i < adressCC.Length; i++)
            {
                if (!string.IsNullOrEmpty(adressCC[i]))
                    mailMsg.Cc = new LumiSoft.Net.Mail.Mail_t_AddressList();
                mailMsg.Cc.Add(new LumiSoft.Net.Mail.Mail_t_Mailbox(adressCC[i], adressCC[i]));

            }
            mailMsg.Subject = mailSujet;
            mailMsg.From = new LumiSoft.Net.Mail.Mail_t_MailboxList();
            mailMsg.From.Add(new LumiSoft.Net.Mail.Mail_t_Mailbox(adressFrom, adressFrom));

            string mailbody = HttpUtility.UrlDecode(strbodymailhtml);

            switch (indexPriorite)
            {
                case 0: mailMsg.Priority = "normal"; break;
                case 1: mailMsg.Priority = "urgent"; break;
                case 2: mailMsg.Priority = "non-urgent"; break;
            }
            var body = new MIME_b_Text(MIME_MediaTypes.Text.html);
            mailMsg.Body = body; //il faut avoir l'autorisation faute de quoi il va envoyer  "Body must be bounded to some entity first" pour l'exception 
            body.SetText(MIME_TransferEncodings.Base64, Encoding.UTF8, mailbody);

            if (WebUserControl1.Emaildatastatic.getInstance().getDataTable().Rows.Count > 0)
            {
                foreach (DataRow dr in WebUserControl1.Emaildatastatic.getInstance().getDataTable().Rows)
                {
                  
                }
            }


           
            try
            {
                
            }
            catch (System.Net.Mail.SmtpException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            mailMsg.Dispose();
            Console.WriteLine("Goodbye.");
        }



        #endregion

        #region Save_Mail
        public static void saveMailTmp(string stradresseRecu, string stradresseCC, string strSujet, string strbodymailhtml)//sauvegarder  les  emails dans MailTmp
        {
            string mailto = stradresseRecu;
            string mailcc = stradresseCC;
            string mailsubject = strSujet;
            string mailbody = HttpUtility.UrlDecode(strbodymailhtml);
            string pathAttachmentFile = "";
            if (WebUserControl1.Emaildatastatic.getInstance().getDataTable().Rows.Count > 0)
            {
                foreach (DataRow dr in WebUserControl1.Emaildatastatic.getInstance().getDataTable().Rows)
                {
                    pathAttachmentFile = pathAttachmentFile + dr["filepath"].ToString() + ";";
                }
            }
            int no = daomail.getCountItem(sqlConnection_SystemEmail, @"MailSaveTmp", "") + 1;
            string dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            saveMailTmp(mailto, mailcc, pathAttachmentFile, no.ToString(), mailsubject, mailbody, dtime);
        }
        public static void saveMailTmp(string mailto, string mailcc, string pathAttachmentFile, string mailNo, string mailsubject, string mailbody, string maildatetime)//sauvegarder  les  emails dans MailTmp
        {
            daomail.saveMailTmp( mailto, mailcc, pathAttachmentFile, mailNo, mailsubject, mailbody, maildatetime);            
        }

        public static void saveMailBrouillons(string stradresseRecu, string stradresseCC, string strSujet, string strbodymailhtml)//sauvegarder  les  emails dans le boîte brouillon 
        {
            string mailto = stradresseRecu;
            string mailcc = stradresseCC;
            string mailsubject = strSujet;
            string mailbody = HttpUtility.UrlDecode(strbodymailhtml);
            string pathAttachmentFile = "";
            if (WebUserControl1.Emaildatastatic.getInstance().getDataTable().Rows.Count > 0)
            {
                foreach (DataRow dr in WebUserControl1.Emaildatastatic.getInstance().getDataTable().Rows)
                {
                    pathAttachmentFile = pathAttachmentFile + dr["filepath"].ToString() + ";";
                }
            }
            int no =daomail.getCountItem(sqlConnection_SystemEmail, @"MailSaveTmp", "") + 1;
            string dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            saveMailBrouillons(mailto, mailcc, pathAttachmentFile, no.ToString(), mailsubject, mailbody, dtime);
        }
        public static void saveMailBrouillons(string mailto, string mailcc, string pathAttachmentFile, string mailNo, string mailsubject, string mailbody, string maildatetime)//sauvegarder  les  emails dans le boîte brouillon 
        {
            daomail.saveMailBrouillons(mailto, mailcc, pathAttachmentFile, mailNo, mailsubject, mailbody, maildatetime);
        }
        public static void saveMailSend(string stradresseRecu, string stradresseCC, string strSujet, string strbodymailhtml)//sauvegarder  les  emails dans Messages envoyées
        {
            string mailto = stradresseRecu;
            string mailcc = stradresseCC;
            string mailsubject = strSujet;
            string mailbody = HttpUtility.UrlDecode(strbodymailhtml);
            string pathAttachmentFile = "";
            if (WebUserControl1.Emaildatastatic.getInstance().getDataTable().Rows.Count > 0)
            {
                foreach (DataRow dr in WebUserControl1.Emaildatastatic.getInstance().getDataTable().Rows)
                {
                    pathAttachmentFile = pathAttachmentFile + dr["filepath"].ToString() + ";";
                }
            }
            int no = daomail.getCountItem(sqlConnection_SystemEmail, @"MailSaveTmp", "") + 1;
            string dtime = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            saveMailTmp(mailto, mailcc, pathAttachmentFile, no.ToString(), mailsubject, mailbody, dtime);

        }
        #endregion

        #region getMail  
        public DataSet getMailList()//obtenir un tableau de Email reçu 
        {
            return daomail.getMailList();
        }
        public DataSet getMailList(string teststr, string teststr2)//obtenir un tableau de Email envoyé 
        {
            return daomail.getMailList(teststr,teststr2);
        }
        public DataSet getMailList(string id)////obtenir un tableau de Email reçu 
        {
            return daomail.getMailList(id);//??????????????????
           
        }
        public DataSet getMailDelete()//obtenir les émail  qui sont supprimé dans un tableau de Email reçu  
        {
            return daomail.getMailDelete();
            
        }



        public DataSet getmailSend()//obtenir un tableau de Email envoyé 
        {
            return daomail.getmailSend();
           
        }

        public DataSet getmailSend(string id)//obtenir un tableau de Email envoyé 
        {
            return daomail.getmailSend();
           
        }

        public static DataSet getmailSaveTmp()//obtenir un tableau de Email SaveTmp
        {
            return daomail.getmailSaveTmp();
        }

        public static DataSet getmailSaveBrouillons()//obtenir un tableau de Email qui sont stocké dans le boîte de brouillon 
        {
            return daomail.getmailSaveBrouillons();
        }

        public static DataSet getmailSaveAll()//obtenir un tableau de Email qui sont stocké dansMailsavetmp
        {
            return daomail.getmailSaveAll();
        }
        public static DataSet getmailRechercher(string id)//obtenir un tableau de Email recherche n'importe dans quelle boîte (reçu et envoyé )
        {
            return daomail.getmailRechercher(id);
        }
        public static DataSet getmailRechercher(string id,string datedebut,string dateau)//obtenir un tableau de Email recherche n'importe dans quelle boîte (reçu et envoyé )
        {
            return daomail.getmailRechercher(id,datedebut,dateau);
        }
        public static DataSet getmailRechercherFrom(string id)//obtenir un tableau de Email recherche dans la boîte reçu 
        {
            return daomail.getmailRechercherFrom(id);
        }
        public static DataSet getmailRechercherFrom(string id,string datedebut,string dateau)//obtenir un tableau de Email recherche dans la boîte reçu 
        {
            return daomail.getmailRechercherFrom(id,datedebut,dateau);
        }
        public static DataSet getmailRechercherTo(string id)//obtenir un tableau de Email recherche dans la boîte envoyé 
        {
            return daomail.getmailRechercherTo(id);
        }
        public static DataSet getmailRechercherTo(string id,string datedebut,string dateau)//obtenir un tableau de Email recherche dans la boîte envoyé 
        {
            return daomail.getmailRechercherTo(id,datedebut,dateau);
        }
        public static string getMailNomFrom(string id)//obtenir le nom de @émetteur  dans Email_reçu  
        {
            return daomail.getMailNomFrom(id);
        }
        public static string getMailNomTo(string id)//obtenir le nom de @destinataire   dans Email_reçu  
        {
            return daomail.getMailNomTo(id);
        }
        public static string getMailSendNomFrom(string id)//obtenir le nom de @émetteur  dans Email_envoyé  
        {
            return daomail.getMailSendNomFrom(id);
        }
        public static string getMailSendNomTo(string id)//obtenir le nom de @destinataire  dans Email_envoyé 
        {
            return daomail.getMailSendNomTo(id);
        }
     
         #endregion

        #region comum

        public static string[] getListAdress(string str)//obtenir plusieurs  adresses en même temps 
        {
            string[] listadress = new string[50];
            listadress = str.Split(';');
            return listadress;
        }

        public SqlDataAdapter CreerDataAdapter()//faire l'initialisation  de EmailGet dans la  base de donnés 
        {
            SqlConnection connection = new SqlConnection(sqlConnection_SystemEmail);
            SqlDataAdapter dataAdpater = new SqlDataAdapter( "SELECT * FROM [EmailGet]",connection);
            dataAdpater.InsertCommand = new SqlCommand(
            "INSERT INTO [EmailGet] (mailID,fk_userid,mailsender,mailfrom,mailto,mailcc,maildateTime,mailsubject,mailbodySimple,mailbody,pathAttachmentFile,NotSeen) " +
            "VALUES (@mailID, @fk_userid,@mailsender,@mailfrom,@mailto,@mailcc,@maildateTime,@mailsubject,@mailbodySimple,@mailbody,@pathAttachmentFile,@NotSeen)", connection);
          
            dataAdpater.InsertCommand.Parameters.Add("@mailID", SqlDbType.NVarChar, 50);
            dataAdpater.InsertCommand.Parameters.Add("@fk_userid", SqlDbType.Int, 4, "fk_userid");
            dataAdpater.InsertCommand.Parameters.Add("@mailsender", SqlDbType.NVarChar, 100, "mailsender");
            dataAdpater.InsertCommand.Parameters.Add("@mailfrom", SqlDbType.NVarChar, 50, "mailfrom");
            dataAdpater.InsertCommand.Parameters.Add("@mailto", SqlDbType.NVarChar, 4000, "mailto");
            dataAdpater.InsertCommand.Parameters.Add("@mailcc", SqlDbType.NVarChar, 4000, "mailcc");
            dataAdpater.InsertCommand.Parameters.Add("@maildateTime", SqlDbType.NVarChar, 20, "maildateTime");
            dataAdpater.InsertCommand.Parameters.Add("@mailsubject", SqlDbType.NVarChar, 50, "mailsubject");
            dataAdpater.InsertCommand.Parameters.Add("@mailbodySimple", SqlDbType.NVarChar, 50, "mailbodySimple");
            
            SqlParameter parameterInsertbody = dataAdpater.InsertCommand.Parameters.Add("@mailbody", SqlDbType.NVarChar);
            parameterInsertbody.SourceColumn = "mailbody";
            parameterInsertbody.SourceVersion = DataRowVersion.Original;
           
            dataAdpater.InsertCommand.Parameters.Add("@pathAttachmentFile", SqlDbType.NVarChar, 4000, "pathAttachmentFile");
            dataAdpater.InsertCommand.CommandTimeout = 500;
            dataAdpater.InsertCommand.Parameters.Add("@NotSeen", SqlDbType.Int, 4, "NotSeen");
            
           
            dataAdpater.UpdateCommand = new SqlCommand(
            "UPDATE [EmailGet] SET fk_userid=@fk_userid,mailsender=@mailsender,mailfrom=@mailfrom,mailto=@mailto,mailcc=@mailcc,maildateTime=@maildateTime,mailsubject=@mailsubject,mailbodySimple=@mailbodySimple,mailbody=@mailbody,pathAttachmentFile=@pathAttachmentFile,NotSeen=@NotSeen) " +
            "WHERE mailID=@mailID", connection);
            dataAdpater.UpdateCommand.Parameters.Add("@fk_userid", SqlDbType.Int, 4, "fk_userid");
            dataAdpater.UpdateCommand.Parameters.Add("@mailsender", SqlDbType.NVarChar, 100, "mailsender");
            dataAdpater.UpdateCommand.Parameters.Add("@mailfrom", SqlDbType.NVarChar, 50, "mailfrom");
            dataAdpater.UpdateCommand.Parameters.Add("@mailto", SqlDbType.NVarChar, 4000, "mailto");
            dataAdpater.UpdateCommand.Parameters.Add("@mailcc", SqlDbType.NVarChar, 4000, "mailcc");
            dataAdpater.UpdateCommand.Parameters.Add("@maildateTime", SqlDbType.NVarChar, 20, "maildateTime");
            dataAdpater.UpdateCommand.Parameters.Add("@mailsubject", SqlDbType.NVarChar, 50, "mailsubject");
            dataAdpater.UpdateCommand.Parameters.Add("@mailbodySimple", SqlDbType.NVarChar, 50, "mailbodySimple");
            dataAdpater.UpdateCommand.Parameters.Add("@NotSeen", SqlDbType.Int, 4, "NotSeen");
            SqlParameter parameterUpdatebody = dataAdpater.UpdateCommand.Parameters.Add("@mailbody", SqlDbType.NVarChar);
            parameterUpdatebody.SourceColumn = "mailbody";
            parameterUpdatebody.SourceVersion = DataRowVersion.Original;
            dataAdpater.UpdateCommand.Parameters.Add("@pathAttachmentFile", SqlDbType.NVarChar, 4000, "pathAttachmentFile");
            SqlParameter parameterUpdate = dataAdpater.UpdateCommand.Parameters.Add("@mailID", SqlDbType.NVarChar, 50);
            parameterUpdate.SourceColumn = "mailID";
            parameterUpdate.SourceVersion = DataRowVersion.Original;
            dataAdpater.UpdateCommand.CommandTimeout = 500;
           
            return dataAdpater;
        }

        public SqlDataAdapter CreerDataAdapterFormail(string baseTableName)//faire l'initialisation  de MailSend dans la  base de donnés 
        {
            SqlConnection connection = new SqlConnection(sqlConnection_SystemEmail);
            SqlDataAdapter dataAdpater = new SqlDataAdapter(
                "SELECT * FROM @baseTableName".Replace("@baseTableName", baseTableName),
                connection);
            dataAdpater.InsertCommand = new SqlCommand(
            "INSERT INTO @baseTableName (mailID,fk_userid,mailsender,mailfrom,mailto,mailcc,maildateTime,mailsubject,mailbodySimple,mailbody,pathAttachmentFile,NotSeen) ".Replace("@baseTableName", baseTableName) +
            "VALUES (@mailID, @fk_userid,@mailsender,@mailfrom,@mailto,@mailcc,@maildateTime,@mailsubject,@mailbodySimple,@mailbody,@pathAttachmentFile,@NotSeen)", connection);
            SqlParameter parameterInsert = dataAdpater.InsertCommand.Parameters.Add("@mailID", SqlDbType.NVarChar, 50);
            parameterInsert.SourceColumn = "mailID";
            parameterInsert.SourceVersion = DataRowVersion.Original;
            dataAdpater.InsertCommand.Parameters.Add("@fk_userid", SqlDbType.Int, 4, "fk_userid");
            dataAdpater.InsertCommand.Parameters.Add("@mailsender", SqlDbType.NVarChar, 100, "mailsender");
            dataAdpater.InsertCommand.Parameters.Add("@mailfrom", SqlDbType.NVarChar, 50, "mailfrom");
            dataAdpater.InsertCommand.Parameters.Add("@mailto", SqlDbType.NVarChar, 4000, "mailto");
            dataAdpater.InsertCommand.Parameters.Add("@mailcc", SqlDbType.NVarChar, 4000, "mailcc");
            dataAdpater.InsertCommand.Parameters.Add("@maildateTime", SqlDbType.NVarChar, 20, "maildateTime");
            dataAdpater.InsertCommand.Parameters.Add("@mailsubject", SqlDbType.NVarChar, 50, "mailsubject");
            dataAdpater.InsertCommand.Parameters.Add("@NotSeen", SqlDbType.Int, 4, "NotSeen");
            dataAdpater.InsertCommand.Parameters.Add("@mailbodySimple", SqlDbType.NVarChar, 50, "mailbodySimple");
            SqlParameter parameterInsertbody = dataAdpater.InsertCommand.Parameters.Add("@mailbody", SqlDbType.NVarChar);
            parameterInsertbody.SourceColumn = "mailbody";
            parameterInsertbody.SourceVersion = DataRowVersion.Original;
            dataAdpater.InsertCommand.Parameters.Add("@pathAttachmentFile", SqlDbType.NVarChar, 4000, "pathAttachmentFile");
            dataAdpater.InsertCommand.CommandTimeout = 500;
            //dataAdpater.InsertCommand.Parameters.Add("@NotSeen", SqlDbType.Int, 4, "pathAttachmentFile");
            //dataAdpater.InsertCommand.Parameters.Add("@Suppr", SqlDbType.Int, 4, "pathAttachmentFile");
            //dataAdpater.InsertCommand.Parameters.Add("@Important", SqlDbType.Int, 4, "pathAttachmentFile");
            dataAdpater.UpdateCommand = new SqlCommand(
            "UPDATE @baseTableName SET fk_userid=@fk_userid,mailsender=@mailsender,mailfrom=@mailfrom,mailto=@mailto,mailcc=@mailcc,maildateTime=@maildateTime,mailsubject=@mailsubject,mailbodySimple=@mailbodySimple,mailbody=@mailbody,pathAttachmentFile=@pathAttachmentFile,NotSeen=@NotSeen) ".Replace("@baseTableName", baseTableName) +
            "WHERE mailID=@mailID", connection);
            dataAdpater.UpdateCommand.Parameters.Add("@fk_userid", SqlDbType.Int, 4, "fk_userid");
            dataAdpater.UpdateCommand.Parameters.Add("@mailsender", SqlDbType.NVarChar, 100, "mailsender");
            dataAdpater.UpdateCommand.Parameters.Add("@mailfrom", SqlDbType.NVarChar, 50, "mailfrom");
            dataAdpater.UpdateCommand.Parameters.Add("@mailto", SqlDbType.NVarChar, 4000, "mailto");
            dataAdpater.UpdateCommand.Parameters.Add("@mailcc", SqlDbType.NVarChar, 4000, "mailcc");
            dataAdpater.UpdateCommand.Parameters.Add("@maildateTime", SqlDbType.NVarChar, 20, "maildateTime");
            dataAdpater.UpdateCommand.Parameters.Add("@mailsubject", SqlDbType.NVarChar, 50, "mailsubject");
            dataAdpater.UpdateCommand.Parameters.Add("@NotSeen", SqlDbType.Int, 4, "NotSeen");
            dataAdpater.UpdateCommand.Parameters.Add("@mailbodySimple", SqlDbType.NVarChar, 50, "mailbodySimple");
            SqlParameter parameterUpdatebody = dataAdpater.UpdateCommand.Parameters.Add("@mailbody", SqlDbType.NVarChar);
            parameterUpdatebody.SourceColumn = "mailbody";
            parameterUpdatebody.SourceVersion = DataRowVersion.Original;
            dataAdpater.UpdateCommand.Parameters.Add("@pathAttachmentFile", SqlDbType.NVarChar, 4000, "pathAttachmentFile");
            SqlParameter parameterUpdate = dataAdpater.UpdateCommand.Parameters.Add("@mailID", SqlDbType.NVarChar, 50);
            parameterUpdate.SourceColumn = "mailID";
            parameterUpdate.SourceVersion = DataRowVersion.Original;
            dataAdpater.UpdateCommand.CommandTimeout = 500;
            //dataAdpater.DeleteCommand = new SqlCommand(
            //    "DELETE FROM Customers WHERE CustomerID = @CustomerID", connection);
            //dataAdpater.(categoryTable);
            return dataAdpater;
        }

        public static void markMailAsSeen(string mailId)//faire le marque d'email qu'on a pas vu 
        {
            daomail.markMailAsSeen(mailId);           
        }
        public static bool IsUser(string user, string password) 
        {
            bool loginis = true;
            if (daomail.IsUser(user,password)!=null) { loginis = true; }
            else { loginis = false; }
            return loginis;
        }
        #endregion
        #region IsExist
        public static bool IsExistMailNomFrom(string text)
        {
            bool str = false;
            if (daomail.IsExistMailNomFrom(text) != 0) { str = true; }
            else if (daomail.IsExistMailNomFrom(text) == 0) { str = false; }
            return str;
        }
        public static bool IsExistMailNomTo(string text)
        {
            bool str = false;
            if (daomail.IsExistMailNomTo(text) != 0) { str = true; }
            else if (daomail.IsExistMailNomTo(text) == 0) { str = false; }
            return str;
        }
        public static bool IsExistMailNomCc(string text)
        {
            bool str = false;
            if (daomail.IsExistMailNomCc(text) != 0) { str = true; }
            else if (daomail.IsExistMailNomCc(text) == 0) { str = false; }
            return str;
        }
        public static bool IsExistMailNomSubject(string text)
        {
            bool str = false;
            if (daomail.IsExistMailNomSubject(text) != 0) { str = true; }
            else if (daomail.IsExistMailNomSubject(text) == 0) { str = false; }
            return str;
        }
        public static bool IsExistMailNomBodySimple(string text)
        {
            bool str = false;
            if (daomail.IsExistMailNomBodySimple(text) != 0) { str = true; }
            else if (daomail.IsExistMailNomBodySimple(text) == 0) { str = false; }
            return str;
        }
        public static bool IsExistMailNomBody(string text)
        {
            bool str = false;
            if (daomail.IsExistMailNomBody(text) != 0) { str = true; }
            else if (daomail.IsExistMailNomBody(text) == 0) { str = false; }
            return str;
        }
        public static bool IsExistMailSendFrom(string text)
        {
            bool str = false;
            if (daomail.IsExistMailSendFrom(text) != 0) { str = true; }
            else if (daomail.IsExistMailSendFrom(text) == 0) { str = false; }
            return str;
        }
        public static bool IsExistMailSendTo(string text)
        {
            bool str = false;
            if (daomail.IsExistMailSendTo(text) != 0) { str = true; }
            else if (daomail.IsExistMailSendTo(text) == 0) { str = false; }
            return str;
        }
        public static bool IsExistMailSendCc(string text)
        {
            bool str = false;
            if (daomail.IsExistMailSendCc(text) != 0) { str = true; }
            else if (daomail.IsExistMailSendCc(text) == 0) { str = false; }
            return str;
        }
        public static bool IsExistMailSendSubject(string text)
        {
            bool str = false;
            if (daomail.IsExistMailSendSubject(text) != 0) { str = true; }
            else if (daomail.IsExistMailSendSubject(text) == 0) { str = false; }
            return str;
        }
        public static bool IsExistMailSendBodySimple(string text)
        {
            bool str = false;
            if (daomail.IsExistMailSendBodySimple(text) != 0) { str = true; }
            else if (daomail.IsExistMailSendBodySimple(text) == 0) { str = false; }
            return str;
        }
        public static bool IsExistMailSendBody(string text)
        {
            bool str = false;
            if (daomail.IsExistMailSendBody(text) != 0) { str = true; }
            else if (daomail.IsExistMailSendBody(text) == 0) { str = false; }
            return str;
        }
        #endregion
    }

     

    public class datalistmail
    {
        private static datalistmail dtl;
        private DataTable dtmailGet = new DataTable();
        private DataTable dtmailSend = new DataTable();

        private datalistmail()
        {
            EmailClass rm = new EmailClass();
            SqlDataAdapter adapteremailrecu = rm.CreerDataAdapter();
            SqlDataAdapter adapteremailSend = rm.CreerDataAdapterFormail("MailSend");
            adapteremailrecu.Fill(dtmailGet);
            adapteremailSend.Fill(dtmailSend);
        }
        public static datalistmail getInstance()
        {
            if (dtl == null)
            {
                dtl = new datalistmail();
            }
            return dtl;
        }
        public DataTable getdt()
        {
            return dtmailGet;
        }
        public DataTable getdtSend()
        {
            return dtmailSend;
        }

        public void deleteDT()
        {
            dtl = null;
        }
    }
}