using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;

namespace Galileo
{
    public class Email
    {

        public static void Correo(string Remitente,int ID)
        {

            SmtpClient SmtpServer = new SmtpClient("mail.ppenaw.com");
            var mail = new MailMessage();
            mail.From = new MailAddress("apl_mx@ppenaw.com");
            mail.To.Add(Remitente);
            mail.CC.Add("aponcedeleon51@hotmail.com");
            mail.CC.Add("a.ponce@aec.mx");
            mail.Subject = "Registro Galileo OMR encuestado: " + ID.ToString();
            mail.IsBodyHtml = true;
            string htmlbody = "En breve su registro será validado  . Gracias";
            mail.Body = htmlbody;
            SmtpServer.Port = 8889;
            SmtpServer.UseDefaultCredentials = true;
            SmtpServer.Credentials = new System.Net.NetworkCredential("apl_mx@ppenaw.com", "Pru3b4_");
            SmtpServer.EnableSsl = false;
            SmtpServer.Send(mail);

        }
    
    }



}