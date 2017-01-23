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


        public static void Correo2(string Remitente)
        {
            SmtpClient SmtpServer = new SmtpClient("smtp.live.com");
            var mail = new MailMessage();
            mail.From = new MailAddress("aponcedeleon51@hotmail.com.com");
            mail.To.Add(Remitente);
            mail.CC.Add("aponcedeleon51@hotmail.com");
            mail.CC.Add("andres.ponce.de.leon.huerta@gmail.com");
            mail.Subject = "Ingresar 200 registros más";
            mail.IsBodyHtml = true;
            string htmlbody = "Urge ingresar otros 200 registros.";
            mail.Body = htmlbody;
            SmtpServer.Port = 8889; //587
            SmtpServer.UseDefaultCredentials = false;
            SmtpServer.Credentials = new System.Net.NetworkCredential("aponcedeleon51@hotmail.com", "ep1tafi0");
            SmtpServer.EnableSsl = true; //false
            SmtpServer.Send(mail);
        }

        public static void Correo3(string Remitente)
        {
            SmtpClient SmtpServer = new SmtpClient("mail.ppenaw.com");
            var mail = new MailMessage();
            mail.From = new MailAddress("apl_mx@ppenaw.com");
            mail.To.Add(Remitente);
            mail.CC.Add("aponcedeleon51@hotmail.com");
            mail.CC.Add("a.ponce@aec.mx");
            mail.Subject = "Evaluación Galileo OMR";
            mail.IsBodyHtml = true;
            string htmlbody = "Pronto nos pondremos en contacto, agradecemos su evaluación. Gracias";
            mail.Body = htmlbody;
            SmtpServer.Port = 8889;
            SmtpServer.UseDefaultCredentials = true;
            SmtpServer.Credentials = new System.Net.NetworkCredential("apl_mx@ppenaw.com", "Pru3b4_");
            SmtpServer.EnableSsl = false;
            SmtpServer.Send(mail);
        }
    
    }



}