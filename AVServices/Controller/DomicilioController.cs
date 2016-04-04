using AVServices.DB;
using AVServices.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace AVServices.Controller
{
    public class DomicilioController
    {
        public string ConsultaDetallesxCP(string sCP)
        {
            List<CodigoPostalDM> lr = new List<CodigoPostalDM>();
            if (string.IsNullOrEmpty(sCP))
            {
                Mensaje om = new Mensaje();
                om.Estatus = 0;
                om.RespuestaCorta = "NO";
                om.Detalles = "Sin resultados ";
                return JsonConvert.SerializeObject(om, Formatting.None);
            }
            try
            {
                using (DBEntities context = new DBEntities())
                {
                    context.Configuration.LazyLoadingEnabled = false;
                    IQueryable<CATALOGODETALLE> listaResultadoCP = context.CATALOGODETALLE.AsQueryable().Where(acd => acd.CATALOGOID == (int)E_CATALOGOS.CODIGOPOSTAL && acd.CATALOGODETALLECLAVE == sCP);

                    foreach (var item in listaResultadoCP)
                    {
                        CodigoPostalDM cpo = new CodigoPostalDM();
                        cpo.cp = item.CATALOGODETALLECLAVE;
                        cpo.d_asenta_colonia = item.CATALOGODETALLEDESCRIPCION;
                        cpo.d_tipo_asenta = item.CREADOR;
                        cpo.activo = item.CATALOGODETALLEACTIVO;
                        cpo.fecha_consulta = item.DATECREATION;
                        cpo.D_mnpio = item.D0;
                        cpo.d_estado = item.D1;
                        cpo.d_ciudad = item.D2;
                        cpo.d_zona = item.D3;
                        lr.Add(cpo);
                    }

                    Mensaje om = new Mensaje();
                    om.Estatus = 1;
                    om.RespuestaCorta = "OK";
                    om.Detalles = JsonConvert.SerializeObject(lr.ToArray());
                    return JsonConvert.SerializeObject(om, Formatting.None);
                }
            }
            catch (Exception ex)
            {
                BitacoraController.CreaRegistroError((int)E_CATALOGOS.CODIGOPOSTAL, MethodBase.GetCurrentMethod().Name, ex.Message, MethodBase.GetCurrentMethod().DeclaringType.Name);
                Mensaje om = new Mensaje();
                om.Estatus = -1;
                om.RespuestaCorta = "ERROR";
                om.Detalles = string.Concat("al consultar eventos", ex.Message, ", ", ex.InnerException);
                return JsonConvert.SerializeObject(om, Formatting.None);
            }
        }

        public List<CodigoPostalDM> CPGetdetails(string sCP)
        {
            List<CodigoPostalDM> lc = new List<CodigoPostalDM>();
            using (DBEntities context = new DBEntities())
            {
                context.Configuration.LazyLoadingEnabled = false;
                IQueryable<CATALOGODETALLE> listaResultadoCP = context.CATALOGODETALLE.AsQueryable().Where(acd => acd.CATALOGOID == (int)E_CATALOGOS.CODIGOPOSTAL && acd.CATALOGODETALLECLAVE == sCP);

                foreach (var item in listaResultadoCP)
                {
                    CodigoPostalDM cpo = new CodigoPostalDM();
                    cpo.cp = item.CATALOGODETALLECLAVE;
                    cpo.d_asenta_colonia = item.CATALOGODETALLEDESCRIPCION;
                    cpo.d_tipo_asenta = item.CREADOR;
                    cpo.activo = item.CATALOGODETALLEACTIVO;
                    cpo.fecha_consulta = item.DATECREATION;
                    cpo.D_mnpio = item.D0;
                    cpo.d_estado = item.D1;
                    cpo.d_ciudad = item.D2;
                    cpo.d_zona = item.D3;
                    lc.Add(cpo);
                }

                return lc;
            }
        }

    }
}
