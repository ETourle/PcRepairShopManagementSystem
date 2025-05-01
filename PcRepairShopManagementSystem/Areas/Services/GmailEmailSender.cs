using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PcRepairShopManagementSystem.Models;

namespace PcRepairShopManagementSystem.Areas.Services
{
    public class GmailEmailSender : IEmailSender
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<GmailEmailSender> _logger;

        public GmailEmailSender(IOptions<EmailSettings> options, ILogger<GmailEmailSender> logger)
        {
            _settings = options.Value;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using var msg = new MailMessage
            {
                From = new MailAddress(_settings.UserName),
                Subject = subject,
                Body = htmlMessage,
                IsBodyHtml = true,
            };
            msg.To.Add(email);

            using var client = new SmtpClient(_settings.Host, _settings.Port)
            {
                Credentials = new NetworkCredential(_settings.UserName, _settings.Password),
                EnableSsl = _settings.EnableSSL,
                Timeout = _settings.Timeout
            };

            try
            {
                await client.SendMailAsync(msg);
            }
            catch (Exception ex)
            {
                // Log it, but don’t crash your registration page
                _logger.LogError(ex, "Failed to send confirmation email to {Email}", email);
            }
        }
    }
}
