using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WALL_R.Models;
using System.IO;
using System.Net.Mail;

namespace WALL_R.Libraries
{
    public static class MailManager
    // partially copied from user documentary: https://stackoverflow.com/questions/18326738/how-to-send-email-in-asp-net-c-sharp
    {
        public static bool SendRepairMail(string room_number, string device_name, string owner_mail)
        // sends repair mail to the connected workshop
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient("mail.wallr.com", 25);

                smtpClient.Credentials = new System.Net.NetworkCredential("noreply@wallr.com", "placeholder");
                smtpClient.UseDefaultCredentials = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.EnableSsl = true;
                MailMessage mail = new MailMessage();

                //Setting From , To and CC
                mail.From = new MailAddress("info@MyWebsiteDomainName", "MyWeb Site");
                mail.To.Add(new MailAddress("info@MyWebsiteDomainName"));
                mail.CC.Add(new MailAddress("MyEmailID@gmail.com"));
                mail.Body =
                    "Sehr geehrte Damen und Herren,\n" +
                    "im Raum \"" + room_number + "\" am Gerät \"" + device_name + "\"kam es zu einem Fehler, der durch den Raumbetreuer " +
                    "nicht zu beheben ist. Wir bitten Sie daher darum, das Problem zu analysieren und nach Möglichkeit zu beheben.\n" +
                    "Ihre Rückmeldung richten Sie bitte an die E-Mail-Adresse des Raumbetreuers: " + owner_mail + "\n" +
                    "\n" +
                    "Mit freundlichen Grüßen\n" +
                    "Ihr WALL.R-Service\n" +
                    "\n\n" +
                    "Dies ist eine automatische Nachricht. Bitte nicht an diese Adresse antworten.";

                smtpClient.Send(mail);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
