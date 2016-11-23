using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Mail;
using OpenPop.Pop3;
using OpenPop.Mime;
using System.Collections;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Net;
using OpenPop.Mime.Header;
using System.Data.SqlClient;
using LumiSoft.Net.IMAP.Client;
using LumiSoft.Net.IMAP;
using LumiSoft.Net.AUTH;
using LumiSoft.Net.Mail;
using LumiSoft.Net.MIME;
using LumiSoft.Net;
using System.Threading;






namespace Email_Test
{
    public partial class recevoirmail : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            GridView1.DataSource = getMailList().Tables[0];
            GridView1.DataBind();
        }

        public class datalistmail
        {
            private static datalistmail dtl;
            private DataTable dtmail = new DataTable();
            private DataTable dtmailSend = new DataTable();

            private datalistmail()
            {
                recevoirmail rm = new recevoirmail();
                SqlDataAdapter adapteremailrecu = rm.CreerDataAdapter();
                SqlDataAdapter adapteremailSned = rm.CreerDataAdapterFormail("MailSend");
                adapteremailrecu.Fill(dtmail);
                adapteremailSned.Fill(dtmailSend);
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
                return dtmail;
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
        }
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
        }
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
        }

        private void m_pImap_Fetch_MessageItems_UntaggedResponse(object sender, EventArgs<IMAP_r_u> e)
        {
            DataTable dtmail=null;

            switch (Defaut_Start.flagViewLast)
            {
                case Defaut_Start.ViewEtat.ViewReceive: dtmail = datalistmail.getInstance().getdt();
                    break;
                case Defaut_Start.ViewEtat.ViewSend: dtmail = datalistmail.getInstance().getdtSend();
                    break;
                case Defaut_Start.ViewEtat.ViewSuppr:
                    break;
                case Defaut_Start.ViewEtat.ViewImport:
                    break;
                case Defaut_Start.ViewEtat.ViewBrouillon:
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
                        string dir = "C:\\Users\\Sipcom-pc-info\\Documents\\Visual Studio 2012\\Projects\\Email_Test\\Email_Test\\attchment\\";
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
                //imapc.SelectFolder("INBOX");
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
                //System.Threading.Thread.Sleep(500);
                imapc.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine("ErrGetMail:" + ex.Message);
            }
        }



        public void receiveMail(string userName, string psw, string service)
        {
            DataTable dtmail = new DataTable();
            SqlDataAdapter adapteremailrecu = CreerDataAdapter();
            adapteremailrecu.Fill(dtmail);
            //dtmail.Columns.Add(new DataColumn("mailsender", typeof(string)));
            //dtmail.Columns.Add(new DataColumn("mailfrom", typeof(string)));
            //dtmail.Columns.Add(new DataColumn("mailto", typeof(string)));
            //dtmail.Columns.Add(new DataColumn("mailcc", typeof(string)));
            //dtmail.Columns.Add(new DataColumn("maildateTime", typeof(string)));
            //dtmail.Columns.Add(new DataColumn("mailsubject", typeof(string)));
            //dtmail.Columns.Add(new DataColumn("mailbodySimple", typeof(string)));
            //dtmail.Columns.Add(new DataColumn("mailbody", typeof(string)));
            //dtmail.Columns.Add(new DataColumn("pathAttachmentFile", typeof(string)));
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
                            // The message had a text/plain version - show that one
                            //TextBox1.Text = plainTextPart.GetBodyAsText();
                            body = plainHtmlPart.GetBodyAsText();
                            bodys = plainTextPart.GetBodyAsText();
                            //byte[] bodyy = plainTextPart.MessageParts[0].MessageParts.ToArray();
                            //string html = Encoding.GetEncoding("gb18030").GetString(bodyy);
                        }
                        else
                        {
                            // Try to find a body to show in some of the other text versions
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
                        string dir = Server.MapPath("~/attchment/");
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
                                    //MessageBox.Show("Web服务器文件目前不可写入，请检查Web服务器目录权限设置！","系统提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
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
        }

        //,@NotSeen,@Suppr,@Important
        public SqlDataAdapter CreerDataAdapter()
        {
            SqlConnection connection = new SqlConnection("server=localhost;uid=sa;pwd=123456;database=test");
            SqlDataAdapter dataAdpater = new SqlDataAdapter(
              "SELECT * FROM Email",
              connection);
            dataAdpater.InsertCommand = new SqlCommand(
           "INSERT INTO Email (mailID,fk_userid,mailsender,mailfrom,mailto,mailcc,maildateTime,mailsubject,mailbodySimple,mailbody,pathAttachmentFile,NotSeen) " +
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
            dataAdpater.InsertCommand.Parameters.Add("@mailbodySimple", SqlDbType.NVarChar, 50, "mailbodySimple");
            SqlParameter parameterInsertbody = dataAdpater.InsertCommand.Parameters.Add("@mailbody", SqlDbType.NVarChar);
            parameterInsertbody.SourceColumn = "mailbody";
            parameterInsertbody.SourceVersion = DataRowVersion.Original;
            dataAdpater.InsertCommand.Parameters.Add("@pathAttachmentFile", SqlDbType.NVarChar, 4000, "pathAttachmentFile");
            dataAdpater.InsertCommand.CommandTimeout = 500;
            dataAdpater.InsertCommand.Parameters.Add("@NotSeen", SqlDbType.Int, 4, "NotSeen");
            //dataAdpater.InsertCommand.Parameters.Add("@Suppr", SqlDbType.Int, 4, "pathAttachmentFile");
            //dataAdpater.InsertCommand.Parameters.Add("@Important", SqlDbType.Int, 4, "pathAttachmentFile");
            dataAdpater.UpdateCommand = new SqlCommand(
          "UPDATE Email SET fk_userid=@fk_userid,mailsender=@mailsender,mailfrom=@mailfrom,mailto=@mailto,mailcc=@mailcc,maildateTime=@maildateTime,mailsubject=@mailsubject,mailbodySimple=@mailbodySimple,mailbody=@mailbody,pathAttachmentFile=@pathAttachmentFile,NotSeen=@NotSeen) " +
          "WHERE mailID=@mailID", connection);
            dataAdpater.UpdateCommand.Parameters.Add("fk_userid", SqlDbType.Int, 4, "fk_userid");
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
            //dataAdpater.DeleteCommand = new SqlCommand(
            //    "DELETE FROM Customers WHERE CustomerID = @CustomerID", connection);
            //dataAdpater.(categoryTable);
            return dataAdpater;
        }

        public SqlDataAdapter CreerDataAdapterFormail(string baseTableName)
        {
            SqlConnection connection = new SqlConnection("server=localhost;uid=sa;pwd=123456;database=test");
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
        //public string RemplaceUrlImage(string path, string pathattchment)
        //{
        //    string pathtmp=path;
        //    string  pathimg="/attchment/";
        //    string tmp = @"img src=";
        //    int count = 0;
        //    int start = 0;
        //    while (start != -1)
        //    {
        //        start = pathtmp.IndexOf(tmp, start);//获取字符的索引
        //        if (start != -1)
        //        {
        //            count++;
        //            start++;
        //        }
        //    }
        //    if (count != 0) {
        //        string endchar=@">";
        //        int placebegin = 0;
        //        int placeend = 0;
        //        for (int i = 0; i < count; i++) {
        //            placebegin = pathtmp.IndexOf(tmp, placebegin);
        //            placeend = pathtmp.IndexOf(endchar, placebegin);
        //            string imgname=
        //            pathtmp = pathtmp.Remove(placebegin + 9, placeend - placebegin - 10);
        //            pathtmp = pathtmp.Insert(placebegin + 9, pathimg);
        //            placebegin = placeend;
        //        }







        //    }
        //    return pathtmp;
        //}

        public DataSet getMailList()
        {
            DataSet ds = new DataSet();
            SqlConnection connection = new SqlConnection("server=localhost;uid=sa;pwd=123456;database=test");
            SqlDataAdapter dataAdpater = new SqlDataAdapter(
              "SELECT * FROM Email where Suppr=0 order by  maildateTime  DESC , mailID DESC",
              connection);
            dataAdpater.Fill(ds);
            return ds;
        }
        public DataSet getMailList(string id)
        {
            DataSet ds = new DataSet();
            SqlConnection connection = new SqlConnection("server=localhost;uid=sa;pwd=123456;database=test");
            SqlDataAdapter dataAdpater = new SqlDataAdapter(
              "SELECT * FROM Email Where mailID='@id'".Replace("@id", id),
              connection);
            dataAdpater.Fill(ds);
            return ds;
        }

        public DataSet getmailSend()
        {
            DataSet ds = new DataSet();
            SqlConnection connection = new SqlConnection("server=localhost;uid=sa;pwd=123456;database=test");
            SqlDataAdapter dataAdpater = new SqlDataAdapter(
              "SELECT * FROM MailSend where Suppr=0 order by  maildateTime  DESC , mailID DESC",
              connection);
            dataAdpater.Fill(ds);
            return ds;
        }

        public DataSet getmailSend(string id)
        {
            DataSet ds = new DataSet();
            SqlConnection connection = new SqlConnection("server=localhost;uid=sa;pwd=123456;database=test");
            SqlDataAdapter dataAdpater = new SqlDataAdapter(
              "SELECT * FROM MailSend Where mailID='@id'".Replace("@id", id),
              connection);
            dataAdpater.Fill(ds);
            return ds;
        }





        protected void Button1_Click(object sender, EventArgs e)
        {
            //receiveMail("zhangziyu830@gmail.com", "zhang882451272", "pop.gmail.com");
            //getMailImap("zhangziyu830@gmail.com", "zhang882451272", "imap.gmail.com", true, 993);
            //getMailSendImap("zhangziyu830@gmail.com", "zhang882451272", "imap.gmail.com", true, 993);
            getMailImap("test@sipcom.fr", "test123", "imap.1and1.fr", true, 993);
        }

        protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            GridView1.PageIndex = e.NewPageIndex;
            GridView1.DataSource = getMailList().Tables[0];
            GridView1.DataBind();

        }

        //protected int InsertData(DataTable dt, string strTblName)
        //{
        //    int nbr = 0;
        //    DataSet ds = new DataSet();
        //    ds.Tables.Add(dt);
        //    //SqlCommand SelectCmd = new SqlCommand(SelectSQL, conn);
        //    //string  InsertSQL = "insert into tblName (Input_Time,Location_ID) values (@Input_Time,@Location_ID)";
        //    //SqlCommand InsertCmd = new SqlCommand(InsertSQL, Con);
        //    //InsertCmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@Input_Time", System.Data.SqlDbType.Time));
        //    //InsertCmd.Parameters.Add(new System.Data.SqlClient.SqlParameter("@Location_ID", System.Data.SqlDbType.NVarChar));
        //    //SqlDataAdapter Dad = new SqlDataAdapter();
        //    //Dad.SelectCommand = SelectCmd;
        //    //Dad.InsertCommand = InsertCmd;
        //    //Dad.Update(ds); 
        //    try
        //    {
        //        SqlConnection conn = new SqlConnection("server=localhost;uid=sa;pwd=123456;database=test");
        //        string SelectSQL = "select * from Email";
        //        SqlDataAdapter myAdapter = new SqlDataAdapter();
        //        SqlCommand myCommand = new SqlCommand(SelectSQL, conn);
        //        myAdapter.SelectCommand = myCommand;
        //        SqlCommandBuilder myCommandBuilder = new SqlCommandBuilder(myAdapter);
        //        myAdapter.Update(ds, strTblName);
        //    }
        //    catch (Exception err)
        //    {
        //        ;
        //    }
        //    return nbr;
        //}






    }
}