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
                if(!EmailSent)
                {
                    ModelState.AddModelError("Email", "The form was not sent because of an invalid email address.");
                    return View("Index");
                }
                return RedirectToAction("Index");
            }
            return View("Index");
        }

        private bool SendMail(string Name, string Email1, string Message, string ConfigEmail, string AccountName, string SmtpClientAddress, string ConfigPassword, int Port)
        {
            Console.WriteLine(Email1);
            bool sent = true;
            using(MailMessage Email = new MailMessage())
            {
                Email.From = new MailAddress(Email1, Name);
                Email.To.Add(new MailAddress(ConfigEmail, AccountName));
                Email.Subject = $"Contact Form - dimaino.com - {Email1}";
                Email.Body = Message;
                Email.Priority = MailPriority.High;
                Email.IsBodyHtml = true;

                // SmtpClient MailClient = null;
                // SmtpClient MailClient = new SmtpClient();
                using (var MailClient = new SmtpClient(Config.Value.SmtpClientAddress))
                {
                    MailClient.Port = 587;
                    MailClient.Credentials = new NetworkCredential(ConfigEmail, ConfigPassword);
                    MailClient.EnableSsl = true;
                    MailClient.Send(Email);
                }
                // try
                // {
                //     // MailClient = new SmtpClient(SmtpClientAddress, Port);
                //     MailClient.Credentials = new NetworkCredential(ConfigEmail, ConfigPassword, Config.Value.SmtpClientAddress);
                //     MailClient.Host = Config.Value.SmtpClientAddress;
                //     MailClient.Port = Port;
                //     MailClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                //     MailClient.EnableSsl = true;
                //     MailClient.UseDefaultCredentials = false;
                //     // MailClient.Credentials = new System.Net.NetworkCredential(ConfigEmail, ConfigPassword, Config.Value.SmtpClientAddress);
                //     MailClient.Send(Email);
                // }
                // catch(SmtpException ex)
                // {
                //     Console.WriteLine(ex);
                //     sent = false;
                // }
                // finally
                // {
                //     if(MailClient is IDisposable)
                //     {
                //         MailClient.Dispose();
                //     }
                // }
            }
            return sent;
        }
    }
}
