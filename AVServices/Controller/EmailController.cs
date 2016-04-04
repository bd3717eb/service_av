using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using AVServices.Model;
using AVServices.DB;

namespace AVServices.Controller
{
    public class EmailController
    {
        // CREA
        // CONSULTA ID 
        // MODIFICA
        // BAJA FISICA
        // BAJA LOGICA
        public static int CreaRegistro(string sAsunto, string sMensaje, string sEmisor, string sReceptor, string sCreador, int ID, int iOpcion, DBEntities context)
        {
            try
            {
                if (iOpcion == 0)
                {
                    CATALOGODETALLE ce = new CATALOGODETALLE();
                    ce.CATALOGOID = (int)EMAIL_GENERICOS.TABLEID;
                    ce.CATALOGODETALLEIDCLAVE = Configuracion.ConsultaConsecutivoCatalogodeCatalogos((int)EMAIL_GENERICOS.TABLEID);
                    ce.CATALOGODETALLECLAVE = E_EMAIL_Configuracion.ENVIANDO.ToString();
                    ce.CATALOGODETALLEDESCRIPCION = sMensaje;
                    ce.CREADOR = sEmisor;
                    ce.CATALOGODETALLEACTIVO = false;
                    ce.DATECREATION = DateTime.Now;
                    ce.DATEUPDATE = DateTime.Now;
                    ce.D0 = sReceptor;
                    ce.D1 = sAsunto;
                    context.CATALOGODETALLE.Add(ce);
                    context.SaveChanges();
                    return ce.CATALOGODETALLEIDCLAVE;
                }
                else if (iOpcion == 1)
                {

                    CATALOGODETALLE emailgenerico = context.CATALOGODETALLE.AsQueryable().FirstOrDefault(xe => xe.CATALOGODETALLEIDCLAVE == ID && xe.CATALOGOID == (int)EMAIL_GENERICOS.TABLEID);
                    emailgenerico.CATALOGODETALLEACTIVO = true;
                    emailgenerico.DATEUPDATE = DateTime.Now;
                    context.SaveChanges();
                    return emailgenerico.CATALOGODETALLEIDCLAVE;
                }
                return 0;
            }
            catch (Exception ex)
            {
                BitacoraController.CreaRegistroError((int)EMAIL_GENERICOS.TABLEID, MethodBase.GetCurrentMethod().Name, ex.Message, MethodBase.GetCurrentMethod().DeclaringType.Name);
                //Mensaje om = new Mensaje();
                //om.Estatus = -1;
                //om.Descripcion = "Error al consultar eventos ";
                //om.Resultado = string.Concat(ex.Message, ", ", ex.InnerException);
                return -1;
            }
        }

        public static string ConsultaEmailGenericos(int IDEmail, int IDEmisor, USUARIO usuario_receptor)
        {
            Mensaje om = new Mensaje();

            using (DBEntities context = new DBEntities())
            {
                // TODO 
                // -- 0 Cambiar el estatus del usuario ya que se pidio el cambio de contraseña
                // -- 1 buscar estructura de email generico 
                // 1.1 Asunto 
                // 1.2 Cuerpo de Mensaje
                // 1.3 Sustituir la informacion de Nombre , Apellido Paterno y Apellido Materno si es que esta capturada 
                // 1.4 crear un link de recuperación.
                // 1.5 cambiar contraseña.

                CATALOGODETALLE emailestructura = context.CATALOGODETALLE.AsQueryable().FirstOrDefault(xe => xe.CATALOGODETALLEIDCLAVE == IDEmail && xe.CATALOGODETALLEACTIVO == true && xe.CATALOGOID == (int)EMAIL_GENERICOS.TABLEID);
                // Asunto emailestructura.D0   
                emailestructura.D0 = emailestructura.D0.Replace("@AulaVirtual", "");
                // Cuerpo de Mensaje emailestructura.CATALOGODETALLEDESCRIPCION 
                emailestructura.CATALOGODETALLEDESCRIPCION = emailestructura.CATALOGODETALLEDESCRIPCION.Replace("@usuariodetalles", string.Concat(usuario_receptor.NOMBRE, " ", usuario_receptor.APELLIDOPATERNO, " ", usuario_receptor.APELLIDOMATERNO));

                //@ligarestablecimientopwd 


                CATALOGODETALLE emailemisor = context.CATALOGODETALLE.AsQueryable().FirstOrDefault(xe => xe.CATALOGODETALLEIDCLAVE == IDEmisor && xe.CATALOGODETALLEACTIVO == true && xe.CATALOGOID == (int)E_EMAIL_Configuracion.TABLEID);
                EmailDM oe = new EmailDM();
                oe.sAsunto = emailestructura.D0;
                oe.sMensaje = emailestructura.CATALOGODETALLEDESCRIPCION;
                oe.sEmisor = emailemisor.CATALOGODETALLECLAVE;
                oe.sReceptor = "usuario_receptor";
                oe.sImagenes = string.Empty;
                oe.bEstatus = true;
                oe.dFechaCreacion = DateTime.Now;
                oe.dFechaModificacion = DateTime.Now;
                oe.sContrasenia = emailemisor.CATALOGODETALLEDESCRIPCION.Split('|')[0];
                oe.sPuertoSMTP = emailemisor.CATALOGODETALLEDESCRIPCION.Split('|')[1];
                oe.iPuerto = int.Parse(emailemisor.CATALOGODETALLEDESCRIPCION.Split('|')[2]);
                int iTemp = BitacoraController.CreaRegistroLOG(E_EMAIL_Configuracion.ENVIANDO.ToString(), string.Concat(emailestructura.CATALOGOID, "|", emailestructura.CATALOGODETALLEIDCLAVE, "|CATALOGODETALLE"), string.Concat(emailemisor.CATALOGOID, "|", emailestructura.CATALOGODETALLEIDCLAVE, "|CATALOGODETALLE"), false);
                EnviaEmailGenerico(oe.listaContactos, oe.listaAdjuntos, oe.sReceptor, oe.sAsunto, oe.sMensaje, oe.sEmisor, oe.sContrasenia, oe.sPuertoSMTP, oe.iPuerto);
                BitacoraController.ModificaRegistroLOG(iTemp, E_EMAIL_Configuracion.ENVIADO.ToString(), string.Concat(emailestructura.CATALOGOID, "|", emailestructura.CATALOGODETALLEIDCLAVE, "|CATALOGODETALLE"), string.Concat(emailemisor.CATALOGOID, "|", emailestructura.CATALOGODETALLEIDCLAVE, "|CATALOGODETALLE"), true);
            }
            return "Mensaje Enviado";

            // balbuceando atrozidades
            // 
        }

        private static bool EnviaEmailGenerico(List<string> listaReceptoresContactosEmail, List<string> listaAdjuntos, string sReceptorEmail, string sAsunto, string sCuerpoEmail, string sEmisorEmail, string sPWD, string sSMTP, int iPuerto)
        {
            MailMessage mail = new MailMessage(sEmisorEmail, sReceptorEmail);
            NetworkCredential EmailloginInfo = new NetworkCredential(sEmisorEmail, sPWD);

            SmtpClient client = new SmtpClient();

            client.Port = iPuerto;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = sSMTP;
            mail.Subject = sAsunto;
            mail.IsBodyHtml = true;
            mail.Body = sCuerpoEmail;
            client.EnableSsl = true;
            client.UseDefaultCredentials = false;
            client.Credentials = EmailloginInfo;
            ////EmailsmtpClient.Credentials = EmailloginInfo;

            client.Send(mail);

            //Servidor
            //var EmailloginInfo = new NetworkCredential(sEmisorEmail, sPWD);
            //var Emailmsg = new System.Net.Mail.MailMessage();
            //var EmailsmtpClient = new SmtpClient(sSMTP, iPuerto);
            //Emailmsg.To = new MailAddress( sReceptorEmail);

            //if (listaReceptoresContactosEmail != null)
            //{
            //    foreach (string contacto in listaReceptoresContactosEmail)
            //        Emailmsg.To.Add(new MailAddress(contacto));
            //}
            //if (listaAdjuntos != null)
            //{
            //    foreach (string adjunto in listaAdjuntos)
            //    {
            //        Attachment aadjunto = new Attachment(@adjunto);
            //        Emailmsg.Attachments.Add(@aadjunto);
            //    }
            //}
            //Emailmsg.Subject = sAsunto;
            //Emailmsg.IsBodyHtml = true;
            //Emailmsg.Body = sCuerpoEmail;
            //EmailsmtpClient.EnableSsl = false;
            //EmailsmtpClient.UseDefaultCredentials = false;
            //EmailsmtpClient.Credentials = EmailloginInfo;
            //try
            //{
            //    EmailsmtpClient.Send(Emailmsg);

            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
            return true;
        }
    }
}