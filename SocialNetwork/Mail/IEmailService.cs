using SocialNetwork.Mail;
namespace firstapi.Service
{
    public interface IEmailService
    {
        Task SendEmailAsync(Mailrequest mailrequest);
        string GetHtmlcontent(String bodystring);
        
    }
}
