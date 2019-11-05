using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ContactFormExample.Models;
using System.IO;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using System.Reflection;
using Microsoft.AspNetCore.Hosting;
using System.Net;

namespace ContactFormExample.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOptions<EmailCredentials> Config;
        private readonly IHostingEnvironment _hostingEnvironment;

        public HomeController(IOptions<EmailCredentials> config, IHostingEnvironment hostingEnvironment)
        {
            Config = config;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            if(Config.Value.EmailAddress == null || Config.Value.AccountName == null || Config.Value.SmtpClientAddress == null || Config.Value.Password == null || Config.Value.Port.ToString() == null)
            {
                ViewBag.AppSettingsExists = "Some Problems Yo!";
            }
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

        // Sends an email as long as the modelstate is true. Then calls the SendMail Method and reads in important information 
        // that was recieved from the model form. It also reads in important information from the appsettings.json file.
        [HttpPost("SendEmail")]
        public IActionResult SendEmail(Contact contact)
        {
            if(ModelState.IsValid)
            {
                string AppSettingsPath = Path.Combine(_hostingEnvironment.ContentRootPath, "appsettings.json");
                if(!System.IO.File.Exists(AppSettingsPath))
                {
                    ViewBag.AppSettingsExists = "You have not created an appsettings.json file.";
                    return View("Index");
                }

                bool EmailSent = SendMail(contact.FullName, contact.Email1, contact.Message, Config.Value.EmailAddress, Config.Value.AccountName, Config.Value.SmtpClientAddress, Config.Value.Password, Config.Value.Port);
                SendMailToSender(contact.FullName, contact.Email1, contact.Message, Config.Value.EmailAddress, Config.Value.AccountName, Config.Value.SmtpClientAddress, Config.Value.Password, Config.Value.Port);
                if(!EmailSent)
                {
                    ModelState.AddModelError("Email", "The form was not sent because of an invalid email address.");
                    return View("Index");
                }
                return RedirectToAction("Index");
            }
            return View("Index");
        }

        // This is a private method that tries to deliver an email to the host provided information with the person trying to contact the webhost.
        private bool SendMail(string Name, string Email1, string Message, string ConfigEmail, string AccountName, string SmtpClientAddress, string ConfigPassword, int Port)
        {
            string websiteName = ".com";
            bool sent = true;
            using(MailMessage Email = new MailMessage())
            {
                Email.From = new MailAddress(Email1, Name);
                Email.To.Add(new MailAddress(ConfigEmail, AccountName));
                Email.Subject = $"Contact Form - {websiteName} - {Email1}";
                Email.Body = Message;
                Email.Priority = MailPriority.High;
                Email.IsBodyHtml = true;

                try
                {
                    using (var MailClient = new SmtpClient(SmtpClientAddress))
                    {
                        MailClient.Port = 587;
                        MailClient.Credentials = new NetworkCredential(ConfigEmail, ConfigPassword);
                        MailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        MailClient.EnableSsl = true;
                        MailClient.Send(Email);
                    }
                }
                catch(SmtpException ex)
                {
                    Console.WriteLine(ex);
                    sent = false;
                }
            }
            return sent;
        }

        private bool SendMailToSender(string Name, string Email1, string Message, string ConfigEmail, string AccountName, string SmtpClientAddress, string ConfigPassword, int Port)
        {
            bool sent = true;
            using(MailMessage Email = new MailMessage())
            {
                Email.From = new MailAddress(ConfigEmail, AccountName);
                Email.To.Add(new MailAddress(Email1, Name));
                Email.Subject = $"Automated Reply from .com";
                Email.Body = "<h1 \"style=text-align: center;\">Automated reply from .com</h1>" + 
                "<br>" +
                $"<p>Hello {Name}, thanks so much for reaching out! I will respond to your email as soon as I " +
                "am able to see it.</p>" +
                "<br>" +
                "<p>If you did not use the contact me feature on .com please ignore this email.</p>";
                Email.Priority = MailPriority.High;
                Email.IsBodyHtml = true;

                try
                {
                    using (var MailClient = new SmtpClient(SmtpClientAddress))
                    {
                        MailClient.Port = 587;
                        MailClient.Credentials = new NetworkCredential(ConfigEmail, ConfigPassword);
                        MailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                        MailClient.EnableSsl = true;
                        MailClient.Send(Email);
                    }
                }
                catch(SmtpException ex)
                {
                    Console.WriteLine(ex);
                    sent = false;
                }
            }
            return sent;
        }        
    }
}
