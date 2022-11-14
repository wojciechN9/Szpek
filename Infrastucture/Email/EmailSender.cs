﻿using System;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MimeKit;

namespace Szpek.Infrastructure.Email
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Szpek.pl", "noreply@szpek.pl"));
            message.To.Add(new MailboxAddress("Customer", email));
            message.Subject = subject;
            message.Body = new TextPart("html")
            {
                Text = htmlMessage
            };

            try
            {
                using (var client = new SmtpClient())
                {
                    //SSL not working 
                    //await client.ConnectAsync("smtp.szpek.pl", 465, true);

                    //move to app.Settings
                    await client.ConnectAsync("smtp.szpek.pl", 587, MailKit.Security.SecureSocketOptions.None);
                    await client.AuthenticateAsync("noreply@szpek.pl", "szpekPOLEX1!");
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                };
            }
            catch (Exception ex)
            {
                throw new Exception("CANNOT_SEND_EMAIL_CONTACT_WITH_ADMINISTRATOR", ex);
            }
        }
    }
}
