using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using Microsoft.Extensions.Configuration;

namespace ApplicationCore.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendMailAsync(String to, String subject, String body)
        {
            try
            {
                var smtpClient = new SmtpClient()
                {
                    Host = _configuration.GetValue<String>("Email:Smtp:Host"),
                    Port = _configuration.GetValue<int>("Email:Smtp:Port"),
                    Credentials = new NetworkCredential(
                        _configuration.GetValue<String>("Email:Smtp:Username"),
                        _configuration.GetValue<String>("Email:Smtp:Password")
                    ),
                    EnableSsl = true
                };

                var mailMessage = new MailMessage();
                mailMessage.To.Add(to);
                mailMessage.Subject = subject;
                mailMessage.Body = body;
                mailMessage.IsBodyHtml = true;
                mailMessage.From = new MailAddress(_configuration.GetValue<String>("Email:Smtp:From"));


                Console.WriteLine("Attempting to send email...");
                await smtpClient.SendMailAsync(mailMessage);
                Console.WriteLine("Email sent!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("The email was not sent.");
                Console.WriteLine("Error message: " + ex.Message);
            }
        }
    }
}