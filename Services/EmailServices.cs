using System.Net;
using System.Net.Mail;

public class LocalhostEmailService
{
    public async Task SendAsync(string subject, string body, string toEmail, string toName = "", string fromEmail = "delivered@resend.dev", string fromName = "Equipe Jonatas")
    {
        string smtpServer = "smtp.resend.com";
        int smtpPort = 587;

        using MailMessage message = new();
        message.From = new MailAddress(fromEmail, fromName);
        message.To.Add(new MailAddress(toEmail, toName));
        message.Subject = subject;
        message.Body = body;
        message.IsBodyHtml = true;

        using (SmtpClient smtpClient = new(smtpServer, smtpPort))
        {
            smtpClient.Credentials = new NetworkCredential("resend", "re_6UZbiV6M_LG4gc2gdUu3s4QRwbabSLdmm");
            smtpClient.EnableSsl = true;

            try
            {
                smtpClient.Send(message);
                Console.WriteLine("Email enviado com sucesso.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao enviar o e-mail: " + ex.Message);
                Console.WriteLine("Stack Trace: " + ex.StackTrace);
            }
        }
    }
}