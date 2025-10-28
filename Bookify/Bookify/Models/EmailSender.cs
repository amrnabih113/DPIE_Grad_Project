using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailSender : IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var mail = new MailMessage();
        mail.To.Add(email);
        mail.From = new MailAddress("amrnabih112@gmail.com"); //  ده الميل بتاعي الي هيتبعت منه الايميلات
        mail.Subject = subject;
        mail.Body = htmlMessage;
        mail.IsBodyHtml = true;

        using var smtp = new SmtpClient("smtp.gmail.com", 587)
        {
            Credentials = new NetworkCredential("amrnabih112@gmail.com", "eqmgjmuiljnwzvrv"),
            EnableSsl = true
        };
        await smtp.SendMailAsync(mail);
    }
}
