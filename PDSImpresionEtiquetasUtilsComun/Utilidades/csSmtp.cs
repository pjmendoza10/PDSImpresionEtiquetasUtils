using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace PDSImpresionEtiquetasUtils.Comun.Utilidades
{
    public class csSmtp
    {
        public static bool SendMail(csConfiguracion p_configuracion, List<string> ToList, string subject, string body)
        {
            try
            {

                // Para debug:
                //if (!string.IsNullOrEmpty(Program.State.Debug_EmailUnico))
                //{
                //    ToList.Clear();
                //    ToList.Add(Program.State.Debug_EmailUnico);
                //}


                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(p_configuracion.Datos.smtpServer);

                foreach (var item in ToList)
                {
                    try
                    {
                        mail.From = new MailAddress(p_configuracion.Datos.smtpFromAddress);
                        mail.To.Clear();
                        mail.To.Add(item);

                        mail.Subject = subject;
                        mail.Body = body;


                        SmtpServer.Port = p_configuracion.Datos.smtpPort;
                        SmtpServer.UseDefaultCredentials = false;
                        SmtpServer.Credentials = new System.Net.NetworkCredential(p_configuracion.Datos.smtpUser, p_configuracion.Datos.smtpPassword);
                        SmtpServer.EnableSsl = p_configuracion.Datos.smtpSSL;
                        SmtpServer.Send(mail);
                    }
                    catch (Exception ex)
                    {
                        //Program.logger.Error(string.Format("Objeto : {0} {1} Mesaje : {2} {3} Fuente : {4}",
                        //    ex.Source.ToString(),
                        //    Environment.NewLine,
                        //    ex.Message.ToString(),
                        //    Environment.NewLine,
                        //    ex.StackTrace.ToString()));

                        Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);
                        //errors.Add(new ErrorDetail(ex.Source, ex.Message));
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                //Program.logger.Error(string.Format("Objeto : {0} {1} Mesaje : {2} {3} Fuente : {4}",
                //    ex.Source.ToString(),
                //    Environment.NewLine,
                //    ex.Message.ToString(),
                //    Environment.NewLine,
                //    ex.StackTrace.ToString()));
                Comun.Utilidades.csLogUtils.EscribeLineaLogError(ex);

                //errors.Add(new ErrorDetail(ex.Source, ex.Message));
                return false;
            }
        }
    }
}
