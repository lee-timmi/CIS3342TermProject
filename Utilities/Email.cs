using System;
using System.Net.Mail;

namespace Utilities
{
    // Sends an email through Temple's mail server.
    // Same public API as Prof. Pascucci's Email class (CIS-3342 site, "Email Example": Email_class.htm):
    //   Email objEmail = new Email();
    //   objEmail.SendMail(recipient, sender, subject, body);   // throws an exception if it fails
    // Difference: a new MailMessage is built for every SendMail call, so one Email object can send
    // several messages without the recipients piling up.
    public class Email
    {
        private string mailHost = "smtp.temple.edu";
        private bool htmlBody = true;
        private MailPriority priority = MailPriority.Normal;

        public string MailHost
        {
            get { return mailHost; }
            set { mailHost = value; }
        }

        public bool HTMLBody
        {
            get { return htmlBody; }
            set { htmlBody = value; }
        }

        public MailPriority Priority
        {
            get { return priority; }
            set { priority = value; }
        }

        // Sends one email. cc / bcc are optional. Throws an exception if the email can't be sent.
        public void SendMail(String recipient, String sender, String subject, String body, String cc = "", String bcc = "")
        {
            MailMessage objMail = new MailMessage();
            objMail.To.Add(new MailAddress(recipient));
            objMail.From = new MailAddress(sender);
            if (cc != "")
            {
                objMail.CC.Add(new MailAddress(cc));
            }
            if (bcc != "")
            {
                objMail.Bcc.Add(new MailAddress(bcc));
            }
            objMail.Subject = subject;
            objMail.Body = body;
            objMail.IsBodyHtml = this.htmlBody;
            objMail.Priority = this.priority;

            SmtpClient smtpMailClient = new SmtpClient(this.mailHost);
            smtpMailClient.Send(objMail);
        }
    }
}
