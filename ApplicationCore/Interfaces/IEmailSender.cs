using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IEmailSender
    {
        Task SendMailAsync(string to, string subject, string body, string[] ccs);
    }
}