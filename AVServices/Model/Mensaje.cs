using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AVServices.Model
{
    /// <summary>
    /// Mensaje respuesta
    /// </summary>
	public class Mensaje
	{
        
        public int Estatus { get; set; }
        
        public string RespuestaCorta { get; set; }
        
        public string Detalles { get; set; }
    }
}