using DiplomaApi.ModelsDto;
using MailKit.Net.Smtp;
using MimeKit;
using System.Text.RegularExpressions;

namespace DiplomaApi.Services
{
    public class SmtpService : ISmtpService
    {
        private readonly ISmtpSettings _smtpSettings;

        public SmtpService(ISmtpSettings smtpSettings)
        {
            _smtpSettings = smtpSettings;
        }

        private MimeMessage CreateMessage(string url, SendDto smtp)
        {
            var rawhtml = string.Empty;
            try
            {
                using (StreamReader reader = new StreamReader($"{Directory.GetCurrentDirectory()}/Smtp/Verify.html"))
                    rawhtml = reader.ReadToEnd();

                rawhtml = Regex.Replace(rawhtml, @"" + $"%URL%" + "", $"{url}");
                rawhtml = Regex.Replace(rawhtml, @"" + $"%HANDLE%" + "", $"{smtp.Login}");

                var result = new MimeMessage();
                result.From.Add(new MailboxAddress($"Guester", _smtpSettings.AuthUser));
                result.To.Add(new MailboxAddress("User", smtp.Login));

                result.Subject = string.IsNullOrEmpty(smtp.Subject) ? "Guester" : smtp.Subject;

                var htmlBody = new BodyBuilder();
                htmlBody.HtmlBody = rawhtml;

                result.Body = htmlBody.ToMessageBody();
                return result;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool SendMessage(string url, SendDto smtp)
        {
            try
            {
                using var message = CreateMessage(url, smtp);
                using (var client = new SmtpClient())
                {
                    client.Connect(_smtpSettings.ConnectionString, 465, true);

                    client.Authenticate(_smtpSettings.AuthUser, _smtpSettings.AuthPassword);

                    client.Send(message);
                    client.Disconnect(true);
                }
                return true;
            }
            catch (Exception ex)
            {
                if (ex is Exception)
                {
                    throw ex;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
