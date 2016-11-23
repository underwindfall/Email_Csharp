

using LumiSoft.Net.SMTP.Client;
using LumiSoft.Net.AUTH;
using LumiSoft.Net.Mail;
using LumiSoft.Net.MIME;
public class UseLumiSoft : ISendMail
{
    private SMTP_Client Host { get; set; }
    private Mail_Message Mail { get; set; }
    public void CreateHost(ConfigHost host)
    {
        Host = new SMTP_Client();
        Host.Connect(host.Server, host.Port, host.EnableSsl);
        Host.EhloHelo(host.Server);
        Host.Auth(Host.AuthGetStrongestMethod(host.Username, host.Password));
    }
    public void CreateMail(ConfigMail mail)
    {
        Mail = new Mail_Message();
        Mail.Subject = mail.Subject;
        Mail.From = new Mail_t_MailboxList();
        Mail.From.Add(new Mail_t_Mailbox(mail.From, mail.From));
        Mail.To = new Mail_t_AddressList();
        foreach (var to in mail.To)
        { Mail.To.Add(new Mail_t_Mailbox(to, to)); }
        var body = new MIME_b_Text(MIME_MediaTypes.Text.html);
        Mail.Body = body; //Need to be assigned first or will throw "Body must be bounded to some entity first" exception.  
        body.SetText(MIME_TransferEncodings.Base64, Encoding.UTF8, mail.Body);
    }
    public void CreateMultiMail(ConfigMail mail)
    {
        CreateMail(mail);
        var contentTypeMixed = new MIME_h_ContentType(MIME_MediaTypes.Multipart.mixed);
        contentTypeMixed.Param_Boundary = Guid.NewGuid().ToString().Replace("-", "_");
        var multipartMixed = new MIME_b_MultipartMixed(contentTypeMixed);
        Mail.Body = multipartMixed;            //Create a entity to hold multipart/alternative body         
        var entityAlternative = new MIME_Entity();
        var contentTypeAlternative = new MIME_h_ContentType(MIME_MediaTypes.Multipart.alternative);
        contentTypeAlternative.Param_Boundary = Guid.NewGuid().ToString().Replace("-", "_");
        var multipartAlternative = new MIME_b_MultipartAlternative(contentTypeAlternative);
        entityAlternative.Body = multipartAlternative;
        multipartMixed.BodyParts.Add(entityAlternative);
        var entityTextPlain = new MIME_Entity();
        var plain = new MIME_b_Text(MIME_MediaTypes.Text.plain);
        entityTextPlain.Body = plain;
        plain.SetText(MIME_TransferEncodings.Base64, Encoding.UTF8, "If you see this message, it means that your mail client does not support html.");
        multipartAlternative.BodyParts.Add(entityTextPlain);
        var entityTextHtml = new MIME_Entity();
        var html = new MIME_b_Text(MIME_MediaTypes.Text.html);
        entityTextHtml.Body = html;
        html.SetText(MIME_TransferEncodings.Base64, Encoding.UTF8, mail.Body);
        multipartAlternative.BodyParts.Add(entityTextHtml);
        foreach (string attachment in mail.Attachments)
        { multipartMixed.BodyParts.Add(Mail_Message.CreateAttachment(attachment)); }
        foreach (string resource in mail.Resources)
        {
            var entity = new MIME_Entity();
            entity.ContentDisposition = new MIME_h_ContentDisposition(MIME_DispositionTypes.Inline);
            entity.ContentID = Convert.ToBase64String(Encoding.Default.GetBytes(Path.GetFileName(resource))); //eg.<img src="cid:ContentID"/>     
            var image = new MIME_b_Image(MIME_MediaTypes.Image.jpeg);
            entity.Body = image;
            image.SetDataFromFile(resource, MIME_TransferEncodings.Base64);
            multipartMixed.BodyParts.Add(entity);
        }
    }
    public void SendMail()
    {
        if (Host != null && Mail != null)
        {
            foreach (Mail_t_Mailbox from in Mail.From.ToArray())
            { Host.MailFrom(from.Address, -1); }
            foreach (Mail_t_Mailbox to in Mail.To)
            { Host.RcptTo(to.Address); }
            using (var stream = new MemoryStream())
            {
                Mail.ToStream(stream, new MIME_Encoding_EncodedWord(MIME_EncodedWordEncoding.Q, Encoding.UTF8), Encoding.UTF8);
                stream.Position = 0;//Need to be reset to 0, otherwise nothing will be sent;   
                Host.SendMessage(stream); Host.Disconnect();
            }
        }
        else
            throw new Exception("These is not a host to send mail or there is not a mail need to be sent.");
    }
}



