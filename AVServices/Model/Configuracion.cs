
using System.Linq;
using AVServices.DB;
using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

public enum E_CATALOGOS
{
    EMAIL = 1,
    ESTATUS = 2,
    USUARIO = 4,
    SEXO = 5,
    MENSAJE = 6,
    CODIGOPOSTAL = 7,
    LOG = 8,
    CALENDARIO = 9,
    GRUPO = 100,
}
public enum RESPUESTA
{
    Default = 0,
    Incidencia = -1,
    Primero = 1,
    OK = 1,
}
public enum EMAIL_GENERICOS
{
    TABLEID = 1,
    CONFIRMACION = 0,
    PERDIENTE = 1,
    APROBADA = 2,
    RECHAZADA = 3,
    REENVIA = 4,
    RECUPERACION_ACCESO = 5
}
public enum E_EMAIL_Configuracion
{
    TABLEID = 10,
    ENVIANDO = 0,
    ENVIADO = 1,
}

/// <summary>
/// CATALOGO 
/// IDTABLE = 2
/// Nombre corto = id
/// </summary>
public enum e_ESTATUS
{
    PENDIENTE = 1,
    RECHAZADA = 2,
    APROBADA = 3,
    CANCELADA = 4,
    ALTA = 5,
    BAJA = 6,
    NUEVA = 7,
    VERIFICACION = 8,
    CONTRASENIASET = 9,
}
public enum e_USUARIO
{
    TABLEID = 4,
    MAESTRO = 1,
    ALUMNO = 2,
    ADMON = 3,
    DIRECTOR = 4,
    SOPORTE = 5,
    // APP = 6 se creo desde la aplicación.
    APP = 6,
}
public enum Flujo
{
    basico = 0,
    alterno = 1,
}

public enum EnumCalendario
{
    TABLAID = 9,
    SUSPENSIONDELABORESDOCENTE = 1,
    INICIO_DE_CURSOS = 2,
    CREA_EVENTO = 8,

}
namespace AVServices.Model
{
    public class Configuracion
    {
        /// <summary>
        /// Consulta el consecutivo del campo "CATALOGODETALLEIDCLAVE" registro siguiente de esa tabla "CATALOGO"
        /// </summary>
        /// <param name="ID">ID de catalogo</param>
        /// <returns>El registro siguiente</returns>
        public static int ConsultaConsecutivoCatalogodeCatalogos(int ID)
        {
            try
            {
                using (DBEntities context = new DBEntities())
                {
                    var max_Query = (from xcd in context.CATALOGODETALLE where xcd.CATALOGOID == ID select xcd.CATALOGODETALLEIDCLAVE).Max();
                    return (int)max_Query + 1;
                }
            }
            catch (System.Exception)
            {
                return 0;
            }
        }
        /// <summary>
        /// Respuestas
        /// </summary>
        /// <param name="opcion">-1 INCIDENCIA,0 NO , 1 OK ,</param>
        /// <returns></returns>
        public static string Respuesta(int opcion)
        {
            switch (opcion)
            {
                case -1:
                    return "INCIDENCIA";
                case 0:
                    return "NO";
                case 1:
                    return "OK";
                default:
                    return "NOFOUND";
            }
        }


        public string Encrypt(string input)
        {
            return Convert.ToBase64String(Encrypt(Encoding.UTF8.GetBytes(input)));
        }
        private byte[] Encrypt(byte[] input)
        {
            //PasswordDeriveBytes pdb = new PasswordDeriveBytes("hjiweykaksd", new byte[] { 0x43, 0x87, 0x23, 0x72, 0x45, 0x56, 0x68, 0x14, 0x62, 0x84 });
            // hjiweykaksd
            PasswordDeriveBytes pdb = new PasswordDeriveBytes("aeioubcdfgh", new byte[] { 0x43, 0x87, 0x23, 0x72, 0x45, 0x56, 0x68, 0x14, 0x62, 0x84 });
            MemoryStream ms = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = pdb.GetBytes(aes.KeySize / 8);
            aes.IV = pdb.GetBytes(aes.BlockSize / 8);
            CryptoStream cs = new CryptoStream(ms, aes.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(input, 0, input.Length);
            cs.Close();
            return ms.ToArray();
        }
        public string Decrypt(string input)
        {
            return Encoding.UTF8.GetString(Decrypt(Convert.FromBase64String(input)));
        }
        private byte[] Decrypt(byte[] input)
        {
            PasswordDeriveBytes pdb = new PasswordDeriveBytes("aeioubcdfgh", new byte[] { 0x43, 0x87, 0x23, 0x72, 0x45, 0x56, 0x68, 0x14, 0x62, 0x84 });
            MemoryStream ms = new MemoryStream();
            Aes aes = new AesManaged();
            aes.Key = pdb.GetBytes(aes.KeySize / 8);
            aes.IV = pdb.GetBytes(aes.BlockSize / 8);
            CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(input, 0, input.Length);
            cs.Close();
            return ms.ToArray();
        }

    }
}