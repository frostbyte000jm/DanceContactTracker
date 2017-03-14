using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Mail;
using System.Windows.Forms;

namespace DanceContactTracker
{
    class Email
    {
        //declarations
        private NetworkCredential myCreds;
        private SmtpClient client;
        private MailAddress from;
        private MailMessage message;

        public Email(string emailAdd, string name, string subject, string body)
        {
            //This is just creating the class.
            myCreds = new NetworkCredential("djanddancepro@gmail.com", "Sw1ngsat10n", "");
            client = new SmtpClient("smtp.gmail.com", 587);
            from = new MailAddress("djanddancepro@gmail.com", "Joe & Shari Tessier");

            //dedicate who this e-mail is going to.
            MailAddress to = new MailAddress(emailAdd, name);
            message = new MailMessage(from, to);

            //Login to your e-mail system.
            client.Credentials = myCreds;
            client.EnableSsl = true;
            message.IsBodyHtml = true;

            //This is where we create the e-mail.
            message.Subject = subject;
            message.Body = body;
        }

        public void Send()
        {   
            //Just in case they are not connected.
            client.Send(message);
        }
    }
}
