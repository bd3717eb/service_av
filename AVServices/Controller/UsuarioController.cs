using Newtonsoft.Json;
using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AVServices.Model;
using AVServices.DB;
using System.Globalization;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace AVServices.Controller
{
    public class UsuarioController
    {
        /// <summary>
        /// Login 
        /// </summary>
        /// <param name="sUsuario">Nombre del usuario , Email o Matricula</param>
        /// <param name="sPWD">Contraseña</param>
        /// <returns>OK:Información de usuario basica del usuario |NO: Verificar usuario o contraseña</returns>
        public static string Login(string sUsuario, string sPWD)
        {
            Mensaje om = new Mensaje();
            DBEntities context = new DBEntities();
            string sResTemp = string.Empty;
            // Login por email
            string[] sEmailoUser = sUsuario.Split('@');
            if (sEmailoUser.Length > 1)
            {
                var ob = context.USUARIO.AsQueryable().FirstOrDefault((au => au.EMAIL.ToUpper() == sUsuario.ToUpper() && au.CONTRASENIA == sPWD));
                if (ob != null)
                    sResTemp = JsonConvert.SerializeObject(ob, Formatting.None);
            }

            // Login por matricula
            int iMatricula;
            bool res = int.TryParse(sUsuario, out iMatricula);
            if (res == true && string.IsNullOrEmpty(sResTemp))
            {
                USUARIO obM = context.USUARIO.AsQueryable().FirstOrDefault(au => au.MATRICULAESCOLAR == iMatricula.ToString() && au.CONTRASENIA == sPWD);
                if (obM != null)
                    sResTemp = JsonConvert.SerializeObject(obM, Formatting.None);
            }

            // login por usuario
            USUARIO obU = context.USUARIO.AsQueryable().FirstOrDefault(au => au.USUARIONAME.ToUpper() == sUsuario.ToUpper() && au.CONTRASENIA == sPWD);
            if (obU != null && string.IsNullOrEmpty(sResTemp))
                sResTemp = JsonConvert.SerializeObject(obU, Formatting.None);

            if (string.IsNullOrEmpty(sResTemp))
            {
                om.Estatus = 0;
                om.RespuestaCorta = "NO";
                om.Detalles = "Verificar usuario o contraseña";
            }
            else
            {
                if (obU.MATRICULAESCOLAR.Length < 2)
                {
                    int estatus_matricula_temp = int.Parse(obU.MATRICULAESCOLAR);
                    CATALOGODETALLE cdmatriculatemp = context.CATALOGODETALLE.AsQueryable().FirstOrDefault(amd => amd.CATALOGOID == (int)E_CATALOGOS.ESTATUS && amd.CATALOGODETALLEIDCLAVE == estatus_matricula_temp);
                    obU.MATRICULAESCOLAR = string.Concat(cdmatriculatemp.CATALOGODETALLEIDCLAVE, "|", cdmatriculatemp.CATALOGODETALLEDESCRIPCION);
                    sResTemp = JsonConvert.SerializeObject(obU, Formatting.None);
                }

                om.Estatus = 1;
                om.RespuestaCorta = "OK";
                om.Detalles = sResTemp;


            }

            // This works :JsonConvert.SerializeObject(myObject, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore}); 
            return JsonConvert.SerializeObject(om, Formatting.None);
            //return JsonConvert.SerializeObject(om, Newtonsoft.Json.Formatting.None, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        /// <summary>
        /// Crea un nuevo Usuario
        /// </summary>
        /// <param name="pUser">Objeto lleno con la información del usuario</param>
        /// <returns>Bandera si se realizo el cambio o no</returns>
        public static string Crea(USUARIO uo)
        {
            Mensaje om = new Mensaje();
            try
            {
                if (!new EmailAddressAttribute().IsValid(uo.EMAIL) && string.IsNullOrEmpty(uo.EMAIL) || string.IsNullOrEmpty(uo.CONTRASENIA))
                {
                    om.Estatus = -1;
                    om.RespuestaCorta = "ERROR";
                    om.Detalles = "Verificar información ingresada.";
                    return JsonConvert.SerializeObject(om, Formatting.None);
                }
                using (DBEntities context = new DBEntities())
                {
                    IQueryable<USUARIO> ob = context.USUARIO.Where(au => au.EMAIL.ToUpper() == uo.EMAIL);
                    if (ob.Count() > 0)
                    {
                        om.Estatus = -1;
                        om.RespuestaCorta = "ERROR";
                        om.Detalles = "El usuario ya existe";
                        return JsonConvert.SerializeObject(om, Formatting.None);
                    }

                    Task tarea = new Task(() => EmailController.ConsultaEmailGenericos((int)EMAIL_GENERICOS.CONFIRMACION, 0, uo));
                    tarea.Start();
                    // -------------------------------------------------------------------------------------------------------------------
                    //  Se deben inicializar los valores del objeto porque Newtonsoft.Json genera una excepcion al momento de deserializar valores null
                    uo.USUARIONAME = string.Empty;
                    uo.NOMBRE = string.Empty;
                    uo.APELLIDOPATERNO = string.Empty;
                    uo.APELLIDOMATERNO = string.Empty;

                    uo.EMAILALTERNO = string.Empty;
                    uo.CP = string.Empty;
                    uo.CALLE = string.Empty;
                    uo.NUMEROINTERIOR = string.Empty;
                    uo.NUMEROEXTERIOR = string.Empty;
                    uo.D_MNPIO = string.Empty;
                    //uo.IMGUSER = string.Empty;
                    uo.D_ESTADO = string.Empty;
                    uo.D_CIUDAD = string.Empty;
                    uo.D_ASENTA = string.Empty;
                    uo.D_TIPO_ASENTA = string.Empty;
                    uo.REFERENCIADOMICILIO = string.Empty;
                    uo.TELCASA = string.Empty;
                    uo.TELMOVIL = string.Empty;
                    uo.TELOTRO = string.Empty;
                    uo.ANIONACIMIENTO = DateTime.Parse("01/01/1900", new CultureInfo("es-MX"));
                    uo.D1 = string.Empty;
                    uo.D2 = string.Empty;
                    uo.D3 = string.Empty;
                    // ------------------------------------------
                    int itemp = (int)(e_ESTATUS.PENDIENTE);
                    uo.MATRICULAESCOLAR = itemp.ToString();
                    uo.FECHAALTA = DateTime.Now;
                    itemp = (int)e_USUARIO.APP;
                    uo.OWNER = itemp.ToString();
                    uo.ESTATUS_C = (int)e_ESTATUS.NUEVA;
                    uo.DATECREATION = DateTime.Now;
                    uo.DATEUPDATE = DateTime.Now;
                    // Por default todos los usuarios son alumnos hasta que se diga o modifique lo contrario u otro tipo 
                    uo.TIPOUSUARIO_C = (int)e_USUARIO.ALUMNO;
                    context.USUARIO.Add(uo);
                    context.SaveChanges();

                    om = new Mensaje();
                    om.Estatus = 1;
                    om.RespuestaCorta = "OK";
                    om.Detalles = uo.IDUSUARIO.ToString();
                    tarea.Wait();
                    return JsonConvert.SerializeObject(om, Formatting.None);
                }
            }
            catch (Exception ex)
            {
                BitacoraController.CreaRegistroError((int)E_CATALOGOS.USUARIO, MethodBase.GetCurrentMethod().Name, ex.Message, MethodBase.GetCurrentMethod().DeclaringType.Name);
                om.Estatus = -1;
                om.RespuestaCorta = "Error ";
                om.Detalles = string.Concat(ex.Message, ", ", ex.InnerException);
                return JsonConvert.SerializeObject(om, Formatting.None);
            }
        }

        /// <summary>
        /// Modifica Usuario existente
        /// </summary>
        /// <param name="pUser">Objeto de Usuario con cambios </param>
        /// <returns>Bandera si se realizo el cambio o no</returns>
        public static string Modifica(int IDUSUARIO, string USUARIONAME, string CONTRASENIA, string NOMBRE, string APELLIDOPATERNO, string APELLIDOMATERNO, string IMGUSER, DateTime FECHAALTA, string EMAIL, string EMAILALTERNO, string CP, string CALLE, string NUMEROINTERIOR, string NUMEROEXTERIOR, string D_MNPIO, string D_CIUDAD, string D_ESTADO, string D_TIPO_ASENTA, string D_ASENTA, string REFERENCIADOMICILIO, string TELCASA, string TELMOVIL, string TELOTRO, DateTime ANIONACIMIENTO, int TIPOUSUARIO_C, int ESTATUS_C, string MATRICULAESCOLAR, string OWNER, DateTime DATECREATION, DateTime DATEUPDATE, string D1, string D2, string D3)
        {
            bool bandera = false;
            try
            {
                using (DBEntities context = new DBEntities())
                {
                    Mensaje om = new Mensaje();
                    USUARIO ob = context.USUARIO.Where(au => au.IDUSUARIO == IDUSUARIO).FirstOrDefault<USUARIO>();
                    if (ob == null)
                    {
                        om.Estatus = 0;
                        om.RespuestaCorta = "NO";
                        om.Detalles = "No se encontro información de usuario";
                        return JsonConvert.SerializeObject(om, Formatting.Indented);
                    }
                    switch (ob.ESTATUS_C)
                    {
                        case (int)e_ESTATUS.NUEVA:
                            var obUresultado = context.USUARIO.Where(au => au.USUARIONAME.ToUpper() == USUARIONAME.ToUpper());
                            // Verifica que el usuario no exista actualmente
                            if (obUresultado.Count() > 0 || string.IsNullOrEmpty(USUARIONAME) || string.IsNullOrEmpty(CONTRASENIA) || string.IsNullOrEmpty(NOMBRE) || string.IsNullOrEmpty(APELLIDOPATERNO) || string.IsNullOrEmpty(EMAIL) || ANIONACIMIENTO == DateTime.MinValue)
                            {
                                om.Detalles = " Verificar información capturada";
                                bandera = false;
                                break;
                            }
                            bandera = true;
                            ob.USUARIONAME = USUARIONAME;
                            ob.NOMBRE = NOMBRE;
                            ob.APELLIDOPATERNO = APELLIDOPATERNO;
                            ob.APELLIDOMATERNO = APELLIDOMATERNO;
                            ob.CONTRASENIA = (ob.CONTRASENIA == CONTRASENIA) ? ob.CONTRASENIA : CONTRASENIA;
                            ob.IMGUSER = (ob.IMGUSER == IMGUSER) ? ob.IMGUSER : IMGUSER;
                            ob.EMAIL = (new EmailAddressAttribute().IsValid(EMAIL)) ? EMAIL : ob.EMAIL;
                            ob.EMAILALTERNO = (new EmailAddressAttribute().IsValid(EMAILALTERNO)) ? EMAILALTERNO : string.Empty;
                            ob.CP = CP;
                            ob.CALLE = CALLE;
                            ob.NUMEROINTERIOR = NUMEROINTERIOR;
                            ob.NUMEROEXTERIOR = NUMEROEXTERIOR;
                            ob.D_MNPIO = D_MNPIO;
                            ob.D_CIUDAD = D_CIUDAD;
                            ob.D_ESTADO = D_ESTADO;
                            ob.D_TIPO_ASENTA = D_TIPO_ASENTA;
                            ob.D_ASENTA = D_ASENTA;
                            ob.REFERENCIADOMICILIO = REFERENCIADOMICILIO;
                            ob.TELCASA = TELCASA;
                            ob.TELMOVIL = TELMOVIL;
                            ob.TELOTRO = TELOTRO;
                            ob.ANIONACIMIENTO = ANIONACIMIENTO;
                            ob.ESTATUS_C = (int)e_ESTATUS.PENDIENTE;
                            ob.MATRICULAESCOLAR = ob.TIPOUSUARIO_C.ToString();
                            ob.D1 = string.IsNullOrEmpty(D1) ? ob.D1 : D1;
                            ob.D2 = string.IsNullOrEmpty(D2) ? ob.D2 : D2;
                            ob.D3 = string.IsNullOrEmpty(D3) ? ob.D3 : D3;
                            ob.OWNER = OWNER;
                            ob.DATEUPDATE = DateTime.Now;
                            break;
                        case (int)e_ESTATUS.PENDIENTE:
                            var vuo = context.USUARIO.Where(au => au.USUARIONAME.ToUpper() == USUARIONAME.ToUpper());
                            // Verifica que el usuario no exista actualmente
                            if (vuo.Count() > 0 || string.IsNullOrEmpty(USUARIONAME) || string.IsNullOrEmpty(CONTRASENIA) || string.IsNullOrEmpty(NOMBRE) || string.IsNullOrEmpty(APELLIDOPATERNO) || string.IsNullOrEmpty(EMAIL) || (string.IsNullOrEmpty(CP) || (CP.Length != 5) || string.IsNullOrEmpty(NUMEROEXTERIOR) || ANIONACIMIENTO == DateTime.MinValue))
                            {
                                om.Detalles = " Verificar información capturada";
                                bandera = false;
                                break;
                            }
                            bandera = true;
                            ob.USUARIONAME = USUARIONAME;
                            ob.NOMBRE = NOMBRE;
                            ob.APELLIDOPATERNO = APELLIDOPATERNO;
                            ob.APELLIDOMATERNO = APELLIDOMATERNO;
                            ob.CONTRASENIA = (ob.CONTRASENIA == CONTRASENIA) ? ob.CONTRASENIA : CONTRASENIA;
                            ob.IMGUSER = (ob.IMGUSER == IMGUSER) ? ob.IMGUSER : IMGUSER;
                            ob.EMAIL = (new EmailAddressAttribute().IsValid(EMAIL)) ? EMAIL : ob.EMAIL;
                            ob.EMAILALTERNO = (new EmailAddressAttribute().IsValid(EMAILALTERNO)) ? EMAILALTERNO : string.Empty;
                            List<CodigoPostalDM> lcp = new DomicilioController().CPGetdetails(CP);
                            ob.CP = lcp[0].cp;
                            ob.CALLE = CALLE;
                            ob.NUMEROINTERIOR = NUMEROINTERIOR;
                            ob.NUMEROEXTERIOR = NUMEROEXTERIOR;
                            ob.D_MNPIO = lcp[0].D_mnpio;
                            ob.D_CIUDAD = lcp[0].d_ciudad;
                            ob.D_ESTADO = lcp[0].d_estado;
                            ob.D_TIPO_ASENTA = lcp[0].d_tipo_asenta;
                            ob.D_ASENTA = lcp[0].d_asenta_colonia;
                            ob.REFERENCIADOMICILIO = REFERENCIADOMICILIO;
                            ob.TELCASA = TELCASA;
                            ob.TELMOVIL = TELMOVIL;
                            ob.TELOTRO = TELOTRO;
                            ob.ANIONACIMIENTO = ANIONACIMIENTO;
                            ob.ESTATUS_C = (int)e_ESTATUS.VERIFICACION;
                            ob.MATRICULAESCOLAR = ob.TIPOUSUARIO_C.ToString();
                            ob.D1 = string.IsNullOrEmpty(D1) ? ob.D1 : D1;
                            ob.D2 = string.IsNullOrEmpty(D2) ? ob.D2 : D2;
                            ob.D3 = string.IsNullOrEmpty(D3) ? ob.D3 : D3;
                            ob.OWNER = OWNER;
                            ob.DATEUPDATE = DateTime.Now;
                            break;
                        case (int)e_ESTATUS.VERIFICACION:
                            var vuov = context.USUARIO.Where(au => au.IDUSUARIO == IDUSUARIO);
                            // Verifica que el usuario no exista actualmente
                            if (vuov.Count() > 0)
                            {
                                if ((int)ESTATUS_C <= (int)e_ESTATUS.RECHAZADA && (int)ESTATUS_C >= (int)e_ESTATUS.CANCELADA)
                                {
                                    ob.ESTATUS_C = ESTATUS_C;
                                    bandera = true;
                                }
                                else
                                {
                                    om.Detalles = " Verificar información capturada";
                                    bandera = false;
                                }
                                break;
                            }
                            break;
                        case (int)e_ESTATUS.BAJA:
                            var vuob = context.USUARIO.Where(au => au.IDUSUARIO == IDUSUARIO);
                            // Verifica que el usuario no exista actualmente
                            if (vuob.Count() > 0)
                            {
                                ob.ESTATUS_C = (int)e_ESTATUS.BAJA;
                                bandera = true;
                            }
                            else
                            {
                                om.Detalles = " No se puede dar de baja usuario";
                                bandera = false;
                            }
                            break;

                        default:
                            break;
                    }

                    if (bandera)
                    {
                        om.Estatus = 1;
                        om.RespuestaCorta = "OK";
                        om.Detalles = JsonConvert.SerializeObject(ob, Formatting.None);
                        context.SaveChanges();
                    }
                    else
                    {
                        om.Estatus = -1;
                        om.RespuestaCorta = "ERROR";
                        om.Detalles = string.IsNullOrEmpty(om.Detalles) ? "Operación no completada" : om.Detalles;

                        return JsonConvert.SerializeObject(om, Formatting.None);
                    }


                    return JsonConvert.SerializeObject(om, Formatting.None);
                }
            }
            catch (DbEntityValidationException e)
            {
                BitacoraController.CreaRegistroError((int)E_CATALOGOS.USUARIO, "Modifica", e.Message, "UsuarioController");
                Mensaje om = new Mensaje();
                om.Estatus = -1;
                om.RespuestaCorta = "ERROR";
                om.Detalles = e.Message;
                return JsonConvert.SerializeObject(om, Formatting.None);
            }
        }

        /// <summary>
        /// Baja logica del usuario 
        /// </summary>
        /// <param name="IDUSUARIO">ID del usuario a dar de baja lógica</param>
        /// <returns>Bandera si se realizo el cambio o no</returns>
        public static string BajaLogicaFisica(int IDUSUARIO, int iOpcion)
        {
            Mensaje om = new Mensaje();
            try
            {
                if (iOpcion == 0)
                {
                    using (DBEntities context = new DBEntities())
                    {
                        var vuo = context.USUARIO.AsQueryable().FirstOrDefault(au => au.IDUSUARIO == IDUSUARIO);
                        //var ou = context.USUARIO.Where(au => au.IDUSUARIO == IDUSUARIO).FirstOrDefault<USUARIO>();
                        vuo.ESTATUS_C = (int)e_ESTATUS.BAJA;
                        context.SaveChanges();
                        om.Estatus = 1;
                        om.RespuestaCorta = "OK";
                        om.Detalles = IDUSUARIO.ToString();
                        return JsonConvert.SerializeObject(om, Formatting.None);
                    }
                }
                else if (iOpcion == 1)
                {
                    using (DBEntities context = new DBEntities())
                    {
                        var ou = context.USUARIO.AsQueryable().FirstOrDefault(au => au.IDUSUARIO == IDUSUARIO);
                        context.USUARIO.Remove(ou);
                        context.SaveChanges();
                        om.Estatus = 1;
                        om.RespuestaCorta = "OK";
                        om.Detalles = IDUSUARIO.ToString();
                        return JsonConvert.SerializeObject(om, Formatting.None);
                    }
                }

                om.Estatus = 0;
                om.RespuestaCorta = "NO";
                om.Detalles = "Sin resultados ";
                return JsonConvert.SerializeObject(om, Formatting.None);
            }
            catch (Exception ex)
            {
                BitacoraController.CreaRegistroError((int)E_CATALOGOS.USUARIO, MethodBase.GetCurrentMethod().Name, ex.Message, MethodBase.GetCurrentMethod().DeclaringType.Name);
                om.Estatus = -1;
                om.RespuestaCorta = "Error ";
                om.Detalles = string.Concat(ex.Message, ", ", ex.InnerException);
                return JsonConvert.SerializeObject(om, Formatting.None);
            }

        }

        public static string RecuperaAcceso(string sEmailMatriculaUsuario)
        {
            Mensaje om = new Mensaje();
            DBEntities context = new DBEntities();
            string sAdicional = string.Empty;
            USUARIO obUresultado = null;

            try
            {
                // Se busca el usuario con tres posibilidades (email , matricula, usuario)
                // email
                string[] sEmailoUser = sEmailMatriculaUsuario.Split('@');
                om.Estatus = 1;
                om.RespuestaCorta = "OK";
                if (sEmailoUser.Length > 1)
                {
                    USUARIO ob = context.USUARIO.AsQueryable().FirstOrDefault(au => au.EMAIL.ToUpper() == sEmailMatriculaUsuario.ToUpper());
                    if (ob != null)
                    {
                        ob.ESTATUS_C = (int)e_ESTATUS.CONTRASENIASET;
                        om.Detalles = EmailController.ConsultaEmailGenericos((int)EMAIL_GENERICOS.RECUPERACION_ACCESO, 0, ob);
                        return JsonConvert.SerializeObject(om, Formatting.None);
                    }
                }
                // matricula
                int iMatricula;
                bool res = int.TryParse(sEmailMatriculaUsuario, out iMatricula);
                if (res == true)
                {
                    USUARIO obM = context.USUARIO.AsParallel().FirstOrDefault(au => au.MATRICULAESCOLAR == iMatricula.ToString());
                    if (obM != null)
                    {
                        om.Detalles = EmailController.ConsultaEmailGenericos((int)EMAIL_GENERICOS.RECUPERACION_ACCESO, 0, obM);
                        return JsonConvert.SerializeObject(om, Formatting.None);
                    }
                }
                // USUARIO
                obUresultado = context.USUARIO.AsQueryable().FirstOrDefault(au => au.USUARIONAME.ToUpper() == sEmailMatriculaUsuario.ToUpper());
                if (obUresultado != null)
                {
                    om.Detalles = EmailController.ConsultaEmailGenericos((int)EMAIL_GENERICOS.RECUPERACION_ACCESO, 0, obUresultado);
                    return JsonConvert.SerializeObject(om, Formatting.None);
                }
                om.Estatus = 0;
                om.RespuestaCorta = "NO";
                om.Detalles = "Verificar información proporcionada para recuperación de contraseña";
                return JsonConvert.SerializeObject(om, Formatting.None);
            }
            catch (Exception ex)
            {
                BitacoraController.CreaRegistroError((int)E_CATALOGOS.USUARIO, MethodBase.GetCurrentMethod().Name, ex.Message, MethodBase.GetCurrentMethod().DeclaringType.Name);
                om.Estatus = -1;
                om.RespuestaCorta = "ERROR";
                om.Detalles = string.Concat(ex.Message, ", ", ex.InnerException);
                return JsonConvert.SerializeObject(om, Formatting.None);
            }
        }
    }
}