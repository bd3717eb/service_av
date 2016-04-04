using System;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;

namespace AVServices
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IServicio" in both code and config file together.
    [ServiceContract]
    public interface IServicio
    {
        [OperationContract]
        void DoWork();

        //string UsuarioCrea(int sUsuarioSerializado, string USUARIONAME, string APELLIDOMATERNO, string EMAILALTERNO, string CP, string CALLE, string NUMEROINTERIOR, string NUMEROEXTERIOR, string D_MNPIO, string IMGUSER, string D_ESTADO, string D_CIUDAD, string D_ASENTA, string D_TIPO_ASENTA, string REFERENCIADOMICILIO, string TELCASA, string TELMOVIL, string TELOTRO, DateTime ANIONACIMIENTO, string D1, string D2, string D3);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
        string UsuarioLogin(string sUser, string sPWD);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
        string UsuarioCrea(string USUARIONAME, string CONTRASENIA);


        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
        string UsuarioModifica(int IDUSUARIO, string USUARIONAME, string CONTRASENIA, string NOMBRE, string APELLIDOPATERNO, string APELLIDOMATERNO, string IMGUSER, DateTime FECHAALTA, string EMAIL, string EMAILALTERNO, string CP, string CALLE, string NUMEROINTERIOR, string NUMEROEXTERIOR, string D_MNPIO, string D_CIUDAD, string D_ESTADO, string D_TIPO_ASENTA, string D_ASENTA, string REFERENCIADOMICILIO, string TELCASA, string TELMOVIL, string TELOTRO, DateTime ANIONACIMIENTO, int TIPOUSUARIO_C, int ESTATUS_C, string MATRICULAESCOLAR, string OWNER, DateTime DATECREATION, DateTime DATEUPDATE, string D1, string D2, string D3);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
        string UsuarioBajaLogicaFisica(int IDUSUARIO, int iOpcion);

        [OperationContract]
        [WebInvoke(Method = "POST", BodyStyle = WebMessageBodyStyle.Wrapped, ResponseFormat = WebMessageFormat.Json)]
        string UsuarioConsultaRecuperacionContrasenia(string sEmailMatriculaUsuario);
    }
}
