using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Mail;

namespace Galileo
{
    public class Email
    {

        public static void Correo(string Remitente,int ID, string body, int Tipo, string origen, string contraseña)
        {
            if (Tipo == 1)
            {
            SmtpClient SmtpServer = new SmtpClient("mail.ppenaw.com");
            var mail = new MailMessage();
            mail.From = new MailAddress(origen);
            mail.To.Add(Remitente);
            mail.CC.Add("aponcedeleon51@hotmail.com");
            mail.CC.Add("a.ponce@aec.mx");
            mail.Subject = "Registro Galileo OMR encuestado: " + ID.ToString();
            mail.IsBodyHtml = true;
            string htmlbody = body;
            mail.Body = htmlbody;
            SmtpServer.Port = 8889;
            SmtpServer.UseDefaultCredentials = true;
            SmtpServer.Credentials = new System.Net.NetworkCredential(origen, contraseña);
            SmtpServer.EnableSsl = false;
            SmtpServer.Send(mail);
            }
            if (Tipo == 2)
            {
                SmtpClient SmtpServer = new SmtpClient("mail.ppenaw.com");
                var mail = new MailMessage();
                mail.From = new MailAddress(origen);
                mail.To.Add(Remitente);
                mail.CC.Add("aponcedeleon51@hotmail.com");
                mail.CC.Add("a.ponce@aec.mx");
                mail.Subject = "Evaluación Galileo OMR";
                mail.IsBodyHtml = true;
                string htmlbody = body;
                mail.Body = htmlbody;
                SmtpServer.Port = 8889;
                SmtpServer.UseDefaultCredentials = true;
                SmtpServer.Credentials = new System.Net.NetworkCredential(origen, contraseña);
                SmtpServer.EnableSsl = false;
                SmtpServer.Send(mail);
            }
            if (Tipo == 3)
            {
                SmtpClient SmtpServer = new SmtpClient("smtp.live.com");
                var mail = new MailMessage();
                mail.From = new MailAddress(origen);
                mail.To.Add(Remitente);
                mail.CC.Add(origen);
                //mail.CC.Add("andres.ponce.de.leon.huerta@gmail.com");
                mail.Subject = "Ingresar 200 registros más";
                mail.IsBodyHtml = true;
                string htmlbody = body;
                mail.Body = htmlbody;
                SmtpServer.Port = 8889; //587
                SmtpServer.UseDefaultCredentials = false;
                SmtpServer.Credentials = new System.Net.NetworkCredential(origen, contraseña);
                SmtpServer.EnableSsl = true; //false
                SmtpServer.Send(mail);

            }
        }


        public static void Correo2(string Remitente)
        {
        }

       
    
    }



}