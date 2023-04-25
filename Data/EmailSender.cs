using System.Net;
using System.Net.Mail;
using CarRentalApp.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace CarRentalApp.Data
{
    public class EmailSender : IEmailSender
    {
        private readonly string _fromEmail;
        private readonly string _fromPassword;

        public EmailSender(IOptions<EmailSenderOptions> optionsAccessor)
        {
            _fromEmail = optionsAccessor.Value.FromEmail;
            _fromPassword = optionsAccessor.Value.FromPassword;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(_fromEmail);
            mailMessage.Subject = subject;
            mailMessage.To.Add(new MailAddress(email));
            mailMessage.Body = message;
            mailMessage.IsBodyHtml = true;

            using (var smtpClient = new SmtpClient("smtp.gmail.com", 587))
            {
                smtpClient.Credentials = new NetworkCredential(_fromEmail, _fromPassword);
                smtpClient.EnableSsl = true;
                await smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
}
