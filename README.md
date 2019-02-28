# ContactFormExample

Simple Contact form that uses the MailMessage and SmtpClient to send an email message to the setup variables in appsettings.json.

## appsettings.json
'''javascript
{
  "Logging": {
    "LogLevel": {
      "Default": "Warning"
    }
  },
  "AllowedHosts": "*",
  "EmailCredentials": {
    "EmailAddress": "ChangeToYourEmail@gmail.com",
    "AccountName": "YourName/AccountName",
    "SmtpClientAddress": "smtp.gmail.com",
    "Password": "PasswordToYourEmailAddress",
    "Port": 587
  }
}
'''

Nuget Package:
dotnet add package Microsoft.Extensions.Configuration.Binder --version 2.2.0
