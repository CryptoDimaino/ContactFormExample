﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ContactFormExample.Models;
using System.IO;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace ContactFormExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOptions<EmailCredentials> Config;

        public HomeController(IOptions<EmailCredentials> config)
        {
            Config = config;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("EmailAddress")]
        public string EmailAddress()
        {
            return Config.Value.EmailAddress;
        }

        [HttpGet("AccountName")]
        public string AccountName()
        {
            return Config.Value.AccountName;
        }

        [HttpGet("Password")]
        public string Password()
        {
            return Config.Value.Password;
        }

        [HttpGet("port")]
        public int Port()
        {
            return Config.Value.Port;
        }

        [HttpPost("SendEmail")]
        public IActionResult SendEmail(Contact contact)
        {
            if(ModelState.IsValid)
            {
                bool EmailSent = SendMail(contact.FullName, contact.Email, contact.Message, Config.Value.EmailAddress, Config.Value.AccountName, Config.Value.SmtpClientAddress, Config.Value.Password, Config.Value.Port);
                if(!EmailSent)
                {
                    ModelState.AddModelError("Email", "The form was not sent because of an invalid email address.");
                    return View("Index");
                }
                return RedirectToAction("Index");
            }
            return View("Index");
        }

        private bool SendMail(string Name, string EmailAddress, string Message, string ConfigEmail, string AccountName, string SmtpClientAddress, string ConfigPassword, int Port)
        {
            bool sent = true;
            using(MailMessage  Email = new MailMessage())
            {
                Email.From = new MailAddress(EmailAddress, Name);
                Email.To.Add(new MailAddress(ConfigEmail, AccountName));
                Email.Subject = $"Send from contact form";
                Email.Body = Message;
                Email.Priority = MailPriority.Normal;

                SmtpClient MailClient = null;
                try
                {
                    MailClient = new SmtpClient(SmtpClientAddress, Port);
                    MailClient.EnableSsl = true;
                    MailClient.Credentials = new System.Net.NetworkCredential(ConfigEmail, ConfigPassword);
                    MailClient.Send(Email);
                }
                catch(SmtpException ex)
                {
                    Console.WriteLine(ex);
                    sent = false;
                }
                finally
                {
                    if(MailClient is IDisposable)
                    {
                        MailClient.Dispose();
                    }
                }
            }
            return sent;
        }
    }
}
