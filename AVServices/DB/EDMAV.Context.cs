﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Core.Objects;
    using System.Linq;
    
    public partial class DBEntities : DbContext
    {
        public DBEntities()
            : base("name=DBEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ASIGNATURA> ASIGNATURA { get; set; }
        public virtual DbSet<CATALOGO> CATALOGO { get; set; }
        public virtual DbSet<CATALOGODETALLE> CATALOGODETALLE { get; set; }
        public virtual DbSet<GRUPO> GRUPO { get; set; }
        public virtual DbSet<GRUPODETALLE> GRUPODETALLE { get; set; }
        public virtual DbSet<HORARIO> HORARIO { get; set; }
        public virtual DbSet<LOGMSG> LOGMSG { get; set; }
        public virtual DbSet<SALON> SALON { get; set; }
        public virtual DbSet<USUARIO> USUARIO { get; set; }
    
        public virtual int pa_AsignaturaInserta()
        {
            return ((IObjectContextAdapter)this).ObjectContext.ExecuteFunction("pa_AsignaturaInserta");
        }
    }
}