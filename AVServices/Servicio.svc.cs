using AVServices.Controller;
using AVServices.DB;
using AVServices.Model;
using System;
using System.ComponentModel.DataAnnotations;
namespace AVServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Servicio" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Servicio.svc or Servicio.svc.cs at the Solution Explorer and start debugging.
    public class Servicio : IServicio
    {
        public void DoWork()
        {
            string es = new Configuracion().Encrypt("holamundo");
            string ese = new Configuracion().Decrypt(es);
            new DomicilioController().ConsultaDetallesxCP("57900");
        }
        #region Usuario
        public string UsuarioLogin(string sUser, string sPWD)
        {
            return UsuarioController.Login(sUser, sPWD);
        }

        public string UsuarioCrea(string EMAIL, string CONTRASENIA)
        {
            USUARIO uo = new USUARIO();
            uo.CONTRASENIA = CONTRASENIA;
            uo.EMAIL = EMAIL;
            uo.IMGUSER = "DefaultUser.PNG";
            return UsuarioController.Crea(uo);
        }
        public string UsuarioModifica(int IDUSUARIO, string USUARIONAME, string CONTRASENIA, string NOMBRE, string APELLIDOPATERNO, string APELLIDOMATERNO, string IMGUSER, DateTime FECHAALTA, string EMAIL, string EMAILALTERNO, string CP, string CALLE, string NUMEROINTERIOR, string NUMEROEXTERIOR, string D_MNPIO, string D_CIUDAD, string D_ESTADO, string D_TIPO_ASENTA, string D_ASENTA, string REFERENCIADOMICILIO, string TELCASA, string TELMOVIL, string TELOTRO, DateTime ANIONACIMIENTO, int TIPOUSUARIO_C, int ESTATUS_C, string MATRICULAESCOLAR, string OWNER, DateTime DATECREATION, DateTime DATEUPDATE, string D1, string D2, string D3)
        {
            return UsuarioController.Modifica(IDUSUARIO, USUARIONAME, CONTRASENIA, NOMBRE, APELLIDOPATERNO, APELLIDOMATERNO, IMGUSER, FECHAALTA, EMAIL, EMAILALTERNO, CP, CALLE, NUMEROINTERIOR, NUMEROEXTERIOR, D_MNPIO, D_CIUDAD, D_ESTADO, D_TIPO_ASENTA, D_ASENTA, REFERENCIADOMICILIO, TELCASA, TELMOVIL, TELOTRO, ANIONACIMIENTO, TIPOUSUARIO_C, ESTATUS_C, MATRICULAESCOLAR, OWNER, DATECREATION, DATEUPDATE, D1, D2, D3);
        }
        public string UsuarioBajaLogicaFisica(int IDUSUARIO, int iOpcion)
        {
            return UsuarioController.BajaLogicaFisica(IDUSUARIO, iOpcion);
        }

        public string UsuarioConsultaRecuperacionContrasenia(string sEmailMatriculaUsuario)
        {
            return UsuarioController.RecuperaAcceso(sEmailMatriculaUsuario);
        }

        #endregion
    }
}
