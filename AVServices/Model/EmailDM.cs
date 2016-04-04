using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AVServices.Model
{
    public class EmailDM
    {
        public int ID { get; set; }
        public string sAsunto { get; set; }
        public string sMensaje { get; set; }
        public bool bEnableSsl { get; set; }
        public bool bUseDefaultCredentials { get; set; }
        public string sEmisor { get; set; }
        public string sReceptor { get; set; }
        public string sImagenes { get; set; }
        public DateTime dFechaCreacion { get; set; }
        public DateTime dFechaModificacion { get; set; }
        public bool bEstatus { get; set; }
        public List<string> listaContactos { get; set; }
        public List<string> listaAdjuntos { get; set; }
        public string sContrasenia { get; set; }
        public string sPuertoSMTP { get; set; }
        public int iPuerto { get; set; }
    }
}