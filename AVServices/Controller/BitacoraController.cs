using System;
using System.Linq;
using AVServices.DB;
using AVServices.Model;
namespace AVServices.Controller
{
    public class BitacoraController
    {
        public static string CreaRegistroError(int IDCatalogo, string sTitulo, string sDetalle, string sResponsable = "MASTER")
        {
            int iMax = 0;
            try
            {
                using (DBEntities context = new DBEntities())
                {
                    CATALOGODETALLE cdo = new CATALOGODETALLE();
                    cdo.CATALOGOID = (int)E_CATALOGOS.LOG;
                    cdo.CATALOGODETALLEIDCLAVE = iMax;
                    cdo.CATALOGODETALLECLAVE = sTitulo;
                    cdo.CATALOGODETALLEDESCRIPCION = sDetalle;
                    cdo.CREADOR = IDCatalogo.ToString();
                    cdo.CATALOGODETALLEACTIVO = true;
                    cdo.DATECREATION = DateTime.Now;
                    cdo.DATEUPDATE = DateTime.Now;
                    cdo.D0 = string.Empty;
                    cdo.D1 = string.Empty;
                    cdo.D2 = string.Empty;
                    cdo.D3 = string.Empty;
                    context.CATALOGODETALLE.Add(cdo);
                    context.SaveChanges();
                }
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                Console.WriteLine(ex.Message);
            }

            return "OK";
        }
        public static int CreaRegistroLOG(string sDetalleCorto, string sDetalleCompleto, string sCreador, bool bEstatus)
        {
            using (DBEntities context = new DBEntities())
            {
                CATALOGODETALLE cdo = new CATALOGODETALLE();
                cdo.CATALOGOID = (int)E_CATALOGOS.LOG;
                cdo.CATALOGODETALLEIDCLAVE = Configuracion.ConsultaConsecutivoCatalogodeCatalogos((int)E_CATALOGOS.LOG);
                cdo.CATALOGODETALLECLAVE = sDetalleCorto;
                cdo.CATALOGODETALLEDESCRIPCION = sDetalleCompleto;
                cdo.CREADOR = sCreador;
                cdo.CATALOGODETALLEACTIVO = bEstatus;
                cdo.DATECREATION = DateTime.Now;
                cdo.DATEUPDATE = DateTime.Now;
                cdo.D0 = string.Empty;
                cdo.D1 = string.Empty;
                cdo.D2 = string.Empty;
                cdo.D3 = string.Empty;
                context.CATALOGODETALLE.Add(cdo);
                context.SaveChanges();
                return cdo.CATALOGODETALLEID;
            }

        }
        public static bool ModificaRegistroLOG(int ID, string sDetalleCorto, string sDetalleCompleto, string sCreador, bool bEstatus)
        {
            using (DBEntities context = new DBEntities())
            {
                CATALOGODETALLE cdBusca = context.CATALOGODETALLE.AsQueryable().FirstOrDefault(xc => xc.CATALOGOID == (int)E_CATALOGOS.LOG && xc.CATALOGODETALLEID == ID);
                //IQueryable<CATALOGODETALLE> cdBusca = from xcd in context.CATALOGODETALLE where xcd.CATALOGODETALLEID == (int)enumCATALOGOS.LOG && xcd.CATALOGODETALLEID == ID select xcd;
                //var cdBusca = (from xcd in context.CATALOGODETALLE where xcd.CATALOGOID == (int)enumCATALOGOS.LOG && xcd.CATALOGODETALLEID == ID select xcd).FirstOrDefault();
                // context.CATALOGODETALLE.AsQueryable().FirstOrDefault(xc => xc.CATALOGODETALLEID == (int)enumCATALOGOS.LOG && xc.CATALOGODETALLEID == ID);
                cdBusca.CATALOGODETALLECLAVE = sDetalleCorto;
                cdBusca.CATALOGODETALLEDESCRIPCION = sDetalleCompleto;
                cdBusca.CREADOR = sCreador;
                cdBusca.CATALOGODETALLEACTIVO = bEstatus;
                cdBusca.DATEUPDATE = DateTime.Now;
                context.SaveChanges();
            }
            return true;
        }

        public static void Pruebas()
        {
            using (DBEntities context = new DBEntities())
            {
                IQueryable<CATALOGODETALLE> listaResultadoCP = context.CATALOGODETALLE.AsQueryable().Where(acd => acd.CATALOGOID == (int)E_CATALOGOS.CODIGOPOSTAL).OrderBy(xcd => xcd.CATALOGODETALLECLAVE);
                int indice = 0;
                foreach (CATALOGODETALLE item in listaResultadoCP.ToList())
                {
                    CATALOGODETALLE cd = new CATALOGODETALLE();
                    cd = context.CATALOGODETALLE.Single(xcd => xcd.CATALOGODETALLEID == item.CATALOGODETALLEID && xcd.CATALOGOID == (int)E_CATALOGOS.CODIGOPOSTAL);
                    cd.CATALOGODETALLEIDCLAVE = indice;
                    cd.DATEUPDATE = DateTime.Now;
                    context.SaveChanges();
                    indice++;
                }

            }
        }
    }
}