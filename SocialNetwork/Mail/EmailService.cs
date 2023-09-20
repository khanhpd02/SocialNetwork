using SocialNetwork.Mail;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace firstapi.Service
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings emailSettings;
        public EmailService(IOptions<EmailSettings> options) { 
            this.emailSettings=options.Value;
        }

        public string GetHtmlcontent(string bodystring)
        {
            string Response = "<div style=\"width:100%;background-color:lightblue;text-align:center;margin:10px\">";
            Response += "<h1>Welcome to KCT Network</h1>";
            /*string imagePath = "D:\\TLCN\\SocialNetwork\\SocialNetwork\\Upload\\Logos.png";
            Response += "<img src='" + imagePath + "' alt='Hình ảnh của bạn' />";*/
            Response += "<img src=\"https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcShkXjvbSbR2iZy9RZgRNZQIP9dthW5iIHvYg&usqp=CAU\" />";
            Response += "<h2>"+bodystring+"</h2>";
            Response += "<a href=\"https://www.youtube.com/channel/UCv1YqTpyeGUzoeSv4YBrsjQ\">Please join membership by click the link</a>";
            Response += "<div><h1> Contact us : kctsocialnetwork@gmail.com</h1></div>";
            Response += "</div>";
            return Response;
        }

        public async Task SendEmailAsync(Mailrequest mailrequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(emailSettings.Email);
            email.To.Add(MailboxAddress.Parse(mailrequest.ToEmail));
            email.Subject=mailrequest.Subject;
            var builder = new BodyBuilder();
          

            byte[] fileBytes; 
            if (System.IO.File.Exists("Logo/dummy.pdf"))
            {
                FileStream file = new FileStream("Logo/dummy.pdf", FileMode.Open, FileAccess.Read);
                using(var ms=new MemoryStream())
                {
                    file.CopyTo(ms);
                    fileBytes=ms.ToArray();
                }
               // builder.Attachments.Add("attachment.pdf", fileBytes, ContentType.Parse("application/octet-stream"));
                //builder.Attachments.Add("attachment2.pdf", fileBytes, ContentType.Parse("application/octet-stream"));
            }

            builder.HtmlBody = mailrequest.Body;
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(emailSettings.Host, emailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(emailSettings.Email, emailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
