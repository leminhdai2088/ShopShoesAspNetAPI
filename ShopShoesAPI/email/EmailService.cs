using MailKit.Net.Smtp;
using MimeKit;
using ShopShoesAPI.model;

namespace ShopShoesAPI.email
{
    public class EmailService : IEmail
    {
        private readonly EmailConfiguration _emailConfiguration;
        public EmailService(EmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;
        }
        public async Task SendEmail(MailMessage mailMessage)
        {
            var emailMessage = CreateMailMessage(mailMessage);
            await SendAync(emailMessage);
        }
        private MimeMessage CreateMailMessage(MailMessage mailMessage)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("email", _emailConfiguration.From));
            emailMessage.To.AddRange(mailMessage.To);
            emailMessage.Subject = mailMessage.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Text)
            {
                Text = mailMessage.Content
            };
            return emailMessage;
        }

        private async Task SendAync(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_emailConfiguration.SmtpServer, _emailConfiguration.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                await client.AuthenticateAsync(_emailConfiguration.Username, _emailConfiguration.Password);
                await client.SendAsync(mailMessage);
            }catch(Exception ex)
            {
                throw new BadHttpRequestException(ex.Message);
            }
            finally
            {
                await client.DisconnectAsync(true);
                client.Dispose();
            }
        }

    }
}
