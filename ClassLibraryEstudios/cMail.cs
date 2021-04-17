using System;
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.IO;
using System.Configuration;
using System.Net;

namespace ClassLibraryEstudios
{
    public class cMail
    {
        public static string ubi_Estudios = ConfigurationManager.AppSettings["Estudios"];
        public static string ubi_Imagenes = ConfigurationManager.AppSettings["Imagenes"];
        public static string ubi_Envios = ConfigurationManager.AppSettings["Envios"];
        public static string ubi_Template = ConfigurationManager.AppSettings["Template"];

        public static string mail_From = ConfigurationManager.AppSettings["From"];
        public static string mail_Reply = ConfigurationManager.AppSettings["Reply"];
        public static string mail_BCC = ConfigurationManager.AppSettings["BCC"];
        public static string mail_Pass = ConfigurationManager.AppSettings["Pass"];
        public static string mail_Subject = ConfigurationManager.AppSettings["Subject"];

        public static string mail_Dominio = ConfigurationManager.AppSettings["Dominio"];
        public static string mail_Puerto = ConfigurationManager.AppSettings["Puerto"];
        public static string mail_TimeOut = ConfigurationManager.AppSettings["TimeOut"];
        public static string mail_EnableSSL = ConfigurationManager.AppSettings["EnableSSL"];

        public static string es_Prueba = ConfigurationManager.AppSettings["Prueba"];
        public static string mail_Prueba = ConfigurationManager.AppSettings["PFrom"];

        public static bool comprobarFormatoEmail(string sEmailAComprobar)
        {
            String sFormato;
            sFormato = "\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*";
            if (Regex.IsMatch(sEmailAComprobar, sFormato))
            {
                if (Regex.Replace(sEmailAComprobar, sFormato, String.Empty).Length == 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool enviarMail(string puerta, string orden, string mail, 
            string paciente, string fecha, string profesional)
        {
            try
            {
                string[] imgAdjuntas;

                MailMessage correo = new MailMessage();
                SmtpClient envio = new SmtpClient();

                correo.Body = HtmlBody(paciente, fecha, profesional);
                correo.IsBodyHtml = true;
                correo.Subject = mail_Subject;

                if(bool.Parse(es_Prueba))
                   correo.To.Add(mail_Prueba);
                else
                   correo.To.Add(mail);

                //Agrego copia CC
                if (mail_BCC != "")
                    correo.Bcc.Add(mail_BCC);

                //Llamo al metodo para listar las imagenes
                imgAdjuntas = listarArchivos(puerta, orden);

                if (imgAdjuntas != null)
                {
                    //agregado de archivo
                    foreach (string archivo in imgAdjuntas)
                    {
                        //comprobamos si existe el archivo y lo agregamos a los adjuntos
                        if (System.IO.File.Exists(@archivo))
                            correo.Attachments.Add(new Attachment(@archivo));
                    }
                }

                correo.Attachments.Add(new Attachment(ubi_Estudios + "\\" + puerta + "_" + orden + ".pdf"));

                correo.From = new MailAddress(mail_From);
                correo.ReplyToList.Add(mail_Reply);

                envio.Credentials = new NetworkCredential(mail_From, mail_Pass);
                envio.Host = mail_Dominio;
                envio.Port = int.Parse(mail_Puerto);
                envio.Timeout = int.Parse(mail_TimeOut);
                envio.EnableSsl = bool.Parse(mail_EnableSSL);

                envio.Send(correo);

                correo.To.Clear();

                cLog.log.Debug("cMail (enviarMail): " +String.Format("Estudio enviado: Paciente {0}, Mail: {1}, Estudio: {2} ", paciente, mail, puerta + orden));

                return true;
            }
            catch (Exception ex)
            {
                cLog.log.Error("cMail (enviarMail): " + ex.ToString());
                return false;
                //throw ex;
            }
            
        }

        public static string HtmlBody(string paciente, string fecha, string profesional)
        {
            try
            {
                string body = String.Empty;

                StreamReader html = new StreamReader(ubi_Template);

                body = html.ReadToEnd();
                body = body.Replace("{Paciente}", paciente);
                body = body.Replace("{Fecha}", fecha);
                body = body.Replace("{Profesional}", profesional);
                html.Close();
                return body;
            }
            catch (Exception ex)
            {
                cLog.log.Error("cMail (HtmlBody): " + ex.ToString());
                throw;
            }
           
        }

        public static string[] listarArchivos(string puerta, string orden)
        {
            try
            {
                string[] archivos;

                archivos = Directory.GetFiles(ubi_Imagenes + "\\" + puerta + orden + "\\");

                return archivos;

            }
            catch (Exception ex)
            {
                cLog.log.Error("cMail (listarArchivos): " + ex.ToString());
                return null;
            }
           
        }
    }
}