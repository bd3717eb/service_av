//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AVServices.DB
{
    using System;
    using System.Collections.Generic;
    
    public partial class CATALOGODETALLE
    {
        public int CATALOGODETALLEID { get; set; }
        public int CATALOGOID { get; set; }
        public int CATALOGODETALLEIDCLAVE { get; set; }
        public string CATALOGODETALLECLAVE { get; set; }
        public string CATALOGODETALLEDESCRIPCION { get; set; }
        public string CREADOR { get; set; }
        public bool CATALOGODETALLEACTIVO { get; set; }
        public System.DateTime DATECREATION { get; set; }
        public System.DateTime DATEUPDATE { get; set; }
        public string D0 { get; set; }
        public string D1 { get; set; }
        public string D2 { get; set; }
        public string D3 { get; set; }
    
        public virtual CATALOGO CATALOGO { get; set; }
    }
}
