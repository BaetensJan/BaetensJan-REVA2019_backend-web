using System;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IEmailSender
    {
        Task SendMailAsync(String to, String subject, String body);
    }
}