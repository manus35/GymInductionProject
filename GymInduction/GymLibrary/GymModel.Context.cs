﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GymLibrary
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class GymDbEntities : DbContext
    {
        public GymDbEntities(String connectionString)
            : base(connectionString)
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AccessLevel> AccessLevels { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Evaluation> Evaluations { get; set; }
        public virtual DbSet<Induction> Inductions { get; set; }
        public virtual DbSet<InductionStatu> InductionStatus { get; set; }
        public virtual DbSet<Instructor> Instructors { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<User> Users { get; set; }
    }
}
